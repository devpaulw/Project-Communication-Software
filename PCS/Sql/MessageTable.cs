using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace PCS.Sql
{
    class MessageTable : Table<BroadcastMessage>
    {
        protected override string Name => "Messages";

        public int GetNewID()
        {
            return GetLastRowsInRange(0, 1).First().ID + 1;
        }

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
                new Message(values["Text"] as string ?? throw new NullReferenceException(), 
                values["Channel"] as string ?? throw new NullReferenceException()),
                values["DateTime"] as DateTime? ?? throw new NullReferenceException(),
                new Member((values["AuthorId"] as int? ?? throw new NullReferenceException()).ToString(), 
                values["AuthorId"] as int? ?? throw new NullReferenceException()));
        }
    }
}
