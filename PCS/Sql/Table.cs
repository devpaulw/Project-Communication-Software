using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Data.Common;

namespace PCS.Sql
{
    internal abstract class Table<T>
    {
        protected abstract string Name { get; }

        public Table()
        {
            Database.TryCreate();
            TryCreate(); // The table
        }

        public void AddRow(T rowObj)
        {
            var values = GetValues(rowObj);

            string cmdText = string.Format(CultureInfo.InvariantCulture, "INSERT INTO {0} ({1}) Values ({2})",
                Name,
                string.Join(",", values.Keys),
                string.Join(",", from value in values select '@' + value.Key ?? throw new NullReferenceException())
                );

            using (SqlConnection conn = new SqlConnection($"server=;Initial Catalog = {Database.Name};Integrated security=SSPI"))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;

                foreach (var value in values)
                    cmd.Parameters.AddWithValue('@' + value.Key, value.Value ?? throw new NullReferenceException());

                cmd.CommandText = cmdText;

                conn.Open();

                cmd.ExecuteNonQuery();
            }
        }

        protected abstract Dictionary<string, object> GetValues(T item);
        protected abstract T GetObject(Dictionary<string, object> values);
        protected abstract SqlParameter[] GetParameters();
        protected SqlParameter KeyParameter
            => GetParameters()[0];

        private void TryCreate()
        {
            string cmdTxt = $"IF OBJECT_ID('{Name}') IS NULL " +
                                $"CREATE TABLE [dbo].[{Name}](";

            var parameters = GetParameters();
            int i = 0;
            foreach (var parameter in parameters)
            {
                cmdTxt += string.Format(CultureInfo.InvariantCulture,
                    "[{0}] {1} {2} {3}",
                    parameter.ParameterName,
                    parameter.SqlDbType.ToString().ToUpper(CultureInfo.InvariantCulture) + (parameter.Size != 0 ? "(" + parameter.Size + ")" : ""),
                    parameter.IsNullable ? "NULL" : "NOT NULL",
                    i == 0 ? "PRIMARY KEY" : ""); // TODO Change Key should be separated

                i++;
                if (i != parameters.Length)
                    cmdTxt += ',';
            }
            cmdTxt += ")";

            using (SqlConnection conn = new SqlConnection($"server=;Initial Catalog = {Database.Name};Integrated security=SSPI"))
            using (var cmd = new SqlCommand(cmdTxt, conn))
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        protected static class Database
        {
            public const string Name = "PcsDatabase";

            public static void TryCreate()
            {
                const int dataSize = 2,
                    logSize = 1;
                const int dataMaxSize = 4000,
                    logMaxSize = 2000;
                string todayStr = string.Concat(DateTime.Today.Day.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'), DateTime.Today.Month.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'), DateTime.Today.Year.ToString(CultureInfo.InvariantCulture)).PadLeft(4, '0');
                
                string cmdTxt = $"IF DB_ID('{Name}') IS NULL " +
                    $"CREATE DATABASE {Name} ON PRIMARY" +
                    $"(NAME = {Name}_Data, " +
                    $"FILENAME = '{Path.GetFullPath(Name)}_{todayStr}.mdf', " +
                    $"SIZE = {dataSize}MB, MAXSIZE = {dataMaxSize}MB, FILEGROWTH = 10%) " +
                    $"LOG ON (NAME = {Name}_Log," +
                    $"FILENAME = '{Path.GetFullPath(Name)}_{todayStr}.ldf'," +
                    $"SIZE = {logSize}MB, " +
                    $"MAXSIZE = {logMaxSize}MB, " +
                    "FILEGROWTH = 10%)"; // DOLATER: Make the limit never buggy

                using (SqlConnection conn = new SqlConnection("server=;Integrated security=SSPI"))
                using (var cmd = new SqlCommand(cmdTxt, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
