using System;

namespace StaRTS.Utils.Json
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
	public class JsonPropertyAttribute : Attribute
	{
		public string Name
		{
			get;
			set;
		}

		public JsonPropertyAttribute(string name)
		{
			this.Name = name;
		}
	}
}
