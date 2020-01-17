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
    public class PCSServer
    {
        private readonly Socket listener;
        readonly List<Socket> connectedClients;

        public const ushort Port = 6783;

        public PCSServer()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); // TODO: Why I have to put localhost wheareas it's a server
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);

            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);

            connectedClients = new List<Socket>();
        }

        public void HostClients()
        {
            try
            {
                listener.Listen(10);

                Console.WriteLine("Server started.");

                while (true)
                {
                    Socket client = listener.Accept();
                    connectedClients.Add(client);

                    var connectionThread = new Thread(() => HandleConnection(client));
                    connectionThread.Start();
                }
            }
            catch
            {
                throw;
            }
        }

        private void HandleConnection(Socket client)
        {
            Member identifiedMember = Identify(client);

            Console.WriteLine("{0} connected!", identifiedMember);

            while (true)
            {
                try
                {
                    string receivedMsg = ReceiveMessage(client);

                    Console.WriteLine("{0} sent: {1}", identifiedMember, receivedMsg);

                    var message = new Message(identifiedMember, receivedMsg);
                    var messageAdder = new MessageHandler(AddMessage);
                    messageAdder.Invoke(message);
                }
                catch
                {
                    break;
                }
            }

            Disconnect(client);

            Console.WriteLine("{0} disconnected.", identifiedMember);

        }

        delegate void MessageHandler(Message message);
        private void AddMessage(Message message)
        {
            byte[] bytesMsg = message.GetBytes();

            // Send to all clients
            foreach (var connectedClient in connectedClients)
            {
                connectedClient.Send(bytesMsg);
            }
        }

        static Member Identify(Socket client)
        {
            string signedInMember = ReceiveMessage(client);

            var infos = signedInMember.Split(new string[] { ";:!", "\0" }, StringSplitOptions.RemoveEmptyEntries);

            return new Member(infos[0], Convert.ToInt32(infos[1]));
        }

        static string ReceiveMessage(Socket client)
        {
            var incomingBuffer = new byte[1024];

            string data = string.Empty;

            while (true)
            {
                int bytesRecording = client.Receive(incomingBuffer);

                string appendData = Encoding.UTF8.GetString(incomingBuffer, 0, bytesRecording);

                data += appendData;

                if (data.EndsWith("\0"))
                    break;
            }

            return data;
        }

        //void SendEchoMessage(Socket client, string message)
        //{
        //    var echoMsg = Encoding.UTF8.GetBytes(message);
        //    client.Send(echoMsg);
        //}

        void Disconnect(Socket client)
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();

            connectedClients.Remove(client);
        }
    }
}
