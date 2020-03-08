using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
	public class Channel
	{
		public string Name { get; private set; }

		public Channel(string name)
		{
			Name = name;
		}

		public override bool Equals(object obj)
		{
			return obj is Channel channel &&
				   Name == channel.Name;
		}

		public override int GetHashCode()
		{
			return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
		}
	}
}
