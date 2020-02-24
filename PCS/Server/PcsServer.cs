using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem.DotNet;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly List<PcsClient> connectedClients;
        private readonly object @lock = new object();

        public const ushort Port = 6783;
        public static readonly Encoding Encoding = Encoding.UTF8;

        public PcsServer(IPAddress serverAddress)
        {
            if (serverAddress == null)
                throw new ArgumentNullException(nameof(serverAddress));

            listener = new PcsListener(serverAddress);
            ftpClient = new PcsFtpClient(serverAddress);
            connectedClients = new List<PcsClient>();
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
                    OnClientConnect(client);
                    
                    var connectionThread = new Thread(() =>
                    {
                        lock (@lock) // Prevent that two thread don't call this function at the same time
                            new ClientConnectionManager(client, CanSignIn, OnMessageReceived, OnClientDisconnect);
                    });

                    connectionThread.Start();
                }
            }
            catch
            {
                throw;
            }
        }

        private bool CanSignIn(Member member)
        {
            return member.Username != "Herobrine";
        }

        private void OnClientConnect(PcsClient client)
        {
            connectedClients.Add(client);
        }

        private void OnMessageReceived(BroadcastMessage message)
        {
            Console.WriteLine("Received: " + message);

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
            }
        }

        private void OnClientDisconnect(PcsClient client, Member member)
        {
            connectedClients.Remove(client);
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