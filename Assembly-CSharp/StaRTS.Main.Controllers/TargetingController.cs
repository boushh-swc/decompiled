using Net.RichardLord.Ash.Core;
using StaRTS.DataStructures;
using StaRTS.GameBoard;
using StaRTS.GameBoard.Pathfinding;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class TargetingController : IEventObserver
	{
		public delegate void UpdateTarget(ref int numTargetingDone);

		public delegate void OnTargetingDone(SmartEntity entity);

		public delegate bool TargetValidator(SmartEntity target, object cookie);

		private const int MAX_TARGETING_PER_UPDATE = 1;

		private const int MAX_REPATHING_PER_UPDATE = 1;

		private PathingManager pathingManager;

		private ShooterController shooterController;

		private SpatialIndexController spatialIndexController;

		private SmartEntityPriorityList buildingsToAttack;

		private NodeList<DefensiveTroopNode> defensiveTroopNodeList;

		private NodeList<OffensiveTroopNode> offensiveTroopNodeList;

		private NodeList<OffensiveHealerNode> offensiveHealerNodeList;

		private NodeList<DefensiveHealerNode> defensiveHealerNodeList;

		private int delayIndex;

		private static readonly int[] randomTargetingDelays = new int[]
		{
			14,
			41,
			32,
			7,
			23,
			16,
			21,
			41,
			8,
			10,
			13,
			8,
			35,
			13,
			29,
			3,
			17,
			42,
			20,
			4,
			26
		};

		private static readonly int[] x_mul = new int[]
		{
			1,
			1000,
			0,
			-1000,
			-1,
			-1000,
			0,
			1000
		};

		private static readonly int[] x_div = new int[]
		{
			1,
			1414,
			1,
			1414,
			1,
			1414,
			1,
			1414
		};

		private static readonly int[] z_mul = new int[]
		{
			0,
			1000,
			1,
			1000,
			0,
			-1000,
			-1,
			-1000
		};

		private static readonly int[] z_div = new int[]
		{
			1,
			1414,
			1,
			1414,
			1,
			1414,
			1,
			1414
		};

		public TargetingController()
		{
			Service.TargetingController = this;
			this.pathingManager = Service.PathingManager;
			this.shooterController = Service.ShooterController;
			this.spatialIndexController = Service.SpatialIndexController;
			EntityController entityController = Service.EntityController;
			this.defensiveTroopNodeList = entityController.GetNodeList<DefensiveTroopNode>();
			this.offensiveTroopNodeList = entityController.GetNodeList<OffensiveTroopNode>();
			this.offensiveHealerNodeList = entityController.GetNodeList<OffensiveHealerNode>();
			this.defensiveHealerNodeList = entityController.GetNodeList<DefensiveHealerNode>();
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.ProcBuff);
			eventManager.RegisterObserver(this, EventId.RemovingBuff);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			BuffEventData buffEventData = (BuffEventData)cookie;
			Service.Logger.DebugFormat("Buff proc: {0}", new object[]
			{
				buffEventData.BuffObj.BuffType.Uid
			});
			if (buffEventData.BuffObj.BuffType.Modify != BuffModify.Aggro)
			{
				return EatResponse.NotEaten;
			}
			if (id != EventId.ProcBuff)
			{
				if (id == EventId.RemovingBuff)
				{
					this.RemoveAggroBuff(buffEventData);
				}
			}
			else
			{
				this.ApplyAggroBuff(buffEventData);
			}
			return EatResponse.NotEaten;
		}

		public void RemoveAggroBuff(BuffEventData data)
		{
			this.InvalidateCurrentTarget(data.Target);
		}

		public void ApplyAggroBuff(BuffEventData data)
		{
			if (data.BuffObj.BuffData.ContainsKey("originator"))
			{
				Buff buffObj = data.BuffObj;
				SmartEntity smartEntity = (SmartEntity)buffObj.BuffData["originator"];
				ShooterComponent shooterComp = data.Target.ShooterComp;
				if (data.Target.TurretShooterComp == null && shooterComp == null)
				{
					return;
				}
				if (smartEntity == null)
				{
					return;
				}
				if (smartEntity.HealthComp == null)
				{
					return;
				}
				if (smartEntity.HealthComp.IsDead())
				{
					return;
				}
				int squaredDistanceToTarget = GameUtils.GetSquaredDistanceToTarget(shooterComp, smartEntity);
				if ((long)squaredDistanceToTarget > (long)((ulong)shooterComp.MaxAttackRangeSquared) || (long)squaredDistanceToTarget <= (long)((ulong)shooterComp.MinAttackRangeSquared))
				{
					return;
				}
				this.InvalidateCurrentTarget(data.Target);
				this.UpdateShooterTarget(true, data.Target, smartEntity);
			}
		}

		public void SeedRandomNumberGenerator(uint seed)
		{
			this.delayIndex = (int)(seed % (uint)TargetingController.randomTargetingDelays.Length);
		}

		public void Update(ref int flip, TargetingController.UpdateTarget updateOffensive, TargetingController.UpdateTarget updateDefensive, TargetingController.UpdateTarget updateOffensiveTroopPeriodicUpdate)
		{
			if (this.pathingManager.IsPathingOngoing())
			{
				SmartEntity smartEntity = null;
				bool flag;
				this.pathingManager.UpdatePathing(out flag, out smartEntity);
				if (flag)
				{
					this.OnPathingComplete(smartEntity, smartEntity.SecondaryTargetsComp, smartEntity.StateComp, smartEntity.ShooterComp, this.shooterController.GetPrimaryTarget(smartEntity.ShooterComp));
				}
			}
			else
			{
				int num = 0;
				if (flip == 0)
				{
					updateOffensive(ref num);
					updateDefensive(ref num);
					updateOffensiveTroopPeriodicUpdate(ref num);
				}
				else
				{
					updateDefensive(ref num);
					updateOffensive(ref num);
				}
				flip ^= 1;
			}
		}

		private SmartEntity FindValidTargetFromTroopNodeList<T>(NodeList<T> troopNodeList, SmartEntity attackerEntity, ref int maxWeight, SmartEntity finalTarget) where T : Node<T>, new()
		{
			for (T t = troopNodeList.Tail; t != null; t = t.Previous)
			{
				SmartEntity smartEntity = (SmartEntity)t.Entity;
				HealthComponent healthComp = smartEntity.HealthComp;
				if (!healthComp.IsDead() && (long)GameUtils.GetSquaredDistanceToTarget(attackerEntity.ShooterComp, smartEntity) >= (long)((ulong)attackerEntity.ShooterComp.MinAttackRangeSquared) && this.CheckTarget(attackerEntity, smartEntity, ref maxWeight))
				{
					finalTarget = smartEntity;
				}
			}
			return finalTarget;
		}

		private bool IsReachedMaxTargetingLimit(ref int numTroopTargetingDone)
		{
			return numTroopTargetingDone >= 1;
		}

		public void UpdateNodes<T>(NodeList<T> troopNodeList, ref int numTroopTargetingDone, TargetingController.OnTargetingDone onTroopTargetingDone, bool updateWallBreakingTroops) where T : Node<T>, new()
		{
			if (!this.IsReachedMaxTargetingLimit(ref numTroopTargetingDone))
			{
				for (T t = troopNodeList.Tail; t != null; t = t.Previous)
				{
					SmartEntity entity = (SmartEntity)t.Entity;
					if (this.UpdateNode(entity, onTroopTargetingDone, updateWallBreakingTroops))
					{
						numTroopTargetingDone++;
						if (this.IsReachedMaxTargetingLimit(ref numTroopTargetingDone))
						{
							break;
						}
					}
				}
			}
		}

		private bool UpdateWallBreakingTroops(SmartEntity entity, TargetingController.OnTargetingDone onTroopTargetingDone)
		{
			if (!entity.TroopComp.UpdateWallAttackerTroop)
			{
				return false;
			}
			entity.TroopComp.UpdateWallAttackerTroop = false;
			if (entity.ShooterComp.Target == null)
			{
				return false;
			}
			HealthComponent healthComp = entity.ShooterComp.Target.HealthComp;
			if (healthComp == null || healthComp.IsDead())
			{
				return false;
			}
			if (entity.ShooterComp.AttackFSM.IsAttacking && entity.SecondaryTargetsComp.CurrentAlternateTarget != null && entity.SecondaryTargetsComp.ObstacleTarget == null)
			{
				entity.ShooterComp.AttackFSM.StopAttacking(true);
				bool flag = false;
				bool flag2 = Service.PathingManager.RestartPathing(entity, out flag, false);
				if (flag2 && flag)
				{
					SmartEntity primaryTarget = this.shooterController.GetPrimaryTarget(entity.ShooterComp);
					onTroopTargetingDone(entity);
					this.shooterController.StopSearch(entity.ShooterComp);
					this.OnPathingComplete(entity, entity.SecondaryTargetsComp, entity.StateComp, entity.ShooterComp, primaryTarget);
				}
				return true;
			}
			return false;
		}

		private bool UpdateNode(SmartEntity entity, TargetingController.OnTargetingDone onTroopTargetingDone, bool updateWallBreakingTroops)
		{
			if (entity.StateComp.CurState == EntityState.Disable)
			{
				return false;
			}
			bool flag = false;
			if (updateWallBreakingTroops)
			{
				return this.UpdateWallBreakingTroops(entity, onTroopTargetingDone);
			}
			if (!GameUtils.IsEligibleToFindTarget(entity.ShooterComp))
			{
				return false;
			}
			ShooterComponent shooterComp = entity.ShooterComp;
			if (!shooterComp.Searching && !shooterComp.ReevaluateTarget)
			{
				return false;
			}
			bool flag2;
			if (shooterComp.Searching)
			{
				if (shooterComp.TargetingDelayAmount > 0)
				{
					shooterComp.TargetingDelayAmount--;
					return false;
				}
				flag2 = this.FindTargetForTroopNode(entity, false);
			}
			else
			{
				flag2 = this.FindTargetForTroopNode(entity, true);
			}
			shooterComp.ReevaluateTarget = false;
			if (!flag2)
			{
				return false;
			}
			if (!shooterComp.Searching)
			{
				shooterComp.AttackFSM.StopAttacking(true);
			}
			SmartEntity primaryTarget = this.shooterController.GetPrimaryTarget(entity.ShooterComp);
			if (primaryTarget.TransformComp == null)
			{
				return false;
			}
			flag = false;
			TroopComponent troopComp = entity.TroopComp;
			ITroopDeployableVO troopType = troopComp.TroopType;
			IShooterVO shooterVO = shooterComp.ShooterVO;
			uint maxAttackRange = this.pathingManager.GetMaxAttackRange(entity, primaryTarget);
			PathTroopParams troopParams = new PathTroopParams
			{
				TroopWidth = entity.SizeComp.Width,
				DPS = shooterVO.DPS,
				MinRange = shooterVO.MinAttackRange,
				MaxRange = maxAttackRange,
				MaxSpeed = troopComp.SpeedVO.MaxSpeed,
				PathSearchWidth = troopType.PathSearchWidth,
				IsMelee = shooterComp.IsMelee,
				IsOverWall = shooterComp.ShooterVO.OverWalls,
				IsHealer = TroopController.IsEntityHealer(entity),
				SupportRange = troopType.SupportFollowDistance,
				CrushesWalls = TroopController.CanEntityCrushWalls(entity),
				ProjectileType = shooterVO.ProjectileType,
				IsTargetShield = GameUtils.IsEntityShieldGenerator(primaryTarget),
				TargetInRangeModifier = troopType.TargetInRangeModifier
			};
			PathBoardParams boardParams = new PathBoardParams
			{
				IgnoreWall = TroopController.CanEntityIgnoreWalls(entity),
				Destructible = entity.TeamComp.CanDestructBuildings()
			};
			bool flag3 = this.pathingManager.StartPathing(entity, primaryTarget, entity.TransformComp, true, out flag, -1, troopParams, boardParams, false, false);
			if (!flag3)
			{
				onTroopTargetingDone(entity);
				this.shooterController.StopSearch(shooterComp);
				return true;
			}
			if (!flag)
			{
				GameUtils.UpdateMinimumFrameCountForNextTargeting(shooterComp);
				return false;
			}
			this.RandomizeTargetingDelay(shooterComp);
			onTroopTargetingDone(entity);
			this.shooterController.StopSearch(shooterComp);
			this.OnPathingComplete(entity, entity.SecondaryTargetsComp, entity.StateComp, shooterComp, primaryTarget);
			return true;
		}

		public void RandomizeTargetingDelay(ShooterComponent ShooterComp)
		{
			if (ShooterComp.TargetingDelayed)
			{
				ShooterComp.TargetingDelayAmount = TargetingController.randomTargetingDelays[this.delayIndex];
				this.delayIndex = (this.delayIndex + 1) % TargetingController.randomTargetingDelays.Length;
			}
		}

		public void OnPathingComplete(SmartEntity self, SecondaryTargetsComponent secondaryTargetsComp, StateComponent stateComp, ShooterComponent shooterComp, SmartEntity target)
		{
			this.UpdateAlterantiveTargets(secondaryTargetsComp, self, target);
			this.shooterController.StartMoving(self);
			if (!shooterComp.FirstTargetAcquired || (target.BuildingComp != null && target.BuildingComp.BuildingType.ShowReticleWhenTargeted))
			{
				shooterComp.FirstTargetAcquired = true;
				this.OnTroopAcquiredFirstTarget(self);
			}
		}

		private void TroopWandering(SmartEntity entity)
		{
			BoardController boardController = Service.BoardController;
			DefenderComponent defenderComp = entity.DefenderComp;
			if (defenderComp == null || defenderComp.Patrolling)
			{
				return;
			}
			TransformComponent transformComp = entity.TransformComp;
			if (transformComp == null)
			{
				return;
			}
			defenderComp.Patrolling = true;
			int x = transformComp.X;
			int z = transformComp.Z;
			BoardCell cellAt = boardController.Board.GetCellAt(x, z);
			int num = defenderComp.PatrolLoc;
			int num2 = (defenderComp.SpawnBuilding != null) ? 4 : 8;
			int viewRange = (int)entity.ShooterComp.ShooterVO.ViewRange;
			for (int i = 0; i < num2; i++)
			{
				BoardCell boardCell;
				if (defenderComp.SpawnBuilding == null)
				{
					num = (num + 1) % num2;
					int x2 = defenderComp.SpawnX + viewRange * TargetingController.x_mul[num] / TargetingController.x_div[num] / 2;
					int z2 = defenderComp.SpawnZ + viewRange * TargetingController.z_mul[num] / TargetingController.z_div[num] / 2;
					boardCell = boardController.Board.GetClampedToBoardCellAt(x2, z2, entity.SizeComp.Width);
				}
				else
				{
					boardCell = defenderComp.SpawnBuilding.FindNextPatrolPoint(entity.SizeComp.Width, ref num);
				}
				if (boardCell.IsWalkable())
				{
					if (Service.PathingManager.StartPathingWorkerOrPatrol(entity, null, cellAt, boardCell, entity.SizeComp.Width, entity.TroopComp != null && TroopController.CanEntityCrushWalls(entity)))
					{
						Service.ShooterController.StartMoving(entity);
						defenderComp.PatrolLoc = num;
						Service.ShooterController.StopSearch(entity.ShooterComp);
						return;
					}
				}
			}
			entity.StateComp.CurState = EntityState.Idle;
		}

		private SmartEntity FindBuildingAsTarget(SmartEntity entity, ref int maxWeight)
		{
			TransformComponent transformComp = entity.TransformComp;
			this.buildingsToAttack = this.spatialIndexController.GetBuildingsToAttack(transformComp.CenterGridX(), transformComp.CenterGridZ());
			SmartEntity result = null;
			if (this.buildingsToAttack != null)
			{
				ShooterComponent shooterComp = entity.ShooterComp;
				result = this.GetPrefferedBuilding(shooterComp, this.buildingsToAttack, ref maxWeight);
			}
			return result;
		}

		private SmartEntity FindTargetForAttacker(SmartEntity entity)
		{
			int num = -1;
			SmartEntity result = null;
			SmartEntity smartEntity = this.FindBuildingAsTarget(entity, ref num);
			smartEntity = this.FindValidTargetFromTroopNodeList<DefensiveTroopNode>(this.defensiveTroopNodeList, entity, ref num, smartEntity);
			smartEntity = this.FindValidTargetFromTroopNodeList<DefensiveHealerNode>(this.defensiveHealerNodeList, entity, ref num, smartEntity);
			if (smartEntity != null)
			{
				result = smartEntity;
			}
			return result;
		}

		private bool UpdateShooterTarget(bool onlyUpdateIfNewTargetFound, SmartEntity shooter, SmartEntity target)
		{
			bool result;
			if (onlyUpdateIfNewTargetFound)
			{
				result = this.UpdateShooterTargetIfDistinct(shooter, target);
			}
			else
			{
				result = this.UpdateShooterTargetIfNotNull(shooter, target);
			}
			return result;
		}

		private bool UpdateShooterTargetIfNotNull(SmartEntity shooter, SmartEntity target)
		{
			if (target != null)
			{
				shooter.ShooterComp.Target = target;
				return true;
			}
			return false;
		}

		private bool UpdateShooterTargetIfDistinct(SmartEntity shooter, SmartEntity target)
		{
			if (target != shooter.ShooterComp.Target)
			{
				this.InvalidateCurrentTarget(shooter);
				shooter.ShooterComp.Target = target;
				return target != null;
			}
			return false;
		}

		public bool FindTargetForTroopNode(SmartEntity entity, bool onlyUpdateIfNewTargetFound)
		{
			if (entity.TroopComp.TroopShooterVO.TargetLocking && entity.ShooterComp.Target != null)
			{
				return !onlyUpdateIfNewTargetFound;
			}
			if (!onlyUpdateIfNewTargetFound)
			{
				entity.ShooterComp.Target = null;
			}
			if (entity.TroopComp.TroopShooterVO.TargetSelf)
			{
				return this.UpdateShooterTarget(onlyUpdateIfNewTargetFound, entity, entity);
			}
			bool flag = false;
			if (TroopController.IsEntityHealer(entity))
			{
				SmartEntity smartEntity = this.FindBestTargetForHealer(entity);
				flag = this.UpdateShooterTarget(onlyUpdateIfNewTargetFound, entity, smartEntity);
			}
			else if (entity.TeamComp.TeamType == TeamType.Attacker)
			{
				SmartEntity smartEntity = this.FindTargetForAttacker(entity);
				if (smartEntity != null && entity.ShooterComp != null && entity.ShooterComp.ShooterVO.NewTargetOnReload)
				{
					entity.ShooterComp.AddEntityTargetIdToHistory(smartEntity.ID);
				}
				flag = this.UpdateShooterTarget(onlyUpdateIfNewTargetFound, entity, smartEntity);
			}
			else if (entity.TeamComp.TeamType == TeamType.Defender)
			{
				DefenderComponent defenderComp = entity.DefenderComp;
				SmartEntity smartEntity = this.FindOffensiveTroopAsTarget(entity);
				flag = this.UpdateShooterTarget(onlyUpdateIfNewTargetFound, entity, smartEntity);
				if (smartEntity != null && entity.ShooterComp != null && entity.ShooterComp.ShooterVO.NewTargetOnReload)
				{
					entity.ShooterComp.AddEntityTargetIdToHistory(smartEntity.ID);
				}
				if (flag)
				{
					entity.DefenderComp.Patrolling = false;
				}
				else if ((defenderComp.Leashed || !entity.ShooterComp.FirstTargetAcquired) && !onlyUpdateIfNewTargetFound && entity.StateComp.CurState != EntityState.Disable)
				{
					this.TroopWandering(entity);
				}
			}
			return flag;
		}

		private SmartEntity GetPrefferedBuilding(ShooterComponent shooterComp, SmartEntityPriorityList buildings, ref int maxWeight)
		{
			HashSet<string> hashSet = new HashSet<string>();
			SmartEntity result = null;
			int i = 0;
			int count = buildings.Count;
			while (i < count)
			{
				SmartEntityElementPriorityPair smartEntityElementPriorityPair = buildings.Get(i);
				SmartEntity element = smartEntityElementPriorityPair.Element;
				if (!shooterComp.ShooterVO.NewTargetOnReload || shooterComp.IsPotentialTargetNew(element.ID))
				{
					HealthComponent healthComp = element.HealthComp;
					if (healthComp != null && !healthComp.IsDead())
					{
						BuildingComponent buildingComp = element.BuildingComp;
						if (buildingComp.BuildingType.Type != BuildingType.Blocker)
						{
							if (element.TrapComp == null || element.TrapComp.CurrentState == TrapState.Armed)
							{
								if (hashSet.Add(buildingComp.BuildingType.BuildingID))
								{
									int num = this.CalculateWeight(shooterComp, null, healthComp.ArmorType, smartEntityElementPriorityPair.Priority);
									if (num > maxWeight)
									{
										maxWeight = num;
										result = element;
									}
								}
							}
						}
					}
				}
				i++;
			}
			return result;
		}

		private int CalculateWeight(ShooterComponent shooterComp, HealthComponent healthComp, ArmorType targetArmorType, int targetNearness)
		{
			int num = 0;
			if (shooterComp.ShooterVO.Preference != null)
			{
				num = shooterComp.ShooterVO.Preference[(int)targetArmorType] * 100;
			}
			int num2 = 1;
			if (healthComp != null)
			{
				num2 = (healthComp.MaxHealth - healthComp.Health) * 10000 / healthComp.MaxHealth;
			}
			return num * shooterComp.ShooterVO.PreferencePercentile + targetNearness * shooterComp.ShooterVO.NearnessPercentile + num2;
		}

		public bool CanBeHealed(SmartEntity target, SmartEntity healer)
		{
			if (target == null || healer == null)
			{
				return false;
			}
			TroopComponent troopComp = target.TroopComp;
			ShooterComponent shooterComp = healer.ShooterComp;
			return troopComp != null && shooterComp != null && shooterComp.ShooterVO.Preference[(int)troopComp.TroopType.ArmorType] > 0;
		}

		private bool IsEnemy(SmartEntity target, SmartEntity selfEntity)
		{
			return target.TeamComp != null && target.TeamComp.TeamType != selfEntity.TeamComp.TeamType;
		}

		private bool CheckTarget(SmartEntity shooter, SmartEntity target, ref int maxWeight)
		{
			if (shooter == null || target == null)
			{
				return false;
			}
			TransformComponent transformComp = target.TransformComp;
			TroopComponent troopComp = target.TroopComp;
			if (transformComp == null || troopComp == null)
			{
				return false;
			}
			ShooterComponent shooterComp = shooter.ShooterComp;
			if ((long)GameUtils.GetSquaredDistanceToTarget(shooterComp, target) < (long)((ulong)shooterComp.MinAttackRangeSquared))
			{
				return false;
			}
			if (TroopController.IsEntityPhantom(target))
			{
				return false;
			}
			if (shooterComp.OriginalShooterVO.NewTargetOnReload && !shooterComp.IsPotentialTargetNew(target.ID))
			{
				return false;
			}
			TransformComponent transformComp2 = shooter.TransformComp;
			int squaredDistance = GameUtils.SquaredDistance(transformComp2.CenterGridX(), transformComp2.CenterGridZ(), transformComp.CenterGridX(), transformComp.CenterGridZ());
			int targetNearness = this.spatialIndexController.CalcNearness(squaredDistance);
			int num = this.CalculateWeight(shooterComp, null, troopComp.TroopType.ArmorType, targetNearness);
			if (num > maxWeight)
			{
				maxWeight = num;
				return true;
			}
			return false;
		}

		private SmartEntity FindDefensiveTroopAsTarget(SmartEntity attacker, ref int maxWeight)
		{
			SmartEntity result = null;
			if (!this.defensiveTroopNodeList.Empty)
			{
				for (DefensiveTroopNode defensiveTroopNode = this.defensiveTroopNodeList.Head; defensiveTroopNode != null; defensiveTroopNode = defensiveTroopNode.Next)
				{
					if (this.CheckTarget(attacker, (SmartEntity)defensiveTroopNode.Entity, ref maxWeight))
					{
						result = (SmartEntity)defensiveTroopNode.Entity;
					}
				}
			}
			return result;
		}

		private SmartEntity FindOffensiveTroopAsTarget(SmartEntity entity)
		{
			if (this.offensiveTroopNodeList.Empty && this.offensiveHealerNodeList.Empty)
			{
				return null;
			}
			SmartEntity smartEntity = null;
			int num = -1;
			if (entity.WalkerComp != null)
			{
				if (!this.offensiveTroopNodeList.Empty)
				{
					for (OffensiveTroopNode offensiveTroopNode = this.offensiveTroopNodeList.Head; offensiveTroopNode != null; offensiveTroopNode = offensiveTroopNode.Next)
					{
						if (this.CheckTarget(entity, (SmartEntity)offensiveTroopNode.Entity, ref num))
						{
							smartEntity = (SmartEntity)offensiveTroopNode.Entity;
						}
					}
				}
				return this.FindValidTargetFromTroopNodeList<OffensiveHealerNode>(this.offensiveHealerNodeList, entity, ref num, smartEntity);
			}
			ShooterComponent shooterComp = entity.ShooterComp;
			TransformComponent transformComp = entity.TransformComp;
			SmartEntity smartEntity2 = this.TraverseSpiralToFindTarget((int)shooterComp.ShooterVO.ViewRange, transformComp.CenterGridX(), transformComp.CenterGridZ(), new TargetingController.TargetValidator(this.IsAttacker), entity);
			if (smartEntity2 != null && (long)GameUtils.GetSquaredDistanceToTarget(shooterComp, smartEntity2) >= (long)((ulong)shooterComp.MinAttackRangeSquared))
			{
				smartEntity = smartEntity2;
			}
			else
			{
				num = -1;
				smartEntity = this.FindValidTargetFromTroopNodeList<OffensiveTroopNode>(this.offensiveTroopNodeList, entity, ref num, null);
				smartEntity = this.FindValidTargetFromTroopNodeList<OffensiveHealerNode>(this.offensiveHealerNodeList, entity, ref num, smartEntity);
			}
			return smartEntity;
		}

		private SmartEntity FindBestTargetForHealer(SmartEntity entity)
		{
			ShooterComponent shooterComp = entity.ShooterComp;
			TransformComponent transformComp = entity.TransformComp;
			SmartEntity smartEntity = this.TraverseSpiralToFindTarget((int)shooterComp.ShooterVO.ViewRange, transformComp.CenterGridX(), transformComp.CenterGridZ(), new TargetingController.TargetValidator(this.IsHealable), entity);
			if (smartEntity == null)
			{
				if (entity.TeamComp.TeamType == TeamType.Defender)
				{
					for (DefensiveTroopNode defensiveTroopNode = this.defensiveTroopNodeList.Head; defensiveTroopNode != null; defensiveTroopNode = defensiveTroopNode.Next)
					{
						SmartEntity smartEntity2 = (SmartEntity)defensiveTroopNode.Entity;
						if (this.IsHealable(smartEntity2, entity))
						{
							smartEntity = smartEntity2;
							break;
						}
					}
				}
				else
				{
					for (OffensiveTroopNode offensiveTroopNode = this.offensiveTroopNodeList.Head; offensiveTroopNode != null; offensiveTroopNode = offensiveTroopNode.Next)
					{
						SmartEntity smartEntity3 = (SmartEntity)offensiveTroopNode.Entity;
						if (this.IsHealable(smartEntity3, entity))
						{
							smartEntity = smartEntity3;
							break;
						}
					}
				}
			}
			return smartEntity;
		}

		public void InformTurretsAboutTroop(List<EntityElementPriorityPair> turretsInRangeOf, SmartEntity entity, HashSet<ShooterComponent> resetReevaluateTargetSet)
		{
			int i = 0;
			int count = turretsInRangeOf.Count;
			while (i < count)
			{
				EntityElementPriorityPair entityElementPriorityPair = turretsInRangeOf[i];
				SmartEntity smartEntity = (SmartEntity)entityElementPriorityPair.Element;
				HealthComponent healthComp = smartEntity.HealthComp;
				if (healthComp != null && !healthComp.IsDead())
				{
					ShooterComponent shooterComp = smartEntity.ShooterComp;
					if (shooterComp != null)
					{
						TurretShooterComponent turretShooterComp = smartEntity.TurretShooterComp;
						if (turretShooterComp != null)
						{
							this.AddTurretTarget(shooterComp, turretShooterComp, entity, entityElementPriorityPair.Priority, resetReevaluateTargetSet);
						}
					}
				}
				i++;
			}
		}

		private void AddTurretTarget(ShooterComponent shooterComp, TurretShooterComponent turretShooterComp, SmartEntity target, int nearness, HashSet<ShooterComponent> resetReevaluateTargetSet)
		{
			if (!shooterComp.Searching && !shooterComp.ReevaluateTarget)
			{
				return;
			}
			if (shooterComp.ReevaluateTarget)
			{
				resetReevaluateTargetSet.Add(shooterComp);
			}
			TroopComponent troopComp = target.TroopComp;
			if (shooterComp.ShooterVO.Preference[(int)troopComp.TroopType.ArmorType] <= 0)
			{
				return;
			}
			int num = this.CalculateWeight(shooterComp, null, troopComp.TroopType.ArmorType, nearness);
			Entity turretTarget = this.shooterController.GetTurretTarget(shooterComp);
			if (turretTarget == null || num > turretShooterComp.TargetWeight)
			{
				turretShooterComp.TargetWeight = num;
				if (shooterComp.ReevaluateTarget && shooterComp.Target != target)
				{
					shooterComp.AttackFSM.StopAttacking(false);
				}
				shooterComp.Target = target;
			}
		}

		public void ReevaluateTarget(ShooterComponent shooterComp)
		{
			shooterComp.ReevaluateTarget = true;
			SmartEntity smartEntity = (SmartEntity)shooterComp.Entity;
			if (smartEntity.TurretShooterComp != null)
			{
				SmartEntity target = shooterComp.Target;
				int targetWeight = -1;
				if (target != null && !GameUtils.IsEntityDead(target))
				{
					List<EntityElementPriorityPair> turretsInRangeOf = this.spatialIndexController.GetTurretsInRangeOf(target.TransformComp.CenterGridX(), target.TransformComp.CenterGridZ());
					int i = 0;
					int count = turretsInRangeOf.Count;
					while (i < count)
					{
						EntityElementPriorityPair entityElementPriorityPair = turretsInRangeOf[i];
						if (smartEntity == entityElementPriorityPair.Element)
						{
							TroopComponent troopComp = target.TroopComp;
							targetWeight = this.CalculateWeight(shooterComp, null, troopComp.TroopType.ArmorType, entityElementPriorityPair.Priority);
							break;
						}
						i++;
					}
				}
				smartEntity.TurretShooterComp.TargetWeight = targetWeight;
			}
		}

		public void OnTroopAcquiredFirstTarget(Entity troopEntity)
		{
			Service.EventManager.SendEvent(EventId.TroopAcquiredFirstTarget, troopEntity);
		}

		public void StopSearchIfTargetFound(ShooterComponent shooterComp)
		{
			if (shooterComp.Target == null)
			{
				return;
			}
			this.shooterController.StopSearch(shooterComp);
		}

		public void UpdateAlterantiveTargets(SecondaryTargetsComponent secondarTargets, SmartEntity troop, SmartEntity target)
		{
			if (!this.ShouldSeekAlternativeTarget(troop))
			{
				return;
			}
			secondarTargets.CurrentAlternateTarget = null;
			PathingComponent pathingComp = troop.PathingComp;
			if (pathingComp != null)
			{
				secondarTargets.WallTargets = new LinkedList<Entity>();
				secondarTargets.AlternateTargets = pathingComp.CurrentPath.GetBlockingEntities(target.ID, out secondarTargets.WallTargets);
			}
		}

		private bool ShouldSeekAlternativeTarget(SmartEntity troopEntity)
		{
			if (troopEntity.TroopComp == null)
			{
				Service.Logger.Error("Non troop entity checking for alternative target in TargetingSystem");
				return false;
			}
			return troopEntity.DefenderComp == null && !TroopController.IsEntityHealer(troopEntity) && !TroopController.IsEntityPhantom(troopEntity);
		}

		public bool IsHealable(SmartEntity target, object self)
		{
			if (target == null)
			{
				return false;
			}
			SmartEntity smartEntity = (SmartEntity)self;
			if (target == smartEntity)
			{
				return false;
			}
			TroopComponent troopComp = target.TroopComp;
			HealthComponent healthComp = target.HealthComp;
			TeamComponent teamComp = target.TeamComp;
			if (troopComp == null || healthComp == null || teamComp == null)
			{
				return false;
			}
			if (target.TeamComp.TeamType != smartEntity.TeamComp.TeamType)
			{
				return false;
			}
			if (TroopController.IsEntityPhantom(target))
			{
				return false;
			}
			if (TroopController.IsEntityHealer(target))
			{
				return false;
			}
			TroopComponent troopComp2 = smartEntity.TroopComp;
			return troopComp2 == null || !TroopController.IsEntityHealer(smartEntity) || this.CanBeHealed(target, smartEntity);
		}

		public bool IsAttacker(SmartEntity target, object self)
		{
			return target != null && target.AttackerComp != null;
		}

		public bool IsAttackerThenFlag(SmartEntity target, object self)
		{
			if (target == null)
			{
				return false;
			}
			if (target.ShooterComp != null && target.ShooterComp.Target != null)
			{
				HealthComponent healthComp = target.ShooterComp.Target.HealthComp;
				if (healthComp == null || healthComp.IsDead())
				{
					return false;
				}
			}
			if (target.AttackerComp != null)
			{
				TroopComponent troopComp = target.TroopComp;
				if (troopComp != null && !troopComp.IsAbilityModeActive)
				{
					troopComp.UpdateWallAttackerTroop = true;
				}
			}
			return false;
		}

		public void UpdateNearbyTroops(int radius, int centerX, int centerZ)
		{
			TargetingController.TraverseSpiralToFindTargets(radius, centerX, centerZ, new TargetingController.TargetValidator(this.IsAttackerThenFlag), null, false);
		}

		private SmartEntity TraverseSpiralToFindTarget(int radius, int centerX, int centerZ, TargetingController.TargetValidator validator, object caller)
		{
			List<SmartEntity> list = TargetingController.TraverseSpiralToFindTargets(radius, centerX, centerZ, validator, caller, true);
			if (list != null && list.Count > 0)
			{
				return list[0];
			}
			return null;
		}

		public static List<SmartEntity> TraverseSpiralToFindTargets(int radius, int centerX, int centerZ, TargetingController.TargetValidator validator, object caller, bool returnFirstFound)
		{
			BoardCellDynamicArray boardCellDynamicArray = GameUtils.TraverseSpiral(radius, centerX, centerZ);
			List<SmartEntity> list = new List<SmartEntity>();
			for (int i = 0; i < boardCellDynamicArray.Length; i++)
			{
				BoardCell boardCell = boardCellDynamicArray.Array[i];
				if (boardCell.Children != null)
				{
					foreach (BoardItem current in boardCell.Children)
					{
						if (validator((SmartEntity)current.Data, caller))
						{
							list.Add((SmartEntity)current.Data);
							if (returnFirstFound)
							{
								return list;
							}
						}
					}
				}
			}
			return list;
		}

		public void InvalidateCurrentTarget(SmartEntity entity)
		{
			ShooterComponent shooterComp = entity.ShooterComp;
			if (shooterComp == null)
			{
				return;
			}
			shooterComp.Target = null;
			SecondaryTargetsComponent secondaryTargetsComp = entity.SecondaryTargetsComp;
			if (secondaryTargetsComp != null)
			{
				secondaryTargetsComp.ObstacleTarget = null;
				secondaryTargetsComp.CurrentAlternateTarget = null;
				secondaryTargetsComp.AlternateTargets = null;
				secondaryTargetsComp.WallTargets = null;
				secondaryTargetsComp.CurrentWallTarget = null;
			}
		}
	}
}
