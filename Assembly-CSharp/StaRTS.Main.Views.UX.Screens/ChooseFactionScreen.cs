using StaRTS.Externals.Manimal;
using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.World;
using StaRTS.Main.Controllers.World.Transitions;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class ChooseFactionScreen : ScreenBase
	{
		private const string LABEL_TITLE = "LabelTitle";

		private const string LABEL_TITLE_NO_FB = "LabelTitleNoFB";

		private const string LABEL_LEFT = "LabelChoiceLeft";

		private const string LABEL_RIGHT = "LabelChoiceRight";

		private const string LABEL_FB_INCENTIVE = "LabelFacebookConnect";

		private const string LABEL_CRYSTALS = "LabelCrystals";

		private const string LABEL_CRYSTAL_COUNT = "LabelCrystalCount";

		private const string LABEL_BUTTON_FACEBOOK = "LabelFacebook";

		private const string ICON_CRYSTAL = "SpriteIcoCrystal";

		private const string BUTTON_LEFT = "ButtonSelectLeft";

		private const string BUTTON_RIGHT = "ButtonSelectRight";

		private const string BUTTON_LABEL_LEFT = "LabelButtonLeft";

		private const string BUTTON_LABEL_RIGHT = "LabelButtonRight";

		private const string BUTTON_FACEBOOK = "BtnFacebook";

		private const string GROUP_FACEBOOK_LOGIN = "FacebookConnect";

		private const string GROUP_FACEBOOK_FREINDS = "FriendPics";

		private const string GRID_FACEBOOK_FRIENDS_EMPIRE = "GridEmpireFriends";

		private const string GRID_FACEBOOK_FRIENDS_REBEL = "GridRebelFriends";

		private const string FRIEND_REBEL_TEMPLATE = "FriendRebelThumb";

		private const string FRIEND_REBEL_PIC = "FriendRebelPic";

		private const string FRIEND_EMPIRE_TEMPLATE = "FriendEmpireThumb";

		private const string FRIEND_EMPIRE_PIC = "FriendEmpirePic";

		private const string DIALOGBG_FACEBOOK = "BgDialogFB";

		private const string DIALOGBG = "BgDialog";

		private const string LEFT_SECTION = "ChoiceLeft";

		private const string RIGHT_SECTION = "ChoiceRight";

		private const string FACEBOOK = "FBFriends";

		private const string TEXTURE_HOLDER_EMPIRE = "SpriteEmpireImage";

		private const string TEXTURE_HOLDER_REBEL = "SpriteRebelImage";

		private const string TEXTURE_EMPIRE = "FactionEmpire";

		private const string TEXTURE_REBEL = "FactionRebel";

		private List<Texture2D> loadedFBTextures;

		private UXGrid empireGrid;

		private UXGrid rebelGrid;

		[CompilerGenerated]
		private static Comparison<BuildingTypeVO> <>f__mg$cache0;

		public ChooseFactionScreen() : base("gui_faction_choice")
		{
			this.loadedFBTextures = new List<Texture2D>();
		}

		protected override void OnScreenLoaded()
		{
			this.InitLabels();
			this.InitButtons();
			this.InitTextures();
			this.HideFacebookIfNeeded();
		}

		private void InitLabels()
		{
			UXLabel element = base.GetElement<UXLabel>("LabelFacebookConnect");
			element.Text = this.lang.Get("CONNECT_FB_FACTION_DESC", new object[0]);
			UXLabel element2 = base.GetElement<UXLabel>("LabelFacebook");
			element2.Text = this.lang.Get("SETTINGS_NOTCONNECTED", new object[0]);
		}

		private void InitButtons()
		{
			UXButton element = base.GetElement<UXButton>("ButtonSelectLeft");
			element.Tag = FactionType.Empire;
			element.OnClicked = new UXButtonClickedDelegate(this.onChooseFaction);
			UXButton element2 = base.GetElement<UXButton>("ButtonSelectRight");
			element2.Tag = FactionType.Rebel;
			element2.OnClicked = new UXButtonClickedDelegate(this.onChooseFaction);
			Service.UserInputInhibitor.AddToAllow(element);
			Service.UserInputInhibitor.AddToAllow(element2);
			base.GetElement<UXButton>("BtnFacebook").OnClicked = new UXButtonClickedDelegate(this.OnFacebookConnect);
			if (Service.EnvironmentController.IsRestrictedProfile())
			{
				base.GetElement<UXElement>("FacebookConnect").Visible = false;
				base.GetElement<UXElement>("FriendPics").Visible = false;
			}
			else
			{
				this.ShowFacebookCallToAction(!Service.ISocialDataController.IsLoggedIn);
			}
		}

		private void InitTextures()
		{
			base.GetElement<UXTexture>("SpriteEmpireImage").LoadTexture("FactionEmpire");
			base.GetElement<UXTexture>("SpriteRebelImage").LoadTexture("FactionRebel");
		}

		private void HideFacebookIfNeeded()
		{
			UXElement element = base.GetElement<UXElement>("BgDialog");
			bool nO_FB_FACTION_CHOICE_ANDROID = GameConstants.NO_FB_FACTION_CHOICE_ANDROID;
			UXLabel element2;
			if (nO_FB_FACTION_CHOICE_ANDROID)
			{
				base.GetElement<UXElement>("FBFriends").Visible = false;
				base.GetElement<UXElement>("BgDialogFB").Visible = false;
				element.Visible = true;
				Vector3 b = new Vector3(0f, -element.Height / 4f, 0f);
				element.LocalPosition += b;
				base.GetElement<UXElement>("ChoiceLeft").LocalPosition += b;
				base.GetElement<UXElement>("ChoiceRight").LocalPosition += b;
				element2 = base.GetElement<UXLabel>("LabelTitle");
				element2.Visible = false;
				element2 = base.GetElement<UXLabel>("LabelTitleNoFB");
			}
			else
			{
				element.Visible = false;
				element2 = base.GetElement<UXLabel>("LabelTitleNoFB");
				element2.Visible = false;
				element2 = base.GetElement<UXLabel>("LabelTitle");
			}
			element2.Text = this.lang.Get("CHOOSE_FACTION_TITLE", new object[0]);
		}

		private void OnFacebookConnect(UXButton button)
		{
			Service.AudioManager.PlayAudio("sfx_button_facebook");
			Service.ISocialDataController.Login(new OnAllDataFetchedDelegate(this.OnFacebookLoggedIn));
		}

		private void OnFacebookLoggedIn()
		{
			this.ShowFacebookCallToAction(false);
			Service.EventManager.SendEvent(EventId.FacebookLoggedIn, "UI_faction_choice");
		}

		private void ShowFacebookCallToAction(bool show)
		{
			base.GetElement<UXElement>("FacebookConnect").Visible = show;
			base.GetElement<UXElement>("FriendPics").Visible = !show;
			if (show)
			{
				UXLabel element = base.GetElement<UXLabel>("LabelCrystalCount");
				UXLabel element2 = base.GetElement<UXLabel>("LabelCrystals");
				if (Service.CurrentPlayer.IsConnectedAccount)
				{
					element.Visible = false;
					element2.Visible = false;
					base.GetElement<UXElement>("SpriteIcoCrystal").Visible = false;
				}
				else
				{
					element.Text = this.lang.Get("GET_AMOUNT", new object[]
					{
						GameConstants.FB_CONNECT_REWARD
					});
					element2.Text = this.lang.Get("CRYSTALS", new object[0]);
				}
			}
			else
			{
				ISocialDataController iSocialDataController = Service.ISocialDataController;
				iSocialDataController.FriendsDetailsCB = new OnFBFriendsDelegate(this.OnFBDataPopulated);
				Service.ISocialDataController.CheckFacebookLoginOnStartup();
			}
		}

		private void OnFBDataPopulated()
		{
			bool flag = false;
			ServerAPI serverAPI = Service.ServerAPI;
			if (!serverAPI.Enabled && Service.CurrentPlayer.CampaignProgress.FueInProgress)
			{
				serverAPI.Enabled = true;
				flag = true;
			}
			ISocialDataController iSocialDataController = Service.ISocialDataController;
			iSocialDataController.FriendsDetailsCB = new OnFBFriendsDelegate(this.RefreshFriendData);
			iSocialDataController.UpdateFriends();
			if (flag)
			{
				serverAPI.Enabled = false;
			}
		}

		private void RefreshFriendData()
		{
			if (!base.IsLoaded())
			{
				return;
			}
			List<SocialFriendData> friends = Service.ISocialDataController.Friends;
			this.empireGrid = base.GetElement<UXGrid>("GridEmpireFriends");
			this.rebelGrid = base.GetElement<UXGrid>("GridRebelFriends");
			this.empireGrid.SetTemplateItem("FriendEmpireThumb");
			this.rebelGrid.SetTemplateItem("FriendRebelThumb");
			this.empireGrid.Clear();
			this.rebelGrid.Clear();
			if (friends != null)
			{
				for (int i = 0; i < friends.Count; i++)
				{
					SocialFriendData socialFriendData = friends[i];
					if (socialFriendData.PlayerData != null)
					{
						FactionType faction = socialFriendData.PlayerData.Faction;
						UXButton uXButton = null;
						if (faction == FactionType.Empire)
						{
							uXButton = (UXButton)this.empireGrid.CloneTemplateItem(socialFriendData.Id);
							UXTexture subElement = this.empireGrid.GetSubElement<UXTexture>(socialFriendData.Id, "FriendEmpirePic");
							Service.ISocialDataController.GetFriendPicture(socialFriendData, new OnGetProfilePicture(this.OnGetFriendPicture), subElement);
							this.empireGrid.AddItem(uXButton, i);
						}
						else if (faction == FactionType.Rebel)
						{
							uXButton = (UXButton)this.rebelGrid.CloneTemplateItem(socialFriendData.Id);
							UXTexture subElement2 = this.rebelGrid.GetSubElement<UXTexture>(socialFriendData.Id, "FriendRebelPic");
							Service.ISocialDataController.GetFriendPicture(socialFriendData, new OnGetProfilePicture(this.OnGetFriendPicture), subElement2);
							this.rebelGrid.AddItem(uXButton, i);
						}
						if (uXButton != null)
						{
							uXButton.Tag = socialFriendData.PlayerData.PlayerName + "\n" + socialFriendData.Name;
							uXButton.OnPressed = new UXButtonPressedDelegate(this.OnPressFriendButton);
							uXButton.OnReleased = new UXButtonReleasedDelegate(this.OnReleaseFriendButton);
						}
					}
				}
			}
			this.empireGrid.RepositionItems();
			this.rebelGrid.RepositionItems();
		}

		private void OnPressFriendButton(UXButton button)
		{
			string localizedText = (string)button.Tag;
			Service.UXController.MiscElementsManager.ShowTroopTooltip(button, localizedText);
		}

		private void OnReleaseFriendButton(UXButton button)
		{
			Service.UXController.MiscElementsManager.HideTroopTooltip();
		}

		public override void OnDestroyElement()
		{
			ISocialDataController iSocialDataController = Service.ISocialDataController;
			for (int i = 0; i < this.loadedFBTextures.Count; i++)
			{
				iSocialDataController.DestroyFriendPicture(this.loadedFBTextures[i]);
			}
			this.loadedFBTextures = null;
			if (this.empireGrid != null)
			{
				this.empireGrid.Clear();
				this.empireGrid = null;
			}
			if (this.rebelGrid != null)
			{
				this.rebelGrid.Clear();
				this.rebelGrid = null;
			}
			base.OnDestroyElement();
		}

		private void OnGetFriendPicture(Texture2D tex, object cookie)
		{
			UXTexture uXTexture = cookie as UXTexture;
			uXTexture.MainTexture = tex;
			this.loadedFBTextures.Add(tex);
		}

		private void onChooseFaction(UXButton button)
		{
			if (GameConstants.FACTION_CHOICE_CONFIRM_SCREEN_ENABLED)
			{
				Service.ScreenController.AddScreen(new TwoButtonFueScreen(true, new OnScreenModalResult(this.OnConfrimFactionChoice), button.Tag, string.Empty, false));
			}
			else
			{
				this.OnConfrimFactionChoice(true, button.Tag);
			}
		}

		private void OnConfrimFactionChoice(object result, object cookie)
		{
			if (result == null)
			{
				return;
			}
			Service.AudioManager.PlayAudio("sfx_button_startbattle");
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			FactionType faction = (FactionType)cookie;
			currentPlayer.Faction = faction;
			Service.BILoggingController.TrackFaction();
			SetFactionRequest request = new SetFactionRequest(currentPlayer.Faction);
			SetFactionCommand setFactionCommand = new SetFactionCommand(request);
			setFactionCommand.AddSuccessCallback(new AbstractCommand<SetFactionRequest, DefaultResponse>.OnSuccessCallback(this.OnFactionServerSuccess));
			Service.ServerAPI.Sync(setFactionCommand);
			Service.CurrentPlayer.RemoveAllDeployables();
			Service.PostBattleRepairController.StopHealthRepairs();
			this.UpdateBuildings(currentPlayer.Faction);
			Service.WorldTransitioner.StartTransition(new WorldToWorldTransition(null, Service.HomeMapDataLoader, new TransitionCompleteDelegate(this.OnNewHomeTransitionedTo), false, true));
		}

		private void OnFactionServerSuccess(DefaultResponse response, object cookie)
		{
			Service.HolonetController.PrepareContent(null);
		}

		private void UpdateBuildings(FactionType newFaction)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			List<BuildingTypeVO> list = new List<BuildingTypeVO>();
			foreach (BuildingTypeVO current in staticDataController.GetAll<BuildingTypeVO>())
			{
				list.Add(current);
			}
			List<BuildingTypeVO> arg_69_0 = list;
			if (ChooseFactionScreen.<>f__mg$cache0 == null)
			{
				ChooseFactionScreen.<>f__mg$cache0 = new Comparison<BuildingTypeVO>(GameUtils.SortBuildingByUID);
			}
			arg_69_0.Sort(ChooseFactionScreen.<>f__mg$cache0);
			List<Building> buildings = Service.CurrentPlayer.Map.Buildings;
			for (int i = 0; i < buildings.Count; i++)
			{
				Building building = buildings[i];
				BuildingTypeVO buildingTypeVO = staticDataController.Get<BuildingTypeVO>(building.Uid);
				if (buildingTypeVO.Faction != FactionType.Neutral)
				{
					building.Uid = GameUtils.GetEquivalentFromPreSortedList(list, buildingTypeVO, newFaction);
				}
			}
			list.Clear();
			list = null;
		}

		private void OnNewHomeTransitionedTo()
		{
			Service.PlayerValuesController.RecalculateAll();
			Service.CampaignController.StartCampaignProgress();
		}
	}
}
