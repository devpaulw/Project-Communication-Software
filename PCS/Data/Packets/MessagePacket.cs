using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    class MessagePacket : Packet
    {
        public Message Message { get; set; }

        public MessagePacket(Message message) : base(Flags.Message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        protected override string[] GetAttributes()
        {
            return new string[] {
                 Message.ChannelName,
                 Message.Text
            };
        }
    }
}
