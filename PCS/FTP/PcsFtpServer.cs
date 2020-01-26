﻿using FubarDev.FtpServer;
using FubarDev.FtpServer.AccountManagement;
using FubarDev.FtpServer.FileSystem.DotNet;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace PCS
{
    public class PcsFtpServer : IDisposable
    {
        private static readonly string directory = Path.Combine(Path.GetTempPath(), "PcsFtpServer");

        private readonly IFtpServerHost serverHost;

        public const ushort Port = 6784;

        public PcsFtpServer()
        {
            // Setup dependency injection
            var services = new ServiceCollection();

            // use %TEMP%/TestFtpServer as root folder
            services.Configure<DotNetFileSystemOptions>(opt => opt
                .RootPath = directory);

            // Add FTP server services
            // DotNetFileSystemProvider = Use the .NET file system functionality
            // AnonymousMembershipProvider = allow only anonymous logins
            services.AddFtpServer(builder => builder
                .UseDotNetFileSystem() // Use the .NET file system functionality
                .EnableAnonymousAuthentication()); // allow anonymous logins

            // Configure the FTP server
            services.Configure<FtpServerOptions>(opt => opt.ServerAddress = IPAddressHelper.GetLocalIPAddress().ToString());
            services.Configure<FtpServerOptions>(opt => opt.Port = Port);
            
            //var cert = new X509Certificate2("my.pfx", "my-super-strong-password-that-nobody-knows");
            //services.Configure<AuthTlsOptions>(cfg => cfg.);

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Initialize the FTP server
            serverHost = serviceProvider.GetRequiredService<IFtpServerHost>();
        }

        public void StartAsync()
        {
            // Start the FTP server
            serverHost.StartAsync(CancellationToken.None).Wait();
        }

        public void Dispose()
        {
            // Stop the FTP server
            serverHost.StopAsync(CancellationToken.None).Wait();
        }
    }
}