using StaRTS.Main.Models.Entities;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class ShieldGeneratorUpgradeScreen : ShieldGeneratorInfoScreen
	{
		public ShieldGeneratorUpgradeScreen(SmartEntity selectedBuilding) : base(selectedBuilding)
		{
			this.useUpgradeGroup = true;
		}
	}
}
