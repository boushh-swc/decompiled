using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers.Planets
{
	public class PlanetDetailsFeaturedViewModule : AbstractPlanetDetailsViewModule
	{
		private const string TIMED_EVENT_FOOTER_TEXTURE = "PlanetFooterTexture";

		private const string CAMPAIGN_GROUP = "CampaignSelection";

		private const string TOURNAMENT_GROUP = "TournamentSelection";

		private const string EVENTS_TITLE_STRING = "LABEL_ELITE_OPS";

		private const string COMING_SOON_STRING = "SPEC_OPS_COMING_SOON";

		public PlanetDetailsTournamentsViewModule tournamentsView;

		private UXTexture timedEventFooterTexture;

		private UXElement tournamentGroup;

		public PlanetDetailsFeaturedViewModule(PlanetDetailsScreen screen) : base(screen)
		{
			this.tournamentsView = new PlanetDetailsTournamentsViewModule(screen);
		}

		public void OnScreenLoaded()
		{
			this.timedEventFooterTexture = this.screen.GetElement<UXTexture>("PlanetFooterTexture");
			this.timedEventFooterTexture.LoadTexture(this.screen.viewingPlanetVO.FooterTexture);
			this.tournamentGroup = this.screen.GetElement<UXElement>("TournamentSelection");
			this.tournamentsView.OnScreenLoaded();
			this.RefreshView(false);
		}

		public void RefreshScreenForPlanetChange()
		{
			this.RefreshView(true);
			this.tournamentsView.RefreshScreenForPlanetChange();
		}

		private void RefreshView(bool reloadTexture)
		{
			TournamentController tournamentController = Service.TournamentController;
			TournamentVO activeTournamentOnPlanet = TournamentController.GetActiveTournamentOnPlanet(this.screen.viewingPlanetVO.Uid);
			if (activeTournamentOnPlanet == null)
			{
				this.ShowNoActiveEventsAvailable();
				if (reloadTexture)
				{
					this.timedEventFooterTexture.LoadTexture(this.screen.viewingPlanetVO.FooterTexture);
				}
			}
			else if (tournamentController.IsThisTournamentLive(activeTournamentOnPlanet))
			{
				this.timedEventFooterTexture.LoadTexture(this.screen.viewingPlanetVO.FooterConflictTexture);
			}
			else
			{
				this.timedEventFooterTexture.LoadTexture(this.screen.viewingPlanetVO.FooterTexture);
			}
			this.tournamentsView.RefreshView();
		}

		public void OnAnimateShowUI()
		{
			this.tournamentsView.OnAnimateShowUI();
		}

		public void ShowNoActiveEventsAvailable()
		{
			this.tournamentGroup.Visible = false;
		}

		public void Destroy()
		{
			this.tournamentsView.Destroy();
			this.tournamentsView = null;
		}
	}
}
