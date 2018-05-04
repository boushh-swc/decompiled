using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaRTS.Main.Utils
{
	public class CrashEventDataController : IEventObserver
	{
		private const string METADATA_GUILD = "guildId";

		private const string METADATA_PAYER = "isPayer";

		private const string METADATA_FACTION = "faction";

		private const string METADATA_ABTESTS = "ABtests";

		private const string METADATA_HQLEVEL = "HQLevel";

		private const string METADATA_SPD = "SPDAvailable";

		private const string BREADCRUMB_STATE_CHANGE = "Game state changed to: ";

		private const string BREADCRUMB_SCREEN_OPEN = "Screen opened: ";

		private const string BREADCRUMB_SCREEN_CLOSE = "Screen closed: ";

		private const string BREADCRUMB_OVERLAY_SCREEN_CLOSE = "Screen overlay closed: ";

		public CrashEventDataController()
		{
			this.SetupData();
			this.SetupBreadcrumbs();
		}

		private void SetupData()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (currentPlayer == null)
			{
				EventManager eventManager = Service.EventManager;
				if (eventManager != null)
				{
					eventManager.RegisterObserver(this, EventId.PlayerLoginSuccess);
				}
				else
				{
					Service.Logger.Warn("CrashEventDataController was not set up, no event manager exists");
				}
				return;
			}
			this.RefreshData(currentPlayer);
		}

		private void SetupBreadcrumbs()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.GameStateChanged);
			eventManager.RegisterObserver(this, EventId.ScreenLoaded);
			eventManager.RegisterObserver(this, EventId.ScreenClosing);
			eventManager.RegisterObserver(this, EventId.ScreenOverlayClosing);
		}

		private void RefreshData(CurrentPlayer player)
		{
			Crittercism.SetUsername(player.PlayerId);
			SquadController squadController = Service.SquadController;
			if (squadController != null)
			{
				Squad currentSquad = squadController.StateManager.GetCurrentSquad();
				if (currentSquad != null)
				{
					Crittercism.SetValue("guildId", currentSquad.SquadID);
				}
				else
				{
					Crittercism.SetValue("guildId", string.Empty);
				}
			}
			Crittercism.SetValue("isPayer", (player.LastPaymentTime <= 0u) ? "False" : "True");
			Crittercism.SetValue("faction", player.Faction.ToString());
			if (player.AbTests != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (KeyValuePair<string, object> current in player.AbTests)
				{
					stringBuilder.Append(current.Key);
					stringBuilder.Append(",");
				}
				string value = stringBuilder.ToString();
				Crittercism.SetValue("ABtests", value);
			}
			Crittercism.SetValue("HQLevel", player.Map.FindHighestHqLevel().ToString());
			TargetedBundleController targetedBundleController = Service.TargetedBundleController;
			if (targetedBundleController != null)
			{
				string value2 = (targetedBundleController.CurrentTargetedOffer == null) ? "null" : targetedBundleController.CurrentTargetedOffer.Uid;
				Crittercism.SetValue("SPDAvailable", value2);
			}
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.PlayerIdSet);
			eventManager.RegisterObserver(this, EventId.PlayerFactionChanged);
			eventManager.RegisterObserver(this, EventId.SquadUpdated);
			eventManager.RegisterObserver(this, EventId.MetaDataLoadEnd);
			eventManager.RegisterObserver(this, EventId.TargetedBundleContentPrepared);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.ScreenClosing:
				if (cookie != null && cookie is ScreenBase)
				{
					string str = (cookie as ScreenBase).GetType().Name;
					Crittercism.LeaveBreadcrumb("Screen closed: " + str);
				}
				return EatResponse.NotEaten;
			case EventId.ScreenClosed:
				IL_1C:
				switch (id)
				{
				case EventId.PlayerFactionChanged:
				case EventId.PlayerIdSet:
				case EventId.PlayerLoginSuccess:
					goto IL_6D;
				case EventId.PvpRatingChanged:
				case EventId.PvpNewBattleOccured:
					IL_3C:
					if (id == EventId.MetaDataLoadEnd)
					{
						goto IL_6D;
					}
					if (id == EventId.GameStateChanged)
					{
						string name = Service.GameStateMachine.CurrentState.GetType().Name;
						Crittercism.LeaveBreadcrumb("Game state changed to: " + name);
						return EatResponse.NotEaten;
					}
					if (id != EventId.TargetedBundleContentPrepared && id != EventId.SquadUpdated)
					{
						return EatResponse.NotEaten;
					}
					goto IL_6D;
				}
				goto IL_3C;
				IL_6D:
				this.RefreshData(Service.CurrentPlayer);
				return EatResponse.NotEaten;
			case EventId.ScreenOverlayClosing:
				if (cookie != null)
				{
					string str = (string)cookie;
					Crittercism.LeaveBreadcrumb("Screen overlay closed: " + str);
				}
				return EatResponse.NotEaten;
			case EventId.ScreenLoaded:
				if (cookie != null && cookie is ScreenBase)
				{
					string str = (cookie as ScreenBase).GetType().Name;
					Crittercism.LeaveBreadcrumb("Screen opened: " + str);
				}
				return EatResponse.NotEaten;
			}
			goto IL_1C;
		}
	}
}
