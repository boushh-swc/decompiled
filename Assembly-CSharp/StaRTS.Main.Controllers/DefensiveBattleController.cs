using Net.RichardLord.Ash.Core;
using StaRTS.DataStructures;
using StaRTS.GameBoard;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class DefensiveBattleController : IEventObserver
	{
		private class CameraEventData
		{
			public float timeStamp;

			public DefensiveCameraEventType type;

			public float posX;

			public float posZ;
		}

		private const int TROOP_UID_ARG = 0;

		private const int QUANTITY_ARG = 1;

		private const int DIRECTION_ARG = 2;

		private const int SPREAD_ARG = 3;

		private const int RANGE_ARG = 4;

		private const int TIME_ARG = 5;

		private const int ARGUMENT_COUNT = 6;

		private const float EXPIRATION_TIME_SPAWN_EVENT = 0.5f;

		private const float EXPIRATION_TIME_DEATH_EVENT = 0.5f;

		private const float EXPIRATION_TIME_DAMAGE_EVENT = 0.2f;

		private const float MIN_CAMERA_TRAVEL_DISTANCE = 10f;

		private const int MIN_ENTITY_HITS_FOR_CAMERA_RELEVANCE = 5;

		private SimTimerManager timerManager;

		private StaticDataController sdc;

		private TroopController troopController;

		private EventManager eventManager;

		private List<uint> timers;

		private List<DefenseWave> waves;

		private int waveCount;

		private int currentWaveIndex;

		private int wavesDeployed;

		private DefenseWave currentWave;

		private int waveDirectionOffset;

		private bool randomizeWaves = true;

		private CampaignMissionVO currentMission;

		private bool activeDefenseBattle;

		private bool autoMoveCamera;

		private DefensiveBattleController.CameraEventData currentCameraEvent;

		private List<DefensiveBattleController.CameraEventData> cameraEvents;

		private Dictionary<uint, int> numTimesEntityHit;

		public bool AllWavesClear
		{
			get;
			private set;
		}

		public bool Active
		{
			get
			{
				return this.activeDefenseBattle;
			}
		}

		public DefensiveBattleController()
		{
			Service.DefensiveBattleController = this;
			this.sdc = Service.StaticDataController;
			this.timerManager = Service.SimTimerManager;
			this.timers = new List<uint>();
			this.cameraEvents = new List<DefensiveBattleController.CameraEventData>();
			this.numTimesEntityHit = new Dictionary<uint, int>();
		}

		public void StartDefenseMission(CampaignMissionVO mission)
		{
			if (this.activeDefenseBattle)
			{
				Service.Logger.WarnFormat("Mission {0} is already in progress.  Cannot start Mission {1}.", new object[]
				{
					this.currentMission.Uid,
					mission.Uid
				});
				return;
			}
			this.troopController = Service.TroopController;
			this.eventManager = Service.EventManager;
			this.activeDefenseBattle = true;
			this.autoMoveCamera = false;
			this.currentMission = mission;
			this.randomizeWaves = !mission.FixedWaves;
			this.waves = DefensiveBattleController.ParseWaves(this.currentMission.Waves);
			this.wavesDeployed = 0;
			this.waveCount = this.waves.Count;
			this.AllWavesClear = false;
			if (mission.IsRaidDefense())
			{
				Service.RaidDefenseController.OnStartRaidDefenseMission();
			}
			this.eventManager.RegisterObserver(this, EventId.EntityKilled, EventPriority.BeforeDefault);
			this.eventManager.RegisterObserver(this, EventId.UserStartedCameraMove, EventPriority.Default);
			this.eventManager.RegisterObserver(this, EventId.CameraFinishedMoving, EventPriority.Default);
			this.eventManager.RegisterObserver(this, EventId.PreEntityKilled, EventPriority.Default);
			this.eventManager.RegisterObserver(this, EventId.EntityHit, EventPriority.Default);
			TrapViewController trapViewController = Service.TrapViewController;
			NodeList<TrapNode> trapNodeList = Service.BuildingLookupController.TrapNodeList;
			for (TrapNode trapNode = trapNodeList.Head; trapNode != null; trapNode = trapNode.Next)
			{
				trapViewController.UpdateTrapVisibility((SmartEntity)trapNode.Entity);
			}
			Vector3 zero = Vector3.zero;
			Service.WorldInitializer.View.ZoomTo(0.7f);
			Service.WorldInitializer.View.PanToLocation(zero);
			this.DeployNextWave(0u, null);
		}

		private void DeployNextWave(uint id, object cookie)
		{
			this.timers.Remove(id);
			this.currentWaveIndex = this.wavesDeployed;
			this.currentWave = this.waves[this.currentWaveIndex];
			this.waveDirectionOffset = ((!this.randomizeWaves) ? 0 : Service.Rand.SimRange(0, 360));
			List<DefenseTroopGroup> list = DefensiveBattleController.ParseTroopGroups(this.currentWave.Encounter.Uid, this.currentWave.Encounter.WaveGroup, this.waveDirectionOffset);
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				DefenseTroopGroup defenseTroopGroup = list[i];
				this.timers.Add(this.timerManager.CreateSimTimer(defenseTroopGroup.Seconds * 1000u, false, new TimerDelegate(this.DeployGroupAfterDelay), defenseTroopGroup));
			}
			this.wavesDeployed++;
		}

		private void EndCurrentWave()
		{
			this.currentWave = null;
			if (this.wavesDeployed < this.waveCount)
			{
				uint delay = (uint)(this.waves[this.currentWaveIndex + 1].Delay * 1000f);
				this.timers.Add(this.timerManager.CreateSimTimer(delay, false, new TimerDelegate(this.DeployNextWave), null));
				Lang lang = Service.Lang;
				string instructions = lang.Get("NEXT_WAVE_IN", new object[]
				{
					(int)this.waves[this.currentWaveIndex + 1].Delay
				});
				Service.UXController.MiscElementsManager.ShowPlayerInstructions(instructions, 3f, 2f);
			}
			else
			{
				this.AllWavesClear = true;
			}
		}

		private void DeployGroupAfterDelay(uint id, object cookie)
		{
			this.timers.Remove(id);
			DefenseTroopGroup defenseTroopGroup = (DefenseTroopGroup)cookie;
			TroopTypeVO troop = this.sdc.Get<TroopTypeVO>(defenseTroopGroup.TroopUid);
			for (int i = 0; i < defenseTroopGroup.Quantity; i++)
			{
				this.DeployTroopGroup(i, troop, defenseTroopGroup);
			}
			if (this.waves[this.currentWaveIndex].Troops.Count == 0 && this.timers.Count == 0)
			{
				this.EndCurrentWave();
			}
			if (Service.CurrentPlayer.CampaignProgress.FueInProgress)
			{
				int boardX = 0;
				int boardZ = 0;
				DefensiveBattleController.GetBoardEdge(defenseTroopGroup.Direction, defenseTroopGroup.Range, out boardX, out boardZ);
				this.AddCameraEvent(Units.BoardToWorldX(boardX), Units.BoardToWorldZ(boardZ), DefensiveCameraEventType.TroopSpawned);
			}
		}

		public void EndEncounter()
		{
			if (!this.activeDefenseBattle)
			{
				return;
			}
			this.eventManager.UnregisterObserver(this, EventId.EntityKilled);
			if (this.currentMission.IsRaidDefense())
			{
				Service.RaidDefenseController.OnEndRaidDefenseMission(this.waves[this.currentWaveIndex].Encounter.Uid);
			}
			this.currentMission = null;
			this.activeDefenseBattle = false;
			this.currentCameraEvent = null;
			for (int i = 0; i < this.timers.Count; i++)
			{
				this.timerManager.KillSimTimer(this.timers[i]);
			}
			this.timers.Clear();
			this.cameraEvents.Clear();
			this.numTimesEntityHit.Clear();
			this.UnRegisterObservers();
			Service.CurrentPlayer.DamagedBuildings = Service.BattleController.GetBuildingDamageMap();
		}

		private void UnRegisterObservers()
		{
			this.eventManager.UnregisterObserver(this, EventId.CameraFinishedMoving);
			this.eventManager.UnregisterObserver(this, EventId.UserStartedCameraMove);
			this.eventManager.UnregisterObserver(this, EventId.PreEntityKilled);
			this.eventManager.UnregisterObserver(this, EventId.EntityHit);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.PreEntityKilled:
			{
				Entity entity = (Entity)cookie;
				BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
				TransformComponent transformComponent = entity.Get<TransformComponent>();
				if (transformComponent != null)
				{
					DefensiveCameraEventType type = DefensiveCameraEventType.TroopDestroyed;
					if (buildingComponent != null)
					{
						if (buildingComponent.BuildingType.Type == BuildingType.Wall)
						{
							type = DefensiveCameraEventType.WallDestroyed;
						}
						else
						{
							type = DefensiveCameraEventType.BuildingDestroyed;
						}
					}
					this.AddCameraEvent(Units.BoardToWorldX(transformComponent.CenterX()), Units.BoardToWorldZ(transformComponent.CenterZ()), type);
				}
				return EatResponse.NotEaten;
			}
			case EventId.EntityKilled:
			{
				Entity item = (Entity)cookie;
				if (this.currentWave != null && this.currentWave.Troops.Contains(item))
				{
					this.currentWave.Troops.Remove(item);
					if (this.currentWave.Troops.Count == 0 && this.timers.Count == 0)
					{
						this.EndCurrentWave();
					}
				}
				return EatResponse.NotEaten;
			}
			case EventId.PostBuildingEntityKilled:
			case EventId.EntityDestroyed:
				IL_21:
				if (id == EventId.CameraFinishedMoving)
				{
					this.MoveCameraToAction();
					return EatResponse.NotEaten;
				}
				if (id != EventId.UserStartedCameraMove)
				{
					return EatResponse.NotEaten;
				}
				this.autoMoveCamera = false;
				this.cameraEvents.Clear();
				this.numTimesEntityHit.Clear();
				this.UnRegisterObservers();
				return EatResponse.NotEaten;
			case EventId.EntityHit:
			{
				Bullet bullet = (Bullet)cookie;
				SmartEntity target = bullet.Target;
				uint iD = target.ID;
				TransformComponent transformComp = target.TransformComp;
				BuildingComponent buildingComp = target.BuildingComp;
				if (buildingComp != null && transformComp != null)
				{
					if (this.numTimesEntityHit.ContainsKey(iD))
					{
						Dictionary<uint, int> dictionary;
						Dictionary<uint, int> expr_191 = dictionary = this.numTimesEntityHit;
						uint key;
						uint expr_196 = key = iD;
						int num = dictionary[key];
						expr_191[expr_196] = num + 1;
					}
					else
					{
						this.numTimesEntityHit.Add(iD, 1);
					}
					if (this.currentCameraEvent == null || this.numTimesEntityHit[iD] > 5)
					{
						this.numTimesEntityHit[iD] = 0;
						this.AddCameraEvent(Units.BoardToWorldX(transformComp.CenterX()), Units.BoardToWorldZ(transformComp.CenterZ()), DefensiveCameraEventType.EntityDamaged);
					}
				}
				return EatResponse.NotEaten;
			}
			}
			goto IL_21;
		}

		private void AddCameraEvent(float x, float z, DefensiveCameraEventType type)
		{
			if (!this.autoMoveCamera)
			{
				return;
			}
			DefensiveBattleController.CameraEventData cameraEventData = new DefensiveBattleController.CameraEventData();
			cameraEventData.posX = x;
			cameraEventData.posZ = z;
			cameraEventData.type = type;
			cameraEventData.timeStamp = Time.time;
			this.cameraEvents.Add(cameraEventData);
			this.MoveCameraToAction();
		}

		private void MoveCameraToAction()
		{
			if (!this.autoMoveCamera)
			{
				return;
			}
			List<DefensiveBattleController.CameraEventData> mostReleventCameraEvents = this.GetMostReleventCameraEvents();
			if (mostReleventCameraEvents != null && mostReleventCameraEvents.Count > 0)
			{
				float num = 0f;
				float num2 = 0f;
				foreach (DefensiveBattleController.CameraEventData current in mostReleventCameraEvents)
				{
					num += current.posX;
					num2 += current.posZ;
				}
				Vector3 zero = Vector3.zero;
				zero.x = num / (float)mostReleventCameraEvents.Count;
				zero.z = num2 / (float)mostReleventCameraEvents.Count;
				Vector3 vector = Service.CameraManager.MainCamera.Camera.WorldToViewportPoint(zero);
				this.currentCameraEvent = mostReleventCameraEvents[0];
				this.currentCameraEvent.posX = zero.x;
				this.currentCameraEvent.posZ = zero.z;
				if ((double)vector.x < 0.1 || (double)vector.x > 0.9 || (double)vector.y < 0.1 || (double)vector.y > 0.9)
				{
					Service.WorldInitializer.View.PanToLocation(zero);
				}
			}
			else
			{
				this.currentCameraEvent = null;
			}
		}

		private List<DefensiveBattleController.CameraEventData> GetMostReleventCameraEvents()
		{
			if (this.cameraEvents.Count > 0)
			{
				DefensiveCameraEventType defensiveCameraEventType = DefensiveCameraEventType.None;
				if (this.currentCameraEvent != null)
				{
					defensiveCameraEventType = this.currentCameraEvent.type;
				}
				List<DefensiveBattleController.CameraEventData> list = new List<DefensiveBattleController.CameraEventData>();
				float num = 10f;
				float time = Time.time;
				for (int i = this.cameraEvents.Count - 1; i >= 0; i--)
				{
					DefensiveBattleController.CameraEventData cameraEventData = this.cameraEvents[i];
					if (this.currentCameraEvent != null)
					{
						Vector2 a = new Vector2(cameraEventData.posX, cameraEventData.posZ);
						Vector2 b = new Vector2(this.currentCameraEvent.posX, this.currentCameraEvent.posZ);
						num = Math.Abs(Vector2.Distance(a, b));
					}
					if (num >= 10f && time < cameraEventData.timeStamp + this.GetExpirationTimeForEvent(cameraEventData.type) && cameraEventData.type <= defensiveCameraEventType)
					{
						list.Add(cameraEventData);
					}
				}
				return list;
			}
			return null;
		}

		private void DeployTroopGroup(int index, TroopTypeVO troop, DefenseTroopGroup group)
		{
			int degrees = group.Direction;
			if (group.Quantity > 1)
			{
				degrees = group.Direction + group.Spread * index / (group.Quantity - 1) - group.Spread / 2;
			}
			int locX = 0;
			int locZ = 0;
			DefensiveBattleController.GetBoardEdge(degrees, group.Range, out locX, out locZ);
			IntPosition nearestValidBoardPosition = this.GetNearestValidBoardPosition(locX, locZ);
			bool sendPlacedEvent = index == 0;
			Entity entity = this.troopController.SpawnTroop(troop, TeamType.Attacker, nearestValidBoardPosition, TroopSpawnMode.Unleashed, sendPlacedEvent);
			if (entity != null)
			{
				if (!troop.IsHealer)
				{
					this.waves[this.currentWaveIndex].Troops.Add(entity);
				}
				Service.BattleController.OnTroopDeployed(troop.Uid, TeamType.Attacker, nearestValidBoardPosition);
				Service.EventManager.SendEvent(EventId.TroopDeployed, entity);
			}
		}

		private IntPosition GetNearestValidBoardPosition(int locX, int locZ)
		{
			Board board = Service.BoardController.Board;
			BoardCell cellAt = board.GetCellAt(locX, locZ);
			if (cellAt != null && !cellAt.IsNotSpawnProtected())
			{
				int i = 1;
				int boardSize = board.BoardSize;
				bool flag = false;
				while (i < boardSize)
				{
					for (int j = -1; j < 1; j++)
					{
						for (int k = -1; k < 1; k++)
						{
							cellAt = board.GetCellAt(locX + i * j, locZ + i * k);
							if (cellAt != null && cellAt.IsNotSpawnProtected())
							{
								flag = true;
								locX += i * j;
								locZ += i * k;
								j = 1;
								break;
							}
						}
					}
					if (flag)
					{
						break;
					}
					i++;
				}
				if (!flag)
				{
					Service.Logger.WarnFormat("Could not find valid spawn location near {0}, {1}", new object[]
					{
						locX,
						locZ
					});
				}
			}
			return new IntPosition(locX, locZ);
		}

		public static void GetBoardEdge(int degrees, int gridSize, out int finalX, out int finalY)
		{
			degrees %= 360;
			int num = gridSize / 2;
			if (degrees > 315 || degrees <= 45)
			{
				if (degrees >= 315)
				{
					degrees -= 360;
				}
				finalX = num;
				finalY = degrees * 1024 / 45 * num / 1024;
			}
			else if (degrees > 45 && degrees <= 135)
			{
				degrees -= 90;
				finalX = degrees * 1024 / 45 * -num / 1024;
				finalY = num;
			}
			else if (degrees > 135 && degrees <= 225)
			{
				degrees -= 180;
				finalX = -num;
				finalY = degrees * 1024 / 45 * -num / 1024;
			}
			else if (degrees > 225 && degrees <= 315)
			{
				degrees -= 270;
				finalX = degrees * 1024 / 45 * num / 1024;
				finalY = -num;
			}
			else
			{
				finalX = 0;
				finalY = 0;
			}
		}

		public static List<DefenseWave> ParseWaves(string waveData)
		{
			List<DefenseWave> list = new List<DefenseWave>();
			string[] array = waveData.Split(new char[]
			{
				' '
			});
			for (int i = 0; i < array.Length; i += 2)
			{
				float delay = 0f;
				if (i != 0)
				{
					delay = Convert.ToSingle(array[i - 1]);
				}
				DefenseWave item = new DefenseWave(array[i], delay);
				list.Add(item);
			}
			return list;
		}

		public static List<DefenseTroopGroup> ParseTroopGroups(string uid, string encounterData, int directionOffset)
		{
			encounterData = encounterData.Trim();
			List<DefenseTroopGroup> list = new List<DefenseTroopGroup>();
			string[] array = encounterData.Split(new char[]
			{
				' '
			});
			try
			{
				for (int i = 0; i < array.Length; i++)
				{
					DefenseTroopGroup defenseTroopGroup = new DefenseTroopGroup();
					string text = array[i];
					string[] array2 = text.Split(new char[]
					{
						','
					});
					if (array2.Length != 6)
					{
						Service.Logger.ErrorFormat("{0} There is an error parsing group {1} of DefenseEncounter {2}", new object[]
						{
							"Bad Metadata.",
							i.ToString(),
							uid
						});
					}
					defenseTroopGroup.TroopUid = array2[0];
					defenseTroopGroup.Quantity = Convert.ToInt32(array2[1]);
					defenseTroopGroup.Direction = Convert.ToInt32(array2[2]);
					defenseTroopGroup.Direction += directionOffset;
					defenseTroopGroup.Direction %= 360;
					defenseTroopGroup.Spread = Convert.ToInt32(array2[3]);
					defenseTroopGroup.Range = Math.Min(Convert.ToInt32(array2[4]), 45);
					defenseTroopGroup.Seconds = Convert.ToUInt32(array2[5]);
					list.Add(defenseTroopGroup);
				}
			}
			catch (Exception ex)
			{
				Service.Logger.ErrorFormat("There was an error parsing the data for DefenseEncounter: {0}", new object[]
				{
					uid
				});
				throw ex;
			}
			return list;
		}

		private float GetExpirationTimeForEvent(DefensiveCameraEventType type)
		{
			switch (type)
			{
			case DefensiveCameraEventType.BuildingDestroyed:
			case DefensiveCameraEventType.WallDestroyed:
			case DefensiveCameraEventType.TroopDestroyed:
				return 0.5f;
			case DefensiveCameraEventType.TroopSpawned:
				return 0.5f;
			case DefensiveCameraEventType.EntityDamaged:
				return 0.2f;
			default:
				return 0f;
			}
		}
	}
}
