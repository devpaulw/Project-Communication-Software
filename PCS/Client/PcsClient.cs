using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Sockets;
using System.Text;

namespace PCS
{
    public class PcsClient : IDisposable
    {
        private bool disposed;

        protected Socket AdapteeSocket { get; set; }

        public PcsClient() { }

        public PcsClient(Socket socket)
        {
            AdapteeSocket = socket;
        }

        public string Receive()
        {
            var incomingBuffer = new byte[1024];

            string data = string.Empty;

            while (true)
            {
                int bytesRecording = AdapteeSocket.Receive(incomingBuffer);

                string appendData = PcsServer.Encoding.GetString(incomingBuffer, 0, bytesRecording);

                data += appendData;

                if (data.EndsWith(Flags.EndOfTransmission.ToString(CultureInfo.CurrentCulture), StringComparison.CurrentCulture))
                {
                    break;
                }
            }

            return data;
        }

        public void Send(string text)
        {
            text += Flags.EndOfTransmission;

            byte[] encodedMessage = PcsServer.Encoding.GetBytes(text);

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
            if (!disposed)
            {
                if (disposing)
                {
                    Disconnect();
                    AdapteeSocket.Dispose();
                }

                disposed = true;
            }
        }
    }
}
