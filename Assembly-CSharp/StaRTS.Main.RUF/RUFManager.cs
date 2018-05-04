using StaRTS.Externals.GameServices;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.RUF.RUFTasks;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.RUF
{
	public class RUFManager : IEventObserver
	{
		private const string AUTO_TRIGGER_TYPE = "Auto";

		private const string AUTO_LOAD_TRIGGER_TYPE = "AutoLoad";

		private List<AbstractRUFTask> queue;

		private List<AbstractRUFTask> loadStateQueue;

		private bool processingLoadState;

		public List<int> OmitRateAppLevels
		{
			get;
			set;
		}

		public RUFManager()
		{
			this.queue = new List<AbstractRUFTask>();
			this.loadStateQueue = new List<AbstractRUFTask>();
			this.OmitRateAppLevels = new List<int>();
			Service.RUFManager = this;
		}

		public void PrepareReturningUserFlow()
		{
			this.processingLoadState = true;
			Service.EventManager.RegisterObserver(this, EventId.WorldInTransitionComplete, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.PurgeHomeStateRUFTask, EventPriority.Default);
			this.AddOfflineContractTasksToQueue();
			this.AddTaskToAppropriateQueue(new PromoRUFTask());
			this.AddTaskToAppropriateQueue(new FueRUFTask());
			this.AddTaskToAppropriateQueue(new FueResumeRUFTask());
			this.AddTaskToAppropriateQueue(new CallsignRUFTask());
			this.AddTaskToAppropriateQueue(new HolonetRUFTask());
			this.AddTaskToAppropriateQueue(new GoToHomeStateRUFTask());
			this.AddTaskToAppropriateQueue(new CompensationRUFTask());
			this.AddAutoTriggerTasksToQueue();
			this.SortTaskQueues();
			this.ProcessQueue(true);
		}

		private void AddTaskToAppropriateQueue(AbstractRUFTask task)
		{
			if (task.ShouldPlayFromLoadState)
			{
				this.loadStateQueue.Add(task);
			}
			else
			{
				this.queue.Add(task);
			}
		}

		private void SortTaskQueues()
		{
			this.loadStateQueue.Sort(new Comparison<AbstractRUFTask>(RUFManager.CompareRUFTasks));
			this.queue.Sort(new Comparison<AbstractRUFTask>(RUFManager.CompareRUFTasks));
		}

		private void AddOfflineContractTasksToQueue()
		{
			List<ContractTO> contractEventsThatHappenedOffline = Service.ISupportController.GetContractEventsThatHappenedOffline();
			if (contractEventsThatHappenedOffline == null || contractEventsThatHappenedOffline.Count < 1)
			{
				return;
			}
			int count = contractEventsThatHappenedOffline.Count;
			StaticDataController staticDataController = Service.StaticDataController;
			for (int i = 0; i < count; i++)
			{
				ContractTO contractTO = contractEventsThatHappenedOffline[i];
				if (contractTO.ContractType == ContractType.Upgrade)
				{
					BuildingTypeVO buildingTypeVO = staticDataController.Get<BuildingTypeVO>(contractTO.Uid);
					if (buildingTypeVO.Type == BuildingType.HQ)
					{
						this.AddTaskToAppropriateQueue(new HQCelebRUFTask());
					}
				}
			}
		}

		public void UpdateProcessingLoadState(bool processLoadState)
		{
			this.processingLoadState = processLoadState;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.WorldInTransitionComplete)
			{
				this.ProcessQueue(true);
				GameServicesManager.AttemptAutomaticSignInPrompt();
				Service.EventManager.UnregisterObserver(this, EventId.WorldInTransitionComplete);
			}
			else if (id == EventId.PurgeHomeStateRUFTask)
			{
				this.PurgeQueueByPriority(50);
			}
			return EatResponse.NotEaten;
		}

		public void AddAutoTriggerTasksToQueue()
		{
			Dictionary<string, StoryTriggerVO>.ValueCollection all = Service.StaticDataController.GetAll<StoryTriggerVO>();
			foreach (StoryTriggerVO current in all)
			{
				bool flag = current.TriggerType.Equals("Auto");
				bool flag2 = current.TriggerType.Equals("AutoLoad");
				if (flag || flag2)
				{
					AbstractRUFTask rUFTaskForAutoTrigger = AutoStoryTriggerUtils.GetRUFTaskForAutoTrigger(current);
					if (rUFTaskForAutoTrigger != null)
					{
						this.AddTaskToAppropriateQueue(rUFTaskForAutoTrigger);
					}
				}
			}
		}

		public void ProcessQueue(bool continueProcessing)
		{
			if (!continueProcessing)
			{
				return;
			}
			AbstractRUFTask task;
			if (this.processingLoadState)
			{
				if (this.loadStateQueue.Count < 1)
				{
					return;
				}
				task = this.loadStateQueue[0];
			}
			else
			{
				if (this.queue.Count < 1)
				{
					this.OnComplete();
					return;
				}
				task = this.queue[0];
			}
			this.ProcessTask(task, continueProcessing);
		}

		private void RemoveTaskFromQueue(AbstractRUFTask task)
		{
			if (task.ShouldPlayFromLoadState)
			{
				this.loadStateQueue.Remove(task);
			}
			else
			{
				this.queue.Remove(task);
			}
		}

		private void ProcessTask(AbstractRUFTask task, bool continueProcessing)
		{
			if (task != null)
			{
				this.RemoveTaskFromQueue(task);
				if (task.ShouldProcess && task.ShouldPurgeQueue)
				{
					if (task.PriorityPurgeThreshold == 0)
					{
						this.queue.Clear();
					}
					else
					{
						this.PurgeQueueByPriority(task.PriorityPurgeThreshold);
					}
				}
				task.Process(continueProcessing);
			}
		}

		private void PurgeQueueByPriority(int priority)
		{
			List<AbstractRUFTask> list = new List<AbstractRUFTask>();
			for (int i = this.queue.Count - 1; i >= 0; i--)
			{
				if (priority <= this.queue[i].Priority)
				{
					list.Add(this.queue[i]);
				}
			}
			int count = list.Count;
			for (int j = 0; j < count; j++)
			{
				this.queue.Remove(list[j]);
			}
			list.Clear();
			for (int k = this.loadStateQueue.Count - 1; k >= 0; k--)
			{
				if (priority <= this.loadStateQueue[k].Priority)
				{
					list.Add(this.loadStateQueue[k]);
				}
			}
			count = list.Count;
			for (int l = 0; l < count; l++)
			{
				this.loadStateQueue.Remove(list[l]);
			}
			list.Clear();
		}

		private void OnComplete()
		{
			Service.ISupportController.ReleaseContractEventsThatHappnedOffline();
		}

		private static int CompareRUFTasks(AbstractRUFTask a, AbstractRUFTask b)
		{
			if (a == b)
			{
				return 0;
			}
			if (a.Priority != b.Priority)
			{
				return a.Priority - b.Priority;
			}
			if (!a.ShouldPurgeQueue && !b.ShouldPurgeQueue)
			{
				return 0;
			}
			if (a.ShouldPurgeQueue && b.ShouldPurgeQueue)
			{
				return 0;
			}
			if (a.ShouldPurgeQueue)
			{
				return 1;
			}
			return -1;
		}
	}
}
