using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Text;

namespace PCS
{
    public class PcsClient : IDisposable
    {
        private protected bool disposedValue;

        protected Socket AdapteeSocket { get; set; }

        public PcsClient() { }

        public PcsClient(Socket socket)
        {
            AdapteeSocket = socket;
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
                    int bytesRecording = AdapteeSocket.Receive(incomingBuffer);

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
            AdapteeSocket.Send(encodedMessage);
        }

        public virtual void Disconnect()
        {
            AdapteeSocket.Shutdown(SocketShutdown.Both);
            AdapteeSocket.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Disconnect();
                    AdapteeSocket.Dispose();
                }

                disposedValue = true;
            }
        }
    }
}
