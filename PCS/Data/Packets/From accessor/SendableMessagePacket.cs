using System;
using System.Collections.Generic;
using System.Text;

namespace PCS.Data.Packets
{
    class SendableMessagePacket : Packet<SendableMessage>
    {
        public SendableMessagePacket(SendableMessage message) : base(message, Flags.SendableMessage) { }

        public override string[] GetAttributes()
        {
            return new string[] {
                 Item.ChannelName,
                 Item.Text
            };
        }

        public static Packet FromAttributes(string[] attributes)
        {
            string channelTitle = attributes[0];
            string text = attributes[1];

            return new SendableMessagePacket(new SendableMessage(text, channelTitle));
        }
    }
}
