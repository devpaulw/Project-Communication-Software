using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
	static class ChannelGenerator
	{
		private static int id = 0;

		public static Channel Generate(string name)
		{
			id++;
			return new Channel(id, name);
		}
	}
}
