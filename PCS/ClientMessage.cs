using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public class ClientMessage : IDataStream
    {
        public string Text { get; }
        public string ChannelTitle { get; }

        public ClientMessage(string text, string channelTitle)
        {
            Text = text;
            ChannelTitle = channelTitle;
        }

        public string GetPacketData()
        {
            return ChannelTitle + Flags.EndOfText + Text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
