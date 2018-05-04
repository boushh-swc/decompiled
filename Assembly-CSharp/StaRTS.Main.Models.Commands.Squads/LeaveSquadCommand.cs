using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class LeaveSquadCommand : SquadGameCommand<PlayerIdRequest, DefaultResponse>
	{
		public const string ACTION = "guild.leave";

		public LeaveSquadCommand(PlayerIdRequest request) : base("guild.leave", request, new DefaultResponse())
		{
		}
	}
}
