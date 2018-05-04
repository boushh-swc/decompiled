using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Chat;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.Squads
{
	public class SquadScreenWarLogView : AbstractSquadScreenViewModule, IEventObserver, IViewFrameTimeObserver
	{
		private const string CONTAINER_NAME = "WarlogContainer";

		private const string TAB_BUTTON_NAME = "SocialWarLogBtn";

		private const string LABEL_EMPTY_WARLOG = "LabelEmptyWarlog";

		private const string SQUAD_WAR_LOG_WIN = "SQUAD_WAR_LOG_WIN";

		private const string SQUAD_WAR_LOG_LOSS = "SQUAD_WAR_LOG_LOSS";

		private const string SQUAD_WAR_LOG_DRAW = "SQUAD_WAR_LOG_DRAW";

		private const string SQUAD_WAR_LOG_SCORE = "SQUAD_WAR_LOG_SCORE";

		private const string SQUAD_WAR_LOG_COLLECT_REWARD = "SQUAD_WAR_LOG_COLLECT_REWARD";

		private const string SQUAD_WAR_LOG_OPPONENT_VS = "SQUAD_WAR_LOG_OPPONENT_VS";

		private const string EXPIRES_IN = "expires_in";

		private const string TIME_AGO = "TIME_AGO";

		private const string SQUAD_WAR_LOG_EMPTY_HAS_REQUIREMENTS = "SQUAD_WAR_LOG_EMPTY_HAS_REQUIREMENTS";

		private const string SQUAD_WAR_LOG_EMPTY_LACKS_REQUIREMENTS = "SQUAD_WAR_LOG_EMPTY_LACKS_REQUIREMENTS";

		private const string GRID_WAR_LOG = "GridWarlog";

		private const string ITEM_WAR_LOG = "ItemWarlog";

		private const string BTN_COLLECT_WAR_REWARD = "BtnCollectWarReward";

		private const string LABEL_BTN_COLLECT_WAR_REWARD = "LabelBtnCollectWarReward";

		private const string LABEL_WAR_LOG_TIMESTAMP = "LabelWarlogTimestamp";

		private const string LABEL_WAR_LOG_SCORE = "LabelWarlogScore";

		private const string LABEL_WAR_LOG_RESULT = "LabelWarlogResult";

		private const string LABEL_WAR_LOG_OPPONENT = "LabelWarlogOpponent";

		private const string LABEL_WAR_LOG_EXPIRES = "LabelWarlogExpires";

		private const string SPRITE_WAR_LOG_BG_DRAW = "SpriteWarlogBgDraw";

		private const string SPRITE_WAR_LOG_BG_LOST = "SpriteWarlogBgLost";

		private const string SPRITE_WAR_LOG_BG_WON = "SpriteWarlogBgWon";

		private const string WAR_LOG = "warlog";

		private UXElement viewContainer;

		private UXCheckbox tabButton;

		private UXLabel labelEmptyWarlog;

		private UXGrid gridWarLog;

		private JewelControl warLogBadge;

		public SquadScreenWarLogView(SquadSlidingScreen screen) : base(screen)
		{
		}

		public override void OnScreenLoaded()
		{
			this.viewContainer = this.screen.GetElement<UXElement>("WarlogContainer");
			this.labelEmptyWarlog = this.screen.GetElement<UXLabel>("LabelEmptyWarlog");
			this.tabButton = this.screen.GetElement<UXCheckbox>("SocialWarLogBtn");
			this.tabButton.OnSelected = new UXCheckboxSelectedDelegate(this.OnTabButtonSelected);
			this.gridWarLog = this.screen.GetElement<UXGrid>("GridWarlog");
			this.gridWarLog.SetTemplateItem("ItemWarlog");
			if (this.warLogBadge == null)
			{
				this.warLogBadge = JewelControl.Create(this.screen, "SocialWarLog");
			}
		}

		public override void ShowView()
		{
			EventManager eventManager = Service.EventManager;
			this.viewContainer.Visible = true;
			eventManager.SendEvent(EventId.SquadSelect, null);
			eventManager.SendEvent(EventId.UISquadScreenTabShown, "warlog");
			SquadController squadController = Service.SquadController;
			SquadMemberWarData currentMemberWarData = squadController.WarManager.GetCurrentMemberWarData();
			uint serverTime = Service.ServerAPI.ServerTime;
			if (SquadUtils.DoesRewardWithoutWarHistoryExist(squadController, currentMemberWarData, serverTime))
			{
				ProcessingScreen.Show();
				EventManager eventManager2 = Service.EventManager;
				eventManager2.RegisterObserver(this, EventId.SquadUpdateCompleted);
				squadController.UpdateCurrentSquad();
			}
			else
			{
				this.RefreshView();
			}
			this.tabButton.Selected = true;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		public override void HideView()
		{
			this.viewContainer.Visible = false;
			this.tabButton.Selected = false;
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			if (this.gridWarLog != null)
			{
				this.gridWarLog.Clear();
			}
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.SquadUpdateCompleted);
		}

		public override void RefreshView()
		{
			this.gridWarLog.Clear();
			SquadController squadController = Service.SquadController;
			Squad currentSquad = squadController.StateManager.GetCurrentSquad();
			if (currentSquad == null)
			{
				return;
			}
			uint serverTime = Service.ServerAPI.ServerTime;
			SquadMemberWarData currentMemberWarData = squadController.WarManager.GetCurrentMemberWarData();
			int i = 0;
			int count = currentSquad.WarHistory.Count;
			while (i < count)
			{
				SquadWarHistoryEntry squadWarHistoryEntry = currentSquad.WarHistory[i];
				UXElement uXElement = this.gridWarLog.CloneTemplateItem(squadWarHistoryEntry.WarId);
				this.gridWarLog.AddItem(uXElement, i);
				uXElement.Tag = squadWarHistoryEntry.WarId;
				SquadWarRewardData rewardForWar = SquadUtils.GetRewardForWar(squadWarHistoryEntry.WarId, currentMemberWarData);
				UXButton subElement = this.gridWarLog.GetSubElement<UXButton>(squadWarHistoryEntry.WarId, "BtnCollectWarReward");
				UXLabel subElement2 = this.gridWarLog.GetSubElement<UXLabel>(squadWarHistoryEntry.WarId, "LabelBtnCollectWarReward");
				subElement2.Text = this.lang.Get("SQUAD_WAR_LOG_COLLECT_REWARD", new object[0]);
				UXLabel subElement3 = this.gridWarLog.GetSubElement<UXLabel>(squadWarHistoryEntry.WarId, "LabelWarlogTimestamp");
				subElement3.Text = this.lang.Get("TIME_AGO", new object[]
				{
					ChatTimeConversionUtils.GetFormattedAge(squadWarHistoryEntry.EndDate, this.lang)
				});
				UXLabel subElement4 = this.gridWarLog.GetSubElement<UXLabel>(squadWarHistoryEntry.WarId, "LabelWarlogScore");
				subElement4.Text = this.lang.Get("SQUAD_WAR_LOG_SCORE", new object[]
				{
					squadWarHistoryEntry.Score,
					squadWarHistoryEntry.OpponentScore
				});
				UXLabel subElement5 = this.gridWarLog.GetSubElement<UXLabel>(squadWarHistoryEntry.WarId, "LabelWarlogOpponent");
				string text = string.Empty;
				if (!string.IsNullOrEmpty(squadWarHistoryEntry.OpponentName))
				{
					text = this.lang.Get("SQUAD_WAR_LOG_OPPONENT_VS", new object[]
					{
						squadWarHistoryEntry.OpponentName
					});
				}
				subElement5.Text = text;
				UXLabel subElement6 = this.gridWarLog.GetSubElement<UXLabel>(squadWarHistoryEntry.WarId, "LabelWarlogExpires");
				bool flag = rewardForWar != null && rewardForWar.ExpireDate > serverTime;
				if (flag)
				{
					int num = (int)(rewardForWar.ExpireDate - serverTime);
					string text2 = LangUtils.FormatTime((long)num);
					subElement6.Visible = true;
					text2 = this.lang.Get("expires_in", new object[]
					{
						text2
					});
					subElement6.Text = text2;
					subElement.Visible = true;
					subElement.OnClicked = new UXButtonClickedDelegate(this.OnCollectButtonClicked);
					subElement.Tag = rewardForWar.WarId;
				}
				else
				{
					subElement6.Visible = false;
					subElement.Visible = false;
				}
				UXSprite subElement7 = this.gridWarLog.GetSubElement<UXSprite>(squadWarHistoryEntry.WarId, "SpriteWarlogBgDraw");
				subElement7.Visible = false;
				UXSprite subElement8 = this.gridWarLog.GetSubElement<UXSprite>(squadWarHistoryEntry.WarId, "SpriteWarlogBgLost");
				subElement8.Visible = false;
				UXSprite subElement9 = this.gridWarLog.GetSubElement<UXSprite>(squadWarHistoryEntry.WarId, "SpriteWarlogBgWon");
				subElement9.Visible = false;
				UXLabel subElement10 = this.gridWarLog.GetSubElement<UXLabel>(squadWarHistoryEntry.WarId, "LabelWarlogResult");
				if (squadWarHistoryEntry.Score > squadWarHistoryEntry.OpponentScore)
				{
					subElement9.Visible = true;
					subElement10.Text = this.lang.Get("SQUAD_WAR_LOG_WIN", new object[0]);
				}
				else if (squadWarHistoryEntry.Score < squadWarHistoryEntry.OpponentScore)
				{
					subElement8.Visible = true;
					subElement10.Text = this.lang.Get("SQUAD_WAR_LOG_LOSS", new object[0]);
				}
				else
				{
					subElement7.Visible = true;
					subElement10.Text = this.lang.Get("SQUAD_WAR_LOG_DRAW", new object[0]);
				}
				i++;
			}
			this.gridWarLog.RepositionItemsFrameDelayed();
			if (currentSquad.WarHistory.Count == 0)
			{
				this.labelEmptyWarlog.Visible = true;
				if (SquadUtils.SquadMeetsMatchmakingRequirements(squadController))
				{
					this.labelEmptyWarlog.Text = this.lang.Get("SQUAD_WAR_LOG_EMPTY_HAS_REQUIREMENTS", new object[0]);
				}
				else
				{
					this.labelEmptyWarlog.Text = this.lang.Get("SQUAD_WAR_LOG_EMPTY_LACKS_REQUIREMENTS", new object[0]);
				}
			}
			else
			{
				this.labelEmptyWarlog.Visible = false;
			}
			this.RefreshBadge();
		}

		public int RefreshBadge()
		{
			SquadController squadController = Service.SquadController;
			SquadMemberWarData currentMemberWarData = squadController.WarManager.GetCurrentMemberWarData();
			uint serverTime = Service.ServerAPI.ServerTime;
			int unclaimedSquadWarRewardsCount = SquadUtils.GetUnclaimedSquadWarRewardsCount(currentMemberWarData, serverTime);
			if (this.warLogBadge == null)
			{
				this.warLogBadge = JewelControl.Create(this.screen, "SocialWarLog");
			}
			this.warLogBadge.Value = unclaimedSquadWarRewardsCount;
			return unclaimedSquadWarRewardsCount;
		}

		private void OnTabButtonSelected(UXCheckbox checkbox, bool selected)
		{
			if (selected)
			{
				SquadController squadController = Service.SquadController;
				squadController.StateManager.SquadScreenState = SquadScreenState.WarLog;
				this.screen.RefreshViews();
			}
		}

		public void OnViewFrameTime(float dt)
		{
			List<UXElement> elementList = this.gridWarLog.GetElementList();
			SquadController squadController = Service.SquadController;
			uint serverTime = Service.ServerAPI.ServerTime;
			SquadMemberWarData currentMemberWarData = squadController.WarManager.GetCurrentMemberWarData();
			int i = 0;
			int count = elementList.Count;
			while (i < count)
			{
				string text = elementList[i].Tag as string;
				UXLabel subElement = this.gridWarLog.GetSubElement<UXLabel>(text, "LabelWarlogExpires");
				SquadWarRewardData rewardForWar = SquadUtils.GetRewardForWar(text, currentMemberWarData);
				if (rewardForWar != null)
				{
					int num = (int)(rewardForWar.ExpireDate - serverTime);
					if (num < 0)
					{
						this.HideCollectButton(text);
					}
					else
					{
						string text2 = LangUtils.FormatTime((long)num);
						subElement.Visible = true;
						subElement.Text = text2;
					}
				}
				else
				{
					this.HideCollectButton(text);
				}
				i++;
			}
		}

		public override void OnDestroyElement()
		{
			this.HideView();
		}

		public override bool IsVisible()
		{
			return this.viewContainer.Visible;
		}

		private void HideCollectButton(string warId)
		{
			UXLabel subElement = this.gridWarLog.GetSubElement<UXLabel>(warId, "LabelWarlogExpires");
			UXButton subElement2 = this.gridWarLog.GetSubElement<UXButton>(warId, "BtnCollectWarReward");
			subElement.Visible = false;
			subElement2.Visible = false;
		}

		private void OnCollectButtonClicked(UXButton button)
		{
			string text = button.Tag as string;
			this.HideCollectButton(text);
			SquadWarManager warManager = Service.SquadController.WarManager;
			SquadWarData currentSquadWar = warManager.CurrentSquadWar;
			if (currentSquadWar != null && currentSquadWar.WarId == text && warManager.GetCurrentStatus() == SquadWarStatusType.PhaseCooldown && !(Service.GameStateMachine.CurrentState is GalaxyState))
			{
				Service.UXController.HUD.SlideSquadScreenClosedInstantly();
				warManager.EnterWarBoardMode();
			}
			else
			{
				warManager.ClaimSquadWarReward(text);
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.SquadUpdateCompleted)
			{
				EventManager eventManager = Service.EventManager;
				eventManager.UnregisterObserver(this, EventId.SquadUpdateCompleted);
				ProcessingScreen.Hide();
				this.RefreshView();
			}
			return EatResponse.NotEaten;
		}
	}
}
