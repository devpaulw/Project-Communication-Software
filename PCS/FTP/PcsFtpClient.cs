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

        public void SaveMessage(BroadcastMessage broadcastMsg)
        {
            if (broadcastMsg == null)
                throw new ArgumentNullException(nameof(broadcastMsg));

            string remotePath = GetMessagePath(broadcastMsg.Message.ChannelName, broadcastMsg.DateTime);

            CreateMissingDirectories(remotePath, true);

            using (var appendStream = ftpClient.OpenAppend(remotePath))
            using (var writer = new StreamWriter(appendStream, PcsServer.Encoding))
            {
                string fileMessage = (char)2 + broadcastMsg.ToFileMessage() + (char)3; // TODO Do something for these variables
                writer.WriteLine(fileMessage);
            }
        }

        public IEnumerable<BroadcastMessage> GetDailyMessages(string channelName, DateTime day)
        {
            string remotePath = GetMessagePath(channelName, day);

            CreateMissingDirectories(remotePath, true);

            if (!ftpClient.FileExists(remotePath))
                yield break;

            ftpClient.Download(out byte[] buffer, remotePath);

            string fileContent = PcsServer.Encoding.GetString(buffer, 0, buffer.Length);

            var fileMessages = GetTextMessages(fileContent);

            foreach (string fileMessage in fileMessages)
            {
                yield return BroadcastMessage.FromFileMessage(fileMessage);
            }

            IEnumerable<string> GetTextMessages(string fullText)
            {
                string currentTextMsg = string.Empty;
                bool readingTextMsg = false;

                foreach (char c in fullText)
                {
                    if (c == (char)2)
                        readingTextMsg = true;
                    else if (c == (char)3)
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

        private static string GetMessagePath(string channelName, DateTime date)
        {
            string extension = ".txt";
            string dayFormat = "yyyyMMdd";

            return Path.Combine(
                PcsFtpServer.MessagePath,
                channelName,
                date.ToString(dayFormat, CultureInfo.InvariantCulture) + extension);
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
