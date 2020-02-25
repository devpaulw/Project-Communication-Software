using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PCS.Data.Packets
{
    class BroadcastMessagePacket : Packet // TODO MAYBE Do something so that we don't have to do same class both
    {
        public BroadcastMessage BroadcastMessage { get; set; }

        public BroadcastMessagePacket(BroadcastMessage message) : base(Flags.BroadcastMessage)
        {
            BroadcastMessage = message;
        }

        protected override string[] GetAttributes()
        {
            return new string[] {
                 BroadcastMessage.BaseMessage.ChannelName,
                 BroadcastMessage.Author.Username,
                 BroadcastMessage.Author.ID.ToString(CultureInfo.CurrentCulture),
                 BroadcastMessage.DateTime.ToFileTime().ToString(CultureInfo.CurrentCulture),
                 BroadcastMessage.BaseMessage.Text
            };
        }
    }
}
