using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS.Data
{
    class DeleteMessageRequest : Request
    {
        public int MessageId { get; set; }

        public DeleteMessageRequest(int messageId) : base(RequestCode.DeleteMessage)
        {
            MessageId = messageId;
        }

        internal override string[] GetAdditionalAttributes()
        {
            return new[]
                {
                    MessageId.ToString(CultureInfo.InvariantCulture)
                };
        }
    }
}
