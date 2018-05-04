using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Story;
using StaRTS.Main.Views.UserInput;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class HolocommScreen : ScreenBase, IEventObserver, IUserInputObserver
	{
		public delegate void HoloCallback();

		private const string INFO_PANEL = "InfoItems";

		private const string INFO_LABEL = "LabelInfoItem";

		private const string INFO_TEXTURE_HOLDER = "SpriteInfoItem";

		private const string PLANET_TEXTURE_HOLDER = "SpriteInfoItemPlanets";

		private const string INFO_TITLE_LABEL = "LabelInfoItemTitle";

		public const string NEXT_BUTTON = "BtnNext";

		public const string NEXT_BUTTON_LABEL = "LabelBtnNext";

		public const string STORE_BUTTON = "ButtonStore";

		private const string REGULAR_TEXT_BOX_GROUP = "NpcDialogLarge";

		private const string REGULAR_TITLE_LABEL = "LabelNpcMessageBottomLeftTitleLarge";

		private const string REGULAR_BODY_LABEL = "LabelNpcMessageBottomLeftBodyLarge";

		private const string STORE_TEXT_BOX_GROUP = "NpcDialog";

		private const string STORE_TITLE_LABEL = "LabelNpcMessageBottomLeftTitle";

		private const string STORE_BODY_LABEL = "LabelNpcMessageBottomLeftBody";

		private const string HOLO_POSITION_PANEL = "HoloHolder";

		private const string TEXTURE_INFO_ALLOY = "InfoAlloy";

		private const string TEXTURE_INFO_ATST = "InfoATST";

		private const string TEXTURE_INFO_CREDIT = "InfoCredit";

		private const string TEXTURE_INFO_CROSSGRADE = "InfoCrossgrade";

		private const string TEXTURE_INFO_DROID = "InfoDroid";

		private const string TEXTURE_INFO_HAN = "InfoHan";

		private const string TEXTURE_INFO_STRIX = "InfoStrix";

		private const string TEXTURE_INFO_TURRET = "InfoTurret";

		private const string TEXTURE_INFO_GALAXY_VIEW = "InfoGalaxyView";

		private const string TEXTURE_INFO_PLANETARY_EMPIRE = "InfoPlanetaryCommandEmpire";

		private const string TEXTURE_INFO_PLANETARY_REBEL = "InfoPlanetaryCommandRebel";

		private UXLabel infoLabel;

		private UXElement infoPanel;

		private UXTexture infoTexture;

		private UXTexture infoPlanetTexture;

		private UXLabel infoTitleLabel;

		private UXButton nextButton;

		private UXButton storeButton;

		private UXElement holoPositioner;

		private UXElement regularTextBoxGroup;

		private UXLabel regularTitleLabel;

		private UXLabel regularBodyLabel;

		private UXElement storeTextBoxGroup;

		private UXLabel storeTitleLabel;

		private UXLabel storeBodyLabel;

		private HoloCharacter currentCharacter;

		public HolocommScreen() : base("gui_npc_dialog")
		{
			base.IsAlwaysOnTop = true;
			Service.UserInputManager.RegisterObserver(this, UserInputLayer.Screen);
			Service.EventManager.RegisterObserver(this, EventId.ShowHologramComplete);
		}

		public bool HasCharacterShowing()
		{
			return this.currentCharacter != null;
		}

		public void RepositionCharacters()
		{
			if (this.currentCharacter != null)
			{
				this.currentCharacter.UpdatePositionOnScreen();
			}
		}

		protected override void OnScreenLoaded()
		{
			this.InitElements();
		}

		private void InitElements()
		{
			base.GetElement<UXLabel>("LabelBtnNext").Text = this.lang.Get("s_WhatsNextButton", new object[0]);
			this.holoPositioner = base.GetElement<UXElement>("HoloHolder");
			this.nextButton = base.GetElement<UXButton>("BtnNext");
			this.storeButton = base.GetElement<UXButton>("ButtonStore");
			this.infoPanel = base.GetElement<UXElement>("InfoItems");
			this.infoLabel = base.GetElement<UXLabel>("LabelInfoItem");
			this.infoTexture = base.GetElement<UXTexture>("SpriteInfoItem");
			this.infoPlanetTexture = base.GetElement<UXTexture>("SpriteInfoItemPlanets");
			this.infoTitleLabel = base.GetElement<UXLabel>("LabelInfoItemTitle");
			this.regularTextBoxGroup = base.GetElement<UXElement>("NpcDialogLarge");
			this.regularTitleLabel = base.GetElement<UXLabel>("LabelNpcMessageBottomLeftTitleLarge");
			this.regularBodyLabel = base.GetElement<UXLabel>("LabelNpcMessageBottomLeftBodyLarge");
			this.storeTextBoxGroup = base.GetElement<UXElement>("NpcDialog");
			this.storeTitleLabel = base.GetElement<UXLabel>("LabelNpcMessageBottomLeftTitle");
			this.storeBodyLabel = base.GetElement<UXLabel>("LabelNpcMessageBottomLeftBody");
			this.HideAllElements();
		}

		public override void SetupRootCollider()
		{
		}

		public void HideAllElements()
		{
			this.regularTextBoxGroup.Visible = false;
			this.storeTextBoxGroup.Visible = false;
			this.nextButton.Visible = false;
			this.infoPanel.Visible = false;
			this.infoLabel.Visible = false;
			this.infoTitleLabel.Visible = false;
			this.storeButton.Visible = false;
		}

		public void ShowInfoPanel(string graphicId, string text, string title, bool planetPanel)
		{
			this.infoPanel.Visible = true;
			if (planetPanel)
			{
				this.infoPlanetTexture.LoadTexture(graphicId);
				this.infoPlanetTexture.Visible = true;
				this.infoTexture.Visible = false;
			}
			else
			{
				this.infoTexture.LoadTexture(graphicId);
				this.infoTexture.Visible = true;
				this.infoPlanetTexture.Visible = false;
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.infoLabel.Visible = true;
				this.infoLabel.Text = this.lang.Get(text, new object[0]);
			}
			else
			{
				this.infoLabel.Visible = false;
			}
			if (!string.IsNullOrEmpty(title))
			{
				this.infoTitleLabel.Visible = true;
				this.infoTitleLabel.Text = this.lang.Get(title, new object[0]);
			}
			else
			{
				this.infoTitleLabel.Visible = false;
			}
		}

		public void HideInfoPanel()
		{
			this.infoPanel.Visible = false;
			this.infoLabel.Visible = false;
			this.infoTitleLabel.Visible = false;
			this.DestroyIfEmpty();
		}

		public bool CharacterAlreadyShowing(string characterId)
		{
			return this.currentCharacter != null && this.currentCharacter.CharacterId == characterId;
		}

		public void ShowHoloCharacter(string characterId)
		{
			if (!base.IsLoaded())
			{
				Service.Logger.ErrorFormat("Cannot display {0} because screen is not loaded yet!", new object[]
				{
					characterId
				});
				return;
			}
			Service.UXController.HUD.Visible = false;
			if (this.currentCharacter != null)
			{
				this.currentCharacter.ChangeCharacter(characterId);
			}
			else
			{
				Vector3 vector = this.holoPositioner.LocalPosition;
				vector = base.UXCamera.Camera.ScreenToViewportPoint(vector);
				this.currentCharacter = new HoloCharacter(characterId, vector);
			}
		}

		public void PlayHoloAnimation(string animName)
		{
			if (this.currentCharacter != null)
			{
				this.currentCharacter.Animate(animName);
			}
			else
			{
				Service.Logger.ErrorFormat("There is no character currently on screen.", new object[0]);
			}
		}

		public void CloseAndDestroyHoloCharacter()
		{
			if (this.currentCharacter != null)
			{
				this.currentCharacter.CloseAndDestroy(new HolocommScreen.HoloCallback(this.OnCharacterAnimatedAway));
			}
		}

		private void OnCharacterAnimatedAway()
		{
			this.currentCharacter = null;
			this.DestroyIfEmpty();
		}

		private void DestroyCharacterImmediately()
		{
			if (this.currentCharacter != null)
			{
				this.currentCharacter.Destroy();
				this.currentCharacter = null;
			}
		}

		public void AddDialogue(string text, string title)
		{
			UXLabel uXLabel;
			UXElement uXElement;
			UXLabel uXLabel2;
			this.FindLabelAndPanelForSide(out uXLabel, out uXElement, out uXLabel2);
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = this.lang.Get(text, new object[0]);
				uXLabel.Text = ((!string.IsNullOrEmpty(text2)) ? text2 : text);
				uXElement.Visible = true;
			}
			if (!string.IsNullOrEmpty(title))
			{
				string text3 = this.lang.Get(title, new object[0]);
				uXLabel2.Text = text3;
				uXElement.Visible = true;
			}
		}

		public void RemoveDialogue()
		{
			UXLabel uXLabel;
			UXElement uXElement;
			UXLabel uXLabel2;
			this.FindLabelAndPanelForSide(out uXLabel, out uXElement, out uXLabel2);
			uXLabel.Text = string.Empty;
			uXLabel2.Text = string.Empty;
			uXElement.Visible = false;
			this.DestroyIfEmpty();
		}

		private void FindLabelAndPanelForSide(out UXLabel label, out UXElement panel, out UXLabel title)
		{
			if (this.storeButton.Visible)
			{
				label = this.storeBodyLabel;
				panel = this.storeTextBoxGroup;
				title = this.storeTitleLabel;
			}
			else
			{
				label = this.regularBodyLabel;
				panel = this.regularTextBoxGroup;
				title = this.regularTitleLabel;
			}
		}

		public void ShowButton(string buttonType)
		{
			UXButton uXButton = this.nextButton;
			if (buttonType == "BtnNext")
			{
				this.storeButton.Visible = false;
				uXButton = this.nextButton;
			}
			else if (buttonType == "ButtonStore")
			{
				this.nextButton.Visible = false;
				uXButton = this.storeButton;
			}
			uXButton.Visible = true;
		}

		public void OnNextButtonClicked(UXButton button)
		{
			Service.EventManager.SendEvent(EventId.StoryNextButtonClicked, null);
		}

		public Camera GetHoloCamera()
		{
			if (this.currentCharacter != null)
			{
				return this.currentCharacter.Camera;
			}
			return null;
		}

		private void DestroyIfEmpty()
		{
			if (this.currentCharacter == null && !this.storeTextBoxGroup.Visible && !this.regularTextBoxGroup.Visible && !this.infoPanel.Visible)
			{
				base.DestroyScreen();
			}
		}

		public override void OnDestroyElement()
		{
			Service.EventManager.UnregisterObserver(this, EventId.ShowHologramComplete);
			Service.UserInputManager.UnregisterObserver(this, UserInputLayer.Screen);
			Service.EventManager.SendEvent(EventId.HoloCommScreenDestroyed, null);
			this.DestroyCharacterImmediately();
			base.OnDestroyElement();
		}

		public EatResponse OnPress(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			if (!base.IsLoaded() || Service.ScreenController.GetHighestLevelScreen<ScreenBase>() != this)
			{
				return EatResponse.NotEaten;
			}
			if (this.nextButton.Visible)
			{
				this.OnNextButtonClicked(this.nextButton);
				return EatResponse.Eaten;
			}
			if (this.storeButton.Visible)
			{
				this.OnNextButtonClicked(this.nextButton);
				return EatResponse.Eaten;
			}
			return EatResponse.NotEaten;
		}

		public EatResponse OnDrag(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			return EatResponse.NotEaten;
		}

		public EatResponse OnRelease(int id)
		{
			return EatResponse.NotEaten;
		}

		public EatResponse OnScroll(float delta, Vector2 screenPosition)
		{
			return EatResponse.NotEaten;
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ShowHologramComplete)
			{
				Service.Engine.ForceGarbageCollection(null);
			}
			return base.OnEvent(id, cookie);
		}
	}
}
