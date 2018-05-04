using System;

namespace StaRTS.Main.Models
{
	public struct AddOnMapping
	{
		public string Parent;

		public string Model;

		public AddOnMapping(string parent, string model)
		{
			this.Parent = parent;
			this.Model = model;
		}
	}
}
