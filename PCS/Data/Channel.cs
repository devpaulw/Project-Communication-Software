using System;

namespace PCS
{
	public class Channel
	{
		public string Name { get; set; }
		public int Id { get; set; }

		public Channel(int id, string name)
		{
			Id = id;
			Name = name;
		}

		public override bool Equals(object other)
		{
			return other is Channel channel
				&& Equals(channel);
		}

		private bool Equals(Channel channel)
		{
			return Id == channel.Id;
		}

		public override int GetHashCode()
		{
			return Id;
		}
	}
}