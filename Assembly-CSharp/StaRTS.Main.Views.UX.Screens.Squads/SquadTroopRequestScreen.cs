using Net.RichardLord.Ash.Core;
using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Tags;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.Squads
{
	public class SquadTroopRequestScreen : AbstractSquadRequestScreen, IEventObserver, IViewClockTimeObserver
	{
		private const string REQUEST_NOW_COST_LABEL = "FinishCostLabel";

		private const string WAR_REQUEST_INPUT = "LabelInputNameSquadWar";

		private const string GRID_TEMPLATE = "CardCurrentTroops";

		private const string GRID_ITEM_COUNT = "LabelDonateTroops";

		private const string GRID_ITEM_TROOP_ICON = "SpriteDonateTroopsItem";

		private const string GRID_ITEM_LEVEL = "LabelTroopLevel";

		private const string CARD_FULL = "CardFull";

		private const string CARD_EMPTY = "SpriteCardEmpty";

		private const string WAR_TEXTURE_HOLDER = "TextureWarRequest";

		private const string WAR_TEXTURE_NAME = "squadwars_playerdetails_bg";

		private const string REQUEST_TROOPS = "REQUEST_TROOPS";

		private const string REQUEST_WAR_TROOPS = "REQUEST_WAR_TROOPS";

		private const string REQUEST_WAR_TROOPS_FULL_MESSAGE = "REQUEST_WAR_TROOPS_FULL_MESSAGE";

		private const string REQUEST_TIME_LIMIT_MESSAGE = "REQUEST_TIME_LIMIT_MESSAGE";

		private const string REQUEST_TIME_LEFT_MESSAGE = "REQUEST_TIME_LEFT_MESSAGE";

		private const string SQUAD_DONATED_TROOP = "SQUAD_DONATED_TROOP";

		private const string SQUAD_DONATED_TROOP_SENDER = "SQUAD_DONATED_TROOP_SENDER";

		private const string SQUAD_DONATED_TROOP_SENDER_OTHER = "SQUAD_DONATED_TROOP_SENDER_OTHER";

		private const string SQUAD_DONATED_TO_USER = "SQUAD_DONATED_TO_USER";

		private const int MAX_DONATORS_DISPLAYED = 3;

		private UXLabel requestNowCostLabel;

		protected UXGrid storageItemGrid;

		protected List<TroopUpgradeTag> troopList;

		private string defaultRequestText;

		private bool observingClockTime;

		private bool isWarRequest;

		private bool canSendFreeRequest = true;

		private bool canRequestTroops = true;

		public SquadTroopRequestScreen(string defaultText, bool warMode)
		{
			this.defaultRequestText = defaultText;
			this.isWarRequest = warMode;
		}

		protected override void OnScreenLoaded()
		{
			base.OnScreenLoaded();
			base.GetElement<UXElement>("InputFieldRequestMessage").Visible = !this.isWarRequest;
			base.GetElement<UXElement>("InputFieldRequestMessageWar").Visible = this.isWarRequest;
			this.requestNowCostLabel = base.GetElement<UXLabel>("FinishCostLabel");
			this.requestLabel.Text = this.lang.Get("REQUEST_TROOPS", new object[0]);
			this.requestPerksLabel.Text = this.requestLabel.Text;
			this.requestLabel.Visible = true;
			this.troopsPanel.Visible = true;
			this.troopsLabel.Visible = true;
			this.troopsLabel.Text = this.lang.Get("SQUAD_DONATED_TO_USER", new object[0]);
			if (this.isWarRequest)
			{
				this.input = base.GetElement<UXInput>("LabelInputNameSquadWar");
				UIInput uIInputComponent = this.input.GetUIInputComponent();
				uIInputComponent.onValidate = new UIInput.OnValidate(LangUtils.OnValidateWNewLines);
				UXTexture element = base.GetElement<UXTexture>("TextureWarRequest");
				element.LoadTexture("squadwars_playerdetails_bg");
				Service.EventManager.RegisterObserver(this, EventId.CurrentPlayerMemberDataUpdated);
			}
			this.input.InitText(this.defaultRequestText);
			this.InitTroopGrid();
			this.UpdateRequestState();
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			Entity currentSquadBuilding = Service.BuildingLookupController.GetCurrentSquadBuilding();
			if (currentSquadBuilding == null)
			{
				return;
			}
			BuildingTypeVO buildingType = currentSquadBuilding.Get<BuildingComponent>().BuildingType;
			if (buildingType == null)
			{
				return;
			}
			this.SetupPerksButton(buildingType);
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.CurrentPlayerMemberDataUpdated)
			{
				if (this.isWarRequest)
				{
					this.InitTroopGrid();
				}
			}
			return base.OnEvent(id, cookie);
		}

		protected override void OnClicked(UXButton button)
		{
			if (base.IsClosing || !base.CheckForValidInput())
			{
				return;
			}
			string text = this.input.Text;
			if (string.IsNullOrEmpty(text))
			{
				text = this.defaultRequestText;
			}
			Service.SquadController.SendTroopRequest(text, this.isWarRequest);
			this.Close(null);
		}

		private void UpdateRequestState()
		{
			uint time = ServerTime.Time;
			SquadController squadController = Service.SquadController;
			uint troopRequestDate;
			if (this.isWarRequest)
			{
				troopRequestDate = squadController.StateManager.WarTroopRequestDate;
			}
			else
			{
				troopRequestDate = squadController.StateManager.TroopRequestDate;
			}
			int troopRequestTimeLeft = SquadUtils.GetTroopRequestTimeLeft(time, troopRequestDate);
			this.canSendFreeRequest = (troopRequestTimeLeft <= 0);
			this.UpdateScreenElementVisibility();
			if (this.canSendFreeRequest)
			{
				int troopRequestCooldownTime = Service.PerkManager.GetTroopRequestCooldownTime();
				this.instructionsLabel.Text = this.lang.Get("REQUEST_TIME_LIMIT_MESSAGE", new object[]
				{
					(uint)((ulong)(GameConstants.SQUAD_TROOP_REQUEST_THROTTLE_MINUTES * 60u) - (ulong)((long)troopRequestCooldownTime)) / 60u
				});
				if (this.observingClockTime)
				{
					this.observingClockTime = false;
					Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
				}
			}
			else
			{
				this.instructionsLabel.Text = this.lang.Get("REQUEST_TIME_LEFT_MESSAGE", new object[]
				{
					Math.Ceiling((double)((float)troopRequestTimeLeft / 60f))
				});
				int troopRequestCrystalCost = SquadUtils.GetTroopRequestCrystalCost(time, troopRequestDate);
				this.costButton.Enabled = GameUtils.CanAffordCrystals(troopRequestCrystalCost);
				this.requestNowCostLabel.Text = troopRequestCrystalCost.ToString();
				this.observingClockTime = true;
				Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
			}
			if (this.isWarRequest)
			{
				bool flag = SquadUtils.IsPlayerSquadWarTroopsAtMaxCapacity();
				this.instructionsLabel.Visible = !flag;
			}
			this.instructionsPerksLabel.Text = this.instructionsLabel.Text;
		}

		protected void InitTroopGrid()
		{
			this.storageItemGrid = base.GetElement<UXGrid>("GridCurrentTroops");
			this.storageItemGrid.SetTemplateItem("CardCurrentTroops");
			this.storageItemGrid.Clear();
			this.troopList = new List<TroopUpgradeTag>();
			int num = 0;
			GamePlayer worldOwner = GameUtils.GetWorldOwner();
			List<SquadDonatedTroop> list;
			int squadStorageCapacity;
			if (this.isWarRequest)
			{
				SquadMemberWarData currentMemberWarData = Service.SquadController.WarManager.GetCurrentMemberWarData();
				list = currentMemberWarData.WarTroops;
				squadStorageCapacity = currentMemberWarData.BaseMap.GetSquadStorageCapacity();
			}
			else
			{
				list = SquadUtils.GetWorldOwnerSquadBuildingTroops();
				squadStorageCapacity = worldOwner.Map.GetSquadStorageCapacity();
			}
			int count = list.Count;
			if (count > 0)
			{
				StaticDataController staticDataController = Service.StaticDataController;
				for (int i = 0; i < count; i++)
				{
					SquadDonatedTroop squadDonatedTroop = list[i];
					TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(squadDonatedTroop.TroopUid);
					int totalAmount = squadDonatedTroop.GetTotalAmount();
					if (totalAmount > 0)
					{
						this.AddTroopItem(troopTypeVO, totalAmount);
						num += totalAmount * troopTypeVO.Size;
					}
				}
			}
			this.canRequestTroops = (num < squadStorageCapacity);
			this.UpdateScreenElementVisibility();
			this.storageItemGrid.RepositionItems();
		}

		protected void AddTroopItem(IUpgradeableVO troop, int troopCount)
		{
			TroopUpgradeTag item = new TroopUpgradeTag(troop as IDeployableVO, true);
			this.troopList.Add(item);
			string uid = troop.Uid;
			UXElement item2 = this.storageItemGrid.CloneTemplateItem(uid);
			UXLabel subElement = this.storageItemGrid.GetSubElement<UXLabel>(uid, "LabelDonateTroops");
			subElement.Text = LangUtils.GetMultiplierText(troopCount);
			UXSprite subElement2 = this.storageItemGrid.GetSubElement<UXSprite>(uid, "SpriteDonateTroopsItem");
			ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(troop as IDeployableVO, subElement2);
			projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
			ProjectorUtils.GenerateProjector(projectorConfig);
			UXLabel subElement3 = this.storageItemGrid.GetSubElement<UXLabel>(uid, "LabelTroopLevel");
			subElement3.Text = LangUtils.GetLevelText(troop.Lvl);
			this.storageItemGrid.GetSubElement<UXSprite>(uid, "SpriteCardEmpty").Visible = false;
			this.storageItemGrid.AddItem(item2, troop.Order);
		}

		public void OnViewClockTime(float dt)
		{
			this.UpdateRequestState();
		}

		public override void OnDestroyElement()
		{
			if (this.observingClockTime)
			{
				Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
				this.observingClockTime = false;
			}
			Service.EventManager.UnregisterObserver(this, EventId.CurrentPlayerMemberDataUpdated);
			base.OnDestroyElement();
		}

		private void UpdateScreenElementVisibility()
		{
			if (this.isWarRequest)
			{
				this.costButton.Visible = (!this.canSendFreeRequest && this.canRequestTroops);
				this.button.Visible = (this.canSendFreeRequest && this.canRequestTroops);
				if (!this.canRequestTroops)
				{
					this.instructionsLabel.Text = this.lang.Get("REQUEST_WAR_TROOPS_FULL_MESSAGE", new object[0]);
					base.GetElement<UXElement>("InputFieldRequestMessageWar").Visible = false;
					this.instructionsPerksLabel.Text = this.instructionsLabel.Text;
				}
			}
			else
			{
				this.costButton.Visible = !this.canSendFreeRequest;
				this.button.Visible = this.canSendFreeRequest;
			}
		}
	}
}
