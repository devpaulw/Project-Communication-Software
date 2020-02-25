using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using PCS.Data;

namespace PCS.Sql
{
    class MemberTable : Table<Member>
    {
        protected override string Name => "Members";

        public bool MemberExists(int memberId)
        {
            string cmdTxt = $"SELECT * FROM {Name} WHERE Id = {memberId}";

            using (SqlConnection conn = new SqlConnection($"server=;Initial Catalog = {Database.Name};Integrated security=SSPI"))
            using (var cmd = new SqlCommand(cmdTxt, conn))
            {
                conn.Open();
                using (DbDataReader dataReader = cmd.ExecuteReader())
                    return dataReader.Read(); // If the reader never reads, return false and vice versa
            }
        }

        public Member GetMemberFromId(int id)
        {
            string cmdTxt = $"SELECT * FROM {Name} WHERE Id = {id}";

            using (SqlConnection conn = new SqlConnection($"server=;Initial Catalog = {Database.Name};Integrated security=SSPI"))
            using (var cmd = new SqlCommand(cmdTxt, conn))
            {
                conn.Open();
                using (DbDataReader dataReader = cmd.ExecuteReader())
                {
                    if (dataReader.Read())
                        return new Member(dataReader["Username"] as string, id);
                    else return null;
                }
            }
        }

        protected override Member GetObject(Dictionary<string, object> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return new Member(
                values["Username"] as string ?? throw new NullReferenceException(),
                values["Id"] as int? ?? throw new NullReferenceException());
        }

        protected override SqlParameter[] GetParameters() => new[]
            {
                new SqlParameter() { ParameterName = "Id", SqlDbType = SqlDbType.Int, IsNullable = false },
                new SqlParameter() { ParameterName = "Username", SqlDbType = SqlDbType.NVarChar, Size = 32, IsNullable = false },
            };

        protected override Dictionary<string, object> GetValues(Member member)
        {
            if (member == null)
                throw new ArgumentNullException(nameof(member));
            
            return new Dictionary<string, object>()
            {
                { "Id", member.ID },
                { "Username", member.Username }
            };
        }
    }
}
