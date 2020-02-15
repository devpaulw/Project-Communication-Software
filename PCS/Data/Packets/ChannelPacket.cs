using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
	class ChannelPacket : Packet
	{
		public Channel Channel { get; private set; }

		public ChannelPacket(Channel channel) : base(Flags.Channel)
		{
			Channel = channel;
		}

		protected override string[] GetAttributes()
		{
			return new string[] {
				Channel.Name
			};
		}

		protected Channel Unpack(string[] attributes)
		{
			return new Channel(attributes[0]);
		}
	}
}
