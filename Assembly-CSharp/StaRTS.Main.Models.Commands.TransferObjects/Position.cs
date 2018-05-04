using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.TransferObjects
{
	public class Position : ISerializable
	{
		public int X
		{
			get;
			set;
		}

		public int Z
		{
			get;
			set;
		}

		public string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.Add<int>("x", this.X);
			serializer.Add<int>("z", this.Z);
			return serializer.End().ToString();
		}

		public ISerializable FromObject(object obj)
		{
			return null;
		}
	}
}
