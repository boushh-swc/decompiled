using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Goals;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Episodes;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Main.Views.UX.Screens.Leaderboard;
using StaRTS.Main.Views.UX.Screens.Squads;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Controls
{
	public class EpisodeWidgetView : IEventObserver, IViewClockTimeObserver, IViewFrameTimeObserver
	{
		private const string BUTTON_EPISODE = "Episode";

		private const string LABEL_NOTICE = "LabelNoticeEpisode";

		private const string LABEL_TITLE = "LabelTitleEpisode";

		private const string TEXTURE_BG = "TextureEpisodeBg";

		private const string TEXTURE_ICON = "TextureEpisodeIcon";

		private const string TEXTURE_BORDER = "ParticleBorder";

		private const string PROGRESS_BAR = "pBarEpisodeProgress";

		private const string PROGRESS_BAR_GOAL = "pBarEpisodeProgressDelta";

		private const string PROGRESS_TITLE = "LabelEpisodeProgress";

		private const string PROGRESS_TOASTER = "WidgetEpisodeProgress";

		private const string LABEL_COMPLETE_STATUS = "LabelStatus";

		private const string LABEL_COMPLETE_CTA = "LabelCTA";

		private const string WIDGET_CIRCLE_TIMER = "WidgetCircleTimer";

		private const string PROGRESS_BAR_CIRCLE_TIMER = "pBarCircleTimer";

		private const string ENDS_IN = "ENDS_IN";

		private const string EPISODE_NEW_CTA = "EPISODE_NEW_EPISODE_CTA";

		private const string TIMEGATE_TASK_COMPLETE = "TIMEGATE_TASK_COMPLETE";

		private const string TIMEGATE_START = "TIMEGATE_START";

		private const string TIMEGATE_TASK_COMPLETE_START_RESEARCH = "TIMEGATE_TASK_COMPLETE_START_RESEARCH";

		private const string TIMEGATE_TOTAL_TIME = "TIMEGATE_TOTAL_TIME";

		private const string TIMEGATE_ENDS_IN = "RESEARCH_ENDS_IN";

		private const string TIMEGATE_ENDED_COLLECT_REWARDS = "TIMEGATE_ENDED_COLLECT_REWARDS";

		private const string TOAST_COLLECT = "gui_event_reward_available";

		private const string PROGRESS_DESC_RAID = "epw_event_point_raid_";

		private const string PROGRESS_DESC_PVP = "epw_event_point_pvp_";

		private const string PROGRESS_DESC_OBJECTIVE = "epw_event_point_obje_";

		private const string PROGRESS_DESC_CONFLICT = "epw_event_point_conf_";

		private const string PROGRESS_DESC_GENERIC = "epw_event_points_generic_";

		private const string EPISODE_COLLECT_ICON_DEFAULT = "episode_hudicon_crate_ready_bronzium";

		private const float TIMEGATE_TOAST_HIDE_DELAY = 2f;

		private const float TIMEGATE_TOAST_HIDE_COMPLETE = 1f;

		private const string ANIM_TRIG_HIDE = "Hide";

		private const string ANIM_TRIG_SHOW = "Show";

		private const string ANIM_TRIG_SHOW_PROGRESS = "ShowProgress";

		private const string ANIM_TRIG_HIDE_PROGRESS = "HideProgress";

		private const string ANIM_TRIG_SHOW_TIMER = "ShowTimer";

		private const string ANIM_TRIG_HIDE_TIMER = "HideTimer";

		private const string ANIM_TRIG_RESET = "Reset";

		private const string ANIM_TRIG_SHOW_COMPLETE = "Animate2CTA";

		private const string ANIM_TRIG_SET_COMPLETE = "SetCTA";

		private const float PROGRESS_HIDE_COMPLETE = 1f;

		private const float PROGRESS_SLIDER_DELAY = 1.5f;

		private const float PROGRESS_SLIDER_CURRENT_DELAY = 0.5f;

		private const float PROGRESS_NEXT_DELAY = 1f;

		private UXButton widgetBtn;

		private UXLabel titleLabel;

		private int titleInitAnchorOffsetLeft;

		private UXLabel noticeLabel;

		private int noticeInitAnchorOffsetLeft;

		private UXLabel titleLabelComplete;

		private UXLabel noticeLabelComplete;

		private UXTexture bgTexture;

		private UXTexture iconTexture;

		private UXElement borderTexture;

		private DateTime expirationDate;

		private CountdownControl noticeCC;

		private UXElement widgetCircleTimer;

		private UXSlider pBarCircleTimer;

		private UXSlider progressSlider;

		private UXSlider progressSliderGoal;

		private UXLabel progressLabel;

		private UXElement progressEarnedToast;

		private uint hideProgressTimerId;

		private uint hideProgressCompleteTimerId;

		private uint animateProgressTimerId;

		private uint timeGateEndTimestamp;

		private string loadedIconTextureId;

		private EpisodeWidgetDataVO currentWidgetVO;

		private string lastState;

		private Lang lang;

		private HUDResourceView animatingProgress;

		private bool showTimer;

		private bool needToShowCTA;

		private bool showingCTA;

		public EpisodeWidgetView(UXFactory factory)
		{
			this.widgetBtn = factory.GetElement<UXButton>("Episode");
			this.widgetBtn.OnClicked = new UXButtonClickedDelegate(this.OnWidgetClicked);
			this.titleLabel = factory.GetElement<UXLabel>("LabelTitleEpisode");
			this.noticeLabel = factory.GetElement<UXLabel>("LabelNoticeEpisode");
			this.titleLabelComplete = factory.GetElement<UXLabel>("LabelStatus");
			this.noticeLabelComplete = factory.GetElement<UXLabel>("LabelCTA");
			this.bgTexture = factory.GetElement<UXTexture>("TextureEpisodeBg");
			this.iconTexture = factory.GetElement<UXTexture>("TextureEpisodeIcon");
			this.borderTexture = factory.GetElement<UXElement>("ParticleBorder");
			this.titleInitAnchorOffsetLeft = this.titleLabel.GetLabelAnchorOffset(UXAnchorSection.Left);
			this.noticeInitAnchorOffsetLeft = this.noticeLabel.GetLabelAnchorOffset(UXAnchorSection.Left);
			this.widgetCircleTimer = factory.GetElement<UXElement>("WidgetCircleTimer");
			this.pBarCircleTimer = factory.GetElement<UXSlider>("pBarCircleTimer");
			this.progressSlider = factory.GetElement<UXSlider>("pBarEpisodeProgress");
			this.progressSliderGoal = factory.GetElement<UXSlider>("pBarEpisodeProgressDelta");
			this.progressLabel = factory.GetElement<UXLabel>("LabelEpisodeProgress");
			this.progressEarnedToast = factory.GetElement<UXElement>("WidgetEpisodeProgress");
			this.progressEarnedToast.Visible = false;
			this.showTimer = false;
			this.needToShowCTA = false;
			this.showingCTA = false;
			this.lang = Service.Lang;
			this.ForceHide();
		}

		public void OnViewFrameTime(float dt)
		{
			if (this.animatingProgress != null)
			{
				if (this.animatingProgress.NeedsUpdate)
				{
					this.animatingProgress.Update(dt);
				}
				else
				{
					this.animatingProgress = null;
					this.InvalidateHideProgressTimers();
					this.hideProgressTimerId = Service.ViewTimerManager.CreateViewTimer(1f, false, new TimerDelegate(this.NextProgressCallback), null);
				}
			}
			Animator component = this.widgetBtn.Root.GetComponent<Animator>();
			if (this.showTimer && component.isActiveAndEnabled)
			{
				this.AnimResetTrigger(component, "ShowTimer");
				this.AnimResetTrigger(component, "HideTimer");
				this.AnimSetTrigger(component, "ShowTimer");
				this.showTimer = false;
			}
			if (!this.showTimer && this.animatingProgress == null)
			{
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
		}

		public void AnimResetTrigger(Animator anim, string triggerName)
		{
			if (!Service.UXController.HUD.Visible)
			{
				return;
			}
			anim.ResetTrigger(triggerName);
		}

		public void AnimSetTrigger(Animator anim, string triggerName)
		{
			if (!Service.UXController.HUD.Visible)
			{
				return;
			}
			anim.SetTrigger(triggerName);
		}

		public void OnViewClockTime(float dt)
		{
			uint time = ServerTime.Time;
			int num = (int)(this.timeGateEndTimestamp - time);
			if (num <= 0)
			{
				Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
				Service.EventManager.SendEvent(EventId.EpisodeProgressInfoRefreshed, null);
			}
		}

		private void ShowTimerIcon()
		{
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			this.showTimer = true;
		}

		private void HideTimerIcon()
		{
			this.showTimer = false;
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			Animator component = this.widgetBtn.Root.GetComponent<Animator>();
			if (component.isActiveAndEnabled)
			{
				this.AnimSetTrigger(component, "HideTimer");
			}
		}

		private string GetTaskCrateIcon()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			EpisodeProgressInfo episodeProgressInfo = Service.CurrentPlayer.EpisodeProgressInfo;
			if (episodeProgressInfo == null)
			{
				Service.Logger.WarnFormat("GetTaskCrateIcon: No episode progress found", new object[0]);
				return "episode_hudicon_crate_ready_bronzium";
			}
			EpisodeTaskProgressInfo currentTask = episodeProgressInfo.currentTask;
			if (currentTask == null)
			{
				Service.Logger.WarnFormat("GetTaskCrateIcon: No current task found", new object[0]);
				return "episode_hudicon_crate_ready_bronzium";
			}
			EpisodeTaskVO optional = staticDataController.GetOptional<EpisodeTaskVO>(currentTask.uid);
			if (optional == null)
			{
				Service.Logger.WarnFormat("GetTaskCrateIcon: Missing task data for {0}", new object[]
				{
					currentTask.uid
				});
				return "episode_hudicon_crate_ready_bronzium";
			}
			if (string.IsNullOrEmpty(optional.CrateIconAsset))
			{
				return "episode_hudicon_crate_ready_bronzium";
			}
			return optional.CrateIconAsset;
		}

		public void SetWidgetData(EpisodeWidgetDataVO vo, DateTime endDate, bool isCurrentTaskProgressComplete, bool isCurrentTaskTimeGated, bool isTimeGateActive, bool isTaskTimeGateComplete, int currentTaskTimeGate, uint taskProgressInfoEndTime, bool isNewEpisode)
		{
			this.currentWidgetVO = vo;
			StaticDataController staticDataController = Service.StaticDataController;
			EpisodeWidgetStateVO optional = staticDataController.GetOptional<EpisodeWidgetStateVO>(this.currentWidgetVO.StateId);
			if (optional == null)
			{
				Service.Logger.Warn("Could not find EpisodeWidgetState: " + this.currentWidgetVO.StateId + " for EpisodeWidget: " + this.currentWidgetVO.Uid);
				return;
			}
			this.expirationDate = endDate;
			FactionType faction = Service.CurrentPlayer.Faction;
			this.titleLabel.Visible = true;
			this.titleLabel.Text = this.lang.Get(optional.GetTitleString(faction), new object[0]);
			this.titleLabelComplete.Text = this.lang.Get(optional.GetTitleString(faction), new object[0]);
			string backgroundTextureId = optional.GetBackgroundTextureId(faction);
			if (!string.IsNullOrEmpty(backgroundTextureId))
			{
				Service.AssetManager.RegisterPreloadableAsset(backgroundTextureId);
				this.LoadTexture(this.bgTexture, backgroundTextureId);
			}
			string text = optional.GetIconTextureId(faction);
			if (this.noticeCC != null)
			{
				this.noticeCC.Destroy();
				this.noticeCC = null;
			}
			this.iconTexture.Visible = false;
			this.noticeLabel.Visible = false;
			this.widgetCircleTimer.Visible = false;
			string text2 = string.Empty;
			string text3 = string.Empty;
			string text4 = this.lang.Get("TIMEGATE_ENDED_COLLECT_REWARDS", new object[0]);
			this.HideTimerIcon();
			this.needToShowCTA = false;
			if (isCurrentTaskProgressComplete && isCurrentTaskTimeGated && !isTimeGateActive && !isTaskTimeGateComplete)
			{
				text3 = this.lang.Get("TIMEGATE_TASK_COMPLETE_START_RESEARCH", new object[0]);
				this.needToShowCTA = true;
			}
			else if (isCurrentTaskProgressComplete && isCurrentTaskTimeGated && isTimeGateActive && !isTaskTimeGateComplete)
			{
				this.timeGateEndTimestamp = taskProgressInfoEndTime;
				uint time = ServerTime.Time;
				int num = (int)(this.timeGateEndTimestamp - time);
				float value = 1f * (float)(currentTaskTimeGate - num) / (float)currentTaskTimeGate;
				string text5 = this.lang.Get("RESEARCH_ENDS_IN", new object[0]);
				string timeLabelFromSeconds = GameUtils.GetTimeLabelFromSeconds(num);
				this.pBarCircleTimer.Value = value;
				text = null;
				this.widgetCircleTimer.Visible = true;
				this.noticeCC = new CountdownControl(this.pBarCircleTimer, this.noticeLabel, text5, (int)this.timeGateEndTimestamp, currentTaskTimeGate, text4);
				text2 = string.Format(text5, timeLabelFromSeconds);
				this.ShowTimerIcon();
				Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
			}
			else if ((isCurrentTaskProgressComplete && !isCurrentTaskTimeGated) || (isCurrentTaskProgressComplete && isCurrentTaskTimeGated && isTimeGateActive && isTaskTimeGateComplete))
			{
				text3 = text4;
				this.needToShowCTA = true;
				text = this.GetTaskCrateIcon();
				if (string.Compare(this.loadedIconTextureId, text) != 0)
				{
					this.iconTexture.MainTexture = null;
				}
			}
			else if (optional.ShowTimer)
			{
				this.noticeLabel.Visible = true;
				this.noticeCC = new CountdownControl(this.noticeLabel, this.lang.Get("ENDS_IN", new object[0]), DateUtils.GetSecondsFromEpoch(this.expirationDate));
			}
			else if (isNewEpisode)
			{
				text3 = this.lang.Get("EPISODE_NEW_EPISODE_CTA", new object[0]);
				this.needToShowCTA = true;
			}
			if (!string.IsNullOrEmpty(text2))
			{
				this.noticeLabel.Visible = true;
				this.noticeLabel.Text = text2;
			}
			this.noticeLabelComplete.Text = text3;
			if (!string.IsNullOrEmpty(text))
			{
				this.iconTexture.Visible = true;
				Service.AssetManager.RegisterPreloadableAsset(text);
				this.LoadTexture(this.iconTexture, text);
				this.loadedIconTextureId = text;
				this.ReadjustAnchors(true);
			}
			else
			{
				this.ReadjustAnchors(false);
			}
			if (this.needToShowCTA)
			{
				if (!this.showingCTA)
				{
					this.ShowCTA();
				}
			}
			else if (this.showingCTA)
			{
				bool flag = this.ResetAnims();
				this.showingCTA = !flag;
			}
		}

		private bool UpdateTaskProgressUI(EpisodeProgressData progressData)
		{
			if (progressData == null)
			{
				Service.Logger.Error("EpisodeWidgetView: no progress data given to show");
				return false;
			}
			string arg = string.Empty;
			EventId progressType = progressData.progressType;
			if (progressType != EventId.PvpBattleWon)
			{
				if (progressType != EventId.TournamentTierReached)
				{
					if (progressType != EventId.RaidComplete)
					{
						if (progressType != EventId.ObjectiveCompleted)
						{
							if (progressType != EventId.EpisodeComplexTask)
							{
								Service.Logger.ErrorFormat("Unknown event type {0} when displaying episode points", new object[]
								{
									progressData.progressType
								});
								return false;
							}
							arg = "epw_event_points_generic_";
						}
						else
						{
							arg = "epw_event_point_obje_";
						}
					}
					else
					{
						arg = "epw_event_point_raid_";
					}
				}
				else
				{
					arg = "epw_event_point_conf_";
				}
			}
			else
			{
				arg = "epw_event_point_pvp_";
			}
			this.progressLabel.Text = this.lang.Get(arg + progressData.progressIndex, new object[]
			{
				progressData.progress
			});
			EpisodeProgressInfo episodeProgressInfo = Service.CurrentPlayer.EpisodeProgressInfo;
			EpisodeTaskProgressInfo currentTask = episodeProgressInfo.currentTask;
			int target = currentTask.target;
			this.animatingProgress = new HUDResourceView("episodeProgress", this.progressSlider, null, null);
			this.animatingProgress.SetAmount(progressData.prevProgress, target, false);
			float delay = 0f;
			if (!this.progressEarnedToast.Visible)
			{
				this.progressSliderGoal.Value = (float)progressData.prevProgress / (float)target;
				delay = 1.5f;
			}
			KeyValuePair<int, int> keyValuePair = new KeyValuePair<int, int>(progressData.prevProgress + progressData.progress, target);
			this.animateProgressTimerId = Service.ViewTimerManager.CreateViewTimer(delay, false, new TimerDelegate(this.AnimateProgressStartCallback), keyValuePair);
			return true;
		}

		private void AnimateProgressStartCallback(uint id, object cookie)
		{
			KeyValuePair<int, int> keyValuePair = (KeyValuePair<int, int>)cookie;
			float value = (float)keyValuePair.Key / (float)keyValuePair.Value;
			this.progressSliderGoal.Value = value;
			this.animateProgressTimerId = Service.ViewTimerManager.CreateViewTimer(0.5f, false, new TimerDelegate(this.AnimateProgressContinueCallback), cookie);
		}

		private void AnimateProgressContinueCallback(uint id, object cookie)
		{
			this.animateProgressTimerId = 0u;
			KeyValuePair<int, int> keyValuePair = (KeyValuePair<int, int>)cookie;
			if (this.animatingProgress != null)
			{
				this.animatingProgress.SetAmount(keyValuePair.Key, keyValuePair.Value, true);
			}
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		private void UpdateParticleEffectVisibility()
		{
			ScreenController screenController = Service.ScreenController;
			MiscElementsManager miscElementsManager = Service.UXController.MiscElementsManager;
			SquadSlidingScreen highestLevelScreen = screenController.GetHighestLevelScreen<SquadSlidingScreen>();
			bool effectsVisible = !miscElementsManager.IsHudFactionIconTooltipVisible() && screenController.GetHighestLevelScreen<EpisodeInfoScreen>() == null && screenController.GetHighestLevelScreen<InventoryCrateCollectionScreen>() == null && screenController.GetHighestLevelScreen<PrizeInventoryScreen>() == null && screenController.GetHighestLevelScreen<SquadJoinScreen>() == null && (highestLevelScreen == null || !highestLevelScreen.IsOpen());
			this.UpdateParticleEffectVisibility(effectsVisible);
		}

		private void UpdateParticleEffectVisibility(bool effectsVisible)
		{
			this.iconTexture.Visible = effectsVisible;
			this.borderTexture.Visible = effectsVisible;
		}

		private void ResetAllAnims()
		{
			if (!Service.UXController.HUD.Visible)
			{
				return;
			}
			Animator component = this.widgetBtn.Root.GetComponent<Animator>();
			this.AnimResetTrigger(component, "Hide");
			this.AnimResetTrigger(component, "Show");
			this.AnimResetTrigger(component, "ShowProgress");
			this.AnimResetTrigger(component, "HideProgress");
			this.AnimResetTrigger(component, "ShowTimer");
			this.AnimResetTrigger(component, "HideTimer");
			this.AnimResetTrigger(component, "Reset");
			this.AnimResetTrigger(component, "Animate2CTA");
			this.AnimResetTrigger(component, "SetCTA");
		}

		public void Show(bool showingProgress)
		{
			this.widgetBtn.Visible = true;
			Animator validAnimator = this.GetValidAnimator();
			if (validAnimator == null)
			{
				return;
			}
			if (this.lastState == "Show")
			{
				return;
			}
			this.UpdateParticleEffectVisibility();
			Service.EventManager.RegisterObserver(this, EventId.HUDFactionTooltipVisible);
			Service.EventManager.RegisterObserver(this, EventId.EpisodeInfoScreenOpened);
			Service.EventManager.RegisterObserver(this, EventId.EpisodeInfoScreenRefreshing);
			Service.EventManager.RegisterObserver(this, EventId.ScreenClosing);
			Service.EventManager.RegisterObserver(this, EventId.ScreenLoaded);
			Service.EventManager.RegisterObserver(this, EventId.ScreenClosed);
			Service.EventManager.RegisterObserver(this, EventId.HUDVisibilityChanged);
			if (this.needToShowCTA)
			{
				this.AnimSetTrigger(validAnimator, "Show");
				this.lastState = "Show";
				if (!showingProgress)
				{
					this.ShowCTA();
				}
			}
			else if (this.showingCTA)
			{
				bool flag = this.ResetAnims();
				this.showingCTA = !flag;
			}
			else
			{
				this.AnimSetTrigger(validAnimator, "Show");
				this.lastState = "Show";
			}
			Service.UXController.MiscElementsManager.SetEventTickerViewVisible(false);
		}

		public bool ShowProgress(EpisodeProgressData progressData)
		{
			Animator component = this.widgetBtn.Root.GetComponent<Animator>();
			if (!this.widgetBtn.Visible || !component.isActiveAndEnabled)
			{
				return false;
			}
			if (this.lastState != "Show")
			{
				return false;
			}
			if (Service.WorldTransitioner.IsTransitioning())
			{
				return false;
			}
			EpisodeProgressInfo episodeProgressInfo = Service.CurrentPlayer.EpisodeProgressInfo;
			EpisodeTaskProgressInfo currentTask = episodeProgressInfo.currentTask;
			if (progressData == null || progressData.progress <= 0 || progressData.episodeUid != episodeProgressInfo.uid || progressData.taskUid != currentTask.uid)
			{
				return false;
			}
			if (this.hideProgressTimerId != 0u || this.hideProgressCompleteTimerId != 0u)
			{
				return false;
			}
			if (!this.UpdateTaskProgressUI(progressData))
			{
				return false;
			}
			if (!this.progressEarnedToast.Visible)
			{
				this.ResetAllAnims();
				this.AnimSetTrigger(component, "ShowProgress");
				this.progressEarnedToast.Visible = true;
			}
			return true;
		}

		private void InvalidateHideProgressTimers()
		{
			if (this.hideProgressTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.hideProgressTimerId);
				this.hideProgressTimerId = 0u;
			}
			if (this.hideProgressCompleteTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.hideProgressCompleteTimerId);
				this.hideProgressCompleteTimerId = 0u;
			}
			if (this.animateProgressTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.animateProgressTimerId);
				this.animateProgressTimerId = 0u;
			}
		}

		private void NextProgressCallback(uint id, object cookie)
		{
			this.hideProgressTimerId = 0u;
			if (this.widgetBtn == null || this.widgetBtn.Root == null)
			{
				return;
			}
			Animator component = this.widgetBtn.Root.GetComponent<Animator>();
			if (component == null)
			{
				return;
			}
			if (!component.isActiveAndEnabled)
			{
				this.progressEarnedToast.Visible = false;
				return;
			}
			Service.EventManager.SendEvent(EventId.EpisodeProgressWidgetDone, null);
		}

		public void HideProgress()
		{
			Animator validAnimator = this.GetValidAnimator();
			if (validAnimator == null)
			{
				return;
			}
			this.AnimResetTrigger(validAnimator, "ShowProgress");
			this.AnimResetTrigger(validAnimator, "HideProgress");
			this.AnimSetTrigger(validAnimator, "HideProgress");
			this.hideProgressCompleteTimerId = Service.ViewTimerManager.CreateViewTimer(1f, false, new TimerDelegate(this.HideProgressCompleteCallback), null);
		}

		public bool ShowCTA()
		{
			if (!this.needToShowCTA)
			{
				return false;
			}
			Animator validAnimator = this.GetValidAnimator();
			if (validAnimator == null)
			{
				return false;
			}
			if (this.lastState == "Hide")
			{
				return false;
			}
			this.AnimResetTrigger(validAnimator, "Animate2CTA");
			this.AnimSetTrigger(validAnimator, "Animate2CTA");
			this.showingCTA = true;
			return true;
		}

		private Animator GetValidAnimator()
		{
			Animator component = this.widgetBtn.Root.GetComponent<Animator>();
			if (component != null && component.isActiveAndEnabled)
			{
				return component;
			}
			return null;
		}

		public void SetCTA()
		{
			Animator validAnimator = this.GetValidAnimator();
			if (validAnimator == null)
			{
				return;
			}
			this.AnimResetTrigger(validAnimator, "SetCTA");
			this.AnimSetTrigger(validAnimator, "SetCTA");
		}

		public bool ResetAnims()
		{
			Animator validAnimator = this.GetValidAnimator();
			if (validAnimator == null)
			{
				return false;
			}
			if (this.lastState == "Hide")
			{
				return true;
			}
			this.ResetAllAnims();
			this.HideTimerIcon();
			this.AnimSetTrigger(validAnimator, "Reset");
			this.lastState = "Hide";
			return true;
		}

		private void HideProgressCompleteCallback(uint id, object cookie)
		{
			this.hideProgressCompleteTimerId = 0u;
			this.progressEarnedToast.Visible = false;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.ScreenClosing:
				if (cookie is EpisodeInfoScreen)
				{
					this.UpdateParticleEffectVisibility(true);
				}
				return EatResponse.NotEaten;
			case EventId.ScreenClosed:
			case EventId.ScreenLoaded:
				if (cookie is SettingsScreen || cookie is PrizeInventoryScreen || cookie is SquadJoinScreen)
				{
					this.UpdateParticleEffectVisibility(id == EventId.ScreenClosed);
				}
				return EatResponse.NotEaten;
			case EventId.ScreenOverlayClosing:
			case EventId.ScreenRefreshed:
			case EventId.HUDVisibilityChanging:
				IL_2A:
				if (id == EventId.HUDFactionTooltipVisible)
				{
					this.UpdateParticleEffectVisibility();
					return EatResponse.NotEaten;
				}
				if (id != EventId.EpisodeInfoScreenRefreshing && id != EventId.EpisodeInfoScreenOpened)
				{
					return EatResponse.NotEaten;
				}
				this.UpdateParticleEffectVisibility(false);
				return EatResponse.NotEaten;
			case EventId.HUDVisibilityChanged:
				if (Service.UXController.HUD.Visible)
				{
					this.UpdateParticleEffectVisibility();
				}
				return EatResponse.NotEaten;
			}
			goto IL_2A;
		}

		private void ForceHide()
		{
			this.Hide();
			if (this.widgetBtn != null)
			{
				this.widgetBtn.Visible = false;
			}
		}

		public void Hide()
		{
			this.Hide(true);
		}

		public void Hide(bool checkLastState)
		{
			Animator component = this.widgetBtn.Root.GetComponent<Animator>();
			IState currentState = Service.GameStateMachine.CurrentState;
			if (currentState is BattleStartState)
			{
				this.widgetBtn.Visible = false;
			}
			if (!component.isActiveAndEnabled)
			{
				return;
			}
			if (checkLastState && this.lastState == "Hide")
			{
				return;
			}
			this.ResetAllAnims();
			component.SetTrigger("Hide");
			this.lastState = "Hide";
			this.InvalidateHideProgressTimers();
			Service.EventManager.UnregisterObserver(this, EventId.HUDFactionTooltipVisible);
			Service.EventManager.UnregisterObserver(this, EventId.EpisodeInfoScreenOpened);
			Service.EventManager.UnregisterObserver(this, EventId.EpisodeInfoScreenRefreshing);
			Service.EventManager.UnregisterObserver(this, EventId.ScreenClosing);
			Service.EventManager.UnregisterObserver(this, EventId.ScreenLoaded);
			Service.EventManager.UnregisterObserver(this, EventId.ScreenClosed);
			Service.EventManager.UnregisterObserver(this, EventId.HUDVisibilityChanged);
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
		}

		public void Reset()
		{
			this.ResetAnims();
			this.lastState = "Hide";
		}

		private void ReadjustAnchors(bool forceIconVisible)
		{
			if (!this.iconTexture.Visible && !this.widgetCircleTimer.Visible && !forceIconVisible)
			{
				int labelAnchorOffset = this.titleLabel.GetLabelAnchorOffset(UXAnchorSection.Right);
				this.titleLabel.SetLabelAnchorOffset(UXAnchorSection.Left, -labelAnchorOffset);
				int labelAnchorOffset2 = this.noticeLabel.GetLabelAnchorOffset(UXAnchorSection.Right);
				this.noticeLabel.SetLabelAnchorOffset(UXAnchorSection.Left, -labelAnchorOffset2);
				if (this.noticeLabel.Visible)
				{
					this.titleLabel.Pivot = UIWidget.Pivot.Top;
					this.noticeLabel.Pivot = UIWidget.Pivot.Bottom;
				}
				else
				{
					this.titleLabel.Pivot = UIWidget.Pivot.Center;
				}
			}
			else
			{
				this.titleLabel.SetLabelAnchorOffset(UXAnchorSection.Left, this.titleInitAnchorOffsetLeft);
				this.noticeLabel.SetLabelAnchorOffset(UXAnchorSection.Left, this.noticeInitAnchorOffsetLeft);
				if (this.noticeLabel.Visible)
				{
					this.titleLabel.Pivot = UIWidget.Pivot.TopLeft;
					this.noticeLabel.Pivot = UIWidget.Pivot.BottomLeft;
				}
				else
				{
					this.titleLabel.Pivot = UIWidget.Pivot.Left;
				}
			}
		}

		private void LoadTexture(UXTexture texture, string textureID)
		{
			if (!string.IsNullOrEmpty(textureID))
			{
				texture.Visible = true;
				TextureVO optional = Service.StaticDataController.GetOptional<TextureVO>(textureID);
				if (optional != null)
				{
					texture.LoadTexture(optional.AssetName);
				}
			}
			else
			{
				texture.MainTexture = null;
				texture.Visible = false;
			}
		}

		private void OnWidgetClicked(UXButton button)
		{
			bool flag = Service.EpisodeController.PlayIntroStoryAction();
			if (flag)
			{
				return;
			}
			Service.EpisodeController.AttemptToShowActiveEpisodeInfoScreen();
		}

		public void RefreshCTAState()
		{
			this.needToShowCTA = !Service.CurrentPlayer.EpisodeProgressInfo.introStoryViewed;
		}
	}
}
