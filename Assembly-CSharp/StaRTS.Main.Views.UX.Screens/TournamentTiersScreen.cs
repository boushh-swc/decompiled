using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Tournament;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class TournamentTiersScreen : ClosableScreen, IViewClockTimeObserver, IEventObserver
	{
		private TournamentVO currentTournamentVO;

		private List<TournamentTierVO> tiers;

		private TournamentRank currentPlayerRank;

		private UXGrid tierGrid;

		private UXLabel countdownLabel;

		private const string GRID = "LeagueGrid";

		private const string GRID_TEMPLATE = "LeagueTemplate";

		private const string REWARD_GRID = "GridRewardCards";

		private const string REWARD_GRID_TEMPLATE = "EquipmentItemCard";

		private const string GROUP_CURRENT_TIER = "CurrentLeague";

		private const string LABEL_CURRENT_TIER = "LabelCurrentLeague";

		private const string LABEL_REQUIREMENTS = "LabelRequirements";

		private const string LABEL_TIER_LEVEL = "LabelLeagueLevel";

		private const string ICON_TIER = "SpriteLeagueIcon";

		private const string LABEL_CURRENT_REWARDS = "LabelCurrentRewards";

		private const string LABEL_REWARDS = "LabelRewards";

		private const string LABEL_CRATE_NUMBER = "LabelPrizeNumber";

		private const string LABEL_GUARANTEED_NUMBER = "LabelPrizeCount";

		private const string LABEL_PRIZE = "LabelPrize";

		private const string SPRITE_PRIZE = "SpriteSupplyPrize";

		private const string BUTTON_PRIZE = "BtnConflictPrize";

		private const string BUTTON_GUARANTEED = "EquipmentItemCard";

		private const string SPRITE_GUARANTEED = "SpriteEquipmentItemImage";

		private const string SPRITE_GUARANTEED_FRAG_ICON = "SpriteFragmentIcon";

		private const string ELEMENT_GUARANTEED_Q_FORMAT = "EquipmentItemCardQ{0}";

		private const string ELEMENT_GUARANTEED_BKG_FORMAT = "SpriteEquipmentImageBkgGridQ{0}";

		private const string SPRITE_REWARD2D = "SpriteReward2D";

		private const string PARTICLES_TOP_PRIZE = "ParticlesTopPrize";

		private const string SPRITE_EQUIPMENT_GRADIENT = "SpriteEquipmentGradient";

		private const string SPRITE_EQUIPMENT_GRADIENT_BOTTOM = "SpriteEquipmentGradientBottom";

		private const string SPRITE_EQUIPMENT_IMAGE_BKG = "SpriteEquipmentImageBkg";

		private const string SPRITE_FRAME_HOVER = "SpriteFrameHover";

		private const string SPRITE_PRIZE_NUMBER_SHADOW = "SpritePrizeNumberShadow";

		private const string PANEL_HEADER = "PanelHeader";

		private const string PANEL_FOOTER = "PanelFooter";

		private const string PANEL_SCROLL_UP = "PanelLeaguePanelScrollUp";

		private const string PANEL_SCROLL_DOWN = "PanelLeaguePanelScrollDown";

		private const string LABEL_TITLE = "LabelHeader";

		private const string LABEL_COUNTDOWN = "LabelTimer";

		private const string SHOW_ANIM_TRIGGER = "Show";

		private const string HIDE_ANIM_TRIGGER = "Hide";

		private const float ANIM_DELAY = 0.2f;

		private const string CONFLICT_TIER_REWARDS_CURRENT_TITLE = "CONFLICT_TIER_REWARDS_CURRENT_TITLE";

		private const string CONFLICT_TIER_REWARDS_TITLE = "CONFLICT_TIER_REWARDS_TITLE";

		private const string CONFLICT_PRIZE_GUARANTEED = "CONFLICT_PRIZE_GUARANTEED";

		private const string CONFLICT_TIER_REQ_ANY = "CONFLICT_TIER_REQ_ANY";

		private const string CONFLICT_TIERS_WITH_NAME = "CONFLICT_TIERS_WITH_NAME";

		private const string CONFLICT_CURRENT_PERCENTILE = "CONFLICT_CURRENT_PERCENTILE";

		private const string CONFLICT_FINAL_PERCENTILE = "CONFLICT_FINAL_PERCENTILE";

		private const string CONFLICT_TIER_REQ_PERCENTAGE = "CONFLICT_TIER_REQ_PERCENTAGE";

		private const string CONFLICT_PRIZE_CRATE = "CONFLICT_PRIZE_CRATE";

		private const string CONFLICT_PRIZE_CRATE_MULTIPLIER = "CONFLICT_PRIZE_CRATE_MULTIPLIER";

		private const int REWARD_DISPLAY_COUNT_MAX = 5;

		private uint scrollCallbackTimerId;

		private ScreenParticleHandler particleHandler;

		protected override bool WantTransitions
		{
			get
			{
				return false;
			}
		}

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public TournamentTiersScreen(TournamentVO tournamentVO, Tournament tournament) : base("gui_conflict_leagues")
		{
			this.particleHandler = new ScreenParticleHandler(this);
			Dictionary<string, TournamentTierVO>.ValueCollection all = Service.StaticDataController.GetAll<TournamentTierVO>();
			this.tiers = new List<TournamentTierVO>();
			foreach (TournamentTierVO current in all)
			{
				this.tiers.Add(current);
			}
			this.tiers.Sort(new Comparison<TournamentTierVO>(this.CompareTiers));
			this.currentTournamentVO = tournamentVO;
			this.currentPlayerRank = ((tournament == null) ? null : tournament.CurrentRank);
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.ScreenLoaded, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ScreenClosing, EventPriority.Default);
		}

		private int CompareTiers(TournamentTierVO a, TournamentTierVO b)
		{
			return a.Order.CompareTo(b.Order);
		}

		private void HideParentPlayScreenCloseButton()
		{
			PlanetDetailsScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<PlanetDetailsScreen>();
			if (highestLevelScreen != null)
			{
				highestLevelScreen.CloseButton.Visible = false;
			}
		}

		protected override void OnScreenLoaded()
		{
			this.InitButtons();
			this.InitLabels();
			this.HideParentPlayScreenCloseButton();
			this.Update();
			this.tierGrid = base.GetElement<UXGrid>("LeagueGrid");
			this.tierGrid.SetTemplateItem("LeagueTemplate");
			TournamentTierVO tournamentTierVO = null;
			if (this.currentPlayerRank != null && this.currentPlayerRank.TierUid != null)
			{
				tournamentTierVO = Service.StaticDataController.Get<TournamentTierVO>(this.currentPlayerRank.TierUid);
			}
			Dictionary<string, TournamentRewardsVO> tierRewardMap = TimedEventPrizeUtils.GetTierRewardMap(this.currentTournamentVO.RewardGroupId);
			for (int i = 0; i < this.tiers.Count; i++)
			{
				this.AddTier(this.tiers[i], tournamentTierVO == this.tiers[i], i, tierRewardMap);
			}
			this.tierGrid.RepositionItemsFrameDelayed(new UXDragDelegate(this.ScrollCallback));
			Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
		}

		public void ScrollCallback(AbstractUXList list)
		{
			this.scrollCallbackTimerId = Service.ViewTimerManager.CreateViewTimer(0.1f, false, new TimerDelegate(this.OnTimerCallback), null);
		}

		private void OnTimerCallback(uint id, object cookie)
		{
			if (!base.IsLoaded())
			{
				return;
			}
			base.Root.GetComponent<Animator>().SetTrigger("Show");
		}

		private void OnParticleShowDelay(uint id, object cookie)
		{
			this.particleHandler.ShowAllParticleElements();
		}

		private void InitLabels()
		{
			string tournamentTitle = LangUtils.GetTournamentTitle(this.currentTournamentVO);
			base.GetElement<UXLabel>("LabelHeader").Text = this.lang.Get("CONFLICT_TIERS_WITH_NAME", new object[]
			{
				tournamentTitle
			});
			base.GetElement<UXLabel>("LabelRewards").Text = this.lang.Get("CONFLICT_TIER_REWARDS_TITLE", new object[0]);
			base.GetElement<UXLabel>("LabelCurrentRewards").Text = this.lang.Get("CONFLICT_TIER_REWARDS_CURRENT_TITLE", new object[0]);
			this.countdownLabel = base.GetElement<UXLabel>("LabelTimer");
		}

		private void AddTier(TournamentTierVO tierVO, bool isCurrent, int order, Dictionary<string, TournamentRewardsVO> tierRewardMap)
		{
			if (!tierRewardMap.ContainsKey(tierVO.Uid))
			{
				Service.Logger.ErrorFormat("There is no reward found for tier {0}", new object[]
				{
					tierVO.Uid
				});
				return;
			}
			string uid = tierVO.Uid;
			UXElement item = this.tierGrid.CloneTemplateItem(uid);
			UXElement subElement = this.tierGrid.GetSubElement<UXElement>(uid, "CurrentLeague");
			subElement.Visible = isCurrent;
			if (isCurrent && this.currentPlayerRank != null)
			{
				bool flag = TimedEventUtils.GetState(this.currentTournamentVO) == TimedEventState.Live;
				string id = (!flag) ? "CONFLICT_FINAL_PERCENTILE" : "CONFLICT_CURRENT_PERCENTILE";
				UXLabel subElement2 = this.tierGrid.GetSubElement<UXLabel>(uid, "LabelCurrentLeague");
				subElement2.Text = this.lang.Get(id, new object[]
				{
					Math.Round(this.currentPlayerRank.Percentile, 2)
				});
			}
			float percentage = tierVO.Percentage;
			UXLabel subElement3 = this.tierGrid.GetSubElement<UXLabel>(uid, "LabelRequirements");
			if (percentage < 100f)
			{
				subElement3.Text = this.lang.Get("CONFLICT_TIER_REQ_PERCENTAGE", new object[]
				{
					percentage
				});
			}
			else
			{
				subElement3.Text = this.lang.Get("CONFLICT_TIER_REQ_ANY", new object[0]);
			}
			UXLabel subElement4 = this.tierGrid.GetSubElement<UXLabel>(uid, "LabelLeagueLevel");
			subElement4.Text = this.lang.Get(tierVO.RankName, new object[0]);
			if (tierVO.Division != null)
			{
				string text = this.lang.Get(tierVO.Division, new object[0]);
				if (!string.IsNullOrEmpty(text) && text.Trim().Length != 0)
				{
					UXLabel expr_1BF = subElement4;
					expr_1BF.Text = expr_1BF.Text + " - " + text;
				}
			}
			UXSprite subElement5 = this.tierGrid.GetSubElement<UXSprite>(uid, "SpriteLeagueIcon");
			subElement5.SpriteName = Service.TournamentController.GetTierIconName(tierVO);
			TournamentRewardsVO tournamentRewardsVO = tierRewardMap[tierVO.Uid];
			UXGrid subElement6 = this.tierGrid.GetSubElement<UXGrid>(uid, "GridRewardCards");
			StringBuilder stringBuilder = new StringBuilder(" (");
			stringBuilder.Append(uid);
			stringBuilder.Append(")");
			string text2 = stringBuilder.ToString();
			subElement6.SetTemplateItem("EquipmentItemCard" + text2);
			CrateVO optional = Service.StaticDataController.GetOptional<CrateVO>(tournamentRewardsVO.CrateRewardIds[0]);
			if (optional != null)
			{
				StaticDataController staticDataController = Service.StaticDataController;
				List<CrateFlyoutItemVO> list = new List<CrateFlyoutItemVO>();
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				string[] array = (currentPlayer.Faction != FactionType.Empire) ? optional.FlyoutRebelItems : optional.FlyoutEmpireItems;
				if (array != null)
				{
					int i = 0;
					int num = array.Length;
					while (i < num)
					{
						string text3 = array[i];
						CrateFlyoutItemVO optional2 = staticDataController.GetOptional<CrateFlyoutItemVO>(text3);
						if (optional2 == null)
						{
							Service.Logger.ErrorFormat("CrateInfoModalScreen: FlyoutItemVO Uid {0} not found", new object[]
							{
								text3
							});
						}
						else if (UXUtils.ShouldDisplayCrateFlyoutItem(optional2, CrateFlyoutDisplayType.TournamentTier))
						{
							PlanetVO optional3 = staticDataController.GetOptional<PlanetVO>(this.currentTournamentVO.PlanetId);
							int currentHqLevel = currentPlayer.Map.FindHighestHqLevel();
							if (UXUtils.IsValidRewardItem(optional2, optional3, currentHqLevel))
							{
								bool flag2 = optional2.ReqArmory && !ArmoryUtils.PlayerHasArmory();
								if (!flag2)
								{
									if (list.Count <= 5)
									{
										list.Add(optional2);
										string uid2 = optional2.Uid;
										UXElement uXElement = subElement6.CloneTemplateItem(uid2);
										this.SetupCrateReward(uid2, tournamentRewardsVO, subElement6, uXElement, text2, optional2);
										subElement6.AddItem(uXElement, i);
									}
								}
							}
						}
						i++;
					}
				}
				else
				{
					Service.Logger.ErrorFormat("There is no crate data for {0}", new object[]
					{
						tournamentRewardsVO.CrateRewardIds[0]
					});
				}
				UXSprite subElement7 = subElement6.GetSubElement<UXSprite>(uid, "SpriteSupplyPrize");
				RewardUtils.SetCrateIcon(subElement7, optional, AnimState.Closed);
				UXButton subElement8 = subElement6.GetSubElement<UXButton>(uid, "BtnConflictPrize");
				subElement8.OnClicked = new UXButtonClickedDelegate(this.OnCrateClicked);
				subElement8.Tag = optional.Uid;
				UXLabel subElement9 = subElement6.GetSubElement<UXLabel>(uid, "LabelPrize");
				subElement9.Text = this.lang.Get("CONFLICT_PRIZE_CRATE", new object[]
				{
					LangUtils.GetCrateDisplayName(optional)
				});
				UXLabel subElement10 = subElement6.GetSubElement<UXLabel>(uid, "LabelPrizeNumber");
				if (tournamentRewardsVO.CrateRewardIds.Length > 1)
				{
					subElement10.Text = this.lang.Get("CONFLICT_PRIZE_CRATE_MULTIPLIER", new object[]
					{
						tournamentRewardsVO.CrateRewardIds.Length
					});
				}
				else
				{
					subElement10.Visible = false;
					subElement6.GetSubElement<UXElement>(uid, "SpritePrizeNumberShadow").Visible = false;
				}
			}
			this.tierGrid.AddItem(item, order);
		}

		private void SetupCrateReward(string itemUid, TournamentRewardsVO rewardGroup, UXGrid rewardGrid, UXElement rewardItem, string rewardSuffix, CrateFlyoutItemVO crateFlyoutItemVO)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			UXButton uXButton = rewardItem as UXButton;
			UXSprite subElement = rewardGrid.GetSubElement<UXSprite>(itemUid, "SpriteFragmentIcon" + rewardSuffix);
			UXUtils.HideGridQualityCards(rewardGrid, itemUid, "EquipmentItemCardQ{0}" + rewardSuffix);
			UXElement subElement2 = rewardGrid.GetSubElement<UXElement>(itemUid, "ParticlesTopPrize" + rewardSuffix);
			subElement2.Visible = false;
			uXButton.Enabled = false;
			subElement.Visible = false;
			CrateSupplyVO optional = staticDataController.GetOptional<CrateSupplyVO>(crateFlyoutItemVO.CrateSupplyUid);
			if (optional == null)
			{
				Service.Logger.ErrorFormat("Could not find crate supply {0} for faction {1}", new object[]
				{
					crateFlyoutItemVO.CrateSupplyUid,
					currentPlayer.Faction
				});
				return;
			}
			bool flag = crateFlyoutItemVO.TournamentTierDisplay3D && optional.Type != SupplyType.Currency && optional.Type != SupplyType.Invalid;
			int num = currentPlayer.Map.FindHighestHqLevel();
			UXSprite subElement3 = rewardGrid.GetSubElement<UXSprite>(itemUid, "SpriteReward2D" + rewardSuffix);
			subElement3.Visible = false;
			if (flag)
			{
				IGeometryVO iconVOFromCrateSupply = GameUtils.GetIconVOFromCrateSupply(optional, num);
				if (iconVOFromCrateSupply != null)
				{
					UXSprite subElement4 = rewardGrid.GetSubElement<UXSprite>(itemUid, "SpriteEquipmentItemImage" + rewardSuffix);
					ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(iconVOFromCrateSupply, subElement4, true);
					projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
					ProjectorUtils.GenerateProjector(projectorConfig);
					if (rewardGroup.TournamentTier == this.tiers[0].Uid)
					{
						string name = subElement2.Root.name;
						ScreenParticleFXCookie cookie = new ScreenParticleFXCookie(1f, name);
						this.particleHandler.ScheduleParticleFX(cookie);
					}
				}
				else
				{
					Service.Logger.ErrorFormat("Could not generate geometry for crate supply {0}", new object[]
					{
						optional.Uid
					});
				}
				uXButton.Tag = optional;
				uXButton.OnClicked = new UXButtonClickedDelegate(this.OnGuaranteedRewardClicked);
				uXButton.Enabled = true;
			}
			else
			{
				subElement3.Visible = true;
				int num2 = crateFlyoutItemVO.ListIcons.Length - 1;
				subElement3.SpriteName = crateFlyoutItemVO.ListIcons[num2];
				rewardGrid.GetSubElement<UXElement>(itemUid, "SpriteEquipmentGradient" + rewardSuffix).Visible = false;
				rewardGrid.GetSubElement<UXElement>(itemUid, "SpriteEquipmentGradientBottom" + rewardSuffix).Visible = false;
				rewardGrid.GetSubElement<UXElement>(itemUid, "SpriteEquipmentImageBkg" + rewardSuffix).Visible = false;
				rewardGrid.GetSubElement<UXElement>(itemUid, "SpriteFrameHover" + rewardSuffix).Visible = false;
			}
			UXLabel subElement5 = rewardGrid.GetSubElement<UXLabel>(itemUid, "LabelPrizeCount" + rewardSuffix);
			int rewardAmount = Service.InventoryCrateRewardController.GetRewardAmount(optional, num);
			int num3 = rewardGroup.CrateRewardIds.Length;
			string text = this.lang.ThousandsSeparated(rewardAmount * num3);
			string quantityString = crateFlyoutItemVO.QuantityString;
			if (!string.IsNullOrEmpty(quantityString))
			{
				subElement5.Text = this.lang.Get(quantityString, new object[]
				{
					text
				});
			}
			else if (rewardAmount > 0)
			{
				subElement5.Text = this.lang.Get("CONFLICT_PRIZE_CRATE_MULTIPLIER", new object[]
				{
					text
				});
			}
			else
			{
				subElement5.Visible = false;
			}
			int num4 = -1;
			bool flag2 = false;
			if (optional.Type == SupplyType.ShardSpecialAttack || optional.Type == SupplyType.ShardTroop)
			{
				ShardVO optional2 = staticDataController.GetOptional<ShardVO>(optional.RewardUid);
				if (optional2 != null)
				{
					num4 = (int)optional2.Quality;
				}
			}
			else if (optional.Type == SupplyType.Shard)
			{
				EquipmentVO currentEquipmentDataByID = ArmoryUtils.GetCurrentEquipmentDataByID(optional.RewardUid);
				if (currentEquipmentDataByID != null)
				{
					num4 = (int)currentEquipmentDataByID.Quality;
					flag2 = true;
				}
			}
			else if (optional.Type == SupplyType.Troop || optional.Type == SupplyType.Hero || optional.Type == SupplyType.SpecialAttack)
			{
				num4 = Service.DeployableShardUnlockController.GetUpgradeQualityForDeployableUID(optional.RewardUid);
			}
			if (flag)
			{
				string[] listIcons = crateFlyoutItemVO.ListIcons;
				if (listIcons != null && listIcons.Length > 0)
				{
					subElement.Visible = true;
					subElement.SpriteName = listIcons[listIcons.Length - 1];
				}
				if (num4 > 0)
				{
					string str = string.Format("SpriteEquipmentImageBkgGridQ{0}", num4);
					if (flag2)
					{
						rewardGrid.GetSubElement<UXElement>(itemUid, str + rewardSuffix).Visible = true;
					}
					UXUtils.SetCardQuality(this, rewardGrid, itemUid, num4, "EquipmentItemCardQ{0}" + rewardSuffix);
				}
			}
		}

		public void OnCrateClicked(UXButton btn)
		{
			string crateUid = btn.Tag as string;
			CrateInfoModalScreen crateInfoModalScreen = CrateInfoModalScreen.CreateForInfo(crateUid, this.currentTournamentVO.PlanetId);
			crateInfoModalScreen.IsAlwaysOnTop = true;
			Service.ScreenController.AddScreen(crateInfoModalScreen, true, false);
		}

		private void OnGuaranteedRewardClicked(UXButton btn)
		{
			CrateSupplyVO crateSupplyVO = (CrateSupplyVO)btn.Tag;
			if (crateSupplyVO != null)
			{
				GameUtils.HandleCrateSupplyRewardClicked(crateSupplyVO);
				this.HideHeaderFooter();
				Service.UXController.HUD.SetSquadScreenVisibility(false);
			}
		}

		private void HideHeaderFooter()
		{
			base.GetElement<UXElement>("PanelHeader").Visible = false;
			base.GetElement<UXElement>("PanelFooter").Visible = false;
			base.GetElement<UXElement>("PanelLeaguePanelScrollUp").Visible = false;
			base.GetElement<UXElement>("PanelLeaguePanelScrollDown").Visible = false;
		}

		private void ShowHeaderFooter()
		{
			base.GetElement<UXElement>("PanelHeader").Visible = true;
			base.GetElement<UXElement>("PanelFooter").Visible = true;
			base.GetElement<UXElement>("PanelLeaguePanelScrollUp").Visible = true;
			base.GetElement<UXElement>("PanelLeaguePanelScrollDown").Visible = true;
		}

		public void OnViewClockTime(float dt)
		{
			this.Update();
		}

		private void Update()
		{
			int num = 0;
			string text = null;
			TimedEventState state = TimedEventUtils.GetState(this.currentTournamentVO);
			if (state != TimedEventState.Upcoming)
			{
				if (state == TimedEventState.Live)
				{
					text = "CAMPAIGN_ENDS_IN";
					num = TimedEventUtils.GetSecondsRemaining(this.currentTournamentVO);
				}
			}
			else
			{
				text = "CAMPAIGN_BEGINS_IN";
				num = TimedEventUtils.GetSecondsRemaining(this.currentTournamentVO);
			}
			if (text != null)
			{
				this.countdownLabel.Text = this.lang.Get(text, new object[]
				{
					LangUtils.FormatTime((long)num)
				});
			}
			else if (this.countdownLabel.Visible)
			{
				this.countdownLabel.Visible = false;
			}
		}

		public override void OnDestroyElement()
		{
			if (this.scrollCallbackTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.scrollCallbackTimerId);
			}
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.ScreenLoaded);
			eventManager.UnregisterObserver(this, EventId.ScreenClosing);
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			if (this.tierGrid != null)
			{
				this.tierGrid.Clear();
				this.tierGrid = null;
			}
			base.OnDestroyElement();
		}

		public override void Close(object modalResult)
		{
			this.particleHandler.Destroy();
			base.Root.GetComponent<Animator>().SetTrigger("Hide");
			Service.ViewTimerManager.CreateViewTimer(0.2f, false, new TimerDelegate(this.OnCloseScreenCallback), modalResult);
		}

		public void OnCloseScreenCallback(uint id, object cookie)
		{
			PlanetDetailsScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<PlanetDetailsScreen>();
			if (highestLevelScreen != null)
			{
				highestLevelScreen.CloseButton.Visible = true;
				highestLevelScreen.AnimateShowUI();
			}
			base.Close(cookie);
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.ScreenLoaded)
			{
				if (id == EventId.ScreenClosing)
				{
					ScreenBase screenBase = (ScreenBase)cookie;
					if (screenBase is TournamentTiersScreen)
					{
						Service.UXController.HUD.SetSquadScreenVisibility(true);
					}
					if (screenBase is DeployableInfoScreen || screenBase is EquipmentInfoScreen)
					{
						this.ShowHeaderFooter();
					}
				}
			}
			else
			{
				ScreenBase screenBase = (ScreenBase)cookie;
				if (screenBase is TournamentTiersScreen)
				{
					Service.UXController.HUD.SetSquadScreenVisibility(false);
				}
			}
			return base.OnEvent(id, cookie);
		}
	}
}
