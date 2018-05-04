using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Squads.Requests
{
	public class SendSquadInviteRequest : PlayerIdRequest
	{
		private string recipientPlayerId;

		private string fbFriendId;

		private string accessToken;

		public SendSquadInviteRequest(string playerId, string recipientPlayerId, string fbFriendId, string fbAccessToken)
		{
			base.PlayerId = playerId;
			this.recipientPlayerId = recipientPlayerId;
			this.fbFriendId = fbFriendId;
			this.accessToken = fbAccessToken;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("recipientPlayerId", this.recipientPlayerId);
			if (this.fbFriendId != null)
			{
				serializer.AddString("fbFriendId", this.fbFriendId);
			}
			if (this.accessToken != null)
			{
				serializer.AddString("accessToken", this.accessToken);
			}
			return serializer.End().ToString();
		}
	}
}
