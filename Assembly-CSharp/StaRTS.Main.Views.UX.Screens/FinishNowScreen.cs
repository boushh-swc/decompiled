using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class FinishNowScreen : AlertScreen, IViewClockTimeObserver
	{
		private const string INSTANT_UPGRADE_CONFIRM_TITLE = "instant_upgrade_confirm_title";

		private const string UPGRADE_TITLE_FINISH_NOW = "upgrade_title_FinishNow";

		private const string UPGRADE_DESC_FINISH_NOW = "upgrade_desc_FinishNow";

		private const string UPGRADE_DESC_FINISH_NOW_UNITS = "upgrade_desc_FinishNow_units";

		private const string INSTANT_UPGRADE_CONFIRM_DESC = "instant_upgrade_confirm_desc";

		private string perkId;

		private Entity buildingEntity;

		private Contract currentContract;

		private bool noContract;

		private string displayName;

		private int remainingTimeInSec;

		private int crystals;

		private string titleOverride;

		private string messageOverride;

		private FinishNowScreen(Entity buildingEntity, Contract currentContract, bool noContract) : base(false, null, null, null, false)
		{
			this.buildingEntity = buildingEntity;
			this.perkId = null;
			this.currentContract = currentContract;
			this.noContract = noContract;
			StaticDataController staticDataController = Service.StaticDataController;
			if (currentContract != null)
			{
				switch (currentContract.DeliveryType)
				{
				case DeliveryType.Building:
					this.displayName = LangUtils.GetBuildingDisplayName(buildingEntity.Get<BuildingComponent>().BuildingType);
					break;
				case DeliveryType.UpgradeBuilding:
					this.displayName = LangUtils.GetBuildingDisplayName(buildingEntity.Get<BuildingComponent>().BuildingType);
					break;
				case DeliveryType.UpgradeTroop:
					this.displayName = LangUtils.GetTroopDisplayName(staticDataController.Get<TroopTypeVO>(currentContract.ProductUid));
					break;
				case DeliveryType.UpgradeStarship:
					this.displayName = LangUtils.GetStarshipDisplayName(staticDataController.Get<SpecialAttackTypeVO>(currentContract.ProductUid));
					break;
				case DeliveryType.UpgradeEquipment:
					this.displayName = LangUtils.GetEquipmentDisplayName(staticDataController.Get<EquipmentVO>(currentContract.ProductUid));
					break;
				}
			}
			else
			{
				this.displayName = LangUtils.GetBuildingDisplayName(buildingEntity.Get<BuildingComponent>().BuildingType);
			}
			this.RefreshData();
		}

		private FinishNowScreen(string perkId) : base(false, null, null, null, false)
		{
			this.buildingEntity = null;
			this.perkId = perkId;
			this.currentContract = null;
			this.noContract = true;
			this.displayName = string.Empty;
			this.RefreshData();
		}

		private FinishNowScreen() : base(false, null, null, null, false)
		{
			this.buildingEntity = null;
			this.perkId = null;
			this.currentContract = null;
			this.noContract = true;
			this.displayName = string.Empty;
			this.RefreshData();
		}

		public static FinishNowScreen ShowModalWithNoContract(Entity selectedBuilding, OnScreenModalResult onModalResult, object modalResultCookie, int crystalCost)
		{
			FinishNowScreen finishNowScreen = FinishNowScreen.CreateFinishNowScreen(selectedBuilding, null, true, onModalResult, modalResultCookie);
			finishNowScreen.crystals = crystalCost;
			Service.ScreenController.AddScreen(finishNowScreen);
			return finishNowScreen;
		}

		public static FinishNowScreen ShowModalWithNoContract(Entity selectedBuilding, OnScreenModalResult onModalResult, object modalResultCookie, int crystalCost, string title, string message, bool alwaysOnTop)
		{
			FinishNowScreen finishNowScreen = FinishNowScreen.CreateFinishNowScreen(selectedBuilding, null, true, onModalResult, modalResultCookie);
			finishNowScreen.crystals = crystalCost;
			finishNowScreen.titleOverride = title;
			finishNowScreen.messageOverride = message;
			finishNowScreen.IsAlwaysOnTop = alwaysOnTop;
			Service.ScreenController.AddScreen(finishNowScreen);
			return finishNowScreen;
		}

		public static FinishNowScreen ShowModalPerk(string perkId, OnScreenModalResult onModalResult, object modalResultCookie, int crystalCost, string title, string message, bool alwaysOnTop)
		{
			FinishNowScreen finishNowScreen = FinishNowScreen.CreateFinishNowPerkScreen(perkId, onModalResult, modalResultCookie);
			finishNowScreen.crystals = crystalCost;
			finishNowScreen.titleOverride = title;
			finishNowScreen.messageOverride = message;
			finishNowScreen.IsAlwaysOnTop = alwaysOnTop;
			Service.ScreenController.AddScreen(finishNowScreen);
			return finishNowScreen;
		}

		public static FinishNowScreen ShowModalEpisodeTask(EpisodeTaskVO vo, OnScreenModalResult onModalResult, object modalResultCookie, int crystalCost, string title, string message, bool alwaysOnTop)
		{
			FinishNowScreen finishNowScreen = new FinishNowScreen();
			finishNowScreen.OnModalResult = onModalResult;
			finishNowScreen.ModalResultCookie = modalResultCookie;
			finishNowScreen.crystals = crystalCost;
			finishNowScreen.titleOverride = title;
			finishNowScreen.messageOverride = message;
			finishNowScreen.IsAlwaysOnTop = alwaysOnTop;
			Service.ScreenController.AddScreen(finishNowScreen);
			return finishNowScreen;
		}

		public static void ShowModal(Entity selectedBuilding, OnScreenModalResult onModalResult, object modalResultCookie)
		{
			Contract contract = Service.ISupportController.FindCurrentContract(selectedBuilding.Get<BuildingComponent>().BuildingTO.Key);
			if (contract == null)
			{
				return;
			}
			FinishNowScreen screen = FinishNowScreen.CreateFinishNowScreen(selectedBuilding, contract, false, onModalResult, modalResultCookie);
			Service.ScreenController.AddScreen(screen);
		}

		private static FinishNowScreen CreateFinishNowScreen(Entity buildingEntity, Contract currentContract, bool noContract, OnScreenModalResult onScreenModalResult, object modalResultCookie)
		{
			return new FinishNowScreen(buildingEntity, currentContract, noContract)
			{
				OnModalResult = onScreenModalResult,
				ModalResultCookie = modalResultCookie
			};
		}

		private static FinishNowScreen CreateFinishNowPerkScreen(string perkId, OnScreenModalResult onScreenModalResult, object modalResultCookie)
		{
			return new FinishNowScreen(perkId)
			{
				OnModalResult = onScreenModalResult,
				ModalResultCookie = modalResultCookie
			};
		}

		private void RefreshData()
		{
			if (this.currentContract != null)
			{
				this.remainingTimeInSec = this.currentContract.GetRemainingTimeForSim();
				this.crystals = GameUtils.SecondsToCrystals(this.remainingTimeInSec);
				if (this.remainingTimeInSec <= 0)
				{
					this.Close(null);
				}
			}
		}

		protected override void SetupControls()
		{
			if (!this.noContract)
			{
				Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
			}
			if (Service.BuildingController.SelectedBuilding == this.buildingEntity)
			{
				Service.UXController.HUD.ShowContextButtons(null);
			}
			if (this.geometry != null)
			{
				UXUtils.SetupGeometryForIcon(this.sprite, this.geometry);
			}
			this.payButton.Visible = true;
			if (this.buildingEntity != null)
			{
				this.payButton.Tag = this.buildingEntity;
			}
			else if (!string.IsNullOrEmpty(this.perkId))
			{
				this.payButton.Tag = this.perkId;
			}
			else
			{
				this.payButton.Tag = true;
			}
			if (string.IsNullOrEmpty(this.titleOverride))
			{
				this.titleLabel.Text = Service.Lang.Get((!this.noContract) ? "upgrade_title_FinishNow" : "instant_upgrade_confirm_title", new object[0]);
			}
			else
			{
				this.titleLabel.Text = this.titleOverride;
			}
			this.payButton.OnClicked = new UXButtonClickedDelegate(this.OnPayButtonClicked);
			Service.UserInputInhibitor.AddToAllow(this.CloseButton);
			Service.UserInputInhibitor.AddToAllow(this.payButton);
			base.GetElement<UXLabel>("TickerDialogSmall").Visible = false;
		}

		public override void RefreshView()
		{
			if (!base.IsLoaded())
			{
				return;
			}
			if (this.remainingTimeInSec == 0 && !this.noContract)
			{
				this.Close(null);
			}
			string id = "upgrade_desc_FinishNow";
			if (this.currentContract != null && this.currentContract.DeliveryType != DeliveryType.UpgradeBuilding)
			{
				id = "upgrade_desc_FinishNow_units";
			}
			string text;
			if (this.messageOverride == null)
			{
				text = ((!this.noContract) ? Service.Lang.Get(id, new object[]
				{
					GameUtils.GetTimeLabelFromSeconds(this.remainingTimeInSec),
					this.displayName,
					this.crystals
				}) : Service.Lang.Get("instant_upgrade_confirm_desc", new object[]
				{
					this.crystals
				}));
			}
			else
			{
				text = this.messageOverride;
			}
			if (this.geometry == null)
			{
				this.centerLabel.Text = text.Replace("\\n", Environment.NewLine);
			}
			else
			{
				this.rightLabel.Text = text.Replace("\\n", Environment.NewLine);
			}
			UXUtils.SetupCostElements(this, "Cost", null, 0, 0, 0, this.crystals, false, null);
		}

		public void OnPayButtonClicked(UXButton button)
		{
			button.Enabled = false;
			this.Close(button.Tag);
		}

		public void OnViewClockTime(float dt)
		{
			this.RefreshData();
			this.RefreshView();
		}

		public override void Close(object modalResult)
		{
			if (Service.BuildingController.SelectedBuilding == this.buildingEntity)
			{
				Service.UXController.HUD.ShowContextButtons(this.buildingEntity);
			}
			base.Close(modalResult);
		}

		public override void OnDestroyElement()
		{
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			base.OnDestroyElement();
		}
	}
}
