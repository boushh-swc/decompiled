using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Squads.Requests;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class SendSquadInviteCommand : SquadGameCommand<SendSquadInviteRequest, DefaultResponse>
	{
		public const string ACTION = "guild.invite";

		public SendSquadInviteCommand(SendSquadInviteRequest request) : base("guild.invite", request, new DefaultResponse())
		{
		}
	}
}
