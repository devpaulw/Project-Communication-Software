using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace PCS
{
    public class PcsFtpClient
    {
        private const string MessagePath = "./messages/";
        private const string ResourcePath = "./resources/";

        private readonly SdnFtpClient ftpClient;

        public PcsFtpClient(IPAddress ip)
        {
            ftpClient = new SdnFtpClient(ip, PcsFtpServer.Port);
        }

        public void SaveMessage(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            if (!ftpClient.DirectoryExists(MessagePath)) ftpClient.MakeDirectory(MessagePath); 

            string path = GetPathFromDate(message.DateTime);

            using (var appendStream = ftpClient.GetAppendStream(path))
            using (var writer = new StreamWriter(appendStream, PcsServer.Encoding))
            {
                var messagePacket = DataPacket.FromMessage(message);
                writer.WriteLine(messagePacket);
            }
        }

        public IEnumerable<Message> GetDailyMessages(DateTime day)
        {
            string path = GetPathFromDate(day);

            if (!ftpClient.DirectoryExists(MessagePath)) ftpClient.MakeDirectory(MessagePath);
            if (!ftpClient.FileExists(path)) ftpClient.CreateFile(path);

            using (var downloadStream = ftpClient.GetDownloadStream(path))
            using (var reader = new StreamReader(downloadStream, PcsServer.Encoding))
            {
                var textMessages = GetTextMessages(reader.ReadToEnd());

                foreach (string textMessage in textMessages)
                {
                    var dataPacket = new DataPacket(textMessage, DataPacketType.ServerMessage);

                    yield return dataPacket.GetMessage();
                }
            }

            IEnumerable<string> GetTextMessages(string fullText)
            {
                string currentTextMsg = string.Empty;
                bool readingTextMsg = false;

                foreach (char c in fullText)
                {
                    if (c == Flags.StartOfText)
                        readingTextMsg = true;
                    else if (c == Flags.EndOfText)
                    {
                        readingTextMsg = false;
                        yield return currentTextMsg;
                        currentTextMsg = string.Empty;
                    }

                    if (readingTextMsg)
                        currentTextMsg += c;
                }
            }
        }

        public void UploadResource(string localFilePath, out Uri generatedUri)
        {
            string generatedFileName = Path.GetFileName(localFilePath); // DOLATER: possible bug when two images have the same name!
            string generatedFilePath = Path.Combine(ResourcePath, generatedFileName);
            generatedUri = new Uri($"ftp://{ftpClient.IP.MapToIPv4()}:{PcsFtpServer.Port}/{generatedFilePath}");

            ftpClient.UploadFile(localFilePath, generatedFilePath);
        }

        private static string GetPathFromDate(DateTime date)
        {
            string path = string.Empty;
            string extension = ".txt";

            path += MessagePath;
            path += date.Year;
            path += date.Month.ToString(CultureInfo.CurrentCulture).PadLeft(2, '0');
            path += date.Day.ToString(CultureInfo.CurrentCulture).PadLeft(2, '0');
            path += extension;

            return path;
        }
    }
}
