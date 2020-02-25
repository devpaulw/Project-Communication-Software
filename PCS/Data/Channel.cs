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
	}
}