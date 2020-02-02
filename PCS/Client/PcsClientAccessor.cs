﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PCS
{
    public class PcsClientAccessor : PcsClient
    {
        private Thread serverListenThread;

        public PcsFtpClient Ftp { get; private set; }
        public bool IsConnected { get; private set; }

        public void Connect(IPAddress ip, Member member)
        {
            if (ip == null) throw new ArgumentNullException(nameof(ip));

            AdapteeSocket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            var endPoint = new IPEndPoint(ip, PcsServer.Port);

            try
            {
                AdapteeSocket.Connect(endPoint);
                Console.WriteLine(Messages.Client.Connected, ip.MapToIPv4());
            }
            catch
            {
                throw;
            }

            Ftp = new PcsFtpClient(ip);

            SignIn();

            void SignIn()
            {
                if (member == null) throw new ArgumentNullException(nameof(member));

                Send(Flags.ClientSignIn + DataPacket.FromMember(member));

                IsConnected = true;
            }
        }

        public void StartListenAsync(Action<Message> messageReceived)
        {
            if (!IsConnected)
                throw new Exception(Messages.Exceptions.NotConnected);

            serverListenThread = new Thread(() => Listen());
            serverListenThread.Start();

            void Listen()
            {
                while (true)
                {
                    string receivedData = Receive();
                    var dataPacket = new DataPacket(receivedData);

                    if (dataPacket.Type == DataPacketType.ServerMessage)
                        messageReceived(dataPacket.GetMessage());
                    else
                        throw new DataPacketException(Messages.Exceptions.NotRecognizedDataPacket);
                }
            }
        }

        public void SendMessage(Message message)
        {
            if (!IsConnected)
                throw new Exception(Messages.Exceptions.NotConnected);

            if (message == null) throw new ArgumentNullException(nameof(message));

            if (message.AttachedResources != null)
            {
                for (int i = 0; i < message.AttachedResources.Count; i++) // Transition of local path to ftp path
                {
                    Ftp.UploadResource(message.AttachedResources[i], out string generatedFileName);
                    message.AttachedResources[i] = generatedFileName;
                }
            }

            Send(Flags.ClientMessage + DataPacket.FromMessage(message));
        }

        public override void Disconnect()
        {
            if (IsConnected)
            {
                Send(Flags.ClientDisconnect.ToString(CultureInfo.CurrentCulture));

                serverListenThread.Abort(); // Stop listen server

                IsConnected = false;

                base.Disconnect();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && IsConnected)
            {
                base.Dispose(disposing);
            }
        }
    }
}
