using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Goals;
using StaRTS.Main.Models.Episodes;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class EpisodeWidgetViewController : IEventObserver
	{
		private EpisodeWidgetView widget;

		private Queue<EpisodeProgressData> progressQueue;

		public EpisodeWidgetViewController()
		{
			Service.EpisodeWidgetViewController = this;
			this.progressQueue = new Queue<EpisodeProgressData>();
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.EpisodeDataRefreshed);
			eventManager.RegisterObserver(this, EventId.GameStateChanged);
			eventManager.RegisterObserver(this, EventId.HudComplete);
			eventManager.RegisterObserver(this, EventId.HUDVisibilityChanged);
			eventManager.RegisterObserver(this, EventId.HUDVisibilityChanging);
			eventManager.RegisterObserver(this, EventId.EpisodeProgressInfoRefreshed);
			eventManager.RegisterObserver(this, EventId.EpisodeProgressMade);
			eventManager.RegisterObserver(this, EventId.WorldInTransitionComplete);
			eventManager.RegisterObserver(this, EventId.EpisodeProgressWidgetDone);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			EventManager eventManager = Service.EventManager;
			switch (id)
			{
			case EventId.EpisodeDataRefreshed:
			case EventId.EpisodeProgressInfoRefreshed:
				this.RefreshWidgetViewState();
				break;
			case EventId.EpisodeProgressMade:
				this.ShowProgress((EpisodeProgressData)cookie, false);
				break;
			default:
				if (id != EventId.HUDVisibilityChanging)
				{
					if (id != EventId.HUDVisibilityChanged)
					{
						if (id == EventId.HudComplete)
						{
							this.InitializeEpisodeWidgetElements();
							eventManager.UnregisterObserver(this, EventId.HudComplete);
							break;
						}
						if (id == EventId.WorldInTransitionComplete)
						{
							this.ShowProgress(null, false);
							break;
						}
						if (id != EventId.GameStateChanged)
						{
							if (id != EventId.EpisodeProgressWidgetDone)
							{
								break;
							}
							this.ShowProgress(null, true);
							break;
						}
					}
					if (this.widget != null)
					{
						if (Service.UXController.HUD.Visible)
						{
							if (Service.BuildingController != null && Service.BuildingController.IsPurchasing)
							{
								this.widget.Hide(false);
							}
							else
							{
								IState currentState = Service.GameStateMachine.CurrentState;
								if (currentState is HomeState && Service.EpisodeController.IsEpisodeWidgetActive())
								{
									this.widget.Show(this.progressQueue.Count > 0);
									this.DelayedShowProgress(null, false);
								}
								else
								{
									this.widget.Hide();
								}
							}
						}
					}
				}
				else if (this.widget != null)
				{
					if (!(bool)cookie)
					{
						this.widget.Reset();
					}
				}
				break;
			}
			return EatResponse.NotEaten;
		}

		public void ResetWidget()
		{
			this.widget.Reset();
		}

		private void DelayedShowProgress(EpisodeProgressData progressData, bool showReward)
		{
			Service.ViewTimerManager.CreateViewTimer(1f, false, new TimerDelegate(this.ShowProgressCallback), new KeyValuePair<EpisodeProgressData, bool>(progressData, showReward));
		}

		private void ShowProgressCallback(uint id, object cookie)
		{
			KeyValuePair<EpisodeProgressData, bool> keyValuePair = (KeyValuePair<EpisodeProgressData, bool>)cookie;
			this.ShowProgress(keyValuePair.Key, keyValuePair.Value);
		}

		private void ShowProgress(EpisodeProgressData progressData, bool showReward)
		{
			EpisodeProgressInfo episodeProgressInfo = Service.CurrentPlayer.EpisodeProgressInfo;
			EpisodeTaskProgressInfo currentTask = episodeProgressInfo.currentTask;
			if (progressData != null && progressData.progress > 0 && progressData.episodeUid == episodeProgressInfo.uid && progressData.taskUid == currentTask.uid)
			{
				if (progressData.progressType != EventId.EpisodeComplexTask)
				{
					this.progressQueue.Enqueue(progressData);
				}
				else
				{
					EpisodeProgressData episodeProgressData = null;
					foreach (EpisodeProgressData current in this.progressQueue)
					{
						if (current.taskUid == progressData.taskUid)
						{
							episodeProgressData = current;
							break;
						}
					}
					if (episodeProgressData != null)
					{
						episodeProgressData.progress += progressData.progress;
					}
					else
					{
						this.progressQueue.Enqueue(progressData);
					}
				}
			}
			if (this.widget != null)
			{
				if (this.progressQueue.Count > 0)
				{
					bool flag = this.widget.ShowProgress(this.progressQueue.Peek());
					if (flag)
					{
						this.progressQueue.Dequeue();
					}
				}
				else if (showReward)
				{
					Service.EpisodeController.ForceRefreshState();
					this.widget.HideProgress();
					this.widget.ShowCTA();
				}
				else
				{
					this.widget.HideProgress();
				}
			}
		}

		public void RefreshWidgetViewState()
		{
			EpisodeController episodeController = Service.EpisodeController;
			if (!episodeController.IsEpisodeWidgetActive())
			{
				this.widget.Hide();
				return;
			}
			this.widget.SetWidgetData(episodeController.CurrentWidgetData, episodeController.CurrentWidgetData.EndTime, episodeController.IsCurrentTaskProgressComplete(), episodeController.IsCurrentTaskTimeGated(), episodeController.IsTaskTimeGateActive(), episodeController.IsTaskTimeGateComplete(), episodeController.GetCurrentTaskTimeGate(), episodeController.GetTaskTimeGateEndTime(), episodeController.IsNewEpisode());
			IState currentState = Service.GameStateMachine.CurrentState;
			if (!(currentState is HomeState))
			{
				this.widget.Hide();
				return;
			}
			this.widget.Show(false);
		}

		private void InitializeEpisodeWidgetElements()
		{
			UXFactory hUD = Service.UXController.HUD;
			this.widget = new EpisodeWidgetView(hUD);
			this.widget.Hide();
			this.RefreshWidgetViewState();
		}

		public void RefreshCTAState()
		{
			this.widget.RefreshCTAState();
		}
	}
}
