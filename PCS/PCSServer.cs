using System;
using System.Collections.Generic;
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

                Console.WriteLine("Server started.");

                while (true)
                {
                    var client = listener.Accept();
                    connectedClients.Add(client);

                    var connectionThread = new Thread(() => new ConnectionHandler(client, OnMessageReceived, OnClientDisconnect));
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
            SendToEveryone(message);
            SaveMessage(message);
        }

        private void OnClientDisconnect(PcsClient client)
        {
            connectedClients.Remove(client);
        }

        private void SendToEveryone(Message message)
        {
            // Send to all clients
            foreach (var connectedClient in connectedClients)
                //connectedClient.SendMessage(message);
                connectedClient.SendText(message.GetData());
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
