using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle.Replay;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class EntityIdleController : IEventObserver
	{
		private const float MIN_ANIMATION_DELAY = 10f;

		private const float MAX_ANIMATION_DELAY = 25f;

		private List<uint> idleTimers;

		private List<TrackingComponent> trackComps;

		public EntityIdleController()
		{
			Service.EntityIdleController = this;
			this.idleTimers = new List<uint>();
			this.trackComps = new List<TrackingComponent>();
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.GameStateChanged, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.WorldLoadComplete, EventPriority.Default);
		}

		private void Start()
		{
			this.Stop(false);
			NodeList<TrackingNode> nodeList = Service.EntityController.GetNodeList<TrackingNode>();
			for (TrackingNode trackingNode = nodeList.Head; trackingNode != null; trackingNode = trackingNode.Next)
			{
				BuildingComponent buildingComp = trackingNode.BuildingComp;
				if (buildingComp != null && buildingComp.BuildingType.Type == BuildingType.Turret)
				{
					trackingNode.TrackingComp.MaxVelocity = 0.75f;
					this.trackComps.Add(trackingNode.TrackingComp);
					this.CreateRandomDelay(trackingNode.TrackingComp, 0u);
				}
			}
		}

		private void CreateRandomDelay(TrackingComponent trackingComp, uint timerId)
		{
			float randomDelayTime = this.GetRandomDelayTime(timerId == 0u);
			uint num = Service.ViewTimerManager.CreateViewTimer(randomDelayTime, false, new TimerDelegate(this.OnTurretTimer), trackingComp);
			if (timerId == 0u)
			{
				this.idleTimers.Add(num);
			}
			else
			{
				for (int i = 0; i < this.idleTimers.Count; i++)
				{
					if (this.idleTimers[i] == timerId)
					{
						this.idleTimers[i] = num;
						break;
					}
				}
			}
		}

		private void OnTurretTimer(uint id, object cookie)
		{
			TrackingComponent trackingComp = cookie as TrackingComponent;
			this.UpdateWithRandomAngle(trackingComp);
			this.CreateRandomDelay(trackingComp, id);
		}

		private void UpdateWithRandomAngle(TrackingComponent trackingComp)
		{
			Rand rand = Service.Rand;
			float num = rand.ViewRangeFloat(0f, 6.28318548f);
			trackingComp.Mode = TrackingMode.Angle;
			trackingComp.TargetYaw = num;
			if (trackingComp.TrackPitch)
			{
				num = rand.ViewRangeFloat(0f, 0.2617994f);
				trackingComp.TargetPitch = num;
			}
		}

		private float GetRandomDelayTime(bool firstTime)
		{
			Rand rand = Service.Rand;
			return (!firstTime) ? rand.ViewRangeFloat(10f, 25f) : rand.ViewRangeFloat(0f, 15f);
		}

		private void Stop(bool reset)
		{
			for (int i = 0; i < this.idleTimers.Count; i++)
			{
				Service.ViewTimerManager.KillViewTimer(this.idleTimers[i]);
			}
			this.idleTimers.Clear();
			for (int j = 0; j < this.trackComps.Count; j++)
			{
				this.trackComps[j].MaxVelocity = 0.25f;
				if (reset)
				{
					this.trackComps[j].Mode = TrackingMode.Angle;
					this.trackComps[j].TargetYaw = 3.14159274f;
					this.trackComps[j].TargetPitch = 0f;
				}
			}
			this.trackComps.Clear();
		}

		private void FastForwardTracking(uint timerId, object cookie)
		{
			this.trackComps.Sort(new Comparison<TrackingComponent>(this.SortTrackingForFastForwarding));
			List<TrackingComponent> list = null;
			for (int i = 0; i < this.trackComps.Count; i++)
			{
				TrackingComponent trackingComponent = this.trackComps[i];
				this.UpdateWithRandomAngle(trackingComponent);
				float randomDelayTime = this.GetRandomDelayTime(false);
				if (randomDelayTime > trackingComponent.IdleFastForwardTimeRemaining)
				{
					trackingComponent.IdleFastForwardTimeRemaining = 0f;
					trackingComponent.Yaw = trackingComponent.TargetYaw;
					trackingComponent.Mode = TrackingMode.Disabled;
					if (list == null)
					{
						list = new List<TrackingComponent>();
					}
					list.Add(trackingComponent);
				}
				else
				{
					trackingComponent.IdleFastForwardTimeRemaining -= randomDelayTime;
				}
			}
			if (list != null)
			{
				for (int j = 0; j < list.Count; j++)
				{
					this.trackComps.Remove(list[j]);
				}
			}
			if (this.trackComps.Count > 0)
			{
				this.FastForwardTracking(0u, null);
			}
		}

		private void FastForwardTrackingAfterWorldLoad(float fastForwardTime)
		{
			this.Stop(false);
			NodeList<TrackingNode> nodeList = Service.EntityController.GetNodeList<TrackingNode>();
			for (TrackingNode trackingNode = nodeList.Head; trackingNode != null; trackingNode = trackingNode.Next)
			{
				BuildingComponent buildingComp = trackingNode.BuildingComp;
				if (buildingComp != null && buildingComp.BuildingType.Type == BuildingType.Turret)
				{
					float randomDelayTime = this.GetRandomDelayTime(true);
					if (randomDelayTime < fastForwardTime)
					{
						trackingNode.TrackingComp.IdleFastForwardTimeRemaining = fastForwardTime - randomDelayTime;
						this.trackComps.Add(trackingNode.TrackingComp);
					}
				}
			}
			if (this.trackComps.Count > 0)
			{
				uint item = Service.ViewTimerManager.CreateViewTimer(0f, false, new TimerDelegate(this.FastForwardTracking), null);
				this.idleTimers.Add(item);
			}
		}

		private int SortTrackingForFastForwarding(TrackingComponent a, TrackingComponent b)
		{
			return b.IdleFastForwardTimeRemaining.CompareTo(a.IdleFastForwardTimeRemaining);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			if (id != EventId.WorldLoadComplete)
			{
				if (id == EventId.GameStateChanged)
				{
					Type type = (Type)cookie;
					if (currentState is BattleStartState || currentState is BattlePlayState)
					{
						this.Stop(false);
					}
					else if (currentState is EditBaseState)
					{
						this.Stop(true);
					}
					else if (currentState is HomeState && type == typeof(EditBaseState))
					{
						this.Start();
					}
				}
			}
			else if (currentState is BattlePlaybackState)
			{
				BattleRecord currentBattleRecord = Service.BattlePlaybackController.CurrentBattleRecord;
				this.FastForwardTrackingAfterWorldLoad(currentBattleRecord.ViewTimePassedPreBattle);
			}
			else
			{
				this.Start();
			}
			return EatResponse.NotEaten;
		}
	}
}
