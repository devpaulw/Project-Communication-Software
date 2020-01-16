using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PCS.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new PCSClient(IPAddress.Parse("127.0.0.1"));
            
            while (true)
            {
                client.Connect();
                string readLine = Console.ReadLine();
                if (readLine != "quit")
                    client.SendMessage(readLine);
                else break;
            }

            client.Disconnect();
        }
    }
}
