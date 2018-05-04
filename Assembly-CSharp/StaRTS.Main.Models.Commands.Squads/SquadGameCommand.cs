using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Utils;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public abstract class SquadGameCommand<TRequest, TResponse> : GameCommand<TRequest, TResponse> where TRequest : AbstractRequest where TResponse : AbstractResponse
	{
		public SquadGameCommand(string action, TRequest request, TResponse response) : base(action, request, response)
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
