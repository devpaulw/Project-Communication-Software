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
        private ResponseResetEvent responseEvent;
        private MessageTable messageTable; // TODO Do a ask to server instead of SQL direct download BUT I'm not sure it's the right approach

        public event EventHandler<BroadcastMessage> MessageReceive;
        public event EventHandler<Response> ResponseReceive; // TODO Think about, define the problem and find a solution lol

        public bool IsSignedIn { get; private set; }
        public int ActiveMemberId { get; private set; }

        public PcsAccessor()
        {
        }

        public void Connect(IPAddress ip, AuthenticationInfos authenticationInfos) // TODO I don't know if this password is secured
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
            ResponseReceive += (object sender, Response response) => responseEvent.SetResponse(response);

            StartListenServer();
            #endregion

            messageTable = new MessageTable(); // TODO SHould not be there

            SignIn();

            void SignIn()
            {
                SendPacket(new SignInPacket(authenticationInfos));

                    // TODO Maybe add event caller in the true client that get these...

                var response = responseEvent.WaitResponse();
                if (response.Code == ResponseCode.SignIn)
                {
                    if (response.Succeeded)
                        IsSignedIn = true;
                    else
                        throw new Exception(Messages.Exceptions.UnauthorizedLogin);
                }
                else
                    throw new Exception(Messages.Exceptions.NotRecognizedDataPacket);
            }
        }

        public void SendMessage(SendableMessage message)
        {
            if (!IsSignedIn)
                throw new Exception(Messages.Exceptions.NotConnected);

            SendPacket(new MessagePacket(message ?? throw new ArgumentNullException(nameof(message))));
        }

        public void DeleteMessage(int messageId)
        {
            SendPacket(new RequestPacket(new DeleteMessageRequest(messageId)));

            var response = responseEvent.WaitResponse();
            if (response.Code == ResponseCode.MessageHandle)
            {
                if (response.Succeeded)
                    System.Diagnostics.Debug.WriteLine("Message delete succeeded");// TODO Error handling here
            }
            else
                throw new Exception(Messages.Exceptions.NotRecognizedDataPacket);
        }

        public void ModifyMessage(int messageId, SendableMessage newMessage)
        {
            SendPacket(
                new RequestPacket(
                    new ModifyMessageRequest(
                        messageId, 
                        newMessage ?? throw new ArgumentNullException(nameof(newMessage))
                        )
                    )
                );

            var response = responseEvent.WaitResponse();
            if (response.Code == ResponseCode.MessageHandle)
            {
                if (response.Succeeded)
                    System.Diagnostics.Debug.WriteLine("Message modification succeeded");// TODO Error handling here
            }
            else
                throw new Exception(Messages.Exceptions.NotRecognizedDataPacket);
        }

        public IEnumerable<BroadcastMessage> GetTopMessagesInRange(int start, int end, string channelName)
            => messageTable.GetTopMessagesInRange(start, end, channelName);

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
                ResponseReceive = null;

                base.Disconnect();
            }
        }

        private void StartListenServer() // TODO Listen better handle with Error Handle espacially
        {
            serverListenThread = new Thread(new ThreadStart(Listen));
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
                                ResponseReceive(this, responsePacket.Item);
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

        //private void StartListenBroadcasts() // TODO Listen better handle with Error Handle espacially
        //{
        //    if (!IsSignedIn)
        //        throw new Exception(Messages.Exceptions.NotConnected);
            
        //    serverListenThread = new Thread(new ThreadStart(Listen));
        //    serverListenThread.Start();

        //    void Listen()
        //    {
        //        while (IsSignedIn) // UNDONE Make it not IsConnected obligation
        //        {
        //            try
        //            {
        //                Packet receivedPacket = ReceivePacket();

        //                if (receivedPacket is BroadcastMessagePacket broadcastMessagePacket)
        //                    MessageReceive(this, broadcastMessagePacket.Item);
        //                else if (receivedPacket is ResponsePacket responsePacket) // TODO Is that really useful ? Is this async function should be only broadcast receive ? maybe if we keep no async wait response
        //                    ResponseReceive(this, responsePacket.Item);
        //                else
        //                    throw new Exception(Messages.Exceptions.NotRecognizedDataPacket); // DOLATER: Handle better save messages on the PC, not just resources
                        
        //            }
        //            catch (SocketException)
        //            {
        //                if (IsSignedIn)
        //                    throw;
        //            }
        //        }
        //    }
        //}
    }
}
