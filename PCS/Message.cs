using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public class Message
    {
        public Member Author { get; set; } 
        public string Text { get; set; }
        public string ChannelTitle { get; set; }
        public DateTime DateTime { get; set; }

        public bool IsForClient { get; }

        public Message(string text, string channelTitle, DateTime dateTime, Member author)
        {
            DateTime = dateTime;
            Author = author;
            Text = text;
            ChannelTitle = channelTitle;

            IsForClient = true;
        }

        public Message(string text, string channelTitle)
        {
            Text = text;
            ChannelTitle = channelTitle;

            DateTime = DateTime.Now;
            Author = null;

            IsForClient = false;
        }

        public override string ToString()
        {
            return $"Message from {Author} in {ChannelTitle} at {DateTime}: {Text}";
        }
    }
}
