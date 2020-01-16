using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PCS.ServerApp
{
    class PCSServer
    {
        public const ushort Port = 6783;

        public string Data { get; private set; } = null;

        public PCSServer()
        {
        }

        public void Listen()
        {
            // UNDONE: gestion des thread
        }

        public void StartListening()
        {
            var incomingBuffer = new byte[1024];

            IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); // TODO: Why I have to put localhost wheareas it's a server
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);

            var listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    Socket handler = listener.Accept();
                    Data = null;

                    while (true)
                    {
                        int bytesRecording = handler.Receive(incomingBuffer);

                        string appendData = Encoding.ASCII.GetString(incomingBuffer, 0, bytesRecording);

                        Data += appendData;

                        if (Data.EndsWith("\0"))
                            break;
                    }

                    Console.WriteLine("Text received: {0}", Data);

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
