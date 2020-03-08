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
                 Item.Channel.Name,
                 Item.Text
            };
        }

        public static Packet FromAttributes(string[] attributes)
        {
            var channel = new Channel(attributes[0]);
            string text = attributes[1];

            return new SendableMessagePacket(new SendableMessage(channel, text));
        }
    }
}
