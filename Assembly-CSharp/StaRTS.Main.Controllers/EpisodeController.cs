using StaRTS.Externals.Manimal;
using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models.Commands;
using StaRTS.Main.Models.Commands.Episodes;
using StaRTS.Main.Models.Episodes;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Story;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StaRTS.Main.Controllers
{
	public class EpisodeController : IEventObserver
	{
		public TaskViewState PreviousViewState;

		private DateTime nextScheduledRefreshDate;

		private uint scheduleTimerId;

		[CompilerGenerated]
		private static Comparison<EpisodePointScheduleVO> <>f__mg$cache0;

		public EpisodeDataVO CurrentEpisodeData
		{
			get;
			private set;
		}

		public EpisodeWidgetDataVO CurrentWidgetData
		{
			get;
			private set;
		}

		public EpisodeController()
		{
			this.CurrentEpisodeData = null;
			this.CurrentWidgetData = null;
			this.nextScheduledRefreshDate = DateTime.MinValue;
			Service.EpisodeController = this;
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.WorldLoadComplete);
			this.RefreshEpisodeStates();
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			EventManager eventManager = Service.EventManager;
			if (id != EventId.WorldLoadComplete)
			{
				if (id != EventId.GameStateChanged)
				{
					if (id != EventId.StoryChainCompleted)
					{
						if (id == EventId.FueCompleted)
						{
							eventManager.UnregisterObserver(this, EventId.FueCompleted);
							this.RefreshEpisodeStates();
						}
					}
					else
					{
						eventManager.UnregisterObserver(this, EventId.StoryChainCompleted);
						this.AttemptToShowActiveEpisodeInfoScreen();
					}
				}
				else if (this.IsValidGameStateForRefresh())
				{
					eventManager.UnregisterObserver(this, EventId.GameStateChanged);
					this.RefreshEpisodeStates();
				}
			}
			else
			{
				this.RefreshEpisodeStates();
				eventManager.UnregisterObserver(this, EventId.WorldLoadComplete);
			}
			return EatResponse.NotEaten;
		}

		public EpisodePointScaleVO GetCurrentEpisodePointScaleVO()
		{
			if (this.CurrentEpisodeData == null)
			{
				return null;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			EpisodePointScaleVO result = staticDataController.Get<EpisodePointScaleVO>(this.CurrentEpisodeData.PointScale);
			List<EpisodePointScheduleVO> list = new List<EpisodePointScheduleVO>();
			foreach (EpisodePointScheduleVO current in staticDataController.GetAll<EpisodePointScheduleVO>())
			{
				EpisodePointScheduleVO episodePointScheduleVO = current;
				if ((long)episodePointScheduleVO.EndTimeEpochSecs > (long)((ulong)ServerTime.Time))
				{
					if ((long)episodePointScheduleVO.StartTimeEpochSecs <= (long)((ulong)ServerTime.Time))
					{
						list.Add(episodePointScheduleVO);
					}
				}
			}
			if (list.Count > 0)
			{
				List<EpisodePointScheduleVO> arg_C9_0 = list;
				if (EpisodeController.<>f__mg$cache0 == null)
				{
					EpisodeController.<>f__mg$cache0 = new Comparison<EpisodePointScheduleVO>(EpisodeController.CompareSchedules);
				}
				arg_C9_0.Sort(EpisodeController.<>f__mg$cache0);
				EpisodePointScaleVO optional = staticDataController.GetOptional<EpisodePointScaleVO>(list[0].ScaleId);
				if (optional != null)
				{
					return optional;
				}
			}
			return result;
		}

		private static int CompareSchedules(EpisodePointScheduleVO a, EpisodePointScheduleVO b)
		{
			if (a == b)
			{
				return 0;
			}
			if (a.Priority != b.Priority)
			{
				return (a.Priority <= b.Priority) ? -1 : 1;
			}
			if (a.StartTimeEpochSecs != b.StartTimeEpochSecs)
			{
				return (a.StartTimeEpochSecs <= b.StartTimeEpochSecs) ? -1 : 1;
			}
			return 0;
		}

		public bool IsEpisodeWidgetActive()
		{
			return !Service.CurrentPlayer.CampaignProgress.FueInProgress && this.CurrentWidgetData != null && this.IsEpisodeActive();
		}

		public bool IsEpisodeActive()
		{
			return this.CurrentEpisodeData != null && Service.ServerAPI.ServerDateTime < this.CurrentEpisodeData.EndTime;
		}

		public bool IsEpisodeComplete()
		{
			if (this.CurrentEpisodeData == null)
			{
				return false;
			}
			EpisodeProgressInfo episodeProgressInfo = Service.CurrentPlayer.EpisodeProgressInfo;
			bool flag = episodeProgressInfo.currentTaskIndex >= this.CurrentEpisodeData.Tasks.Length - 1;
			bool flag2 = episodeProgressInfo.currentTask == null;
			return flag && flag2;
		}

		public bool IsCurrentTaskProgressComplete()
		{
			if (this.CurrentEpisodeData == null)
			{
				return false;
			}
			EpisodeProgressInfo episodeProgressInfo = Service.CurrentPlayer.EpisodeProgressInfo;
			if (episodeProgressInfo == null)
			{
				return false;
			}
			EpisodeTaskProgressInfo currentTask = episodeProgressInfo.currentTask;
			return currentTask != null && currentTask.count >= currentTask.target;
		}

		public bool IsCurrentTaskProgressMarkedComplete()
		{
			if (this.CurrentEpisodeData == null)
			{
				return false;
			}
			EpisodeProgressInfo episodeProgressInfo = Service.CurrentPlayer.EpisodeProgressInfo;
			if (episodeProgressInfo == null)
			{
				return false;
			}
			EpisodeTaskProgressInfo currentTask = episodeProgressInfo.currentTask;
			return currentTask != null && currentTask.count >= currentTask.target && currentTask.completed;
		}

		public bool IsTaskTimeGateComplete()
		{
			if (!this.IsCurrentTaskTimeGated())
			{
				return false;
			}
			if (!this.IsTaskTimeGateActive())
			{
				return false;
			}
			EpisodeProgressInfo episodeProgressInfo = Service.CurrentPlayer.EpisodeProgressInfo;
			EpisodeTaskProgressInfo currentTask = episodeProgressInfo.currentTask;
			return currentTask.endTimestamp <= ServerTime.Time;
		}

		public bool IsTaskTimeGateActive()
		{
			uint taskTimeGateEndTime = this.GetTaskTimeGateEndTime();
			return taskTimeGateEndTime > 0u;
		}

		public bool IsNewEpisode()
		{
			EpisodeProgressInfo episodeProgressInfo = Service.CurrentPlayer.EpisodeProgressInfo;
			return !episodeProgressInfo.introStoryViewed;
		}

		public uint GetTaskTimeGateEndTime()
		{
			if (this.CurrentEpisodeData == null)
			{
				return 0u;
			}
			EpisodeProgressInfo episodeProgressInfo = Service.CurrentPlayer.EpisodeProgressInfo;
			if (episodeProgressInfo == null)
			{
				return 0u;
			}
			EpisodeTaskProgressInfo currentTask = episodeProgressInfo.currentTask;
			if (currentTask == null)
			{
				return 0u;
			}
			return currentTask.endTimestamp;
		}

		public bool IsCurrentTaskTimeGated()
		{
			int currentTaskTimeGate = this.GetCurrentTaskTimeGate();
			return currentTaskTimeGate > 0;
		}

		public int GetCurrentTaskTimeGate()
		{
			EpisodeProgressInfo episodeProgressInfo = Service.CurrentPlayer.EpisodeProgressInfo;
			if (episodeProgressInfo == null)
			{
				return 0;
			}
			EpisodeTaskProgressInfo currentTask = episodeProgressInfo.currentTask;
			if (currentTask == null)
			{
				return 0;
			}
			EpisodeTaskVO optional = Service.StaticDataController.GetOptional<EpisodeTaskVO>(currentTask.uid);
			if (optional == null)
			{
				return 0;
			}
			return optional.TimeGate;
		}

		public void ForceRefreshState()
		{
			if (!this.IsValidGameStateForRefresh())
			{
				Service.Logger.Warn("Woah woah woah dude. We're not in the right GameStateto refresh the EpisodeStates");
				return;
			}
			this.RefreshEpisodeStates();
		}

		public bool PlayIntroStoryAction()
		{
			EpisodeProgressInfo episodeProgressInfo = Service.CurrentPlayer.EpisodeProgressInfo;
			if (episodeProgressInfo.uid == null || episodeProgressInfo.currentTaskIndex != 0 || episodeProgressInfo.introStoryViewed)
			{
				return false;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			EpisodeDataVO episodeDataVO = staticDataController.Get<EpisodeDataVO>(episodeProgressInfo.uid);
			bool flag = this.PlayStoryActionForTaskUid(episodeDataVO.Tasks[0]);
			if (flag)
			{
				Service.EpisodeWidgetViewController.ResetWidget();
				this.MarkEpisodePanelOpened();
			}
			return flag;
		}

		public bool PlayStoryActionForTaskUid(string uid)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			EpisodeTaskVO optional = staticDataController.GetOptional<EpisodeTaskVO>(uid);
			if (optional == null)
			{
				Service.Logger.ErrorFormat("EpisodeController: Cannot play story action! Missing EpisodeTaskVO data for uid:{0}", new object[]
				{
					uid
				});
				return false;
			}
			return Service.EpisodeController.PlayTaskStoryAction(optional.StoryID);
		}

		public bool PlayMostRecentStoryAction()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			EpisodeProgressInfo episodeProgressInfo = Service.CurrentPlayer.EpisodeProgressInfo;
			EpisodeDataVO episodeDataVO = staticDataController.Get<EpisodeDataVO>(episodeProgressInfo.uid);
			string text = string.Empty;
			if (episodeProgressInfo.grindInfo.Started > 0)
			{
				text = staticDataController.Get<EpisodeTaskVO>(episodeDataVO.GrindTask).StoryID;
			}
			if (string.IsNullOrEmpty(text))
			{
				int num = Math.Min(episodeProgressInfo.currentTaskIndex, episodeDataVO.Tasks.Length - 1);
				for (int i = num; i >= 0; i--)
				{
					EpisodeTaskVO optional = staticDataController.GetOptional<EpisodeTaskVO>(episodeDataVO.Tasks[i]);
					if (optional != null && !string.IsNullOrEmpty(optional.StoryID))
					{
						text = optional.StoryID;
						break;
					}
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				Service.Logger.Error("EpisodeInfoScreen: Could not determine most recent story ID");
			}
			return this.PlayTaskStoryAction(text);
		}

		public bool PlayTaskStoryAction(string storyId)
		{
			if (string.IsNullOrEmpty(storyId))
			{
				return false;
			}
			Service.ScreenController.CloseAll();
			new ActionChain(storyId);
			Service.EventManager.RegisterObserver(this, EventId.StoryChainCompleted);
			return true;
		}

		public void AttemptToShowActiveEpisodeInfoScreen()
		{
			if (!this.IsEpisodeActive())
			{
				return;
			}
			EpisodeInfoScreen screen = new EpisodeInfoScreen();
			Service.ScreenController.AddScreen(screen);
			this.MarkEpisodePanelOpened();
		}

		public void MarkEpisodePanelOpened()
		{
			EpisodeProgressInfo episodeProgressInfo = Service.CurrentPlayer.EpisodeProgressInfo;
			if (episodeProgressInfo.introStoryViewed)
			{
				return;
			}
			episodeProgressInfo.introStoryViewed = true;
			EpisodeTaskIntroViewedCommand command = new EpisodeTaskIntroViewedCommand(new PlayerIdRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			Service.ServerAPI.Sync(command);
			Service.EpisodeWidgetViewController.RefreshCTAState();
		}

		public void SkipTaskProgress()
		{
			if (!this.IsEpisodeActive())
			{
				ProcessingScreen.Hide();
				return;
			}
			PlayerIdChecksumRequest request = new PlayerIdChecksumRequest();
			EpisodeTaskProgressSkipCommand episodeTaskProgressSkipCommand = new EpisodeTaskProgressSkipCommand(request);
			episodeTaskProgressSkipCommand.AddSuccessCallback(new AbstractCommand<PlayerIdChecksumRequest, EpisodeTaskProgressSkipResponse>.OnSuccessCallback(this.OnSkipTaskProgressSuccess));
			episodeTaskProgressSkipCommand.AddFailureCallback(new AbstractCommand<PlayerIdChecksumRequest, EpisodeTaskProgressSkipResponse>.OnFailureCallback(this.OnSkipTaskProgressFailure));
			Service.ServerAPI.Sync(episodeTaskProgressSkipCommand);
		}

		private void OnSkipTaskProgressSuccess(EpisodeTaskProgressSkipResponse response, object cookie)
		{
			Service.EpisodeController.ForceRefreshState();
			Service.EventManager.SendEvent(EventId.EpisodeTaskProgressSkipped, null);
		}

		private void OnSkipTaskProgressFailure(uint status, object data)
		{
			Service.EpisodeController.ForceRefreshState();
			Service.EventManager.SendEvent(EventId.EpisodeTaskProgressSkipFailed, null);
		}

		public void CompleteTaskProgress()
		{
			bool flag = false;
			if (!this.IsEpisodeActive())
			{
				flag = true;
			}
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			EpisodeProgressInfo episodeProgressInfo = currentPlayer.EpisodeProgressInfo;
			EpisodeTaskProgressInfo currentTask = episodeProgressInfo.currentTask;
			if (currentTask.count < currentTask.target)
			{
				Service.Logger.ErrorFormat("Trying to complete a task. actiondUID:{0} target progress:{1} != count:{2} ", new object[]
				{
					currentTask.actionUID,
					currentTask.target,
					currentTask.count
				});
				flag = true;
			}
			if (flag)
			{
				Service.EventManager.SendEvent(EventId.EpisodeTaskProgressCompleteFailed, null);
				return;
			}
			PlayerIdChecksumRequest request = new PlayerIdChecksumRequest();
			EpisodeTaskProgressCompleteCommand episodeTaskProgressCompleteCommand = new EpisodeTaskProgressCompleteCommand(request);
			episodeTaskProgressCompleteCommand.AddSuccessCallback(new AbstractCommand<PlayerIdChecksumRequest, EpisodeTaskProgressCompleteResponse>.OnSuccessCallback(this.OnCompleteTaskProgressSuccess));
			episodeTaskProgressCompleteCommand.AddFailureCallback(new AbstractCommand<PlayerIdChecksumRequest, EpisodeTaskProgressCompleteResponse>.OnFailureCallback(this.OnCompleteTaskProgressFailure));
			Service.ServerAPI.Sync(episodeTaskProgressCompleteCommand);
		}

		private void OnCompleteTaskProgressSuccess(EpisodeTaskProgressCompleteResponse response, object cookie)
		{
			Service.EpisodeController.ForceRefreshState();
			Service.EventManager.SendEvent(EventId.EpisodeTaskProgressCompleted, null);
		}

		private void OnCompleteTaskProgressFailure(uint status, object data)
		{
			Service.EpisodeController.ForceRefreshState();
			Service.EventManager.SendEvent(EventId.EpisodeTaskProgressCompleteFailed, null);
		}

		public void StartTaskTimeGate()
		{
			if (!this.IsCurrentTaskTimeGated())
			{
				ProcessingScreen.Hide();
				return;
			}
			EpisodeTaskTimeGateStartCommand episodeTaskTimeGateStartCommand = new EpisodeTaskTimeGateStartCommand(new PlayerIdRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			episodeTaskTimeGateStartCommand.AddSuccessCallback(new AbstractCommand<PlayerIdRequest, EpisodeDefaultResponse>.OnSuccessCallback(this.OnStartTaskTimeGateSuccess));
			Service.ServerAPI.Sync(episodeTaskTimeGateStartCommand);
		}

		private void OnStartTaskTimeGateSuccess(EpisodeDefaultResponse response, object cookie)
		{
			Service.EventManager.SendEvent(EventId.EpisodeTaskTimeGateStarted, null);
		}

		public void SkipTaskTimeGate()
		{
			if (!this.IsCurrentTaskTimeGated() || !this.IsTaskTimeGateActive())
			{
				ProcessingScreen.Hide();
				return;
			}
			PlayerIdChecksumRequest request = new PlayerIdChecksumRequest();
			EpisodeTaskTimeGateSkipCommand episodeTaskTimeGateSkipCommand = new EpisodeTaskTimeGateSkipCommand(request);
			episodeTaskTimeGateSkipCommand.AddSuccessCallback(new AbstractCommand<PlayerIdChecksumRequest, EpisodeTaskTimeGateSkipResponse>.OnSuccessCallback(this.OnSkipTimeGateSuccess));
			Service.ServerAPI.Sync(episodeTaskTimeGateSkipCommand);
		}

		private void OnSkipTimeGateSuccess(EpisodeTaskTimeGateSkipResponse response, object cookie)
		{
			this.ForceRefreshState();
			Service.EventManager.SendEvent(EventId.EpisodeTaskTimeGateSkipped, null);
		}

		public void ClaimCurrentEpisodeTask()
		{
			GameUtils.ClaimCurrentEpisodeTask();
		}

		private IEpisodeTimeVO RefreshEpisodeItem<T>(IEpisodeTimeVO currentCachedItem) where T : IEpisodeTimeVO, IValueObject
		{
			DateTime serverDateTime = Service.ServerAPI.ServerDateTime;
			if (this.IsEpisodeItemActive(serverDateTime, currentCachedItem))
			{
				this.TrySetNextScheduledRefreshDate(serverDateTime, currentCachedItem.EndTime);
				return currentCachedItem;
			}
			DateTime dateTime = DateTime.MaxValue;
			currentCachedItem = null;
			List<T> sortedEpisodeTimeVOList = this.GetSortedEpisodeTimeVOList<T>();
			int i = 0;
			int count = sortedEpisodeTimeVOList.Count;
			while (i < count)
			{
				IEpisodeTimeVO episodeTimeVO = sortedEpisodeTimeVOList[i];
				if (this.IsEpisodeItemActive(serverDateTime, episodeTimeVO))
				{
					currentCachedItem = episodeTimeVO;
					this.TrySetNextScheduledRefreshDate(serverDateTime, currentCachedItem.EndTime);
					return currentCachedItem;
				}
				if (episodeTimeVO.StartTime > serverDateTime && episodeTimeVO.StartTime < dateTime)
				{
					dateTime = episodeTimeVO.StartTime;
				}
				i++;
			}
			Service.Logger.Debug("Next Earliest Start Time: " + dateTime.ToString());
			this.TrySetNextScheduledRefreshDate(serverDateTime, dateTime);
			return currentCachedItem;
		}

		private List<T> GetSortedEpisodeTimeVOList<T>() where T : IEpisodeTimeVO, IValueObject
		{
			List<T> list = new List<T>();
			Dictionary<string, T>.ValueCollection all = Service.StaticDataController.GetAll<T>();
			foreach (T current in all)
			{
				list.Add(current);
			}
			list.Sort(new Comparison<T>(this.SortIEpisodeTimeVOs<T>));
			return list;
		}

		private int SortIEpisodeTimeVOs<T>(T a, T b) where T : IEpisodeTimeVO, IValueObject
		{
			if (a.Priority < b.Priority)
			{
				return -1;
			}
			if (a.Priority > b.Priority)
			{
				return 1;
			}
			if (a.StartTime < b.StartTime)
			{
				return -1;
			}
			if (a.StartTime > b.StartTime)
			{
				return 1;
			}
			return 0;
		}

		private void TrySetNextScheduledRefreshDate(DateTime now, DateTime newTime)
		{
			if (newTime < now)
			{
				return;
			}
			if (this.nextScheduledRefreshDate < now || this.nextScheduledRefreshDate > newTime)
			{
				this.nextScheduledRefreshDate = newTime;
			}
		}

		private bool IsEpisodeItemActive(DateTime now, IEpisodeTimeVO vo)
		{
			return vo != null && vo.StartTime <= now && vo.EndTime > now;
		}

		private bool IsValidGameStateForRefresh()
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			return currentState is HomeState;
		}

		private void RefreshEpisodeStates()
		{
			this.RefreshEpisodeData();
			this.CurrentWidgetData = (EpisodeWidgetDataVO)this.RefreshEpisodeItem<EpisodeWidgetDataVO>(this.CurrentWidgetData);
			if (this.CurrentWidgetData != null)
			{
				Service.Logger.Debug(string.Concat(new string[]
				{
					"Current Widget: ",
					this.CurrentWidgetData.Uid,
					" is active: ",
					this.CurrentWidgetData.StartTime.ToString(),
					" ---> ",
					this.CurrentWidgetData.EndTime.ToString()
				}));
			}
			this.ScheduleNextRefresh();
			Service.EventManager.SendEvent(EventId.EpisodeDataRefreshed, null);
		}

		private void RefreshEpisodeData()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			EpisodeProgressInfo episodeProgressInfo = Service.CurrentPlayer.EpisodeProgressInfo;
			if (episodeProgressInfo == null)
			{
				this.CurrentEpisodeData = null;
				return;
			}
			string uid = episodeProgressInfo.uid;
			if (string.IsNullOrEmpty(uid))
			{
				Service.EventManager.RegisterObserver(this, EventId.FueCompleted);
				this.CurrentEpisodeData = null;
			}
			else if (this.CurrentEpisodeData == null || (this.CurrentEpisodeData != null && this.CurrentEpisodeData.Uid != uid))
			{
				EpisodeDataVO optional = staticDataController.GetOptional<EpisodeDataVO>(uid);
				if (optional != null)
				{
					this.CurrentEpisodeData = staticDataController.GetOptional<EpisodeDataVO>(uid);
				}
			}
			if (this.CurrentEpisodeData != null)
			{
				Service.Logger.Debug(string.Concat(new string[]
				{
					"Current Episode: ",
					this.CurrentEpisodeData.Uid,
					" is active: ",
					this.CurrentEpisodeData.StartTime.ToString(),
					" ---> ",
					this.CurrentEpisodeData.EndTime.ToString()
				}));
			}
		}

		private void ScheduleNextRefresh()
		{
			Service.Logger.Debug("Next Scheduled Episode Refresh at: " + this.nextScheduledRefreshDate.ToString());
			ViewTimerManager viewTimerManager = Service.ViewTimerManager;
			if (this.scheduleTimerId != 0u)
			{
				viewTimerManager.KillViewTimer(this.scheduleTimerId);
			}
			TimeSpan timeSpan = this.nextScheduledRefreshDate.Subtract(Service.ServerAPI.ServerDateTime);
			if (timeSpan.TotalMilliseconds > 0.0 && timeSpan.TotalMilliseconds < 432000000.0)
			{
				this.scheduleTimerId = viewTimerManager.CreateViewTimer((float)timeSpan.TotalSeconds, false, new TimerDelegate(this.AttemptRefresh), null);
			}
		}

		private void AttemptRefresh(uint timerId, object cookie)
		{
			Service.ViewTimerManager.KillViewTimer(this.scheduleTimerId);
			this.scheduleTimerId = 0u;
			if (!this.IsValidGameStateForRefresh())
			{
				Service.EventManager.RegisterObserver(this, EventId.GameStateChanged);
				return;
			}
			this.RefreshEpisodeStates();
		}

		public EpisodeTaskActionVO getValidActionForTask(EpisodeTaskVO taskVo)
		{
			EpisodeTaskActionVO result = null;
			string[] actions = taskVo.Actions;
			StaticDataController staticDataController = Service.StaticDataController;
			for (int i = 0; i < actions.Length; i++)
			{
				EpisodeTaskActionVO optional = staticDataController.GetOptional<EpisodeTaskActionVO>(actions[i]);
				if (this.doesPlayerQualifyForTaskAction(optional))
				{
					result = optional;
					break;
				}
			}
			return result;
		}

		public bool doesPlayerQualifyForTaskAction(EpisodeTaskActionVO vo)
		{
			if (vo == null)
			{
				return false;
			}
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (currentPlayer.Faction != vo.Faction)
			{
				return false;
			}
			int num = currentPlayer.Map.FindHighestHqLevel();
			return (vo.MinHQ == -1 || vo.MinHQ <= num) && (vo.MaxHQ == -1 || vo.MaxHQ >= num);
		}

		public int getTaskActionTargetAmount(EpisodeTaskActionVO vo, int hqLevel)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			EpisodeTaskScaleVO optional = staticDataController.GetOptional<EpisodeTaskScaleVO>(vo.ScaleId);
			switch (hqLevel)
			{
			case 1:
				return optional.HQ1;
			case 2:
				return optional.HQ2;
			case 3:
				return optional.HQ3;
			case 4:
				return optional.HQ4;
			case 5:
				return optional.HQ5;
			case 6:
				return optional.HQ6;
			case 7:
				return optional.HQ7;
			case 8:
				return optional.HQ8;
			case 9:
				return optional.HQ9;
			case 10:
				return optional.HQ10;
			default:
				Service.Logger.WarnFormat("EpisodeController: Unhandled HQ level of {0}", new object[]
				{
					hqLevel
				});
				return 0;
			}
		}
	}
}
