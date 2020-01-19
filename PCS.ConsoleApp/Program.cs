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
            // SDNMSG: The console app is no longer maintened. Use the WPF interface instead.

            Console.WriteLine("What is your username?");
            string username = Console.ReadLine();

            using (var client = new PcsClient()) // IDisposable using
            {
                client.Connect(IPAddress.Parse("192.168.1.11"));
                client.SignIn(new Member(username, new Random().Next(0, 255)));

                while (true)
                {
                    string readLine = Console.ReadLine();

                    if (readLine == "quit")
                        break;

                    client.SendText(readLine);

                    //Console.WriteLine("Echoed: {0}", client.ReceiveEchoMessage());
                }
            }
        }
    }
}
