using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class AlertScreen : ClosableScreen
	{
		private const string TITLE_LABEL = "LabelTitle";

		private const string TITLE_CENTER_LABEL = "LabelTitleCenter";

		private const string CENTER_LABEL = "LabelBody";

		private const string RIGHT_LABEL = "LabelBodyWImage";

		private const string SPRITE_IMAGE = "SpriteImage";

		private const string TEXTURE_IMAGE = "TextureImage";

		private const string TEXTURE_IMAGE_INSET = "TextureImageInset";

		private const string PRIMARY_BUTTON = "BtnPrimary";

		private const string PRIMARY_LABEL = "LabelBtnPrimary";

		private const string PAY_BUTTON = "BtnPay";

		private const string PAY_LEFT_BUTTON = "BtnOptionPay1";

		private const string PAY_RIGHT_BUTTON = "BtnOptionPay2";

		private const string PAY_LEFT_LABEL = "CostOptionPay1LabelDescription";

		private const string PAY_RIGHT_LABEL = "CostOptionPay2LabelDescription";

		private const string PRIMARY_2OPTION_BUTTON = "BtnPrimary_2option";

		private const string PRIMARY_2OPTION_LABEL = "LabelBtnPrimary_2option";

		private const string SECONDARY_2OPTION_BUTTON = "BtnSecondary";

		private const string OPTION2_BUTTON_SKIP = "BtnSkip";

		private const string SECONDARY_2OPTION_LABEL = "LabelBtnSecondary";

		private const string BG_GRAPHIC = "SpriteTextBg";

		protected const string COST_GROUP = "Cost";

		protected const string COST_REQUIREMENT = "Requirement";

		protected const string BUTTON_OPTION2 = "2option";

		protected const string LABEL_BODY_REQUIREMENT = "LabelBodyRequirement";

		protected const string LABEL_STAR_PROGRESS = "LabelStarProgress";

		protected const string SLIDER_REQUIREMENT = "pBarStarProgress";

		protected const string LABEL_SKIP_COST = "SkipCostLabel";

		protected const string LABEL_SKIP = "LabelSkip";

		protected const string LABEL_TIMER = "TickerDialogSmall";

		protected const string GROUP_MULTIPLE_ITEMS = "ImageAndTextMultiple";

		protected const string LABEL_MESSAGE_MULTIPLE_ITEMS = "LabelImageAndTextMultiple";

		protected const string TABLE_MULTIPLE_ITEMS = "TableImageAndTextMultiple";

		protected const string TEMPLATE_MULTIPLE_ITEMS = "ItemImageAndTextMultiple";

		protected const string SPRITE_MULTIPLE_ITEMS = "SpriteImageAndTextMultiple";

		protected const string LABEL_MULTIPLE_ITEMS = "LabelItemImageAndTextMultiple";

		protected const string PERK_TITLE_GROUP = "TitleGroupPerks";

		protected const string LABEL_REQUIREMENT = "LabelRequirement";

		protected const string COST_LEFT_GROUP = "CostOptionPay1";

		protected const string COST_RIGHT_GROUP = "CostOptionPay2";

		private const string SKIP_STRING = "s_SKIP";

		private const int DEPTH_FUDGE = 10000;

		protected UXLabel titleLabel;

		protected UXLabel centerLabel;

		protected UXLabel rightLabel;

		protected UXSprite sprite;

		protected UXTexture textureImage;

		protected UXTexture textureImageInset;

		protected UXButton primaryButton;

		protected UXLabel primaryLabel;

		protected UXButton payButton;

		protected UXButton payLeftButton;

		protected UXButton payRightButton;

		protected UXLabel payLeftLabel;

		protected UXLabel payRightLabel;

		protected UXButton primary2OptionButton;

		protected UXLabel primary2Option;

		protected UXButton secondary2OptionButton;

		protected UXButton option2ButtonSkip;

		protected UXLabel secondary2Option;

		protected UXElement bgGraphic;

		protected UXElement costRequirement;

		protected UXElement buttonOption2;

		protected UXLabel labelBodyRequirement;

		protected UXLabel labelStarProgress;

		protected UXSlider requirementSlider;

		protected UXLabel labelSkip;

		protected UXLabel labelSkipCost;

		protected UXLabel labelTimer;

		protected UXElement groupMultipleItems;

		protected UXLabel multiItemMessageLabel;

		protected UXTable multiItemTable;

		protected UXElement perkTitleGroup;

		protected UXLabel requirementLabel;

		protected string title;

		protected string message;

		protected string spriteName;

		protected string primaryLabelOverride;

		protected string secondaryLabelOverride;

		protected IGeometryVO geometry;

		protected string textureInsetAssetName;

		private bool disableCloseBtn;

		protected bool centerTitle;

		public bool IsFatal
		{
			get;
			private set;
		}

		protected override bool AllowGarbageCollection
		{
			get
			{
				return false;
			}
		}

		protected override bool WantTransitions
		{
			get
			{
				return false;
			}
		}

		protected AlertScreen(bool fatal, string title, string message, string spriteName, bool disableCloseBtn) : base("gui_dialog_small")
		{
			this.IsFatal = fatal;
			this.title = title;
			this.message = message;
			this.spriteName = spriteName;
			this.disableCloseBtn = disableCloseBtn;
		}

		public static AlertScreen ShowModalWithImage(bool fatal, string title, string message, string spriteName, OnScreenModalResult onModalResult, object modalResultCookie)
		{
			return AlertScreen.ShowModal(fatal, title, message, spriteName, onModalResult, modalResultCookie, false, false, null, true);
		}

		public static AlertScreen ShowModalWithBI(bool fatal, string title, string message, string biMessage)
		{
			return AlertScreen.ShowModal(fatal, title, message, null, null, null, false, false, biMessage, true);
		}

		public static AlertScreen ShowModal(bool fatal, string title, string message, OnScreenModalResult onModalResult, object modalResultCookie)
		{
			return AlertScreen.ShowModal(fatal, title, message, null, onModalResult, modalResultCookie, false, false, null, true);
		}

		public static AlertScreen ShowModal(bool fatal, string title, string message, OnScreenModalResult onModalResult, object modalResultCookie, bool isAlwaysOnTop)
		{
			return AlertScreen.ShowModal(fatal, title, message, null, onModalResult, modalResultCookie, false, isAlwaysOnTop, null, true);
		}

		public static AlertScreen ShowModal(bool fatal, string title, string message, string spriteName, OnScreenModalResult onModalResult, object modalResultCookie, bool disableCloseBtn, bool isAlwaysOnTop)
		{
			return AlertScreen.ShowModal(fatal, title, message, null, onModalResult, modalResultCookie, false, isAlwaysOnTop, null, true);
		}

		public static AlertScreen ShowModal(bool fatal, string title, string message, string spriteName, OnScreenModalResult onModalResult, object modalResultCookie, bool disableCloseBtn, bool isAlwaysOnTop, string biMessage, bool turnOffTicker)
		{
			AlertScreen alertScreen = new AlertScreen(fatal, title, message, spriteName, disableCloseBtn);
			alertScreen.OnModalResult = onModalResult;
			alertScreen.ModalResultCookie = modalResultCookie;
			alertScreen.IsAlwaysOnTop = isAlwaysOnTop;
			if (fatal && !string.IsNullOrEmpty(biMessage))
			{
				Service.Logger.ErrorFormat("Force Reload Popup: {0}", new object[]
				{
					biMessage
				});
			}
			if (Service.ScreenController != null)
			{
				Service.ScreenController.AddScreen(alertScreen, turnOffTicker);
			}
			return alertScreen;
		}

		public override void OnDestroyElement()
		{
			if (this.IsFatal)
			{
				if (Service.CameraManager != null)
				{
					Service.CameraManager.SetRegularCameraOrder();
				}
				if (Service.UserInputManager != null)
				{
					Service.UserInputManager.Enable(false);
				}
			}
			if (this.primaryButton != null)
			{
				this.primaryButton.Enabled = true;
			}
			if (this.payButton != null)
			{
				this.payButton.Enabled = true;
			}
			if (this.payLeftButton != null)
			{
				this.payLeftButton.Enabled = true;
			}
			if (this.payRightButton != null)
			{
				this.payRightButton.Enabled = true;
			}
			if (this.primary2OptionButton != null)
			{
				this.primary2OptionButton.Enabled = true;
			}
			if (this.secondary2OptionButton != null)
			{
				this.secondary2OptionButton.Enabled = true;
			}
			if (this.option2ButtonSkip != null)
			{
				this.option2ButtonSkip.Enabled = true;
			}
			if (this.sprite != null)
			{
				this.sprite.Enabled = true;
			}
			if (this.textureImage != null)
			{
				this.textureImage.Enabled = true;
			}
			if (this.textureImageInset != null)
			{
				this.textureImageInset.Enabled = true;
			}
			if (this.groupMultipleItems != null)
			{
				this.groupMultipleItems.Visible = false;
			}
			if (this.multiItemTable != null)
			{
				this.multiItemTable.Clear();
				this.multiItemTable = null;
			}
			base.OnDestroyElement();
		}

		protected override void OnScreenLoaded()
		{
			this.InitLabels();
			this.InitButtons();
			this.InitMultiGroupItems();
			if (Service.ScreenController == null)
			{
				this.AdjustDepthRecursively(base.Root, 10000);
			}
			this.Visible = true;
			this.SetupControls();
			if (this.IsFatal)
			{
				if (Service.CameraManager.WipeCamera.IsRendering())
				{
					Service.CameraManager.WipeCamera.StopWipe();
					Service.CameraManager.UXCamera.Camera.enabled = true;
					Service.UXController.HUD.Visible = false;
				}
				Service.UserInputManager.Enable(true);
				Service.CameraManager.SetCameraOrderForPreloadScreens();
				if (Service.UXController != null)
				{
					Service.UXController.MiscElementsManager.HideHighlight();
					Service.UXController.MiscElementsManager.HideTroopCounter();
				}
				base.AllowClose = false;
			}
		}

		private void InitLabels()
		{
			if (this.centerTitle)
			{
				this.titleLabel = base.GetElement<UXLabel>("LabelTitleCenter");
				base.GetElement<UXLabel>("LabelTitle").Visible = false;
			}
			else
			{
				this.titleLabel = base.GetElement<UXLabel>("LabelTitle");
				base.GetElement<UXLabel>("LabelTitleCenter").Visible = false;
			}
			this.centerLabel = base.GetElement<UXLabel>("LabelBody");
			this.rightLabel = base.GetElement<UXLabel>("LabelBodyWImage");
			this.primaryLabel = base.GetElement<UXLabel>("LabelBtnPrimary");
			this.primary2Option = base.GetElement<UXLabel>("LabelBtnPrimary_2option");
			this.secondary2Option = base.GetElement<UXLabel>("LabelBtnSecondary");
			this.centerLabel.Text = string.Empty;
			this.rightLabel.Text = string.Empty;
			UXElement element = base.GetElement<UXElement>("TitleGroupPerks");
			element.Visible = false;
			this.costRequirement = base.GetElement<UXElement>("Requirement");
			this.buttonOption2 = base.GetElement<UXElement>("2option");
			this.labelBodyRequirement = base.GetElement<UXLabel>("LabelBodyRequirement");
			this.labelStarProgress = base.GetElement<UXLabel>("LabelStarProgress");
			this.requirementSlider = base.GetElement<UXSlider>("pBarStarProgress");
			this.labelSkip = base.GetElement<UXLabel>("LabelSkip");
			this.labelSkip.Text = this.lang.Get("s_SKIP", new object[0]);
			this.labelSkipCost = base.GetElement<UXLabel>("SkipCostLabel");
			this.labelTimer = base.GetElement<UXLabel>("TickerDialogSmall");
			this.requirementLabel = base.GetElement<UXLabel>("LabelRequirement");
			this.requirementLabel.Visible = false;
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			this.sprite = base.GetElement<UXSprite>("SpriteImage");
			this.textureImage = base.GetElement<UXTexture>("TextureImage");
			this.textureImageInset = base.GetElement<UXTexture>("TextureImageInset");
			this.primaryButton = base.GetElement<UXButton>("BtnPrimary");
			this.payButton = base.GetElement<UXButton>("BtnPay");
			this.payLeftButton = base.GetElement<UXButton>("BtnOptionPay1");
			this.payRightButton = base.GetElement<UXButton>("BtnOptionPay2");
			this.payLeftLabel = base.GetElement<UXLabel>("CostOptionPay1LabelDescription");
			this.payRightLabel = base.GetElement<UXLabel>("CostOptionPay2LabelDescription");
			this.primary2OptionButton = base.GetElement<UXButton>("BtnPrimary_2option");
			this.secondary2OptionButton = base.GetElement<UXButton>("BtnSecondary");
			this.option2ButtonSkip = base.GetElement<UXButton>("BtnSkip");
			this.bgGraphic = base.GetElement<UXElement>("SpriteTextBg");
			this.bgGraphic.Visible = true;
			this.CloseButton.Enabled = (!this.IsFatal && !this.disableCloseBtn);
			if (!this.IsFatal && Service.UserInputInhibitor != null)
			{
				Service.UserInputInhibitor.AddToAllow(this.CloseButton);
			}
			this.primaryButton.Visible = false;
			this.payButton.Visible = false;
			this.payLeftButton.Visible = false;
			this.payRightButton.Visible = false;
			this.primary2OptionButton.Visible = false;
			this.secondary2OptionButton.Visible = false;
			this.costRequirement.Visible = false;
			this.option2ButtonSkip.Visible = false;
			this.labelBodyRequirement.Visible = false;
		}

		private void InitMultiGroupItems()
		{
			this.groupMultipleItems = base.GetElement<UXElement>("ImageAndTextMultiple");
			this.groupMultipleItems.Visible = false;
			this.multiItemMessageLabel = base.GetElement<UXLabel>("LabelImageAndTextMultiple");
			this.multiItemTable = base.GetElement<UXTable>("TableImageAndTextMultiple");
			this.multiItemTable.SetTemplateItem("ItemImageAndTextMultiple");
		}

		protected virtual void SetupControls()
		{
			this.labelTimer.Visible = false;
			this.primaryButton.Visible = true;
			this.primaryButton.OnClicked = new UXButtonClickedDelegate(this.OnPrimaryButtonClicked);
			if (Service.UserInputInhibitor != null)
			{
				Service.UserInputInhibitor.AddToAllow(this.primaryButton);
			}
			this.titleLabel.Text = this.title;
			if (this.title == null)
			{
				this.titleLabel.Text = this.lang.Get((!this.IsFatal) ? "ALERT" : "ERROR", new object[0]);
			}
			if (string.IsNullOrEmpty(this.primaryLabelOverride))
			{
				this.primary2Option.Text = this.lang.Get("YES", new object[0]);
				this.primaryLabel.Text = this.lang.Get((!this.IsFatal) ? "OK" : "RELOAD", new object[0]);
			}
			else
			{
				this.primary2Option.Text = this.primaryLabelOverride;
				this.primaryLabel.Text = this.primaryLabelOverride;
			}
			if (string.IsNullOrEmpty(this.secondaryLabelOverride))
			{
				this.secondary2Option.Text = this.lang.Get("NO", new object[0]);
			}
			else
			{
				this.secondary2Option.Text = this.secondaryLabelOverride;
			}
			this.textureImage.Enabled = false;
			if (!string.IsNullOrEmpty(this.textureInsetAssetName))
			{
				this.centerLabel.Visible = false;
				this.rightLabel.Visible = true;
				this.rightLabel.Text = this.message;
				this.sprite.SpriteName = "bkgClear";
				this.textureImageInset.LoadTexture(this.textureInsetAssetName);
				this.sprite.Enabled = false;
			}
			else if (this.geometry != null)
			{
				this.centerLabel.Visible = false;
				this.rightLabel.Visible = true;
				this.rightLabel.Text = this.message;
				this.sprite.SpriteName = "bkgClear";
				this.textureImageInset.Enabled = false;
				UXUtils.SetupGeometryForIcon(this.sprite, this.geometry);
			}
			else if (string.IsNullOrEmpty(this.spriteName))
			{
				this.centerLabel.Visible = true;
				this.rightLabel.Visible = false;
				this.centerLabel.Text = this.message;
				this.textureImageInset.Enabled = false;
			}
			else
			{
				this.centerLabel.Visible = false;
				this.rightLabel.Visible = true;
				this.rightLabel.Text = this.message;
				this.sprite.SpriteName = "bkgClear";
				this.textureImageInset.Enabled = false;
				UXUtils.SetupGeometryForIcon(this.sprite, this.spriteName);
			}
		}

		public void OnPrimaryButtonClicked(UXButton button)
		{
			button.Enabled = false;
			this.Close(true);
			if (this.IsFatal)
			{
				Service.Engine.Reload();
			}
		}

		private void AdjustDepthRecursively(GameObject gameObject, int depthDiff)
		{
			UIWidget component = gameObject.GetComponent<UIWidget>();
			if (component != null)
			{
				component.depth += depthDiff;
			}
			Transform transform = gameObject.transform;
			int i = 0;
			int childCount = transform.childCount;
			while (i < childCount)
			{
				this.AdjustDepthRecursively(transform.GetChild(i).gameObject, depthDiff);
				i++;
			}
		}

		public virtual void SetPrimaryLabelText(string text)
		{
			this.primaryLabelOverride = text;
		}

		public virtual void SetSecondaryLabelText(string text)
		{
			this.secondaryLabelOverride = text;
		}

		public virtual void SetIconAsset(IGeometryVO geometery)
		{
			this.geometry = geometery;
		}

		public void SetTextureInset(string assetName)
		{
			this.textureInsetAssetName = assetName;
		}
	}
}
