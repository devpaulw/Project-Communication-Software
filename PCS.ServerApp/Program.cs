using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCS.ServerApp
{
    class Program
    {
        static void Main()
        {
            using (var ftpServer = new PcsFtpServer())
            using (var server = new PcsServer())
            {
                ftpServer.StartAsync();
                server.StartHosting();
            }
        }
    }
}
