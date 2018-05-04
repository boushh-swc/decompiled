using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens.Squads
{
	public class AbstractSquadScreenViewModule
	{
		protected SquadSlidingScreen screen;

		protected Lang lang;

		protected AbstractSquadScreenViewModule(SquadSlidingScreen screen)
		{
			this.screen = screen;
			this.lang = Service.Lang;
		}

		public virtual void OnScreenLoaded()
		{
		}

		public virtual void ShowView()
		{
		}

		public virtual void HideView()
		{
		}

		public virtual void RefreshView()
		{
		}

		public virtual void OnDestroyElement()
		{
		}

		public virtual bool IsVisible()
		{
			return false;
		}
	}
}
