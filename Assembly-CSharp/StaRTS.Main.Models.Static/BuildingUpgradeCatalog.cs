using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Static
{
	public class BuildingUpgradeCatalog : GenericUpgradeCatalog<BuildingTypeVO>
	{
		protected override void InitService()
		{
			Service.BuildingUpgradeCatalog = this;
		}
	}
}
