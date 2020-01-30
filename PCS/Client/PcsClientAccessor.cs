using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PCS
{
    // TODO: Install Boolean Is Connected/Signed In
    // TODO: Maybe mix connect and sign in
    public class PcsClientAccessor : PcsClient
    {
        public PcsFtpClient Ftp { get; } = new PcsFtpClient();

        public void StartListenAsync(Action<Message> messageReceived)
        {
            var thread = new Thread(() => Listen());
            thread.Start();

            void Listen()
            {
                while (true)
                {
                    string receivedData = Receive();
                    var dataPacket = new DataPacket(receivedData);

                    if (dataPacket.Type == DataPacketType.ServerMessage)
                        messageReceived(dataPacket.GetMessage());
                    else
                        throw new DataPacketException(Messages.Exceptions.NotRecognizedDataPacket);
                }
            }
        }

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

            Ftp.Connect(ip);
        }

        public void SignIn(Member member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));

            Send(Flags.ClientSignIn + DataPacket.FromMember(member));
        }

        public void SendMessage(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrEmpty(message.Text) || string.IsNullOrEmpty(message.ChannelTitle)) 
                throw new MessageEmptyFieldException(Messages.Exceptions.MessageEmptyField);

            Send(Flags.ClientMessage + DataPacket.FromMessage(message));
        }

        public override void Disconnect()
        {
            Send(Flags.ClientDisconnect.ToString(CultureInfo.CurrentCulture));

            base.Disconnect();
        }
    }
}
