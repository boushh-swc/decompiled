using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Squads.Responses;
using StaRTS.Main.Models.Perks;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Squads
{
	public class TroopDonationTrackController
	{
		private const string REP_REWARD_AMOUNT = "PERK_TROOP_DONATE_REP_REWARD_AMOUNT";

		private const string REP_REWARD_DESC = "PERK_TROOP_DONATE_REP_REWARD_DESC";

		public TroopDonationTrackController()
		{
			Service.TroopDonationTrackController = this;
		}

		public void UpdateTroopDonationProgress(TroopDonateResponse response)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			currentPlayer.UpdateTroopDonationProgress(response.DonationCount, response.LastTrackedDonationTime, response.DonationCooldownEndTime);
			EventManager eventManager = Service.EventManager;
			eventManager.SendEvent(EventId.TroopDonationTrackProgressUpdated, null);
			if (response.ReputationAwarded)
			{
				Inventory inventory = currentPlayer.Inventory;
				inventory.ModifyReputation(GameConstants.SQUADPERK_DONATION_REPUTATION_AWARD);
				Lang lang = Service.Lang;
				string status = lang.Get("PERK_TROOP_DONATE_REP_REWARD_AMOUNT", new object[]
				{
					GameConstants.SQUADPERK_DONATION_REPUTATION_AWARD
				});
				string toast = lang.Get("PERK_TROOP_DONATE_REP_REWARD_DESC", new object[0]);
				Service.UXController.MiscElementsManager.ShowToast(toast, status, string.Empty);
				eventManager.SendEvent(EventId.TroopDonationTrackRewardReceived, null);
			}
		}

		public bool IsTroopDonationProgressComplete()
		{
			return this.GetTroopDonationProgressAmount() >= GameConstants.SQUADPERK_DONATION_REPUTATION_AWARD_THRESHOLD;
		}

		public int GetTroopDonationProgressAmount()
		{
			TroopDonationProgress troopDonationProgress = Service.CurrentPlayer.TroopDonationProgress;
			int timeRemainingUntilNextProgressTrack = this.GetTimeRemainingUntilNextProgressTrack();
			if (timeRemainingUntilNextProgressTrack <= 0 && troopDonationProgress.DonationCount >= GameConstants.SQUADPERK_DONATION_REPUTATION_AWARD_THRESHOLD)
			{
				return 0;
			}
			return troopDonationProgress.DonationCount;
		}

		public int GetTimeRemainingUntilNextProgressTrack()
		{
			TroopDonationProgress troopDonationProgress = Service.CurrentPlayer.TroopDonationProgress;
			uint donationCooldownEndTime = (uint)troopDonationProgress.DonationCooldownEndTime;
			uint serverTime = Service.ServerAPI.ServerTime;
			return (int)(donationCooldownEndTime - serverTime);
		}
	}
}
