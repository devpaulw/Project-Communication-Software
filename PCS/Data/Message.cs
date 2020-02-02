using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public abstract class Message
    {
        public string Text { get; set; }
        public string ChannelTitle { get; set; }

        public Message(string text, string channelTitle)
        {
            Text = text;
            ChannelTitle = channelTitle;
        }

        public override string ToString()
        {
            return $"Message in {ChannelTitle}: {Text}";
        }
    }
}
