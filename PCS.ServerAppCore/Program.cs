using System;
using System.Net;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data;

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

            string str;
            SqlConnection myConn = new SqlConnection("server=;Integrated security=SSPI");

            str = "CREATE DATABASE MyDatabase5 ON PRIMARY" +
                "(NAME = MyDatabase5_Data, " +
                "FILENAME = 'MyDatabase5.mdf', " +
                "SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
                "LOG ON (NAME = MyDatabase_Log," +
                "FILENAME = 'MyDatabase5.ldf'," +
                "SIZE = 1MB, " +
                "MAXSIZE = 5MB, " +
                "FILEGROWTH = 10%)";

            SqlCommand myCommand = new SqlCommand(str, myConn);

            try
            {
                myConn.Open();
                myCommand.ExecuteNonQuery();
                Console.WriteLine("DataBase is Created Successfully");
            }
            catch (System.Exception ex)
            {
                throw;
            }
            finally
            {
                if (myConn.State == ConnectionState.Open)
                    myConn.Close();
            }

            myConn = new SqlConnection("server =.; Initial Catalog = MyDatabase5; Integrated Security = SSPI");

            str = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Student' AND xtype='U')
                            CREATE TABLE [dbo].[Student](
	                        [id] [int] IDENTITY(1,1) NOT NULL PRIMARY KEY,
	                        [FirstName] [nchar](10) NULL,
	                        [LastName] [nchar](10) NULL,
                            )";

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
