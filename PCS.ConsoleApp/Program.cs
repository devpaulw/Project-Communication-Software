using System;
using System.Collections.Generic;
using System.IO;
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
            var request = WebRequest.Create("ftp://192.168.0.25:6784") as FtpWebRequest;
            //var request = WebRequest.Create("ftp://127.0.0.1/") as FtpWebRequest;

            request.Credentials = new NetworkCredential("anonymous", "paul@paul.paul");
            //request.Credentials = new NetworkCredential("paul", "lol");

            ShowContent(request);
            UploadFile(request, @"C:\Users\BluePaul\Documents\tt1.txt");
            ShowContent(request);
            DownloadFile(WebRequest.Create(@"ftp://192.168.0.25:6784/tt1.txt") as FtpWebRequest);

            //// SDNMSG: The console app is no longer maintened. Use the WPF interface instead.

            //Console.WriteLine("What is your username?");
            //string username = Console.ReadLine();

            //using (var client = new PcsClient()) // IDisposable using
            //{
            //    client.Connect(IPAddress.Parse("192.168.1.11"));
            //    client.SignIn(new Member(username, new Random().Next(0, 255)));

            //    while (true)
            //    {
            //        string readLine = Console.ReadLine();

            //        if (readLine == "quit")
            //            break;

            //        client.SendClientMessage(readLine);

            //        //Console.WriteLine("Echoed: {0}", client.ReceiveEchoMessage());
            //    }
            //}
        }

        static void ShowContent(FtpWebRequest request)
        {
            request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            var response = request.GetResponse() as FtpWebResponse;

            var responseStream = response.GetResponseStream();
            var reader = new StreamReader(responseStream);

            Console.WriteLine(reader.ReadToEnd());

            Console.WriteLine($"Directory List Complete, status {response.StatusDescription}");

            reader.Close();
            response.Close();
        }

        static void UploadFile(FtpWebRequest request, string path)
        {
        //    WebClient client = new WebClient();
        //    client.Credentials = new NetworkCredential("anonymous", "paul@paul.paul");
        //    client.UploadFile(
        //        "ftp://192.168.0.25:6784/tt1.txt", @"C:\Users\BluePaul\Documents\tt1.txt");

        //    return;

            request.Method = WebRequestMethods.Ftp.UploadFile;

            byte[] fileContents;
            
            using (var sourceStream = new StreamReader(path))
            {
                fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            }

            request.ContentLength = fileContents.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(fileContents, 0, fileContents.Length);
            }

            using (var response = request.GetResponse() as FtpWebResponse)
            {
                Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
            }
        }

        static void DownloadFile(FtpWebRequest request)
        {
            request.Method = WebRequestMethods.Ftp.DownloadFile;

            var response = request.GetResponse() as FtpWebResponse;

            var responseStream = response.GetResponseStream();
            var reader = new StreamReader(responseStream);

            Console.WriteLine(reader.ReadToEnd()); 
            
            Console.WriteLine($"Download Complete, status {response.StatusDescription}");

            reader.Close();
            response.Close();
        }
    }
}
