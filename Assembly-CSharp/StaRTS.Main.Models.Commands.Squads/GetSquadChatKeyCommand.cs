using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class GetSquadChatKeyCommand : SquadGameCommand<SquadIDRequest, GetSquadChatKeyResponse>
	{
		public const string ACTION = "guild.get.chatKey";

		public GetSquadChatKeyCommand(SquadIDRequest request) : base("guild.get.chatKey", request, new GetSquadChatKeyResponse())
		{
		}
	}
}
