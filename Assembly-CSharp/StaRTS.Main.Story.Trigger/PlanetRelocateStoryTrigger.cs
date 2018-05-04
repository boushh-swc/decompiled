using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class PlanetRelocateStoryTrigger : AbstractStoryTrigger, IEventObserver
	{
		private const string FIRST_RELOCATION = "FIRST_RELOC";

		private const string SERVER_PREF_DEFAULT = "0";

		private const int FIRST_RELOC_ARG = 0;

		private const int PLANET_UID_ARG = 0;

		private const int SAVE_VAR_ARG = 1;

		private bool isFirstRelocation;

		private string saveVarName;

		private PlanetVO planetVO;

		public PlanetRelocateStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
		}

		public override void Activate()
		{
			if (this.prepareArgs[0].Equals("FIRST_RELOC"))
			{
				this.isFirstRelocation = true;
				if (this.prepareArgs.Length > 1)
				{
					this.saveVarName = this.prepareArgs[1];
				}
			}
			else
			{
				string text = this.prepareArgs[0];
				this.planetVO = Service.StaticDataController.GetOptional<PlanetVO>(text);
				if (this.planetVO == null)
				{
					Service.Logger.Error("PlanetRelocateStoryTrigger: invalid planet uid: " + text);
					return;
				}
			}
			base.Activate();
			Service.EventManager.RegisterObserver(this, EventId.PlanetRelocate, EventPriority.Default);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.PlanetRelocate)
			{
				string text = (string)cookie;
				if (this.isFirstRelocation)
				{
					if (!string.IsNullOrEmpty(this.saveVarName))
					{
						AutoStoryTriggerUtils.SaveTriggerValue(this.saveVarName, text);
					}
					this.parent.SatisfyTrigger(this);
				}
				else if (text.Equals(this.planetVO.Uid))
				{
					this.parent.SatisfyTrigger(this);
				}
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.PlanetRelocate);
			base.Destroy();
		}
	}
}
