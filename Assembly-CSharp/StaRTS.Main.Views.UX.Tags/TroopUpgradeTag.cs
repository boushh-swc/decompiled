using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Views.UX.Tags
{
	public class TroopUpgradeTag
	{
		public IDeployableVO Troop;

		public bool IsMaxLevel;

		public bool ReqMet;

		public string RequirementText;

		public string ShortRequirementText;

		public TroopUpgradeTag(IDeployableVO troop, bool reqMet)
		{
			this.Troop = troop;
			this.ReqMet = reqMet;
		}
	}
}
