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
        private readonly PcsListener listener;
        private readonly List<PcsClient> connectedClients;

        public const ushort Port = 6783;
        public const ushort FtpPort = 6784;
        public static readonly Encoding Encoding = Encoding.Unicode;

        public PcsServer()
        {
            listener = new PcsListener(IPAddressHelper.GetLocalIPAddress());
            connectedClients = new List<PcsClient>();
        }
        
        public void HostClients()
        {
            try
            {
                listener.Listen();

                Console.WriteLine("Server started.");

                while (true)
                {
                    var client = listener.Accept();
                    connectedClients.Add(client);

                    var connectionThread = new Thread(() => new ConnectionHandler(client, OnMessageReceived, OnClientDisconnect));
                    connectionThread.Start();
                }
            }
            catch
            {
                throw;
            }
        }

        public void StartFtp()
        {
            // Setup dependency injection
            var services = new ServiceCollection();

            // use %TEMP%/TestFtpServer as root folder
            services.Configure<DotNetFileSystemOptions>(opt => opt
                .RootPath = Path.Combine(Path.GetTempPath(), "PcsFtpServer"));

            // Add FTP server services
            // DotNetFileSystemProvider = Use the .NET file system functionality
            // AnonymousMembershipProvider = allow only anonymous logins
            services.AddFtpServer(builder => builder
                .UseDotNetFileSystem() // Use the .NET file system functionality
                .EnableAnonymousAuthentication()); // allow anonymous logins

            // Configure the FTP server
            services.Configure<FtpServerOptions>(opt => opt.ServerAddress = IPAddressHelper.GetLocalIPAddress().ToString());
            services.Configure<FtpServerOptions>(opt => opt.Port = FtpPort + 1);

            // Build the service provider
            using (var serviceProvider = services.BuildServiceProvider())
            {
                // Initialize the FTP server
                var ftpServerHost = serviceProvider.GetRequiredService<IFtpServerHost>();

                // Start the FTP server
                ftpServerHost.StartAsync(CancellationToken.None).Wait();

                Console.WriteLine("The P.C.S. FTP Server started.");

                // Stop the FTP server
                // -> ftpServerHost.StopAsync(CancellationToken.None).Wait();
            }
        }

        private void OnMessageReceived(Message message)
        {
            SendToEveryone(message);
            SaveMessage(message);
        }

        private void OnClientDisconnect(PcsClient client)
        {
            connectedClients.Remove(client);
        }

        private void SendToEveryone(Message message)
        {
            // Send to all clients
            foreach (var connectedClient in connectedClients)
                connectedClient.SendMessage(message);
        }

        private void SaveMessage(Message message)
        {
            // Will be filled later
        }

        public void Dispose()
        {
            // Example: Save messages, properly disconnect people...
        }
    }
}
