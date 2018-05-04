using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player.World
{
	public class Map : ISerializable
	{
		private const string PLANET_KEY = "planet";

		private const string NEXT_BUILDING_NUMBER_KEY = "next";

		private string planetUid = string.Empty;

		public List<Building> Buildings
		{
			get;
			set;
		}

		public PlanetVO Planet
		{
			get;
			set;
		}

		public int NextBuildingNumber
		{
			get;
			set;
		}

		public ISerializable FromObject(object obj)
		{
			this.Buildings = new List<Building>();
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return this;
			}
			if (dictionary.ContainsKey("planet"))
			{
				this.planetUid = (dictionary["planet"] as string);
			}
			if (dictionary.ContainsKey("buildings"))
			{
				List<object> list = dictionary["buildings"] as List<object>;
				foreach (object current in list)
				{
					this.Buildings.Add(new Building().FromObject(current) as Building);
				}
			}
			if (dictionary.ContainsKey("next"))
			{
				this.NextBuildingNumber = Convert.ToInt32(dictionary["next"]);
			}
			else
			{
				Service.Logger.Debug("Map does not contain nextBuildingNumber.");
			}
			return this;
		}

		public void GetAllBuildingsWithBaseUid(string baseUId, List<Building> outMatchingBuildings)
		{
			string text = string.Empty;
			for (int i = 0; i < this.Buildings.Count; i++)
			{
				Building building = this.Buildings[i];
				text = building.Uid;
				int indexOfFirstNumericCharacter = StringUtils.GetIndexOfFirstNumericCharacter(text);
				if (indexOfFirstNumericCharacter > 0)
				{
					text = text.Substring(0, indexOfFirstNumericCharacter);
				}
				if (text == baseUId)
				{
					outMatchingBuildings.Add(building);
				}
			}
		}

		public Building GetHighestLevelBuilding(string buildingID)
		{
			int num = -1;
			Building result = null;
			int i = 0;
			int count = this.Buildings.Count;
			while (i < count)
			{
				Building building = this.Buildings[i];
				string text = building.Uid;
				int indexOfFirstNumericCharacter = StringUtils.GetIndexOfFirstNumericCharacter(text);
				int num2 = -1;
				if (indexOfFirstNumericCharacter > 0)
				{
					string s = text.Substring(indexOfFirstNumericCharacter);
					text = text.Substring(0, indexOfFirstNumericCharacter);
					if (int.TryParse(s, out num2) && text == buildingID && num2 > num)
					{
						num = num2;
						result = building;
					}
				}
				i++;
			}
			return result;
		}

		public void ReinitializePlanet(string planet)
		{
			this.planetUid = planet;
			this.InitializePlanet();
		}

		public void InitializePlanet()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			if (!string.IsNullOrEmpty(this.planetUid))
			{
				this.Planet = staticDataController.Get<PlanetVO>(this.planetUid);
			}
			if (this.Planet == null)
			{
				this.Planet = staticDataController.Get<PlanetVO>("planet1");
			}
		}

		public string PlanetId()
		{
			return this.planetUid;
		}

		public string ToJson()
		{
			return Serializer.Start().Add<int>("next", this.NextBuildingNumber).AddString("planet", this.Planet.Uid).AddArray<Building>("buildings", this.Buildings).End().ToString();
		}

		public int FindHighestHqLevel()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			int num = -1;
			int num2 = num;
			if (staticDataController == null || this.Buildings == null)
			{
				return num2;
			}
			int i = 0;
			int count = this.Buildings.Count;
			while (i < count)
			{
				BuildingTypeVO optional = staticDataController.GetOptional<BuildingTypeVO>(this.Buildings[i].Uid);
				if (optional != null && optional.Type == BuildingType.HQ && optional.Lvl >= num2)
				{
					num2 = optional.Lvl;
				}
				i++;
			}
			return num2;
		}

		public bool ScoutTowerExists()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			int i = 0;
			int count = this.Buildings.Count;
			while (i < count)
			{
				BuildingTypeVO optional = staticDataController.GetOptional<BuildingTypeVO>(this.Buildings[i].Uid);
				if (optional != null && optional.Type == BuildingType.ScoutTower)
				{
					return true;
				}
				i++;
			}
			return false;
		}

		public int GetSquadStorageCapacity()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			int i = 0;
			int count = this.Buildings.Count;
			while (i < count)
			{
				BuildingTypeVO optional = staticDataController.GetOptional<BuildingTypeVO>(this.Buildings[i].Uid);
				if (optional != null && optional.Type == BuildingType.Squad)
				{
					return optional.Storage;
				}
				i++;
			}
			return 0;
		}

		public void OnRemoveBuildingFromMap()
		{
			this.NextBuildingNumber--;
		}

		public int GetNextBuildingNumberAndIncrement()
		{
			return this.NextBuildingNumber++;
		}
	}
}
