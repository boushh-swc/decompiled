using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Squads.Requests;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class AcceptSquadInviteCommand : SquadGameCommand<SquadIDRequest, DefaultResponse>
	{
		public const string ACTION = "guild.invite.accept";

		public AcceptSquadInviteCommand(SquadIDRequest request) : base("guild.invite.accept", request, new DefaultResponse())
		{
		}
	}
}
