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

        internal override string[] GetAttributes()
        {
            return new[]
                {
                    MessageId.ToString(CultureInfo.InvariantCulture),
                    NewMessage.ChannelName,
                    NewMessage.Text
                };
        }

        public static Request FromAttributes(string[] attributes)
        {
            return new ModifyMessageRequest(Convert.ToInt32(attributes[0], CultureInfo.InvariantCulture),
                        new SendableMessage(attributes[2], attributes[1]));
        }
    }
}
