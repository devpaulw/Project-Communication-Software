using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using PCS.Data;

namespace PCS.Sql
{
    class MessageTable : Table<BroadcastMessage>
    {
        protected override string Name => "Messages";

        public int GetNewID()
        { // DOLATER: this is working but not fine
            var topMessage = GetTopMessagesInRange(0, 1, null);
            return topMessage.Any() ? topMessage.First().ID + 1 : 0;
        }

        public IEnumerable<BroadcastMessage> GetTopMessagesInRange(int start, int end, string channelName)
        {
            string cmdTxt = $"SELECT TOP {start + end} * FROM {Name} {(channelName != null ? $"WHERE Channel = '{channelName}'" : "")} ORDER BY {KeyParameter.ParameterName} DESC";

            using (SqlConnection conn = new SqlConnection($"server=;Initial Catalog = {Database.Name};Integrated security=SSPI"))
            using (var cmd = new SqlCommand(cmdTxt, conn))
            {
                conn.Open();
                using (DbDataReader dataReader = cmd.ExecuteReader())
                {
                    for (int i = 0; i < start; i++)
                        dataReader.Read(); // DOLATER Optimize because it can be slow when we try to go far

                    for (int i = start; dataReader.Read() && i < end; i++)
                    {
                        var parameters = GetParameters();
                        var values = new Dictionary<string, object>();

                        foreach (var parameter in parameters)
                            values.Add(parameter.ParameterName, dataReader[parameter.ParameterName]);

                        yield return GetObject(values);
                    }
                }
            }
        }

        protected override Dictionary<string, object> GetValues(BroadcastMessage broadcast)
        {
            if (broadcast == null)
                throw new ArgumentNullException(nameof(broadcast));

            return new Dictionary<string, object>()
            {
                { "Id", broadcast.ID },
                { "Text", broadcast.Text },
                { "Channel", broadcast.ChannelName },
                { "AuthorId", broadcast.Author.ID },
                { "DateTime", broadcast.DateTime }
            };
        }

        protected override SqlParameter[] GetParameters()
            => new[]
            {
                new SqlParameter() { ParameterName = "Id", SqlDbType = SqlDbType.Int, IsNullable = false },
                new SqlParameter() { ParameterName = "Text", SqlDbType = SqlDbType.NText, IsNullable = false },
                new SqlParameter() { ParameterName = "Channel", SqlDbType = SqlDbType.NVarChar, Size = 32, IsNullable = false },
                new SqlParameter() { ParameterName = "AuthorId", SqlDbType = SqlDbType.Int, IsNullable = false },
                new SqlParameter() { ParameterName = "DateTime", SqlDbType = SqlDbType.DateTime, IsNullable = false }
            };

        protected override BroadcastMessage GetObject(Dictionary<string, object> values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            return new BroadcastMessage(
                values["Id"] as int? ?? throw new NullReferenceException(),
                values["Text"] as string ?? throw new NullReferenceException(), 
                values["Channel"] as string ?? throw new NullReferenceException(),
                values["DateTime"] as DateTime? ?? throw new NullReferenceException(),
                new MemberTable().GetMemberFromId(values["AuthorId"] as int? ?? throw new NullReferenceException())
                );
        }
    }
}
