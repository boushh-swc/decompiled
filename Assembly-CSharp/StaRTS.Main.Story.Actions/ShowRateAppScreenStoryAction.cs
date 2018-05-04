using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class ShowRateAppScreenStoryAction : AbstractStoryAction, IEventObserver
	{
		private const string RATE_APP_TRUE = "1";

		private int numTimesViewed;

		public ShowRateAppScreenStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			if (!GameConstants.RATE_MY_APP_ENABLED)
			{
				this.parent.ChildComplete(this);
				return;
			}
			if (Service.GalaxyViewController.IsPlanetDetailsScreenOpeningOrOpen())
			{
				this.parent.ChildComplete(this);
				return;
			}
			if (Service.EnvironmentController.IsRestrictedProfile())
			{
				this.parent.ChildComplete(this);
				return;
			}
			if (Service.ServerPlayerPrefs.GetPref(ServerPref.RatedApp) == "1")
			{
				this.parent.ChildComplete(this);
				return;
			}
			if (Service.ScreenController.GetHighestLevelScreen<MissionCompleteScreen>() == null)
			{
				Service.EventManager.RegisterObserver(this, EventId.ScreenClosing, EventPriority.Default);
				Service.ScreenController.AddScreen(new RateAppScreen());
			}
			else
			{
				Service.EventManager.RegisterObserver(this, EventId.MissionCompleteScreenDisplayed, EventPriority.Default);
			}
			this.numTimesViewed = Convert.ToInt32(Service.ServerPlayerPrefs.GetPref(ServerPref.NumRateAppViewed));
			this.numTimesViewed++;
			Service.ServerPlayerPrefs.SetPref(ServerPref.NumRateAppViewed, this.numTimesViewed.ToString());
			SetPrefsCommand command = new SetPrefsCommand(false);
			Service.ServerAPI.Enqueue(command);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			EatResponse result = EatResponse.NotEaten;
			if (id != EventId.ScreenClosing)
			{
				if (id == EventId.MissionCompleteScreenDisplayed)
				{
					Service.EventManager.UnregisterObserver(this, EventId.MissionCompleteScreenDisplayed);
					Service.EventManager.RegisterObserver(this, EventId.ScreenClosing, EventPriority.Default);
					Service.ScreenController.AddScreen(new RateAppScreen());
				}
			}
			else
			{
				RateAppScreen rateAppScreen = cookie as RateAppScreen;
				if (rateAppScreen != null)
				{
					Service.EventManager.UnregisterObserver(this, EventId.ScreenClosing);
					this.OnNotificationScreenClosed(rateAppScreen.ClosedWithConfirmation);
				}
			}
			return result;
		}

		private void OnNotificationScreenClosed(bool gotoStoreFront)
		{
			if (gotoStoreFront)
			{
				Service.BILoggingController.TrackGameAction("rateapp", "yes", this.numTimesViewed.ToString(), null, 1);
				Service.ServerPlayerPrefs.SetPref(ServerPref.RatedApp, "1");
				SetPrefsCommand command = new SetPrefsCommand(false);
				Service.ServerAPI.Sync(command);
				GameUtils.TryAndOpenAppropriateStorePage();
			}
			else
			{
				Service.BILoggingController.TrackGameAction("rateapp", "no", this.numTimesViewed.ToString(), null, 1);
			}
			this.parent.ChildComplete(this);
		}
	}
}
