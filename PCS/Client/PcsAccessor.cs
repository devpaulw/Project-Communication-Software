using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
//using PCS.Sql;

namespace PCS
{
    public class PcsAccessor : PcsClient
    {
        private Thread serverListenThread;
        private PcsFtpClient ftp; // TODO: Should be general data class and not specific (not-known so)
        //private MessageTable messageTable;

        public event EventHandler<BroadcastMessage> MessageReceive;
        public event EventHandler<ResponseCode> ResponseReceive; // TODO Think about, define the problem and find a solution lol

        public bool IsConnected { get; private set; }

        public PcsAccessor()
        {
            ResponseReceive += OnResponseReceive;
        }

        public void Connect(IPAddress ip, Member member)
        {
            if (ip == null)
                throw new ArgumentNullException(nameof(ip));

            AdapteeClient = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var endPoint = new IPEndPoint(ip, PcsServer.Port);

            AdapteeClient.Connect(endPoint);
            Console.WriteLine(Messages.Client.Connected, ip.MapToIPv4());

            ftp = new PcsFtpClient(ip);
            //messageTable = new MessageTable();

            SignIn();

            StartListenBroadcasts();

            void SignIn()
            {
                if (member == null)
                    throw new ArgumentNullException(nameof(member));

                SendPacket(new SignInPacket(member));

                if (ReceivePacket() is ResponsePacket responsePacket)// DOLATER: Is that a good way?
                { 
                    switch (responsePacket.ResponseCode)
                    {
                        case ResponseCode.SignInSucceeded: // TODO Use OnReceive and just enable isConnected when receive this packet!
                            IsConnected = true;
                            break;
                        case ResponseCode.UnauthorizedLogin: // TODO Maybe add event caller in the true client that get these...
                            throw new Exception(Messages.Exceptions.UnauthorizedLogin);
                        default:
                            throw new Exception(Messages.Exceptions.NotRecognizedDataPacket);
                    }
                }
                else
                    throw new Exception(Messages.Exceptions.NotRecognizedDataPacket);
            }
        }

        public void SendMessage(Message message)
        {
            if (!IsConnected)
                throw new Exception(Messages.Exceptions.NotConnected);

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            SendPacket(new MessagePacket(message));
        }

        public IEnumerable<BroadcastMessage> GetDailyMessages(string channelName, DateTime day) // TODO Change system
        {
            var dailyMessages = new List<BroadcastMessage>(ftp.GetDailyMessages(channelName, day));

            return dailyMessages;
        }

        public override void Disconnect()
        {
            if (IsConnected)
            {
                SendPacket(new DisconnectPacket());

                IsConnected = false;

                base.Disconnect();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsConnected)
                return;

            if (!disposedValue && disposing)
            {
                ftp.Dispose();
                MessageReceive = null;

                base.Dispose(disposing);
            }
        }

        private void OnResponseReceive(object sender, ResponseCode responseCode)
        {

        }

        private void StartListenBroadcasts() // TODO Listen better handle with Error Handle espacially
        {
            if (!IsConnected)
                throw new Exception(Messages.Exceptions.NotConnected);

            serverListenThread = new Thread(() => Listen());
            serverListenThread.Start();

            void Listen()
            {
                while (IsConnected) // UNDONE
                {
                    try
                    {
                        Packet receivedPacket = ReceivePacket();

                        if (receivedPacket is BroadcastMessagePacket broadcastMessagePacket)
                            MessageReceive(this, broadcastMessagePacket.BroadcastMessage);
                        else
                            throw new Exception(Messages.Exceptions.NotRecognizedDataPacket); // DOLATER: Handle better save messages on the PC, not just resources

                    }
                    catch (SocketException)
                    {
                        if (IsConnected)
                            throw;
                    }
                }
            }
        }
    }
}
