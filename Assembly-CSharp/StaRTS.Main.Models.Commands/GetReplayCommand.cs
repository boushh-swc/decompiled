using StaRTS.Externals.Manimal.TransferObjects.Request;
using System;

namespace StaRTS.Main.Models.Commands
{
	public class GetReplayCommand : GameCommand<GetReplayRequest, GetReplayResponse>
	{
		public const string ACTION = "player.battle.replay.get";

		public GetReplayCommand(string playerId, string battleId, string participantId) : base("player.battle.replay.get", new GetReplayRequest(playerId, battleId, participantId), new GetReplayResponse())
		{
		}

		public override OnCompleteAction OnFailure(uint status, object data)
		{
			base.OnFailure(status, data);
			if (status == 2110u)
			{
				return OnCompleteAction.Ok;
			}
			return OnCompleteAction.Desync;
		}
	}
}
