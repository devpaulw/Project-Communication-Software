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
    public class PcsAccessor : PcsClient
    {
        private Thread serverListenThread;
        private PcsFtpClient ftp;

        public bool IsConnected { get; private set; }

        public PcsAccessor()
        {
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

                SendPacket(new DataPacket(DataPacketType.MemberSignIn, member));

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
                    var receivedPacket = ReceivePacket();

                    if (receivedPacket.Type != DataPacketType.Message)
                        throw new DataPacketException(Messages.Exceptions.NotRecognizedDataPacket); // DOLATER: Handle better save messages on the PC, not just resources

                    var gotMessage = (Message)receivedPacket.Object;

                    messageReceived(gotMessage);
                }
            }
        }

        public void SendMessage(Message message)
        {
            if (!IsConnected)
                throw new Exception(Messages.Exceptions.NotConnected);

            if (message == null) throw new ArgumentNullException(nameof(message));

            SendPacket(new DataPacket(DataPacketType.Message, message));
        }

        public IEnumerable<Message> GetDailyMessages(string channelName, DateTime day)
        {
            var dailyMessages = new List<Message>(ftp.GetDailyMessages(channelName, day));

            return dailyMessages;
        }

        public override void Disconnect()
        {
            if (IsConnected)
            {
                SendPacket(new DataPacket(DataPacketType.ClientDisconnect));

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
    }
}
