using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Utils;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public abstract class SquadGameActionCommand<TRequest, TResponse> : GameActionCommand<TRequest, TResponse> where TRequest : PlayerIdChecksumRequest where TResponse : AbstractResponse
	{
		public SquadGameActionCommand(string action, TRequest request, TResponse response) : base(action, request, response)
		{
		}

		public override OnCompleteAction OnFailure(uint status, object data)
		{
			if (SquadUtils.IsNotFatalServerError(status))
			{
				return base.EatFailure(status, data);
			}
			return base.OnFailure(status, data);
		}
	}
}
