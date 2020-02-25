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
				Channel.Id + "",
				Channel.Name
			};
		}
	}
}
