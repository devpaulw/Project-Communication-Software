using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;

namespace PCS.Data
{
    public class BroadcastMessage
    {
        public int ID { get; set; }
        public string Text { get; set; }
        public string ChannelName { get; set; }
        public Member Author { get; set; }
        public DateTime DateTime { get; set; }
        public List<File> Files { get; set; }

        public BroadcastMessage(int iD, string text, string channelName, DateTime dateTime, Member author, List<File> files = null)
        {
            ID = iD;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            ChannelName = channelName ?? throw new ArgumentNullException(nameof(channelName));
            Author = author ?? Member.Unknown;
            DateTime = dateTime;
            Files = files;
        }

        [Obsolete("Not using FTP anymore for messages")]
        public string ToFileMessage()
        {
            const char endOfTB = (char)23;
            return ChannelName + endOfTB
                + Author.Username + endOfTB
                + Author.ID + endOfTB
                + DateTime.ToFileTime() + endOfTB
                + Text;
        }

        [Obsolete("Not using FTP anymore for messages")]
        public static BroadcastMessage FromFileMessage(string fileMessage)
        {
            if (fileMessage == null)
                throw new ArgumentNullException(nameof(fileMessage));

            const char endOfTB = (char)23;
            string[] infos = fileMessage.Split(endOfTB);

            return new BroadcastMessage(
                0,
                infos[4],
                infos[0],
                DateTime.FromFileTime(Convert.ToInt64(infos[3], CultureInfo.InvariantCulture)),
                new Member(infos[1], 
                Convert.ToInt32(infos[2], CultureInfo.InvariantCulture))
                );
        }

        public override string ToString()
        {
            return $"Message from {Author} in {ChannelName} at {DateTime}: {Text}";
        }
    }
}
