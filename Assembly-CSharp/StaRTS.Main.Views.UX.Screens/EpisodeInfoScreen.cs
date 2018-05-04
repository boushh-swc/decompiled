using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Episodes;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class EpisodeInfoScreen : ClosableScreen, IEventObserver, IViewClockTimeObserver, IViewFrameTimeObserver
	{
		private const float MINIMUM_RESEARCH_PIE_SLICE = 0.02f;

		private const int SLIDER_GRANULARITY = 1000;

		private const int SLIDER_MINIMUM = 20;

		private const int STAGE_TASK = 1;

		private const int STAGE_RESEARCH = 2;

		private const int STAGE_REWARD = 3;

		private const string LABEL_TITLE = "LabelTitle";

		private const string LABEL_DESC = "LabelDescription";

		private const string LABEL_EXPIRATION = "LabelExpiration";

		private const string LABEL_NEW_CHAPTER = "LabelNewChapter";

		private const string LABEL_TUTORIAL = "LabelEPHelp";

		private const string BTN_REPLAY_INTRO = "BtnReplayIntro";

		private const string TEXTURE_BG_THEME = "TextureTheme";

		private const string TEXTURE_BG_RESEARCH = "TextureResearchBkg";

		private const string TEXTURE_BG_REWARD = "TextureRewardBkg";

		private const string ELEMENT_TASK_STAGES = "WidgetTaskStages";

		private const string BTN_TASK = "BtnTask";

		private const string LABEL_BTN_TASK = "LabelBtnTask";

		private const string LABEL_TASK_NAME = "LabelChapter";

		private const string LABEL_TASK_DESCRIPTION = "LabelTask";

		private const string LABEL_TASK_BUTTON_DESC = "LabelTaskButtonDesc";

		private const string LABEL_TASK_PROGRESS = "LabelTaskProgress";

		private const string LABEL_TASK_COMPLETE = "LabelTaskComplete";

		private const string PBAR_TASK = "pBarTask";

		private const string SPRITE_TASK_ICON = "SpritepBarTaskIcon";

		private const string BTN_EP_HELP = "BtnEPHelp";

		private const string SPRITE_COMPLEX_TASK_ICON = "SpriteComplexTaskIcon";

		private const string BTN_TASK_TIMER = "BtnTimer";

		private const string BTN_SKIP_RESEARCH = "BtnSkipResearch";

		private const string LABEL_BTN_TIMER = "LabelBtnTimer";

		private const string LABEL_BTN_SKIP_RESEARCH = "LabelBtnSkipResearch";

		private const string LABEL_FINISH_INSTANT = "LabelFinishInstant";

		private const string LABEL_RESEARCH = "LabelResearch";

		private const string LABEL_RESEARCH_TIMER = "LabelResearchTimer";

		private const string PBAR_TIMER = "pBarTimer";

		private const string LABEL_RESEARCH_COMPLETE = "LabelResearchComplete";

		private const string BTN_CRATE_OPEN = "BtnPrimary";

		private const string BTN_CRATE_FLYOUT = "BtnRewardPrimary";

		private const string LABEL_BTN_CRATE = "LabelBtnPrimary";

		private const string LABEL_REWARD = "LabelReward";

		private const string LABEL_REWARD_NAME = "LabelRewardName";

		private const string SPRITE_CRATE = "SpriteRewardIcon";

		private const string TASK_REWARD_CONTAINER_PREFIX = "Container_";

		private const string WIDGET_PROGRESS = "WidgetProgress";

		private const string SPRITE_REWARD_PREFIX = "SpriteComplexSelectedItemImage_";

		private const string BUTTON_TASK_REWARD_PREFIX = "BtnInfoComplexReward_";

		private const string TASK_PROGRESS_REWARD_SLIDER_PREFIX = "pBarEpisodeProgress_";

		private const string TASK_PROGRESS_FINAL_SLIDER_PREFIX = "pBarEpisodeProgress_Final";

		private const string TASK_PROGRESS_REWARD_SLIDER_DELTA_PREFIX = "pBarEpisodeProgress_Delta_";

		private const string TASK_PROGRESS_FINAL_SLIDER_DELTA = "pBarEpisodeProgress_Delta_Final";

		private const string TASK_PROGRESS_REWARD_PARENT_PREFIX = "WidgetComplexReward_";

		private const string WIDGET_PROGRESS_INDICATOR = "WidgetProgressIndicator";

		private const string SPRITE_BAR_EPISODE_PROGRESS_THUMB_PREFIX = "SpriteBarEpisodeProgressThumb_";

		private const string SPRITE_BAR_EPISODE_PROGRESS_THUMB_FINAL = "SpriteBarEpisodeProgressThumb_Final";

		private const string SPRITE_OFFSET_THUMB = "SpritepBarEpisodeProgressOffset";

		private const string SPRITE_FINAL_REWARD = "SpriteTroopSelectedItemImage";

		private const string BUTTON_FINAL_REWARD = "BtnInfoFinalReward";

		private const string WIDGET_FINAL_REWARD = "WidgetFinalReward";

		private const string EFFECT_FINAL_REWARD = "ParticlesTopPrize";

		private const float ANIM_INIT_DELAY = 0.2f;

		private const int MAX_TASK_REWARDS_SHOWN = 7;

		private const string LABEL_EPISODE_PROGRESS = "LabelEpisodeProgress";

		private const string CRATE_OPEN = "CRATE_FLYOUT_OPEN_CTA";

		private const string MORE_INFO = "hn_more_info";

		private const string REWARDS = "s_Rewards";

		private const string ENDS_IN = "EPISODE_ENDS_IN";

		private const string TIMEGATE_ENDS_IN = "RESEARCH_ENDS_IN";

		private const string FINISH = "FINISH";

		private const string FINISH_NOW = "context_Finish_Now";

		private const string FRACTION = "FRACTION";

		private const string TIMEGATE_START = "BUTTON_MISSION_EVENT";

		private const string RESEARCH_TITLE = "RESEARCH_TITLE";

		private const string EPISODE_START_RESEARCH_CTA = "EPISODE_START_RESEARCH_CTA";

		private const string HELP_OPEN_BUTTON = "gui_event_points_help";

		private const string FINISHED_REWARD_BUTTON = "FINISHED_REWARD_BUTTON";

		private const string PROGRESS_SKIP_TITLE = "EPISODE_SKIP_PROGRESS_TITLE";

		private const string PROGRESS_SKIP_BODY = "EPISODE_SKIP_PROGRESS_BODY";

		private const string TIMEGATE_SKIP_TITLE = "EPISODE_SKIP_TIMEGATE_TITLE";

		private const string TIMEGATE_SKIP_BODY = "EPISODE_SKIP_TIMEGATE_BODY";

		private const string ANIM_SHOW = "anim_episode_show";

		private const string ANIM_HIDE = "anim_episode_hide";

		private const string ANIM_RESET = "anim_episode_reset";

		private const string ANIM_NO_TIMER = "anim_episode_no_timer";

		private const string ANIM_RESEARCH_COMPLETE = "anim_episode_research_complete";

		private const string ANIM_TASK_COMPLETE = "anim_episode_task_complete";

		private const string ANIM_REWARDS_TRIGGER_PREFIX = "anim_episode_task_0";

		private const string TRG_HIDE = "Hide";

		private const string TRG_HIDE_BOTTOM = "HideBottom";

		private const string TRG_RESET = "Reset";

		private const string TRG_SHOW = "Show";

		private const string TRG_SHOW_TOP = "ShowTop";

		private const string TRG_SHOW_BOTTOM = "ShowBottom";

		private const string TRG_SHOW_BOTTOM_FINAL = "ShowFinal";

		private const string TRG_NEW_CHAPTER = "NewChapter";

		private const string TRG_TASK_NEW = "TaskNew";

		private const string TRG_TASK_COMPLETE = "TaskComplete";

		private const string TRG_SET_TASK_COMPLETE = "SetTaskComplete";

		private const string TRG_TASK_COMPLEX_NEW = "TaskComplexNew";

		private const string TRG_TASK_COMPLEX_COMPLETE = "TaskComplexComplete";

		private const string TRG_SET_TASK_COMPLEX_COMPLETE = "SetTaskComplexComplete";

		private const string TRG_RESEARCH_NEW = "ResearchNew";

		private const string TRG_RESEARCH_COMPLETE = "ResearchComplete";

		private const string TRG_SET_RESEARCH_COMPLETE = "SetResearchComplete";

		private const string TRG_TASK_REWARD_CHECK_COMPLETING = "Completing";

		private const string TRG_TASK_REWARD_CHECK_COMPLETE = "Completed";

		private const string TRG_TASK_REWARD_CHECK_CURRENT = "Current";

		private const string TRG_TASK_REWARD_CHECK_UNSTARTED = "Unstarted";

		private const string TRG_TASK_REWARD_EQUIPMENT = "Equipment";

		private const string TRG_TASK_REWARD_QUALITY = "Quality";

		private const string TRG_WIDGET_PROGRESS_ACTIVE = "TaskActive";

		private const string TRG_WIDGET_PROGRESS_RESEARCH = "ResearchActive";

		private const string TRG_WIDGET_PROGRESS_REWARD = "RewardActive";

		private const string ANIM_PARAM_TASKS = "Tasks";

		private const string ANIM_PARAM_TIME_GATE = "TimeGate";

		private float DEFAULT_DELAY_EPISODES_PLAY_TASK_STORY_ACTION = 3f;

		private float DELAY_INITIAL_BOTTOM = 0.3f;

		private UXLabel taskTimeRemainingLabel;

		private UXSlider taskTimeGateSlider;

		private EpisodeProgressInfo progressInfo;

		private EpisodeTaskProgressInfo taskProgressInfo;

		private uint progressIndicatorTimer;

		private List<string> significantTaskUids;

		private List<bool> taskIndexToNewChapterTitle;

		private List<int> taskIndexToSignificantTask;

		private List<int> significantTaskToTaskCount;

		private List<int> significantTaskToTaskIndex;

		private List<HUDResourceView> significantTaskToView;

		private List<HUDResourceView> significantTaskToDelta;

		private List<Animator> rewardAnimators;

		private List<UXSprite> significantTaskToReward;

		private List<UXElement> significantTaskToContainer;

		private List<UXSlider> significantTaskToSlider;

		private List<UXElement> significantTaskToThumb;

		private UXSprite complexTaskIcon;

		private UXSprite spriteOffsetThumb;

		private EpisodeDataVO episodeDataVO;

		private EpisodeTaskVO taskVO;

		private EpisodeTaskActionVO taskActionVO;

		private CrateVO taskCrateRewardVO;

		private EpisodePanelVO panelVO;

		private EpisodeWidgetDataVO widgetVO;

		private EpisodeWidgetStateVO widgetStateVO;

		private DateTime expirationDate;

		private CountdownControl expirationCC;

		private CountdownControl taskTimeGateCC;

		private uint expirationTimerId;

		private uint playStoryActionTimerID;

		private uint showBottomTriggerTimerId;

		private FactionType faction;

		private bool canRefresh;

		private UXLabel btnLabelTimeGateCost;

		private UXElement widgetProgressIndicator;

		private UXElement widgetProgress;

		private Animator widgetProgressAnimator;

		private TaskViewState currentViewState;

		private TaskViewState previousViewState;

		private TaskViewMoment currentMoment;

		private bool freshScreen = true;

		private uint delayedInitTimerId;

		protected override bool IsFullScreen
		{
			get
			{
				return true;
			}
		}

		public EpisodeInfoScreen() : base("gui_episodes")
		{
			this.previousViewState = Service.EpisodeController.PreviousViewState;
			this.faction = Service.CurrentPlayer.Faction;
			this.UpdateEpisodeStateData();
			this.PopulateSignificantTaskUids();
			this.expirationCC = null;
			this.taskTimeGateCC = null;
			this.canRefresh = true;
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			EventManager eventManager = Service.EventManager;
			switch (id)
			{
			case EventId.InventoryCrateCollectionClosed:
			{
				eventManager.UnregisterObserver(this, EventId.InventoryCrateCollectionClosed);
				this.canRefresh = true;
				this.RefreshUI();
				this.InitButtons();
				float delay = (GameConstants.DELAY_EPISODES_PLAY_TASK_STORY_SECONDS <= 0f) ? this.DEFAULT_DELAY_EPISODES_PLAY_TASK_STORY_ACTION : GameConstants.DELAY_EPISODES_PLAY_TASK_STORY_SECONDS;
				this.playStoryActionTimerID = Service.ViewTimerManager.CreateViewTimer(delay, false, new TimerDelegate(this.DelayedPlayTaskStoryAction), null);
				return EatResponse.NotEaten;
			}
			case EventId.EpisodeDataRefreshed:
				if (this.canRefresh)
				{
					this.RefreshUI();
				}
				return EatResponse.NotEaten;
			case EventId.EpisodeProgressInfoRefreshed:
			case EventId.EpisodeProgressMade:
				IL_3C:
				if (id != EventId.NewTopScreen)
				{
					return EatResponse.NotEaten;
				}
				this.UpdateParticles();
				return EatResponse.NotEaten;
			case EventId.EpisodeTaskProgressSkipped:
			case EventId.EpisodeTaskProgressSkipFailed:
			case EventId.EpisodeTaskProgressCompleted:
			case EventId.EpisodeTaskProgressCompleteFailed:
			case EventId.EpisodeTaskTimeGateStarted:
			case EventId.EpisodeTaskTimeGateSkipped:
				ProcessingScreen.Hide();
				eventManager.UnregisterObserver(this, id);
				return EatResponse.NotEaten;
			}
			goto IL_3C;
		}

		private void DelayedPlayTaskStoryAction(uint id, object cookie)
		{
			if (this.progressInfo.currentTask == null)
			{
				return;
			}
			string uid = this.progressInfo.currentTask.uid;
			StaticDataController staticDataController = Service.StaticDataController;
			EpisodeDataVO episodeDataVO = staticDataController.Get<EpisodeDataVO>(this.progressInfo.uid);
			if (episodeDataVO.GrindTask == uid && this.progressInfo.grindInfo.Started > 0)
			{
				return;
			}
			Service.EpisodeController.PlayStoryActionForTaskUid(uid);
		}

		private void UpdateTimeGateCostUI(int secondsRemaining)
		{
			if (this.btnLabelTimeGateCost == null)
			{
				Service.Logger.Error("EpisodeInfoScreen: Cannot update time gate cost label, label not set!");
				return;
			}
			if (secondsRemaining <= 0)
			{
				Service.Logger.Error("EpisodeInfoScreen: Cannot update time gate cost label, seconds remaining is 0!");
				return;
			}
			int num = GameUtils.SecondsToCrystalsForEpisodeTaskTimeGate(secondsRemaining);
			this.btnLabelTimeGateCost.Text = this.lang.ThousandsSeparated(num);
			UXUtils.UpdateCostColor(this.btnLabelTimeGateCost, null, 0, 0, 0, num, 0, false);
		}

		public void OnViewClockTime(float dt)
		{
			int timeGate = this.taskVO.TimeGate;
			uint endTimestamp = this.taskProgressInfo.endTimestamp;
			uint time = ServerTime.Time;
			int num = (int)(endTimestamp - time);
			if (num <= 0)
			{
				Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
				Service.EpisodeController.ForceRefreshState();
				return;
			}
			this.UpdateTimeGateCostUI(num);
			this.taskTimeGateSlider.Value = Mathf.Max(0.02f, 1f * (float)(timeGate - num) / (float)timeGate);
		}

		protected override void OnScreenLoaded()
		{
			base.InitAnimator();
			this.InitButtons();
			this.InitLabels();
			this.InitSliders();
			this.InitBackgroundTexture();
			this.InitRewards();
			this.InitTaskEpHelpButton();
			this.InitProgressIndicator();
			this.delayedInitTimerId = Service.ViewTimerManager.CreateViewTimer(1f, false, new TimerDelegate(this.DelayedInitialization), 0);
			string messageAddition = string.Empty;
			if (this.widgetStateVO != null)
			{
				messageAddition = this.widgetStateVO.GetIconTextureId(this.faction);
			}
			this.LogBI(EventId.EpisodeInfoScreenOpened, messageAddition);
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.EpisodeDataRefreshed);
			eventManager.RegisterObserver(this, EventId.NewTopScreen);
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		private void DelayedInitialization(uint id, object cookie)
		{
			this.RefreshUI();
		}

		private void InitProgressIndicator()
		{
			this.spriteOffsetThumb = base.GetElement<UXSprite>("SpritepBarEpisodeProgressOffset");
			this.widgetProgressIndicator = base.GetElement<UXElement>("WidgetProgressIndicator");
			this.widgetProgressIndicator.Visible = false;
			this.widgetProgress = base.GetElement<UXElement>("WidgetProgress");
			this.widgetProgress.InitAnimator();
		}

		private void InitSliders()
		{
			this.significantTaskToView = new List<HUDResourceView>();
			this.significantTaskToDelta = new List<HUDResourceView>();
			this.significantTaskToSlider = new List<UXSlider>();
			this.significantTaskToThumb = new List<UXElement>();
			int num = this.significantTaskUids.Count - 1;
			for (int i = 0; i < num; i++)
			{
				string text = "pBarEpisodeProgress_" + (i + 1);
				UXSlider element = base.GetElement<UXSlider>(text);
				this.significantTaskToView.Add(new HUDResourceView(text, element, null, null));
				string text2 = "pBarEpisodeProgress_Delta_" + (i + 1);
				UXSlider element2 = base.GetElement<UXSlider>(text2);
				this.significantTaskToDelta.Add(new HUDResourceView(text2, element2, null, null));
				string name = "SpriteBarEpisodeProgressThumb_" + (i + 1);
				UXElement element3 = base.GetElement<UXElement>(name);
				this.significantTaskToThumb.Add(element3);
				this.significantTaskToSlider.Add(element2);
			}
			UXSlider element4 = base.GetElement<UXSlider>("pBarEpisodeProgress_Final");
			this.significantTaskToView.Add(new HUDResourceView("pBarEpisodeProgress_Final", element4, null, null));
			UXSlider element5 = base.GetElement<UXSlider>("pBarEpisodeProgress_Delta_Final");
			this.significantTaskToDelta.Add(new HUDResourceView("pBarEpisodeProgress_Delta_Final", element5, null, null));
			UXElement element6 = base.GetElement<UXElement>("SpriteBarEpisodeProgressThumb_Final");
			this.significantTaskToThumb.Add(element6);
			this.significantTaskToSlider.Add(element5);
		}

		private void InitTaskEpHelpButton()
		{
			UXButton element = base.GetElement<UXButton>("BtnEPHelp");
			element.OnClicked = new UXButtonClickedDelegate(this.OnTaskEPHelpButtonClicked);
		}

		private void InitRewards()
		{
			this.rewardAnimators = new List<Animator>();
			this.significantTaskToReward = new List<UXSprite>();
			this.significantTaskToContainer = new List<UXElement>();
			int num = this.significantTaskUids.Count - 1;
			for (int i = 0; i < num; i++)
			{
				int num2 = i + 1;
				string name = "WidgetComplexReward_" + num2;
				UXElement element = base.GetElement<UXElement>(name);
				this.rewardAnimators.Add(element.Root.GetComponent<Animator>());
				string name2 = "SpriteComplexSelectedItemImage_" + num2;
				UXSprite element2 = base.GetElement<UXSprite>(name2);
				this.significantTaskToReward.Add(element2);
				string name3 = "Container_" + num2;
				UXElement element3 = base.GetElement<UXElement>(name3);
				this.significantTaskToContainer.Add(element3);
			}
			string name4 = "WidgetFinalReward";
			UXElement element4 = base.GetElement<UXElement>(name4);
			this.rewardAnimators.Add(element4.Root.GetComponent<Animator>());
			string name5 = "SpriteTroopSelectedItemImage";
			UXSprite element5 = base.GetElement<UXSprite>(name5);
			this.significantTaskToReward.Add(element5);
		}

		private void PopulateSignificantTaskUids()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			this.significantTaskUids = new List<string>();
			this.taskIndexToNewChapterTitle = new List<bool>();
			this.taskIndexToSignificantTask = new List<int>();
			this.significantTaskToTaskCount = new List<int>();
			this.significantTaskToTaskIndex = new List<int>();
			int num = 0;
			int num2 = 0;
			string a = string.Empty;
			for (int i = 0; i < this.episodeDataVO.Tasks.Length; i++)
			{
				num2++;
				string text = this.episodeDataVO.Tasks[i];
				EpisodeTaskVO optional = staticDataController.GetOptional<EpisodeTaskVO>(text);
				if (optional == null)
				{
					Service.Logger.WarnFormat("No episode task data found for {0}", new object[]
					{
						text
					});
				}
				else
				{
					string text2 = string.Empty;
					FactionType factionType = this.faction;
					if (factionType != FactionType.Empire)
					{
						if (factionType == FactionType.Rebel)
						{
							text2 = optional.RebelHeaderString;
						}
					}
					else
					{
						text2 = optional.EmpireHeaderString;
					}
					if (a != text2)
					{
						a = text2;
						this.taskIndexToNewChapterTitle.Add(true);
					}
					else
					{
						this.taskIndexToNewChapterTitle.Add(false);
					}
					this.taskIndexToSignificantTask.Add(num);
					if (optional.IsSignificant)
					{
						this.significantTaskToTaskCount.Add(num2 - 1);
						num2 = 0;
						this.significantTaskUids.Add(text);
						num++;
						this.significantTaskToTaskIndex.Add(i);
					}
				}
			}
		}

		private void SetupTaskRewards()
		{
			int num = Math.Min(this.significantTaskUids.Count - 1, 7);
			if (num > 7)
			{
				Service.Logger.WarnFormat("Too many task rewards for episode {0} to be shown", new object[]
				{
					this.episodeDataVO.Uid
				});
			}
			this.AnimParam("Tasks", num);
		}

		public void OnViewFrameTime(float dt)
		{
			if (this.currentMoment == null)
			{
				return;
			}
			if (this.currentMoment.Delay > 0f)
			{
				this.currentMoment.Delay -= dt;
				return;
			}
			this.currentMoment.Elapsed = Mathf.Min(this.currentMoment.Elapsed + dt, this.currentMoment.Duration);
			if (this.currentMoment.Elapsed < this.currentMoment.Duration)
			{
				if (this.currentMoment.UpdateCallback != null)
				{
					this.currentMoment.UpdateCallback(this.currentMoment);
				}
			}
			else
			{
				if (this.currentMoment.CompleteCallback != null)
				{
					this.currentMoment.CompleteCallback(this.currentMoment);
				}
				this.currentMoment = null;
				this.RenderTaskViewState();
			}
		}

		public override void OnDestroyElement()
		{
			Service.EpisodeController.PreviousViewState = this.previousViewState;
			if (this.expirationCC != null)
			{
				this.expirationCC.Destroy();
				this.expirationCC = null;
			}
			if (this.taskTimeGateCC != null)
			{
				this.taskTimeGateCC.Destroy();
				this.taskTimeGateCC = null;
			}
			if (this.significantTaskToView != null)
			{
				this.significantTaskToView.Clear();
				this.significantTaskToView = null;
			}
			if (this.delayedInitTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.delayedInitTimerId);
				this.delayedInitTimerId = 0u;
			}
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.EpisodeDataRefreshed);
			eventManager.UnregisterObserver(this, EventId.EpisodeTaskTimeGateSkipped);
			eventManager.UnregisterObserver(this, EventId.EpisodeTaskTimeGateStarted);
			eventManager.UnregisterObserver(this, EventId.EpisodeTaskProgressSkipped);
			eventManager.UnregisterObserver(this, EventId.EpisodeTaskProgressSkipFailed);
			eventManager.UnregisterObserver(this, EventId.InventoryCrateCollectionClosed);
			eventManager.UnregisterObserver(this, EventId.NewTopScreen);
			eventManager.RegisterObserver(this, EventId.EpisodeTaskProgressCompleted);
			eventManager.RegisterObserver(this, EventId.EpisodeTaskProgressCompleteFailed);
			Service.ViewTimerManager.KillViewTimer(this.expirationTimerId);
			Service.ViewTimerManager.KillViewTimer(this.playStoryActionTimerID);
			Service.ViewTimerManager.KillViewTimer(this.showBottomTriggerTimerId);
			Service.ViewTimerManager.KillViewTimer(this.progressIndicatorTimer);
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			base.OnDestroyElement();
		}

		private void RefreshUI()
		{
			Service.EventManager.SendEvent(EventId.EpisodeInfoScreenRefreshing, null);
			this.UpdateEpisodeStateData();
			this.SetupTaskRewards();
			this.InitUIStates();
			if (!Service.EpisodeController.IsEpisodeComplete())
			{
				this.InitTaskProgress();
				this.InitTimegateSection();
				this.InitRewardSection();
			}
			this.InitEpisodeProgress();
			this.RenderTaskViewState();
		}

		private void Animate(string trigger)
		{
			if (this.animator == null)
			{
				return;
			}
			this.animator.ResetTrigger(trigger);
			this.animator.SetTrigger(trigger);
			Service.Logger.Debug("Triggering: " + trigger);
		}

		private void Animate(string trigger, UXElement specificAnimator)
		{
			if (specificAnimator == null)
			{
				return;
			}
			specificAnimator.ResetTrigger(trigger);
			specificAnimator.SetTrigger(trigger);
			Service.Logger.Debug("Triggering: " + trigger + " on specific UXElement " + specificAnimator.ToString());
		}

		private void AnimParam(string param, bool value)
		{
			if (this.animator == null)
			{
				return;
			}
			this.animator.SetBool(param, value);
			Service.Logger.DebugFormat("Setting Param: {0} = {1}", new object[]
			{
				param,
				value
			});
		}

		private void AnimParam(string param, int value)
		{
			if (this.animator == null)
			{
				return;
			}
			this.animator.SetInteger(param, value);
			Service.Logger.DebugFormat("Setting Param: {0} = {1}", new object[]
			{
				param,
				value
			});
		}

		private void TryPlayNextMoment()
		{
			if (this.currentMoment == null)
			{
				if (this.currentViewState.Moments.Count == 0)
				{
					return;
				}
				this.currentMoment = this.currentViewState.Moments[0];
				this.currentViewState.Moments.RemoveAt(0);
				this.previousViewState = this.currentViewState.Copy();
				if (this.currentMoment.StartCallback != null)
				{
					this.currentMoment.StartCallback(this.currentMoment);
				}
			}
		}

		private void RenderTaskViewState()
		{
			if (this.currentMoment != null)
			{
				return;
			}
			if (this.currentViewState.Moments.Count > 0)
			{
				this.TryPlayNextMoment();
				return;
			}
			if (this.previousViewState == null)
			{
				this.currentViewState.Reset = true;
				this.previousViewState = this.currentViewState.Copy();
			}
			base.GetElement<UXLabel>("LabelDescription").Text = this.lang.Get(this.previousViewState.CurrentTaskInfo.GetBodyString(this.faction), new object[0]);
			base.GetElement<UXLabel>("LabelNewChapter").Text = this.lang.Get(this.previousViewState.CurrentTaskInfo.GetHeaderString(this.faction), new object[0]);
			if (this.currentViewState.TaskIndex > 0)
			{
				this.spriteOffsetThumb.Visible = false;
			}
			for (int i = 0; i < this.significantTaskToView.Count; i++)
			{
				if (i > 0 && this.previousViewState.TaskIndex == this.significantTaskToTaskIndex[i - 1] + 1)
				{
					this.significantTaskToView[i].SetAmount(20, 1000, false);
					this.significantTaskToDelta[i].SetAmount(0, 1000, false);
					this.rewardAnimators[i].SetTrigger("Unstarted");
				}
				else if (this.previousViewState.TaskIndex > this.significantTaskToTaskIndex[i])
				{
					this.significantTaskToView[i].SetAmount(1000, 1000, false);
					this.significantTaskToDelta[i].SetAmount(1000, 1000, false);
					this.rewardAnimators[i].SetTrigger("Completed");
				}
				else if (this.previousViewState.TaskIndex < this.significantTaskToTaskIndex[i])
				{
					this.significantTaskToView[i].SetAmount(0, 1000, false);
					this.significantTaskToDelta[i].SetAmount(0, 1000, false);
					this.rewardAnimators[i].SetTrigger("Unstarted");
				}
				else
				{
					this.significantTaskToView[i].SetAmount(1000, 1000, false);
					this.significantTaskToDelta[i].SetAmount(1000, 1000, false);
					this.rewardAnimators[i].SetTrigger("Current");
				}
			}
			if (this.previousViewState.TaskIndex != this.currentViewState.TaskIndex)
			{
				this.HideProgressIndicator(null);
				int num = this.taskIndexToSignificantTask[this.previousViewState.TaskIndex];
				int num2 = (this.currentViewState.GrindCount <= 0) ? this.taskIndexToSignificantTask[this.currentViewState.TaskIndex] : (num + 1);
				if (num2 > num)
				{
					this.rewardAnimators[num].SetTrigger("Completing");
					this.UpdateProgressIndicator(this.currentViewState.TaskIndex);
					TaskViewMoment taskViewMoment = new TaskViewMoment();
					taskViewMoment.Duration = 0.1f;
					taskViewMoment.StartCallback = new Action<TaskViewMoment>(this.StartNextSignificantTaskIcons);
					taskViewMoment.StartCookie = num;
					taskViewMoment.CompleteCallback = new Action<TaskViewMoment>(this.UpdateProgressIndicator);
					taskViewMoment.CompleteCookie = this.currentViewState.TaskIndex;
					this.currentViewState.Moments.Add(taskViewMoment);
				}
				else
				{
					int num3 = this.significantTaskToTaskCount[num2];
					int num4 = this.currentViewState.TaskIndex;
					if (num2 > 0)
					{
						int num5 = this.significantTaskToTaskIndex[num2 - 1];
						int num6 = num5 + 1;
						num4 -= num6;
					}
					int num7 = Math.Max(0, num4 - 1);
					TaskProgressTO taskProgressTO = new TaskProgressTO();
					taskProgressTO.View = this.significantTaskToDelta[num2];
					taskProgressTO.Start = num7 * 1000;
					taskProgressTO.End = num4 * 1000;
					taskProgressTO.Count = num3 * 1000;
					TaskViewMoment taskViewMoment2 = new TaskViewMoment();
					taskViewMoment2.StartCallback = new Action<TaskViewMoment>(this.SetNextDelta);
					taskViewMoment2.StartCookie = taskProgressTO;
					this.currentViewState.Moments.Add(taskViewMoment2);
					TaskProgressTO taskProgressTO2 = new TaskProgressTO();
					taskProgressTO2.View = this.significantTaskToView[num2];
					taskProgressTO2.Start = num7 * 1000;
					taskProgressTO2.End = num4 * 1000;
					taskProgressTO2.Count = num3 * 1000;
					taskProgressTO2.View.SetAmount(taskProgressTO2.Start, taskProgressTO2.Count, false);
					TaskViewMoment taskViewMoment3 = new TaskViewMoment();
					taskViewMoment3.Delay = 1.5f;
					taskViewMoment3.Duration = 1f;
					taskViewMoment3.UpdateCallback = new Action<TaskViewMoment>(this.AnimateProgress);
					taskViewMoment3.UpdateCookie = taskProgressTO2;
					taskViewMoment3.CompleteCallback = new Action<TaskViewMoment>(this.UpdateProgressIndicator);
					taskViewMoment3.CompleteCookie = this.currentViewState.TaskIndex;
					this.currentViewState.Moments.Add(taskViewMoment3);
				}
				if (this.currentViewState.TaskIndex < this.taskIndexToNewChapterTitle.Count && this.taskIndexToNewChapterTitle[this.currentViewState.TaskIndex])
				{
					base.GetElement<UXLabel>("LabelNewChapter").Text = this.lang.Get(this.currentViewState.CurrentTaskInfo.GetHeaderString(this.faction), new object[0]);
					TaskViewMoment taskViewMoment4 = new TaskViewMoment();
					taskViewMoment4.Delay = 1f;
					taskViewMoment4.StartCallback = new Action<TaskViewMoment>(this.ShowNewChapter);
					taskViewMoment4.Duration = 1f;
					taskViewMoment4.CompleteCallback = new Action<TaskViewMoment>(this.UpdateChapterDescription);
					taskViewMoment4.CompleteCookie = this.lang.Get(this.currentViewState.CurrentTaskInfo.GetBodyString(this.faction), new object[0]);
					this.currentViewState.Moments.Add(taskViewMoment4);
				}
				TaskViewMoment taskViewMoment5 = new TaskViewMoment();
				taskViewMoment5.Delay = 0.5f;
				taskViewMoment5.CompleteCallback = new Action<TaskViewMoment>(this.SetBottomVisible);
				this.currentViewState.Moments.Add(taskViewMoment5);
				this.TryPlayNextMoment();
				return;
			}
			if (this.currentViewState.GrindCount == 0)
			{
				int num8 = this.taskIndexToSignificantTask[this.currentViewState.TaskIndex];
				int num9 = this.significantTaskToTaskCount[num8];
				int num10 = this.currentViewState.TaskIndex;
				if (num8 > 0)
				{
					int num11 = this.significantTaskToTaskIndex[num8 - 1];
					int num12 = num11 + 1;
					num10 -= num12;
				}
				num10 = Math.Max(20, num10 * 1000);
				num9 *= 1000;
				this.significantTaskToView[num8].SetAmount(num10, num9, false);
			}
			if (this.previousViewState.GrindCount != this.currentViewState.GrindCount)
			{
				this.currentViewState.Reset = true;
			}
			if (this.currentViewState.Reset)
			{
				this.Animate("Reset");
				this.currentViewState.Reset = false;
				this.currentViewState.ShowTop = true;
				TaskViewMoment taskViewMoment6 = new TaskViewMoment();
				taskViewMoment6.Delay = this.DELAY_INITIAL_BOTTOM;
				taskViewMoment6.CompleteCallback = new Action<TaskViewMoment>(this.SetBottomVisible);
				this.currentViewState.Moments.Add(taskViewMoment6);
				this.TryPlayNextMoment();
			}
			this.AnimParam("TimeGate", this.currentViewState.IsTimeGated);
			bool flag = this.taskActionVO.Type != "EpisodePoint";
			int num13 = 1;
			if (this.currentViewState.TaskComplete)
			{
				num13 = 2;
				if (this.previousViewState.TaskComplete)
				{
					this.Animate((!flag) ? "SetTaskComplete" : "SetTaskComplexComplete");
				}
				else
				{
					this.Animate((!flag) ? "TaskComplete" : "TaskComplexComplete");
				}
			}
			else
			{
				this.Animate((!flag) ? "TaskNew" : "TaskComplexNew");
			}
			if (this.currentViewState.IsTimeGated)
			{
				if (this.currentViewState.ResearchComplete)
				{
					num13 = 3;
					if (this.previousViewState.ResearchComplete)
					{
						this.Animate("SetResearchComplete");
					}
					else
					{
						this.Animate("ResearchComplete");
					}
				}
				else
				{
					this.Animate("ResearchNew");
				}
			}
			else if (num13 == 2)
			{
				num13 = 3;
			}
			switch (num13)
			{
			case 1:
				this.Animate("TaskActive", this.widgetProgress);
				break;
			case 2:
				this.Animate("ResearchActive", this.widgetProgress);
				break;
			case 3:
				this.Animate("RewardActive", this.widgetProgress);
				break;
			}
			if (this.currentViewState.ShowTop)
			{
				this.Animate("ShowTop");
				this.currentViewState.ShowTop = false;
			}
			if (this.currentViewState.ShowBottom)
			{
				if (this.currentViewState.IsGrind)
				{
					this.Animate("ShowFinal");
				}
				else
				{
					this.Animate("ShowBottom");
					this.UpdateProgressIndicator(this.currentViewState.TaskIndex);
				}
				this.currentViewState.ShowBottom = false;
			}
			this.freshScreen = false;
			this.previousViewState = this.currentViewState.Copy();
			this.UpdateParticles();
		}

		private void SetBottomVisible(TaskViewMoment moment)
		{
			this.currentViewState.ShowBottom = true;
		}

		private void ShowNewChapter(TaskViewMoment moment)
		{
			this.Animate("NewChapter");
		}

		private void UpdateChapterDescription(TaskViewMoment moment)
		{
			base.GetElement<UXLabel>("LabelDescription").Text = (string)moment.CompleteCookie;
		}

		private void AnimateProgress(TaskViewMoment moment)
		{
			TaskProgressTO taskProgressTO = (TaskProgressTO)moment.UpdateCookie;
			float num = moment.Elapsed / moment.Duration;
			int num2 = (int)((float)(taskProgressTO.End - taskProgressTO.Start) * num);
			taskProgressTO.View.SetAmount(taskProgressTO.Start + num2, taskProgressTO.Count, false);
		}

		private void SetNextDelta(TaskViewMoment moment)
		{
			TaskProgressTO taskProgressTO = (TaskProgressTO)moment.StartCookie;
			taskProgressTO.View.SetAmount(taskProgressTO.End, taskProgressTO.Count, false);
		}

		private void HideProgressIndicator(TaskViewMoment moment)
		{
			if (this.widgetProgressIndicator == null)
			{
				return;
			}
			this.widgetProgressIndicator.Visible = false;
		}

		private void UpdateProgressIndicator(TaskViewMoment moment)
		{
			this.UpdateProgressIndicator((int)moment.CompleteCookie);
		}

		private void UpdateProgressIndicator(int taskIndex)
		{
			ViewTimerManager viewTimerManager = Service.ViewTimerManager;
			if (this.progressIndicatorTimer != 0u)
			{
				viewTimerManager.KillViewTimer(this.progressIndicatorTimer);
			}
			this.progressIndicatorTimer = viewTimerManager.CreateViewTimer(1f, false, new TimerDelegate(this.DelayedUpdateProgressIndicator), taskIndex);
		}

		private void DelayedUpdateProgressIndicator(uint id, object cookie)
		{
			this.progressIndicatorTimer = 0u;
			int num = (int)cookie;
			if (this.widgetProgressIndicator == null)
			{
				return;
			}
			int num2 = (num != 0) ? this.taskIndexToSignificantTask[num - 1] : 0;
			int num3 = this.significantTaskToTaskIndex[num2];
			if (num >= this.taskIndexToSignificantTask.Count)
			{
				this.widgetProgressIndicator.Visible = false;
			}
			else if (num == 0)
			{
				this.spriteOffsetThumb.Visible = true;
				this.widgetProgressIndicator.SetAnchorWidget(this.significantTaskToThumb[0]);
				this.significantTaskToDelta[0].SetAmount(0, 1000, false);
				this.significantTaskToView[0].SetAmount(20, 1000, false);
			}
			else if (num2 > 0 && num == this.significantTaskToTaskIndex[num2 - 1] + 1)
			{
				num2 = this.taskIndexToSignificantTask[num];
				this.widgetProgressIndicator.SetAnchorWidget(this.significantTaskToThumb[num2]);
				this.significantTaskToView[num2].SetAmount(20, 1000, false);
				this.significantTaskToDelta[num2].SetAmount(0, 1000, false);
			}
			else if (num == num3)
			{
				if (num2 == this.significantTaskToContainer.Count)
				{
					return;
				}
				this.widgetProgressIndicator.SetAnchorWidget(this.significantTaskToContainer[num2]);
			}
			else
			{
				num2 = this.taskIndexToSignificantTask[num];
				this.widgetProgressIndicator.SetAnchorWidget(this.significantTaskToThumb[num2]);
				this.significantTaskToView[num2].TweakAmount();
			}
			this.widgetProgressIndicator.Visible = true;
		}

		private void UpdateEpisodeStateData()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			EpisodeController episodeController = Service.EpisodeController;
			this.progressInfo = Service.CurrentPlayer.EpisodeProgressInfo;
			this.taskProgressInfo = this.progressInfo.currentTask;
			this.episodeDataVO = staticDataController.Get<EpisodeDataVO>(this.progressInfo.uid);
			this.expirationDate = this.episodeDataVO.EndTime;
			this.panelVO = staticDataController.Get<EpisodePanelVO>(this.episodeDataVO.Panel);
			this.widgetVO = episodeController.CurrentWidgetData;
			if (this.widgetVO != null)
			{
				this.widgetStateVO = staticDataController.GetOptional<EpisodeWidgetStateVO>(this.widgetVO.StateId);
			}
			if (this.taskProgressInfo != null && !string.IsNullOrEmpty(this.taskProgressInfo.uid))
			{
				this.taskVO = staticDataController.Get<EpisodeTaskVO>(this.taskProgressInfo.uid);
				this.taskActionVO = staticDataController.Get<EpisodeTaskActionVO>(this.taskProgressInfo.actionUID);
				this.taskCrateRewardVO = staticDataController.Get<CrateVO>(this.taskVO.CrateId);
				this.currentViewState = new TaskViewState();
				this.currentViewState.CurrentTaskInfo = this.taskVO;
				this.currentViewState.IsGrind = (this.taskProgressInfo.uid == this.episodeDataVO.GrindTask);
				this.currentViewState.GrindCount = this.progressInfo.grindInfo.Started;
				this.currentViewState.TaskComplete = episodeController.IsCurrentTaskProgressMarkedComplete();
				this.currentViewState.ResearchComplete = episodeController.IsTaskTimeGateComplete();
				this.currentViewState.IsTimeGated = episodeController.IsCurrentTaskTimeGated();
				this.currentViewState.TaskIndex = this.progressInfo.currentTaskIndex;
				this.currentViewState.Reset = this.freshScreen;
			}
			if (this.expirationCC != null)
			{
				this.expirationCC.Destroy();
				this.expirationCC = null;
			}
			if (this.taskTimeGateCC != null)
			{
				this.taskTimeGateCC.Destroy();
				this.taskTimeGateCC = null;
			}
		}

		private void InitUIStates()
		{
			EpisodeController episodeController = Service.EpisodeController;
			UXElement element = base.GetElement<UXElement>("WidgetTaskStages");
			if (episodeController.IsEpisodeComplete())
			{
				element.Visible = false;
				return;
			}
			element.Visible = true;
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			UXButton element = base.GetElement<UXButton>("BtnRewardPrimary");
			element.OnClicked = new UXButtonClickedDelegate(this.OnCrateClicked);
			UXButton element2 = base.GetElement<UXButton>("BtnReplayIntro");
			element2.OnClicked = new UXButtonClickedDelegate(this.OnPlayStoryActionClicked);
			StaticDataController staticDataController = Service.StaticDataController;
			int count = this.significantTaskUids.Count;
			if (count == 0)
			{
				Service.Logger.WarnFormat("Have no significant rewards for episode {0}", new object[]
				{
					this.episodeDataVO.Uid
				});
				return;
			}
			string text = string.Empty;
			int num = 1;
			EpisodeTaskVO optional;
			while (num <= count - 1 && num <= 7)
			{
				text = this.significantTaskUids[num - 1];
				optional = staticDataController.GetOptional<EpisodeTaskVO>(text);
				if (optional == null)
				{
					Service.Logger.WarnFormat("Missing task data for {0}", new object[]
					{
						text
					});
				}
				else
				{
					bool complete = this.progressInfo.finishedTasks != null && this.progressInfo.finishedTasks.Contains(text);
					this.SetupRewardforTask("BtnInfoComplexReward_" + num, "SpriteComplexSelectedItemImage_" + num, "WidgetComplexReward_" + num, optional, complete);
				}
				num++;
			}
			text = this.significantTaskUids[this.significantTaskUids.Count - 1];
			optional = staticDataController.GetOptional<EpisodeTaskVO>(text);
			bool complete2 = this.progressInfo != null && this.progressInfo.finishedTasks != null && this.progressInfo.finishedTasks.Contains(text);
			this.SetupRewardforTask("BtnInfoFinalReward", "SpriteTroopSelectedItemImage", "WidgetFinalReward", optional, complete2);
		}

		private void UpdateParticles()
		{
			UXElement element = base.GetElement<UXElement>("ParticlesTopPrize");
			ScreenController screenController = Service.ScreenController;
			if (screenController.GetHighestLevelScreen<ScreenBase>() == this)
			{
				element.Visible = true;
			}
			else
			{
				element.Visible = false;
			}
		}

		private void StartNextSignificantTaskIcons(TaskViewMoment moment)
		{
			int index = (int)moment.StartCookie;
			this.rewardAnimators[index].SetTrigger("Current");
		}

		private void SetupRewardforTask(string buttonName, string buttonSpriteName, string rewardParentName, EpisodeTaskVO rewardTaskVO, bool complete)
		{
			UXSprite element = base.GetElement<UXSprite>(buttonSpriteName);
			UXElement element2 = base.GetElement<UXElement>(rewardParentName);
			UXButton element3 = base.GetElement<UXButton>(buttonName);
			string uid = rewardTaskVO.Uid;
			element3.OnClicked = new UXButtonClickedDelegate(this.OnRewardInfoClicked);
			StaticDataController staticDataController = Service.StaticDataController;
			List<PlanetLootEntryVO> featuredLootEntriesForEpisodeTask = Service.InventoryCrateRewardController.GetFeaturedLootEntriesForEpisodeTask(rewardTaskVO, 1);
			if (featuredLootEntriesForEpisodeTask.Count != 0 && featuredLootEntriesForEpisodeTask[0] != null)
			{
				PlanetLootEntryVO planetLootEntryVO = featuredLootEntriesForEpisodeTask[0];
				CrateSupplyVO optional = staticDataController.GetOptional<CrateSupplyVO>(planetLootEntryVO.SupplyDataUid);
				if (optional != null)
				{
					string a = (string)element3.Tag;
					if (a != uid)
					{
						int playerHq = Service.CurrentPlayer.Map.FindHighestHqLevel();
						IGeometryVO iconVOFromCrateSupply = GameUtils.GetIconVOFromCrateSupply(optional, playerHq);
						if (iconVOFromCrateSupply != null)
						{
							ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(iconVOFromCrateSupply, element, true);
							projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
							ProjectorUtils.GenerateProjector(projectorConfig);
							element3.Tag = uid;
						}
						else
						{
							Service.Logger.ErrorFormat("EpisodeInfoScreen: Could not generate geometry for crate supply {0}", new object[]
							{
								optional.Uid
							});
						}
					}
					int value = 0;
					bool value2 = false;
					if (!complete)
					{
						if (optional.Type == SupplyType.ShardTroop || optional.Type == SupplyType.ShardSpecialAttack)
						{
							ShardVO optional2 = Service.StaticDataController.GetOptional<ShardVO>(optional.RewardUid);
							value = (int)optional2.Quality;
						}
						else if (optional.Type == SupplyType.Shard)
						{
							EquipmentVO currentEquipmentDataByID = ArmoryUtils.GetCurrentEquipmentDataByID(optional.RewardUid);
							value = (int)currentEquipmentDataByID.Quality;
							value2 = true;
						}
						else if (optional.Type == SupplyType.Troop || optional.Type == SupplyType.Hero || optional.Type == SupplyType.SpecialAttack)
						{
							value = Service.DeployableShardUnlockController.GetUpgradeQualityForDeployableUID(optional.RewardUid);
						}
					}
					Animator component = element2.Root.GetComponent<Animator>();
					if (component.isActiveAndEnabled)
					{
						component.SetInteger("Quality", value);
						component.SetBool("Equipment", value2);
					}
					else
					{
						Service.Logger.Error("EpisodeInfoScreen: Failed to show reward quality and equipment grid UI. Animator not set!");
					}
				}
				else
				{
					Service.Logger.ErrorFormat("EpisodeInfoScreen: Failed to populate reward UI, CrateSupplyVO is null for :{0}", new object[]
					{
						planetLootEntryVO.SupplyDataUid
					});
				}
			}
			else
			{
				string a2 = (string)element3.Tag;
				if (a2 != uid)
				{
					AnimState animState = (!complete) ? AnimState.Closed : AnimState.Idle;
					CrateVO optional3 = staticDataController.GetOptional<CrateVO>(rewardTaskVO.CrateId);
					if (optional3 != null)
					{
						RewardUtils.SetCrateIcon(element, optional3, animState);
						element3.Tag = uid;
					}
				}
			}
		}

		private void SetRewardState(int rewardIndex, bool complete)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			UXSprite sprite = this.significantTaskToReward[rewardIndex];
			string uid = this.significantTaskUids[rewardIndex];
			EpisodeTaskVO episodeTaskVO = staticDataController.Get<EpisodeTaskVO>(uid);
			CrateVO optional = staticDataController.GetOptional<CrateVO>(episodeTaskVO.CrateId);
			if (optional != null)
			{
				AnimState animState = (!complete) ? AnimState.Closed : AnimState.Idle;
				RewardUtils.SetCrateIcon(sprite, optional, animState);
			}
		}

		private void InitTaskProgress()
		{
			if (this.taskProgressInfo == null)
			{
				return;
			}
			EpisodeController episodeController = Service.EpisodeController;
			int count = this.taskProgressInfo.count;
			int target = this.taskProgressInfo.target;
			base.GetElement<UXLabel>("LabelTaskProgress").Text = this.lang.Get("FRACTION", new object[]
			{
				count,
				target
			});
			float value = 1f * (float)count / (float)target;
			UXSlider element = base.GetElement<UXSlider>("pBarTask");
			element.Value = value;
			UXLabel element2 = base.GetElement<UXLabel>("LabelChapter");
			element2.Text = this.lang.Get(this.taskActionVO.ActionName, new object[0]);
			UXLabel element3 = base.GetElement<UXLabel>("LabelTask");
			element3.Text = this.lang.Get(this.taskActionVO.ActionDesc, new object[]
			{
				this.taskProgressInfo.target
			});
			UXLabel element4 = base.GetElement<UXLabel>("LabelTaskComplete");
			element4.Text = this.lang.Get("FINISHED_REWARD_BUTTON", new object[0]);
			UXLabel element5 = base.GetElement<UXLabel>("LabelResearchComplete");
			element5.Text = this.lang.Get("FINISHED_REWARD_BUTTON", new object[0]);
			if (this.taskActionVO.Type != "EpisodePoint")
			{
				GoalType goalType = Service.EpisodeTaskManager.GetGoalType(this.taskActionVO);
				IGeometryVO iconVOFromGoalType = GameUtils.GetIconVOFromGoalType(goalType, this.taskActionVO.ActionIcon, this.taskProgressInfo.hq);
				this.complexTaskIcon = base.GetElement<UXSprite>("SpriteComplexTaskIcon");
				ProjectorConfig config = ProjectorUtils.GenerateGeometryConfig(iconVOFromGoalType, this.complexTaskIcon);
				ProjectorUtils.GenerateProjector(config);
			}
			UXLabel element6 = base.GetElement<UXLabel>("LabelTaskButtonDesc");
			if (episodeController.IsCurrentTaskTimeGated() && !episodeController.IsCurrentTaskProgressMarkedComplete())
			{
				element6.Text = this.lang.Get("EPISODE_START_RESEARCH_CTA", new object[0]);
				element6.Visible = true;
			}
			else
			{
				element6.Visible = false;
			}
			UXButton element7 = base.GetElement<UXButton>("BtnTask");
			UXLabel element8 = base.GetElement<UXLabel>("LabelBtnTask");
			int num = target - count;
			element8.Text = this.lang.ThousandsSeparated(target);
			UXUtils.UpdateCostColor(element8, null, 0, 0, 0, 0, 0, num, false);
			if (num > 0 && this.taskActionVO.IsSkippable)
			{
				element7.Visible = true;
				element7.OnClicked = new UXButtonClickedDelegate(this.OnTaskSkipButtonClicked);
			}
			else if (num <= 0 && !episodeController.IsCurrentTaskProgressMarkedComplete())
			{
				element7.Visible = true;
				element7.OnClicked = new UXButtonClickedDelegate(this.OnTaskCompleteButtonClicked);
			}
			else
			{
				element7.Visible = false;
			}
		}

		private void InitTimegateSection()
		{
			EpisodeController episodeController = Service.EpisodeController;
			bool visible = episodeController.IsCurrentTaskProgressMarkedComplete();
			bool flag = episodeController.IsTaskTimeGateActive();
			this.taskTimeRemainingLabel = base.GetElement<UXLabel>("LabelResearchTimer");
			this.taskTimeGateSlider = base.GetElement<UXSlider>("pBarTimer");
			UXLabel element = base.GetElement<UXLabel>("LabelResearch");
			element.Text = this.lang.Get("RESEARCH_TITLE", new object[0]);
			UXLabel element2 = base.GetElement<UXLabel>("LabelFinishInstant");
			element2.Text = this.lang.Get("context_Finish_Now", new object[0]);
			element2.Visible = flag;
			UXButton element3 = base.GetElement<UXButton>("BtnSkipResearch");
			UXButton element4 = base.GetElement<UXButton>("BtnTimer");
			element4.Visible = false;
			element3.Visible = false;
			if (this.taskTimeGateCC != null)
			{
				this.taskTimeGateCC.Destroy();
				this.taskTimeGateCC = null;
			}
			bool flag2 = false;
			if (flag)
			{
				flag2 = episodeController.IsTaskTimeGateComplete();
			}
			if (flag2)
			{
				return;
			}
			if (flag)
			{
				int timeGate = this.taskVO.TimeGate;
				uint endTimestamp = this.taskProgressInfo.endTimestamp;
				uint time = ServerTime.Time;
				int num = (int)(endTimestamp - time);
				this.taskTimeGateSlider.Value = Mathf.Max(0.02f, 1f * (float)(timeGate - num) / (float)timeGate);
				this.taskTimeGateCC = new CountdownControl(this.taskTimeRemainingLabel, this.lang.Get("RESEARCH_ENDS_IN", new object[0]), (int)endTimestamp);
				this.UpdateTimeGateCostUI(num);
				element3.Visible = true;
				element3.OnClicked = new UXButtonClickedDelegate(this.OnTimeGateSkipButtonClicked);
				Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
			}
			else
			{
				this.taskTimeGateSlider.Value = 0f;
				this.taskTimeRemainingLabel.Text = GameUtils.GetTimeLabelFromSeconds(this.taskVO.TimeGate);
				UXLabel element5 = base.GetElement<UXLabel>("LabelBtnTimer");
				element5.Text = this.lang.Get("BUTTON_MISSION_EVENT", new object[0]);
				element4.Visible = visible;
				element4.OnClicked = new UXButtonClickedDelegate(this.OnTimeGateTimerButtonClicked);
			}
		}

		private void InitRewardSection()
		{
			UXLabel element = base.GetElement<UXLabel>("LabelReward");
			element.Text = this.lang.Get("s_Rewards", new object[0]);
			UXLabel element2 = base.GetElement<UXLabel>("LabelRewardName");
			element2.Text = LangUtils.GetCrateDisplayName(this.taskCrateRewardVO);
			UXSprite element3 = base.GetElement<UXSprite>("SpriteRewardIcon");
			string a = (string)element3.Tag;
			if (a != this.taskCrateRewardVO.Uid)
			{
				RewardUtils.SetCrateIcon(element3, this.taskCrateRewardVO, AnimState.Closed);
				element3.Tag = this.taskCrateRewardVO.Uid;
			}
			element3.OnElementClicked = new UXButtonClickedDelegate(this.OnCrateClicked);
			UXLabel element4 = base.GetElement<UXLabel>("LabelBtnPrimary");
			element4.Text = this.lang.Get("CRATE_FLYOUT_OPEN_CTA", new object[0]);
			UXButton element5 = base.GetElement<UXButton>("BtnPrimary");
			element5.OnClicked = new UXButtonClickedDelegate(this.OnClaimRewardClicked);
			bool visible = false;
			EpisodeController episodeController = Service.EpisodeController;
			if (episodeController.IsCurrentTaskProgressMarkedComplete() && (!episodeController.IsCurrentTaskTimeGated() || episodeController.IsTaskTimeGateComplete()))
			{
				visible = true;
			}
			element5.Visible = visible;
		}

		private void InitEpisodeProgress()
		{
			if (this.progressInfo == null)
			{
				return;
			}
			int currentTaskIndex = this.progressInfo.currentTaskIndex;
			if (this.episodeDataVO != null)
			{
				int num = this.episodeDataVO.Tasks.Length;
				base.GetElement<UXLabel>("LabelEpisodeProgress").Text = this.lang.Get("FRACTION", new object[]
				{
					currentTaskIndex + 1,
					num
				});
			}
		}

		private void InitLabels()
		{
			if (this.taskVO == null)
			{
				Service.Logger.WarnFormat("No Episode Task data found for UI labels", new object[0]);
				return;
			}
			Lang lang = Service.Lang;
			base.GetElement<UXLabel>("LabelTitle").Text = lang.Get(this.panelVO.GetTitleString(this.faction), new object[0]);
			base.GetElement<UXLabel>("LabelEPHelp").Text = lang.Get("gui_event_points_help", new object[0]);
			UXLabel element = base.GetElement<UXLabel>("LabelExpiration");
			DateTime serverDateTime = Service.ServerAPI.ServerDateTime;
			TimeSpan timeSpan = this.expirationDate.Subtract(serverDateTime);
			this.btnLabelTimeGateCost = base.GetElement<UXLabel>("LabelBtnSkipResearch");
			this.expirationCC = new CountdownControl(element, lang.Get("EPISODE_ENDS_IN", new object[0]), DateUtils.GetSecondsFromEpoch(this.expirationDate));
			if (timeSpan.TotalSeconds < 432000.0)
			{
				this.expirationTimerId = Service.ViewTimerManager.CreateViewTimer((float)timeSpan.TotalSeconds, false, new TimerDelegate(this.HandleExpiration), null);
			}
		}

		private void HandleExpiration(uint timerId, object cookie)
		{
			this.Close(null);
		}

		private void OnGoToStoreClicked(UXButton button)
		{
			this.LogBI(EventId.EpisodeInfoScreenGotoStore, "store");
			Service.ScreenController.CloseAll();
			GameUtils.OpenStoreWithTab(StoreTab.Treasure);
		}

		private void OnPlayStoryActionClicked(UXButton button)
		{
			this.LogBI(EventId.EpisodeInfoScreenStoryAction, "more_info");
			Service.EpisodeController.PlayMostRecentStoryAction();
		}

		private void OnTaskSkipButtonClicked(UXButton button)
		{
			if (!this.taskActionVO.IsSkippable)
			{
				Service.Logger.ErrorFormat("Trying to skip a non skippable task action with uid: {0}, isSkippable: {1} ", new object[]
				{
					this.taskActionVO.Uid,
					this.taskActionVO.IsSkippable
				});
				return;
			}
			bool flag = this.taskProgressInfo.count >= this.taskProgressInfo.target;
			if (flag)
			{
				Service.Logger.Warn("Trying to skip an already completed task.");
				return;
			}
			int crystalCost = GameUtils.EpisodeTaskProgressToCrystals(this.taskProgressInfo.count, this.taskProgressInfo.target, this.taskActionVO);
			string title = this.lang.Get("EPISODE_SKIP_PROGRESS_TITLE", new object[0]);
			string message = this.lang.Get("EPISODE_SKIP_PROGRESS_BODY", new object[0]);
			FinishNowScreen.ShowModalEpisodeTask(this.taskVO, new OnScreenModalResult(this.OnTaskProgressFinishNowResult), this.taskVO.Uid, crystalCost, title, message, true);
		}

		private void OnTaskEPHelpButtonClicked(UXButton button)
		{
			this.LogBI(EventId.EpisodePointsHelpScreenOpened, "EP_info");
			EpisodePointsHelpScreen screen = new EpisodePointsHelpScreen();
			Service.ScreenController.AddScreen(screen);
		}

		private void OnTaskCompleteButtonClicked(UXButton button)
		{
			Service.EventManager.RegisterObserver(this, EventId.EpisodeTaskProgressCompleted);
			Service.EventManager.RegisterObserver(this, EventId.EpisodeTaskProgressCompleteFailed);
			ProcessingScreen.Show();
			Service.EpisodeController.CompleteTaskProgress();
		}

		private void OnTaskProgressFinishNowResult(object result, object cookie)
		{
			if (result == null)
			{
				return;
			}
			int crystals = GameUtils.EpisodeTaskProgressToCrystals(this.taskProgressInfo.count, this.taskProgressInfo.target, this.taskActionVO);
			if (!GameUtils.SpendCrystals(crystals))
			{
				return;
			}
			ProcessingScreen.Show();
			Service.EpisodeController.SkipTaskProgress();
			Service.EventManager.RegisterObserver(this, EventId.EpisodeTaskProgressSkipped);
			Service.EventManager.RegisterObserver(this, EventId.EpisodeTaskProgressSkipFailed);
		}

		private void OnTimeGateSkipButtonClicked(UXButton button)
		{
			EpisodeController episodeController = Service.EpisodeController;
			bool flag = episodeController.IsTaskTimeGateComplete();
			if (flag)
			{
				return;
			}
			uint endTimestamp = this.taskProgressInfo.endTimestamp;
			uint time = ServerTime.Time;
			int seconds = (int)(endTimestamp - time);
			int crystalCost = GameUtils.SecondsToCrystalsForEpisodeTaskTimeGate(seconds);
			string title = this.lang.Get("EPISODE_SKIP_TIMEGATE_TITLE", new object[0]);
			string message = this.lang.Get("EPISODE_SKIP_TIMEGATE_BODY", new object[0]);
			FinishNowScreen.ShowModalEpisodeTask(this.taskVO, new OnScreenModalResult(this.OnTimeGateFinishNowResult), this.taskVO.Uid, crystalCost, title, message, true);
		}

		private void OnTimeGateTimerButtonClicked(UXButton button)
		{
			EpisodeController episodeController = Service.EpisodeController;
			ProcessingScreen.Show();
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.EpisodeTaskTimeGateStarted);
			episodeController.StartTaskTimeGate();
		}

		private void OnTimeGateFinishNowResult(object result, object cookie)
		{
			if (result == null)
			{
				return;
			}
			uint endTimestamp = this.taskProgressInfo.endTimestamp;
			uint time = ServerTime.Time;
			int seconds = (int)(endTimestamp - time);
			int crystals = GameUtils.SecondsToCrystalsForEpisodeTaskTimeGate(seconds);
			if (!GameUtils.SpendCrystals(crystals))
			{
				return;
			}
			ProcessingScreen.Show();
			Service.EpisodeController.SkipTaskTimeGate();
			Service.EventManager.RegisterObserver(this, EventId.EpisodeTaskTimeGateSkipped);
		}

		private void OnClaimRewardClicked(UXButton button)
		{
			button.Visible = false;
			this.Animate("HideBottom");
			ProcessingScreen.Show();
			Service.EpisodeController.ClaimCurrentEpisodeTask();
			this.canRefresh = false;
			Service.EventManager.RegisterObserver(this, EventId.InventoryCrateCollectionClosed);
		}

		private void OnCrateClicked(UXButton button)
		{
			CrateInfoModalScreen crateInfoModalScreen = CrateInfoModalScreen.CreateForInfo(this.taskVO.CrateId, Service.CurrentPlayer.PlanetId);
			crateInfoModalScreen.IsAlwaysOnTop = true;
			Service.ScreenController.AddScreen(crateInfoModalScreen, true, false);
		}

		private void OnRewardInfoClicked(UXButton button)
		{
			string text = (string)button.Tag;
			if (text == null)
			{
				Service.Logger.Error("EpisodeInfoScreen: Failed to show task reward info");
				return;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			EpisodeTaskVO optional = staticDataController.GetOptional<EpisodeTaskVO>(text);
			List<PlanetLootEntryVO> featuredLootEntriesForEpisodeTask = Service.InventoryCrateRewardController.GetFeaturedLootEntriesForEpisodeTask(optional, 1);
			bool flag = false;
			if (featuredLootEntriesForEpisodeTask.Count != 0 && featuredLootEntriesForEpisodeTask[0] != null)
			{
				PlanetLootEntryVO planetLootEntryVO = featuredLootEntriesForEpisodeTask[0];
				CrateSupplyVO optional2 = staticDataController.GetOptional<CrateSupplyVO>(planetLootEntryVO.SupplyDataUid);
				if (optional2 != null)
				{
					flag = GameUtils.HandleCrateSupplyRewardClicked(optional2, false);
				}
			}
			if (!flag)
			{
				ClosableScreen closableScreen = CrateInfoModalScreen.CreateForInfo(optional.CrateId, Service.CurrentPlayer.PlanetId);
				closableScreen.IsAlwaysOnTop = true;
				Service.ScreenController.AddScreen(closableScreen, true, false);
			}
		}

		private void InitBackgroundTexture()
		{
			string backgroundTextureId = this.panelVO.GetBackgroundTextureId(this.faction);
			if (string.IsNullOrEmpty(backgroundTextureId))
			{
				return;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			UXTexture element = base.GetElement<UXTexture>("TextureTheme");
			TextureVO optional = staticDataController.GetOptional<TextureVO>(backgroundTextureId);
			if (optional == null)
			{
				Service.Logger.Warn("EpisodeInfoScreen: Could not find TextureVO: " + backgroundTextureId);
				return;
			}
			element.LoadTexture(optional.AssetName);
			this.InitWidgetBackgroundTextures();
		}

		private void InitWidgetBackgroundTextures()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			if (!string.IsNullOrEmpty(this.panelVO.ResearchTextureId))
			{
				TextureVO optional = staticDataController.GetOptional<TextureVO>(this.panelVO.ResearchTextureId);
				if (optional != null)
				{
					UXTexture element = base.GetElement<UXTexture>("TextureResearchBkg");
					element.LoadTexture(optional.AssetName);
				}
				else
				{
					Service.Logger.Warn("EpisodeInfoScreen: Could not find research background texture data: " + this.panelVO.ResearchTextureId);
				}
			}
			else
			{
				Service.Logger.Warn("EpisodeInfoScreen: Could not find research background texture id");
			}
			if (!string.IsNullOrEmpty(this.panelVO.RewardTextureId))
			{
				TextureVO optional2 = staticDataController.GetOptional<TextureVO>(this.panelVO.RewardTextureId);
				if (optional2 != null)
				{
					UXTexture element2 = base.GetElement<UXTexture>("TextureRewardBkg");
					element2.LoadTexture(optional2.AssetName);
				}
				else
				{
					Service.Logger.Warn("EpisodeInfoScreen: Could not find reward background texture data: " + this.panelVO.RewardTextureId);
				}
			}
			else
			{
				Service.Logger.Warn("EpisodeInfoScreen: Could not find reward background texture id");
			}
		}

		private void LogBI(EventId eventID, string messageAddition)
		{
			string text = string.Empty;
			if (this.episodeDataVO != null)
			{
				text += this.episodeDataVO.Uid;
			}
			text = text + "|" + messageAddition;
			Service.EventManager.SendEvent(eventID, text);
		}
	}
}
