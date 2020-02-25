using FubarDev.FtpServer;
using FubarDev.FtpServer.AccountManagement;
using FubarDev.FtpServer.FileSystem.DotNet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace PCS.Ftp
{
    public class PcsFtpServer : IDisposable
    {
        private bool disposed;
        private readonly IFtpServerHost serverHost;

        public const string Directory = @"./ftp_server/";
        public const string MessagePath = "messages"; // TODO: It's to the server to do that
        public const string ResourcePath = "resources";
        public const ushort Port = 6784;

        public PcsFtpServer(IPAddress serverAddress)
        {
            // Setup dependency injection
            var services = new ServiceCollection();

            // use %TEMP%/TestFtpServer as root folder
            services.Configure<DotNetFileSystemOptions>(opt => opt
                .RootPath = Directory);
            
            // Add FTP server services
            // DotNetFileSystemProvider = Use the .NET file system functionality
            // AnonymousMembershipProvider = allow only anonymous logins
            services.AddFtpServer(builder => builder
                .UseDotNetFileSystem() // Use the .NET file system functionality
                .EnableAnonymousAuthentication()); // allow anonymous logins

            // Configure the FTP server
            services.Configure<FtpServerOptions>(opt => opt.ServerAddress = serverAddress.ToString());
            services.Configure<FtpServerOptions>(opt => opt.Port = Port);

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Initialize the FTP server
            serverHost = serviceProvider.GetRequiredService<IFtpServerHost>();
        }

        public void StartAsync()
        {
            // Start the FTP server
            serverHost.StartAsync(CancellationToken.None).Wait();

            Console.WriteLine(Messages.Server.FtpStarted, DateTime.Now);

            ///**tmp */
            //var ftpClient = new PcsFtpClient();
            //ftpClient.Connect(IPAddressHelper.GetLocalIPAddress());
            //ftpClient.SaveMessage(new Message("Salut, je suis un test ça va", "general", DateTime.Now, new Member("Paul", 365)));
            //foreach (Message m in ftpClient.GetDailyMessages(DateTime.Now))
            //{
            //    Console.WriteLine(m);
            //}
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
                    // Stop the FTP server
                    serverHost.StopAsync(CancellationToken.None).Wait();
                }

                disposed = true;
            }
        }
    }
}
