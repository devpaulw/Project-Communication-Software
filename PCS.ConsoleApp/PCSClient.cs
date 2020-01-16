using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PCS.ConsoleApp
{
    class PCSClient : IDisposable
    {
        private readonly Socket sender;

        public const ushort ConnectingPort = 6783;

        public IPAddress ConnectedAddress { get; set; }

        public PCSClient(IPAddress address)
        {
            ConnectedAddress = address;

            sender = new Socket(ConnectedAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        
        public void Connect()
        {
            try
            {
                var endPoint = new IPEndPoint(ConnectedAddress, ConnectingPort);

                try
                {
                    sender.Connect(endPoint);

                    Console.WriteLine("Client connected to {0}", sender.RemoteEndPoint);
                }
                catch
                {
                    throw;
                }
            }
            catch
            {
                throw;
            }
        }

        public void SignIn(Member member)
        {
            sender.Send(member.GetBytes());
        }

        public void SendMessage(string message)
        {
            message += '\0';

            byte[] encodedMessage = Encoding.ASCII.GetBytes(message);

            sender.Send(encodedMessage);
        }

        public string ReceiveEchoMessage()
        {
            byte[] echoBuffer = new byte[1024];

            int bytesRec = sender.Receive(echoBuffer);

            return Encoding.ASCII.GetString(echoBuffer, 0, bytesRec);
        }

        public void Disconnect()
        {
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
