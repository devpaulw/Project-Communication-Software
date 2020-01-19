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
    public class PcsServerHost
    {
        private readonly PcsListener listener;

        readonly List<PcsClient> connectedClients;

        public static readonly Encoding Encoding = Encoding.UTF8;

        public PcsServerHost()
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
            byte[] bytesMsg = message.GetBytes();

            // Send to all clients
            foreach (var connectedClient in connectedClients)
                connectedClient.SendBytes(bytesMsg);
        }


        private void OnClientDisconnect(PcsClient client)
        {
            connectedClients.Remove(client);
        }
    }
}
