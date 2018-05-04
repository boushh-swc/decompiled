using Midcore.Chat.Photon;
using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands;
using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Utils.Chat;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Utils
{
	public static class SquadMsgUtils
	{
		public static PlayerIdRequest GeneratePlayerIdRequest(SquadMsg message)
		{
			return new PlayerIdRequest
			{
				PlayerId = message.OwnerData.PlayerId
			};
		}

		public static SquadIDRequest GenerateSquadIdRequest(SquadMsg message)
		{
			return new SquadIDRequest
			{
				SquadId = message.SquadData.Id,
				PlayerId = message.OwnerData.PlayerId
			};
		}

		public static MemberIdRequest GenerateMemberIdRequest(SquadMsg message)
		{
			return new MemberIdRequest
			{
				PlayerId = message.OwnerData.PlayerId,
				MemberId = message.MemberData.MemberId
			};
		}

		public static ApplyToSquadRequest GenerateApplyToSquadRequest(SquadMsg message)
		{
			return new ApplyToSquadRequest
			{
				SquadId = message.SquadData.Id,
				PlayerId = message.OwnerData.PlayerId,
				Message = WWW.EscapeURL(message.RequestData.Text)
			};
		}

		public static CreateSquadRequest GenerateCreateSquadRequest(SquadMsg message)
		{
			SqmSquadData squadData = message.SquadData;
			return new CreateSquadRequest(WWW.EscapeURL(squadData.Name), WWW.EscapeURL(squadData.Desc), squadData.Icon, squadData.MinTrophies, squadData.Open)
			{
				PlayerId = message.OwnerData.PlayerId
			};
		}

		public static EditSquadRequest GenerateEditSquadRequest(SquadMsg message)
		{
			SqmSquadData squadData = message.SquadData;
			return new EditSquadRequest
			{
				Desc = WWW.EscapeURL(squadData.Desc),
				Icon = squadData.Icon,
				OpenSquad = squadData.Open,
				MinTrophy = squadData.MinTrophies,
				PlayerId = message.OwnerData.PlayerId
			};
		}

		public static TroopSquadRequest GenerateTroopRequest(SquadMsg message)
		{
			return new TroopSquadRequest
			{
				PlayerId = message.OwnerData.PlayerId,
				PayToSkip = message.RequestData.PayToSkip,
				Message = WWW.EscapeURL(message.RequestData.Text)
			};
		}

		public static TroopDonateRequest GenerateTroopDonateRequest(SquadMsg message)
		{
			return new TroopDonateRequest
			{
				PlayerId = message.OwnerData.PlayerId,
				RecipientId = message.DonationData.RecipientId,
				RequestId = message.DonationData.RequestId,
				Donations = new Dictionary<string, int>(message.DonationData.Donations)
			};
		}

		public static ShareReplayRequest GenerateShareReplayRequest(SquadMsg message)
		{
			return new ShareReplayRequest
			{
				PlayerId = message.OwnerData.PlayerId,
				BattleId = message.ReplayData.BattleId,
				Message = WWW.EscapeURL(message.ReplayData.Text)
			};
		}

		public static SendSquadInviteRequest GenerateSendInviteRequest(SquadMsg message)
		{
			SqmFriendInviteData friendInviteData = message.FriendInviteData;
			return new SendSquadInviteRequest(message.OwnerData.PlayerId, friendInviteData.PlayerId, friendInviteData.FacebookFriendId, friendInviteData.FacebookAccessToken);
		}

		public static SquadInvite GenerateSquadInvite(SquadMsg message)
		{
			return new SquadInvite
			{
				SquadId = message.SquadData.Id,
				SenderId = message.FriendInviteData.SenderId,
				SenderName = message.FriendInviteData.SenderName
			};
		}

		public static SquadWarStartMatchmakingRequest GenerateStartWarMatchmakingRequest(SquadMsg message)
		{
			return new SquadWarStartMatchmakingRequest(message.WarParticipantData);
		}

		public static PlayerIdChecksumRequest GeneratePlayerIdChecksumRequest(SquadMsg message)
		{
			return new PlayerIdChecksumRequest();
		}

		public static SquadMsg GenerateMessageFromSquadResponse(SquadResponse response, LeaderboardController lbc)
		{
			SqmSquadData sqmSquadData = new SqmSquadData();
			sqmSquadData.Id = response.SquadId;
			Squad orCreateSquad = lbc.GetOrCreateSquad(sqmSquadData.Id);
			orCreateSquad.FromObject(response.SquadData);
			return new SquadMsg
			{
				SquadData = sqmSquadData,
				RespondedSquad = orCreateSquad
			};
		}

		public static SquadMsg GenerateMessageFromSquadMemberResponse(SquadMemberResponse response)
		{
			SquadMember squadMember = new SquadMember();
			squadMember.FromObject(response.SquadMemberData);
			return new SquadMsg
			{
				SquadMemberResponse = squadMember
			};
		}

		public static SquadMsg GenerateMessageFromTroopDonateResponse(TroopDonateResponse response)
		{
			SqmDonationData sqmDonationData = new SqmDonationData();
			sqmDonationData.Donations = response.TroopsDonated;
			return new SquadMsg
			{
				DonationData = sqmDonationData
			};
		}

		public static SquadMsg GenerateMessageFromGetSquadWarStatusResponse(GetSquadWarStatusResponse response)
		{
			SquadWarSquadData squadWarSquadData = new SquadWarSquadData();
			SquadWarSquadData squadWarSquadData2 = new SquadWarSquadData();
			List<SquadWarBuffBaseData> list = new List<SquadWarBuffBaseData>();
			squadWarSquadData.FromObject(response.Squad1Data);
			squadWarSquadData2.FromObject(response.Squad2Data);
			List<object> list2 = response.BuffBaseData as List<object>;
			int i = 0;
			int count = list2.Count;
			while (i < count)
			{
				SquadWarBuffBaseData squadWarBuffBaseData = new SquadWarBuffBaseData();
				squadWarBuffBaseData.FromObject(list2[i]);
				list.Add(squadWarBuffBaseData);
				i++;
			}
			SquadWarData squadWarData = new SquadWarData();
			squadWarData.WarId = response.Id;
			squadWarData.Squads[0] = squadWarSquadData;
			squadWarData.Squads[1] = squadWarSquadData2;
			squadWarData.BuffBases = list;
			squadWarData.PrepGraceStartTimeStamp = response.PrepGraceStartTimeStamp;
			squadWarData.PrepEndTimeStamp = response.PrepEndTimeStamp;
			squadWarData.ActionGraceStartTimeStamp = response.ActionGraceStartTimeStamp;
			squadWarData.ActionEndTimeStamp = response.ActionEndTimeStamp;
			squadWarData.StartTimeStamp = response.StartTimeStamp;
			squadWarData.CooldownEndTimeStamp = response.CooldownEndTimeStamp;
			squadWarData.RewardsProcessed = response.RewardsProcessed;
			return new SquadMsg
			{
				CurrentSquadWarData = squadWarData
			};
		}

		public static SquadMsg GenerateMessageFromNotifObject(object notif)
		{
			Dictionary<string, object> dictionary = notif as Dictionary<string, object>;
			if (dictionary == null)
			{
				return null;
			}
			SquadMsg squadMsg = new SquadMsg();
			if (dictionary.ContainsKey("id"))
			{
				squadMsg.NotifId = Convert.ToString(dictionary["id"]);
			}
			if (dictionary.ContainsKey("date"))
			{
				squadMsg.TimeSent = Convert.ToUInt32(dictionary["date"]);
			}
			if (dictionary.ContainsKey("type"))
			{
				string name = Convert.ToString(dictionary["type"]);
				squadMsg.Type = StringUtils.ParseEnum<SquadMsgType>(name);
			}
			if (dictionary.ContainsKey("playerId"))
			{
				SqmOwnerData sqmOwnerData = new SqmOwnerData();
				squadMsg.OwnerData = sqmOwnerData;
				sqmOwnerData.PlayerId = Convert.ToString(dictionary["playerId"]);
				if (dictionary.ContainsKey("name"))
				{
					sqmOwnerData.PlayerName = Convert.ToString(dictionary["name"]);
				}
			}
			if (dictionary.ContainsKey("message"))
			{
				SqmChatData sqmChatData = new SqmChatData();
				squadMsg.ChatData = sqmChatData;
				sqmChatData.Message = WWW.UnEscapeURL(Convert.ToString(dictionary["message"]));
			}
			if (dictionary.ContainsKey("data"))
			{
				Dictionary<string, object> dictionary2 = dictionary["data"] as Dictionary<string, object>;
				if (dictionary2 != null)
				{
					if (dictionary2.ContainsKey("senderName"))
					{
						SqmFriendInviteData sqmFriendInviteData = new SqmFriendInviteData();
						squadMsg.FriendInviteData = sqmFriendInviteData;
						sqmFriendInviteData.SenderName = Convert.ToString(dictionary2["senderName"]);
					}
					if (dictionary2.ContainsKey("toRank"))
					{
						SqmMemberData sqmMemberData = new SqmMemberData();
						squadMsg.MemberData = sqmMemberData;
						string name2 = Convert.ToString(dictionary2["toRank"]);
						sqmMemberData.MemberRole = StringUtils.ParseEnum<SquadRole>(name2);
					}
					if (dictionary2.ContainsKey("acceptor"))
					{
						SqmApplyData sqmApplyData = new SqmApplyData();
						squadMsg.ApplyData = sqmApplyData;
						sqmApplyData.AcceptorId = Convert.ToString(dictionary2["acceptor"]);
					}
					if (dictionary2.ContainsKey("rejector"))
					{
						SqmApplyData sqmApplyData2 = new SqmApplyData();
						squadMsg.ApplyData = sqmApplyData2;
						sqmApplyData2.RejectorId = Convert.ToString(dictionary2["rejector"]);
					}
					if (dictionary2.ContainsKey("battleId"))
					{
						SqmReplayData sqmReplayData = new SqmReplayData();
						squadMsg.ReplayData = sqmReplayData;
						sqmReplayData.BattleId = Convert.ToString(dictionary2["battleId"]);
						if (dictionary2.ContainsKey("battleVersion"))
						{
							sqmReplayData.BattleVersion = Convert.ToString(dictionary2["battleVersion"]);
						}
						if (dictionary2.ContainsKey("cmsVersion"))
						{
							sqmReplayData.CMSVersion = Convert.ToString(dictionary2["cmsVersion"]);
						}
						if (dictionary2.ContainsKey("type"))
						{
							string name3 = Convert.ToString(dictionary2["type"]);
							sqmReplayData.BattleType = StringUtils.ParseEnum<SquadBattleReplayType>(name3);
						}
						if (dictionary2.ContainsKey("battleScoreDelta"))
						{
							object obj = dictionary2["battleScoreDelta"];
							if (obj != null)
							{
								sqmReplayData.MedalDelta = Convert.ToInt32(obj);
							}
						}
						if (dictionary2.ContainsKey("damagePercent"))
						{
							sqmReplayData.DamagePercent = Convert.ToInt32(dictionary2["damagePercent"]);
						}
						if (dictionary2.ContainsKey("stars"))
						{
							sqmReplayData.Stars = Convert.ToInt32(dictionary2["stars"]);
						}
						if (dictionary2.ContainsKey("opponentId"))
						{
							sqmReplayData.OpponentId = Convert.ToString(dictionary2["opponentId"]);
						}
						if (dictionary2.ContainsKey("opponentName"))
						{
							sqmReplayData.OpponentName = Convert.ToString(dictionary2["opponentName"]);
						}
						if (dictionary2.ContainsKey("opponentFaction"))
						{
							string name4 = Convert.ToString(dictionary2["opponentFaction"]);
							sqmReplayData.OpponentFaction = StringUtils.ParseEnum<FactionType>(name4);
						}
						if (dictionary2.ContainsKey("faction"))
						{
							string name5 = Convert.ToString(dictionary2["faction"]);
							sqmReplayData.SharerFaction = StringUtils.ParseEnum<FactionType>(name5);
						}
					}
					if (dictionary2.ContainsKey("totalCapacity"))
					{
						SqmRequestData sqmRequestData = new SqmRequestData();
						squadMsg.RequestData = sqmRequestData;
						sqmRequestData.TotalCapacity = Convert.ToInt32(dictionary2["totalCapacity"]);
						if (dictionary2.ContainsKey("amount"))
						{
							sqmRequestData.StartingAvailableCapacity = Convert.ToInt32(dictionary2["amount"]);
						}
						if (dictionary2.ContainsKey("warId"))
						{
							sqmRequestData.WarId = Convert.ToString(dictionary2["warId"]);
						}
						if (dictionary2.ContainsKey("troopDonationLimit"))
						{
							sqmRequestData.TroopDonationLimit = Convert.ToInt32(dictionary2["troopDonationLimit"]);
						}
						else
						{
							Service.Logger.Error("Missing troop request data param: troopDonationLimitdefaulting to " + GameConstants.MAX_PER_USER_TROOP_DONATION);
							sqmRequestData.TroopDonationLimit = GameConstants.MAX_PER_USER_TROOP_DONATION;
						}
					}
					if (dictionary2.ContainsKey("troopsDonated"))
					{
						SqmDonationData sqmDonationData = new SqmDonationData();
						squadMsg.DonationData = sqmDonationData;
						Dictionary<string, object> dictionary3 = dictionary2["troopsDonated"] as Dictionary<string, object>;
						if (dictionary3 != null)
						{
							sqmDonationData.Donations = new Dictionary<string, int>();
							foreach (KeyValuePair<string, object> current in dictionary3)
							{
								string key = current.Key;
								int value = Convert.ToInt32(current.Value);
								sqmDonationData.Donations.Add(key, value);
							}
						}
						if (dictionary2.ContainsKey("requestId"))
						{
							sqmDonationData.RequestId = Convert.ToString(dictionary2["requestId"]);
						}
						if (dictionary2.ContainsKey("recipientId"))
						{
							sqmDonationData.RecipientId = Convert.ToString(dictionary2["recipientId"]);
						}
						if (dictionary2.ContainsKey("amount"))
						{
							sqmDonationData.DonationCount = Convert.ToInt32(dictionary2["amount"]);
						}
					}
					if (dictionary2.ContainsKey("warId"))
					{
						SqmWarEventData sqmWarEventData = new SqmWarEventData();
						squadMsg.WarEventData = sqmWarEventData;
						sqmWarEventData.WarId = Convert.ToString(dictionary2["warId"]);
						if (dictionary2.ContainsKey("buffBaseUid"))
						{
							sqmWarEventData.BuffBaseUid = Convert.ToString(dictionary2["buffBaseUid"]);
						}
						if (dictionary2.ContainsKey("captured"))
						{
							sqmWarEventData.BuffBaseCaptured = Convert.ToBoolean(dictionary2["captured"]);
						}
						if (dictionary2.ContainsKey("opponentId"))
						{
							sqmWarEventData.OpponentId = Convert.ToString(dictionary2["opponentId"]);
						}
						if (dictionary2.ContainsKey("opponentName"))
						{
							sqmWarEventData.OpponentName = Convert.ToString(dictionary2["opponentName"]);
						}
						if (dictionary2.ContainsKey("stars"))
						{
							sqmWarEventData.StarsEarned = Convert.ToInt32(dictionary2["stars"]);
						}
						if (dictionary2.ContainsKey("victoryPoints"))
						{
							sqmWarEventData.VictoryPointsTaken = Convert.ToInt32(dictionary2["victoryPoints"]);
						}
						if (dictionary2.ContainsKey("attackExpirationDate"))
						{
							sqmWarEventData.AttackExpirationTime = Convert.ToUInt32(dictionary2["attackExpirationDate"]);
						}
					}
					if (dictionary2.ContainsKey("level") || dictionary2.ContainsKey("totalRepInvested"))
					{
						SquadMsgUtils.AddSquadLevelToSquadMessageData(dictionary2, squadMsg);
					}
					if (dictionary2.ContainsKey("perkId"))
					{
						SquadMsgUtils.AddPerkUnlockUpgrdeDataToSquadMessageData(dictionary2, squadMsg);
					}
				}
			}
			return squadMsg;
		}

		public static SquadMsg GenerateMessageFromServerMessageObject(object messageObj)
		{
			Dictionary<string, object> dictionary = messageObj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return null;
			}
			SquadMsg squadMsg = new SquadMsg();
			if (dictionary.ContainsKey("notification"))
			{
				squadMsg = SquadMsgUtils.GenerateMessageFromNotifObject(dictionary["notification"]);
				if (dictionary.ContainsKey("guildId"))
				{
					if (squadMsg.SquadData == null)
					{
						squadMsg.SquadData = new SqmSquadData();
					}
					squadMsg.SquadData.Id = Convert.ToString(dictionary["guildId"]);
					if (dictionary.ContainsKey("guildName"))
					{
						squadMsg.SquadData.Name = WWW.UnEscapeURL(Convert.ToString(dictionary["guildName"]));
					}
				}
				return squadMsg;
			}
			if (dictionary.ContainsKey("serverTime"))
			{
				squadMsg.TimeSent = Convert.ToUInt32(dictionary["serverTime"]);
			}
			if (dictionary.ContainsKey("event"))
			{
				string name = Convert.ToString(dictionary["event"]);
				squadMsg.Type = StringUtils.ParseEnum<SquadMsgType>(name);
			}
			if (dictionary.ContainsKey("guildId"))
			{
				SqmSquadData sqmSquadData = new SqmSquadData();
				squadMsg.SquadData = sqmSquadData;
				sqmSquadData.Id = Convert.ToString(dictionary["guildId"]);
				if (dictionary.ContainsKey("guildName"))
				{
					sqmSquadData.Name = WWW.UnEscapeURL(Convert.ToString(dictionary["guildName"]));
				}
			}
			if (dictionary.ContainsKey("senderId"))
			{
				SqmFriendInviteData sqmFriendInviteData = new SqmFriendInviteData();
				squadMsg.FriendInviteData = sqmFriendInviteData;
				sqmFriendInviteData.SenderId = Convert.ToString(dictionary["senderId"]);
				if (dictionary.ContainsKey("senderName"))
				{
					sqmFriendInviteData.SenderName = Convert.ToString(dictionary["senderName"]);
				}
			}
			if (dictionary.ContainsKey("recipientId"))
			{
				if (squadMsg.FriendInviteData == null)
				{
					squadMsg.FriendInviteData = new SqmFriendInviteData();
				}
				squadMsg.FriendInviteData.PlayerId = Convert.ToString(dictionary["recipientId"]);
			}
			if (dictionary.ContainsKey("warId"))
			{
				SqmWarEventData sqmWarEventData = new SqmWarEventData();
				squadMsg.WarEventData = sqmWarEventData;
				sqmWarEventData.WarId = Convert.ToString(dictionary["warId"]);
				if (dictionary.ContainsKey("empireName"))
				{
					sqmWarEventData.EmpireSquadName = Convert.ToString(dictionary["empireName"]);
				}
				if (dictionary.ContainsKey("empireScore"))
				{
					sqmWarEventData.EmpireScore = Convert.ToInt32(dictionary["empireScore"]);
				}
				if (dictionary.ContainsKey("rebelName"))
				{
					sqmWarEventData.RebelSquadName = Convert.ToString(dictionary["rebelName"]);
				}
				if (dictionary.ContainsKey("rebelScore"))
				{
					sqmWarEventData.RebelScore = Convert.ToInt32(dictionary["rebelScore"]);
				}
				if (dictionary.ContainsKey("buffBaseUid"))
				{
					sqmWarEventData.BuffBaseUid = Convert.ToString(dictionary["buffBaseUid"]);
				}
				if (dictionary.ContainsKey("empireCrateTier"))
				{
					sqmWarEventData.EmpireCrateId = Convert.ToString(dictionary["empireCrateTier"]);
				}
				else if (dictionary.ContainsKey("empireCrateId"))
				{
					sqmWarEventData.EmpireCrateId = Convert.ToString(dictionary["empireCrateId"]);
				}
				if (dictionary.ContainsKey("rebelCrateTier"))
				{
					sqmWarEventData.RebelCrateId = Convert.ToString(dictionary["rebelCrateTier"]);
				}
				else if (dictionary.ContainsKey("rebelCrateId"))
				{
					sqmWarEventData.RebelCrateId = Convert.ToString(dictionary["rebelCrateId"]);
				}
			}
			if (dictionary.ContainsKey("level") || dictionary.ContainsKey("totalRepInvested"))
			{
				SquadMsgUtils.AddSquadLevelToSquadMessageData(dictionary, squadMsg);
			}
			if (dictionary.ContainsKey("perkId"))
			{
				SquadMsgUtils.AddPerkUnlockUpgrdeDataToSquadMessageData(dictionary, squadMsg);
			}
			return squadMsg;
		}

		private static void AddSquadLevelToSquadMessageData(Dictionary<string, object> dict, SquadMsg message)
		{
			if (message.SquadData == null)
			{
				message.SquadData = new SqmSquadData();
			}
			int num = 0;
			if (dict.ContainsKey("level"))
			{
				num = Convert.ToInt32(dict["level"]);
			}
			int num2 = 0;
			if (dict.ContainsKey("totalRepInvested"))
			{
				num2 = Convert.ToInt32(dict["totalRepInvested"]);
				message.SquadData.TotalRepInvested = num2;
			}
			if (num == 0 && num2 > 0)
			{
				num = GameUtils.GetSquadLevelFromInvestedRep(num2);
			}
			message.SquadData.Level = num;
		}

		private static void AddPerkUnlockUpgrdeDataToSquadMessageData(Dictionary<string, object> dict, SquadMsg message)
		{
			if (message.PerkData == null)
			{
				message.PerkData = new SqmPerkData();
			}
			message.PerkData.PerkUId = Convert.ToString(dict["perkId"]);
			if (dict.ContainsKey("perkInvestAmt"))
			{
				message.PerkData.PerkInvestedAmt = Convert.ToInt32(dict["perkInvestAmt"]);
			}
		}

		public static SquadMsg GenerateMessageFromChatObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return null;
			}
			SquadMsg squadMsg = new SquadMsg();
			squadMsg.Type = SquadMsgType.Chat;
			SqmChatData sqmChatData = new SqmChatData();
			squadMsg.ChatData = sqmChatData;
			if (dictionary.ContainsKey("text"))
			{
				string json = WWW.UnEscapeURL(Convert.ToString(dictionary["text"]));
				object obj2 = new JsonParser(json).Parse();
				Dictionary<string, object> dictionary2 = obj2 as Dictionary<string, object>;
				if (dictionary2 != null)
				{
					SqmOwnerData sqmOwnerData = new SqmOwnerData();
					squadMsg.OwnerData = sqmOwnerData;
					if (dictionary2.ContainsKey("userId"))
					{
						sqmOwnerData.PlayerId = Convert.ToString(dictionary2["userId"]);
					}
					if (dictionary2.ContainsKey("userName"))
					{
						sqmOwnerData.PlayerName = Convert.ToString(dictionary2["userName"]);
					}
					if (dictionary2.ContainsKey("message"))
					{
						sqmChatData.Message = Convert.ToString(dictionary2["message"]);
					}
					if (dictionary2.ContainsKey("timestamp"))
					{
						squadMsg.TimeSent = Convert.ToUInt32(dictionary2["timestamp"]);
					}
				}
			}
			if (dictionary.ContainsKey("tag"))
			{
				sqmChatData.Tag = Convert.ToString(dictionary["tag"]);
			}
			if (dictionary.ContainsKey("time"))
			{
				sqmChatData.Time = Convert.ToString(dictionary["time"]);
			}
			return squadMsg;
		}

		public static SquadMsg GenerateMessageFromPhotonChatMessage(object sender, PhotonChatMessageTO photonChatMessage)
		{
			if (photonChatMessage == null)
			{
				return null;
			}
			SquadMsg squadMsg = new SquadMsg();
			squadMsg.Type = SquadMsgType.Chat;
			SqmChatData sqmChatData = new SqmChatData();
			squadMsg.ChatData = sqmChatData;
			SqmOwnerData sqmOwnerData = new SqmOwnerData();
			squadMsg.OwnerData = sqmOwnerData;
			sqmOwnerData.PlayerId = (string)sender;
			sqmOwnerData.PlayerName = photonChatMessage.UserName;
			sqmChatData.Message = photonChatMessage.Text;
			squadMsg.TimeSent = Convert.ToUInt32(photonChatMessage.TimeStamp);
			sqmChatData.Tag = string.Empty;
			sqmChatData.Time = string.Empty;
			return squadMsg;
		}

		public static SquadMsg GenerateMessageFromChatMessage(string message)
		{
			return new SquadMsg
			{
				Type = SquadMsgType.Chat,
				TimeSent = ChatTimeConversionUtils.GetUnixTimestamp(),
				OwnerData = new SqmOwnerData
				{
					PlayerId = Service.CurrentPlayer.PlayerId,
					PlayerName = Service.CurrentPlayer.PlayerName
				},
				ChatData = new SqmChatData
				{
					Message = message
				}
			};
		}

		private static SquadMsg CreateActionMessage(SquadAction action, SquadController.ActionCallback callback, object cookie)
		{
			SqmActionData sqmActionData = new SqmActionData();
			sqmActionData.Type = action;
			sqmActionData.Callback = callback;
			sqmActionData.Cookie = cookie;
			SqmOwnerData sqmOwnerData = new SqmOwnerData();
			sqmOwnerData.PlayerId = Service.CurrentPlayer.PlayerId;
			sqmOwnerData.PlayerName = Service.CurrentPlayer.PlayerName;
			return new SquadMsg
			{
				OwnerData = sqmOwnerData,
				ActionData = sqmActionData
			};
		}

		private static SquadMsg CreateMemberIdMessage(string memberId, SquadAction action, SquadController.ActionCallback callback, object cookie)
		{
			SqmMemberData sqmMemberData = new SqmMemberData();
			sqmMemberData.MemberId = memberId;
			SquadMsg squadMsg = SquadMsgUtils.CreateActionMessage(action, callback, cookie);
			squadMsg.MemberData = sqmMemberData;
			return squadMsg;
		}

		public static SquadMsg CreateDemoteMemberMessage(string memberId, SquadController.ActionCallback callback, object cookie)
		{
			SquadMsg squadMsg = SquadMsgUtils.CreateMemberIdMessage(memberId, SquadAction.DemoteMember, callback, cookie);
			squadMsg.MemberData.MemberRole = SquadRole.Member;
			return squadMsg;
		}

		public static SquadMsg CreatePromoteMemberMessage(string memberId, SquadController.ActionCallback callback, object cookie)
		{
			SquadMsg squadMsg = SquadMsgUtils.CreateMemberIdMessage(memberId, SquadAction.PromoteMember, callback, cookie);
			squadMsg.MemberData.MemberRole = SquadRole.Officer;
			return squadMsg;
		}

		public static SquadMsg CreateRemoveMemberMessage(string memberId, SquadController.ActionCallback callback, object cookie)
		{
			return SquadMsgUtils.CreateMemberIdMessage(memberId, SquadAction.RemoveMember, callback, cookie);
		}

		public static SquadMsg CreateAcceptJoinRequestMessage(string requesterId, string biSource, SquadController.ActionCallback callback, object cookie)
		{
			SquadMsg squadMsg = SquadMsgUtils.CreateMemberIdMessage(requesterId, SquadAction.AcceptApplicationToJoin, callback, cookie);
			squadMsg.BISource = biSource;
			return squadMsg;
		}

		public static SquadMsg CreateRejectJoinRequestMessage(string requesterId, SquadController.ActionCallback callback, object cookie)
		{
			return SquadMsgUtils.CreateMemberIdMessage(requesterId, SquadAction.RejectApplicationToJoin, callback, cookie);
		}

		public static SquadMsg CreateWarDonateMessage(string recipientId, Dictionary<string, int> donations, int donationCount, string requestId, SquadController.ActionCallback callback, object cookie)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			SquadMsg squadMsg = SquadMsgUtils.CreateMemberIdMessage(currentPlayer.PlayerId, SquadAction.DonateWarTroops, callback, cookie);
			SqmDonationData sqmDonationData = new SqmDonationData();
			squadMsg.DonationData = sqmDonationData;
			sqmDonationData.RecipientId = recipientId;
			sqmDonationData.Donations = donations;
			sqmDonationData.DonationCount = donationCount;
			sqmDonationData.RequestId = requestId;
			return squadMsg;
		}

		public static SquadMsg CreateDonateMessage(string recipientId, Dictionary<string, int> donations, int donationCount, string requestId, SquadController.ActionCallback callback, object cookie)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			SquadMsg squadMsg = SquadMsgUtils.CreateMemberIdMessage(currentPlayer.PlayerId, SquadAction.DonateTroops, callback, cookie);
			SqmDonationData sqmDonationData = new SqmDonationData();
			squadMsg.DonationData = sqmDonationData;
			sqmDonationData.RecipientId = recipientId;
			sqmDonationData.Donations = donations;
			sqmDonationData.DonationCount = donationCount;
			sqmDonationData.RequestId = requestId;
			return squadMsg;
		}

		private static SquadMsg CreateSquadMessage(string name, string description, string symbolName, int scoreReq, bool openEnrollment, SquadAction action, SquadController.ActionCallback callback, object cookie)
		{
			SqmSquadData sqmSquadData = new SqmSquadData();
			sqmSquadData.Name = name;
			sqmSquadData.Desc = description;
			sqmSquadData.Icon = symbolName;
			sqmSquadData.MinTrophies = scoreReq;
			sqmSquadData.Open = openEnrollment;
			SquadMsg squadMsg = SquadMsgUtils.CreateActionMessage(action, callback, cookie);
			squadMsg.SquadData = sqmSquadData;
			return squadMsg;
		}

		private static SquadMsg CreateSquadMessage(string squadId, SquadAction action, SquadController.ActionCallback callback, object cookie)
		{
			SqmSquadData sqmSquadData = new SqmSquadData();
			sqmSquadData.Id = squadId;
			SquadMsg squadMsg = SquadMsgUtils.CreateActionMessage(action, callback, cookie);
			squadMsg.SquadData = sqmSquadData;
			return squadMsg;
		}

		public static SquadMsg CreateNewSquadMessage(string name, string description, string symbolName, int scoreReq, bool openEnrollment, SquadController.ActionCallback callback, object cookie)
		{
			return SquadMsgUtils.CreateSquadMessage(name, description, symbolName, scoreReq, openEnrollment, SquadAction.Create, callback, cookie);
		}

		public static SquadMsg CreateEditSquadMessage(string description, string symbolName, int scoreReq, bool openEnrollment, SquadController.ActionCallback callback, object cookie)
		{
			return SquadMsgUtils.CreateSquadMessage(null, description, symbolName, scoreReq, openEnrollment, SquadAction.Edit, callback, cookie);
		}

		public static SquadMsg CreateJoinSquadMessage(string squadId, string biSource, SquadController.ActionCallback callback, object cookie)
		{
			SquadMsg squadMsg = SquadMsgUtils.CreateSquadMessage(squadId, SquadAction.Join, callback, cookie);
			squadMsg.BISource = biSource;
			return squadMsg;
		}

		public static SquadMsg CreateApplyToJoinSquadMessage(string squadId, string message, SquadController.ActionCallback callback, object cookie)
		{
			SqmRequestData sqmRequestData = new SqmRequestData();
			sqmRequestData.Text = message;
			SquadMsg squadMsg = SquadMsgUtils.CreateSquadMessage(squadId, SquadAction.ApplyToJoin, callback, cookie);
			squadMsg.RequestData = sqmRequestData;
			return squadMsg;
		}

		public static SquadMsg CreateLeaveSquadMessage(SquadController.ActionCallback callback, object cookie)
		{
			return SquadMsgUtils.CreateActionMessage(SquadAction.Leave, callback, cookie);
		}

		public static SquadMsg CreateAcceptSquadInviteMessage(string squadId, SquadController.ActionCallback callback, object cookie)
		{
			return SquadMsgUtils.CreateSquadMessage(squadId, SquadAction.AcceptInviteToJoin, callback, cookie);
		}

		public static SquadMsg CreateRejectSquadInviteMessage(string squadId, SquadController.ActionCallback callback, object cookie)
		{
			return SquadMsgUtils.CreateSquadMessage(squadId, SquadAction.RejectInviteToJoin, callback, cookie);
		}

		public static SquadMsg CreateSendInviteMessage(string recipientId, string fbFriendId, string fbAccessToken, SquadController.ActionCallback callback, object cookie)
		{
			SqmFriendInviteData sqmFriendInviteData = new SqmFriendInviteData();
			sqmFriendInviteData.PlayerId = recipientId;
			sqmFriendInviteData.FacebookFriendId = fbFriendId;
			sqmFriendInviteData.FacebookAccessToken = fbAccessToken;
			SquadMsg squadMsg = SquadMsgUtils.CreateActionMessage(SquadAction.SendInviteToJoin, callback, cookie);
			squadMsg.FriendInviteData = sqmFriendInviteData;
			return squadMsg;
		}

		public static SquadMsg CreateRequestWarTroopsMessage(bool payToSkip, int resendCrystalCost, string message)
		{
			SqmRequestData sqmRequestData = new SqmRequestData();
			sqmRequestData.PayToSkip = payToSkip;
			sqmRequestData.ResendCrystalCost = resendCrystalCost;
			sqmRequestData.Text = message;
			SquadMsg squadMsg = SquadMsgUtils.CreateActionMessage(SquadAction.RequestWarTroops, null, null);
			squadMsg.RequestData = sqmRequestData;
			return squadMsg;
		}

		public static SquadMsg CreateRequestTroopsMessage(bool payToSkip, int resendCrystalCost, string message)
		{
			SqmRequestData sqmRequestData = new SqmRequestData();
			sqmRequestData.PayToSkip = payToSkip;
			sqmRequestData.ResendCrystalCost = resendCrystalCost;
			sqmRequestData.Text = message;
			SquadMsg squadMsg = SquadMsgUtils.CreateActionMessage(SquadAction.RequestTroops, null, null);
			squadMsg.RequestData = sqmRequestData;
			return squadMsg;
		}

		public static SquadMsg CreateSendReplayMessage(string battleId, string message)
		{
			SqmReplayData sqmReplayData = new SqmReplayData();
			sqmReplayData.BattleId = battleId;
			sqmReplayData.Text = message;
			SquadMsg squadMsg = SquadMsgUtils.CreateActionMessage(SquadAction.ShareReplay, null, null);
			squadMsg.ReplayData = sqmReplayData;
			return squadMsg;
		}

		public static SquadMsg CreateShareReplayMessage(string message, BattleEntry entry)
		{
			SqmReplayData sqmReplayData = new SqmReplayData();
			sqmReplayData.BattleId = entry.RecordID;
			sqmReplayData.Text = message;
			SquadMsg squadMsg = SquadMsgUtils.CreateActionMessage(SquadAction.ShareReplay, null, null);
			squadMsg.ReplayData = sqmReplayData;
			return squadMsg;
		}

		public static SquadMsg CreateStartMatchmakingMessage(List<string> memberIds, bool allowSameFaction)
		{
			SqmWarParticipantData sqmWarParticipantData = new SqmWarParticipantData();
			int i = 0;
			int count = memberIds.Count;
			while (i < count)
			{
				sqmWarParticipantData.Participants.Add(memberIds[i]);
				i++;
			}
			sqmWarParticipantData.AllowSameFactionMatchMaking = allowSameFaction;
			SquadMsg squadMsg = SquadMsgUtils.CreateActionMessage(SquadAction.StartWarMatchmaking, null, null);
			squadMsg.WarParticipantData = sqmWarParticipantData;
			return squadMsg;
		}

		public static SquadMsg CreateCancelMatchmakingMessage()
		{
			return SquadMsgUtils.CreateActionMessage(SquadAction.CancelWarMatchmaking, null, null);
		}
	}
}
