﻿using FluentFTP;
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

            string remotePath = GetMessagePath(broadcastMsg.BaseMessage.ChannelName, broadcastMsg.DateTime);

            CreateMissingDirectories(remotePath, true);

            using (var appendStream = ftpClient.OpenAppend(remotePath))
            using (var writer = new StreamWriter(appendStream, PcsServer.Encoding))
            {
                string fileMessage = ControlChars.StartOfText + broadcastMsg.ToFileMessage() + ControlChars.EndOfText; // TODO Do something for these variables
                writer.WriteLine(fileMessage);
            }
        }

#pragma warning disable CA1822 // Mark members as static
        public IEnumerable<Channel> GetChannels()
#pragma warning restore CA1822 // Mark members as static
        {
            // TODO retrieve from folders or from DB ?
            return new List<Channel>()
            {
                ChannelGenerator.Generate("channel1"),
                ChannelGenerator.Generate("channel2"),
                ChannelGenerator.Generate("channel3")
            };
        }

        public IEnumerable<BroadcastMessage> GetDailyMessages(Channel channel, DateTime day)
        {
            if (channel is null)
                throw new ArgumentNullException(nameof(channel));

            string remotePath = GetMessagePath(channel.Name, day);

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
                    if (c == ControlChars.StartOfText)
                        readingTextMsg = true;
                    else if (c == ControlChars.EndOfText)
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
