using System;
using System.Collections.Generic;
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
            SendText(/*Flags.Connect + Flags.EndOfText +*/ member.GetData());
        }

        public void SendText(string message)
        {
            message += Flags.EndOfTransmission;

            byte[] encodedMessage = PcsServer.Encoding.GetBytes(message);

            adapteeSocket.Send(encodedMessage);
        }

        public string ReceiveText()
        {
            var incomingBuffer = new byte[1024];

            string data = string.Empty;

            while (true)
            {
                int bytesRecording = adapteeSocket.Receive(incomingBuffer);

                string appendData = PcsServer.Encoding.GetString(incomingBuffer, 0, bytesRecording);

                data += appendData;

                if (data.EndsWith(Flags.EndOfTransmission.ToString()))
                {
                    data = data.Remove(data.Length - 1, 1); // Remove end of transmission char
                    break;
                }
            }

            return data;
        }

        public bool IsConnected() // Temporary before a new approach
        {
            try
            {
                return !(adapteeSocket.Poll(1, SelectMode.SelectRead) && adapteeSocket.Available == 0);
            }
            catch (SocketException) { return false; }
        }

        public void Disconnect()
        {
            adapteeSocket.Dispose();
            return;
            adapteeSocket.Shutdown(SocketShutdown.Both);
            adapteeSocket.Close();
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
