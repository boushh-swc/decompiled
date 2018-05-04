using StaRTS.Main.Models.Player.World;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Cee.Serializables
{
	public class CombatEncounter : ISerializable
	{
		private const string MAP_KEY = "map";

		public Map map;

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.map = (new Map().FromObject(dictionary["map"]) as Map);
			this.map.InitializePlanet();
			return this;
		}

		public string ToJson()
		{
			return Serializer.Start().AddObject<Map>("map", this.map).End().ToString();
		}
	}
}
