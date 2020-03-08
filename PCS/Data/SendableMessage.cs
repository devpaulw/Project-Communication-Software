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
        public Channel Channel { get; set; }

        public SendableMessage(Channel channel, string text)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }
    }
}
