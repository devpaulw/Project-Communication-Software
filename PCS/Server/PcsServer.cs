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

namespace PCS
{
    public class PcsServer : IDisposable
    {
        private bool disposed;

        private readonly PcsListener listener;
        private readonly PcsFtpClient ftpClient;
        private readonly MessageTable messageTable = new MessageTable();
        private readonly List<PcsClient> connectedClients = new List<PcsClient>();
        private readonly object @lock = new object();

        public const ushort Port = 6783;
        public static readonly Encoding Encoding = Encoding.UTF8;

        public PcsServer(IPAddress serverAddress)
        {
            if (serverAddress == null)
                throw new ArgumentNullException(nameof(serverAddress));

            listener = new PcsListener(serverAddress);
            ftpClient = new PcsFtpClient(serverAddress);
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
            Member signedInMember = Member.Unknown;
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
                                OnSignIn(signInPacket.Member);
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

            void OnSignIn(Member member)
            {
                signedInMember = member;

                if (CanSignIn())
                {
                    signedIn = true;

                    client.SendPacket(new ResponsePacket(ResponseCode.SignInSucceeded));
                    Console.WriteLine(Messages.Server.ClientConnect, member, client.RemoteIP.Address.ToString());
                }
                else
                {
                    client.SendPacket(new ResponsePacket(ResponseCode.UnauthorizedLogin));
                    connected = false;
                }

                bool CanSignIn()
                {
                    return member.Username != "Herobrine";
                }
            }

            void OnMessageReceived(Message message)
            {
                Console.WriteLine("Received: " + message);

                var broadcastMsg = new BroadcastMessage(messageTable.GetNewID(), message, DateTime.Now, signedInMember);
                AddBroadcast(broadcastMsg);
            }

            void OnDisconnect()
            {
                client.Disconnect();
                Console.WriteLine(Messages.Server.ClientDisconnect, signedInMember);

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
                ftpClient.SaveMessage(message);
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
                    ftpClient.Dispose();
                }

                disposed = true;
            }
        }
    }
}