using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class GetFeaturedSquadsCommand : SquadGameCommand<PlayerIdRequest, FeaturedSquadsResponse>
	{
		public const string ACTION = "guild.list.open";

		public GetFeaturedSquadsCommand(PlayerIdRequest request) : base("guild.list.open", request, new FeaturedSquadsResponse())
		{
		}
	}
}
