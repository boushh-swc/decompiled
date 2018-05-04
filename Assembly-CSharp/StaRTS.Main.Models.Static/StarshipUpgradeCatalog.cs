using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Static
{
	public class StarshipUpgradeCatalog : GenericUpgradeCatalog<SpecialAttackTypeVO>
	{
		public int MaxSpecialAttackDps
		{
			get;
			private set;
		}

		protected override void InitService()
		{
			FactionType faction = Service.CurrentPlayer.Faction;
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (SpecialAttackTypeVO current in staticDataController.GetAll<SpecialAttackTypeVO>())
			{
				if (current.PlayerFacing && current.Faction == faction)
				{
					if (current.DPS > this.MaxSpecialAttackDps)
					{
						this.MaxSpecialAttackDps = current.DPS;
					}
				}
			}
			Service.StarshipUpgradeCatalog = this;
		}
	}
}
