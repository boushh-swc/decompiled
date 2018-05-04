using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player.Misc
{
	public class Campaign : AbstractTimedEvent
	{
		public bool Completed
		{
			get;
			set;
		}

		public float TimeZone
		{
			get;
			set;
		}

		public uint Points
		{
			get;
			set;
		}

		public Dictionary<string, int> Purchases
		{
			get;
			set;
		}

		public override ISerializable FromObject(object obj)
		{
			base.FromObject(obj);
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.Completed = (bool)dictionary["completed"];
			this.TimeZone = Convert.ToSingle(dictionary["timeZone"]);
			if (dictionary.ContainsKey("points"))
			{
				this.Points = Convert.ToUInt32(dictionary["points"]);
			}
			else
			{
				this.Points = 0u;
			}
			if (dictionary.ContainsKey("items"))
			{
				Dictionary<string, object> dictionary2 = dictionary["items"] as Dictionary<string, object>;
				if (dictionary2 != null)
				{
					this.Purchases = new Dictionary<string, int>();
					foreach (KeyValuePair<string, object> current in dictionary2)
					{
						this.Purchases.Add(current.Key, Convert.ToInt32(current.Value));
					}
				}
			}
			return this;
		}
	}
}
