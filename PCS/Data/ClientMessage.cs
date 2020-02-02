using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
    public class ClientMessage : Message
    {
        public ClientMessage(string text, string channelTitle) : base(text, channelTitle)
        {
            Text = text;
            ChannelTitle = channelTitle;
        }
    }
}
