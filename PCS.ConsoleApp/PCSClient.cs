using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PCS.ConsoleApp
{
    class PCSClient
    {
        public const ushort ConnectingPort = 6783;

        private Socket sender;
        public IPAddress ConnectedAddress { get; set; }

        public PCSClient(IPAddress address)
        {
            ConnectedAddress = address;
        }

        public void Connect()
        {
            try
            {
                var endPoint = new IPEndPoint(ConnectedAddress, ConnectingPort);

                sender = new Socket(ConnectedAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

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

        }

        public void SendMessage(string message)
        {
            message += '\0';
            byte[] encodedMessage = Encoding.ASCII.GetBytes(message);

            int bytesSent = sender.Send(encodedMessage);
        }

        public void Disconnect()
        {
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }
    }
}
