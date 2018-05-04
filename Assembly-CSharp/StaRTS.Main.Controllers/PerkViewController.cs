using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Main.Views.UX.Screens.Squads;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class PerkViewController : IEventObserver
	{
		private const string LANG_PERK_TITLE_PREFIX = "perk_title_";

		private const string LANG_PERK_DESC_PREFIX = "perk_desc_";

		private const string LANG_PERK_UPGRADE_LVL_REQ = "PERK_UPGRADE_LVL_REQ";

		private const string GENERATOR_TYPE = "generator";

		private const string CONTRACT_COST_TYPE = "contractCost";

		private const string CONTRACT_TIME_TYPE = "contractTime";

		private const string RELOCATION_TYPE = "relocation";

		private const string TRP_REQUEST_AMT_TYPE = "troopRequestAmount";

		private const string TRP_REQUEST_TIME_TYPE = "troopRequestTime";

		private const string PERK_CANCEL_POPUP_TITLE = "PERK_CANCEL_POPUP_TITLE";

		private const string PERK_CANCEL_POPUP_DESC = "PERK_CANCEL_POPUP_DESC";

		private uint perkBuildingHighlightTimerID;

		public PerkViewController()
		{
			Service.PerkViewController = this;
			this.perkBuildingHighlightTimerID = 0u;
			this.RegisterEvents();
		}

		private void RegisterEvents()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.BuildingConstructed, EventPriority.AfterDefault);
			eventManager.RegisterObserver(this, EventId.BuildingLevelUpgraded, EventPriority.AfterDefault);
			eventManager.RegisterObserver(this, EventId.WorldLoadComplete);
			eventManager.RegisterObserver(this, EventId.ActivePerksUpdated);
			eventManager.RegisterObserver(this, EventId.GameStateChanged);
		}

		private void RefreshPerkBuildingHighlightTimer()
		{
			if (this.perkBuildingHighlightTimerID != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.perkBuildingHighlightTimerID);
			}
			PerkManager perkManager = Service.PerkManager;
			List<Entity> buildingsForActivePerks = perkManager.GetBuildingsForActivePerks();
			uint num = 4294967295u;
			Entity entity = null;
			int i = 0;
			int count = buildingsForActivePerks.Count;
			while (i < count)
			{
				Entity entity2 = buildingsForActivePerks[i];
				uint maxActivationTimeRemaining = perkManager.GetMaxActivationTimeRemaining(entity2);
				if (maxActivationTimeRemaining > 0u && maxActivationTimeRemaining < num)
				{
					num = maxActivationTimeRemaining;
					entity = entity2;
				}
				i++;
			}
			if (entity != null)
			{
				this.perkBuildingHighlightTimerID = Service.ViewTimerManager.CreateViewTimer(num, false, new TimerDelegate(this.BuildingActivePerkEndTimer), entity);
			}
		}

		private void BuildingActivePerkEndTimer(uint timerId, object cookie)
		{
			if (cookie != null)
			{
				Entity building = cookie as Entity;
				Service.BuildingController.UpdateBuildingHighlightForPerks(building);
				this.RefreshPerkBuildingHighlightTimer();
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.BuildingLevelUpgraded:
			case EventId.BuildingConstructed:
			{
				ContractEventData contractEventData = cookie as ContractEventData;
				Entity entity = (cookie as ContractEventData).Entity;
				BuildingComponent buildingComp = ((SmartEntity)entity).BuildingComp;
				if (entity != null && buildingComp != null && contractEventData.BuildingVO != null && Service.PerkManager.IsPerkAppliedToBuilding(contractEventData.BuildingVO))
				{
					Service.BuildingController.UpdateBuildingHighlightForPerks(entity);
				}
				return EatResponse.NotEaten;
			}
			case EventId.BuildingSwapped:
				IL_19:
				if (id == EventId.WorldLoadComplete)
				{
					this.HighlightActivePerkBuildings();
					this.RefreshPerkBuildingHighlightTimer();
					return EatResponse.NotEaten;
				}
				if (id == EventId.GameStateChanged)
				{
					IState currentState = Service.GameStateMachine.CurrentState;
					if (currentState is HomeState)
					{
						this.HighlightActivePerkBuildings();
						this.RefreshPerkBuildingHighlightTimer();
					}
					return EatResponse.NotEaten;
				}
				if (id != EventId.ActivePerksUpdated)
				{
					return EatResponse.NotEaten;
				}
				this.RefreshPerkBuildingHighlightTimer();
				return EatResponse.NotEaten;
			}
			goto IL_19;
		}

		private void HighlightActivePerkBuildings()
		{
			List<Entity> buildingsForActivePerks = Service.PerkManager.GetBuildingsForActivePerks();
			int i = 0;
			int count = buildingsForActivePerks.Count;
			while (i < count)
			{
				Service.BuildingController.UpdateBuildingHighlightForPerks(buildingsForActivePerks[i]);
				i++;
			}
		}

		public string GetPerkNameForGroup(string perkGroup)
		{
			Lang lang = Service.Lang;
			return lang.Get("perk_title_" + perkGroup, new object[0]);
		}

		public string GetPerkDescForGroup(string perkGroup)
		{
			Lang lang = Service.Lang;
			return lang.Get("perk_desc_" + perkGroup, new object[0]);
		}

		public void SetPerkImage(UXTexture perkImage, PerkVO perkVO)
		{
			FactionType faction = Service.CurrentPlayer.Faction;
			string text = string.Empty;
			if (faction == FactionType.Empire)
			{
				text = perkVO.TextureIdEmpire;
			}
			else
			{
				text = perkVO.TextureIdRebel;
			}
			if (!string.IsNullOrEmpty(text))
			{
				StaticDataController staticDataController = Service.StaticDataController;
				TextureVO optional = staticDataController.GetOptional<TextureVO>(text);
				if (optional != null && perkImage.Tag != optional)
				{
					perkImage.LoadTexture(optional.AssetName);
					perkImage.Tag = optional;
				}
			}
		}

		public void SetupStatGridForPerk(PerkVO targetPerkVO, UXGrid statGrid, string templateElement, string descriptionElement, string valueElement, bool useUpgrade)
		{
			Lang lang = Service.Lang;
			StaticDataController staticDataController = Service.StaticDataController;
			string[] array = null;
			string[] perkEffects = targetPerkVO.PerkEffects;
			int num = perkEffects.Length;
			statGrid.SetTemplateItem(templateElement);
			string perkGroup = targetPerkVO.PerkGroup;
			int perkTier = targetPerkVO.PerkTier;
			if (useUpgrade && perkTier > 1)
			{
				PerkVO perkByGroupAndTier = GameUtils.GetPerkByGroupAndTier(perkGroup, perkTier - 1);
				array = perkByGroupAndTier.PerkEffects;
				if (perkEffects.Length != num)
				{
					Service.Logger.Error("PerkEffects list not consistent between " + perkByGroupAndTier.Uid + " and " + targetPerkVO.Uid);
				}
			}
			statGrid.Clear();
			for (int i = 0; i < num; i++)
			{
				PerkEffectVO perkEffectVO = staticDataController.Get<PerkEffectVO>(perkEffects[i]);
				PerkEffectVO prevVO = null;
				if (array != null)
				{
					prevVO = staticDataController.Get<PerkEffectVO>(array[i]);
				}
				string itemUid = perkEffectVO.Uid + i;
				UXElement item = statGrid.CloneTemplateItem(itemUid);
				UXLabel subElement = statGrid.GetSubElement<UXLabel>(itemUid, descriptionElement);
				UXLabel subElement2 = statGrid.GetSubElement<UXLabel>(itemUid, valueElement);
				subElement.Text = lang.Get(perkEffectVO.StatStringId, new object[0]);
				subElement2.Text = this.GetFormattedValueBasedOnEffectType(perkEffectVO, prevVO);
				statGrid.AddItem(item, i);
			}
			statGrid.RepositionItems();
		}

		public string GetFormattedValueBasedOnEffectType(PerkEffectVO currentVO, PerkEffectVO prevVO)
		{
			Lang lang = Service.Lang;
			string empty = string.Empty;
			string displayValueForPerk = this.GetDisplayValueForPerk(currentVO);
			string id = currentVO.StatValueFormatStringId;
			if (prevVO != null)
			{
				string displayValueForPerk2 = this.GetDisplayValueForPerk(prevVO);
				if (displayValueForPerk2 != displayValueForPerk)
				{
					id = currentVO.StatUpgradeValueFormatStringId;
					return lang.Get(id, new object[]
					{
						displayValueForPerk2,
						displayValueForPerk
					});
				}
			}
			return lang.Get(id, new object[]
			{
				displayValueForPerk
			});
		}

		private string GetDisplayValueForPerk(PerkEffectVO vo)
		{
			string type = vo.Type;
			string result = string.Empty;
			if ("troopRequestTime" == type)
			{
				result = LangUtils.FormatTime((long)vo.TroopRequestTimeDiscount);
			}
			else if ("generator" == type)
			{
				result = Mathf.FloorToInt(vo.GenerationRate * 100f).ToString();
			}
			else if ("contractCost" == type)
			{
				result = Mathf.FloorToInt(vo.ContractDiscount * 100f).ToString();
			}
			else if ("contractTime" == type)
			{
				result = Mathf.FloorToInt(vo.ContractTimeReduction * 100f).ToString();
			}
			else if ("relocation" == type)
			{
				result = vo.RelocationDiscount.ToString();
			}
			else if ("troopRequestAmount" == type)
			{
				result = vo.TroopRequestAmount.ToString();
			}
			return result;
		}

		public void ShowActivePerksScreen(BuildingTypeVO vo)
		{
			ActivePerksInfoScreen screen = new ActivePerksInfoScreen(vo);
			Service.ScreenController.AddScreen(screen);
		}

		public void OnPerksButtonClicked(UXButton button)
		{
			this.ShowActivePerksScreen((BuildingTypeVO)button.Tag);
		}

		public void ShowCancelPerkAlert(string perkId, string perkGroup)
		{
			Lang lang = Service.Lang;
			string title = lang.Get("PERK_CANCEL_POPUP_TITLE", new object[0]);
			string perkNameForGroup = this.GetPerkNameForGroup(perkGroup);
			string message = lang.Get("PERK_CANCEL_POPUP_DESC", new object[]
			{
				perkNameForGroup
			});
			bool alwaysOnTop = true;
			YesNoScreen.ShowModal(title, message, false, false, false, alwaysOnTop, new OnScreenModalResult(this.OnCancelPerkModalResult), perkId);
		}

		public uint GetLastViewedPerkTime()
		{
			SharedPlayerPrefs sharedPlayerPrefs = Service.SharedPlayerPrefs;
			return sharedPlayerPrefs.GetPref<uint>("perks_last_view");
		}

		public void UpdateLastViewedPerkTime()
		{
			SharedPlayerPrefs sharedPlayerPrefs = Service.SharedPlayerPrefs;
			sharedPlayerPrefs.SetPref("perks_last_view", Service.ServerAPI.ServerTime.ToString());
		}

		public void AddToPerkBadgeList(string perkId)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			PerkVO perkVO = staticDataController.Get<PerkVO>(perkId);
			if (perkVO != null)
			{
				List<string> list = this.GetListOfBadgedPerkGroups();
				if (list == null)
				{
					list = new List<string>();
				}
				int count = list.Count;
				int num = GameConstants.SQUADPERK_MAX_PERK_CARD_BADGES - count;
				if (num <= 0)
				{
					this.TrimPerkBadgeList(ref list, Math.Abs(num) + 1);
				}
				string perkGroup = perkVO.PerkGroup;
				list.Add(perkGroup);
				this.SetPerkBadgeList(list);
			}
			else
			{
				Service.Logger.Error("PerkViewController.AddToPerkBadgeList Failed to find Perk Data for: " + perkId);
			}
		}

		public void RemovePerkGroupFromBadgeList(string perkGroup)
		{
			List<string> listOfBadgedPerkGroups = this.GetListOfBadgedPerkGroups();
			if (listOfBadgedPerkGroups != null && listOfBadgedPerkGroups.Count > 0)
			{
				listOfBadgedPerkGroups.Remove(perkGroup);
				this.SetPerkBadgeList(listOfBadgedPerkGroups);
			}
		}

		public int GetBadgedPerkCount()
		{
			List<string> listOfBadgedPerkGroups = this.GetListOfBadgedPerkGroups();
			int result = 0;
			if (listOfBadgedPerkGroups != null)
			{
				result = listOfBadgedPerkGroups.Count;
			}
			return result;
		}

		public bool IsPerkGroupBadged(string perkGroup)
		{
			bool result = false;
			List<string> listOfBadgedPerkGroups = this.GetListOfBadgedPerkGroups();
			if (listOfBadgedPerkGroups != null && listOfBadgedPerkGroups.Count > 0)
			{
				result = listOfBadgedPerkGroups.Contains(perkGroup);
			}
			return result;
		}

		private void TrimPerkBadgeList(ref List<string> perkBadges, int amtToRemove)
		{
			for (int i = 0; i < amtToRemove; i++)
			{
				if (perkBadges.Count == 0)
				{
					break;
				}
				perkBadges.RemoveAt(0);
			}
		}

		private List<string> GetListOfBadgedPerkGroups()
		{
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			if (currentSquad == null)
			{
				return null;
			}
			Dictionary<string, string> available = currentSquad.Perks.Available;
			int level = currentSquad.Level;
			PerkManager perkManager = Service.PerkManager;
			SharedPlayerPrefs sharedPlayerPrefs = Service.SharedPlayerPrefs;
			string pref = sharedPlayerPrefs.GetPref<string>("perk_badges");
			if (!string.IsNullOrEmpty(pref))
			{
				List<string> list = new List<string>();
				string[] array = pref.Split(new char[]
				{
					' '
				});
				int num = array.Length;
				for (int i = 0; i < num; i++)
				{
					string text = array[i];
					if (available.ContainsKey(text))
					{
						PerkVO perkData = Service.StaticDataController.Get<PerkVO>(available[text]);
						if (!perkManager.IsPerkLevelLocked(perkData, level) && !perkManager.IsPerkReputationLocked(perkData, level, available))
						{
							if (!perkManager.IsPerkGroupActive(text) && !perkManager.IsPerkGroupInCooldown(text))
							{
								list.Add(text);
							}
						}
					}
				}
				return list;
			}
			return null;
		}

		private void SetPerkBadgeList(List<string> badgedGroups)
		{
			SharedPlayerPrefs sharedPlayerPrefs = Service.SharedPlayerPrefs;
			string text = string.Empty;
			if (badgedGroups != null)
			{
				int count = badgedGroups.Count;
				for (int i = 0; i < count; i++)
				{
					text += badgedGroups[i];
					if (i < count - 1)
					{
						text += " ";
					}
				}
			}
			sharedPlayerPrefs.SetPrefUnlimitedLength("perk_badges", text);
		}

		public void ShowSquadLevelUpIfPending()
		{
			ScreenController screenController = Service.ScreenController;
			PerkManager perkManager = Service.PerkManager;
			SquadController squadController = Service.SquadController;
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			int level = currentSquad.Level;
			int lastViewedSquadLevelUp = squadController.GetLastViewedSquadLevelUp();
			if (!perkManager.HasPlayerSeenPerkTutorial())
			{
				return;
			}
			if (level > 1 && level > lastViewedSquadLevelUp)
			{
				squadController.SetLastViewedSquadLevelUp(level);
				int num = 1;
				if (lastViewedSquadLevelUp > 0)
				{
					num = level - lastViewedSquadLevelUp;
				}
				int sQUADPERK_MAX_SQUAD_LEVEL_CELEBRATIONS_SHOWN = GameConstants.SQUADPERK_MAX_SQUAD_LEVEL_CELEBRATIONS_SHOWN;
				if (sQUADPERK_MAX_SQUAD_LEVEL_CELEBRATIONS_SHOWN < num)
				{
					num = sQUADPERK_MAX_SQUAD_LEVEL_CELEBRATIONS_SHOWN;
				}
				int num2 = level - num + 1;
				QueueScreenBehavior subType = QueueScreenBehavior.Default;
				SquadLevelUpCelebrationScreen highestLevelScreen = screenController.GetHighestLevelScreen<SquadLevelUpCelebrationScreen>();
				if (highestLevelScreen != null)
				{
					num2 = level;
					subType = QueueScreenBehavior.QueueAndDeferTillClosed;
				}
				for (int i = num2; i <= level; i++)
				{
					List<PerkVO> perksUnlockedAtSquadLevel = perkManager.GetPerksUnlockedAtSquadLevel(i);
					SquadLevelUpCelebrationScreen screen = new SquadLevelUpCelebrationScreen(i, perksUnlockedAtSquadLevel);
					screenController.AddScreen(screen, subType);
					subType = QueueScreenBehavior.QueueAndDeferTillClosed;
				}
			}
		}

		private void OnCancelPerkModalResult(object result, object cookie)
		{
			if (result == null)
			{
				return;
			}
			PerkManager perkManager = Service.PerkManager;
			string text = cookie as string;
			bool flag = perkManager.CancelPlayerPerk(text);
			if (flag)
			{
				PerkVO perkVO = Service.StaticDataController.Get<PerkVO>(text);
				List<Entity> buildingsForPerk = perkManager.GetBuildingsForPerk(perkVO);
				int i = 0;
				int count = buildingsForPerk.Count;
				while (i < count)
				{
					Service.BuildingController.UpdateBuildingHighlightForPerks(buildingsForPerk[i]);
					i++;
				}
			}
		}
	}
}
