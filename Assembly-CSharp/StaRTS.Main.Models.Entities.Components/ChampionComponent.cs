using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class ChampionComponent : ComponentBase
	{
		public TroopTypeVO ChampionType
		{
			get;
			private set;
		}

		public ChampionComponent(TroopTypeVO championType)
		{
			this.ChampionType = championType;
		}
	}
}
