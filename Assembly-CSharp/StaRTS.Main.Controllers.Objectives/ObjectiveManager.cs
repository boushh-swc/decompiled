using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Controllers.Goals;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Crates;
using StaRTS.Main.Models.Commands.Objectives;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Objectives;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Objectives
{
	public class ObjectiveManager : AbstractGoalManager
	{
		private Dictionary<BaseGoalProcessor, ObjectiveProgress> processorMap;

		public ObjectiveManager()
		{
			Service.ObjectiveManager = this;
		}

		protected override void Login()
		{
			this.processorMap = new Dictionary<BaseGoalProcessor, ObjectiveProgress>();
			base.Login();
		}

		protected override void VerifyCurrentGoalsAgainstMeta()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			Dictionary<string, ObjectiveGroup> objectives = Service.CurrentPlayer.Objectives;
			foreach (KeyValuePair<string, ObjectiveGroup> current in objectives)
			{
				bool flag = false;
				int i = 0;
				int count = current.Value.ProgressObjects.Count;
				while (i < count)
				{
					string objectiveUid = current.Value.ProgressObjects[i].ObjectiveUid;
					if (staticDataController.GetOptional<ObjectiveVO>(objectiveUid) == null)
					{
						flag = true;
						Service.Logger.WarnFormat("Planet {0} has an invalid objective {1}", new object[]
						{
							current.Key,
							objectiveUid
						});
					}
					i++;
				}
				if (flag)
				{
					ForceObjectivesUpdateCommand command = new ForceObjectivesUpdateCommand(current.Key);
					Service.ServerAPI.Sync(command);
				}
			}
		}

		public override void Update()
		{
			if (this.refreshing)
			{
				return;
			}
			int serverTime = (int)Service.ServerAPI.ServerTime;
			Dictionary<string, ObjectiveGroup> objectives = Service.CurrentPlayer.Objectives;
			foreach (KeyValuePair<string, ObjectiveGroup> current in objectives)
			{
				if (serverTime > current.Value.EndTimestamp)
				{
					this.Expire(current.Key, current.Value);
					base.RefreshFromServer();
				}
				else if (serverTime > current.Value.GraceTimestamp)
				{
					this.Grace(current.Key, current.Value);
				}
			}
		}

		private void Grace(string planetUid, ObjectiveGroup group)
		{
			if (planetUid == Service.CurrentPlayer.PlanetId)
			{
				this.ClearProcessorMap(false);
			}
		}

		private void Expire(string planetUid, ObjectiveGroup group)
		{
			if (planetUid == Service.CurrentPlayer.PlanetId)
			{
				this.ClearProcessorMap(false);
			}
			group.ProgressObjects.Clear();
		}

		public override void Progress(BaseGoalProcessor processor, int amount, object cookie)
		{
			if (Service.PlanetRelocationController.IsRelocationInProgress())
			{
				return;
			}
			if (!this.processorMap.ContainsKey(processor))
			{
				return;
			}
			EventManager eventManager = Service.EventManager;
			ObjectiveProgress objectiveProgress = this.processorMap[processor];
			objectiveProgress.Count = Math.Min(objectiveProgress.Count + amount, objectiveProgress.Target);
			if (objectiveProgress.Count >= objectiveProgress.Target)
			{
				objectiveProgress.State = ObjectiveState.Complete;
				eventManager.SendEvent(EventId.UpdateObjectiveToastData, objectiveProgress);
				int objectiveProgressIndex = this.GetObjectiveProgressIndex(objectiveProgress.ObjectiveUid);
				eventManager.SendEvent(EventId.ObjectiveCompleted, objectiveProgressIndex);
				this.processorMap.Remove(processor);
				processor.Destroy();
			}
			eventManager.SendEvent(EventId.ObjectivesUpdated, objectiveProgress);
		}

		public void Claim(ObjectiveProgress objective)
		{
			objective.State = ObjectiveState.Rewarded;
			Service.EventManager.SendEvent(EventId.ObjectivesUpdated, null);
			ClaimObjectiveRequest request = new ClaimObjectiveRequest(Service.CurrentPlayer.PlayerId, objective.PlanetId, objective.ObjectiveUid);
			ClaimObjectiveCommand claimObjectiveCommand = new ClaimObjectiveCommand(request);
			claimObjectiveCommand.AddSuccessCallback(new AbstractCommand<PlayerIdRequest, CrateDataResponse>.OnSuccessCallback(base.ClaimCallback));
			claimObjectiveCommand.AddFailureCallback(new AbstractCommand<PlayerIdRequest, CrateDataResponse>.OnFailureCallback(this.ClaimFailed));
			claimObjectiveCommand.Context = objective;
			Service.ServerAPI.Sync(claimObjectiveCommand);
		}

		private void ClaimFailed(uint status, object cookie)
		{
			Service.Logger.DebugFormat("Failed to claim objectives from server ({0}).", new object[]
			{
				status
			});
			Service.EventManager.SendEvent(EventId.ClaimObjectiveFailed, null);
		}

		protected override void FillProcessorMap()
		{
			if (this.processorMap.Count > 0)
			{
				Service.Logger.Error("Attempting to fill an already-full processorMap!");
			}
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			string planetId = currentPlayer.PlanetId;
			if (currentPlayer.Objectives.ContainsKey(planetId))
			{
				ObjectiveGroup objectiveGroup = currentPlayer.Objectives[planetId];
				int i = 0;
				int count = objectiveGroup.ProgressObjects.Count;
				while (i < count)
				{
					ObjectiveProgress objectiveProgress = objectiveGroup.ProgressObjects[i];
					if (objectiveProgress.State == ObjectiveState.Active)
					{
						BaseGoalProcessor processor = GoalFactory.GetProcessor(objectiveProgress.VO, this);
						this.processorMap.Add(processor, objectiveProgress);
					}
					i++;
				}
			}
		}

		protected override void ClearProcessorMap(bool sendExpirationEvent)
		{
			foreach (KeyValuePair<BaseGoalProcessor, ObjectiveProgress> current in this.processorMap)
			{
				if (sendExpirationEvent)
				{
					Service.EventManager.SendEvent(EventId.UpdateObjectiveToastData, current.Value);
				}
				current.Key.Destroy();
			}
			this.processorMap.Clear();
		}

		protected override void AttemptRefreshFromServer()
		{
			this.refreshing = true;
			GetObjectivesCommand getObjectivesCommand = new GetObjectivesCommand(new PlayerIdRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			getObjectivesCommand.AddSuccessCallback(new AbstractCommand<PlayerIdRequest, GetObjectivesResponse>.OnSuccessCallback(this.OnObjectivesRefreshed));
			getObjectivesCommand.AddFailureCallback(new AbstractCommand<PlayerIdRequest, GetObjectivesResponse>.OnFailureCallback(this.OnObjectivesFailed));
			Service.ServerAPI.Enqueue(getObjectivesCommand);
		}

		private void OnObjectivesRefreshed(GetObjectivesResponse response, object cookie)
		{
			this.ClearProcessorMap(false);
			this.FillProcessorMap();
			Service.EventManager.SendEvent(EventId.ObjectivesUpdated, null);
			this.refreshing = false;
		}

		private void OnObjectivesFailed(uint status, object cookie)
		{
			Service.Logger.ErrorFormat("Failed to refresh objectives from server ({0}).", new object[]
			{
				status
			});
			this.refreshing = false;
			this.ClearProcessorMap(false);
			Service.CurrentPlayer.Objectives.Clear();
			this.retryCount++;
			if (this.retryCount < 3)
			{
				this.AttemptRefreshFromServer();
			}
		}

		public int GetCompletedObjectivesCount()
		{
			int num = 0;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			foreach (KeyValuePair<string, ObjectiveGroup> current in currentPlayer.Objectives)
			{
				ObjectiveGroup value = current.Value;
				int i = 0;
				int count = value.ProgressObjects.Count;
				while (i < count)
				{
					ObjectiveProgress objectiveProgress = value.ProgressObjects[i];
					if (objectiveProgress.State == ObjectiveState.Complete)
					{
						num++;
					}
					i++;
				}
			}
			return num;
		}

		public int GetObjectiveProgressIndex(string objectiveUid)
		{
			int num = 0;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			string planetId = currentPlayer.PlanetId;
			ObjectiveGroup objectiveGroup = null;
			if (objectiveUid != null && planetId != null && currentPlayer.Objectives != null && currentPlayer.Objectives.TryGetValue(planetId, out objectiveGroup))
			{
				int i = 0;
				int count = objectiveGroup.ProgressObjects.Count;
				while (i < count)
				{
					ObjectiveProgress objectiveProgress = objectiveGroup.ProgressObjects[i];
					if (objectiveProgress.ObjectiveUid != null && objectiveProgress.ObjectiveUid == objectiveUid)
					{
						num = i + 1;
						break;
					}
					i++;
				}
			}
			if (num == 0)
			{
				Service.Logger.ErrorFormat("Failed to get Objective progressIndex for uid:{0}", new object[]
				{
					objectiveUid
				});
			}
			return num;
		}

		public override string GetGoalItem(IValueObject goalVO)
		{
			return ((ObjectiveVO)goalVO).Item;
		}

		public override GoalType GetGoalType(IValueObject goalVO)
		{
			return ((ObjectiveVO)goalVO).ObjectiveType;
		}

		public override bool GetGoalAllowPvE(IValueObject goalVO)
		{
			return ((ObjectiveVO)goalVO).AllowPvE;
		}
	}
}
