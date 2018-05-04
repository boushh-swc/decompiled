using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models
{
	public class HudConfig
	{
		private List<string> elements;

		public HudConfig(params string[] list)
		{
			this.elements = new List<string>();
			for (int i = 0; i < list.Length; i++)
			{
				string item = list[i];
				this.elements.Add(item);
			}
		}

		public void Add(string elementName)
		{
			if (!this.elements.Contains(elementName))
			{
				this.elements.Add(elementName);
			}
		}

		public void Remove(string elementName)
		{
			if (this.elements.Contains(elementName))
			{
				this.elements.Remove(elementName);
			}
		}

		public bool Has(string elementName)
		{
			return this.elements.Contains(elementName);
		}
	}
}
