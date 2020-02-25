using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using PCS.Sql;
using PCS.Data;
using PCS.Data.Packets;

namespace PCS
{
    public class PcsAccessor : PcsClient
    {
        private Thread serverListenThread;
        private MessageTable messageTable; // TODO Do a ask to server instead of SQL direct download BUT I'm not sure it's the right approach

        public event EventHandler<BroadcastMessage> MessageReceive;
        public event EventHandler<ResponseCode> ResponseReceive; // TODO Think about, define the problem and find a solution lol

        public bool IsConnected { get; private set; }
        public int ActiveUserId { get; private set; }

        public PcsAccessor()
        {
            ResponseReceive += OnResponseReceive;
        }

        public void Connect(IPAddress ip, AuthenticationInfos authenticationInfos) // TODO I don't know if this password is secured
        {
            if (ip == null)
                throw new ArgumentNullException(nameof(ip));
            if (authenticationInfos == null)
                throw new ArgumentNullException(nameof(authenticationInfos));

            ActiveUserId = authenticationInfos.MemberId;

            AdapteeClient = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var endPoint = new IPEndPoint(ip, PcsServer.Port);

            AdapteeClient.Connect(endPoint);
            Console.WriteLine(Messages.Client.Connected, ip.MapToIPv4());

            messageTable = new MessageTable();

            SignIn();

            StartListenBroadcasts();

            void SignIn()
            {
                SendPacket(new SignInPacket(authenticationInfos));

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

        public IEnumerable<BroadcastMessage> GetTopMessagesInRange(int start, int end, string channelName)
            => messageTable.GetTopMessagesInRange(start, end, channelName);

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
