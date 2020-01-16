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
            using (var client = new PCSClient(IPAddress.Parse("127.0.0.1"))) // idisposable using
            {
                client.Connect();

                while (true)
                {
                    string readLine = Console.ReadLine();

                    if (readLine == "quit")
                        break;

                    client.SendMessage(readLine);

                    Console.WriteLine("Echoed: {0}", client.ReceiveEchoMessage());
                }
            }
        }
    }
}
