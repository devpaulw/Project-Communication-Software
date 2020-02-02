﻿using System;
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

        private readonly IPAddress m_ip;

        public PcsFtpClient(IPAddress ip)
        {
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

            using (var requestStream = request.GetRequestStream())
            using (var writer = new StreamWriter(requestStream, PcsServer.Encoding))
            {
                var messagePacket = DataPacket.FromMessage(message);
                writer.WriteLine(messagePacket);
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

        public void UploadResource(string localFilePath, out string generatedFileName)
        {
            if (!PathExists(ResourcePath)) MakeDirectory(MessagePath);
            
            generatedFileName = Path.GetFileName(localFilePath); // DOLATER: possible bug when two images have the same name!
            string generatedFilePath = Path.Combine(ResourcePath, generatedFileName);

            UploadFile(localFilePath, generatedFilePath);
        }

        public void UploadFile(string localFilePath, string path)
        {
            var request = WebRequest.Create($"ftp://{m_ip.MapToIPv4()}:{PcsFtpServer.Port}/{path}") as FtpWebRequest;
            request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs"); // DOLATER: Find cleaner, use passwords, and SSL
            request.Method = WebRequestMethods.Ftp.UploadFile;

            byte[] buffer;

            using (FileStream stream = File.OpenRead(localFilePath))
            {
                buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
            }

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(buffer, 0, buffer.Length);
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
