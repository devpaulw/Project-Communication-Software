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

            string path = GetPathFromDate(message.DateTime);

            var request = WebRequest.Create($"ftp://{m_ip.MapToIPv4()}:{PcsFtpServer.Port}/{path}") as FtpWebRequest;
            request.Credentials = new NetworkCredential("anonymous", "pcs@pcs.pcs"); // DOLATER: Find cleaner, use passwords, and SSL
            request.Method = WebRequestMethods.Ftp.AppendFile;

            using (var requestStream = new StreamWriter(request.GetRequestStream(), PcsServer.Encoding))
            {
                requestStream.WriteLine(DataPacket.FromMessage(message, true));
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
            using (var reader = new StreamReader(responseStream))
            {
                for (int i = 0; i < 2; i++) // TODO: Use a more reliable index!
                {
                    var dataPacket = new DataPacket(Flags.ServerMessage + Flags.EndOfText + reader.ReadLine()); // TODO! In DataPacket, we should proceed differently, not force flag headers!
                    yield return dataPacket.GetServerMessage(); // TODO: Rename this function Message with withAuthor parameter
                }

                Console.WriteLine($"Download Complete, status {response.StatusDescription}");
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
    }
}
