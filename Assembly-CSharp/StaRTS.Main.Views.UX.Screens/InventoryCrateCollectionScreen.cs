using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Crates;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class InventoryCrateCollectionScreen : ClosableScreen, IViewFrameTimeObserver
	{
		private const float REWARD_BAR_FILL_TIME_SEC = 0.75f;

		private const float PBAR_DELAY_TIME_SEC = 1f;

		public const string NEXT_REWARD_BUTTON = "NEXT_REWARD_BUTTON";

		public const string FINISHED_REWARD_BUTTON = "FINISHED_REWARD_BUTTON";

		private const string OBJECTIVE_PROGRESS = "OBJECTIVE_PROGRESS";

		private const string MAX_LEVEL = "MAX_LEVEL";

		private const string REWARD_COUNT = "CRATE_OPEN_SEQUENCE_REWARD_COUNT";

		private const string OPEN_CRATE = "CRATE_OPEN_SEQUENCE_CTA_OPEN";

		private const string NEXT_CRATE = "CRATE_OPEN_SEQUENCE_CTA_NEXT";

		private const string BUTTON_DONE = "BUTTON_DONE";

		private const string CRATE_REWARD_POPUP_PROGRESS_UPGRADE_SEQ1 = "CRATE_REWARD_POPUP_PROGRESS_UPGRADE_SEQ1";

		private const string CRATE_REWARD_POPUP_PROGRESS_UPGRADE_SEQ2 = "CRATE_REWARD_POPUP_PROGRESS_UPGRADE_SEQ2";

		private const string BTN_NEXT = "BtnPrimary";

		private const string BTN_FULLSCREEN = "BtnCloseFullScreen";

		private const string LABEL_CRATE_TITLE = "LabelCrateType";

		private const string LABEL_BUTTON_NEXT = "LabelBtnPrimary";

		private const string GRID_REWARD_ITEMS = "GridListItem";

		private const string TEMPLATE_REWARD_ITEM = "TemplateListItem";

		private const string SPRITE_REWARD_ITEM = "SpriteTypeListItem";

		private const string LABEL_REWARD_ITEM = "LabelTypeListItem";

		private const string PBAR_REWARD_TOP = "pBarCurrentItemTop";

		private const string SPRITE_TOP_PBAR_REWARD = "SpriteCurrentItempTopBar";

		private const string SPRITE_BOTTOM_PBAR_REWARD = "SpriteCurrentItempBottomBar";

		private const string PBAR_REWARD_BOTTOM = "pBarCurrentItemBottom";

		private const string LABEL_CURRENT_REWARD = "LabelCurrentItem";

		private const string LABEL_REWARD_COUNTER = "LabelRewardCounter";

		private const string LABEL_REWARD_AVAILABLE = "LabelUpgradeAvailable";

		private const string LABEL_REWARD_AVAILABLE_AT = "LabelUpgradeAvailableAlt";

		private const string LABEL_REWARD_PBAR_COUNTER = "LabelCounter";

		private const string UNIT_REWARD_SPRITE = "icoTroopSample";

		private const string TIER_REWARD_SPRITE_FORMAT = "icoDataFragQ{0}";

		private const string SHOW_REWARD = "ShowReward";

		private const string NEXT_REWARD = "NextReward";

		private const string HIDE_BUTTON = "HideButton";

		private const string SHOW_BUTTON = "ShowButton";

		private const string SHOW_REWARD_W_PBAR = "ShowPbar";

		private const string RESET_REWARD = "Reset";

		private const string SHOW_LIST_REWARD = "ShowListItem";

		private const string SHOW_BUTTON_STATE = "Show Button";

		private const string HIDE_BUTTON_STATE = "Hide Button";

		private UXButton btnNextReward;

		private UXButton btnFullScreen;

		private UXLabel lblCrateTitle;

		private UXLabel lblNextReward;

		private UXGrid gridRewardItems;

		private UXSlider pBarRewardTop;

		private UXSprite spriteRewardTopPBar;

		private UXSprite spriteRewardBottomPBar;

		private UXSlider pBarRewardBottom;

		private UXLabel lblCurrentReward;

		private UXLabel lblRewardCount;

		private UXLabel lblRewardAvailable;

		private UXLabel lblRewardAvailableAt;

		private UXLabel lblRewardPBarCounter;

		private InventoryCrateAnimation parent;

		private CrateData crateData;

		private uint pbarDelayTimerId;

		private bool isCrateOpen;

		private int maxRewardItemAmount;

		private int currentRewardItemAmount;

		private int newRewardItemAmount;

		private float interpTimer;

		private bool shardReward;

		private bool showUpgrade;

		private bool rewardCycleReady;

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
				return false;
			}
		}

		public InventoryCrateCollectionScreen(InventoryCrateAnimation parent, CrateData crateData) : base("gui_supply_crate_celebration")
		{
			base.IsAlwaysOnTop = true;
			this.crateData = crateData;
			this.parent = parent;
			this.pbarDelayTimerId = 0u;
		}

		protected override void OnScreenLoaded()
		{
			base.OnScreenLoaded();
			this.InitButtons();
			this.isCrateOpen = false;
			this.rewardCycleReady = true;
			this.CloseButton.Visible = false;
			this.shardReward = false;
			this.showUpgrade = false;
			this.btnNextReward = base.GetElement<UXButton>("BtnPrimary");
			this.btnNextReward.OnClicked = new UXButtonClickedDelegate(this.OnNextRewardClicked);
			this.btnFullScreen = base.GetElement<UXButton>("BtnCloseFullScreen");
			this.btnFullScreen.OnClicked = null;
			base.CurrentBackDelegate = new UXButtonClickedDelegate(this.OnNextRewardClicked);
			base.CurrentBackButton = this.btnNextReward;
			this.lblCrateTitle = base.GetElement<UXLabel>("LabelCrateType");
			this.lblCrateTitle.Text = LangUtils.GetCrateDisplayName(this.crateData.CrateId);
			this.lblRewardCount = base.GetElement<UXLabel>("LabelRewardCounter");
			this.lblRewardCount.Visible = false;
			this.lblNextReward = base.GetElement<UXLabel>("LabelBtnPrimary");
			this.lblNextReward.Text = this.lang.Get("CRATE_OPEN_SEQUENCE_CTA_OPEN", new object[0]);
			this.gridRewardItems = base.GetElement<UXGrid>("GridListItem");
			this.InitRewardGrid();
			this.pBarRewardTop = base.GetElement<UXSlider>("pBarCurrentItemTop");
			this.spriteRewardTopPBar = base.GetElement<UXSprite>("SpriteCurrentItempTopBar");
			this.spriteRewardBottomPBar = base.GetElement<UXSprite>("SpriteCurrentItempBottomBar");
			this.pBarRewardBottom = base.GetElement<UXSlider>("pBarCurrentItemBottom");
			this.lblCurrentReward = base.GetElement<UXLabel>("LabelCurrentItem");
			this.lblRewardAvailable = base.GetElement<UXLabel>("LabelUpgradeAvailable");
			this.lblRewardAvailable.Text = this.lang.Get("CRATE_REWARD_POPUP_PROGRESS_UPGRADE_SEQ1", new object[0]);
			this.lblRewardAvailableAt = base.GetElement<UXLabel>("LabelUpgradeAvailableAlt");
			this.lblRewardAvailableAt.Text = this.lang.Get("CRATE_REWARD_POPUP_PROGRESS_UPGRADE_SEQ2", new object[0]);
			this.lblRewardPBarCounter = base.GetElement<UXLabel>("LabelCounter");
			this.HideUpgradeLabels();
			base.InitAnimator();
		}

		private void HideUpgradeLabels()
		{
			this.lblRewardAvailable.Visible = false;
			this.lblRewardAvailableAt.Visible = false;
		}

		private void ShowUpgradeLabels()
		{
			if (this.showUpgrade)
			{
				this.lblRewardPBarCounter.Visible = false;
				this.lblRewardAvailable.Visible = true;
				this.lblRewardAvailableAt.Visible = true;
				Color textColor = this.lblRewardAvailable.TextColor;
				textColor.a = 1f;
				this.lblRewardAvailable.TextColor = textColor;
				textColor = this.lblRewardAvailableAt.TextColor;
				textColor.a = 0f;
				this.lblRewardAvailableAt.TextColor = textColor;
			}
		}

		private void InitRewardGrid()
		{
			this.gridRewardItems.SetTemplateItem("TemplateListItem");
			List<SupplyCrateTag> rewardList = this.parent.GetRewardList();
			int count = rewardList.Count;
			int crateHQLevel = this.parent.GetCrateHQLevel();
			for (int i = 0; i < count; i++)
			{
				SupplyCrateTag supplyCrateTag = rewardList[i];
				CrateSupplyVO crateSupply = supplyCrateTag.CrateSupply;
				string itemUid = crateSupply.Uid + i.ToString();
				UXElement uXElement = this.gridRewardItems.CloneTemplateItem(itemUid);
				this.gridRewardItems.AddItem(uXElement, i);
				UXSprite subElement = this.gridRewardItems.GetSubElement<UXSprite>(itemUid, "SpriteTypeListItem");
				subElement.SpriteName = this.GetRewardListSpriteName(supplyCrateTag);
				UXLabel subElement2 = this.gridRewardItems.GetSubElement<UXLabel>(itemUid, "LabelTypeListItem");
				subElement2.Text = GameUtils.GetRewardSupplyName(supplyCrateTag.CrateSupply, crateHQLevel);
				uXElement.InitAnimator();
			}
			this.gridRewardItems.RepositionItems();
		}

		private string GetRewardListSpriteName(SupplyCrateTag tag)
		{
			CrateSupplyVO crateSupply = tag.CrateSupply;
			string result = null;
			switch (crateSupply.Type)
			{
			case SupplyType.Currency:
				result = GameUtils.GetCurrencyIconName(crateSupply.RewardUid);
				break;
			case SupplyType.Shard:
			case SupplyType.ShardTroop:
			case SupplyType.ShardSpecialAttack:
			{
				ShardQuality shardQuailty = tag.ShardQuailty;
				result = string.Format("icoDataFragQ{0}", (int)shardQuailty);
				break;
			}
			case SupplyType.Troop:
			case SupplyType.Hero:
			case SupplyType.SpecialAttack:
				result = "icoTroopSample";
				break;
			default:
				Service.Logger.Error("Unsupported supply type in GetRewardListSpriteName " + crateSupply.Type.ToString());
				break;
			}
			return result;
		}

		private void SetupForNextReward()
		{
			if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Show Button"))
			{
				base.SetTrigger("HideButton");
			}
			else
			{
				base.SetTrigger("Reset");
			}
		}

		private void ShowListItem()
		{
			int i = this.parent.GetCurrentRewardIndex() - 1;
			UXElement item = this.gridRewardItems.GetItem(i);
			if (item == null)
			{
				return;
			}
			this.UpdateRewardCount();
			item.Visible = true;
			item.SetTrigger("ShowListItem");
		}

		private void ShowReward()
		{
			this.ShowListItem();
			if (this.animator.GetCurrentAnimatorStateInfo(0).IsName("Hide Button"))
			{
				base.SetTrigger("ShowReward");
			}
			else
			{
				base.SetTrigger("NextReward");
			}
			this.rewardCycleReady = true;
		}

		private void ShowRewardWithPBar()
		{
			this.lblRewardPBarCounter.Visible = true;
			this.lblRewardPBarCounter.Text = this.currentRewardItemAmount.ToString() + "/" + this.maxRewardItemAmount.ToString();
			this.ShowReward();
			base.SetTrigger("ShowPbar");
		}

		private void ResetAllTriggers()
		{
			base.ResetTrigger("ShowReward");
			base.ResetTrigger("NextReward");
			base.ResetTrigger("HideButton");
			base.ResetTrigger("ShowButton");
			base.ResetTrigger("ShowPbar");
			base.ResetTrigger("Reset");
		}

		public void ShowOpenButton()
		{
			base.SetTrigger("ShowButton");
		}

		private void OnFinalSkipClicked(UXButton btn)
		{
			if (!this.rewardCycleReady)
			{
				this.SkipToEndOfRewardAnim();
			}
		}

		private void OnNextRewardClicked(UXButton btnNextReward)
		{
			if (!this.parent.IsLoaded)
			{
				Service.Logger.WarnFormat("Crate animations {0} not ready to play", new object[]
				{
					this.crateData.CrateId
				});
				return;
			}
			if (!this.parent.AvailableToAnimate())
			{
				Service.Logger.ErrorFormat("Crate animations {0} can no longer be played", new object[]
				{
					this.crateData.CrateId
				});
				base.InitDefaultBackDelegate();
				this.HandleClose(null);
				return;
			}
			if (!this.isCrateOpen)
			{
				this.btnFullScreen.OnClicked = new UXButtonClickedDelegate(this.OnNextRewardClicked);
			}
			if (!this.rewardCycleReady)
			{
				this.SkipToEndOfRewardAnim();
				return;
			}
			this.rewardCycleReady = false;
			this.SetupForNextReward();
			this.parent.ShowNextReward();
		}

		private void SkipToEndOfRewardAnim()
		{
			this.parent.SkipToEndOfCrateRewardAnim();
		}

		private void UpdateRewardCount()
		{
			if (!this.isCrateOpen)
			{
				this.isCrateOpen = true;
				this.lblRewardCount.Visible = true;
				this.lblNextReward.Text = this.lang.Get("CRATE_OPEN_SEQUENCE_CTA_NEXT", new object[0]);
			}
			int totalRewardsCount = this.parent.GetTotalRewardsCount();
			int num = Math.Min(totalRewardsCount, this.parent.GetCurrentRewardIndex());
			this.lblRewardCount.Text = this.lang.Get("CRATE_OPEN_SEQUENCE_REWARD_COUNT", new object[]
			{
				num,
				totalRewardsCount
			});
		}

		private void SetupCurrentRewardTitle(SupplyCrateTag tag)
		{
			if (tag == null || tag.CrateSupply == null)
			{
				this.lblCurrentReward.Text = string.Empty;
				return;
			}
			int crateHQLevel = this.parent.GetCrateHQLevel();
			this.lblCurrentReward.Text = GameUtils.GetRewardSupplyName(tag.CrateSupply, crateHQLevel);
		}

		private void SetupBottomPBarValue()
		{
			if (this.maxRewardItemAmount == 0)
			{
				return;
			}
			this.pBarRewardBottom.Value = (float)this.newRewardItemAmount / (float)this.maxRewardItemAmount;
		}

		private float InterpRewardItemAmount(float alpha)
		{
			if (this.maxRewardItemAmount == 0)
			{
				return 0f;
			}
			float num = (float)this.currentRewardItemAmount / (float)this.maxRewardItemAmount;
			float num2 = (float)this.newRewardItemAmount / (float)this.maxRewardItemAmount;
			return num * (1f - alpha) + num2 * alpha;
		}

		private void SetupShardPBars(CrateSupplyVO crateSupply)
		{
			float value = this.InterpRewardItemAmount(0f);
			this.HandlePBarValueChange(value, false);
			this.SetupBottomPBarValue();
		}

		private void HandlePBarValueChange(float value, bool showCompleteLabels)
		{
			UXUtils.SetShardProgressBarValue(this.pBarRewardTop, this.spriteRewardTopPBar, value);
			Color color = this.spriteRewardTopPBar.Color;
			color.a *= 0.5f;
			this.spriteRewardBottomPBar.Color = color;
			if (value >= 1f && showCompleteLabels)
			{
				this.ShowUpgradeLabels();
			}
			else
			{
				this.HideUpgradeLabels();
			}
		}

		private void SetupPBarInterpTimerIfNeeded()
		{
			this.pbarDelayTimerId = Service.ViewTimerManager.CreateViewTimer(1f, false, new TimerDelegate(this.EnablePBarUpdate), null);
		}

		private void EnablePBarUpdate(uint timerId, object cookie)
		{
			this.pbarDelayTimerId = 0u;
			this.interpTimer = 0f;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		public void OnViewFrameTime(float dt)
		{
			if (base.IsAnimatorTransitioning())
			{
				return;
			}
			this.interpTimer += dt;
			float num = this.interpTimer / 0.75f;
			bool flag = num >= 1f;
			if (flag)
			{
				num = 1f;
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
			float value = this.InterpRewardItemAmount(num);
			int num2 = Mathf.CeilToInt((float)this.currentRewardItemAmount * (1f - num) + (float)this.newRewardItemAmount * num);
			this.lblRewardPBarCounter.Text = num2.ToString() + "/" + this.maxRewardItemAmount.ToString();
			if (this.shardReward)
			{
				this.HandlePBarValueChange(value, flag);
			}
			Service.EventManager.SendEvent(EventId.InventoryCrateAnimationStateChange, InventoryCrateAnimationState.ShowPBar);
		}

		public void ShowNameAndAmountUI(SupplyCrateTag tag)
		{
			this.showUpgrade = false;
			this.shardReward = false;
			this.ResetAllTriggers();
			this.SetupCurrentRewardTitle(tag);
			this.ShowReward();
		}

		public void ShowEquipmentPBarUI(SupplyCrateTag tag, int shardsFrom, int shardsTo, int shardsNeededForLevel, bool showLevel)
		{
			this.showUpgrade = showLevel;
			this.shardReward = true;
			this.SetupCurrentRewardTitle(tag);
			this.maxRewardItemAmount = shardsNeededForLevel;
			this.currentRewardItemAmount = shardsFrom;
			this.newRewardItemAmount = shardsTo;
			this.SetupShardPBars(tag.CrateSupply);
			this.SetupPBarInterpTimerIfNeeded();
			this.ShowRewardWithPBar();
		}

		public void ShowUnlockShardPBarUI(SupplyCrateTag tag, int shardsFrom, int shardsTo, int shardsNeededForLevel, bool showLevel)
		{
			this.showUpgrade = showLevel;
			this.shardReward = true;
			this.SetupCurrentRewardTitle(tag);
			this.maxRewardItemAmount = shardsNeededForLevel;
			this.currentRewardItemAmount = shardsFrom;
			this.newRewardItemAmount = shardsTo;
			this.SetupShardPBars(tag.CrateSupply);
			this.SetupPBarInterpTimerIfNeeded();
			this.ShowRewardWithPBar();
		}

		public void ChangePrimaryButtonToClose()
		{
			this.btnNextReward.OnClicked = new UXButtonClickedDelegate(this.OnCloseButtonClicked);
			this.btnFullScreen.OnClicked = new UXButtonClickedDelegate(this.OnFinalSkipClicked);
			base.InitDefaultBackDelegate();
			this.lblNextReward.Text = this.lang.Get("BUTTON_DONE", new object[0]);
		}

		public override void Close(object modalResult)
		{
			if (this.crateData.Context != "objec" && this.crateData.Context != "sqwar")
			{
				AwardCrateSuppliesRequest request = new AwardCrateSuppliesRequest(this.crateData.UId);
				AwardCrateSuppliesCommand command = new AwardCrateSuppliesCommand(request);
				Service.ServerAPI.Sync(command);
			}
			if (this.pbarDelayTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.pbarDelayTimerId);
			}
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			this.gridRewardItems.Clear();
			this.parent.CleanUp();
			base.Close(modalResult);
			Service.EventManager.SendEvent(EventId.InventoryCrateCollectionClosed, false);
		}
	}
}
