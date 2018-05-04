using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class BuildingComponent : AssetComponent
	{
		public const string SPARK_FX_ID = "effect203";

		public BuildingTypeVO BuildingType
		{
			get;
			set;
		}

		public Building BuildingTO
		{
			get;
			set;
		}

		public BuildingComponent(BuildingTypeVO buildingType, Building buildingTO) : base(buildingType.AssetName)
		{
			this.BuildingType = buildingType;
			this.BuildingTO = buildingTO;
		}
	}
}
