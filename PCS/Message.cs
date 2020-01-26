using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public class Message : IDataStream // Deprecate IDataStream, and put every thing in the same class 
    {
        public Member Author { get; } 
        public string Text { get; }
        public string ChannelTitle { get; }
        public DateTime DateTime { get; }

        public Message(string text, string channelTitle, DateTime dateTime, Member author = null)
        {
            DateTime = dateTime;
            Author = author;
            Text = text;
            ChannelTitle = channelTitle;
        }

        public string GetPacketData()
        {
            if (Author != null) return ChannelTitle + Flags.EndOfText + Author.GetPacketData() + Flags.EndOfText + Text;
            else return ChannelTitle + Flags.EndOfText + Text;
        }
    }
}
