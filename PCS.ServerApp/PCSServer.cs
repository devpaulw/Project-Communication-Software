using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCS.ServerApp
{
    class PCSServer
    {
        private readonly Socket listener;

        public const ushort Port = 6783;

        public PCSServer()
        {
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); // TODO: Why I have to put localhost wheareas it's a server
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);

            listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
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
            var identifiedMember = Identify();

            Console.WriteLine("{0} connected!", identifiedMember);

            while (true)
            {
                try
                {
                    string receivedMsg = ReceiveMessage();

                    Console.WriteLine("{0} sent: {1}", identifiedMember, receivedMsg);

                    SendEchoMessage(receivedMsg);
                }
                catch
                {
                    break;
                }
            }

            Disconnect();
            
            Member Identify()
            {
                string signedInMember = ReceiveMessage();

                var infos = signedInMember.Split(new string[] { "MBR::UN:", ";ID:", ";;", "\0" }, StringSplitOptions.RemoveEmptyEntries);

                return new Member(infos[0], Convert.ToInt32(infos[1]));
            }

            string ReceiveMessage()
            {
                var incomingBuffer = new byte[1024];

                string data = string.Empty;

                while (true)
                {
                    int bytesRecording = client.Receive(incomingBuffer);

                    string appendData = Encoding.ASCII.GetString(incomingBuffer, 0, bytesRecording);

                    data += appendData;

                    if (data.EndsWith("\0"))
                        break;
                }

                return data;
            }

            void SendEchoMessage(string message)
            {
                var echoMsg = Encoding.ASCII.GetBytes(message.ToUpper());
                client.Send(echoMsg);
            }

            void Disconnect()
            {
                Console.WriteLine("{0} disconnected.", identifiedMember);

                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }

            // UNDONE managed functions ...
        }
    }
}
