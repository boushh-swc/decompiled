using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Player
{
	public class VisitNeighborResponse : AbstractResponse
	{
		public Map MapData;

		public Inventory InventoryData;

		public string Name;

		public int AttackRating;

		public int DefenseRating;

		public int AttacksWon;

		public int DefensesWon;

		public Squad Squad;

		public UnlockedLevelData UpgradesData;

		public FactionType Faction;

		public List<SquadDonatedTroop> SquadTroops = new List<SquadDonatedTroop>();

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("player"))
			{
				Dictionary<string, object> dictionary2 = dictionary["player"] as Dictionary<string, object>;
				this.Name = (dictionary2["name"] as string);
				if (dictionary2.ContainsKey("scalars"))
				{
					Dictionary<string, object> dictionary3 = dictionary2["scalars"] as Dictionary<string, object>;
					if (dictionary3.ContainsKey("attackRating"))
					{
						this.AttackRating = Convert.ToInt32(dictionary3["attackRating"]);
					}
					if (dictionary3.ContainsKey("defenseRating"))
					{
						this.DefenseRating = Convert.ToInt32(dictionary3["defenseRating"]);
					}
					if (dictionary3.ContainsKey("attacksWon"))
					{
						this.AttacksWon = Convert.ToInt32(dictionary3["attacksWon"]);
					}
					if (dictionary3.ContainsKey("defensesWon"))
					{
						this.DefensesWon = Convert.ToInt32(dictionary3["defensesWon"]);
					}
				}
				if (dictionary2.ContainsKey("playerModel"))
				{
					Dictionary<string, object> dictionary4 = dictionary2["playerModel"] as Dictionary<string, object>;
					if (dictionary4.ContainsKey("map"))
					{
						this.MapData = new Map();
						this.MapData.FromObject(dictionary4["map"]);
						this.MapData.InitializePlanet();
					}
					Dictionary<string, object> dictionary5 = dictionary4["inventory"] as Dictionary<string, object>;
					if (dictionary5.ContainsKey("capacity") && dictionary5["capacity"] != null)
					{
						this.InventoryData = new Inventory();
						this.InventoryData.FromObject(dictionary4["inventory"]);
					}
					this.Faction = StringUtils.ParseEnum<FactionType>(dictionary4["faction"].ToString());
					if (dictionary4.ContainsKey("guildInfo"))
					{
						Dictionary<string, object> dictionary6 = dictionary4["guildInfo"] as Dictionary<string, object>;
						if (dictionary6 != null && dictionary6.ContainsKey("guildId"))
						{
							string squadID = dictionary6["guildId"] as string;
							this.Squad = Service.LeaderboardController.GetOrCreateSquad(squadID);
							this.Squad.FromVisitNeighborObject(dictionary6);
						}
					}
					if (dictionary4.ContainsKey("donatedTroops"))
					{
						this.SquadTroops = SquadUtils.GetSquadDonatedTroopsFromObject(dictionary4["donatedTroops"]);
					}
					if (dictionary4.ContainsKey("upgrades"))
					{
						this.UpgradesData = new UnlockedLevelData();
						this.UpgradesData.FromObject(dictionary4["upgrades"]);
					}
				}
				return this;
			}
			Service.Logger.Error("Unable to parse response for neighbor.visit");
			return null;
		}
	}
}
