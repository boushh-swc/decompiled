using StaRTS.Externals.Manimal.TransferObjects.Request;
using System;

namespace StaRTS.Main.Models.Commands
{
	public class GetEndpointsCommand : GameCommand<DefaultRequest, GetEndpointsResponse>
	{
		public const string ACTION = "config.endpoints.get";

		public GetEndpointsCommand(DefaultRequest request) : base("config.endpoints.get", request, new GetEndpointsResponse())
		{
		}
	}
}
