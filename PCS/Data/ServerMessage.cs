using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public class ServerMessage : Message
    {
        public Member Author { get; set; }
        public DateTime DateTime { get; set; }

        public ServerMessage(string text, string channelTitle, DateTime dateTime, Member author) : base(text, channelTitle)
        {
            Author = author;
            DateTime = dateTime;
        }

        public override string ToString()
        {
            return $"Message from {Author} in {ChannelTitle} at {DateTime}: {Text}";
        }
    }
}
