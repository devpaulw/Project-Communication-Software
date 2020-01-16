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
            var server = new PCSServer();
            server.HostClients();
        }
    }
}
