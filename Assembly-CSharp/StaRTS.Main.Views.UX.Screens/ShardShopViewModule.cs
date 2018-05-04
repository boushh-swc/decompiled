using StaRTS.Main.Controllers.ShardShop;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class ShardShopViewModule : IEventObserver, IViewFrameTimeObserver
	{
		private const float POST_BLOCK_ANIM_DELAY = 0.4f;

		private const float PBAR_BAR_FILL_TIME_SEC = 0.75f;

		private const string SHARD_SHOP_SHARDS_REMAINING = "shard_shop_shards_remaining";

		private const string SHARD_SHOP_BUY_EQUIPMENT_SHARDS = "shard_shop_buy_equipment_shards";

		private const string SHARD_SHOP_BUY_UNIT_SHARDS = "shard_shop_buy_unit_shards";

		private const string SHARD_SHOP_BUY_SINGLE = "shard_shop_buy_single";

		private const string SHARD_SHOP_BUY_ALL = "shard_shop_buy_all";

		private const string SHARD_PROGRESS_UPGRADE_SEQ1 = "CRATE_REWARD_POPUP_PROGRESS_UPGRADE_SEQ1";

		private const string SHARD_PROGRESS_UPGRADE_SEQ2 = "CRATE_REWARD_POPUP_PROGRESS_UPGRADE_SEQ2";

		private const string SHARD_SHOP_CRYSTAL_WARNING_TITLE = "shard_shop_crystal_warning_title";

		private const string SHARD_SHOP_CRYSTAL_WARNING_BODY = "shard_shop_crystal_warning_body";

		private const string MODAL_PURCHASE = "ModalPurchase";

		private const string WIDGET_FRAG_PLACEMENT = "WidgetFragPlaceholder";

		private const string ITEM_FRAGMENT_TEMPLATE = "FragmentItemTemplate";

		private const string MODAL_CLOSE_BUTTON = "BtnModalClose";

		private const string PURCHASE_SINGLE_BUTTON = "BtnPurchase1";

		private const string PURCHASE_ALL_BUTTON = "BtnPurchaseAll";

		private const string RETURN_BUTTON = "BtnReturn";

		private const string QUANTITY_COUNTER_LABEL = "LabelQuantityCounter";

		private const string QUANTITY_FLOATER_COUNTER_LABEL = "LabelQuantityCounterFloater";

		private const string BODY_TEXT_LABEL = "LabelBodyText";

		private const string TITLE_TEXT_LABEL = "LabelTitle";

		private const string PURCHASE_SINGLE_COST_LABEL = "LabelPurchase1";

		private const string PURCHASE_All_COST_LABEL = "LabelPurchaseAll";

		private const string PURCHASE_SINGLE_DESC_LABEL = "CostOptionPay1LabelDescription";

		private const string PURCHASE_ALL_DESC_LABEL = "CostOptionPayAllLabelDescription";

		private const string PURCHASE_SINGLE_SPRITE = "SpriteCurrencyPurchase1";

		private const string PURCHASE_ALL_SPRITE = "SpritePurchaseAllIcon";

		private const string ICON_FRAG_PROGRESS_LABEL = "LabelFragProgress";

		private const string ICON_FRAG_NAME = "LabelFragmentName";

		private const string ICON_FRAG_REQUIREMENT = "LabelFragmentRequirement";

		private const string ICON_PROGRESS_MESSAGE_1_LABEL = "LabelFragProgressMessage1";

		private const string ICON_PROGRESS_MESSAGE_2_LABEL = "LabelFragProgressMessage2";

		private const string ICON_FRAG_COUNT = "LabelMesageCountFragmentItems";

		private const string ICON_FRAG_COST = "LabelFragmentCost";

		private const string ICON_BUTTON = "ButtonFragmentCard";

		private const string ICON_SPRITE = "SpriteFragmentImage";

		private const string ICON_BASE_SHARD_PROGRESS_SLIDER = "pBarFragmentCount";

		private const string ICON_DELTA_SHARD_PROGRESS_SLIDER = "pBarFragmentCountDelta";

		private const string ICON_WIDGET_NAME = "ModalIcon";

		private const string ICON_MODAL_STATE = "InModal";

		private const string ICON_QUALITY_INT = "Quality";

		private const string ICON_SHOW = "Visible";

		private const string PBAR_UPGRADEABLE = "Upgradeable";

		private const string TEMP_ITEM_KEY = "Temp";

		private const string QUANTITY_FLOATER_TRIGGER = "BuyFragment";

		private const string CARD_FLOURISH_TRIGGER = "CardFlourish";

		private const string EQUIPMENT_SHARD_TRIGGER = "Equipment";

		private static bool warned;

		private UXElement modal;

		private UXElement iconHolder;

		private UXElement icon;

		private UXLabel quantityCounter;

		private UXLabel quantityFloatingCounter;

		private UXLabel bodyText;

		private UXLabel titleText;

		private UXLabel purchaseSingleDescLabel;

		private UXLabel purchaseAllDescLabel;

		private UXLabel purchaseSingleLabel;

		private UXLabel purchaseAllLabel;

		private UXSprite purchaseSingleSprite;

		private UXSprite purchaseAllSprite;

		private UXButton closeModalBtn;

		private UXButton purchaseSingleBtn;

		private UXButton purchaseAllBtn;

		private UXButton returnBtn;

		private UXButton iconButtonCard;

		private UXLabel iconProgressLabel;

		private UXLabel iconProgressMessage1;

		private UXLabel iconProgressMessage2;

		private UXLabel iconRequirement;

		private UXSprite iconSprite;

		private UXSlider iconBaseShardProgressBar;

		private UXSlider iconDeltaShardProgressBar;

		private int previousShardsEarned;

		private bool isProgressBarLerping;

		private float nextProgressBarValue;

		private float previousProgressBarValue;

		private float progressLerpTimer;

		private StoreScreen parent;

		private ShardShopViewTO vto;

		private Animator iconAnimator;

		private Animator modalAnimator;

		private Lang lang;

		private GeometryProjector projector;

		private bool initialized;

		private uint animDelayTimerId;

		private ShardShopData predictedData;

		public ShardShopViewModule(StoreScreen parent)
		{
			this.parent = parent;
			this.lang = Service.Lang;
			this.modal = parent.GetElement<UXElement>("ModalPurchase");
			this.modalAnimator = this.modal.Root.GetComponent<Animator>();
			this.animDelayTimerId = 0u;
			this.quantityCounter = parent.GetElement<UXLabel>("LabelQuantityCounter");
			this.quantityFloatingCounter = parent.GetElement<UXLabel>("LabelQuantityCounterFloater");
			this.iconHolder = parent.GetElement<UXElement>("WidgetFragPlaceholder");
			this.bodyText = parent.GetElement<UXLabel>("LabelBodyText");
			this.titleText = parent.GetElement<UXLabel>("LabelTitle");
			this.purchaseSingleLabel = parent.GetElement<UXLabel>("LabelPurchase1");
			this.purchaseAllLabel = parent.GetElement<UXLabel>("LabelPurchaseAll");
			this.purchaseSingleDescLabel = parent.GetElement<UXLabel>("CostOptionPay1LabelDescription");
			this.purchaseAllDescLabel = parent.GetElement<UXLabel>("CostOptionPayAllLabelDescription");
			this.purchaseSingleSprite = parent.GetElement<UXSprite>("SpriteCurrencyPurchase1");
			this.purchaseAllSprite = parent.GetElement<UXSprite>("SpritePurchaseAllIcon");
			this.closeModalBtn = parent.GetElement<UXButton>("BtnModalClose");
			this.purchaseSingleBtn = parent.GetElement<UXButton>("BtnPurchase1");
			this.purchaseAllBtn = parent.GetElement<UXButton>("BtnPurchaseAll");
			this.returnBtn = parent.GetElement<UXButton>("BtnReturn");
			this.closeModalBtn.OnClicked = new UXButtonClickedDelegate(this.OnCloseModalClicked);
			this.purchaseSingleBtn.OnClicked = new UXButtonClickedDelegate(this.OnPurchaseShardsClicked);
			this.purchaseAllBtn.OnClicked = new UXButtonClickedDelegate(this.OnPurchaseShardsClicked);
			this.returnBtn.OnClicked = new UXButtonClickedDelegate(this.OnCloseModalClicked);
			UXElement element = parent.GetElement<UXElement>("FragmentItemTemplate");
			this.icon = parent.CloneElement<UXElement>(element, "ModalIcon", this.iconHolder.Root);
			this.iconProgressLabel = parent.GetElement<UXLabel>(UXUtils.FormatAppendedName("LabelFragProgress", "ModalIcon"));
			this.iconProgressMessage1 = parent.GetElement<UXLabel>(UXUtils.FormatAppendedName("LabelFragProgressMessage1", "ModalIcon"));
			this.iconProgressMessage1.Text = this.lang.Get("CRATE_REWARD_POPUP_PROGRESS_UPGRADE_SEQ1", new object[0]);
			this.iconProgressMessage2 = parent.GetElement<UXLabel>(UXUtils.FormatAppendedName("LabelFragProgressMessage2", "ModalIcon"));
			this.iconProgressMessage2.Text = this.lang.Get("CRATE_REWARD_POPUP_PROGRESS_UPGRADE_SEQ2", new object[0]);
			this.iconRequirement = parent.GetElement<UXLabel>(UXUtils.FormatAppendedName("LabelFragmentRequirement", "ModalIcon"));
			this.iconSprite = parent.GetElement<UXSprite>(UXUtils.FormatAppendedName("SpriteFragmentImage", "ModalIcon"));
			this.iconButtonCard = parent.GetElement<UXButton>(UXUtils.FormatAppendedName("ButtonFragmentCard", "ModalIcon"));
			this.iconBaseShardProgressBar = parent.GetElement<UXSlider>(UXUtils.FormatAppendedName("pBarFragmentCount", "ModalIcon"));
			this.iconDeltaShardProgressBar = parent.GetElement<UXSlider>(UXUtils.FormatAppendedName("pBarFragmentCountDelta", "ModalIcon"));
			this.iconAnimator = this.iconButtonCard.Root.GetComponent<Animator>();
			this.icon.Visible = false;
			Service.ShardShopController.ResetClientPredictionId();
			this.predictedData = Service.ShardShopController.CurrentShopData.Copy();
		}

		public void Render(ShardShopViewTO vto, bool shouldShowProgressLerp)
		{
			bool flag = this.vto == null || this.vto.SupplyVO != vto.SupplyVO;
			bool flag2 = this.vto != null && !this.vto.ValueEquals(vto);
			if (flag2)
			{
				this.previousShardsEarned = this.vto.UpgradeShardsEarned;
			}
			this.vto = vto;
			if (flag)
			{
				IGeometryVO iconVOFromCrateSupply = GameUtils.GetIconVOFromCrateSupply(vto.SupplyVO, vto.PlayerHQLevel);
				ProjectorConfig config = ProjectorUtils.GenerateGeometryConfig(iconVOFromCrateSupply, this.iconSprite);
				this.projector = ProjectorUtils.GenerateProjector(config);
			}
			int upgradeShardsEarned = vto.UpgradeShardsEarned;
			int upgradeShardsRequired = vto.UpgradeShardsRequired;
			this.quantityCounter.Text = vto.RemainingShardsForSale.ToString();
			this.bodyText.Text = this.lang.Get("shard_shop_shards_remaining", new object[]
			{
				vto.ItemName
			});
			this.purchaseSingleDescLabel.Text = this.lang.Get("shard_shop_buy_single", new object[0]);
			this.purchaseAllDescLabel.Text = this.lang.Get("shard_shop_buy_all", new object[0]);
			this.nextProgressBarValue = this.GetProgessBarValue(upgradeShardsEarned, upgradeShardsRequired);
			this.previousProgressBarValue = this.GetProgessBarValue(this.previousShardsEarned, upgradeShardsRequired);
			this.iconDeltaShardProgressBar.Value = this.nextProgressBarValue;
			if (!shouldShowProgressLerp)
			{
				this.isProgressBarLerping = false;
				this.iconProgressLabel.Text = upgradeShardsEarned + "/" + upgradeShardsRequired;
				this.iconBaseShardProgressBar.Value = this.nextProgressBarValue;
			}
			else if (flag2)
			{
				this.progressLerpTimer = 0f;
				this.isProgressBarLerping = true;
				this.iconProgressLabel.Text = this.previousShardsEarned + "/" + upgradeShardsRequired;
				this.iconBaseShardProgressBar.Value = this.previousProgressBarValue;
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			}
			switch (vto.SupplyVO.Type)
			{
			case SupplyType.Shard:
				this.titleText.Text = this.lang.Get("shard_shop_buy_equipment_shards", new object[0]);
				break;
			case SupplyType.ShardTroop:
			case SupplyType.ShardSpecialAttack:
				this.titleText.Text = this.lang.Get("shard_shop_buy_unit_shards", new object[0]);
				break;
			}
			if (vto.RemainingShardsForSale > 0)
			{
				UXUtils.SetupSingleResourceUI(vto.CostOfNextShard, this.purchaseSingleLabel, this.purchaseSingleSprite);
				UXUtils.SetupSingleResourceUI(vto.CostOfAllShards, this.purchaseAllLabel, this.purchaseAllSprite);
			}
			this.returnBtn.Visible = (vto.RemainingShardsForSale == 0);
			this.purchaseSingleBtn.Visible = (vto.RemainingShardsForSale > 0);
			this.purchaseAllBtn.Visible = (vto.RemainingShardsForSale > 0);
			this.parent.RevertToOriginalNameRecursively(this.icon.Root, "ModalIcon");
			Service.Engine.StartCoroutine(this.SetAnimatorState());
		}

		public void OnViewFrameTime(float dt)
		{
			this.progressLerpTimer += dt;
			float num = this.progressLerpTimer / 0.75f;
			int upgradeShardsEarned = this.vto.UpgradeShardsEarned;
			int upgradeShardsRequired = this.vto.UpgradeShardsRequired;
			this.iconBaseShardProgressBar.Value = Mathf.Lerp(this.previousProgressBarValue, this.nextProgressBarValue, num);
			int num2 = (int)Mathf.Lerp((float)this.previousShardsEarned, (float)upgradeShardsEarned, num);
			this.iconProgressLabel.Text = num2 + "/" + upgradeShardsRequired;
			if (num >= 1f)
			{
				this.isProgressBarLerping = false;
				bool value = this.vto.UpgradeShardsEarned >= this.vto.UpgradeShardsRequired;
				this.iconAnimator.SetBool("Upgradeable", value);
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
		}

		private void RegisterEvents()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.EquipmentUnlocked);
			eventManager.RegisterObserver(this, EventId.ShardUnitUpgraded);
		}

		private void UnregisterEvents()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.EquipmentUnlocked);
			eventManager.UnregisterObserver(this, EventId.ShardUnitUpgraded);
			eventManager.UnregisterObserver(this, EventId.ScreenClosing);
		}

		private float GetProgessBarValue(int shardsEarned, int shardsRequired)
		{
			float result = 1f;
			if (shardsEarned < shardsRequired)
			{
				result = (float)shardsEarned / (float)shardsRequired;
			}
			return result;
		}

		[DebuggerHidden]
		private IEnumerator SetAnimatorState()
		{
			yield return new WaitForEndOfFrame();
			this.icon.Visible = true;
			bool flag = this.vto.UpgradeShardsEarned >= this.vto.UpgradeShardsRequired;
			this.iconAnimator.SetInteger("Quality", this.vto.Quality);
			this.iconAnimator.SetBool("InModal", true);
			this.iconAnimator.SetBool("Visible", true);
			this.iconAnimator.SetBool("Upgradeable", flag && !this.isProgressBarLerping);
			this.modalAnimator.SetBool("Equipment", this.vto.SupplyVO.Type.Equals(SupplyType.Shard));
			yield break;
		}

		public void OnPurchaseShardsClicked(UXButton button)
		{
			int num = 1;
			bool flag = button == this.purchaseAllBtn;
			if (flag)
			{
				num = this.vto.RemainingShardsForSale;
			}
			CostVO costVO = (!flag) ? this.vto.CostOfNextShard : this.vto.CostOfAllShards;
			int crystals = costVO.Crystals;
			if (!ShardShopViewModule.warned && crystals >= GameConstants.CRYSTAL_SPEND_WARNING_MINIMUM)
			{
				ShardShopViewModule.warned = true;
				string currencyItemAssetName = UXUtils.GetCurrencyItemAssetName(CurrencyType.Crystals.ToString());
				AlertScreen.ShowModalWithImage(false, this.lang.Get("shard_shop_crystal_warning_title", new object[0]), this.lang.Get("shard_shop_crystal_warning_body", new object[]
				{
					crystals
				}), currencyItemAssetName, new OnScreenModalResult(this.PurchaseShards), num);
				return;
			}
			this.PurchaseShards(button, num);
		}

		private void PurchaseShards(object result, object cookie)
		{
			if (result == null)
			{
				ShardShopViewModule.warned = false;
				return;
			}
			int num = (int)cookie;
			bool flag = Service.ShardShopController.BuyShards(this.vto.SlotIndex, num, new Action<int, bool>(this.PredictNewPurchase), new Action<object>(this.OnPurchaseSuccess));
			if (flag)
			{
				this.PredictNewPurchase(num, false);
			}
		}

		private void PredictNewPurchase(int quantityToPurchase, bool blocking)
		{
			ShardShopController shardShopController = Service.ShardShopController;
			if (this.predictedData.Purchases[this.vto.SlotIndex].ContainsKey("Temp"))
			{
				Dictionary<string, int> dictionary;
				Dictionary<string, int> expr_4B = dictionary = this.predictedData.Purchases[this.vto.SlotIndex];
				string key;
				string expr_52 = key = "Temp";
				int num = dictionary[key];
				expr_4B[expr_52] = num + quantityToPurchase;
			}
			else
			{
				this.predictedData.Purchases[this.vto.SlotIndex].Add("Temp", quantityToPurchase);
			}
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			ShardShopViewTO shardShopViewTO = shardShopController.GenerateViewTO(this.vto.SlotIndex, currentPlayer, this.predictedData);
			shardShopViewTO.UpgradeShardsEarned += quantityToPurchase;
			this.quantityFloatingCounter.Text = this.vto.RemainingShardsForSale.ToString();
			this.modalAnimator.SetBool("BuyFragment", true);
			this.Render(shardShopViewTO, true);
			if (blocking)
			{
				ProcessingScreen.Show();
			}
			else
			{
				this.ShowPurchaseFlourishAnimation();
			}
		}

		private void ShowPurchaseFlourishAnimation()
		{
			if (this.iconAnimator != null)
			{
				this.iconAnimator.SetTrigger("CardFlourish");
			}
		}

		private void OnPostBlockingTriggerAnimations(uint timerId, object cookie)
		{
			this.animDelayTimerId = 0u;
			this.ShowPurchaseFlourishAnimation();
		}

		private void OnPurchaseSuccess(object cookie)
		{
			if (cookie == null)
			{
				return;
			}
			if (ProcessingScreen.IsShowing())
			{
				ProcessingScreen.Hide();
				this.animDelayTimerId = Service.ViewTimerManager.CreateViewTimer(0.4f, false, new TimerDelegate(this.OnPostBlockingTriggerAnimations), null);
			}
			int num = (int)cookie;
			int clientPredictionId = Service.ShardShopController.ClientPredictionId;
			if (num < clientPredictionId)
			{
				return;
			}
			this.predictedData = Service.ShardShopController.CurrentShopData.Copy();
			if (this.vto != null)
			{
				ShardShopViewTO shardShopViewTO = Service.ShardShopController.GenerateViewTO(this.vto.SlotIndex, Service.CurrentPlayer, this.predictedData);
				this.Render(shardShopViewTO, false);
			}
		}

		public void OnCloseModalClicked(UXButton button)
		{
			this.Hide();
		}

		public void Show(ShardShopViewTO vto)
		{
			this.Render(vto, false);
			this.RegisterEvents();
			this.modal.Visible = true;
			this.parent.CurrentBackDelegate = new UXButtonClickedDelegate(this.OnCloseModalClicked);
			this.parent.CurrentBackButton = this.closeModalBtn;
		}

		public void Hide()
		{
			this.isProgressBarLerping = false;
			this.modal.Visible = false;
			this.UnregisterEvents();
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			this.CleanUpAnimDelayTimer();
			this.vto = null;
			this.parent.SetupBackButtonDelegate();
			Service.EventManager.SendEvent(EventId.ShardViewClosed, null);
		}

		public bool IsModalVisible()
		{
			return this.modal.Visible;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.ScreenClosing)
			{
				if (id != EventId.EquipmentUnlocked)
				{
					if (id == EventId.ShardUnitUpgraded)
					{
						IDeployableVO deployableVO = (IDeployableVO)cookie;
						if (deployableVO == null)
						{
							return EatResponse.NotEaten;
						}
						if (deployableVO.Lvl > 1)
						{
							return EatResponse.NotEaten;
						}
						Service.EventManager.RegisterObserver(this, EventId.ScreenClosing);
					}
				}
				else
				{
					Service.EventManager.RegisterObserver(this, EventId.ScreenClosing);
				}
			}
			else
			{
				this.Render(this.vto, false);
			}
			return EatResponse.NotEaten;
		}

		private void CleanUpAnimDelayTimer()
		{
			if (this.animDelayTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.animDelayTimerId);
				this.animDelayTimerId = 0u;
			}
		}

		public void Destroy()
		{
			this.isProgressBarLerping = false;
			this.UnregisterEvents();
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			this.CleanUpAnimDelayTimer();
			if (this.projector != null)
			{
				this.projector.Destroy();
				this.projector = null;
			}
			if (this.icon != null)
			{
				UnityEngine.Object.Destroy(this.icon.Root);
				this.icon = null;
			}
		}
	}
}
