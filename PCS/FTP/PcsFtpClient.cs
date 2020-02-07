using FluentFTP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace PCS
{
    // TODO: This same class but with System.IO copied to the local machine, there will be an interface, and the Client Accessor will save data to same directories of FTP server by copied constants ; The FtpClient of Accessor will be private now
    public class PcsFtpClient : IDisposable
    {
        private readonly FtpClient ftpClient;

        public PcsFtpClient(IPAddress ip)
        {
            if (ip == null)
                throw new ArgumentNullException(nameof(ip));

            ftpClient = new FtpClient
            {
                Host = ip.MapToIPv4().ToString(),
                Port = PcsFtpServer.Port,
                Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs")
            };
            ftpClient.Connect();
        }

        public void SaveMessage(Message message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            string remotePath = GetPathFromDate(message.DateTime);

            CreateMissingDirectories(remotePath, true);

            using (var appendStream = ftpClient.OpenAppend(remotePath))
            using (var writer = new StreamWriter(appendStream, PcsServer.Encoding))
            {
                var messagePacket = DataPacket.FromMessage(message);
                writer.WriteLine(messagePacket);
            }
        }

        public IEnumerable<Message> GetDailyMessages(DateTime day)
        {
            string remotePath = GetPathFromDate(day);

            CreateMissingDirectories(remotePath, true);
            if (!ftpClient.FileExists(remotePath))
                yield break;

            ftpClient.Download(out byte[] buffer, remotePath);

            string fileContent = PcsServer.Encoding.GetString(buffer, 0, buffer.Length);

            var textMessages = GetTextMessages(fileContent);

            foreach (string textMessage in textMessages)
            {
                var dataPacket = new DataPacket(textMessage, DataPacketType.ServerMessage);

                yield return dataPacket.GetMessage();
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

        public void UploadResource(string localFilePath, out string generatedRemoteFileName)
        {
            generatedRemoteFileName = Path.GetFileName(localFilePath); // DOLATER: possible bug when two images have the same name!
            string generatedRemotePath = Path.Combine(PcsFtpServer.ResourcePath, generatedRemoteFileName);

            CreateMissingDirectories(generatedRemotePath, true);

            ftpClient.UploadFile(localFilePath, generatedRemotePath);
        }

        public void DownloadResource(string remoteFileName, string localFilePath)
        {
            ftpClient.DownloadFile(localFilePath,
                Path.Combine(PcsFtpServer.ResourcePath, remoteFileName));
        }

        private void CreateMissingDirectories(string remotePath, bool isFilePath)
        {
            var parentDirs = new List<string>(GetParentDirectories());
            parentDirs.Reverse();

            foreach (var parentDir in parentDirs)
            {
                if (!ftpClient.DirectoryExists(parentDir))
                    ftpClient.CreateDirectory(parentDir);
            }

            IEnumerable<string> GetParentDirectories()
            {
                string directory = remotePath;

                if (!isFilePath)
                    yield return remotePath;

                while (!string.IsNullOrEmpty(directory))
                {
                    directory = Path.GetDirectoryName(directory);

                    if (!string.IsNullOrEmpty(directory))
                        yield return directory;
                }
            }
        }

        private static string GetPathFromDate(DateTime date)
        {
            string path = string.Empty;
            string extension = ".txt";

            path += date.Year;
            path += date.Month.ToString(CultureInfo.CurrentCulture).PadLeft(2, '0');
            path += date.Day.ToString(CultureInfo.CurrentCulture).PadLeft(2, '0');
            path += extension;
            path = Path.Combine(PcsFtpServer.MessagePath, path);

            return path;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ftpClient.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
