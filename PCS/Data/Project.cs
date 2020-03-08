using System;
using System.Collections.Generic;
using System.Text;

namespace PCS
{
	class Project
	{
		public string Name { get; private set; }

		public Project(string name)
		{
			Name = name;
		}
	}
}
