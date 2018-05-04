using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaRTS.Main.Models.Player.World
{
	public class Building : ISerializable
	{
		public const string BUILDING_ID_PREFIX = "bld_";

		public string Key;

		public int X;

		public int Z;

		public string Uid;

		public uint LastCollectTime;

		public int CurrentStorage;

		public int AccruedCurrency;

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.Key = (dictionary["key"] as string);
			this.X = Convert.ToInt32(dictionary["x"]);
			this.Z = Convert.ToInt32(dictionary["z"]);
			this.Uid = (dictionary["uid"] as string);
			if (dictionary.ContainsKey("lastCollectTime"))
			{
				this.LastCollectTime = Convert.ToUInt32(dictionary["lastCollectTime"]);
			}
			if (dictionary.ContainsKey("currentStorage"))
			{
				this.CurrentStorage = Convert.ToInt32(dictionary["currentStorage"]);
			}
			return this;
		}

		public string ToJson()
		{
			Serializer serializer = Serializer.Start().AddString("key", this.Key).Add<int>("x", this.X).Add<int>("z", this.Z).AddString("uid", this.Uid).Add<int>("currentStorage", this.CurrentStorage);
			return serializer.End().ToString();
		}

		public static Building FromBuildingTypeVO(BuildingTypeVO buildingType)
		{
			Building building = new Building();
			building.X = 0;
			building.Z = 0;
			building.Uid = buildingType.Uid;
			building.Key = "bld_" + Service.CurrentPlayer.Map.GetNextBuildingNumberAndIncrement();
			if (buildingType.Type == BuildingType.Trap)
			{
				building.CurrentStorage = 1;
			}
			return building;
		}

		public void SyncWithTransform(TransformComponent transform)
		{
			this.X = Units.BoardToGridX(transform.X);
			this.Z = Units.BoardToGridZ(transform.Z);
		}

		public void AddString(StringBuilder sb, string uidOverride, uint timeOverride)
		{
			this.AddString(sb, uidOverride, timeOverride, this.X, this.Z);
		}

		public void AddString(StringBuilder sb, string uidOverride, uint timeOverride, int manualX, int manualZ)
		{
			string value = (uidOverride == null) ? this.Uid : uidOverride;
			uint value2 = (timeOverride == 0u) ? this.LastCollectTime : timeOverride;
			sb.Append(this.Key).Append("|").Append(manualX).Append("|").Append(manualZ).Append("|").Append(value).Append("|").Append(value2).Append("|").Append(this.CurrentStorage).Append("|").Append("\n");
		}

		public Building Clone()
		{
			return (Building)base.MemberwiseClone();
		}
	}
}
