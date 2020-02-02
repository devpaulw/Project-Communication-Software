using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PCS
{
    public class PcsClientAccessor : PcsClient
    {
        private Thread serverListenThread;

        public PcsFtpClient Ftp { get; private set; }
        public bool IsConnected { get; private set; }

        public void Connect(IPAddress ip, Member member)
        {
            if (ip == null) throw new ArgumentNullException(nameof(ip));

            AdapteeSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var endPoint = new IPEndPoint(ip, PcsServer.Port);

            try
            {
                AdapteeSocket.Connect(endPoint);
                Console.WriteLine(Messages.Client.Connected, ip.MapToIPv4());
            }
            catch
            {
                throw;
            }

            Ftp = new PcsFtpClient(ip);

            SignIn();

            void SignIn()
            {
                if (member == null) throw new ArgumentNullException(nameof(member));

                Send(Flags.ClientSignIn + DataPacket.FromMember(member));

                IsConnected = true;
            }
        }

        public void StartListenAsync(Action<ServerMessage> messageReceived)
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

                    if (dataPacket.Type == DataPacketType.ServerMessage)
                        messageReceived(dataPacket.GetServerMessage());
                    else
                        throw new DataPacketException(Messages.Exceptions.NotRecognizedDataPacket);
                }
            }
        }

        public void SendMessage(ClientMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrEmpty(message.Text) || string.IsNullOrEmpty(message.ChannelTitle)) 
                throw new MessageEmptyFieldException(Messages.Exceptions.MessageEmptyField);

            Send(Flags.ClientMessage + DataPacket.FromClientMessage(message));
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
    }
}
