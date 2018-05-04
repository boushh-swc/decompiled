using Net.RichardLord.Ash.Core;
using StaRTS.Externals.Manimal;
using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.FX;
using StaRTS.Main.Configs;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player.Raids;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class RaidDefenseController : IEventObserver, IViewClockTimeObserver
	{
		public delegate void OnSuccessCallback(AbstractResponse response, object cookie);

		public const string HERO_TRAINER_DESTROYED = "HERO_TRAINER_DESTROYED";

		public const string STARSHIP_TRAINER_DESTROYED = "STARSHIP_TRAINER_DESTROYED";

		public const string SQUAD_CENTER_DESTROYED = "SQUAD_CENTER_DESTROYED";

		private const string HOLO_LOCATOR = "locator_fx";

		private const string RAID_TIME_REMAINING_ACTIVE = "RAID_TIME_REMAINING_ACTIVE";

		private const string OK = "s_Ok";

		private const string RAID_CONFIRM_TITLE = "RAID_CONFIRM_TITLE";

		private const string RAID_CONFIRM_DESC = "RAID_CONFIRM_DESC";

		private const string RAID_CANCEL = "s_Cancel";

		private const string SKIP_FUTURE_CONFIRMATION = "SKIP_FUTURE_CONFIRMATION";

		private const string RAID_WAIT_TITLE = "RAID_WAIT_TITLE";

		private const string RAID_WAIT_DESC = "RAID_WAIT_DESC";

		private const string RAID_START = "RAID_START";

		private HashSet<BuildingType> raidDefenseTrainerBindings;

		private Action onRaidEndCallback;

		private int raidStarsEarned;

		private uint nextUpdateTimestamp;

		private RaidDefenseController.OnSuccessCallback raidStartCallback;

		private string lastAwardedCrateUid;

		private string finalWaveIdOfLastDefense;

		private BuildingHoloEffect scoutHolo;

		public Color ActiveRaidColor
		{
			get;
			private set;
		}

		public Color InactiveColor
		{
			get;
			private set;
		}

		public Color InactiveTickerColor
		{
			get;
			private set;
		}

		public RaidDefenseController()
		{
			this.InactiveColor = new Color(0.95f, 0.84f, 0.18f);
			this.InactiveTickerColor = Color.white;
			this.ActiveRaidColor = new Color(0.9f, 0f, 0f);
			this.nextUpdateTimestamp = 0u;
			this.raidStarsEarned = 0;
			Service.RaidDefenseController = this;
			string[] array = GameConstants.RAID_DEFENSE_TRAINER_BINDINGS.Split(new char[]
			{
				','
			});
			this.raidDefenseTrainerBindings = new HashSet<BuildingType>();
			int i = 0;
			int num = array.Length;
			while (i < num)
			{
				this.raidDefenseTrainerBindings.Add(StringUtils.ParseEnum<BuildingType>(array[i]));
				i++;
			}
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.ContractStarted, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ContractContinued, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingCancelled, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingConstructed, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingReplaced, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingViewReady, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.PlanetRelocateStarted, EventPriority.Default);
			Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
			this.SetupRaidUpdateTimer();
			Service.UXController.MiscElementsManager.AddRaidsTickerStatus(this);
		}

		public bool AreRaidsAccessible()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			int num = currentPlayer.Map.FindHighestHqLevel();
			bool flag = true;
			flag &= (GameConstants.RAIDS_HQ_UNLOCK_LEVEL <= num && currentPlayer.CurrentRaid != null);
			return flag & currentPlayer.Map.ScoutTowerExists();
		}

		public void OnStartRaidDefenseMission()
		{
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			this.RegisterBattleObservers();
		}

		public void OnEndRaidDefenseMission(string finalWaveId)
		{
			this.finalWaveIdOfLastDefense = finalWaveId;
			this.UnregisterBattleObservers();
		}

		private void StartCurrentRaidDefenseInternal()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (currentPlayer.CurrentRaid == null || !Service.RaidDefenseController.IsRaidAvailable())
			{
				return;
			}
			Service.CampaignController.StartMission(currentPlayer.CurrentRaid);
		}

		public int GetRaidTimeSeconds()
		{
			uint result;
			if (this.IsRaidAvailable())
			{
				result = this.GetSecondsLeftBeforeRaidEnds();
			}
			else
			{
				result = this.GetSecondsTillNextRaid();
			}
			return (int)result;
		}

		public void AttemptToShowRaidWaitConfirmation()
		{
			Lang lang = Service.Lang;
			if (!PlayerSettings.GetSkipRaidWaitConfirmation() && this.IsRaidAvailable())
			{
				AlertWithCheckBoxScreen alertWithCheckBoxScreen = new AlertWithCheckBoxScreen(lang.Get("RAID_WAIT_TITLE", new object[0]), lang.Get("RAID_WAIT_DESC", new object[0]), "SKIP_FUTURE_CONFIRMATION", "RAID_TIME_REMAINING_ACTIVE", this.GetRaidTimeSeconds(), this.ActiveRaidColor, new AlertWithCheckBoxScreen.OnCheckBoxScreenModalResult(this.OnWaitScreenClosed));
				alertWithCheckBoxScreen.SetPrimaryLabelText(lang.Get("s_Ok", new object[0]));
				alertWithCheckBoxScreen.Set2ButtonGroupEnabledState(false);
				Service.ScreenController.AddScreen(alertWithCheckBoxScreen);
			}
		}

		private void OnWaitScreenClosed(object result, bool skipFuture)
		{
			PlayerSettings.SetSkipRaidWaitConfirmation(skipFuture);
		}

		public void StartCurrentRaidDefense()
		{
			Lang lang = Service.Lang;
			if (!PlayerSettings.GetSkipRaidDefendConfirmation())
			{
				AlertWithCheckBoxScreen alertWithCheckBoxScreen = new AlertWithCheckBoxScreen(lang.Get("RAID_CONFIRM_TITLE", new object[0]), lang.Get("RAID_CONFIRM_DESC", new object[0]), "SKIP_FUTURE_CONFIRMATION", "RAID_TIME_REMAINING_ACTIVE", this.GetRaidTimeSeconds(), this.ActiveRaidColor, new AlertWithCheckBoxScreen.OnCheckBoxScreenModalResult(this.OnDefendNowScreenClosed));
				alertWithCheckBoxScreen.SetPrimaryLabelText(lang.Get("RAID_START", new object[0]));
				alertWithCheckBoxScreen.SetSecondaryLabelText(lang.Get("s_Cancel", new object[0]));
				alertWithCheckBoxScreen.Set2ButtonGroupEnabledState(true);
				Service.ScreenController.AddScreen(alertWithCheckBoxScreen);
			}
			else
			{
				this.StartCurrentRaidDefenseInternal();
			}
		}

		private void OnDefendNowScreenClosed(object result, bool skipFuture)
		{
			if (result != null)
			{
				PlayerSettings.SetSkipRaidDefendConfirmation(skipFuture);
				this.StartCurrentRaidDefenseInternal();
			}
		}

		public void SetupRaidUpdateTimer()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (this.IsRaidAvailable())
			{
				this.nextUpdateTimestamp = currentPlayer.RaidEndTime;
			}
			else
			{
				this.nextUpdateTimestamp = currentPlayer.RaidStartTime;
			}
		}

		public string GetDisplayForTimeTillNextRaid()
		{
			return GameUtils.GetTimeLabelFromSeconds((int)this.GetSecondsTillNextRaid());
		}

		public string GetDisplayForTimeLeftInRaid()
		{
			return GameUtils.GetTimeLabelFromSeconds((int)this.GetSecondsLeftBeforeRaidEnds());
		}

		public uint GetSecondsTillNextRaid()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			uint result = 0u;
			uint num = currentPlayer.RaidStartTime;
			uint raidEndTime = currentPlayer.RaidEndTime;
			uint time = ServerTime.Time;
			if (this.IsRaidAvailable())
			{
				num = currentPlayer.NextRaidStartTime;
			}
			if (num > raidEndTime && num > time)
			{
				result = num - time;
			}
			return result;
		}

		public uint GetSecondsLeftBeforeRaidEnds()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			uint result = 0u;
			uint raidStartTime = currentPlayer.RaidStartTime;
			uint raidEndTime = currentPlayer.RaidEndTime;
			uint time = ServerTime.Time;
			if (raidEndTime > raidStartTime && raidEndTime > time)
			{
				result = raidEndTime - time;
			}
			return result;
		}

		private void UpdateRaidExpiration()
		{
			if (this.nextUpdateTimestamp > 0u && ServerTime.Time >= this.nextUpdateTimestamp)
			{
				this.nextUpdateTimestamp = 0u;
				this.SendRaidDefenseUpdate();
			}
		}

		public void OnViewClockTime(float dt)
		{
			this.UpdateRaidExpiration();
		}

		public void SendRaidDefenseUpdate()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			RaidUpdateRequest request = new RaidUpdateRequest(currentPlayer.PlanetId);
			RaidUpdateCommand raidUpdateCommand = new RaidUpdateCommand(request);
			raidUpdateCommand.AddSuccessCallback(new AbstractCommand<RaidUpdateRequest, RaidUpdateResponse>.OnSuccessCallback(this.OnRaidUpdateSuccess));
			Service.ServerAPI.Sync(raidUpdateCommand);
		}

		private void OnRaidUpdateSuccess(RaidUpdateResponse response, object cookie)
		{
			this.SetupRaidUpdateTimer();
			if (this.IsRaidAvailable())
			{
				this.CreateScoutHolo();
			}
			else
			{
				this.DestroyScoutHolo();
			}
		}

		public void SendRaidDefenseComplete(CampaignMissionVO raidMission, Action endRaidCallback, EndPvEBattleTO endBattleTO)
		{
			CampaignMissionVO currentRaid = Service.CurrentPlayer.CurrentRaid;
			if (GameUtils.SafeVOEqualityValidation(raidMission, currentRaid))
			{
				RaidDefenseCompleteRequest request = new RaidDefenseCompleteRequest(endBattleTO, this.finalWaveIdOfLastDefense);
				RaidDefenseCompleteCommand raidDefenseCompleteCommand = new RaidDefenseCompleteCommand(request);
				raidDefenseCompleteCommand.AddSuccessCallback(new AbstractCommand<RaidDefenseCompleteRequest, RaidDefenseCompleteResponse>.OnSuccessCallback(this.OnRaidCompleteSuccessWrapper));
				this.onRaidEndCallback = endRaidCallback;
				this.raidStarsEarned = endBattleTO.Battle.EarnedStars;
				Service.ServerAPI.Sync(raidDefenseCompleteCommand);
				this.finalWaveIdOfLastDefense = string.Empty;
			}
			else
			{
				Service.Logger.Error("Ended Raid Defense does not match the current raid.Ended: " + this.GetUidToLog(raidMission) + ", Scheduled: " + this.GetUidToLog(currentRaid));
			}
		}

		public void SendRaidDefenseStart(CampaignMissionVO raidMission, RaidDefenseController.OnSuccessCallback successCB)
		{
			CampaignMissionVO currentRaid = Service.CurrentPlayer.CurrentRaid;
			if (GameUtils.SafeVOEqualityValidation(raidMission, currentRaid))
			{
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				RaidDefenseStartRequest request = new RaidDefenseStartRequest(currentPlayer.PlanetId, raidMission.Uid);
				RaidDefenseStartCommand raidDefenseStartCommand = new RaidDefenseStartCommand(request);
				this.raidStartCallback = successCB;
				raidDefenseStartCommand.AddSuccessCallback(new AbstractCommand<RaidDefenseStartRequest, RaidDefenseStartResponse>.OnSuccessCallback(this.OnRaidStartSuccessWrapper));
				Service.ServerAPI.Sync(raidDefenseStartCommand);
			}
			else
			{
				Service.Logger.Error("Started Raid Defense does not match the next scheduled raid.Started: " + this.GetUidToLog(raidMission) + ", Scheduled: " + this.GetUidToLog(currentRaid));
			}
		}

		private void OnRaidStartSuccessWrapper(RaidDefenseStartResponse response, object cookie)
		{
			if (this.raidStartCallback != null)
			{
				this.raidStartCallback(response, cookie);
			}
		}

		private bool RaidCompleteDidAwardCrate()
		{
			return this.lastAwardedCrateUid != null;
		}

		private void OnRaidCompleteSuccessWrapper(RaidDefenseCompleteResponse response, object cookie)
		{
			this.SetupRaidUpdateTimer();
			Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
			this.UpdateRaidExpiration();
			this.lastAwardedCrateUid = response.AwardedCrateUid;
			if (this.RaidCompleteDidAwardCrate())
			{
				Service.EventManager.RegisterObserver(this, EventId.WorldLoadComplete);
			}
			this.WinRaid(this.raidStarsEarned);
			this.raidStarsEarned = 0;
			if (this.onRaidEndCallback != null)
			{
				this.onRaidEndCallback();
			}
		}

		public bool IsRaidAvailable()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			uint raidEndTime = currentPlayer.RaidEndTime;
			uint raidStartTime = currentPlayer.RaidStartTime;
			if (raidEndTime > 0u)
			{
				uint time = ServerTime.Time;
				return raidEndTime > time && raidStartTime <= time;
			}
			return false;
		}

		public void ShowRaidInfo()
		{
			if (this.AreRaidsAccessible())
			{
				Service.ScreenController.AddScreen(new RaidInfoScreen());
			}
			else
			{
				Service.Logger.Warn("Tried to Show Raid Briefing While Raids Are Not Accessible");
			}
		}

		private string GetUidToLog(IValueObject vo)
		{
			if (vo != null)
			{
				return vo.Uid;
			}
			return "NULL";
		}

		public bool SquadTroopDeployAllowed()
		{
			if (this.AreRaidsAccessible() && this.IsRaidAvailable())
			{
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				RaidVO raidVO = Service.StaticDataController.Get<RaidVO>(currentPlayer.CurrentRaidId);
				return raidVO.SquadEnabled;
			}
			return false;
		}

		private void CreateScoutHolo()
		{
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			Entity availableScoutTower = buildingLookupController.GetAvailableScoutTower();
			if (availableScoutTower != null && this.AreRaidsAccessible() && this.IsRaidAvailable())
			{
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				RaidVO raidVO = Service.StaticDataController.Get<RaidVO>(currentPlayer.CurrentRaidId);
				this.scoutHolo = new BuildingHoloEffect(availableScoutTower);
				this.scoutHolo.CreateGenericHolo(raidVO.BuildingHoloAssetName, "locator_fx");
			}
		}

		private void DestroyScoutHolo()
		{
			if (this.scoutHolo != null)
			{
				this.scoutHolo.Cleanup();
				this.scoutHolo = null;
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.WorldLoadComplete:
			{
				IState currentState = Service.GameStateMachine.CurrentState;
				if (currentState is HomeState && this.RaidCompleteDidAwardCrate())
				{
					GameUtils.ShowCrateAwardModal(this.lastAwardedCrateUid);
					this.lastAwardedCrateUid = null;
					Service.EventManager.UnregisterObserver(this, EventId.WorldLoadComplete);
				}
				return EatResponse.NotEaten;
			}
			case EventId.WorldInTransitionComplete:
			case EventId.WorldOutTransitionComplete:
			{
				IL_1D:
				switch (id)
				{
				case EventId.BuildingViewReady:
				{
					EntityViewParams entityViewParams = (EntityViewParams)cookie;
					if (entityViewParams.Entity.Has<ScoutTowerComponent>())
					{
						this.CreateScoutHolo();
					}
					return EatResponse.NotEaten;
				}
				case EventId.BuildingViewFailed:
					IL_32:
					if (id == EventId.BuildingConstructed)
					{
						goto IL_B1;
					}
					if (id == EventId.BuildingReplaced)
					{
						Entity entity = cookie as Entity;
						if (entity.Has<ScoutTowerComponent>())
						{
							this.CreateScoutHolo();
						}
						return EatResponse.NotEaten;
					}
					if (id == EventId.ContractStarted || id == EventId.ContractContinued)
					{
						ContractEventData contractEventData = (ContractEventData)cookie;
						if (contractEventData.BuildingVO.Type == BuildingType.ScoutTower)
						{
							this.DestroyScoutHolo();
						}
						return EatResponse.NotEaten;
					}
					if (id != EventId.EntityKilled)
					{
						if (id == EventId.HeroDeployed)
						{
							EntityController entityController = Service.EntityController;
							NodeList<OffensiveTroopNode> nodeList = entityController.GetNodeList<OffensiveTroopNode>();
							TroopAttackController troopAttackController = Service.TroopAttackController;
							for (OffensiveTroopNode offensiveTroopNode = nodeList.Head; offensiveTroopNode != null; offensiveTroopNode = offensiveTroopNode.Next)
							{
								troopAttackController.RefreshTarget((SmartEntity)offensiveTroopNode.Entity);
							}
							return EatResponse.NotEaten;
						}
						if (id != EventId.PlanetRelocateStarted)
						{
							return EatResponse.NotEaten;
						}
						if (this.AreRaidsAccessible())
						{
							this.SendRaidDefenseUpdate();
						}
						return EatResponse.NotEaten;
					}
					else
					{
						SmartEntity smartEntity = (SmartEntity)cookie;
						if (smartEntity.BuildingComp == null)
						{
							return EatResponse.NotEaten;
						}
						BuildingType type = smartEntity.BuildingComp.BuildingType.Type;
						if (!this.raidDefenseTrainerBindings.Contains(type))
						{
							return EatResponse.NotEaten;
						}
						UXController uXController = Service.UXController;
						Lang lang = Service.Lang;
						switch (type)
						{
						case BuildingType.FleetCommand:
							Service.DeployerController.SpecialAttackDeployer.ExitMode();
							uXController.HUD.DisableSpecialAttacks();
							uXController.MiscElementsManager.ShowPlayerInstructions(lang.Get("STARSHIP_TRAINER_DESTROYED", new object[0]));
							break;
						case BuildingType.HeroMobilizer:
							Service.DeployerController.HeroDeployer.ExitMode();
							uXController.HUD.DisableHeroDeploys();
							uXController.MiscElementsManager.ShowPlayerInstructions(lang.Get("HERO_TRAINER_DESTROYED", new object[0]));
							break;
						case BuildingType.Squad:
							Service.DeployerController.SquadTroopDeployer.ExitMode();
							uXController.HUD.DisableSquadDeploy();
							uXController.MiscElementsManager.ShowPlayerInstructions(lang.Get("SQUAD_CENTER_DESTROYED", new object[0]));
							break;
						}
						return EatResponse.NotEaten;
					}
					break;
				case EventId.BuildingCancelled:
					goto IL_B1;
				}
				goto IL_32;
				IL_B1:
				ContractEventData contractEventData2 = (ContractEventData)cookie;
				if (contractEventData2.BuildingVO.Type == BuildingType.ScoutTower)
				{
					this.SendRaidDefenseUpdate();
				}
				return EatResponse.NotEaten;
			}
			case EventId.WorldReset:
				this.DestroyScoutHolo();
				return EatResponse.NotEaten;
			}
			goto IL_1D;
		}

		private void RegisterBattleObservers()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.EntityKilled, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.HeroDeployed, EventPriority.AfterDefault);
		}

		private void UnregisterBattleObservers()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.EntityKilled);
			eventManager.UnregisterObserver(this, EventId.HeroDeployed);
		}

		public void WinRaid(int stars)
		{
			Service.EventManager.SendEvent(EventId.RaidComplete, stars);
		}
	}
}
