using System;

namespace StaRTS.Main.Models.Squads
{
	public enum SquadAction
	{
		Create = 0,
		Join = 1,
		Leave = 2,
		Edit = 3,
		ApplyToJoin = 4,
		AcceptApplicationToJoin = 5,
		RejectApplicationToJoin = 6,
		SendInviteToJoin = 7,
		AcceptInviteToJoin = 8,
		RejectInviteToJoin = 9,
		PromoteMember = 10,
		DemoteMember = 11,
		RemoveMember = 12,
		RequestTroops = 13,
		DonateTroops = 14,
		RequestWarTroops = 15,
		DonateWarTroops = 16,
		ShareReplay = 17,
		StartWarMatchmaking = 18,
		CancelWarMatchmaking = 19
	}
}
