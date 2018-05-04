using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Leaderboard;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaRTS.Main.Utils
{
	public static class SquadUtils
	{
		private const string GENERIC_SQUAD_ISSUE = "GENERIC_SQUAD_ISSUE";

		private const string GENERIC_SQUAD_JOIN_ISSUE = "GENERIC_SQUAD_JOIN_ISSUE";

		private const string GENERIC_SQUAD_CREATE_ISSUE = "GENERIC_SQUAD_CREATE_ISSUE";

		private const string GENERIC_SQUAD_EDIT_ISSUE = "GENERIC_SQUAD_EDIT_ISSUE";

		private const string GENERIC_SQUAD_LEAVE_ISSUE = "GENERIC_SQUAD_LEAVE_ISSUE";

		private const string GENERIC_DONATE_ISSUE = "GENERIC_DONATE_ISSUE";

		private const string PLAYER_IS_IN_SQUAD_WAR = "PLAYER_IS_IN_SQUAD_WAR";

		private const string ALREADY_IN_A_SQUAD = "ALREADY_IN_A_GUILD";

		private const string JOIN_SQUAD_IS_FULL = "GUILD_IS_FULL";

		private const string JOIN_SQUAD_IS_PRIVATE = "GUILD_IS_NOT_OPEN_ENROLLMENT";

		private const string SQUAD_SCORE_REQ_NOT_MET = "GUILD_SCORE_REQUIREMENT_NOT_MET";

		private const string SQUAD_WRONG_FACTION = "IN_WRONG_FACTION";

		private const string SQUAD_NAME_TAKEN = "GUILD_NAME_ALREADY_TAKEN";

		private const string SQUAD_NAME_INVALID = "INVALID_SQUAD_NAME";

		private const string SQUAD_DESC_INVALID = "INVALID_SQUAD_DESC";

		private const string NOT_IN_SQUAD = "NOT_IN_GUILD";

		private const string INVALID_TROOPS = "CAN_ONLY_DONATE_TROOPS";

		private const string SQUAD_TROOPS_FULL = "NOT_ENOUGH_GUILD_TROOP_CAPACITY";

		private const string NOT_ENOUGH_TROOPS = "CANNOT_DEDUCT_NEGATIVE_AMOUNTS";

		private const string NOT_IN_SAME_SQUAD = "NOT_IN_SAME_GUILD";

		private const string SQUAD_INVITE_ALREADY_IN_A_SQUAD = "SQUAD_INVITE_ALREADY_IN_A_GUILD";

		private const string SQUAD_INVITE_SQUAD_IS_FULL = "SQUAD_INVITE_GUILD_IS_FULL";

		private const string SQUAD_INVITE_WRONG_FACTION = "SQUAD_INVITE_WRONG_FACTION";

		private const string SQUAD_INVITE_NOT_ENOUGH_SQUAD_RANK = "SQUAD_INVITE_NOT_ENOUGH_GUILD_RANK";

		private const string SQUAD_INVITE_FAILED = "SQUAD_INVITE_FAILED";

		private const string SQUAD_INVITE_REJECT_FAILED = "SQUAD_INVITE_REJECT_FAILED";

		private const string ALREADY_IN_THIS_SQUAD = "ALREADY_IN_THIS_SQUAD";

		private const string ALREADY_IN_SQUAD = "ALREADY_IN_SQUAD";

		private const string SQUAD_IS_WRONG_FACTION = "SQUAD_IS_WRONG_FACTION";

		private const string SQUAD_IS_FULL = "SQUAD_IS_FULL";

		private const string INSUFFICIENT_TROPHIES = "INSUFFICIENT_TROPHIES";

		private const string NOT_IN_ACTION_PHASE_TEXT_PREFIX = "WAR_SCOUT_NOT_IN_ACTION_PHASE_ERROR_";

		private const string NO_TURNS_LEFT_TEXT_PREFIX = "WAR_SCOUT_NO_TURNS_LEFT_ERROR_";

		private const string NOT_WAR_PARTICIPANT_TEXT_PREFIX = "WAR_SCOUT_NOT_IN_WAR_ERROR_";

		private const string OPPONENT_HAS_NO_VICTORY_POINTS_TEXT_PREFIX = "WAR_SCOUT_NO_VICTORY_POINTS_LEFT_ERROR_";

		private const string UNDER_ATTACK_TEXT_PREFIX = "WAR_SCOUT_UNDER_ATTACK_ERROR_";

		private const string PLAYER_UNDER_ATTACK_PREFIX = "WAR_SCOUT_UNDER_PLAYER_ATTACK_ERROR_";

		private const string WAR_ERROR_BUFF_BASE_OWNED = "WAR_ERROR_BUFF_BASE_OWNED";

		private const string WAR_ERROR_BUFF_BASE_OWNER_CHANGED = "WAR_ERROR_BUFF_BASE_OWNER_CHANGED";

		private const string PVP_TEXT_POSTFIX = "PVP";

		private const string PVE_TEXT_POSTFIX = "PVE";

		public static string GetFailureStringIdByScoutState(SquadWarScoutState state, bool pvp)
		{
			StringBuilder stringBuilder = new StringBuilder(string.Empty);
			switch (state)
			{
			case SquadWarScoutState.NotInActionPhase:
				stringBuilder.Append("WAR_SCOUT_NOT_IN_ACTION_PHASE_ERROR_");
				break;
			case SquadWarScoutState.NoTurnsLeft:
				stringBuilder.Append("WAR_SCOUT_NO_TURNS_LEFT_ERROR_");
				break;
			case SquadWarScoutState.NotPatricipantInWar:
				stringBuilder.Append("WAR_SCOUT_NOT_IN_WAR_ERROR_");
				break;
			case SquadWarScoutState.UnderAttack:
				stringBuilder.Append("WAR_SCOUT_UNDER_ATTACK_ERROR_");
				break;
			case SquadWarScoutState.OpponentHasNoVictoryPointsLeft:
				stringBuilder.Append("WAR_SCOUT_NO_VICTORY_POINTS_LEFT_ERROR_");
				break;
			}
			if (pvp)
			{
				stringBuilder.Append("PVP");
			}
			else
			{
				stringBuilder.Append("PVE");
			}
			return stringBuilder.ToString();
		}

		public static void ForceCloseSquadWarScreen()
		{
			if (Service.WorldTransitioner.IsTransitioning())
			{
				return;
			}
			SquadWarScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<SquadWarScreen>();
			if (highestLevelScreen != null)
			{
				highestLevelScreen.Close(null);
			}
		}

		public static string GetFailureStringIdByStatus(uint status, bool isPvp)
		{
			string str = string.Empty;
			if (status == 2402u)
			{
				return "WAR_ERROR_BUFF_BASE_OWNED";
			}
			if (status == 2403u)
			{
				str = "WAR_SCOUT_UNDER_ATTACK_ERROR_";
			}
			else if (status == 2404u)
			{
				str = "WAR_SCOUT_UNDER_ATTACK_ERROR_";
			}
			else if (status == 2406u)
			{
				str = "WAR_SCOUT_NO_TURNS_LEFT_ERROR_";
			}
			else if (status == 2407u)
			{
				str = "WAR_SCOUT_NO_VICTORY_POINTS_LEFT_ERROR_";
			}
			else if (status == 2409u)
			{
				str = "WAR_SCOUT_NOT_IN_ACTION_PHASE_ERROR_";
			}
			else if (status == 2418u)
			{
				str = "WAR_SCOUT_UNDER_PLAYER_ATTACK_ERROR_";
			}
			else if (status == 2421u)
			{
				return "WAR_ERROR_BUFF_BASE_OWNER_CHANGED";
			}
			string str2 = "PVE";
			if (isPvp)
			{
				str2 = "PVP";
			}
			return str + str2;
		}

		public static bool IsPlayerInSquad(string playerId, Squad squad)
		{
			int i = 0;
			int count = squad.MemberList.Count;
			while (i < count)
			{
				if (playerId == squad.MemberList[i].MemberID)
				{
					return true;
				}
				i++;
			}
			return false;
		}

		public static SquadMember GetSquadMemberById(Squad squad, string memberId)
		{
			if (squad != null && squad.MemberList != null)
			{
				int i = 0;
				int count = squad.MemberList.Count;
				while (i < count)
				{
					if (squad.MemberList[i].MemberID == memberId)
					{
						return squad.MemberList[i];
					}
					i++;
				}
			}
			return null;
		}

		public static List<SquadDonatedTroop> GetWorldOwnerSquadBuildingTroops()
		{
			List<SquadDonatedTroop> result;
			if (GameUtils.IsVisitingNeighbor())
			{
				result = Service.NeighborVisitManager.NeighborSquadTroops;
			}
			else
			{
				result = Service.SquadController.StateManager.Troops;
			}
			return result;
		}

		public static int GetDonatedTroopStorageUsedByWorldOwner()
		{
			List<SquadDonatedTroop> worldOwnerSquadBuildingTroops = SquadUtils.GetWorldOwnerSquadBuildingTroops();
			return SquadUtils.GetDonatedTroopStorageUsed(worldOwnerSquadBuildingTroops);
		}

		public static int GetDonatedTroopStorageUsedByCurrentPlayer()
		{
			List<SquadDonatedTroop> troops = Service.SquadController.StateManager.Troops;
			return SquadUtils.GetDonatedTroopStorageUsed(troops);
		}

		public static int GetDonatedWarTroopStorageUsedByCurrentPlayer()
		{
			SquadMemberWarData currentMemberWarData = Service.SquadController.WarManager.GetCurrentMemberWarData();
			if (currentMemberWarData != null)
			{
				List<SquadDonatedTroop> warTroops = currentMemberWarData.WarTroops;
				return SquadUtils.GetDonatedTroopStorageUsed(warTroops);
			}
			return 0;
		}

		private static int GetDonatedTroopStorageUsed(List<SquadDonatedTroop> donatedTroops)
		{
			int num = 0;
			if (donatedTroops != null)
			{
				StaticDataController staticDataController = Service.StaticDataController;
				int i = 0;
				int count = donatedTroops.Count;
				while (i < count)
				{
					TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(donatedTroops[i].TroopUid);
					num += donatedTroops[i].GetTotalAmount() * troopTypeVO.Size;
					i++;
				}
			}
			return num;
		}

		public static List<SquadDonatedTroop> GetSquadDonatedTroopsFromObject(object obj)
		{
			List<SquadDonatedTroop> list = new List<SquadDonatedTroop>();
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				foreach (KeyValuePair<string, object> current in dictionary)
				{
					Dictionary<string, object> dictionary2 = current.Value as Dictionary<string, object>;
					if (dictionary2 != null)
					{
						string key = current.Key;
						SquadDonatedTroop squadDonatedTroop = new SquadDonatedTroop(key);
						list.Add(squadDonatedTroop);
						foreach (KeyValuePair<string, object> current2 in dictionary2)
						{
							string key2 = current2.Key;
							int value = Convert.ToInt32(current2.Value);
							squadDonatedTroop.SenderAmounts.Add(key2, value);
						}
					}
				}
			}
			return list;
		}

		public static bool CanCurrentPlayerJoinSquad(CurrentPlayer player, Squad currentSquad, Squad squad, Lang lang, out string details)
		{
			details = null;
			if (currentSquad != null && currentSquad.SquadID == squad.SquadID)
			{
				details = lang.Get("ALREADY_IN_THIS_SQUAD", new object[0]);
				return false;
			}
			if (player.Faction != squad.Faction)
			{
				details = lang.Get("SQUAD_IS_WRONG_FACTION", new object[0]);
				return false;
			}
			if (squad.MemberCount >= squad.MemberMax)
			{
				details = lang.Get("SQUAD_IS_FULL", new object[0]);
				return false;
			}
			if (player.PlayerMedals < squad.RequiredTrophies)
			{
				details = lang.Get("INSUFFICIENT_TROPHIES", new object[0]);
				return false;
			}
			return true;
		}

		public static string GetMessageForServerActionFailure(SquadAction actionType, uint status)
		{
			string text = null;
			switch (status)
			{
			case 2300u:
				text = "ALREADY_IN_A_GUILD";
				goto IL_89;
			case 2301u:
				IL_2C:
				if (status != 2321u)
				{
					goto IL_89;
				}
				text = "PLAYER_IS_IN_SQUAD_WAR";
				goto IL_89;
			case 2302u:
				text = "GUILD_IS_FULL";
				goto IL_89;
			case 2303u:
				text = "GUILD_IS_NOT_OPEN_ENROLLMENT";
				goto IL_89;
			case 2304u:
				text = "GUILD_SCORE_REQUIREMENT_NOT_MET";
				goto IL_89;
			case 2305u:
				text = "IN_WRONG_FACTION";
				goto IL_89;
			case 2306u:
				text = "NOT_IN_GUILD";
				goto IL_89;
			}
			goto IL_2C;
			IL_89:
			switch (actionType)
			{
			case SquadAction.Create:
				if (text == null)
				{
					if (status != 701u)
					{
						if (status != 2301u)
						{
							text = "GENERIC_SQUAD_CREATE_ISSUE";
						}
						else
						{
							text = "GUILD_NAME_ALREADY_TAKEN";
						}
					}
					else
					{
						text = "INVALID_SQUAD_NAME";
					}
				}
				return text;
			case SquadAction.Join:
			case SquadAction.ApplyToJoin:
			case SquadAction.AcceptApplicationToJoin:
				if (text == null)
				{
					text = "GENERIC_SQUAD_JOIN_ISSUE";
				}
				return text;
			case SquadAction.Leave:
				if (text == null)
				{
					text = "GENERIC_SQUAD_LEAVE_ISSUE";
				}
				return text;
			case SquadAction.Edit:
				if (text == null)
				{
					if (status != 701u)
					{
						text = "GENERIC_SQUAD_EDIT_ISSUE";
					}
					else
					{
						text = "INVALID_SQUAD_DESC";
					}
				}
				return text;
			case SquadAction.SendInviteToJoin:
				switch (status)
				{
				case 2300u:
					text = "SQUAD_INVITE_ALREADY_IN_A_GUILD";
					goto IL_248;
				case 2301u:
				case 2303u:
				case 2304u:
					IL_201:
					if (status != 2309u)
					{
						text = "SQUAD_INVITE_FAILED";
						goto IL_248;
					}
					text = "SQUAD_INVITE_NOT_ENOUGH_GUILD_RANK";
					goto IL_248;
				case 2302u:
					text = "SQUAD_INVITE_GUILD_IS_FULL";
					goto IL_248;
				case 2305u:
					text = "SQUAD_INVITE_WRONG_FACTION";
					goto IL_248;
				}
				goto IL_201;
				IL_248:
				return text;
			case SquadAction.RejectInviteToJoin:
				text = "SQUAD_INVITE_REJECT_FAILED";
				return text;
			case SquadAction.DonateTroops:
			case SquadAction.DonateWarTroops:
				switch (status)
				{
				case 2315u:
					text = "NOT_IN_SAME_GUILD";
					goto IL_1D6;
				case 2316u:
					text = "CANNOT_DEDUCT_NEGATIVE_AMOUNTS";
					goto IL_1D6;
				case 2318u:
					text = "CAN_ONLY_DONATE_TROOPS";
					goto IL_1D6;
				case 2319u:
					text = "NOT_ENOUGH_GUILD_TROOP_CAPACITY";
					goto IL_1D6;
				}
				text = "GENERIC_DONATE_ISSUE";
				IL_1D6:
				return text;
			}
			text = "GENERIC_SQUAD_ISSUE";
			return text;
		}

		public static bool IsNotFatalServerError(uint status)
		{
			return (status >= 2300u && status <= 2322u) || status == 14u || status == 2413u || status == 2414u || status == 2402u || status == 2403u || status == 2404u || status == 2406u || status == 2407u || status == 2409u || status == 2418u || status == 2421u || status == 2320u;
		}

		public static int GetTroopRequestCrystalCost(uint currentServerTime, uint troopRequestDate)
		{
			return GameUtils.SecondsToCrystals(SquadUtils.GetTroopRequestTimeLeft(currentServerTime, troopRequestDate));
		}

		public static int GetTroopRequestTimeLeft(uint currentServerTime, uint troopRequestDate)
		{
			int troopRequestCooldownTime = Service.PerkManager.GetTroopRequestCooldownTime();
			uint num = troopRequestDate + (uint)((ulong)(GameConstants.SQUAD_TROOP_REQUEST_THROTTLE_MINUTES * 60u) - (ulong)((long)troopRequestCooldownTime));
			int num2 = (int)(num - currentServerTime);
			if (num2 < 0)
			{
				num2 = 0;
			}
			return num2;
		}

		public static int GetReputationReqForSquadLevel(int currLevel)
		{
			int result = -1;
			StaticDataController staticDataController = Service.StaticDataController;
			string squadLevelUIDFromLevel = GameUtils.GetSquadLevelUIDFromLevel(currLevel);
			SquadLevelVO optional = staticDataController.GetOptional<SquadLevelVO>(squadLevelUIDFromLevel);
			if (optional != null)
			{
				result = optional.RepThreshold;
			}
			return result;
		}

		public static bool CanSendFreeTroopRequest(uint currentServerTime, uint troopRequestDate)
		{
			return SquadUtils.GetTroopRequestTimeLeft(currentServerTime, troopRequestDate) <= 0;
		}

		public static bool IsPlayerSquadWarTroopsAtMaxCapacity()
		{
			SquadMemberWarData currentMemberWarData = Service.SquadController.WarManager.GetCurrentMemberWarData();
			if (currentMemberWarData != null)
			{
				int donatedWarTroopStorageUsedByCurrentPlayer = SquadUtils.GetDonatedWarTroopStorageUsedByCurrentPlayer();
				int squadStorageCapacity = currentMemberWarData.BaseMap.GetSquadStorageCapacity();
				return donatedWarTroopStorageUsedByCurrentPlayer >= squadStorageCapacity;
			}
			return false;
		}

		public static void SetSquadMemberRole(Squad squad, string memberId, SquadRole role)
		{
			SquadMember squadMemberById = SquadUtils.GetSquadMemberById(squad, memberId);
			if (squadMemberById != null)
			{
				squadMemberById.Role = role;
			}
		}

		public static void RemoveSquadMember(Squad squad, string memberId)
		{
			SquadMember squadMemberById = SquadUtils.GetSquadMemberById(squad, memberId);
			if (squadMemberById != null && squad != null)
			{
				squad.MemberList.Remove(squadMemberById);
				squad.BattleScore -= squadMemberById.Score;
				squad.MemberCount--;
			}
		}

		public static void AddSquadMember(Squad squad, SquadMember newMember)
		{
			int i = 0;
			int count = squad.MemberList.Count;
			while (i < count)
			{
				if (newMember.MemberID == squad.MemberList[i].MemberID)
				{
					return;
				}
				i++;
			}
			squad.MemberList.Add(newMember);
			squad.MemberList.Sort();
			squad.MemberCount = squad.MemberList.Count;
			squad.BattleScore += newMember.Score;
		}

		public static int GetDonationCount(List<SquadMsg> msgs, string requestId, string playerId)
		{
			int num = 0;
			int i = 0;
			int count = msgs.Count;
			while (i < count)
			{
				SquadMsg squadMsg = msgs[i];
				if (squadMsg.OwnerData != null && squadMsg.OwnerData.PlayerId == playerId && squadMsg.DonationData != null && squadMsg.DonationData.RequestId == requestId)
				{
					num += squadMsg.DonationData.DonationCount;
				}
				i++;
			}
			return num;
		}

		public static List<string> GetFriendIdsInSquad(string squadId, LeaderboardController lbc)
		{
			List<string> list = null;
			if (!string.IsNullOrEmpty(squadId))
			{
				List<PlayerLBEntity> list2 = lbc.Friends.List;
				int i = 0;
				int count = list2.Count;
				while (i < count)
				{
					if (squadId == list2[i].SquadID)
					{
						if (list == null)
						{
							list = new List<string>();
						}
						list.Add(list2[i].PlayerID);
					}
					i++;
				}
			}
			return list;
		}

		public static string GetFriendIdsString(List<string> friendIds)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			int count = friendIds.Count;
			while (i < count)
			{
				stringBuilder.Append(friendIds[i]);
				stringBuilder.Append(",");
				i++;
			}
			return stringBuilder.ToString();
		}

		public static bool IsPlayerMedalCountHigherThanSquadAvg(Squad squad, int playerMedals)
		{
			if (squad != null)
			{
				int count = squad.MemberList.Count;
				if (count > 0)
				{
					int num = 0;
					for (int i = 0; i < count; i++)
					{
						SquadMember squadMember = squad.MemberList[i];
						num += squadMember.Score;
					}
					if (playerMedals > num / count)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool IsTimeWithinSquadWarPhase(SquadWarData data, uint serverTime)
		{
			bool result = false;
			if (data != null && (ulong)serverTime >= (ulong)((long)data.StartTimeStamp) && (ulong)serverTime < (ulong)((long)data.CooldownEndTimeStamp))
			{
				SquadWarStatusType warStatus = SquadUtils.GetWarStatus(data, Service.ServerAPI.ServerTime);
				int num = 0;
				int num2 = 0;
				if (SquadUtils.FillOutWarPhaseTimeRange(data, warStatus, out num, out num2))
				{
					result = ((ulong)serverTime >= (ulong)((long)num) && (ulong)serverTime < (ulong)((long)num2));
				}
			}
			return result;
		}

		public static bool FillOutWarPhaseTimeRange(SquadWarData data, SquadWarStatusType type, out int startTime, out int endTime)
		{
			bool result = false;
			startTime = 0;
			endTime = 0;
			if (data != null)
			{
				switch (type)
				{
				case SquadWarStatusType.PhasePrep:
				case SquadWarStatusType.PhasePrepGrace:
					result = true;
					startTime = data.StartTimeStamp;
					endTime = data.PrepEndTimeStamp;
					break;
				case SquadWarStatusType.PhaseAction:
				case SquadWarStatusType.PhaseActionGrace:
					result = true;
					startTime = data.PrepEndTimeStamp;
					endTime = data.ActionEndTimeStamp;
					break;
				case SquadWarStatusType.PhaseCooldown:
					result = true;
					startTime = data.ActionEndTimeStamp;
					endTime = data.CooldownEndTimeStamp;
					break;
				}
			}
			return result;
		}

		public static SquadWarStatusType GetWarStatus(SquadWarData data, uint serverTime)
		{
			if (data == null)
			{
				return SquadWarStatusType.PhaseOpen;
			}
			if ((ulong)serverTime >= (ulong)((long)data.PrepGraceStartTimeStamp) && (ulong)serverTime <= (ulong)((long)data.PrepEndTimeStamp))
			{
				return SquadWarStatusType.PhasePrepGrace;
			}
			if ((ulong)serverTime < (ulong)((long)data.PrepEndTimeStamp))
			{
				return SquadWarStatusType.PhasePrep;
			}
			if ((ulong)serverTime >= (ulong)((long)data.ActionGraceStartTimeStamp) && (ulong)serverTime <= (ulong)((long)data.ActionEndTimeStamp))
			{
				return SquadWarStatusType.PhaseActionGrace;
			}
			if ((ulong)serverTime < (ulong)((long)data.ActionEndTimeStamp))
			{
				return SquadWarStatusType.PhaseAction;
			}
			if ((ulong)serverTime <= (ulong)((long)data.CooldownEndTimeStamp))
			{
				return SquadWarStatusType.PhaseCooldown;
			}
			return SquadWarStatusType.PhaseOpen;
		}

		public static bool CanLeaveSquad()
		{
			bool result = true;
			SquadController squadController = Service.SquadController;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			Squad currentSquad = squadController.StateManager.GetCurrentSquad();
			if (currentSquad == null)
			{
				return result;
			}
			SquadWarStatusType currentStatus = squadController.WarManager.GetCurrentStatus();
			if (currentStatus == SquadWarStatusType.PhaseCooldown)
			{
				return result;
			}
			SquadMember squadMemberById = SquadUtils.GetSquadMemberById(currentSquad, currentPlayer.PlayerId);
			if (squadController.WarManager.IsMemberInWarParty(squadMemberById.MemberID) || squadController.WarManager.IsSquadMemberInWarOrMatchmaking(squadMemberById))
			{
				result = false;
			}
			return result;
		}

		public static SquadWarRewardData GetRewardForWar(string warId, SquadMemberWarData memberWarData)
		{
			if (memberWarData == null)
			{
				return null;
			}
			SquadWarRewardData result = null;
			List<SquadWarRewardData> warRewards = memberWarData.WarRewards;
			int i = 0;
			int count = warRewards.Count;
			while (i < count)
			{
				if (warRewards[i].WarId == warId)
				{
					result = warRewards[i];
					break;
				}
				i++;
			}
			return result;
		}

		public static int GetUnclaimedSquadWarRewardsCount(SquadMemberWarData memberWarData, uint serverTime)
		{
			int num = 0;
			if (memberWarData == null)
			{
				return num;
			}
			List<SquadWarRewardData> warRewards = memberWarData.WarRewards;
			int i = 0;
			int count = warRewards.Count;
			while (i < count)
			{
				if (warRewards[i].ExpireDate > serverTime)
				{
					num++;
				}
				i++;
			}
			return num;
		}

		public static bool CanStartMatchmakingPrep(SquadController squadController, BuildingLookupController blc)
		{
			SquadRole role = squadController.StateManager.Role;
			bool flag = role == SquadRole.Owner || role == SquadRole.Officer;
			int highestLevelHQ = blc.GetHighestLevelHQ();
			bool flag2 = highestLevelHQ >= GameConstants.WAR_PARTICIPANT_MIN_LEVEL;
			SquadWarStatusType currentStatus = squadController.WarManager.GetCurrentStatus();
			bool flag3 = currentStatus == SquadWarStatusType.PhaseCooldown || currentStatus == SquadWarStatusType.PhaseOpen;
			SquadWarData currentSquadWar = squadController.WarManager.CurrentSquadWar;
			bool flag4 = currentSquadWar == null || currentSquadWar.RewardsProcessed;
			return flag2 && flag && flag3 && flag4;
		}

		public static bool SquadMeetsMatchmakingRequirements(SquadController squadController)
		{
			int num = 0;
			Squad currentSquad = squadController.StateManager.GetCurrentSquad();
			int i = 0;
			int memberCount = currentSquad.MemberCount;
			while (i < memberCount)
			{
				if (currentSquad.MemberList[i].HQLevel >= GameConstants.WAR_PARTICIPANT_MIN_LEVEL)
				{
					num++;
				}
				i++;
			}
			return num >= GameConstants.WAR_PARTICIPANT_COUNT;
		}

		public static bool DoesRewardWithoutWarHistoryExist(SquadController squadController, SquadMemberWarData memberWarData, uint serverTime)
		{
			if (memberWarData == null)
			{
				return false;
			}
			Squad currentSquad = squadController.StateManager.GetCurrentSquad();
			List<SquadWarRewardData> warRewards = memberWarData.WarRewards;
			int i = 0;
			int count = warRewards.Count;
			while (i < count)
			{
				SquadWarRewardData squadWarRewardData = warRewards[i];
				if (squadWarRewardData.ExpireDate > serverTime)
				{
					bool flag = false;
					int j = 0;
					int count2 = currentSquad.WarHistory.Count;
					while (j < count2)
					{
						SquadWarHistoryEntry squadWarHistoryEntry = currentSquad.WarHistory[j];
						if (squadWarHistoryEntry.WarId == squadWarRewardData.WarId)
						{
							flag = true;
							break;
						}
						j++;
					}
					if (!flag)
					{
						return true;
					}
				}
				i++;
			}
			return false;
		}
	}
}
