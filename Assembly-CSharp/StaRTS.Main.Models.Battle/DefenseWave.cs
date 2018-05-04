using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Battle
{
	public class DefenseWave
	{
		public float Delay
		{
			get;
			private set;
		}

		public DefenseEncounterVO Encounter
		{
			get;
			private set;
		}

		public List<Entity> Troops
		{
			get;
			private set;
		}

		public DefenseWave(string defenseEncounter, float delay)
		{
			this.Delay = delay;
			StaticDataController staticDataController = Service.StaticDataController;
			this.Encounter = staticDataController.GetOptional<DefenseEncounterVO>(defenseEncounter);
			this.Troops = new List<Entity>();
			if (this.Encounter == null)
			{
				Service.Logger.ErrorFormat("Defense Encounter {0} not found", new object[]
				{
					defenseEncounter
				});
			}
		}
	}
}
