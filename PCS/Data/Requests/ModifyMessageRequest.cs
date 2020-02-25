using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS.Data
{
    class ModifyMessageRequest : Request
    {
        public int MessageId { get; set; }
        public SendableMessage NewMessage { get; set; }

        public ModifyMessageRequest(int messageId, SendableMessage newMessage) : base(RequestCode.ModifyMessage)
        {
            MessageId = messageId;
            NewMessage = newMessage ?? throw new ArgumentNullException(nameof(newMessage));
        }

        internal override string[] GetAdditionalAttributes()
        {
            return new[]
                {
                    MessageId.ToString(CultureInfo.InvariantCulture),
                    NewMessage.ChannelName,
                    NewMessage.Text
                };
        }
    }
}
