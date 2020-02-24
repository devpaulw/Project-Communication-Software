using System;
using System.Net;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;
using System.IO;

namespace PCS.ServerAppCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("P.C.S. Server Core - Version 0.3");

            SqlTest();

            Console.Write("SERVER ADDRESS: ");
            var serverAddress = IPAddress.Parse(Console.ReadLine());

            using var ftpServer = new PcsFtpServer(serverAddress);
            ftpServer.StartAsync();

            var server = new PcsServer(serverAddress);
            server.StartHosting();
        }

        static void SqlTest()
        {
            string rl;
            do rl = Console.ReadLine();
            while (rl != "2+2");

            // Create a database
            // Create a table
            // Fill a Value Of Key 2+2 and result 4
            // Read it and write 4

            SqlConnection myConn = new SqlConnection("Server=localhost;Integrated security=SSPI;database=master");

            SqlCommand myCommand = new SqlCommand();
            myCommand.Connection = myConn;


            myConn.Open();
            myCommand.ExecuteNonQuery();

            myConn.Close();
            myConn.ConnectionString = "server =; Initial Catalog=MyDatabase1; Integrated Security = SSPI";
            myConn.Open();

            myCommand.CommandText = @"CREATE TABLE [dbo].[TableTest]
                    (
	                    [Expression] NCHAR(10) NOT NULL PRIMARY KEY, 
                        [Result] INT NULL
                    )";
            myCommand.ExecuteNonQuery();

            myCommand.CommandText = "INSERT INTO TableTest (Expression, Result)" +
                    "VALUES ('2+2', 4)";

            myCommand.ExecuteNonQuery();

            myCommand.CommandText = "SELECT * FROM TableTest";

            using (DbDataReader dataReader = myCommand.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    Console.WriteLine($"{dataReader["Result"]}");
                }
            }

            return;

            string connStr = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"D:\\Fichiers personnels\\Documents\\[]Projects\\Project Communication Software\\PCS.ServerAppCore\\Database1.mdf\";Integrated Security=True";

            using (var connection = new SqlConnection())
            {
                if (connection == null)
                {
                    Console.WriteLine("connect error");
                    Console.ReadLine();
                    return;
                }

                connection.ConnectionString = connStr;

                connection.Open();

                SqlCommand command = new SqlCommand();

                if (command == null)
                {
                    Console.WriteLine("Command error");
                    Console.ReadLine();
                    return;
                }

                command.Connection = connection;
                command.CommandText = "Select * From Members";

                using (DbDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Console.WriteLine($"Then {dataReader["Id"]} {dataReader["Username"]}");
                        Console.ReadLine();
                    }
                }

                command.CommandText = "INSERT INTO Members (Id, Username)" +
                    "Values (366, 'FromC#')";

                command.ExecuteNonQuery();
            }
        }
    }
}
