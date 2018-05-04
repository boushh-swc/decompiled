using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player
{
	public class ActiveArmory : ISerializable
	{
		public List<string> Equipment
		{
			get;
			private set;
		}

		public int MaxCapacity
		{
			get;
			private set;
		}

		public string ToJson()
		{
			return "{}";
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return this;
			}
			if (dictionary.ContainsKey("equipment"))
			{
				List<object> list = dictionary["equipment"] as List<object>;
				if (list != null)
				{
					this.Equipment = new List<string>();
					int i = 0;
					int count = list.Count;
					while (i < count)
					{
						this.Equipment.Add(list[i] as string);
						i++;
					}
				}
			}
			if (dictionary.ContainsKey("capacity"))
			{
				this.MaxCapacity = Convert.ToInt32(dictionary["capacity"]);
			}
			return this;
		}

		public void SetMaxEquipmentCapacity(int capacity)
		{
			this.MaxCapacity = capacity;
		}
	}
}
