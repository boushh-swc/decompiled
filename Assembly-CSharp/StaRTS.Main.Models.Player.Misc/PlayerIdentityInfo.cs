using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.Player.Misc
{
	public class PlayerIdentityInfo : ISerializable
	{
		public string PlayerId
		{
			get;
			private set;
		}

		public string PlayerName
		{
			get;
			private set;
		}

		public int HQLevel
		{
			get;
			private set;
		}

		public int Medals
		{
			get;
			private set;
		}

		public string SquadName
		{
			get;
			private set;
		}

		public FactionType Faction
		{
			get;
			private set;
		}

		public bool ActiveIdentity
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
			if (dictionary.ContainsKey("playerId"))
			{
				this.PlayerId = (dictionary["playerId"] as string);
			}
			if (dictionary.ContainsKey("name"))
			{
				this.PlayerName = (dictionary["name"] as string);
			}
			if (dictionary.ContainsKey("scalars"))
			{
				Dictionary<string, object> dictionary2 = dictionary["scalars"] as Dictionary<string, object>;
				int attackRating = 0;
				int defenseRating = 0;
				if (dictionary2 != null)
				{
					if (dictionary2.ContainsKey("attackRating"))
					{
						attackRating = Convert.ToInt32(dictionary2["attackRating"]);
					}
					if (dictionary2.ContainsKey("defenseRating"))
					{
						defenseRating = Convert.ToInt32(dictionary2["defenseRating"]);
					}
				}
				this.Medals = GameUtils.CalcuateMedals(attackRating, defenseRating);
			}
			if (dictionary.ContainsKey("playerModel"))
			{
				Dictionary<string, object> dictionary3 = dictionary["playerModel"] as Dictionary<string, object>;
				if (dictionary3 != null)
				{
					if (dictionary3.ContainsKey("faction"))
					{
						this.Faction = StringUtils.ParseEnum<FactionType>(dictionary3["faction"].ToString());
					}
					if (dictionary3.ContainsKey("guildInfo"))
					{
						Dictionary<string, object> dictionary4 = dictionary3["guildInfo"] as Dictionary<string, object>;
						if (dictionary4 != null && dictionary4.ContainsKey("guildName"))
						{
							this.SquadName = WWW.UnEscapeURL(dictionary4["guildName"] as string);
						}
					}
					if (dictionary3.ContainsKey("map"))
					{
						Dictionary<string, object> dictionary5 = dictionary3["map"] as Dictionary<string, object>;
						if (dictionary5 != null && dictionary5.ContainsKey("buildings"))
						{
							List<object> list = dictionary5["buildings"] as List<object>;
							if (list != null)
							{
								StaticDataController staticDataController = Service.StaticDataController;
								int i = 0;
								int count = list.Count;
								while (i < count)
								{
									Dictionary<string, object> dictionary6 = list[i] as Dictionary<string, object>;
									if (dictionary6 != null && dictionary6.ContainsKey("uid"))
									{
										BuildingTypeVO buildingTypeVO = staticDataController.Get<BuildingTypeVO>(dictionary6["uid"] as string);
										if (buildingTypeVO.Type == BuildingType.HQ)
										{
											this.HQLevel = buildingTypeVO.Lvl;
											break;
										}
									}
									i++;
								}
							}
						}
					}
					this.ActiveIdentity = true;
					if (dictionary3.ContainsKey("identitySwitchTimes"))
					{
						Dictionary<string, object> dictionary7 = dictionary3["identitySwitchTimes"] as Dictionary<string, object>;
						if (dictionary7 != null && dictionary7.ContainsKey(this.PlayerId))
						{
							this.ActiveIdentity = (Convert.ToInt32(dictionary7[this.PlayerId]) == 0);
						}
					}
				}
			}
			return this;
		}
	}
}
