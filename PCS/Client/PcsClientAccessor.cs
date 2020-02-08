using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PCS
{
    public class PcsClientAccessor : PcsClient
    {
        private Thread serverListenThread;
        private PcsFtpClient ftp;

        public string ReceiveStorePath { get; }
        public bool IsConnected { get; private set; }

        public PcsClientAccessor(string receiveStorePath)
        {
            ReceiveStorePath = receiveStorePath ?? throw new ArgumentNullException(nameof(receiveStorePath));
        }

        public void Connect(IPAddress ip, Member member)
        {
            if (ip == null) throw new ArgumentNullException(nameof(ip));

            AdapteeSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var endPoint = new IPEndPoint(ip, PcsServer.Port);

            AdapteeSocket.Connect(endPoint);
            Console.WriteLine(Messages.Client.Connected, ip.MapToIPv4());

            ftp = new PcsFtpClient(ip);

            SignIn();

            void SignIn()
            {
                if (member == null)
                    throw new ArgumentNullException(nameof(member));

                Send(Flags.ClientSignIn + DataPacket.FromMember(member));

                IsConnected = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageReceived">
        /// Receive a message with filled LocalFilePath for possible attached resources.
        /// </param>
        public void StartListenAsync(Action<Message> messageReceived)
        {
            if (!IsConnected)
                throw new Exception(Messages.Exceptions.NotConnected);

            serverListenThread = new Thread(() => Listen());
            serverListenThread.Start();

            void Listen()
            {
                while (true)
                {
                    string receivedData = Receive();

                    var dataPacket = new DataPacket(receivedData);

                    var gotMessage = dataPacket.GetMessage();

                    if (dataPacket.Type != DataPacketType.ServerMessage)
                        throw new DataPacketException(Messages.Exceptions.NotRecognizedDataPacket);

                    gotMessage = GetLocalResources(gotMessage); // DOLATER: Handle better save messages on the PC, not just resources

                    messageReceived(gotMessage);
                }
            }
        }

        public void SendMessage(Message message)
        {
            if (!IsConnected)
                throw new Exception(Messages.Exceptions.NotConnected);

            if (message == null) throw new ArgumentNullException(nameof(message));

            if (!message.HasNoResource)
            {
                for (int i = 0; i < message.AttachedResources.Count; i++) // Transition of local path to ftp path
                {
                    ftp.UploadResource(message.AttachedResources[i].LocalPath, out string generatedFileName);
                    message.AttachedResources[i].RemoteFileName = generatedFileName;
                }
            }

            Send(Flags.ClientMessage + DataPacket.FromMessage(message));
        }

        public IEnumerable<Message> GetDailyMessages(string channelName, DateTime day)
        {
            var dailyMessages = new List<Message>(ftp.GetDailyMessages(channelName, day));

            for (int i = 0; i < dailyMessages.Count; i++)
                dailyMessages[i] = GetLocalResources(dailyMessages[i]); // TODO Not clean

            return dailyMessages;
        }

        public override void Disconnect()
        {
            if (IsConnected)
            {
                Send(Flags.ClientDisconnect.ToString(CultureInfo.CurrentCulture));

                serverListenThread.Abort(); // Stop listen server

                IsConnected = false;

                base.Disconnect();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && IsConnected)
            {
                base.Dispose(disposing);
            }
        }

        private Message GetLocalResources(Message message)
        {
            if (!message.HasNoResource)
            {
                for (int i = 0; i < message.AttachedResources.Count; i++)
                {
                    string localPath = Path.Combine(ReceiveStorePath, message.AttachedResources[i].RemoteFileName);
                    
                    if (!File.Exists(localPath))
                        ftp.DownloadResource(message.AttachedResources[i].RemoteFileName, localPath);
                    
                    message.AttachedResources[i].LocalPath = localPath;
                }
            }

            return message;
        }
    }
}
