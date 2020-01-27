using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public class Message
    {
        public Member Author { get; } 
        public string Text { get; }
        public string ChannelTitle { get; }
        public DateTime DateTime { get; }

        public Message(string text, string channelTitle, DateTime dateTime, Member author)
        {
            DateTime = dateTime;
            Author = author;
            Text = text;
            ChannelTitle = channelTitle;
        }

        public Message(string text, string channelTitle)
        {
            Text = text;
            ChannelTitle = channelTitle;

            DateTime = DateTime.Now;
            Author = null;
        }
    }
}
