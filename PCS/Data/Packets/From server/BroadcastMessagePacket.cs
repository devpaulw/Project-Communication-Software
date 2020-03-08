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
                 Item.ID.ToString(CultureInfo.InvariantCulture),
                 Item.Channel.Name,
                 Item.Author.Username,
                 Item.Author.ID.ToString(CultureInfo.CurrentCulture),
                 Item.DateTime.ToFileTime().ToString(CultureInfo.CurrentCulture),
                 Item.Text
            };
        }

        public static BroadcastMessagePacket FromAttributes(string[] attributes)
        {
            int id = int.Parse(attributes[0], CultureInfo.InvariantCulture);
            var channel = new Channel(attributes[1]);
            string memberUsername = attributes[2];
            int memberId = Convert.ToInt32(attributes[3], CultureInfo.CurrentCulture);
            var dateTime = DateTime.FromFileTime(Convert.ToInt64(attributes[4], CultureInfo.CurrentCulture));
            string text = attributes[5];

            var author = new Member(memberUsername, memberId);

            return new BroadcastMessagePacket(new BroadcastMessage(id, text, channel, dateTime, author));
        }
    }
}
