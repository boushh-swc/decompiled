using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Squads.War
{
	public class SquadMemberWarData : ISerializable
	{
		public string SquadMemberName;

		public string SquadMemberId;

		public int VictoryPointsLeft;

		public Map BaseMap;

		public List<SquadDonatedTroop> WarTroops;

		public List<SquadWarRewardData> WarRewards;

		public SquadWarRewardData WarReward
		{
			get
			{
				return (this.WarRewards == null || this.WarRewards.Count <= 0) ? null : this.WarRewards[0];
			}
		}

		public SquadMemberWarData()
		{
			this.WarTroops = new List<SquadDonatedTroop>();
			this.WarRewards = new List<SquadWarRewardData>();
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("id"))
			{
				this.SquadMemberId = Convert.ToString(dictionary["id"]);
			}
			if (dictionary.ContainsKey("name"))
			{
				this.SquadMemberName = Convert.ToString(dictionary["name"]);
			}
			if (dictionary.ContainsKey("victoryPoints"))
			{
				this.VictoryPointsLeft = Convert.ToInt32(dictionary["victoryPoints"]);
			}
			if (dictionary.ContainsKey("warMap") && dictionary["warMap"] != null)
			{
				this.BaseMap = new Map();
				this.BaseMap.FromObject(dictionary["warMap"]);
				this.BaseMap.InitializePlanet();
			}
			if (dictionary.ContainsKey("donatedTroops"))
			{
				this.WarTroops = SquadUtils.GetSquadDonatedTroopsFromObject(dictionary["donatedTroops"]);
			}
			if (dictionary.ContainsKey("rewards"))
			{
				Dictionary<string, object> dictionary2 = dictionary["rewards"] as Dictionary<string, object>;
				if (dictionary2 != null)
				{
					this.WarRewards.Clear();
					foreach (object current in dictionary2.Values)
					{
						SquadWarRewardData squadWarRewardData = new SquadWarRewardData();
						squadWarRewardData.FromObject(current);
						if (squadWarRewardData.ExpireDate > Service.ServerAPI.ServerTime)
						{
							this.WarRewards.Add(squadWarRewardData);
						}
					}
					this.WarRewards.Sort(new Comparison<SquadWarRewardData>(this.CompareExpirationDates));
				}
			}
			return this;
		}

		public bool RemoveSquadWarReward()
		{
			if (this.WarRewards == null || this.WarRewards.Count == 0)
			{
				return false;
			}
			this.WarRewards.RemoveAt(0);
			return true;
		}

		private int CompareExpirationDates(SquadWarRewardData a, SquadWarRewardData b)
		{
			if (b.ExpireDate > a.ExpireDate)
			{
				return 1;
			}
			if (b.ExpireDate < a.ExpireDate)
			{
				return -1;
			}
			return 0;
		}

		public string ToJson()
		{
			return Serializer.Start().End().ToString();
		}
	}
}
