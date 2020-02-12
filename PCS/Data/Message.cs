using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public class Message
    {
        public string Text { get; set; }
        public string ChannelName { get; set; }

        public Message(string text, string channelName)
        {
            Text = text;
            ChannelName = channelName;

            if (HasEmptyField)
                throw new MessageEmptyFieldException(Messages.Exceptions.MessageEmptyField);
        }

        public bool HasEmptyField
            => string.IsNullOrEmpty(Text) || string.IsNullOrEmpty(ChannelName);
    }
}
