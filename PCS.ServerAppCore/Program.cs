using System;
using System.Net;

namespace PCS.ServerAppCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("SERVER ADDRESS: ");
            var serverAddress = IPAddress.Parse(Console.ReadLine());

            using (var ftpServer = new PcsFtpServer(serverAddress))
            {
                ftpServer.StartAsync();

                var server = new PcsServer(serverAddress);
                server.StartHosting();
            }
        }
    }
}
