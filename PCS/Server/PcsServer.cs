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
        private readonly PcsFtpClient ftpClient;
        private readonly List<PcsClient> connectedClients;

        public const ushort Port = 6783;
        public static readonly Encoding Encoding = Encoding.UTF8;

        public PcsServer()
        {
            ftpClient = new PcsFtpClient();
            listener = new PcsListener(IPAddressHelper.GetLocalIPAddress());
            connectedClients = new List<PcsClient>();
        }
        
        public void StartHosting()
        {
            ftpClient.Connect(IPAddressHelper.GetLocalIPAddress());

            try
            {
                listener.Listen();

                Console.WriteLine(Messages.Server.Started, DateTime.Now);

                while (true)
                {
                    var client = listener.Accept();
                    connectedClients.Add(client);

                    var connectionThread = new Thread(() 
                        => new ClientConnectionHandler(client, OnClientSignIn, OnMessageReceived, OnClientDisconnect));

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
            Console.WriteLine(Messages.Server.ClientSentMessage, message.Author, message.ChannelTitle, message.DateTime.ToLongTimeString(), message.Text);

            SendToEveryone(message);
            SaveMessage(message);
        }

        private void OnClientSignIn(Member member)
        {
            Console.WriteLine(Messages.Server.ClientConnect, member);;
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
                SendToClientAccessor(connectedClient, message);
        }

        private void SendToClientAccessor(PcsClient client, Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            client.Send(Flags.ServerMessage + Flags.EndOfText + DataPacket.FromMessage(message));
        }

        private void SaveMessage(Message message)
        {
            ftpClient.SaveMessage(message);
        }

        public void Dispose()
        {
            // DOLATER
        }
    }
}
