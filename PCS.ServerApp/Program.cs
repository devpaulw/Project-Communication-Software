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
            using (var server = new PcsServer())
            {
                server.HostClients();
            }
        }
    }
}
