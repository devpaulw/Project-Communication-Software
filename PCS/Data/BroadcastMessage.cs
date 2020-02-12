using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace PCS
{
    public class BroadcastMessage
    {
        public Message Message { get; set; }
        public Member Author { get; set; }
        public DateTime DateTime { get; set; }

        public BroadcastMessage(Message message, DateTime dateTime, Member author)
        {
            Message = message;
            Author = author;
            DateTime = dateTime;
        }

        public string ToFileMessage()
        {
            const char endOfTB = (char)23;
            return Message.ChannelName + endOfTB
                + Author.Username + endOfTB
                + Author.ID + endOfTB
                + DateTime.ToFileTime() + endOfTB
                + Message.Text;
        }
        // DOLATER: When It will be C# 8.0, implement interface with static functions: IDataObject and discontinue DataPacket(Attributes)/(Objects)
        public static BroadcastMessage FromFileMessage(string fileMessage)
        {
            if (fileMessage == null)
                throw new ArgumentNullException(nameof(fileMessage));

            const char endOfTB = (char)23;
            string[] infos = fileMessage.Split(endOfTB);

            return new BroadcastMessage(
                new Message(infos[4],
                infos[0]),
                DateTime.FromFileTime(Convert.ToInt64(infos[3], CultureInfo.InvariantCulture)),
                new Member(infos[1], 
                Convert.ToInt32(infos[2], CultureInfo.InvariantCulture))
                );
        }

        public override string ToString()
        {
            return $"Message from {Author} in {Message.ChannelName} at {DateTime}: {Message.Text}";
        }
    }
}
