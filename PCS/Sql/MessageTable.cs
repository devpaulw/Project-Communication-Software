using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace PCS.Sql
{
    public class MessageTable : Table<BroadcastMessage>
    {
        protected override string Name => "Messages";

        protected override Dictionary<string, object> GetValues(BroadcastMessage broadcast)
        {
            if (broadcast == null)
                throw new ArgumentNullException(nameof(broadcast));

            return new Dictionary<string, object>()
            {
                { "Id", broadcast.ID },
                { "Text", broadcast.BaseMessage.Text },
                { "Channel", broadcast.BaseMessage.ChannelName },
                { "AuthorId", broadcast.Author.ID },
                { "DateTime", broadcast.DateTime }
            };
        }

        protected override SqlParameter[] GetParameters()
            => new[]
            {
                new SqlParameter() { ParameterName = "Id", SqlDbType = SqlDbType.Int, IsNullable = false },
                new SqlParameter() { ParameterName = "Text", SqlDbType = SqlDbType.NText, IsNullable = false },
                new SqlParameter() { ParameterName = "Channel", SqlDbType = SqlDbType.NChar, Size = 50, IsNullable = false },
                new SqlParameter() { ParameterName = "AuthorId", SqlDbType = SqlDbType.Int, IsNullable = false },
                new SqlParameter() { ParameterName = "DateTime", SqlDbType = SqlDbType.DateTime, IsNullable = false }
            };
    }
}
