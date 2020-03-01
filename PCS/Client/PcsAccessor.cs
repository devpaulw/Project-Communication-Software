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
        private ResponseResetEvent responseEvent;

        public event EventHandler<BroadcastMessage> MessageReceive;

        public bool IsSignedIn { get; private set; }
        public int ActiveMemberId { get; private set; }

        public PcsAccessor()
        {
        }

        public void Connect(IPAddress ip, AuthenticationInfos authenticationInfos) // DOLATER I don't know if this password is secured
        {
            ActiveMemberId = (authenticationInfos ?? throw new ArgumentNullException(nameof(authenticationInfos))).MemberId;

            #region Connect to the network
            AdapteeClient = new Socket((ip ?? throw new ArgumentNullException(nameof(ip))).AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var endPoint = new IPEndPoint(ip, PcsServer.Port);
            AdapteeClient.Connect(endPoint);
            Console.WriteLine(Messages.Client.Connected, ip.MapToIPv4());
            #endregion

            #region Init Listen
            IsConnected = true;
            responseEvent = new ResponseResetEvent();

            StartListenServer();
            #endregion

            SignIn();

            void SignIn()
            {
                Response response = SendRequest(new SignInRequest(authenticationInfos));

                if (response.Succeeded)
                    IsSignedIn = true;
                else
                    throw new Exception(Messages.Exceptions.UnauthorizedLogin);
            }
        }

        public void SendMessage(SendableMessage message)
        {
            if (!IsSignedIn)
                throw new Exception(Messages.Exceptions.NotConnected);

            SendPacket(new SendableMessagePacket(message ?? throw new ArgumentNullException(nameof(message))));
        }

        public void DeleteMessage(int messageId)
        {
            Response response = SendRequest(new DeleteMessageRequest(messageId));
            if (response.Succeeded)
                System.Diagnostics.Debug.WriteLine("Message delete succeeded");
            else
                throw new Exception(Messages.Exceptions.UnauthorizedHandleMessage);
        }

        public void ModifyMessage(int messageId, SendableMessage newMessage)
        {
            Response response = SendRequest(new ModifyMessageRequest(
                        messageId,
                        newMessage ?? throw new ArgumentNullException(nameof(newMessage))
                        ));

            if (response.Succeeded)
                System.Diagnostics.Debug.WriteLine("Message modification succeeded");
            else
                throw new Exception(Messages.Exceptions.UnauthorizedHandleMessage);
        }

        public IEnumerable<BroadcastMessage> GetTopMessagesInRange(int start, int end, string channelName)
        {
            var response = SendRequest(new BroadcastDeliveryRequest(start, end, channelName)) as BroadcastDeliveryResponse;

            if (response.Succeeded)
            {
                foreach(var broadcast in response.BroadcastMessages)
                    yield return broadcast;
            }
        }

        public override void Disconnect()
        {
            if (IsSignedIn)
            {
                SendPacket(new DisconnectPacket());

                IsSignedIn = false; // TODO Put it in client directly so that it can be used by server too
            }

            if (IsConnected)
            {
                MessageReceive = null;

                base.Disconnect();
            }
        }

        private void StartListenServer() // TODO Listen better handle with Error Handle espacially
        {
            Thread serverListenThread = new Thread(new ThreadStart(Listen));
            serverListenThread.Start();

            void Listen()
            {
                while (IsConnected)
                {
                    try
                    {
                        Packet receivedPacket = ReceivePacket();

                        switch (receivedPacket)
                        {
                            case BroadcastMessagePacket broadcastMessagePacket when IsSignedIn:
                                MessageReceive(this, broadcastMessagePacket.Item);
                                break;
                            case ResponsePacket responsePacket:
                                responseEvent.SetResponse(responsePacket.Item);
                                break;
                            default:
                                throw new Exception(Messages.Exceptions.NotRecognizedDataPacket); // DOLATER: Handle better save messages on the PC, not just resources
                        }
                    }
                    catch (SocketException)
                    {
                        if (IsConnected)
                            throw;
                    }
                }
            }
        }

        private Response SendRequest(Request request)
        {
            SendPacket(new RequestPacket(request));

            var response = responseEvent.WaitResponse(); // TODO Timeout system ?
            if (response.Code == request.Code)
            {
                return response;
            }
            else
                throw new Exception(Messages.Exceptions.NotRecognizedDataPacket);
        }
    }
}