using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using PCS.Data;
using PCS.Data.Packets;

namespace PCS
{
    public class PcsClient : IDisposable
    {
        private bool disposedValue;

        protected Socket AdapteeClient { get; set; }

        public bool IsConnected { get; set; }

        // TODO public Member Member { get; set; }

        public IPEndPoint RemoteIP
            => AdapteeClient.RemoteEndPoint == null ? null : AdapteeClient.RemoteEndPoint as IPEndPoint;
        public IPEndPoint LocalIP
            => AdapteeClient.LocalEndPoint == null ? null : AdapteeClient.LocalEndPoint as IPEndPoint;

        public PcsClient() { }

        public PcsClient(Socket client)
        {
            AdapteeClient = client;
            IsConnected = true;
        }

        internal Packet ReceivePacket()
        {
            string receivedData = Receive();

            var dataPacket = Packet.FromTextData(receivedData);

            return dataPacket;

            string Receive()
            {
                var incomingBuffer = new byte[1024];

                string data = string.Empty;

                while (true)
                {
                    int bytesRecording = AdapteeClient.Receive(incomingBuffer);

                    string appendData = PcsServer.Encoding.GetString(incomingBuffer, 0, bytesRecording);

                    data += appendData;

                    if (data.EndsWith(ControlChars.EndOfTransmission.ToString(CultureInfo.CurrentCulture), StringComparison.CurrentCulture))
                    {
                        break;
                    }
                }

                return data;
            }
        }

        internal void SendPacket(Packet packet)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            string textData = packet.GetTextData();
            textData += ControlChars.EndOfTransmission;

            byte[] encodedMessage = PcsServer.Encoding.GetBytes(textData);

            AdapteeClient.Send(encodedMessage);
        }

        public virtual void Disconnect()
        {
            IsConnected = false;

            AdapteeClient.Shutdown(SocketShutdown.Both);
            AdapteeClient.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsConnected)
                return;

            if (!disposedValue)
            {
                if (disposing)
                {
                    Disconnect();
                    AdapteeClient.Dispose();
                }

                disposedValue = true;
            }
        }
    }
}
