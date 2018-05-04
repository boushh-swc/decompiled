using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Commands.Squads;
using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using StaRTS.Main.Models.Leaderboard;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class LeaderboardController : IEventObserver
	{
		public delegate void OnUpdateData(bool success);

		public delegate void OnUpdateSquadData(Squad squad, bool success);

		private const string PARAM_CALLBACK = "callback";

		private const string PARAM_LIST = "list";

		private const string PARAM_NEARBY = "listnearby";

		private List<Squad> cachedSquads;

		private List<PlayerLBEntity> cachedPlayers;

		private const int DATA_REFRESH_THROTTLE = 30;

		private const int RECHECK_LB_FRIENDS_COUNT = 0;

		private const int RECHECK_LB_LEADERS_COUNT = 10;

		public List<PlayerLBEntity> topPlayers;

		public LeaderboardList<Squad> TopSquads
		{
			get;
			private set;
		}

		public LeaderboardList<Squad> SquadsNearMe
		{
			get;
			private set;
		}

		public LeaderboardList<Squad> FeaturedSquads
		{
			get;
			private set;
		}

		public LeaderboardList<Squad> SearchedSquads
		{
			get;
			private set;
		}

		public LeaderboardList<PlayerLBEntity> Friends
		{
			get;
			private set;
		}

		public LeaderboardList<PlayerLBEntity> GlobalLeaders
		{
			get;
			private set;
		}

		public LeaderboardList<PlayerLBEntity> GlobalNearMeLeaders
		{
			get;
			private set;
		}

		public Dictionary<string, LeaderboardList<PlayerLBEntity>> LeadersByPlanet
		{
			get;
			private set;
		}

		public Dictionary<string, LeaderboardList<PlayerLBEntity>> LeadersNearMeByPlanet
		{
			get;
			private set;
		}

		public Dictionary<string, LeaderboardList<PlayerLBEntity>> TournamentLeadersByPlanet
		{
			get;
			private set;
		}

		public Dictionary<string, LeaderboardList<PlayerLBEntity>> TournamentLeadersNearMeByPlanet
		{
			get;
			private set;
		}

		public LeaderboardController()
		{
			Service.LeaderboardController = this;
			this.TopSquads = new LeaderboardList<Squad>();
			this.SquadsNearMe = new LeaderboardList<Squad>();
			this.FeaturedSquads = new LeaderboardList<Squad>();
			this.SearchedSquads = new LeaderboardList<Squad>();
			this.Friends = new LeaderboardList<PlayerLBEntity>();
			this.GlobalLeaders = new LeaderboardList<PlayerLBEntity>();
			this.GlobalNearMeLeaders = new LeaderboardList<PlayerLBEntity>();
			this.LeadersByPlanet = new Dictionary<string, LeaderboardList<PlayerLBEntity>>();
			this.LeadersNearMeByPlanet = new Dictionary<string, LeaderboardList<PlayerLBEntity>>();
			this.TournamentLeadersByPlanet = new Dictionary<string, LeaderboardList<PlayerLBEntity>>();
			this.TournamentLeadersNearMeByPlanet = new Dictionary<string, LeaderboardList<PlayerLBEntity>>();
			this.topPlayers = new List<PlayerLBEntity>();
			this.cachedSquads = new List<Squad>();
			this.cachedPlayers = new List<PlayerLBEntity>();
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.SquadJoinedByCurrentPlayer);
			eventManager.RegisterObserver(this, EventId.SquadLeft);
		}

		public void InitLeaderBoardListForPlanet()
		{
			List<PlanetVO> allPlayerFacingPlanets = PlanetUtils.GetAllPlayerFacingPlanets();
			for (int i = 0; i < allPlayerFacingPlanets.Count; i++)
			{
				string uid = allPlayerFacingPlanets[i].Uid;
				LeaderboardList<PlayerLBEntity> value = new LeaderboardList<PlayerLBEntity>();
				this.LeadersByPlanet.Add(uid, value);
				value = new LeaderboardList<PlayerLBEntity>();
				this.LeadersNearMeByPlanet.Add(uid, value);
			}
			List<TournamentVO> allLiveAndClosingTournaments = TournamentController.GetAllLiveAndClosingTournaments();
			int j = 0;
			int count = allLiveAndClosingTournaments.Count;
			while (j < count)
			{
				string planetId = allLiveAndClosingTournaments[j].PlanetId;
				if (this.TournamentLeadersByPlanet.ContainsKey(planetId))
				{
					Service.Logger.Error("Multiple tournaments are active on planet " + planetId);
				}
				else
				{
					this.InitTournamentListForPlanet(planetId);
				}
				j++;
			}
		}

		private void InitTournamentListForPlanet(string planetUid)
		{
			LeaderboardList<PlayerLBEntity> value = new LeaderboardList<PlayerLBEntity>();
			this.TournamentLeadersByPlanet.Add(planetUid, value);
			value = new LeaderboardList<PlayerLBEntity>();
			this.TournamentLeadersNearMeByPlanet.Add(planetUid, value);
		}

		public void UpdateTopSquads(LeaderboardController.OnUpdateData callback)
		{
			GetLeaderboardSquadsCommand getLeaderboardSquadsCommand = new GetLeaderboardSquadsCommand(new PlayerIdRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			getLeaderboardSquadsCommand.AddSuccessCallback(new AbstractCommand<PlayerIdRequest, LeaderboardResponse>.OnSuccessCallback(this.OnGetTopSquadsSuccess));
			getLeaderboardSquadsCommand.AddFailureCallback(new AbstractCommand<PlayerIdRequest, LeaderboardResponse>.OnFailureCallback(this.OnUpdateFailure));
			getLeaderboardSquadsCommand.Context = callback;
			Service.ServerAPI.Sync(getLeaderboardSquadsCommand);
			this.TopSquads.LastRefreshTime = Service.ServerAPI.ServerTime;
		}

		private void OnGetTopSquadsSuccess(LeaderboardResponse response, object cookie)
		{
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			this.ParseSquadResponse(response.TopData, this.TopSquads, false, currentSquad);
			this.ParseSquadResponse(response.SurroundingData, this.SquadsNearMe, false, currentSquad);
			this.FireCallbackFromCookie(cookie, true);
		}

		public void UpdateFeaturedSquads(LeaderboardController.OnUpdateData callback)
		{
			GetFeaturedSquadsCommand getFeaturedSquadsCommand = new GetFeaturedSquadsCommand(new PlayerIdRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			getFeaturedSquadsCommand.AddSuccessCallback(new AbstractCommand<PlayerIdRequest, FeaturedSquadsResponse>.OnSuccessCallback(this.OnGetFeaturedSquadsSuccess));
			getFeaturedSquadsCommand.AddFailureCallback(new AbstractCommand<PlayerIdRequest, FeaturedSquadsResponse>.OnFailureCallback(this.OnUpdateFailure));
			getFeaturedSquadsCommand.Context = callback;
			Service.ServerAPI.Sync(getFeaturedSquadsCommand);
			this.FeaturedSquads.LastRefreshTime = Service.ServerAPI.ServerTime;
		}

		private void OnGetFeaturedSquadsSuccess(FeaturedSquadsResponse response, object cookie)
		{
			this.ParseSquadResponse(response.SquadData, this.FeaturedSquads, true, null);
			this.FeaturedSquads.List.Sort();
			this.FireCallbackFromCookie(cookie, true);
		}

		public void SearchSquadsByName(string searchStr, LeaderboardController.OnUpdateData callback)
		{
			SearchSquadsByNameCommand searchSquadsByNameCommand = new SearchSquadsByNameCommand(new SearchSquadsByNameRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId,
				SearchTerm = searchStr
			});
			searchSquadsByNameCommand.AddSuccessCallback(new AbstractCommand<SearchSquadsByNameRequest, FeaturedSquadsResponse>.OnSuccessCallback(this.OnGetSearchedSquadsSuccess));
			searchSquadsByNameCommand.AddFailureCallback(new AbstractCommand<SearchSquadsByNameRequest, FeaturedSquadsResponse>.OnFailureCallback(this.OnUpdateFailure));
			searchSquadsByNameCommand.Context = callback;
			Service.ServerAPI.Sync(searchSquadsByNameCommand);
			this.SearchedSquads.LastRefreshTime = Service.ServerAPI.ServerTime;
		}

		private void OnGetSearchedSquadsSuccess(FeaturedSquadsResponse response, object cookie)
		{
			this.ParseSquadResponse(response.SquadData, this.SearchedSquads, true, null);
			this.SearchedSquads.List.Sort();
			this.FireCallbackFromCookie(cookie, true);
		}

		public void UpdateSquadDetails(string squadId, LeaderboardController.OnUpdateSquadData callback)
		{
			GetPublicSquadCommand getPublicSquadCommand = new GetPublicSquadCommand(new SquadIDRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId,
				SquadId = squadId
			});
			getPublicSquadCommand.AddSuccessCallback(new AbstractCommand<SquadIDRequest, SquadResponse>.OnSuccessCallback(this.OnUpdateSquadSuccess));
			getPublicSquadCommand.AddFailureCallback(new AbstractCommand<SquadIDRequest, SquadResponse>.OnFailureCallback(this.OnUpdateSquadFailure));
			getPublicSquadCommand.Context = callback;
			Service.ServerAPI.Sync(getPublicSquadCommand);
		}

		private void OnUpdateSquadSuccess(SquadResponse response, object cookie)
		{
			Squad orCreateSquad = this.GetOrCreateSquad(response.SquadId);
			orCreateSquad.FromObject(response.SquadData);
			LeaderboardController.OnUpdateSquadData onUpdateSquadData = (LeaderboardController.OnUpdateSquadData)cookie;
			if (onUpdateSquadData != null)
			{
				onUpdateSquadData(orCreateSquad, true);
			}
		}

		private void OnUpdateSquadFailure(uint status, object cookie)
		{
			LeaderboardController.OnUpdateSquadData onUpdateSquadData = (LeaderboardController.OnUpdateSquadData)cookie;
			if (onUpdateSquadData != null)
			{
				onUpdateSquadData(null, false);
			}
		}

		private void ParseSquadResponse(List<object> squads, LeaderboardList<Squad> leaderboardList, bool featured, Squad currentPlayerSquad)
		{
			if (squads != null && squads.Count > 0)
			{
				leaderboardList.List.Clear();
				int i = 0;
				int count = squads.Count;
				while (i < count)
				{
					Dictionary<string, object> dictionary = squads[i] as Dictionary<string, object>;
					if (dictionary != null)
					{
						string text = null;
						if (dictionary.ContainsKey("_id"))
						{
							text = Convert.ToString(dictionary["_id"]);
						}
						if (text != null)
						{
							Squad orCreateSquad = this.GetOrCreateSquad(text);
							if (featured)
							{
								orCreateSquad.FromFeaturedObject(dictionary);
							}
							else
							{
								orCreateSquad.FromLeaderboardObject(dictionary);
							}
							if (orCreateSquad.MemberCount > 0)
							{
								leaderboardList.List.Add(orCreateSquad);
							}
							if (currentPlayerSquad != null && text == currentPlayerSquad.SquadID)
							{
								leaderboardList.AlwaysRefresh = true;
							}
						}
					}
					i++;
				}
			}
		}

		public Squad GetOrCreateSquad(string squadID)
		{
			Squad squad = this.GetCachedSquad(squadID);
			if (squad == null)
			{
				squad = new Squad(squadID);
				this.cachedSquads.Add(squad);
			}
			return squad;
		}

		private Squad GetCachedSquad(string squadID)
		{
			int i = 0;
			int count = this.cachedSquads.Count;
			while (i < count)
			{
				Squad squad = this.cachedSquads[i];
				if (squad.SquadID == squadID)
				{
					return squad;
				}
				i++;
			}
			return null;
		}

		public void UpdateFriends(string friendIds, LeaderboardController.OnUpdateData callback)
		{
			GetLeaderboardFriendsCommand getLeaderboardFriendsCommand = new GetLeaderboardFriendsCommand(new FriendLBIDRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId,
				FriendIDs = friendIds
			});
			getLeaderboardFriendsCommand.AddSuccessCallback(new AbstractCommand<FriendLBIDRequest, LeaderboardResponse>.OnSuccessCallback(this.OnFriendsUpdated));
			getLeaderboardFriendsCommand.AddFailureCallback(new AbstractCommand<FriendLBIDRequest, LeaderboardResponse>.OnFailureCallback(this.OnUpdateFailure));
			getLeaderboardFriendsCommand.Context = callback;
			Service.ServerAPI.Sync(getLeaderboardFriendsCommand);
			this.Friends.LastRefreshTime = Service.ServerAPI.ServerTime;
		}

		private void OnFriendsUpdated(LeaderboardResponse response, object cookie)
		{
			this.ParsePlayerResponse(response.TopData, this.Friends);
			this.Friends.List.Sort();
			this.FireCallbackFromCookie(cookie, true);
		}

		public void UpdateLeaders(PlanetVO planetVO, LeaderboardController.OnUpdateData callback)
		{
			LeaderboardList<PlayerLBEntity> leaderboardList = null;
			LeaderboardList<PlayerLBEntity> value = null;
			string planetId = (planetVO == null) ? null : planetVO.Uid;
			this.GetPlayerLists(PlayerListType.Leaders, planetId, out leaderboardList, out value);
			PlayerLeaderboardRequest request = new PlayerLeaderboardRequest(planetId, Service.CurrentPlayer.PlayerId);
			GetLeaderboardPlayersCommand getLeaderboardPlayersCommand = new GetLeaderboardPlayersCommand(request);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["callback"] = callback;
			dictionary["list"] = leaderboardList;
			dictionary["listnearby"] = value;
			getLeaderboardPlayersCommand.Context = dictionary;
			getLeaderboardPlayersCommand.AddSuccessCallback(new AbstractCommand<PlayerLeaderboardRequest, LeaderboardResponse>.OnSuccessCallback(this.OnLeadersUpdated));
			getLeaderboardPlayersCommand.AddFailureCallback(new AbstractCommand<PlayerLeaderboardRequest, LeaderboardResponse>.OnFailureCallback(this.OnLeadersUpdateFailure));
			Service.ServerAPI.Sync(getLeaderboardPlayersCommand);
			leaderboardList.LastRefreshTime = Service.ServerAPI.ServerTime;
		}

		private void OnLeadersUpdated(LeaderboardResponse response, object cookie)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)cookie;
			LeaderboardList<PlayerLBEntity> leaderboardList = (LeaderboardList<PlayerLBEntity>)dictionary["list"];
			LeaderboardList<PlayerLBEntity> leaderboardList2 = (LeaderboardList<PlayerLBEntity>)dictionary["listnearby"];
			object cookie2 = dictionary["callback"];
			this.ParsePlayerResponse(response.TopData, leaderboardList);
			this.ParsePlayerResponse(response.SurroundingData, leaderboardList2);
			this.FireCallbackFromCookie(cookie2, true);
		}

		private void OnLeadersUpdateFailure(uint status, object cookie)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)cookie;
			object cookie2 = dictionary["callback"];
			this.FireCallbackFromCookie(cookie2, false);
		}

		public void UpdateTournamentLeaders(PlanetVO planetVO, LeaderboardController.OnUpdateData callback)
		{
			if (planetVO == null)
			{
				Service.Logger.Error("Tournament leaderboard requested without setting planetVO");
				return;
			}
			PlayerLeaderboardRequest request = new PlayerLeaderboardRequest(planetVO.Uid, Service.CurrentPlayer.PlayerId);
			LeaderboardList<PlayerLBEntity> leaderboardList = null;
			LeaderboardList<PlayerLBEntity> value = null;
			this.GetPlayerLists(PlayerListType.TournamentLeaders, planetVO.Uid, out leaderboardList, out value);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["callback"] = callback;
			dictionary["list"] = leaderboardList;
			dictionary["listnearby"] = value;
			GetTournamentLeaderboardPlayersCommand getTournamentLeaderboardPlayersCommand = new GetTournamentLeaderboardPlayersCommand(request);
			getTournamentLeaderboardPlayersCommand.Context = dictionary;
			getTournamentLeaderboardPlayersCommand.AddSuccessCallback(new AbstractCommand<PlayerLeaderboardRequest, LeaderboardResponse>.OnSuccessCallback(this.OnLeadersUpdated));
			getTournamentLeaderboardPlayersCommand.AddFailureCallback(new AbstractCommand<PlayerLeaderboardRequest, LeaderboardResponse>.OnFailureCallback(this.OnLeadersUpdateFailure));
			Service.ServerAPI.Sync(getTournamentLeaderboardPlayersCommand);
			leaderboardList.LastRefreshTime = Service.ServerAPI.ServerTime;
		}

		private void ParsePlayerResponse(List<object> players, LeaderboardList<PlayerLBEntity> leaderboardList)
		{
			if (players != null && players.Count > 0)
			{
				leaderboardList.List.Clear();
				int i = 0;
				int count = players.Count;
				while (i < count)
				{
					Dictionary<string, object> dictionary = players[i] as Dictionary<string, object>;
					if (dictionary != null)
					{
						string text = null;
						if (dictionary.ContainsKey("_id"))
						{
							text = Convert.ToString(dictionary["_id"]);
						}
						if (text != null)
						{
							PlayerLBEntity orCreatePlayer = this.GetOrCreatePlayer(text);
							if (orCreatePlayer.FromObject(dictionary) != null)
							{
								leaderboardList.List.Add(orCreatePlayer);
								if (orCreatePlayer.PlayerID == Service.CurrentPlayer.PlayerId)
								{
									leaderboardList.AlwaysRefresh = true;
								}
							}
							else
							{
								Service.Logger.Warn("Player Leaderboard Entry Failed to parse.");
							}
						}
					}
					i++;
				}
			}
		}

		private PlayerLBEntity GetOrCreatePlayer(string playerId)
		{
			PlayerLBEntity playerLBEntity = this.GetCachedPlayer(playerId);
			if (playerLBEntity == null)
			{
				playerLBEntity = new PlayerLBEntity(playerId);
				this.cachedPlayers.Add(playerLBEntity);
			}
			return playerLBEntity;
		}

		private PlayerLBEntity GetCachedPlayer(string playerId)
		{
			int i = 0;
			int count = this.cachedPlayers.Count;
			while (i < count)
			{
				PlayerLBEntity playerLBEntity = this.cachedPlayers[i];
				if (playerLBEntity.PlayerID == playerId)
				{
					return playerLBEntity;
				}
				i++;
			}
			return null;
		}

		private void OnUpdateFailure(uint status, object cookie)
		{
			this.FireCallbackFromCookie(cookie, false);
		}

		private void FireCallbackFromCookie(object cookie, bool success)
		{
			LeaderboardController.OnUpdateData onUpdateData = (LeaderboardController.OnUpdateData)cookie;
			if (onUpdateData != null)
			{
				onUpdateData(success);
			}
		}

		private void GetPlayerLists(PlayerListType listType, string planetId, out LeaderboardList<PlayerLBEntity> leaderboardList, out LeaderboardList<PlayerLBEntity> nearbyLeaderboardList)
		{
			leaderboardList = null;
			nearbyLeaderboardList = null;
			if (listType != PlayerListType.Friends)
			{
				if (listType != PlayerListType.Leaders)
				{
					if (listType == PlayerListType.TournamentLeaders)
					{
						if (!string.IsNullOrEmpty(planetId))
						{
							if (!this.TournamentLeadersByPlanet.ContainsKey(planetId))
							{
								this.InitTournamentListForPlanet(planetId);
							}
							leaderboardList = this.TournamentLeadersByPlanet[planetId];
							nearbyLeaderboardList = this.TournamentLeadersNearMeByPlanet[planetId];
						}
						else
						{
							Service.Logger.Error("planetId value is null or empty in tournament leaderboard response handling");
						}
					}
				}
				else if (planetId == null)
				{
					leaderboardList = this.GlobalLeaders;
					nearbyLeaderboardList = this.GlobalNearMeLeaders;
				}
				else
				{
					leaderboardList = this.LeadersByPlanet[planetId];
					nearbyLeaderboardList = this.LeadersNearMeByPlanet[planetId];
				}
			}
			else
			{
				leaderboardList = this.Friends;
				nearbyLeaderboardList = this.Friends;
			}
		}

		public bool ShouldRefreshData(PlayerListType listType, string planetId)
		{
			bool flag = false;
			bool flag2 = false;
			uint serverTime = Service.ServerAPI.ServerTime;
			uint timeB = 0u;
			switch (listType)
			{
			case PlayerListType.FeaturedSquads:
				if (this.FeaturedSquads.List.Count == 0)
				{
					flag = true;
				}
				else
				{
					flag2 = true;
				}
				break;
			case PlayerListType.SearchedSquads:
				flag = false;
				flag2 = true;
				break;
			case PlayerListType.Squads:
				flag = this.TopSquads.AlwaysRefresh;
				timeB = this.TopSquads.LastRefreshTime;
				break;
			case PlayerListType.Friends:
				flag = this.Friends.AlwaysRefresh;
				timeB = this.Friends.LastRefreshTime;
				break;
			case PlayerListType.Leaders:
				if (string.IsNullOrEmpty(planetId))
				{
					flag = this.GlobalLeaders.AlwaysRefresh;
					timeB = this.GlobalLeaders.LastRefreshTime;
				}
				else
				{
					flag = this.LeadersByPlanet[planetId].AlwaysRefresh;
					timeB = this.LeadersByPlanet[planetId].LastRefreshTime;
				}
				break;
			case PlayerListType.TournamentLeaders:
				if (this.TournamentLeadersByPlanet.ContainsKey(planetId))
				{
					flag = this.TournamentLeadersByPlanet[planetId].AlwaysRefresh;
					timeB = this.TournamentLeadersByPlanet[planetId].LastRefreshTime;
				}
				break;
			}
			if (!flag && !flag2)
			{
				int timeDifferenceSafe = GameUtils.GetTimeDifferenceSafe(serverTime, timeB);
				flag = (timeDifferenceSafe >= 30);
			}
			return flag;
		}

		public void TopPlayer(PlanetVO planetVO)
		{
			this.UpdateLeaders(planetVO, new LeaderboardController.OnUpdateData(this.OnTopPlayerSuccess));
		}

		private void OnTopPlayerSuccess(bool success)
		{
			if (!success)
			{
				Service.EventManager.SendEvent(EventId.HolonetLeaderBoardUpdated, null);
				return;
			}
			this.topPlayers.Clear();
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (this.LeadersByPlanet.ContainsKey(currentPlayer.PlanetId) && this.LeadersByPlanet[currentPlayer.PlanetId].List.Count >= 0)
			{
				LeaderboardList<PlayerLBEntity> leaderboardList = this.LeadersByPlanet[currentPlayer.PlanetId];
				if (leaderboardList.List.Count > 0)
				{
					this.topPlayers.Add(leaderboardList.List[0]);
					int i = 0;
					int count = leaderboardList.List.Count;
					while (i < count)
					{
						if (this.topPlayers.Count >= 2)
						{
							break;
						}
						if (leaderboardList.List[i].Faction != this.topPlayers[0].Faction)
						{
							this.topPlayers.Add(leaderboardList.List[i]);
						}
						i++;
					}
				}
			}
			Service.EventManager.SendEvent(EventId.HolonetLeaderBoardUpdated, null);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.SquadJoinedByCurrentPlayer)
			{
				if (id == EventId.SquadLeft)
				{
					this.TopSquads.AlwaysRefresh = false;
				}
			}
			else
			{
				string squadID = Service.SquadController.StateManager.GetCurrentSquad().SquadID;
				int i = 0;
				int count = this.TopSquads.List.Count;
				while (i < count)
				{
					if (this.TopSquads.List[i].SquadID == squadID)
					{
						this.TopSquads.AlwaysRefresh = true;
						break;
					}
					i++;
				}
			}
			return EatResponse.NotEaten;
		}
	}
}
