using System;

namespace StaRTS.Main.Models.Squads.War
{
	public enum SquadWarScoutState
	{
		Invalid = 0,
		AttackAvailable = 1,
		NotInActionPhase = 2,
		NoTurnsLeft = 3,
		NotPatricipantInWar = 4,
		UnderAttack = 5,
		OpponentHasNoVictoryPointsLeft = 6,
		DestinationUnavailable = 7
	}
}
