using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS.Data.Packets
{
    class BroadcastMessagePacket : Packet<BroadcastMessage>
    {
        public BroadcastMessagePacket(BroadcastMessage message) : base(message, Flags.BroadcastMessage)
        {
        }

        protected override string[] GetAttributes()
        {
            return new string[] {
                 Item.BaseMessage.ChannelName,
                 Item.Author.Username,
                 Item.Author.ID.ToString(CultureInfo.CurrentCulture),
                 Item.DateTime.ToFileTime().ToString(CultureInfo.CurrentCulture),
                 Item.BaseMessage.Text
            };
        }
    }
}
