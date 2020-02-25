using PCS.Data.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PCS.Data
{
    public class SendableMessage
    {
        public string Text { get; set; }
        public string ChannelName { get; set; }

        public SendableMessage(string text, string channelName)
        {
            Text = text;
            ChannelName = channelName;

            if (HasEmptyField)
                throw new Exception(Messages.Exceptions.MessageEmptyField);
        }

        public bool HasEmptyField
            => string.IsNullOrEmpty(Text) || string.IsNullOrEmpty(ChannelName);
    }
}
