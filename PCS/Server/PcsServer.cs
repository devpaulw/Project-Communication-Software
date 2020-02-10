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
        {
            try
            {
                listener.Listen();

                Console.WriteLine(Messages.Server.Started, DateTime.Now);

                while (true)
                {
                    var client = listener.Accept();
                    connectedClients.Add(client);

                    var connectionThread = new Thread(() 
                        => new ClientConnectionHandler(
                            client, OnClientSignIn, OnMessageReceived, OnClientDisconnect));

                    connectionThread.Start();
                }
            }
            catch
            {
                throw;
            }
        }

        private void OnMessageReceived(Message message)
        {
            Console.WriteLine("Received: " + message);

            SendToEveryone(message);
            SaveMessage(message);
        }

        private void OnClientSignIn(Member member)
        {
            Console.WriteLine(Messages.Server.ClientConnect, member);
        }

        private void OnClientDisconnect(PcsClient client, Member member)
        {
            connectedClients.Remove(client);

            Console.WriteLine(Messages.Server.ClientDisconnect, member);
        }

        private void SendToEveryone(Message message)
        {
            // Send to all clients
            foreach (var connectedClient in connectedClients)
                SendToAccessor(connectedClient);

            void SendToAccessor(PcsClient client)
            {
                if (message == null) throw new ArgumentNullException(nameof(message));

                client.SendPacket(new DataPacket(DataPacketType.Message, message));
            }
        }

        private void SaveMessage(Message message)
        {
            ftpClient.SaveMessage(message);
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