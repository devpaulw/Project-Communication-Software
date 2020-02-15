using FubarDev.FtpServer;
using FubarDev.FtpServer.FileSystem.DotNet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCS
{
	public class PcsServer : IDisposable
	{
		private bool disposed;

		private readonly PcsListener listener;
		private readonly PcsFtpClient ftpClient;
		private readonly List<PcsClient> connectedClients;
		private readonly object @lock = new object();
		private readonly List<Channel> channels;

		public const ushort Port = 6783;
		public static readonly Encoding Encoding = Encoding.UTF8;


		public PcsServer(IPAddress serverAddress)
		{
			if (serverAddress == null)
				throw new ArgumentNullException(nameof(serverAddress));

			listener = new PcsListener(serverAddress);
			ftpClient = new PcsFtpClient(serverAddress);
			connectedClients = new List<PcsClient>();
			channels = new List<Channel>();

			// TEMP
			channels.Add(new Channel("channel1"));
			channels.Add(new Channel("channel2"));
			channels.Add(new Channel("channel3"));
		}

		public void StartHosting()
		{
			try
			{
				listener.Listen();

				Console.WriteLine(Messages.Server.Started, DateTime.Now);

				while (true)
				{
					PcsClient client = listener.Accept();

					//foreach (var channel in channels)
						//client.SendPacket(new ChannelPacket(channel));

					connectedClients.Add(client);

					var connectionThread = new Thread(()
						=> new ClientConnectionHandler(client,
							OnMemberSignIn, OnMessageReceived, OnClientDisconnect));

					connectionThread.Start();
				}
			}
			catch
			{
				throw;
			}
		}

		private void OnMessageReceived(BroadcastMessage message)
		{
			if (message == null)
				throw new ArgumentNullException(nameof(message));

			lock (@lock) // Prevent that two thread don't call this function at the same time
			{
				Console.WriteLine("Received: " + message);

				SendToEveryone();
				SaveMessage();
			}

			void SendToEveryone()
			{
				// Send to all clients
				foreach (var connectedClient in connectedClients)
					connectedClient.SendPacket(new BroadcastMessagePacket(message));
			}

			void SaveMessage()
			{
				ftpClient.SaveMessage(message);
			}
		}

		private void OnMemberSignIn(Member member)
		{
			lock (@lock)
			{
				Console.WriteLine(Messages.Server.ClientConnect, member);
			}
		}

		private void OnClientDisconnect(PcsClient client, Member member)
		{
			lock (@lock)
			{
				connectedClients.Remove(client);

				Console.WriteLine(Messages.Server.ClientDisconnect, member);
			}
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
					listener.Dispose();
					ftpClient.Dispose();
				}

				disposed = true;
			}
		}
	}
}