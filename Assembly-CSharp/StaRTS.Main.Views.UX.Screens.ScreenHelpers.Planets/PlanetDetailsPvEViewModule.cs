using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers.Planets
{
	public class PlanetDetailsPvEViewModule : AbstractPlanetDetailsViewModule
	{
		private const string CHAPTER_ACTION_BUTTON = "BtnChapterAction";

		private const string CHAPTER_ACTION_BUTTON_BACKGROUND = "SpriteBkgBtnChapterAction";

		private const string CHAPTER_ACTION_BUTTON_LABEL = "LabelBtnChapterAction";

		private const string CHAPTER_ACTION_BUTTON_JEWEL_LABEL = "LabelMessageCountChapter";

		private const string CHAPTER_BACK_BUTTON = "BtnBackChapter";

		private const string CHAPTER_SELECTION_PBAR = "ChapterSelectionPbar";

		private const string CHAPTER_NUMBER_LABEL = "LabelChapterNumber";

		private const string CHAPTER_SELECTION_PROGRESS_LABEL = "LabelChapterSelectionProgress";

		private const string CHAPTER_SELECTION_STARS_LABEL = "LabelChapterSelectionStars";

		private const string CHAPTER_STARS_SPRITE = "SpriteChapterSelectionStar";

		private const string CHAPTER_TITLE_LABEL = "LabelChapterTitle";

		private const string LABEL_OPERATIONS_LOCKED = "LabelOperationsLocked";

		private const string MESH_PVE_IMAGE = "MeshPVEImage";

		private const string CHAPTER_JEWEL = "ContainerJewelChapter";

		private const string ALTERNATE_CHAPTER_BUTTON = "BtnChapterActionTop";

		private const string ALTERNATE_CHAPTER_BACKGROUND = "TextureChapterBg";

		private const string ALTERNATE_CHAPTER_LABEL = "LabelBtnChapterActionTop";

		private const string ALTERNATE_CHAPTER_GROUP = "ChapterButtonTop";

		private const string ALTERNATE_CHAPTER_JEWEL = "ContainerJewelChapterTop";

		private const string ALTERNATE_CHAPTER_BG_PREFIX = "chap_btn_";

		private const string CAMPAIGN_STARS_STRING = "LABEL_CAMPAIGN_STARS";

		private const string CHAPTER_NUMBER_STRING = "CHAPTER_NUMBER";

		private const string COMING_SOON_STRING = "Planets_Chapter_Coming_Soon";

		private const string CONTINUE_STRING = "CONTINUE";

		private const string PERCENT_COMPLETE_STRING = "PERCENT_COMPLETE";

		private const string PREVIEW_STRING = "Planets_Chapter_Preview";

		private const string CHAPTERS_STRING = "LABEL_CAMPAIGNS";

		private const string BUTTON_BLUE_BACKGROUND = "BtnBlue";

		private const string PVE_COMING_SOON_PREFIX = "PlanetPveComingSoon-";

		private UXElement chapterJewel;

		private UXElement alternateGroup;

		private UXLabel chapterLabel;

		private UXLabel chapterActionButtonLabel;

		private UXLabel nameLabel;

		private UXLabel nameLabelOperationsLocked;

		private UXLabel progressLabel;

		private UXLabel starsLabel;

		private UXSlider progressSlider;

		private UXMeshRenderer pveMeshTexture;

		private UXButton pveContinueButton;

		private UXButton alternateButton;

		private UXSprite chapterButtonBackground;

		private UXSprite chapterStarsSprite;

		private UXTexture alternateBackground;

		public PlanetDetailsPvEViewModule(PlanetDetailsScreen screen) : base(screen)
		{
		}

		public void OnScreenLoaded()
		{
			this.chapterActionButtonLabel = this.screen.GetElement<UXLabel>("LabelBtnChapterAction");
			this.pveMeshTexture = this.screen.GetElement<UXMeshRenderer>("MeshPVEImage");
			this.chapterJewel = this.screen.GetElement<UXElement>("ContainerJewelChapter");
			this.chapterButtonBackground = this.screen.GetElement<UXSprite>("SpriteBkgBtnChapterAction");
			this.chapterStarsSprite = this.screen.GetElement<UXSprite>("SpriteChapterSelectionStar");
			this.chapterLabel = this.screen.GetElement<UXLabel>("LabelChapterNumber");
			this.nameLabel = this.screen.GetElement<UXLabel>("LabelChapterTitle");
			this.nameLabel.Visible = false;
			this.nameLabelOperationsLocked = this.screen.GetElement<UXLabel>("LabelOperationsLocked");
			this.nameLabelOperationsLocked.Visible = false;
			this.progressLabel = this.screen.GetElement<UXLabel>("LabelChapterSelectionProgress");
			this.starsLabel = this.screen.GetElement<UXLabel>("LabelChapterSelectionStars");
			this.progressSlider = this.screen.GetElement<UXSlider>("ChapterSelectionPbar");
			this.pveContinueButton = this.screen.GetElement<UXButton>("BtnChapterAction");
			this.pveContinueButton.Enabled = true;
			CampaignVO highestUnlockedCampaign = GameUtils.GetHighestUnlockedCampaign();
			this.alternateBackground = this.screen.GetElement<UXTexture>("TextureChapterBg");
			this.alternateBackground.LoadTexture("chap_btn_" + highestUnlockedCampaign.Uid);
			this.alternateButton = this.screen.GetElement<UXButton>("BtnChapterActionTop");
			this.alternateButton.Enabled = true;
			this.alternateButton.OnClicked = new UXButtonClickedDelegate(this.OnPveButtonClicked);
			this.screen.GetElement<UXLabel>("LabelBtnChapterActionTop").Text = Service.Lang.Get("LABEL_CAMPAIGNS", new object[0]);
			this.alternateGroup = this.screen.GetElement<UXElement>("ChapterButtonTop");
			UXElement element = this.screen.GetElement<UXElement>("ContainerJewelChapterTop");
			element.Visible = false;
			this.RefreshScreenForPlanetChange();
		}

		public void RefreshScreenForPlanetChange()
		{
			bool flag = this.IsViewingDefaultPlanet();
			bool flag2 = this.IsBasedOnDefaultPlanet();
			bool flag3 = Service.ObjectiveController.ShouldShowObjectives();
			CampaignVO highestUnlockedCampaign = GameUtils.GetHighestUnlockedCampaign();
			if (flag3 || !flag || highestUnlockedCampaign == null)
			{
				this.chapterLabel.Visible = false;
				this.nameLabelOperationsLocked.Visible = false;
				this.chapterStarsSprite.Visible = false;
				this.chapterJewel.Visible = false;
				this.pveMeshTexture.LoadTexture("PlanetPanelLocked");
				this.pveMeshTexture.SetShader("Unlit/Premultiplied Colored");
				this.progressSlider.Visible = false;
				this.progressLabel.Visible = false;
				this.starsLabel.Visible = false;
				this.pveContinueButton.Visible = false;
				this.alternateGroup.Visible = flag;
			}
			else
			{
				this.chapterLabel.Visible = true;
				this.chapterStarsSprite.Visible = false;
				this.chapterJewel.Visible = false;
				this.progressSlider.Visible = true;
				this.progressLabel.Visible = true;
				this.starsLabel.Visible = false;
				this.pveContinueButton.Visible = true;
				this.alternateGroup.Visible = flag3;
				this.chapterLabel.Text = LangUtils.GetCampaignTitle(highestUnlockedCampaign);
				int totalCampaignMissionsCompleted = base.Player.CampaignProgress.GetTotalCampaignMissionsCompleted(highestUnlockedCampaign);
				int totalMissions = highestUnlockedCampaign.TotalMissions;
				this.progressSlider.Value = ((totalMissions != 0) ? ((float)totalCampaignMissionsCompleted / (float)totalMissions) : 0f);
				this.progressLabel.Text = base.LangController.Get("PERCENT_COMPLETE", new object[]
				{
					(int)Mathf.Floor(this.progressSlider.Value * 100f)
				});
				this.starsLabel.Text = base.LangController.Get("LABEL_CAMPAIGN_STARS", new object[]
				{
					base.Player.CampaignProgress.GetTotalCampaignStarsEarned(highestUnlockedCampaign),
					highestUnlockedCampaign.TotalMasteryStars
				});
				this.pveMeshTexture.LoadTexture(highestUnlockedCampaign.Uid);
				this.pveMeshTexture.SetShader("Unlit/Premultiplied Colored");
				this.pveContinueButton.OnClicked = new UXButtonClickedDelegate(this.OnPveButtonClicked);
				if (flag && flag2)
				{
					this.chapterActionButtonLabel.Text = base.LangController.Get("CONTINUE", new object[0]);
				}
				else
				{
					this.chapterActionButtonLabel.Text = base.LangController.Get("Planets_Chapter_Preview", new object[0]);
					this.chapterButtonBackground.SpriteName = "BtnBlue";
				}
			}
		}

		private bool IsViewingDefaultPlanet()
		{
			return this.screen.viewingPlanetVO.Uid.Equals("planet1");
		}

		private bool IsBasedOnDefaultPlanet()
		{
			return Service.CurrentPlayer.PlanetId.Equals("planet1");
		}

		private void OnPveButtonClicked(UXButton button)
		{
			this.screen.currentSection = CampaignScreenSection.PvE;
			this.screen.AnimateHideUI();
			base.CampController.HasNewChapterMission = false;
			CampaignVO highestUnlockedCampaign = GameUtils.GetHighestUnlockedCampaign();
			if (highestUnlockedCampaign != null)
			{
				this.screen.SelectCampaign(highestUnlockedCampaign);
			}
			string planetStatus = Service.CurrentPlayer.GetPlanetStatus(this.screen.viewingPlanetVO.Uid);
			base.EvtManager.SendEvent(EventId.UIAttackScreenSelection, new ActionMessageBIData("PvE", planetStatus));
		}

		public void OnClose()
		{
			this.pveMeshTexture.Visible = false;
		}
	}
}
