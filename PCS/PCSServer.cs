using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PCS
{
    public class PcsServer : IDisposable // TODO Do rather a library that make text exchanges easier
    {
        private readonly Encoding encoding = Encoding.UTF8;
        private PcsClient socket;

        public PcsServer()
        {
        }
        
        public void Connect(IPAddress ip)
        {
            socket = new PcsClient(ip);

            try
            {
                socket.Connect(ip);

                Console.WriteLine("Client connected to {0}", ip.MapToIPv4());
            }
            catch
            {
                throw;
            }

        }

        public void SignIn(Member member)
        {
            socket.SendBytes(member.GetBytes());
        }

        public void SendMessage(string text)
        {
            socket.SendText(text);
        }

        public Message ReceiveServerMessage()
        {
            string receivedMsg = socket.ReceiveText();

            var infos = receivedMsg.Split(new char[] { (char)3, (char)4 }, StringSplitOptions.RemoveEmptyEntries);

            var author = new Member(infos[1], Convert.ToInt32(infos[2]));
            var message = new Message(author, infos[0]);

            return message;
        }

        public void Disconnect()
        {
            // TEMP, it's because legit disconnect handling has not been set-up yet
            socket.Disconnect();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
