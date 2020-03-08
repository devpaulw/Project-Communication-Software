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
        public string Text { get; set; }
        public Channel Channel { get; set; }
        public Member Author { get; set; }
        public DateTime DateTime { get; set; }

        public BroadcastMessage(int iD, string text, Channel channel, DateTime dateTime, Member author)
        {
            ID = iD;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Channel = channel ?? throw new ArgumentNullException(nameof(channel));
            Author = author ?? Member.Unknown;
            DateTime = dateTime;
        }

        [Obsolete("Not using FTP anymore")]
        public string ToFileMessage()
        {
            const char endOfTB = (char)23;
            return Channel.Name + endOfTB
                + Author.Username + endOfTB
                + Author.ID + endOfTB
                + DateTime.ToFileTime() + endOfTB
                + Text;
        }

        [Obsolete("Not using FTP anymore")]
        public static BroadcastMessage FromFileMessage(string fileMessage)
        {
            if (fileMessage == null)
                throw new ArgumentNullException(nameof(fileMessage));

            const char endOfTB = (char)23;
            string[] infos = fileMessage.Split(endOfTB);

            return new BroadcastMessage(
                0,
                infos[4],
                new Channel(infos[0]),
                DateTime.FromFileTime(Convert.ToInt64(infos[3], CultureInfo.InvariantCulture)),
                new Member(infos[1], 
                Convert.ToInt32(infos[2], CultureInfo.InvariantCulture))
                );
        }

        public override string ToString()
        {
            return $"Message from {Author} in {Channel.Name} at {DateTime}: {Text}";
        }
    }
}
