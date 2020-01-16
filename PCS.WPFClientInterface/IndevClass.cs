using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PCS.WPFClientInterface
{
    class IndevClass
    {
        private IPAddress m_ipAddress;
        private int m_port;

        private Action<DateTime, Member, string, Resource> m_addMessageFunction;

        public IndevClass(
            IPAddress ipAddress, 
            int port, 
            Action<DateTime, Member, string, Resource> addMessageFunction)
        {
            m_ipAddress = ipAddress;
            m_port = port;
            m_addMessageFunction = addMessageFunction;
        }

        public void Listen()
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", m_port);

            NetworkStream stm = tcpClient.GetStream();
            Member member = new Member() { ID = 1 };
            byte[] memberTransmittingBytes = member.GetBytes();

            tcpClient.Close();
        }

        public void AddMessage(DateTime dateTime, Member member, string message, Resource resource = null) { }

        public void SendMessage(string message) { }
        public void SendMessage(string message, Resource resource) { }
    }
}
