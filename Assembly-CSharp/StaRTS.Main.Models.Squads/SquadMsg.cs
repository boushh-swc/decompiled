using StaRTS.Main.Models.Squads.War;
using System;

namespace StaRTS.Main.Models.Squads
{
	public class SquadMsg
	{
		public SqmActionData ActionData;

		public SqmSquadData SquadData;

		public SqmRequestData RequestData;

		public SqmDonationData DonationData;

		public SqmMemberData MemberData;

		public SqmReplayData ReplayData;

		public SqmFriendInviteData FriendInviteData;

		public SqmOwnerData OwnerData;

		public SqmChatData ChatData;

		public SqmApplyData ApplyData;

		public SqmWarEventData WarEventData;

		public SqmWarParticipantData WarParticipantData;

		public SqmPerkData PerkData;

		public SquadWarData CurrentSquadWarData;

		public Squad RespondedSquad;

		public SquadMember SquadMemberResponse;

		public uint TimeSent;

		public SquadMsgType Type;

		public string NotifId;

		public string BISource;
	}
}
