using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem.DotNet;
using Microsoft.Extensions.DependencyInjection;
using PCS.Sql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PCS.Data;
using PCS.Data.Packets;

namespace PCS
{
    public class PcsServer : IDisposable
    {
        private bool disposed;

        private readonly PcsListener listener;
        private readonly MessageTable messageTable = new MessageTable();
        private readonly MemberTable group = new MemberTable();
        private readonly PasswordTable memberPasswords = new PasswordTable();
        private readonly List<PcsClient> connectedClients = new List<PcsClient>();
        private readonly object @lock = new object();

        public const ushort Port = 6783;
        public static readonly Encoding Encoding = Encoding.UTF8;

        public PcsServer(IPAddress serverAddress)
        {
            #region Temp Members assign
            try
            {
                group.AddRow(new Member("Paul", 1));
                group.AddRow(new Member("Thomas", 2));
                group.AddRow(new Member("Ilian", 3));
                memberPasswords.AddRow(new AuthenticationInfos(1, "gamestar"));
                memberPasswords.AddRow(new AuthenticationInfos(2, "momocmapassion3"));
                memberPasswords.AddRow(new AuthenticationInfos(1, "iliano"));
            }
            catch { }
            #endregion

            if (serverAddress == null)
                throw new ArgumentNullException(nameof(serverAddress));

            listener = new PcsListener(serverAddress);
        }

        public void StartHosting()
        { // TODO Client could contain member
            try
            {
                listener.Listen();

                Console.WriteLine(Messages.Server.Started, DateTime.Now);

                while (true)
                {
                    var client = listener.Accept();
                    connectedClients.Add(client);

                    var connectionThread = new Thread(() => ManageClientConnection(client));
                    connectionThread.Start();
                }
            }
            catch
            {
                throw;
            }
        }

        private void ManageClientConnection(PcsClient client) // TODO I think it would be better in a well managed class ServerClientManager
        {
            Member signInMember = null; // TODO Maybe put this variable in PcsClient so that it can be used both here and by The accessor
            bool signedIn = false; // TODO Put these  variables in PcsClient
            bool connected = true;

            while (connected)
            {
                try
                {
                    Packet packet = client.ReceivePacket();

                    switch (packet)
                    {
                        case SignInPacket signInPacket when !signedIn:
                            lock (@lock)
                                OnSignIn(signInPacket.Item);
                            break;
                        case MessagePacket messagePacket when signedIn:
                            lock (@lock)
                                OnMessageReceived(messagePacket.Item);
                            break;
                        case DisconnectPacket _ when signedIn:
                            connected = false;
                            break;
                        case RequestPacket requestPacket when signedIn:
                            lock (@lock)
                                OnRequest(requestPacket.Item);
                            break;
                    }
                }
                catch (SocketException) // When An existing connection was forcibly closed by the remote host
                {
                    connected = false;
                }
            }

            lock (@lock)
                OnDisconnect();

            void OnSignIn(AuthenticationInfos infos)
            {
                signInMember = group.GetMemberFromId(infos.MemberId);

                if (CanSignIn())
                {
                    signedIn = true;

                    client.SendPacket(new ResponsePacket(new Response(ResponseCode.SignIn, true)));
                    Console.WriteLine(Messages.Server.ClientConnect, signInMember, client.RemoteIP.Address.ToString());
                }
                else
                {
                    client.SendPacket(new ResponsePacket(new Response(ResponseCode.SignIn, false)));
                    connected = false;
                    signInMember = null; // Because sign in failed
                }

                bool CanSignIn()
                {
                    lock (@lock)
                        return group.MemberExists(infos.MemberId)
                            && memberPasswords.PasswordCorrect(infos);
                }
            }

            void OnMessageReceived(SendableMessage message)
            {
                var broadcastMsg = new BroadcastMessage(messageTable.GetNewID(), message, DateTime.Now, signInMember);
                Console.WriteLine("Received: " + broadcastMsg);

                AddBroadcast(broadcastMsg);
            }

            void OnDisconnect()
            {
                client.Disconnect();
                if (signInMember != null)
                    Console.WriteLine(Messages.Server.ClientDisconnect, signInMember);

                connectedClients.Remove(client);
            }

            void OnRequest(Request request)
            {
                switch (request)
                {
                    case DeleteMessageRequest deleteMessageRequest:
                        { // Remove message
                            messageTable.RemoveRow(deleteMessageRequest.MessageId); // TODO try catch
                        }

                        client.SendPacket(new ResponsePacket(
                            new Response(ResponseCode.MessageHandle, true)));

                        break;
                    case ModifyMessageRequest modifyMessageRequest:
                        { // Modify message
                            var oldBroadcast = messageTable.GetItemAt(modifyMessageRequest.MessageId);
                            var newBroadcast = new BroadcastMessage(modifyMessageRequest.MessageId, modifyMessageRequest.NewMessage, oldBroadcast.DateTime, oldBroadcast.Author);

                            messageTable.RemoveRow(modifyMessageRequest.MessageId);
                            messageTable.AddRow(newBroadcast);
                        }

                        client.SendPacket(new ResponsePacket(
                            new Response(ResponseCode.MessageHandle, true)));

                        break;
                }
            }
        }

        private void AddBroadcast(BroadcastMessage message)
        {
            SendToEveryone();
            SaveMessage();

            void SendToEveryone()
            {
                // Send to all clients
                foreach (var connectedClient in connectedClients)
                    SendToAccessor(connectedClient);

                void SendToAccessor(PcsClient client)
                {
                    if (message == null)
                        throw new ArgumentNullException(nameof(message));

                    client.SendPacket(new BroadcastMessagePacket(message));
                }
            }

            void SaveMessage()
            {
                messageTable.AddRow(message);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    listener.Dispose();
                }

                disposed = true;
            }
        }
    }
}