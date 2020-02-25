using System;
using System.Collections.Generic;
using System.Text;

namespace PCS.Data.Packets
{
    class MessagePacket : Packet<SendableMessage>
    {
        public MessagePacket(SendableMessage message) : base(message, Flags.SendableMessage) { }

        protected override string[] GetAttributes()
        {
            return new string[] {
                 Item.ChannelName,
                 Item.Text
            };
        }

        //DOLATER: find a way to put GetItem here
    }
}
