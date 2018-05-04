using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Objectives;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Objectives
{
	public class GetObjectivesCommand : GameCommand<PlayerIdRequest, GetObjectivesResponse>
	{
		public const string ACTION = "player.planet.objective";

		public GetObjectivesCommand(PlayerIdRequest request) : base("player.planet.objective", request, new GetObjectivesResponse())
		{
		}

		public override void OnSuccess()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			currentPlayer.Objectives.Clear();
			foreach (KeyValuePair<string, ObjectiveGroup> current in base.ResponseResult.Groups)
			{
				currentPlayer.Objectives.Add(current.Key, current.Value);
			}
			base.OnSuccess();
		}
	}
}
