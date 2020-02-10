using System;
using System.Net;

namespace PCS.ServerAppCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("P.C.S. Server Core - Version 0.3");

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
