using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class GetLeaderboardFriendsCommand : SquadGameCommand<FriendLBIDRequest, LeaderboardResponse>
	{
		public const string ACTION = "player.leaderboard.getForFriends";

		public GetLeaderboardFriendsCommand(FriendLBIDRequest request) : base("player.leaderboard.getForFriends", request, new LeaderboardResponse())
		{
		}
	}
}
