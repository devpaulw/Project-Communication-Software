using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PCS
{
    public class PCSClient : IDisposable
    {
        private Socket sender;

        public const ushort ConnectingPort = 6783;

        public PCSClient()
        {
        }
        
        public void Connect(IPAddress address)
        {
            sender = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                var endPoint = new IPEndPoint(address, ConnectingPort);

                try
                {
                    sender.Connect(endPoint);

                    Console.WriteLine("Client connected to {0}", sender.RemoteEndPoint); // USELESS
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

            byte[] encodedMessage = Encoding.UTF8.GetBytes(message);

            sender.Send(encodedMessage);
        }

        public Message ReceiveServerMessage()
        {
            byte[] buffer = new byte[1024]; // TODO: Make it bigger, and smarter; Even make a class to handler send/receive

            int bytesRec = sender.Receive(buffer);
            string receivedMsg = Encoding.UTF8.GetString(buffer, 0, bytesRec);

            var infos = receivedMsg.Split(new string[] { ";:!", "\0" }, StringSplitOptions.RemoveEmptyEntries);

            var author = new Member(infos[1], Convert.ToInt32(infos[2]));
            var message = new Message(author, infos[0]);

            return message;
        }

        //public string ReceiveEchoMessage()
        //{
        //    byte[] echoBuffer = new byte[1024];

        //    int bytesRec = sender.Receive(echoBuffer);

        //    return Encoding.UTF8.GetString(echoBuffer, 0, bytesRec);
        //}

        public void Disconnect()
        {
            // TEMP, it's because legit disconnect handling has not been set-up yet
            //sender.Shutdown(SocketShutdown.Both);
            //sender.Close();
            sender.Dispose();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
