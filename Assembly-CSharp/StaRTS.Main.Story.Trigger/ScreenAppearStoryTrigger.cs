using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class ScreenAppearStoryTrigger : AbstractStoryTrigger, IEventObserver
	{
		private const string PLANET_DETAIL_SCREEN = "Planet_Detail_Screen";

		private const string PLANETARY_COMMAND_UPGRADE_SCREEN = "Planetary_Command_Upgrade_Screen";

		private const string PLANET_CONFIRM_SELECTION_SCREEN = "Confirm_Selected_Planet_Screen";

		private const string PLANET_CONFIRM_SMALL_BOX = "Planetary_Command_Confirm_Small_Box";

		public ScreenAppearStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
		}

		public override void Activate()
		{
			base.Activate();
			Service.EventManager.RegisterObserver(this, EventId.ScreenLoaded, EventPriority.Default);
		}

		private void RemoveListeners()
		{
			Service.EventManager.UnregisterObserver(this, EventId.ScreenLoaded);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ScreenLoaded)
			{
				AlertScreen alertScreen = cookie as AlertScreen;
				if (this.vo.PrepareString.Equals("Planetary_Command_Confirm_Small_Box") && alertScreen != null && "Tutorial".Equals(alertScreen.Tag))
				{
					this.RemoveListeners();
					this.parent.SatisfyTrigger(this);
				}
				else if (cookie is PlanetDetailsScreen && this.vo.PrepareString.Equals("Planet_Detail_Screen"))
				{
					this.RemoveListeners();
					this.parent.SatisfyTrigger(this);
				}
				else if (cookie is NavigationCenterUpgradeScreen && this.vo.PrepareString.Equals("Planetary_Command_Upgrade_Screen"))
				{
					this.RemoveListeners();
					this.parent.SatisfyTrigger(this);
				}
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			this.RemoveListeners();
			base.Destroy();
		}
	}
}
