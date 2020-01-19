using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PCS
{
    class PcsListener
    {
        private readonly Socket adapteeListener;

        public PcsListener(IPAddress ip)
        {
            var localEndPoint = new IPEndPoint(ip, PcsClient.Port); // TODO: Is port in the right place?

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
    }
}
