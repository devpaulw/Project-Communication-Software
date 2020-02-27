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

        public override string[] GetAttributes()
        {
            return new string[] {
                 Item.BaseMessage.ChannelName,
                 Item.Author.Username,
                 Item.Author.ID.ToString(CultureInfo.CurrentCulture),
                 Item.DateTime.ToFileTime().ToString(CultureInfo.CurrentCulture),
                 Item.BaseMessage.Text
            };
        }

        public static BroadcastMessagePacket FromAttributes(string[] attributes)
        {
            string channelTitle = attributes[0];

            string username = attributes[1];
            int id = Convert.ToInt32(attributes[2], CultureInfo.CurrentCulture);
            var dateTime = DateTime.FromFileTime(Convert.ToInt64(attributes[3], CultureInfo.CurrentCulture));
            string text = attributes[4];

            var author = new Member(username, id);

            return new BroadcastMessagePacket(new BroadcastMessage(0, new SendableMessage(text, channelTitle), dateTime, author)); // TODO Supply
        }
    }
}
