using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.Leaderboard;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class SquadWarStartScreen : ClosableScreen
	{
		private const string TITLE_LABEL = "LabelTitle";

		private const string DESCRIPTION_LABEL = "LabelSquadWar";

		private const string NEXT_BUTTON = "BtnNext";

		private const string INFO_BUTTON = "BtnInfo";

		private const string NEXT_BUTTON_LABEL = "LabelBtnNext";

		private const string IMAGE_HOLDER = "SpriteNextImage";

		private const string WAR_START_TEXTURE_ASSET = "gui_textures_squadwar_start";

		private const string TITLE = "WAR_START_TITLE";

		private const string DESCRIPTION_OFFICER = "WAR_START_DESCRIPTION_OFFICER";

		private const string DESCRIPTION_OFFICER_INELIGIBLE = "WAR_START_DESCRIPTION_OFFICER_INELIGIBLE";

		private const string DESCRIPTION_NONOFFICER = "WAR_START_DESCRIPTION_NONOFFICER";

		private const string DESCRIPTION_NOSQUAD = "WAR_START_DESCRIPTION_NOSQUAD";

		private const string BUTTON_TEXT_OFFICER = "WAR_START_BUTTON_TEXT";

		private const string BUTTON_TEXT_NONOFFICER = "WAR_START_BUTTON_TEXT_NONOFFICER";

		private const string BUTTON_TEXT_NOSQUAD = "WAR_START_BUTTON_TEXT_NOSQUAD";

		protected override bool WantTransitions
		{
			get
			{
				return false;
			}
		}

		public SquadWarStartScreen() : base("gui_squadwar_start")
		{
		}

		protected override void OnScreenLoaded()
		{
			SquadController squadController = Service.SquadController;
			bool flag = squadController.StateManager.GetCurrentSquad() != null;
			SquadRole role = squadController.StateManager.Role;
			bool flag2 = role == SquadRole.Owner || role == SquadRole.Officer;
			int highestLevelHQ = Service.BuildingLookupController.GetHighestLevelHQ();
			this.InitButtons();
			UXLabel element = base.GetElement<UXLabel>("LabelTitle");
			element.Text = this.lang.Get("WAR_START_TITLE", new object[0]);
			string id;
			string id2;
			if (!flag)
			{
				id = "WAR_START_DESCRIPTION_NOSQUAD";
				id2 = "WAR_START_BUTTON_TEXT_NOSQUAD";
			}
			else if (flag2)
			{
				if (highestLevelHQ >= GameConstants.WAR_PARTICIPANT_MIN_LEVEL)
				{
					id = "WAR_START_DESCRIPTION_OFFICER";
					id2 = "WAR_START_BUTTON_TEXT";
				}
				else
				{
					id = "WAR_START_DESCRIPTION_OFFICER_INELIGIBLE";
					id2 = "WAR_START_BUTTON_TEXT_NONOFFICER";
				}
			}
			else
			{
				this.LogMemberAttemptingWarStart();
				id = "WAR_START_DESCRIPTION_NONOFFICER";
				id2 = "WAR_START_BUTTON_TEXT_NONOFFICER";
			}
			UXLabel element2 = base.GetElement<UXLabel>("LabelSquadWar");
			element2.Text = this.lang.Get(id, new object[]
			{
				highestLevelHQ
			});
			TextureVO optional = Service.StaticDataController.GetOptional<TextureVO>("gui_textures_squadwar_start");
			if (optional != null)
			{
				UXTexture element3 = base.GetElement<UXTexture>("SpriteNextImage");
				element3.LoadTexture(optional.AssetName);
			}
			UXLabel element4 = base.GetElement<UXLabel>("LabelBtnNext");
			element4.Text = this.lang.Get(id2, new object[0]);
			UXButton element5 = base.GetElement<UXButton>("BtnNext");
			element5.OnClicked = new UXButtonClickedDelegate(this.OnNextButtonClicked);
			UXButton element6 = base.GetElement<UXButton>("BtnInfo");
			element6.OnClicked = new UXButtonClickedDelegate(squadController.WarManager.ShowInfoScreen);
		}

		private void LogMemberAttemptingWarStart()
		{
			SquadController squadController = Service.SquadController;
			Squad currentSquad = squadController.StateManager.GetCurrentSquad();
			string squadID = currentSquad.SquadID;
			int memberCount = currentSquad.MemberCount;
			int activeMemberCount = currentSquad.ActiveMemberCount;
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			int highestLevelHQ = buildingLookupController.GetHighestLevelHQ();
			Service.BILoggingController.TrackGameAction(highestLevelHQ.ToString(), "UI_squadwar_askforwar", string.Concat(new object[]
			{
				squadID,
				"|",
				memberCount,
				"|",
				activeMemberCount
			}), null);
		}

		private void OnNextButtonClicked(UXButton button)
		{
			this.Close(null);
			SquadController squadController = Service.SquadController;
			bool flag = squadController.StateManager.GetCurrentSquad() != null;
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			int highestLevelHQ = buildingLookupController.GetHighestLevelHQ();
			bool flag2 = SquadUtils.CanStartMatchmakingPrep(squadController, buildingLookupController);
			if (!flag)
			{
				Service.BILoggingController.TrackGameAction(highestLevelHQ.ToString(), "UI_squadwar_joinsquad", ServerTime.Time.ToString(), null);
				Service.ScreenController.AddScreen(new SquadJoinScreen());
			}
			else if (flag2)
			{
				squadController.WarManager.StartMatchMakingPreparation();
			}
		}
	}
}
