using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Goals;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Episodes;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class EpisodeTaskManager : AbstractGoalManager, IEventObserver
	{
		private Dictionary<BaseGoalProcessor, EpisodeTaskProgressInfo> processorMap;

		public EpisodeTaskManager()
		{
			Service.EpisodeTaskManager = this;
		}

		protected override void Login()
		{
			this.processorMap = new Dictionary<BaseGoalProcessor, EpisodeTaskProgressInfo>();
			Service.EventManager.RegisterObserver(this, EventId.EpisodeProgressInfoRefreshed);
			base.Login();
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			if (id == EventId.EpisodeProgressInfoRefreshed)
			{
				if (!(currentState is ApplicationLoadState))
				{
					this.ClearProcessorMap(false);
					this.FillProcessorMap();
				}
			}
			return EatResponse.NotEaten;
		}

		public override void Progress(BaseGoalProcessor processor, int amount, object cookie)
		{
			if (cookie == null)
			{
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				EpisodeProgressInfo episodeProgressInfo = currentPlayer.EpisodeProgressInfo;
				EpisodeTaskProgressInfo currentTask = episodeProgressInfo.currentTask;
				EpisodeProgressData episodeProgressData = new EpisodeProgressData(episodeProgressInfo.uid, currentTask.uid, amount, currentTask.count, EventId.EpisodeComplexTask, 1);
				cookie = episodeProgressData;
			}
			Service.EventManager.SendEvent(EventId.EpisodeProgressMade, cookie);
			if (Service.PlanetRelocationController.IsRelocationInProgress())
			{
				return;
			}
			if (!this.processorMap.ContainsKey(processor))
			{
				return;
			}
			EpisodeTaskProgressInfo currentEpisodeTaskProgress = this.GetCurrentEpisodeTaskProgress();
			if (currentEpisodeTaskProgress != null && processor.GetGoalUid() == currentEpisodeTaskProgress.uid)
			{
				currentEpisodeTaskProgress.count = Math.Min(currentEpisodeTaskProgress.count + amount, currentEpisodeTaskProgress.target);
				if (currentEpisodeTaskProgress.count == currentEpisodeTaskProgress.target && currentEpisodeTaskProgress.type != "EpisodePoint")
				{
					currentEpisodeTaskProgress.completed = true;
				}
			}
			if (currentEpisodeTaskProgress == null || currentEpisodeTaskProgress.count >= currentEpisodeTaskProgress.target || processor.GetGoalUid() != currentEpisodeTaskProgress.uid)
			{
				this.processorMap.Remove(processor);
				processor.Destroy();
			}
		}

		protected override void FillProcessorMap()
		{
			if (this.processorMap.Count > 0)
			{
				Service.Logger.Error("Attempting to fill an already-full processorMap!");
			}
			EpisodeTaskProgressInfo currentEpisodeTaskProgress = this.GetCurrentEpisodeTaskProgress();
			if (currentEpisodeTaskProgress != null)
			{
				EpisodeTaskVO vo = Service.StaticDataController.Get<EpisodeTaskVO>(currentEpisodeTaskProgress.uid);
				BaseGoalProcessor processor = GoalFactory.GetProcessor(vo, this);
				this.processorMap.Add(processor, currentEpisodeTaskProgress);
			}
		}

		public EpisodeTaskProgressInfo GetCurrentEpisodeTaskProgress()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			EpisodeProgressInfo episodeProgressInfo = currentPlayer.EpisodeProgressInfo;
			if (episodeProgressInfo == null)
			{
				return null;
			}
			if (string.IsNullOrEmpty(episodeProgressInfo.uid))
			{
				return null;
			}
			EpisodeTaskProgressInfo currentTask = episodeProgressInfo.currentTask;
			if (currentTask == null)
			{
				return null;
			}
			return currentTask;
		}

		protected override void ClearProcessorMap(bool sendExpirationEvent)
		{
			foreach (KeyValuePair<BaseGoalProcessor, EpisodeTaskProgressInfo> current in this.processorMap)
			{
				current.Key.Destroy();
			}
			this.processorMap.Clear();
		}

		public override string GetGoalItem(IValueObject goalVO)
		{
			EpisodeTaskProgressInfo currentEpisodeTaskProgress = this.GetCurrentEpisodeTaskProgress();
			if (currentEpisodeTaskProgress != null)
			{
				EpisodeTaskActionVO episodeTaskActionVO = Service.StaticDataController.Get<EpisodeTaskActionVO>(currentEpisodeTaskProgress.actionUID);
				return episodeTaskActionVO.Item;
			}
			return null;
		}

		public override GoalType GetGoalType(IValueObject goalVO)
		{
			EpisodeTaskProgressInfo currentEpisodeTaskProgress = this.GetCurrentEpisodeTaskProgress();
			if (currentEpisodeTaskProgress != null)
			{
				EpisodeTaskActionVO episodeTaskActionVO = Service.StaticDataController.Get<EpisodeTaskActionVO>(currentEpisodeTaskProgress.actionUID);
				return StringUtils.ParseEnum<GoalType>(episodeTaskActionVO.Type);
			}
			return GoalType.Invalid;
		}

		public override bool GetGoalAllowPvE(IValueObject goalVO)
		{
			return false;
		}
	}
}
