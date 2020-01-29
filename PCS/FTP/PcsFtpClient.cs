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

            if (!PathExists(MessagePath)) MakeDirectory(MessagePath); 

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
            string path = GetPathFromDate(day);

            if (!PathExists(MessagePath)) MakeDirectory(MessagePath);
            if (!FileExists(path)) CreateFile(path);

            var request = WebRequest.Create($"ftp://{m_ip.MapToIPv4()}:{PcsFtpServer.Port}/{path}") as FtpWebRequest;
            request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs");
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            var response = request.GetResponse() as FtpWebResponse;

            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream, PcsServer.Encoding))
            {
                //string[] lines = reader.ReadToEnd().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries); 

                //foreach (string line in lines)
                //{
                //    var dataPacket = new DataPacket(line, DataPacketType.ServerMessage);

                //    yield return dataPacket.GetMessage();
                //}

                while (true)
                {
                    string readLine = reader.ReadLine();

                    if (readLine == null)
                        break;

                    var dataPacket = new DataPacket(readLine, DataPacketType.ServerMessage);

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

        private void CreateFile(string path)
        {
            var request = WebRequest.Create($"ftp://{m_ip.MapToIPv4()}:{PcsFtpServer.Port}/{path}") as FtpWebRequest;
            request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs");
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.GetRequestStream().Close();
            request.GetResponse().Close();
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

        private bool FileExists(string path)
        {
            var request = WebRequest.Create($"ftp://{m_ip.MapToIPv4()}:{PcsFtpServer.Port}/{path}") as FtpWebRequest;
            request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs");
            request.Method = WebRequestMethods.Ftp.GetFileSize;

            try
            {
                request.GetResponse();

                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }
    }
}
