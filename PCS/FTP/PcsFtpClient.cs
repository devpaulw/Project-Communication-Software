using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;

namespace PCS
{
    class PcsFtpClient
    {
        private const string MessagePath = "./messages/";

        private IPAddress m_ip;

        
        public PcsFtpClient()
        {
            //ShowContent(request);
            //UploadFile(request, @"C:\Users\BluePaul\Documents\tt1.txt");
            //ShowContent(request);
            //DownloadFile(WebRequest.Create(@"ftp://192.168.0.25:6784/tt1.txt") as FtpWebRequest);
        }

        public void Connect(IPAddress ip)
        {
            // There are not a lot because it's not a connect/disconnect format of FTP library, but It could be.
            if (ip == null) throw new ArgumentNullException(nameof(ip));
            else
                m_ip = ip;
        }

        public void SaveMessage(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            if (!PathExists(MessagePath)) 
                MakeDirectory(MessagePath); 

            string path = GetPathFromDate(message.DateTime);

            var request = WebRequest.Create($"ftp://{m_ip.MapToIPv4()}:{PcsFtpServer.Port}/{path}") as FtpWebRequest;
            request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs"); // DOLATER: Find cleaner, use passwords, and SSL
            request.Method = WebRequestMethods.Ftp.AppendFile;

            var messagePacket = PcsServer.Encoding.GetBytes(DataPacket.FromMessage(message));

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(messagePacket, 0, messagePacket.Length);
                requestStream.WriteByte((byte)'\n');
            }
        }

        public IEnumerable<Message> GetDailyMessages(DateTime day)
        {
            string path = GetPathFromDate(day); // TODO: Handle if the file doesn't exist

            var request = WebRequest.Create($"ftp://{m_ip.MapToIPv4()}:{PcsFtpServer.Port}/{path}") as FtpWebRequest;
            request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs");
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            var response = request.GetResponse() as FtpWebResponse;

            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream, PcsServer.Encoding))
            {
                string[] lines = reader.ReadToEnd().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries); // TODO // BUG: Doesn't work for message with many lines YET! Find a new way!

                foreach (string line in lines)
                {
                    var dataPacket = new DataPacket(line, DataPacketType.ServerMessage);

                    yield return dataPacket.GetMessage();
                }
            }
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

        private void MakeDirectory(string path)
        {
            var request = WebRequest.Create($"ftp://{m_ip.MapToIPv4()}:{PcsFtpServer.Port}/{path}") as FtpWebRequest;
            request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs");
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.GetResponse();
        }

        private bool PathExists(string path)
        {
            var request = WebRequest.Create($"ftp://{m_ip.MapToIPv4()}:{PcsFtpServer.Port}/{path}") as FtpWebRequest;
            request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs");
            request.Method = WebRequestMethods.Ftp.ListDirectory;

            try
            {
                request.GetResponse();

                return true; // The file exists because there are not errors when wishing get infos of it
            }
            catch (WebException)
            {
                return false;
            }
        }
    }
}
