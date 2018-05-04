using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class SearchSquadsByNameCommand : SquadGameCommand<SearchSquadsByNameRequest, FeaturedSquadsResponse>
	{
		public const string ACTION = "guild.search.byName";

		public SearchSquadsByNameCommand(SearchSquadsByNameRequest request) : base("guild.search.byName", request, new FeaturedSquadsResponse())
		{
		}
	}
}
