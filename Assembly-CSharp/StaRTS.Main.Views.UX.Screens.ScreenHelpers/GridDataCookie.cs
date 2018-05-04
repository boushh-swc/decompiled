using StaRTS.Main.Models;
using System;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers
{
	public struct GridDataCookie
	{
		public FactionToggle SelectedFaction
		{
			get;
			private set;
		}

		public SocialTabs SelectedTab
		{
			get;
			private set;
		}

		public string SelectedPlanet
		{
			get;
			private set;
		}

		public GridDataCookie(SocialTabs socialTab, FactionToggle factionToggle, string selectedPlanet)
		{
			this.SelectedFaction = factionToggle;
			this.SelectedTab = socialTab;
			this.SelectedPlanet = selectedPlanet;
		}
	}
}
