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
                Console.WriteLine("Client connected to {0}", ip.MapToIPv4());
            }
            catch
            {
                throw;
            }
        }

        public void SignIn(Member member) // TODO should perhaps be separated
        {
            Send(Flags.SigningIn + Flags.EndOfText + member.GetPacketData());
        }

        public void SendClientMessage(Message message)
        {
            Send(Flags.Text + Flags.EndOfText + message.GetPacketData());
        }

        public void SendServerMessage(Message message)
        {
            Send(Flags.Message + Flags.EndOfText + message.GetPacketData());
        }

        public void Disconnect()
        {
            try // TEMP Because when the server connection handler try to send DC it crashes
            {
                Send(Flags.Disconnection);
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
            Message message = DataPacket.TryGetMessage(receivedData);

            if (message != null)
                return message;
            else
                throw new Exception("Message receipt failed.");
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