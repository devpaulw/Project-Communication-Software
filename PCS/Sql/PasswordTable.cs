using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using PCS.Data;

namespace PCS.Sql
{
    class PasswordTable : Table<AuthenticationInfos>
    {
        protected override string Name => "MemberPasswords";

        public bool PasswordCorrect(AuthenticationInfos authenticationInfos)
        {
            string cmdTxt = $"SELECT * FROM {Name} WHERE MemberId = {authenticationInfos.MemberId}";

            using (SqlConnection conn = new SqlConnection($"server=;Initial Catalog = {Database.Name};Integrated security=SSPI"))
            using (var cmd = new SqlCommand(cmdTxt, conn))
            {
                conn.Open();
                using (DbDataReader dataReader = cmd.ExecuteReader())
                {
                    dataReader.Read();
                    return (string)dataReader["Password"] == authenticationInfos.Password;
                }
            }
        }

        protected override AuthenticationInfos GetObject(Dictionary<string, object> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return new AuthenticationInfos(
                values["MemberId"] as int? ?? throw new NullReferenceException(),
                values["Password"] as string ?? throw new NullReferenceException());
        }

        protected override SqlParameter[] GetParameters() => new[]
            {
                new SqlParameter() { ParameterName = "MemberId", SqlDbType = SqlDbType.Int, IsNullable = false },
                new SqlParameter() { ParameterName = "Password", SqlDbType = SqlDbType.NVarChar, Size = 64, IsNullable = false }
            };

        protected override Dictionary<string, object> GetValues(AuthenticationInfos authenticationInfos)
        {
            if (authenticationInfos == null)
                throw new ArgumentNullException(nameof(authenticationInfos));

            return new Dictionary<string, object>()
            {
                { "MemberId", authenticationInfos.MemberId },
                { "Password", authenticationInfos.Password }
            };
        }
    }
}