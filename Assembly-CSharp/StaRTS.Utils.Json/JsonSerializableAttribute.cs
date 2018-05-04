using System;

namespace StaRTS.Utils.Json
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class JsonSerializableAttribute : Attribute
	{
		public MappingType Type
		{
			get;
			set;
		}

		public JsonSerializableAttribute(MappingType type)
		{
			this.Type = type;
		}

		public JsonSerializableAttribute() : this(MappingType.Properties)
		{
		}
	}
}
