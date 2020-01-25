using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public class ServerMessage : IDataStream
    {
        public Member Author { get; } 
        public string Text { get; } // TODO These variables are similar in many class and should be reghrouped wirty trhg bze 
        public string ChannelTitle { get; }

        public ServerMessage(Member author, string text, string channelTitle)
        {
            Author = author;
            Text = text;
            ChannelTitle = channelTitle;
        }

        public string GetPacketData()
        {
            return ChannelTitle + Flags.EndOfText + Author.GetPacketData() + Flags.EndOfText + Text;
        }
    }
}
