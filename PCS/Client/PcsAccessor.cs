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

		public Channel FocusedChannel { get; set; }
		public bool IsConnected { get; private set; }

		public PcsAccessor()
		{
		}

		public void Connect(IPAddress ip, Member member)
		{
			if (ip == null)
				throw new ArgumentNullException(nameof(ip));
			if (member == null)
				throw new ArgumentNullException(nameof(member));

			AdapteeClient = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			var endPoint = new IPEndPoint(ip, PcsServer.Port);

			AdapteeClient.Connect(endPoint);
			Console.WriteLine(Messages.Client.Connected, ip.MapToIPv4());

			ftp = new PcsFtpClient(ip);

			SignIn();

			void SignIn()
			{
				SendPacket(new SignInPacket(member));

				IsConnected = true;
			}
		}

		public void StartListenAsync(Action<BroadcastMessage> messageReceived, Action<Channel> onChannelReceived)
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

						if (receivedPacket is BroadcastMessagePacket)
							messageReceived((receivedPacket as BroadcastMessagePacket).BroadcastMessage);
						// BBTODO: receive channel modifications
						//else if (receivedPacket is ChannelPacket)
						//	onChannelReceived((receivedPacket as ChannelPacket).Channel);
						else
							throw new Exception(Messages.Exceptions.NotRecognizedDataPacket); // DOLATER: Handle better save messages on the PC, not just resources
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
			if (message == null)
				throw new ArgumentNullException(nameof(message));
			if (!IsConnected)
				throw new Exception(Messages.Exceptions.NotConnected);

			SendPacket(new MessagePacket(message));
		}

		public IEnumerable<Channel> GetChannels()
		{
			return ftp.GetChannels();
		}

		public IEnumerable<BroadcastMessage> GetDailyMessages(DateTime day)
		{
			return new List<BroadcastMessage>(ftp.GetDailyMessages(FocusedChannel, day));
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
