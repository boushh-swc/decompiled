using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Player;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Commands.Objectives
{
	public class ForceObjectivesUpdateCommand : GameCommand<PlanetIdRequest, ForceObjectivesResponse>
	{
		public const string ACTION = "player.objective.forceUpdate";

		private string planetUid;

		public ForceObjectivesUpdateCommand(string planetUid) : base("player.objective.forceUpdate", new PlanetIdRequest(planetUid), new ForceObjectivesResponse(planetUid))
		{
			this.planetUid = planetUid;
		}

		public override void OnSuccess()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (!currentPlayer.Objectives.ContainsKey(this.planetUid))
			{
				currentPlayer.Objectives.Add(this.planetUid, base.ResponseResult.Group);
			}
			else
			{
				currentPlayer.Objectives[this.planetUid] = base.ResponseResult.Group;
			}
			base.OnSuccess();
		}
	}
}
