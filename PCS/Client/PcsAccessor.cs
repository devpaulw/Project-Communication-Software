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
using System.Threading.Tasks;

namespace PCS
{
    public class PcsAccessor : PcsClient
    {
        private ResponseResetEvent responseEvent;
        private bool listen;

        public event EventHandler<BroadcastMessage> MessageReceive;
        public event EventHandler<Exception> ListenException;

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
            listen = true;
            responseEvent = new ResponseResetEvent();
            ListenException += OnListenException;

            StartListenServer();
            #endregion

            SignIn();

            void SignIn()
            {
                Response response = SendRequest(new SignInRequest(authenticationInfos));

                if (response.Succeeded)
                    IsConnected = true;
                else
                    throw new Exception(Messages.Exceptions.UnauthorizedLogin);
            }
        }

        public void SendMessage(SendableMessage message)
        {
            if (!IsConnected)
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
                foreach (var broadcast in response.BroadcastMessages)
                    yield return broadcast;
            }
        }

        public override void Disconnect()
        {
            MessageReceive = null;
            ListenException = null;

            if (IsConnected)
            {
                SendPacket(new DisconnectPacket());
            }

            if (listen)
            {
                base.Disconnect();
                listen = false;
            }

        }

        private void StartListenServer()
        {
            var serverListenTask = Task.Run(() => Listen());

            try
            {
                serverListenTask.Wait();
            }
            catch (PcsTransmissionException ex)
            {
                ListenException(this, ex);
            }
            catch (SocketException ex)
            {
                ListenException(this, ex);
            }

            async void Listen()
            {
                while (listen)
                {
                    try
                    {
                        Packet receivedPacket = await Task.Run(() => ReceivePacket()).ConfigureAwait(false);

                        switch (receivedPacket)
                        {
                            case BroadcastMessagePacket broadcastMessagePacket when IsConnected:
                                MessageReceive(this, broadcastMessagePacket.Item);
                                break;
                            case ResponsePacket responsePacket:
                                responseEvent.SetResponse(responsePacket.Item);
                                break;
                            default:
                                throw new PcsTransmissionException(Messages.Exceptions.NotRecognizedDataPacket); // DOLATER: Handle better save messages on the PC, not just resources
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

            Response response;

            response = responseEvent.WaitResponse(); // TODO Timeout system ?

            if (response.Code == request.Code)
            {
                return response;
            }
            else
                throw new Exception(Messages.Exceptions.NotRecognizedDataPacket);
        }

        private void OnListenException(object sender, Exception exception)
        {
            IsConnected = false;
            Disconnect();
        }
    }
}