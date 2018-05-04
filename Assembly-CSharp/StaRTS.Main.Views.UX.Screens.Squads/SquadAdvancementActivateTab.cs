using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Perks;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.Squads
{
	public class SquadAdvancementActivateTab : SquadAdvancementBaseTab, IEventObserver, IViewClockTimeObserver
	{
		private float TIME_DELAY_TO_SHOW_CELEB = 0.5f;

		private const int AVAILABLE_SORT_ORDER_REPUTATION_OFFSET = 10000;

		private const int AVAILABLE_SORT_ORDER_LOCK_OFFSET = 20000;

		private const string PERK_SLOT_PREFIX = "PerkSlot_";

		private const string PERK_ACTIVATE_GROUP = "ActivateGroupPerks";

		private const string ACTIVATE_VIEW_FILTER_GRID = "GridActFilterPerks";

		private const string ACTIVATE_VIEW_FILTER_TEMPLATE = "TemplateActFilterPerks";

		private const string ACTIVATE_VIEW_FILTER_TEMPLATE_BTN = "BtnActFilterPerks";

		private const string ACTIVATE_VIEW_FILTER_TEMPLATE_LABEL = "LabelActFilterPerks";

		private const string GRID_ACTIVE_PERKS = "GridActSlotsPerks";

		private const string TEMPLATE_ACTIVE_PERKS = "TemplateActSlotPerks";

		private const string GROUP_ACTIVATED_SLOT = "ActivatedSlotGroupPerks";

		private const string GROUP_LOCKED_SLOT = "LockedSlotGroupPerks";

		private const string LABEL_LOCKED_SLOT = "LabelSquadLvlLockedSlotPerks";

		private const string LABEL_LOCKED_SLOT_REQUIRED = "LabelSquadLvlLockedSlotPerks";

		private const string LABEL_ACTIVE_LEVEL = "LabelPerkLvlActSlotPerks";

		private const string LABEL_ACTIVE_TITLE = "LabelPerkTitleActSlotPerks";

		private const string LABEL_ACTIVE_TIMER = "LabelPerkTimerActSlotPerks";

		private const string ACTIVE_SLOT_PERK_IMAGE = "TexturePerkArtActSlotPerks";

		private const string BUTTON_REMOVE_ACTIVE = "BtnRemoveActSlotPerks";

		private const string GROUP_OPEN_SLOT = "AvailableSlotGroupPerks";

		private const string OPEN_PERK_SLOT_LABEL = "LabelAvActSlotPerks";

		private const string LABEL_TIMER_MESSAGE = "LabelPerkMessageTimerActSlotPerks";

		private const string LABEL_TITLE_AVAILABLE_PERKS = "LabelTitleAvailablePerks";

		private const string GRID_AVAILABLE_PERKS = "GridAvActPerks";

		private const string TEMPLATE_AVAILABLE_PERKS = "TemplateAvActCardPerks";

		private const string BUTTON_AVAILABLE = "BtnAvActCardPerks";

		private const string ACTIVATE_VIEW_PERK_IMAGE = "TexturePerkArtAvActCardPerks";

		private const string GROUP_AVAILABLE_COOLDOWN = "CoolDownGroupAvActCardPerks";

		private const string LABEL_AVAILABLE_COOLDOWN_READY_NOW = "LabelReadyNowAvActCardPerks";

		private const string LABEL_AVAILABLE_COOLDOWN = "LabelCoolDownAvActCardPerks";

		private const string GROUP_AVAILABLE_LOCKED = "LockedGroupAvActCardPerks";

		private const string LABEL_AVAILABLE_LOCKED_REQUIRED = "LabelSquadLvlLockedAvActCardPerks";

		private const string LABEL_AVAILABLE_TITLE = "LabelPerkTitleAvActCardPerks";

		private const string LABEL_AVAILABLE_LEVEL = "LabelPerkLvlAvActCardPerks";

		private const string GROUP_AVAILABLE_COST_TOP = "CostAvActPerkTop";

		private const string GROUP_AVAILABLE_COST_BOTTOM = "CostAvActPerkBot";

		private const string LABEL_AVAILABLE_COOLDOWN_COST = "CostReadyNowPerksLabel";

		private static readonly string[] singleCostElementName = new string[]
		{
			"CostAvActPerkBot"
		};

		private static readonly string[] dualCostElementNames = new string[]
		{
			"CostAvActPerkTop",
			"CostAvActPerkBot"
		};

		private const string LANG_PERK_ACTIVE_SLOTS_TITLE = "PERK_ACTIVE_SLOTS_TITLE";

		private const string LANG_PERK_ACTIVE_AVAILABLE_TITLE = "PERK_ACTIVE_AVAILABLE_TITLE";

		private const string LANG_PERK_UPGRADE_LVL_REQ = "PERK_UPGRADE_LVL_REQ";

		private const string LANG_PERK_ACTIVE_SLOT_ACTIVE_TIMER = "PERK_ACTIVE_SLOT_ACTIVE_TIMER";

		private const string LANG_PERK_ACTIVE_COOLDOWN_CTA = "PERK_ACTIVE_CARD_COOLDOWN_CTA";

		private const string LANG_PERK_ACTIVE_COOLDOWN_TIMER = "PERK_ACTIVE_CARD_COOLDOWN_TIMER";

		private const string LANG_PERK_ACTIVE_SLOT_TITLE = "PERK_ACTIVE_SLOT_TITLE";

		private const string LANG_PERK_ACTIVE_POPUP_REP_REQ = "PERK_ACTIVATE_POPUP_REP_REQ";

		private const string LANG_PERK_ACTIVE_UPGRADE_CARD_LVL_REQ = "PERK_ACTIVATE_UPGRADE_CARD_LVL_REQ";

		private const string LANG_PERK_ACTIVE_SLOT_ACTIVE_TIMER_DESC = "PERK_ACTIVE_SLOT_ACTIVE_TIMER_DESC";

		private UXGrid activePerksGrid;

		private UXGrid perksGrid;

		private List<UXLabel> activePerkTimerLabels;

		private List<UXLabel> cooldownTimerLabels;

		private List<UXLabel> cooldownCostLabels;

		private bool needsPerkStatesRefresh;

		private SquadScreenActivationInfoView activationInfoView;

		public SquadAdvancementActivateTab(SquadSlidingScreen screen, string tabLabelName, string tabLabelString) : base(screen, "ActivateGroupPerks", tabLabelName, tabLabelString)
		{
			this.activePerkTimerLabels = new List<UXLabel>();
			this.cooldownTimerLabels = new List<UXLabel>();
			this.cooldownCostLabels = new List<UXLabel>();
			this.perksGrid = screen.GetElement<UXGrid>("GridAvActPerks");
			this.perksGrid.SetTemplateItem("TemplateAvActCardPerks");
			base.InitFilterGrid("GridActFilterPerks", "TemplateActFilterPerks", "BtnActFilterPerks", "LabelActFilterPerks", this.perksGrid);
			this.InitLabels();
			this.RefreshPerkStates();
		}

		private void ResetVisibilityStates()
		{
			this.baseView.Visible = true;
			this.openedModalOnTop = false;
		}

		private void ShowCelebAfterDelay(uint timerId, object cookie)
		{
			PerkVO perkData = (PerkVO)cookie;
			base.ShowPerkCelebration(perkData);
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.ActivePerksUpdated:
			case EventId.SquadPerkUpdated:
			case EventId.SquadLeveledUp:
				this.RefreshPerkStates();
				break;
			case EventId.PerkUnlocked:
			case EventId.PerkUpgraded:
				if (this.activationInfoView != null)
				{
					PerkViewController perkViewController = Service.PerkViewController;
					PerkVO perkVO = (PerkVO)cookie;
					string perkGroup = perkVO.PerkGroup;
					if (perkViewController.IsPerkGroupBadged(perkGroup))
					{
						this.CleanUpActivationInfoView();
						perkViewController.RemovePerkGroupFromBadgeList(perkGroup);
						this.screen.UpdateBadges();
						Service.ViewTimerManager.CreateViewTimer(this.TIME_DELAY_TO_SHOW_CELEB, false, new TimerDelegate(this.ShowCelebAfterDelay), cookie);
						return EatResponse.NotEaten;
					}
				}
				break;
			case EventId.PerkCelebClosed:
				this.ResetVisibilityStates();
				this.RefreshPerkStates();
				break;
			case EventId.PerkActivationClosed:
				this.activationInfoView = null;
				this.ResetVisibilityStates();
				this.RefreshPerkStates();
				break;
			}
			return base.OnEvent(id, cookie);
		}

		protected override void RegisterEvents()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.SquadLeveledUp);
			eventManager.RegisterObserver(this, EventId.ActivePerksUpdated);
			eventManager.RegisterObserver(this, EventId.SquadPerkUpdated);
			eventManager.RegisterObserver(this, EventId.PerkCelebClosed);
			eventManager.RegisterObserver(this, EventId.PerkActivationClosed);
			base.RegisterEvents();
		}

		protected override void UnregisterEvents()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.SquadLeveledUp);
			eventManager.UnregisterObserver(this, EventId.ActivePerksUpdated);
			eventManager.UnregisterObserver(this, EventId.SquadPerkUpdated);
			eventManager.UnregisterObserver(this, EventId.PerkCelebClosed);
			eventManager.UnregisterObserver(this, EventId.PerkActivationClosed);
			base.UnregisterEvents();
		}

		public override void OnDestroyElement()
		{
			this.UnregisterClockTimeObserver();
			this.UnregisterEvents();
			if (this.activePerkTimerLabels != null)
			{
				this.activePerkTimerLabels.Clear();
				this.activePerkTimerLabels = null;
			}
			if (this.cooldownTimerLabels != null)
			{
				this.cooldownTimerLabels.Clear();
				this.cooldownTimerLabels = null;
			}
			if (this.cooldownCostLabels != null)
			{
				this.cooldownCostLabels.Clear();
				this.cooldownCostLabels = null;
			}
			if (this.activePerksGrid != null)
			{
				this.activePerksGrid.Clear();
				this.activePerksGrid = null;
			}
			if (this.perksGrid != null)
			{
				this.perksGrid.Clear();
				this.perksGrid = null;
			}
			this.CleanUpActivationInfoView();
			base.OnDestroyElement();
		}

		protected override void OnShow()
		{
			this.CleanUpActivationInfoView();
			this.RegisterEvents();
			this.RefreshPerkStates();
		}

		protected override void OnHide()
		{
			this.lastGridPosition = 0f;
			this.CleanUpActivationInfoView();
			this.UnregisterEvents();
			this.UnregisterClockTimeObserver();
		}

		private void CleanUpActivationInfoView()
		{
			this.openedModalOnTop = false;
			if (this.activationInfoView != null)
			{
				this.activationInfoView.HideAndCleanUp();
				this.activationInfoView = null;
			}
		}

		public void UnregisterClockTimeObserver()
		{
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			this.activePerkTimerLabels.Clear();
			this.cooldownTimerLabels.Clear();
			this.cooldownCostLabels.Clear();
		}

		public void InitLabels()
		{
			Lang lang = Service.Lang;
			UXLabel element = this.screen.GetElement<UXLabel>("LabelTitleAvailablePerks");
			element.Text = lang.Get("PERK_ACTIVE_AVAILABLE_TITLE", new object[0]);
		}

		public override void RefreshPerkStates()
		{
			this.UnregisterClockTimeObserver();
			this.needsPerkStatesRefresh = false;
			this.RefreshActivePerksGrid();
			this.RefreshAvailablePerksGrid();
			if (this.activePerkTimerLabels.Count > 0 || this.cooldownTimerLabels.Count > 0 || this.cooldownCostLabels.Count > 0)
			{
				Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
			}
			this.perksGrid.Scroll(this.lastGridPosition);
			base.RefreshPerkStates();
		}

		private void RefreshAvailablePerksGrid()
		{
			this.cooldownTimerLabels.Clear();
			this.cooldownCostLabels.Clear();
			this.perkBadgeMap.Clear();
			StaticDataController staticDataController = Service.StaticDataController;
			Lang lang = Service.Lang;
			PerkManager perkManager = Service.PerkManager;
			PerkViewController perkViewController = Service.PerkViewController;
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			if (currentSquad == null)
			{
				return;
			}
			Dictionary<string, string> available = currentSquad.Perks.Available;
			int level = currentSquad.Level;
			List<UXElement> list = new List<UXElement>();
			foreach (PerkVO current in staticDataController.GetAll<PerkVO>())
			{
				string uid = current.Uid;
				string perkGroup = current.PerkGroup;
				int perkTier = current.PerkTier;
				int squadLevelUnlock = current.SquadLevelUnlock;
				bool flag = perkManager.IsPerkLevelLocked(current, level);
				bool flag2 = perkManager.IsPerkReputationLocked(current, level, available);
				bool flag3 = flag || flag2;
				bool flag4 = perkManager.IsPerkGroupActive(perkGroup);
				bool flag5 = perkManager.IsPerkGroupInCooldown(perkGroup);
				if (!available.ContainsKey(perkGroup) || !(available[perkGroup] != current.Uid))
				{
					if (!flag3 || perkTier == 1)
					{
						string text = "PerkItem_" + perkGroup;
						UXElement uXElement = base.FetchPerkGridItem(this.perksGrid, text);
						uXElement.Tag = current;
						UXElement subElement = this.perksGrid.GetSubElement<UXElement>(text, "CoolDownGroupAvActCardPerks");
						UXElement subElement2 = this.perksGrid.GetSubElement<UXElement>(text, "LockedGroupAvActCardPerks");
						UXElement subElement3 = this.perksGrid.GetSubElement<UXElement>(text, "CostAvActPerkTop");
						subElement3.Visible = false;
						UXElement subElement4 = this.perksGrid.GetSubElement<UXElement>(text, "CostAvActPerkBot");
						subElement4.Visible = false;
						UXLabel subElement5 = this.perksGrid.GetSubElement<UXLabel>(text, "LabelPerkTitleAvActCardPerks");
						subElement5.Text = perkViewController.GetPerkNameForGroup(perkGroup);
						UXLabel subElement6 = this.perksGrid.GetSubElement<UXLabel>(text, "LabelPerkLvlAvActCardPerks");
						subElement6.Text = StringUtils.GetRomanNumeral(current.PerkTier);
						subElement6.Visible = !flag3;
						UXTexture subElement7 = this.perksGrid.GetSubElement<UXTexture>(text, "TexturePerkArtAvActCardPerks");
						perkViewController.SetPerkImage(subElement7, current);
						UXButton subElement8 = this.perksGrid.GetSubElement<UXButton>(text, "BtnAvActCardPerks");
						subElement8.Tag = current;
						subElement8.OnClicked = new UXButtonClickedDelegate(this.OnPerkButtonClicked);
						uXElement.Visible = true;
						if (flag3)
						{
							subElement.Visible = false;
							subElement2.Visible = true;
							UXLabel subElement9 = this.perksGrid.GetSubElement<UXLabel>(text, "LabelSquadLvlLockedAvActCardPerks");
							if (flag)
							{
								subElement9.Text = lang.Get("PERK_ACTIVATE_UPGRADE_CARD_LVL_REQ", new object[]
								{
									squadLevelUnlock
								});
							}
							else
							{
								subElement9.Text = lang.Get("PERK_ACTIVATE_POPUP_REP_REQ", new object[0]);
							}
						}
						else if (flag4)
						{
							uXElement.Visible = false;
						}
						else if (flag5)
						{
							subElement.Visible = true;
							subElement2.Visible = false;
							UXLabel subElement10 = this.activePerksGrid.GetSubElement<UXLabel>(text, "LabelReadyNowAvActCardPerks");
							subElement10.Text = lang.Get("PERK_ACTIVE_CARD_COOLDOWN_CTA", new object[0]);
							UXLabel subElement11 = this.activePerksGrid.GetSubElement<UXLabel>(text, "LabelCoolDownAvActCardPerks");
							uint num = perkManager.GetPlayerPerkGroupCooldowns()[perkGroup];
							this.UpdateLabelTimeRemaining(subElement11, lang, "PERK_ACTIVE_CARD_COOLDOWN_TIMER", num);
							subElement11.Tag = num;
							this.cooldownTimerLabels.Add(subElement11);
							UXLabel subElement12 = this.activePerksGrid.GetSubElement<UXLabel>(text, "CostReadyNowPerksLabel");
							subElement12.Tag = uid;
							this.cooldownCostLabels.Add(subElement12);
							this.UpdateGridItemCooldownSkipCost(subElement12, uid);
							subElement8.OnClicked = new UXButtonClickedDelegate(this.OnCooldownButtonClicked);
						}
						else if (!flag4 && !flag5)
						{
							subElement.Visible = false;
							subElement2.Visible = false;
							Dictionary<string, int> hQScaledCostForPlayer = GameUtils.GetHQScaledCostForPlayer(current.ActivationCost);
							int count = hQScaledCostForPlayer.Count;
							string[] costElementNames = (count != 2) ? SquadAdvancementActivateTab.singleCostElementName : SquadAdvancementActivateTab.dualCostElementNames;
							UXUtils.SetupMultiCostElements(this.screen, costElementNames, text, current.ActivationCost, count);
						}
						base.SetupPerkBadge(current, text, "ActCardPerks");
						list.Add(uXElement);
					}
				}
			}
			list.Sort(new Comparison<UXElement>(this.SortAvailableList));
			this.perksGrid.ClearWithoutDestroy();
			int i = 0;
			int count2 = list.Count;
			while (i < count2)
			{
				this.perksGrid.AddItem(list[i], i);
				i++;
			}
			this.perksGrid.RepositionItemsFrameDelayed(new UXDragDelegate(base.OnRepositionComplete));
			list.Clear();
			list = null;
		}

		private int SortAvailableList(UXElement a, UXElement b)
		{
			PerkVO perkVO = (PerkVO)a.Tag;
			PerkVO perkVO2 = (PerkVO)b.Tag;
			PerkManager perkManager = Service.PerkManager;
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			int level = currentSquad.Level;
			Dictionary<string, string> available = currentSquad.Perks.Available;
			int num = perkVO.SortOrder;
			int num2 = perkVO2.SortOrder;
			if (perkManager.IsPerkLevelLocked(perkVO, level) && perkManager.IsPerkLevelLocked(perkVO2, level))
			{
				num = perkVO.SquadLevelUnlock;
				num2 = perkVO2.SquadLevelUnlock;
			}
			if (perkManager.IsPerkReputationLocked(perkVO, level, available))
			{
				num += 10000;
			}
			else if (perkManager.IsPerkLevelLocked(perkVO, level))
			{
				num += 20000;
			}
			if (perkManager.IsPerkReputationLocked(perkVO2, level, available))
			{
				num2 += 10000;
			}
			else if (perkManager.IsPerkLevelLocked(perkVO2, level))
			{
				num2 += 20000;
			}
			return num - num2;
		}

		private void RefreshActivePerksGrid()
		{
			if (this.activePerksGrid == null)
			{
				this.activePerksGrid = this.screen.GetElement<UXGrid>("GridActSlotsPerks");
				this.activePerksGrid.SetTemplateItem("TemplateActSlotPerks");
			}
			this.activePerkTimerLabels.Clear();
			StaticDataController staticDataController = Service.StaticDataController;
			Lang lang = Service.Lang;
			PerkManager perkManager = Service.PerkManager;
			List<ActivatedPerkData> playerActivePerks = perkManager.GetPlayerActivePerks();
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			if (currentSquad == null)
			{
				return;
			}
			string squadLevelUIDFromSquad = GameUtils.GetSquadLevelUIDFromSquad(currentSquad);
			int availableSlotsCount = perkManager.GetAvailableSlotsCount(squadLevelUIDFromSquad);
			int squadLevelMax = perkManager.SquadLevelMax;
			List<UXElement> list = new List<UXElement>();
			int num = -1;
			int i = 1;
			int num2 = 0;
			while (i <= squadLevelMax)
			{
				string squadLevelUIDFromLevel = GameUtils.GetSquadLevelUIDFromLevel(i);
				SquadLevelVO squadLevelVO = staticDataController.Get<SquadLevelVO>(squadLevelUIDFromLevel);
				int slots = squadLevelVO.Slots;
				if (num != slots)
				{
					bool flag = slots > availableSlotsCount;
					bool flag2 = num2 < playerActivePerks.Count && !flag;
					int level = squadLevelVO.Level;
					string itemUid = "PerkSlot_" + slots.ToString();
					UXElement uXElement = base.FetchPerkGridItem(this.activePerksGrid, itemUid);
					UXElement subElement = this.activePerksGrid.GetSubElement<UXElement>(itemUid, "AvailableSlotGroupPerks");
					UXLabel subElement2 = this.activePerksGrid.GetSubElement<UXLabel>(itemUid, "LabelAvActSlotPerks");
					UXElement subElement3 = this.activePerksGrid.GetSubElement<UXElement>(itemUid, "ActivatedSlotGroupPerks");
					UXElement subElement4 = this.activePerksGrid.GetSubElement<UXElement>(itemUid, "LockedSlotGroupPerks");
					UXLabel subElement5 = this.activePerksGrid.GetSubElement<UXLabel>(itemUid, "LabelPerkTitleActSlotPerks");
					UXLabel subElement6 = this.activePerksGrid.GetSubElement<UXLabel>(itemUid, "LabelPerkLvlActSlotPerks");
					UXLabel subElement7 = this.activePerksGrid.GetSubElement<UXLabel>(itemUid, "LabelPerkMessageTimerActSlotPerks");
					UXLabel subElement8 = this.activePerksGrid.GetSubElement<UXLabel>(itemUid, "LabelPerkTimerActSlotPerks");
					UXButton subElement9 = this.activePerksGrid.GetSubElement<UXButton>(itemUid, "BtnRemoveActSlotPerks");
					UXTexture subElement10 = this.activePerksGrid.GetSubElement<UXTexture>(itemUid, "TexturePerkArtActSlotPerks");
					UXButton uXButton = uXElement as UXButton;
					if (flag)
					{
						uXButton.Enabled = false;
						subElement.Visible = false;
						subElement4.Visible = true;
						subElement3.Visible = false;
						subElement9.Visible = false;
						UXLabel subElement11 = this.activePerksGrid.GetSubElement<UXLabel>(itemUid, "LabelSquadLvlLockedSlotPerks");
						subElement11.Text = level.ToString();
						UXLabel subElement12 = this.activePerksGrid.GetSubElement<UXLabel>(itemUid, "LabelSquadLvlLockedSlotPerks");
						subElement12.Text = lang.Get("PERK_ACTIVATE_UPGRADE_CARD_LVL_REQ", new object[]
						{
							level
						});
					}
					else if (flag2)
					{
						subElement.Visible = false;
						subElement4.Visible = false;
						subElement3.Visible = true;
						subElement9.Visible = true;
						ActivatedPerkData activatedPerkData = playerActivePerks[num2++];
						string perkId = activatedPerkData.PerkId;
						PerkViewController perkViewController = Service.PerkViewController;
						PerkVO perkVO = staticDataController.Get<PerkVO>(perkId);
						subElement5.Text = perkViewController.GetPerkNameForGroup(perkVO.PerkGroup);
						subElement6.Text = StringUtils.GetRomanNumeral(perkVO.PerkTier);
						perkViewController.SetPerkImage(subElement10, perkVO);
						subElement7.Text = lang.Get("PERK_ACTIVE_SLOT_ACTIVE_TIMER_DESC", new object[0]);
						this.UpdateLabelTimeRemaining(subElement8, lang, "PERK_ACTIVE_SLOT_ACTIVE_TIMER", activatedPerkData.EndTime);
						subElement8.Tag = activatedPerkData;
						this.activePerkTimerLabels.Add(subElement8);
						uXButton.Enabled = true;
						uXButton.Tag = perkVO;
						uXButton.OnClicked = new UXButtonClickedDelegate(this.OnPerkSlotButtonClicked);
						subElement9.Tag = perkId;
						subElement9.Visible = true;
						subElement9.OnClicked = new UXButtonClickedDelegate(this.OnRemoveButtonClicked);
					}
					else
					{
						uXButton.Enabled = false;
						subElement4.Visible = false;
						subElement3.Visible = false;
						subElement9.Visible = false;
						subElement.Visible = true;
						subElement2.Text = lang.Get("PERK_ACTIVE_SLOT_TITLE", new object[0]);
					}
					list.Add(uXElement);
					num = slots;
				}
				i++;
			}
			this.activePerksGrid.ClearWithoutDestroy();
			int j = 0;
			int count = list.Count;
			while (j < count)
			{
				this.activePerksGrid.AddItem(list[j], j);
				j++;
			}
			this.activePerksGrid.RepositionItems();
			list.Clear();
		}

		private int SortPerkItemByLevelAndOrder(UXElement a, UXElement b)
		{
			PerkVO perkVO = (PerkVO)a.Tag;
			PerkVO perkVO2 = (PerkVO)b.Tag;
			int num = perkVO.SquadLevelUnlock.CompareTo(perkVO2.SquadLevelUnlock);
			if (num != 0)
			{
				return num;
			}
			return perkVO.SortOrder.CompareTo(perkVO2.SortOrder);
		}

		protected override bool CanShowGridItem(UXElement item)
		{
			PerkVO perkVO = (PerkVO)item.Tag;
			return (perkVO == null || !Service.PerkManager.IsPerkGroupActive(perkVO.PerkGroup)) && base.CanShowGridItem(item);
		}

		private void UpdateGridItemCooldownSkipCost(UXLabel cooldownSkipCostLabel, string perkId)
		{
			int perkCooldownTimeLeft = this.GetPerkCooldownTimeLeft(perkId);
			int crystals = GameUtils.SecondsToCrystalsForPerk(perkCooldownTimeLeft);
			cooldownSkipCostLabel.Text = crystals.ToString();
			cooldownSkipCostLabel.TextColor = UXUtils.GetCostColor(cooldownSkipCostLabel, GameUtils.CanAffordCrystals(crystals), false);
		}

		private int GetPerkCooldownTimeLeft(string perkId)
		{
			Dictionary<string, uint> playerPerkGroupCooldowns = Service.PerkManager.GetPlayerPerkGroupCooldowns();
			StaticDataController staticDataController = Service.StaticDataController;
			string perkGroup = staticDataController.Get<PerkVO>(perkId).PerkGroup;
			if (!playerPerkGroupCooldowns.ContainsKey(perkGroup))
			{
				return 0;
			}
			return (int)(playerPerkGroupCooldowns[perkGroup] - ServerTime.Time);
		}

		private void ShowActivationView(PerkVO perkVO, bool isActivation)
		{
			this.lastGridPosition = this.perksGrid.GetCurrentScrollPosition(true);
			this.openedModalOnTop = true;
			this.activationInfoView = new SquadScreenActivationInfoView(this.screen, perkVO, isActivation);
			this.activationInfoView.Show();
		}

		private void OnPerkSlotButtonClicked(UXButton button)
		{
			if (base.ShouldBlockInput())
			{
				return;
			}
			Service.EventManager.SendEvent(EventId.PerkSelected, null);
			PerkVO perkVO = (PerkVO)button.Tag;
			this.ShowActivationView(perkVO, false);
		}

		private void OnPerkButtonClicked(UXButton button)
		{
			if (base.ShouldBlockInput())
			{
				return;
			}
			PerkViewController perkViewController = Service.PerkViewController;
			PerkVO perkVO = (PerkVO)button.Tag;
			string perkGroup = perkVO.PerkGroup;
			if (perkViewController.IsPerkGroupBadged(perkGroup))
			{
				perkViewController.RemovePerkGroupFromBadgeList(perkGroup);
				this.screen.UpdateBadges();
			}
			Service.EventManager.SendEvent(EventId.PerkSelected, null);
			this.ShowActivationView(perkVO, true);
		}

		private void OnCooldownButtonClicked(UXButton button)
		{
			if (base.ShouldBlockInput())
			{
				return;
			}
			this.lastGridPosition = this.perksGrid.GetCurrentScrollPosition(true);
			PerkVO perkVO = (PerkVO)button.Tag;
			string uid = perkVO.Uid;
			string perkNameForGroup = Service.PerkViewController.GetPerkNameForGroup(perkVO.PerkGroup);
			int perkCooldownTimeLeft = this.GetPerkCooldownTimeLeft(uid);
			string crystalCost = GameUtils.SecondsToCrystalsForPerk(perkCooldownTimeLeft).ToString();
			Service.PerkManager.ConfirmPurchaseCooldownSkip(uid, perkNameForGroup, LangUtils.FormatTime((long)perkCooldownTimeLeft), crystalCost);
		}

		private void OnRemoveButtonClicked(UXButton button)
		{
			if (base.ShouldBlockInput())
			{
				return;
			}
			string text = button.Tag as string;
			PerkVO perkVO = Service.StaticDataController.Get<PerkVO>(text);
			Service.PerkViewController.ShowCancelPerkAlert(text, perkVO.PerkGroup);
		}

		public void OnViewClockTime(float dt)
		{
			this.UpdatePerkTimerLabelsOnTick();
			if (this.needsPerkStatesRefresh)
			{
				this.RefreshPerkStates();
			}
		}

		private void UpdatePerkTimerLabelsOnTick()
		{
			Lang lang = Service.Lang;
			int count = this.activePerkTimerLabels.Count;
			for (int i = 0; i < count; i++)
			{
				UXLabel uXLabel = this.activePerkTimerLabels[i];
				uint endTime = ((ActivatedPerkData)uXLabel.Tag).EndTime;
				this.UpdateLabelTimeRemaining(uXLabel, lang, "PERK_ACTIVE_SLOT_ACTIVE_TIMER", endTime);
			}
			count = this.cooldownTimerLabels.Count;
			for (int j = 0; j < count; j++)
			{
				UXLabel uXLabel2 = this.cooldownTimerLabels[j];
				uint endTime2 = (uint)uXLabel2.Tag;
				this.UpdateLabelTimeRemaining(uXLabel2, lang, "PERK_ACTIVE_CARD_COOLDOWN_TIMER", endTime2);
			}
			count = this.cooldownCostLabels.Count;
			for (int k = 0; k < count; k++)
			{
				UXLabel uXLabel3 = this.cooldownCostLabels[k];
				string perkId = (string)uXLabel3.Tag;
				this.UpdateGridItemCooldownSkipCost(uXLabel3, perkId);
			}
		}

		private void UpdateLabelTimeRemaining(UXLabel label, Lang lang, string locKey, uint endTime)
		{
			if (ServerTime.Time > endTime)
			{
				this.needsPerkStatesRefresh = true;
			}
			int num = (int)(endTime - ServerTime.Time);
			string text = LangUtils.FormatTime((long)num);
			label.Text = lang.Get(locKey, new object[]
			{
				text
			});
		}
	}
}
