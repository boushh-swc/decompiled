using StaRTS.Main.Models;
using System;
using System.Runtime.InteropServices;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
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
			this = default(GridDataCookie);
			this.SelectedFaction = factionToggle;
			this.SelectedTab = socialTab;
			this.SelectedPlanet = selectedPlanet;
		}
	}
}
