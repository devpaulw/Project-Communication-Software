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
        static void Main()
        {
            Console.WriteLine("What is your username?");
            string username = Console.ReadLine();

            using (var client = new PCSClient(IPAddress.Parse("127.0.0.1"))) // idisposable using
            {
                client.Connect();
                client.SignIn(new Member(username, new Random().Next(0, 255)));

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
