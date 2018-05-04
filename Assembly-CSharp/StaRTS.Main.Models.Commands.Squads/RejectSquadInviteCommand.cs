using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Squads.Requests;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class RejectSquadInviteCommand : SquadGameCommand<SquadIDRequest, DefaultResponse>
	{
		public const string ACTION = "guild.invite.reject";

		public RejectSquadInviteCommand(SquadIDRequest request) : base("guild.invite.reject", request, new DefaultResponse())
		{
		}
	}
}
