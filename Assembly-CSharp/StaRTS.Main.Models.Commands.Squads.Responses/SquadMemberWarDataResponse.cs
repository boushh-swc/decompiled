using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Squads.Responses
{
	public class SquadMemberWarDataResponse : AbstractResponse
	{
		public SquadMemberWarData MemberWarData
		{
			get;
			private set;
		}

		public Dictionary<string, int> DonatedSquadTroops
		{
			get;
			private set;
		}

		public Dictionary<string, int> Champions
		{
			get;
			private set;
		}

		public List<string> Equipment
		{
			get;
			private set;
		}

		public uint ScoutingStatus
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return this;
			}
			this.MemberWarData = new SquadMemberWarData();
			this.MemberWarData.FromObject(obj);
			if (dictionary.ContainsKey("donatedTroops"))
			{
				this.DonatedSquadTroops = new Dictionary<string, int>();
				Dictionary<string, object> dictionary2 = dictionary["donatedTroops"] as Dictionary<string, object>;
				if (dictionary2 != null)
				{
					foreach (KeyValuePair<string, object> current in dictionary2)
					{
						string key = current.Key;
						int num = 0;
						Dictionary<string, object> dictionary3 = current.Value as Dictionary<string, object>;
						if (dictionary3 != null)
						{
							foreach (KeyValuePair<string, object> current2 in dictionary3)
							{
								num += Convert.ToInt32(current2.Value);
							}
							this.DonatedSquadTroops.Add(key, num);
						}
					}
				}
			}
			if (dictionary.ContainsKey("champions"))
			{
				this.Champions = new Dictionary<string, int>();
				Dictionary<string, object> dictionary4 = dictionary["champions"] as Dictionary<string, object>;
				if (dictionary4 != null)
				{
					foreach (KeyValuePair<string, object> current3 in dictionary4)
					{
						string key2 = current3.Key;
						this.Champions.Add(key2, Convert.ToInt32(current3.Value));
					}
				}
			}
			if (dictionary.ContainsKey("scoutingStatus"))
			{
				object obj2 = dictionary["scoutingStatus"];
				if (obj2 != null)
				{
					Dictionary<string, object> dictionary5 = obj2 as Dictionary<string, object>;
					this.ScoutingStatus = Convert.ToUInt32(dictionary5["code"]);
				}
			}
			if (dictionary.ContainsKey("equipment"))
			{
				this.Equipment = new List<string>();
				List<object> list = dictionary["equipment"] as List<object>;
				if (list != null)
				{
					int i = 0;
					int count = list.Count;
					while (i < count)
					{
						this.Equipment.Add(list[i] as string);
						i++;
					}
				}
			}
			return this;
		}
	}
}
