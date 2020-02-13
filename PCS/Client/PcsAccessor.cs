using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PCS
{
    public class PcsAccessor : PcsClient
    {
        private Thread serverListenThread;
        private PcsFtpClient ftp;

        public bool IsConnected { get; private set; }

        public PcsAccessor()
        {
        }

        public void Connect(IPAddress ip, Member member)
        {
            if (ip == null)
                throw new ArgumentNullException(nameof(ip));

            AdapteeClient = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var endPoint = new IPEndPoint(ip, PcsServer.Port);

            AdapteeClient.Connect(endPoint);
            Console.WriteLine(Messages.Client.Connected, ip.MapToIPv4());

            ftp = new PcsFtpClient(ip);

            SignIn();

            void SignIn()
            {
                if (member == null)
                    throw new ArgumentNullException(nameof(member));

                SendPacket(new SignInPacket(member));

                IsConnected = true;
            }
        }

        public void StartListenAsync(Action<BroadcastMessage> messageReceived)
        {
            if (!IsConnected)
                throw new Exception(Messages.Exceptions.NotConnected);

            serverListenThread = new Thread(() => Listen());
            serverListenThread.Start();

            void Listen()
            {
                while (IsConnected)
                {
                    try
                    {
                        Packet receivedPacket = ReceivePacket();

                        if (receivedPacket is BroadcastMessagePacket == false)
                            throw new Exception(Messages.Exceptions.NotRecognizedDataPacket); // DOLATER: Handle better save messages on the PC, not just resources

                        messageReceived((receivedPacket as BroadcastMessagePacket).BroadcastMessage);
                    }
                    catch (SocketException)
                    {
                        if (IsConnected)
                            throw;
                    }
                }
            }
        }

        public void SendMessage(Message message)
        {
            if (!IsConnected)
                throw new Exception(Messages.Exceptions.NotConnected);

            if (message == null)
                throw new ArgumentNullException(nameof(message));

            SendPacket(new MessagePacket(message));
        }

        public void SendTask(Task task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            SendPacket(new TaskPacket(task));
        }

        public void SendList(TaskList taskList)
        {
            if (taskList == null)
                throw new ArgumentNullException(nameof(taskList));

            SendPacket(new TaskListPacket(taskList));
        }

        public IEnumerable<BroadcastMessage> GetDailyMessages(string channelName, DateTime day)
        {
            var dailyMessages = new List<BroadcastMessage>(ftp.GetDailyMessages(channelName, day));

            return dailyMessages;
        }

        public override void Disconnect()
        {
            if (IsConnected)
            {
                SendPacket(new DisconnectPacket());

                IsConnected = false;

                base.Disconnect();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsConnected)
                return;

            if (!disposedValue && disposing)
            {
                ftp.Dispose();

                base.Dispose(disposing);
            }
        }
    }
}
