using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PCS
{
    class PcsListener : IDisposable
    {
        private bool disposed;
        private readonly Socket adapteeListener;

        public PcsListener(IPAddress ip)
        {
            var localEndPoint = new IPEndPoint(ip, PcsServer.Port);

            adapteeListener = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            adapteeListener.Bind(localEndPoint);
        }

        public void Listen()
        {
            adapteeListener.Listen(10);
        }

        public PcsClient Accept()
        {
            return new PcsClient(adapteeListener.Accept());
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
                    adapteeListener.Dispose();
                }

                disposed = true;
            }
        }
    }
}
