using StaRTS.Assets;
using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class RateAppScreen : ClosableScreen
	{
		private const string CRYSTAL_GROUP = "CrystalIncentive";

		private const string CRYSTAL_TEXT_LEFT = "LabelCrystalIncentive";

		private const string CRYSTAL_TEXT_RIGHT = "LabelCrystalIncentive2";

		private const string NO_BUTTON = "BtnSecondary";

		private const string NO_BUTTON_TEXT = "LabelBtnSecondary";

		private const string YES_BUTTON = "BtnPrimary";

		private const string YES_BUTTON_TEXT = "LabelBtnPrimary";

		private const string TITLE_TEXT = "LabelTitle";

		private const string OBIWAN_PLACEHOLDER = "ObiWanHolder";

		private bool showIncent;

		private bool grantIncent;

		private UXButton yesButton;

		private UXButton noButton;

		private AssetHandle obiWanHandle;

		private GameObject obiWanObject;

		private bool obiWanLoaded;

		public bool ClosedWithConfirmation
		{
			get;
			private set;
		}

		protected override bool WantTransitions
		{
			get
			{
				return false;
			}
		}

		public RateAppScreen() : base("gui_rateapp")
		{
			this.ClosedWithConfirmation = false;
			this.obiWanHandle = AssetHandle.Invalid;
			Service.AssetManager.Load(ref this.obiWanHandle, "gui_obiwan", new AssetSuccessDelegate(this.OnObiWanLoadSuccess), new AssetFailureDelegate(this.OnObiWanLoadFail), null);
		}

		protected override void OnScreenLoaded()
		{
			if (this.obiWanLoaded)
			{
				this.Init();
			}
		}

		private void Init()
		{
			this.InitButtons();
			this.showIncent = GameConstants.RATE_APP_INCENTIVE_SHOW_ANDROID;
			this.grantIncent = GameConstants.RATE_APP_INCENTIVE_GRANT_ANDROID;
			if (this.showIncent)
			{
				base.GetElement<UXLabel>("LabelCrystalIncentive").Text = this.lang.Get("crystal_incentive_1", new object[]
				{
					GameConstants.RATE_APP_INCENTIVE_CRYSTALS
				});
				base.GetElement<UXLabel>("LabelCrystalIncentive2").Text = this.lang.Get("crystal_incentive_2", new object[0]);
			}
			else
			{
				base.GetElement<UXElement>("CrystalIncentive").Visible = false;
			}
			base.GetElement<UXLabel>("LabelTitle").Text = this.lang.Get("RATE_APP_SCREEN_TITLE", new object[0]);
			base.GetElement<UXLabel>("LabelBtnSecondary").Text = this.lang.Get("RATE_APP_SCREEN_NO_BUTTON", new object[0]);
			base.GetElement<UXLabel>("LabelBtnPrimary").Text = this.lang.Get("RATE_APP_SCREEN_YES_BUTTON", new object[0]);
			this.yesButton = base.GetElement<UXButton>("BtnPrimary");
			this.yesButton.OnClicked = new UXButtonClickedDelegate(this.OnButtonClicked);
			this.noButton = base.GetElement<UXButton>("BtnSecondary");
			this.noButton.OnClicked = new UXButtonClickedDelegate(this.OnButtonClicked);
		}

		private void OnObiWanLoadSuccess(object asset, object cookie)
		{
			this.obiWanLoaded = true;
			this.obiWanObject = Service.AssetManager.CloneGameObject(asset as GameObject);
			Transform transform = this.obiWanObject.transform;
			UXElement element = base.GetElement<UXElement>("ObiWanHolder");
			transform.parent = element.Root.transform;
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
			UXElement uXElement = base.CreateElements(this.obiWanObject);
			uXElement.WidgetDepth = base.WidgetDepth + 1;
			if (base.IsLoaded())
			{
				this.Init();
			}
		}

		private void OnObiWanLoadFail(object cookie)
		{
			this.obiWanLoaded = true;
			if (base.IsLoaded())
			{
				this.Init();
			}
		}

		private void OnButtonClicked(UXButton button)
		{
			this.ClosedWithConfirmation = (button == this.yesButton);
			if (this.grantIncent && this.ClosedWithConfirmation && !Service.CurrentPlayer.IsRateIncentivized)
			{
				PlayerIdChecksumRequest request = new PlayerIdChecksumRequest();
				AwardIncentiveCommand awardIncentiveCommand = new AwardIncentiveCommand(request);
				awardIncentiveCommand.AddSuccessCallback(new AbstractCommand<PlayerIdChecksumRequest, DefaultResponse>.OnSuccessCallback(this.AwardLocalCrystals));
				Service.ServerAPI.Sync(awardIncentiveCommand);
			}
			this.Close(this.ClosedWithConfirmation);
		}

		public override void Close(object modalResult)
		{
			base.Close(modalResult);
			Service.EventManager.SendEvent(EventId.RateAppScreenClosed, null);
		}

		private void AwardLocalCrystals(DefaultResponse response, object cookie)
		{
			Service.CurrentPlayer.IsRateIncentivized = true;
			Service.CurrentPlayer.Inventory.ModifyCrystals(GameConstants.RATE_APP_INCENTIVE_CRYSTALS);
		}

		public override void OnDestroyElement()
		{
			if (this.obiWanHandle != AssetHandle.Invalid)
			{
				Service.AssetManager.Unload(this.obiWanHandle);
				this.obiWanHandle = AssetHandle.Invalid;
			}
			if (this.obiWanObject != null)
			{
				UnityEngine.Object.Destroy(this.obiWanObject);
				this.obiWanObject = null;
			}
			base.OnDestroyElement();
		}
	}
}
