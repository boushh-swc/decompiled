using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Static
{
	public class EquipmentUpgradeCatalog : GenericUpgradeCatalog<EquipmentVO>
	{
		protected override void InitService()
		{
			Service.EquipmentUpgradeCatalog = this;
		}
	}
}
