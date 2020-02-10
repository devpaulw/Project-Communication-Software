using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace PCS
{
    public class Message
    {
        public string Text { get; set; }
        public string ChannelName { get; set; }
        public Member Author { get; set; }
        public DateTime DateTime { get; set; }

        public Message(string text, string channelName, DateTime dateTime, Member author)
        {
            Text = text;
            ChannelName = channelName;
            Author = author;
            DateTime = dateTime;

            if (HasEmptyField) 
                throw new MessageEmptyFieldException(Messages.Exceptions.MessageEmptyField);
        }

        public bool HasEmptyField
            => string.IsNullOrEmpty(Text) || string.IsNullOrEmpty(ChannelName);

        public string ToFileMessage()
        {
            const char endOfTB = (char)23;
            return ChannelName + endOfTB
                + Author.Username + endOfTB
                + Author.ID + endOfTB
                + DateTime.ToFileTime() + endOfTB
                + Text;
        }

        public static Message FromFileMessage(string fileMessage)
        {
            if (fileMessage == null)
                throw new ArgumentNullException(nameof(fileMessage));

            const char endOfTB = (char)23;
            string[] infos = fileMessage.Split(endOfTB);

            return new Message(
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
