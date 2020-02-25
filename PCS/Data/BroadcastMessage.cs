using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace PCS.Data
{
    public class BroadcastMessage
    {
        public int ID { get; set; }
        public Message BaseMessage { get; set; } // TODO Discontinue this idea
        public Member Author { get; set; }
        public DateTime DateTime { get; set; }

        public BroadcastMessage(int id, Message baseMessage, DateTime dateTime, Member author)
        {
            ID = id;
            BaseMessage = baseMessage;
            Author = author ?? Member.Unknown;
            DateTime = dateTime;
        }

        public string ToFileMessage()
        {
            const char endOfTB = (char)23;
            return BaseMessage.ChannelName + endOfTB
                + Author.Username + endOfTB
                + Author.ID + endOfTB
                + DateTime.ToFileTime() + endOfTB
                + BaseMessage.Text;
        }

        // DOLATER: When there will be C# 8.0, implement interface with static functions: IDataObject and discontinue DataPacket(Attributes)/(Objects)
        public static BroadcastMessage FromFileMessage(string fileMessage)
        {
            if (fileMessage == null)
                throw new ArgumentNullException(nameof(fileMessage));

            const char endOfTB = (char)23;
            string[] infos = fileMessage.Split(endOfTB);

            return new BroadcastMessage(
                0, // TODO Supply
                new Message(infos[4],
                infos[0]),
                DateTime.FromFileTime(Convert.ToInt64(infos[3], CultureInfo.InvariantCulture)),
                new Member(infos[1], 
                Convert.ToInt32(infos[2], CultureInfo.InvariantCulture))
                );
        }

        public override string ToString()
        {
            return $"Message from {Author} in {BaseMessage.ChannelName} at {DateTime}: {BaseMessage.Text}";
        }
    }
}
