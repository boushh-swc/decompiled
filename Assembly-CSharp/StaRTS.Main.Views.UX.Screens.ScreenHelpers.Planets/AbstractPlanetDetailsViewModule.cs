using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers.Planets
{
	public class AbstractPlanetDetailsViewModule
	{
		protected PlanetDetailsScreen screen;

		protected UXElement eventInfoGroup;

		protected CurrentPlayer Player
		{
			get
			{
				return Service.CurrentPlayer;
			}
		}

		protected CampaignController CampController
		{
			get
			{
				return Service.CampaignController;
			}
		}

		protected Lang LangController
		{
			get
			{
				return Service.Lang;
			}
		}

		protected EventManager EvtManager
		{
			get
			{
				return Service.EventManager;
			}
		}

		protected StaticDataController Sdc
		{
			get
			{
				return Service.StaticDataController;
			}
		}

		protected RewardManager RManager
		{
			get
			{
				return Service.RewardManager;
			}
		}

		protected ScreenController ScrController
		{
			get
			{
				return Service.ScreenController;
			}
		}

		public AbstractPlanetDetailsViewModule(PlanetDetailsScreen screen)
		{
			this.screen = screen;
		}
	}
}
