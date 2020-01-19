using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PCS
{
    class PcsClient
    {
        private readonly Encoding encoding = Encoding.UTF8;
        private readonly Socket adapteeSocket;

        public const ushort Port = 6783;

        public PcsClient(IPAddress ip)
        {
            adapteeSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public PcsClient(Socket cloneInstance)
        {
            adapteeSocket = cloneInstance;
        }

        public void Connect(IPAddress ip)
        {
            var endPoint = new IPEndPoint(ip, Port);
            adapteeSocket.Connect(endPoint);
        }

        public void SendBytes(byte[] buffer)
        {
            adapteeSocket.Send(buffer);
        }

        public byte[] ReceiveBytes()
        { // TODO This is not safe and it have to updated
            var incomingBuffer = new byte[1024];

            adapteeSocket.Receive(incomingBuffer);

            return incomingBuffer;
        }

        public void SendText(string message)
        {
            // TODO Regroup separators and end of transmission in a known class
            message += (char)4;

            byte[] encodedMessage = encoding.GetBytes(message);

            adapteeSocket.Send(encodedMessage);
        }

        public string ReceiveText()
        {
            var incomingBuffer = new byte[1024];

            string data = string.Empty;

            while (true)
            {
                int bytesRecording = adapteeSocket.Receive(incomingBuffer);

                string appendData = encoding.GetString(incomingBuffer, 0, bytesRecording);

                data += appendData;

                if (data.EndsWith(((char)4).ToString()))
                {
                    // TODO too ugly
                    data = data.Remove(data.Length - 1, 1);
                    break;
                }
            }

            return data;
        }

        public void Disconnect()
        {
            adapteeSocket.Shutdown(SocketShutdown.Both);
            adapteeSocket.Close();
        }
    }
}
