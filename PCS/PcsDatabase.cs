using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    static class PcsDatabase
    {
        public const string Name = "PcsDatabase";

        public static void TryCreate()
        {
            try
            {
                myCommand.CommandText = "CREATE DATABASE PcsDatabase ON PRIMARY" +
                    "(NAME = PcsDatabase_Data, " +
                    $"FILENAME = '{Path.GetFullPath("MyDatabase1.mdf")}', " +
                    "SIZE = 2MB, MAXSIZE = 10MB, FILEGROWTH = 10%) " +
                    "LOG ON (NAME = MyDatabase1_Log," +
                    $"FILENAME = '{Path.GetFullPath("MyDatabase1.ldf")}'," +
                    "SIZE = 1MB, " +
                    "MAXSIZE = 5MB, " +
                    "FILEGROWTH = 10%)";
            }
        }
    }
}
