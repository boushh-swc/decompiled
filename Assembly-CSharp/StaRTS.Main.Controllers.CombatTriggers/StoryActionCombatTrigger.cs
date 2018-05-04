using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Story;
using System;

namespace StaRTS.Main.Controllers.CombatTriggers
{
	public class StoryActionCombatTrigger : ICombatTrigger
	{
		private const uint EFFECTIVELY_NEVER = 1800000u;

		private string storyAction;

		private bool triggered;

		public object Owner
		{
			get;
			private set;
		}

		public CombatTriggerType Type
		{
			get;
			private set;
		}

		public uint LastDitchDelayMillis
		{
			get;
			private set;
		}

		public StoryActionCombatTrigger(object owner, CombatTriggerType triggerType, string storyAction)
		{
			this.Owner = owner;
			this.Type = triggerType;
			this.LastDitchDelayMillis = 1800000u;
			this.storyAction = storyAction;
		}

		public void Trigger(Entity intruder)
		{
			if (this.Type == CombatTriggerType.Area && intruder != null && intruder.Get<TeamComponent>().TeamType != TeamType.Attacker)
			{
				return;
			}
			this.triggered = true;
			new ActionChain(this.storyAction);
		}

		public bool IsAlreadyTriggered()
		{
			return this.triggered;
		}
	}
}
