using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Battle;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class TeamComponent : ComponentBase
	{
		public TeamType TeamType
		{
			get;
			protected set;
		}

		public TeamComponent(TeamType teamType)
		{
			this.TeamType = teamType;
		}

		public bool CanDestructBuildings()
		{
			return this.TeamType == TeamType.Attacker;
		}

		public bool IsDefender()
		{
			return this.TeamType == TeamType.Defender;
		}
	}
}
