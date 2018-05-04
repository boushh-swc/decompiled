using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Squads
{
	public class SquadWarFlyout : AbstractSquadWarBoardElement
	{
		private SquadWarParticipantState opponentState;

		private SquadWarBuffBaseData buffBaseData;

		private const string SHOW_TOP = "ShowTop";

		private const string SHOW_BOTTOM = "ShowBottom";

		private const string OFF = "Off";

		public const string TOP_OPTION_1_BUTTON = "BtnOption1Top";

		private const string TOP_OPTION_1_LABEL = "LabelOption1Top";

		public const string TOP_OPTION_2_BUTTON = "BtnOption2Top";

		private const string TOP_OPTION_2_LABEL = "LabelOption2Top";

		public const string BOTTOM_OPTION_1_BUTTON = "BtnOption1Bottom";

		private const string BOTTOM_OPTION_1_LABEL = "LabelOption1Bottom";

		public const string BOTTOM_OPTION_2_BUTTON = "BtnOption2Bottom";

		private const string BOTTOM_OPTION_2_LABEL = "LabelOption2Bottom";

		private const string INFO = "context_Info";

		private const string SCOUT = "SCOUT";

		private UXButton scoutMemberButton;

		private UXLabel scoutMemberLabel;

		private UXButton scoutBuffBaseButton;

		private UXLabel scoutBuffBaseLabel;

		public SquadWarFlyout() : base("gui_squadwar_flyout")
		{
		}

		protected override void OnScreenLoaded(object cookie)
		{
			base.OnScreenLoaded(cookie);
			base.InitAnimator();
			Lang lang = Service.Lang;
			string text = lang.Get("context_Info", new object[0]);
			string text2 = lang.Get("SCOUT", new object[0]);
			this.scoutMemberLabel = base.GetElement<UXLabel>("LabelOption1Top");
			this.scoutMemberLabel.Text = text2;
			this.scoutMemberButton = base.GetElement<UXButton>("BtnOption1Top");
			this.scoutMemberButton.OnClicked = new UXButtonClickedDelegate(this.OnScoutMemberClicked);
			base.GetElement<UXLabel>("LabelOption2Top").Text = text;
			base.GetElement<UXButton>("BtnOption2Top").OnClicked = new UXButtonClickedDelegate(this.OnMemberInfoClicked);
			this.scoutBuffBaseLabel = base.GetElement<UXLabel>("LabelOption1Bottom");
			this.scoutBuffBaseLabel.Text = text2;
			this.scoutBuffBaseButton = base.GetElement<UXButton>("BtnOption1Bottom");
			this.scoutBuffBaseButton.OnClicked = new UXButtonClickedDelegate(this.OnScoutBuffBaseClicked);
			base.GetElement<UXLabel>("LabelOption2Bottom").Text = text;
			base.GetElement<UXButton>("BtnOption2Bottom").OnClicked = new UXButtonClickedDelegate(this.OnBuffBaseInfoClicked);
		}

		public bool IsShowingParticipantOptions(SquadWarParticipantState state)
		{
			return state != null && this.opponentState != null && this.opponentState.SquadMemberId == state.SquadMemberId;
		}

		public void ShowParticipantOptions(GameObject obj, SquadWarParticipantState state)
		{
			if (this.opponentState != null && this.opponentState == state)
			{
				return;
			}
			this.opponentState = state;
			this.buffBaseData = null;
			this.Visible = true;
			this.transformToTrack = obj.transform;
			this.animator.Play("Off", 0, 1f);
			this.animator.ResetTrigger("Off");
			this.animator.ResetTrigger("ShowBottom");
			this.animator.SetTrigger("ShowTop");
			SquadWarManager warManager = Service.SquadController.WarManager;
			string empty = string.Empty;
			if (warManager.CanScoutWarMember(this.opponentState.SquadMemberId, ref empty))
			{
				this.scoutMemberButton.VisuallyEnableButton();
				this.scoutMemberLabel.TextColor = this.scoutBuffBaseLabel.OriginalTextColor;
			}
			else
			{
				this.scoutMemberButton.VisuallyDisableButton();
				this.scoutMemberLabel.TextColor = UXUtils.COLOR_LABEL_DISABLED;
			}
			this.PlayShowAudioClip();
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		public bool IsShowingBuffBaseOptions(SquadWarBuffBaseData data)
		{
			return data != null && this.buffBaseData != null && this.buffBaseData.BuffBaseId == data.BuffBaseId;
		}

		public void ShowBuffBaseOptions(UXCheckbox checkbox, SquadWarBuffBaseData data)
		{
			if (this.buffBaseData != null && this.buffBaseData == data)
			{
				return;
			}
			this.buffBaseData = data;
			this.opponentState = null;
			this.Visible = true;
			this.transformToTrack = null;
			Vector3[] worldCorners = checkbox.GetWorldCorners();
			Vector3 position = checkbox.Root.transform.position;
			if (worldCorners != null)
			{
				position.y = worldCorners[0].y;
			}
			this.rootTrans.position = position;
			this.animator.Play("Off", 0, 1f);
			this.animator.ResetTrigger("Off");
			this.animator.ResetTrigger("ShowTop");
			this.animator.SetTrigger("ShowBottom");
			SquadWarManager warManager = Service.SquadController.WarManager;
			string empty = string.Empty;
			if (warManager.CanScoutBuffBase(this.buffBaseData, ref empty))
			{
				this.scoutBuffBaseButton.VisuallyEnableButton();
				this.scoutBuffBaseLabel.TextColor = this.scoutBuffBaseLabel.OriginalTextColor;
			}
			else
			{
				this.scoutBuffBaseButton.VisuallyDisableButton();
				this.scoutBuffBaseLabel.TextColor = UXUtils.COLOR_LABEL_DISABLED;
			}
			this.PlayShowAudioClip();
		}

		private void PlayShowAudioClip()
		{
			Service.AudioManager.PlayAudio("sfx_ui_squadwar_flyout");
		}

		public void Hide()
		{
			this.opponentState = null;
			this.buffBaseData = null;
			if (this.Visible)
			{
				this.animator.ResetTrigger("ShowTop");
				this.animator.ResetTrigger("ShowBottom");
				this.animator.SetTrigger("Off");
			}
			this.Visible = false;
			Service.EventManager.SendEvent(EventId.WarBoardFlyoutHidden, null);
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
		}

		public override void Destroy()
		{
			base.Destroy();
			this.animator = null;
			this.opponentState = null;
			this.buffBaseData = null;
		}

		private void OnScoutMemberClicked(UXButton button)
		{
			if (this.opponentState != null)
			{
				SquadWarManager warManager = Service.SquadController.WarManager;
				string empty = string.Empty;
				if (warManager.CanScoutWarMember(this.opponentState.SquadMemberId, ref empty))
				{
					string squadMemberId = this.opponentState.SquadMemberId;
					if (warManager.ScoutWarMember(squadMemberId))
					{
						this.Hide();
					}
				}
				else
				{
					Service.UXController.MiscElementsManager.ShowPlayerInstructions(empty);
				}
			}
		}

		private void OnScoutBuffBaseClicked(UXButton button)
		{
			if (this.buffBaseData != null)
			{
				SquadWarManager warManager = Service.SquadController.WarManager;
				string empty = string.Empty;
				if (warManager.CanScoutBuffBase(this.buffBaseData, ref empty))
				{
					string buffBaseId = this.buffBaseData.BuffBaseId;
					if (Service.SquadController.WarManager.ScoutBuffBase(buffBaseId))
					{
						this.Hide();
					}
				}
				else
				{
					Service.UXController.MiscElementsManager.ShowPlayerInstructions(empty);
				}
			}
		}

		private void OnMemberInfoClicked(UXButton button)
		{
			Service.ScreenController.AddScreen(new SquadWarPlayerDetailsScreen(this.opponentState));
			this.Hide();
		}

		private void OnBuffBaseInfoClicked(UXButton button)
		{
			Service.ScreenController.AddScreen(new SquadWarBuffBaseDetails(this.buffBaseData));
			this.Hide();
		}

		private void CloseWarScreen()
		{
			SquadWarScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<SquadWarScreen>();
			if (highestLevelScreen != null)
			{
				highestLevelScreen.Close(null);
			}
		}

		public override void OnViewFrameTime(float dt)
		{
			if (this.buffBaseData == null && this.transformToTrack != null)
			{
				base.OnViewFrameTime(dt);
			}
		}
	}
}
