using System;

namespace StaRTS.Main.Models.Player
{
	public enum ServerPref
	{
		Locale = 0,
		AgeGate = 1,
		LoginPopups = 2,
		LastLoginTime = 3,
		NewspaperArticlesViewed = 4,
		[Obsolete("There is now a dedicated property in CurrentPlayer", true)]
		LastTroopRequestTime = 5,
		NumStoreItemsNotViewed = 6,
		NumRateAppViewed = 7,
		RatedApp = 8,
		NumInventoryItemsNotViewed = 9,
		ChapterMissionViewed = 10,
		SpecopsMissionViewed = 11,
		TournamentViewed = 12,
		LastPaymentTime = 13,
		BattlesAdCount = 14,
		SquadIntroViewed = 15,
		TournamentTierChangeTimeViewed = 16,
		FactionFlippingViewed = 17,
		FactionFlippingSkipConfirmation = 18,
		FactionFlipped = 19,
		LastChatViewTime = 20,
		LastPushAuthPromptTroopRequestTime = 21,
		LastPushAuthPromptSquadJoinedTime = 22,
		PushAuthPromptedCount = 23,
		PlanetsTutorialViewed = 24,
		FirstRelocationPlanet = 25,
		NumInventoryCratesNotViewed = 26,
		NumInventoryTroopsNotViewed = 27,
		NumInventoryCurrencyNotViewed = 28,
		COUNT = 29
	}
}
