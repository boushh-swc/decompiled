using System;

namespace StaRTS.Main.Models.Squads
{
	public enum SquadMsgType
	{
		Invalid = 0,
		Chat = 1,
		Join = 2,
		JoinRequest = 3,
		JoinRequestAccepted = 4,
		JoinRequestRejected = 5,
		InviteAccepted = 6,
		Leave = 7,
		Ejected = 8,
		Promotion = 9,
		Demotion = 10,
		ShareBattle = 11,
		TroopRequest = 12,
		TroopDonation = 13,
		WarMatchMakingBegin = 14,
		WarMatchMakingCancel = 15,
		WarStarted = 16,
		WarPrepared = 17,
		WarBuffBaseAttackStart = 18,
		WarBuffBaseAttackComplete = 19,
		WarPlayerAttackStart = 20,
		WarPlayerAttackComplete = 21,
		WarEnded = 22,
		SquadLevelUp = 23,
		PerkUnlocked = 24,
		PerkUpgraded = 25,
		PerkInvest = 26,
		Invite = 27,
		InviteRejected = 28
	}
}
