using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PCS
{
    public class PcsClientAccessor : PcsClient
    {
        private PcsFtpClient ftpClient = new PcsFtpClient();

        public void Connect(IPAddress ip)
        {
            if (ip == null) throw new ArgumentNullException(nameof(ip));

            if (ip.MapToIPv4().ToString() == IPAddressHelper.Localhost)
                ip = IPAddressHelper.GetLocalIPAddress(); // Accept localhost ip

            AdapteeSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var endPoint = new IPEndPoint(ip, PcsServer.Port);

            try
            {
                AdapteeSocket.Connect(endPoint);
                Console.WriteLine(Messages.Client.Connected, ip.MapToIPv4());
            }
            catch
            {
                throw;
            }

            ftpClient.Connect(ip);
        }

        public void SignIn(Member member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));

            Send(Flags.ClientSignIn + Flags.EndOfText + DataPacket.FromMember(member));
        }

        public void SendMessage(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            Send(Flags.ClientMessage + Flags.EndOfText + DataPacket.FromMessage(message));
        }

        public Message ReceiveMessage() // Exclusive for clients applications
        {
            string receivedData = Receive();
            var dataPacket = new DataPacket(receivedData);

            if (dataPacket.Type == DataPacketType.ServerMessage)
                return dataPacket.GetMessage();
            else throw new DataPacketException(Messages.Exceptions.NotRecognizedDataPacket);
        }

        public override void Disconnect()
        {
            Send(Flags.ClientDisconnect.ToString(CultureInfo.CurrentCulture));

            base.Disconnect();
        }

        public IEnumerable<Message> GetDailyMessages(DateTime day) 
            => ftpClient.GetDailyMessages(day);
    }
}
