using StaRTS.Main.Models.Episodes;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Goals
{
	public class EpisodePointGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		public const string EPISODE_POINT_TYPE = "EpisodePoint";

		public EpisodePointGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.RaidComplete);
			eventManager.RegisterObserver(this, EventId.PvpBattleWon);
			eventManager.RegisterObserver(this, EventId.ObjectiveCompleted);
			eventManager.RegisterObserver(this, EventId.TournamentTierReached);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.PvpBattleWon || id == EventId.TournamentTierReached || id == EventId.RaidComplete || id == EventId.ObjectiveCompleted)
			{
				this.RewardEpisodePointsForEvent(id, (int)cookie);
			}
			return EatResponse.NotEaten;
		}

		private void RewardEpisodePointsForEvent(EventId id, int pointIndex)
		{
			if (pointIndex <= 0)
			{
				return;
			}
			EpisodeTaskManager episodeTaskManager = Service.EpisodeTaskManager;
			EpisodeTaskProgressInfo currentEpisodeTaskProgress = episodeTaskManager.GetCurrentEpisodeTaskProgress();
			if (currentEpisodeTaskProgress == null || currentEpisodeTaskProgress.type != "EpisodePoint")
			{
				return;
			}
			EpisodePointScaleVO currentEpisodePointScaleVO = Service.EpisodeController.GetCurrentEpisodePointScaleVO();
			if (currentEpisodePointScaleVO == null)
			{
				return;
			}
			int[] array = null;
			if (id != EventId.PvpBattleWon)
			{
				if (id != EventId.TournamentTierReached)
				{
					if (id != EventId.RaidComplete)
					{
						if (id != EventId.ObjectiveCompleted)
						{
							Service.Logger.ErrorFormat("Unknown event type {0} when awarding episode points", new object[]
							{
								id
							});
						}
						else
						{
							array = currentEpisodePointScaleVO.Objective;
						}
					}
					else
					{
						array = currentEpisodePointScaleVO.Raid;
					}
				}
				else
				{
					array = currentEpisodePointScaleVO.Conflict;
				}
			}
			else
			{
				array = currentEpisodePointScaleVO.PvP;
			}
			if (array == null)
			{
				return;
			}
			int num = Math.Min(pointIndex, array.Length) - 1;
			int num2 = array[num];
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			EpisodeProgressInfo episodeProgressInfo = currentPlayer.EpisodeProgressInfo;
			EpisodeProgressData cookie = new EpisodeProgressData(episodeProgressInfo.uid, currentEpisodeTaskProgress.uid, num2, episodeProgressInfo.currentTask.count, id, pointIndex);
			this.parent.Progress(this, num2, cookie);
		}

		public override void Destroy()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.RaidComplete);
			eventManager.UnregisterObserver(this, EventId.PvpBattleWon);
			eventManager.UnregisterObserver(this, EventId.ObjectiveCompleted);
			eventManager.UnregisterObserver(this, EventId.TournamentTierReached);
			base.Destroy();
		}
	}
}
