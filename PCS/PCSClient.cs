using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PCS
{
    public class PcsClient : IDisposable
    {
        private Socket adapteeSocket;

        public PcsClient() { }

        public PcsClient(Socket cloneInstance)
        {
            adapteeSocket = cloneInstance;
        }

        public void Connect(IPAddress ip)
        {
            if (ip.MapToIPv4().ToString() == "127.0.0.1") ip = IPAddressHelper.GetLocalIPAddress(); // Accept localhost ip
            adapteeSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var endPoint = new IPEndPoint(ip, PcsServer.Port);

            try
            {
                adapteeSocket.Connect(endPoint);
                Console.WriteLine(Properties.Resources.ClientConnected, ip.MapToIPv4());
            }
            catch
            {
                throw;
            }
        }

        public void SignIn(Member member) // TODO should perhaps be separated
        {
            if (member != null) 
                Send(Flags.ClientSignIn + Flags.EndOfText + DataPacket.FromMember(member));
            else throw new ArgumentNullException(nameof(member));
        }

        public void SendClientMessage(Message message)
        {
            if (message != null) 
                Send(Flags.ClientMessage + Flags.EndOfText + DataPacket.FromMessage(message, true));
            else throw new ArgumentNullException(nameof(message));
        }

        public void SendServerMessage(Message message)
        {
            if (message != null)
                Send(Flags.ServerMessage + Flags.EndOfText + DataPacket.FromMessage(message, false));
            else throw new ArgumentNullException(nameof(message));
        }

        public void Disconnect()
        {
            try // TEMP Because when the server connection handler try to send DC it crashes // TODO Separation!
            {
                Send(Flags.ClientDisconnect.ToString(CultureInfo.CurrentCulture));
            }
            catch (SocketException) { }

            adapteeSocket.Shutdown(SocketShutdown.Both);
            adapteeSocket.Close();
        }

        public string Receive()
        {
            var incomingBuffer = new byte[1024];

            string data = string.Empty;

            while (true)
            {
                int bytesRecording = adapteeSocket.Receive(incomingBuffer);

                string appendData = PcsServer.Encoding.GetString(incomingBuffer, 0, bytesRecording);

                data += appendData;

                if (data.EndsWith(Flags.EndOfTransmission.ToString(CultureInfo.CurrentCulture), StringComparison.CurrentCulture))
                {
                    break;
                }
            }

            return data;
        }

        public Message ReceiveMessage() // Exclusive for clients applications
        {
            string receivedData = Receive();
            var dataPacket = new DataPacket(receivedData);

            if (dataPacket.Type == DataPacketType.ServerMessage) return dataPacket.GetServerMessage();
            else throw new DataPacketException(Properties.Resources.NotRecognizedDataPacket);
        }

        public void Dispose()
        {
            Disconnect();
        }

        private void Send(string text)
        {
            text += Flags.EndOfTransmission;

            byte[] encodedMessage = PcsServer.Encoding.GetBytes(text);

            adapteeSocket.Send(encodedMessage);
        }
    }
}