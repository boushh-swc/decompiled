using StaRTS.Main.Models.Squads.War;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Squads
{
	public class SquadWarBoardBuilding
	{
		public GameObject Building;

		public SquadWarBoardPlayerInfo PlayerInfo;

		public bool IsEmpire;

		public SquadWarBoardBuilding(SquadWarParticipantState participantState, GameObject building, bool isEmpire)
		{
			this.Building = building;
			this.IsEmpire = isEmpire;
			this.PlayerInfo = new SquadWarBoardPlayerInfo(participantState, building.transform);
		}

		public void Destroy()
		{
			UnityEngine.Object.Destroy(this.Building);
			this.PlayerInfo.Destroy();
		}

		public void ToggleVisibility(bool flag)
		{
			this.Building.SetActive(flag);
			this.PlayerInfo.ToggleDisplay(flag);
		}
	}
}
