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
        private readonly PcsListener listener;
        private readonly List<PcsClient> connectedClients;

        public const ushort Port = 6783;
        public static readonly Encoding Encoding = Encoding.Unicode;

        public PcsServer()
        {
            listener = new PcsListener(IPAddressHelper.GetLocalIPAddress());
            connectedClients = new List<PcsClient>();
        }
        
        public void HostClients()
        {
            try
            {
                listener.Listen();

                Console.WriteLine("Server stared.");

                while (true)
                {
                    var client = listener.Accept();
                    connectedClients.Add(client);

                    var connectionThread = new Thread(() => new ConnectionHandler(client, OnClientConnect, OnMessageReceived, OnClientDisconnect));
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
            Console.WriteLine("{0} sent from channel {2} at {3}: {1}", message.Author, message.Text, message.ChannelTitle, message.DateTime.ToLongTimeString());

            SendToEveryone(message);
            SaveMessage(message);
        }

        private void OnClientConnect(Member member)
        {
            Console.WriteLine("{0} connected!", member);
        }

        private void OnClientDisconnect(PcsClient client)
        {
            connectedClients.Remove(client);
        }

        private void SendToEveryone(Message message)
        {
            // Send to all clients
            foreach (var connectedClient in connectedClients)
                connectedClient.SendServerMessage(message);
        }

        private void SaveMessage(Message message)
        {
            // Will be filled later
        }

        public void Dispose()
        {
            // Example: Save messages, properly disconnect people...
        }
    }
}
