using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models
{
	public class CrateData : ISerializable
	{
		public string UId
		{
			get;
			private set;
		}

		public string CrateId
		{
			get;
			private set;
		}

		public string Context
		{
			get;
			private set;
		}

		public string PlanetId
		{
			get;
			private set;
		}

		public int HQLevel
		{
			get;
			private set;
		}

		public uint ReceivedTimeStamp
		{
			get;
			private set;
		}

		public uint ExpiresTimeStamp
		{
			get;
			private set;
		}

		public bool DoesExpire
		{
			get;
			private set;
		}

		public bool Claimed
		{
			get;
			set;
		}

		public List<SupplyData> ResolvedSupplies
		{
			get;
			private set;
		}

		public string WarId
		{
			get;
			private set;
		}

		public string GuildId
		{
			get;
			private set;
		}

		public CrateData()
		{
			this.Claimed = false;
			this.DoesExpire = false;
		}

		public string ToJson()
		{
			return "{}";
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				if (dictionary.ContainsKey("uid"))
				{
					this.UId = Convert.ToString(dictionary["uid"]);
				}
				if (dictionary.ContainsKey("crateId"))
				{
					this.CrateId = Convert.ToString(dictionary["crateId"]);
				}
				if (dictionary.ContainsKey("context"))
				{
					this.Context = Convert.ToString(dictionary["context"]);
				}
				if (dictionary.ContainsKey("planet"))
				{
					this.PlanetId = Convert.ToString(dictionary["planet"]);
				}
				if (dictionary.ContainsKey("hqLevel"))
				{
					this.HQLevel = Convert.ToInt32(dictionary["hqLevel"]);
				}
				if (dictionary.ContainsKey("received"))
				{
					this.ReceivedTimeStamp = Convert.ToUInt32(dictionary["received"]);
				}
				if (dictionary.ContainsKey("expires"))
				{
					this.ExpiresTimeStamp = Convert.ToUInt32(dictionary["expires"]);
					if (this.ExpiresTimeStamp == 0u)
					{
						this.DoesExpire = false;
					}
					else
					{
						this.DoesExpire = true;
					}
				}
				if (dictionary.ContainsKey("warId"))
				{
					this.WarId = Convert.ToString(dictionary["warId"]);
				}
				if (dictionary.ContainsKey("guildId"))
				{
					this.GuildId = Convert.ToString(dictionary["guildId"]);
				}
				this.ResolvedSupplies = new List<SupplyData>();
				if (dictionary.ContainsKey("resolvedSupplies"))
				{
					List<object> list = dictionary["resolvedSupplies"] as List<object>;
					int i = 0;
					int count = list.Count;
					while (i < count)
					{
						SupplyData supplyData = new SupplyData();
						supplyData.FromObject(list[i]);
						this.ResolvedSupplies.Add(supplyData);
						i++;
					}
				}
			}
			return this;
		}
	}
}
