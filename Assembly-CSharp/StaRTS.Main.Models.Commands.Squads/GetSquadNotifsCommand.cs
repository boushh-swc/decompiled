using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class GetSquadNotifsCommand : SquadGameCommand<GetSquadNotifsRequest, SquadNotifsResponse>
	{
		public const string ACTION = "guild.notifications.get";

		public GetSquadNotifsCommand(GetSquadNotifsRequest request) : base("guild.notifications.get", request, new SquadNotifsResponse())
		{
		}
	}
}
