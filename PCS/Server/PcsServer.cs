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
        
        private void ManageClientConnection(PcsClient client)
        {
            Member signInMember = null; // TODO Maybe put this variable in PcsClient so that it can be used both here and by The accessor
            bool signedIn = false;
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
                                OnSignIn(signInPacket.AuthenticationInfos);
                            break;
                        case MessagePacket messagePacket when signedIn:
                            lock (@lock)
                                OnMessageReceived(messagePacket.Message);
                            break;
                        case DisconnectPacket _ when signedIn:
                            connected = false;
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

                    client.SendPacket(new ResponsePacket(ResponseCode.SignInSucceeded));
                    Console.WriteLine(Messages.Server.ClientConnect, signInMember, client.RemoteIP.Address.ToString());
                }
                else
                {
                    client.SendPacket(new ResponsePacket(ResponseCode.UnauthorizedLogin));
                    connected = false;
                    signInMember = null; // Because sign in failed
                }

                bool CanSignIn()
                {
                    lock (@lock)
                        return memberPasswords.PasswordCorrect(infos) && 
                            group.MemberExists(infos.MemberId);
                }
            }

            void OnMessageReceived(Message message)
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