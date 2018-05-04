using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.TransferObjects
{
	public class PositionMap : ISerializable
	{
		private Dictionary<string, Position> positions;

		public PositionMap()
		{
			this.positions = new Dictionary<string, Position>();
		}

		public Position GetPosition(string id)
		{
			if (this.positions.ContainsKey(id))
			{
				return this.positions[id];
			}
			return null;
		}

		public void AddPosition(string id, Position pos)
		{
			this.positions.Add(id, pos);
		}

		public void ClearAllPositions()
		{
			this.positions.Clear();
		}

		public string ToJson()
		{
			Serializer serializer = Serializer.Start();
			foreach (string current in this.positions.Keys)
			{
				serializer.AddObject<Position>(current, this.positions[current]);
			}
			return serializer.End().ToString();
		}

		public ISerializable FromObject(object obj)
		{
			return null;
		}
	}
}
