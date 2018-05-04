using StaRTS.DataStructures;
using StaRTS.GameBoard;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class SummonController : IEventObserver
	{
		private Dictionary<SmartEntity, List<SmartEntity>> visitorExecution;

		private int numSummons;

		public SummonController()
		{
			Service.SummonController = this;
			this.visitorExecution = new Dictionary<SmartEntity, List<SmartEntity>>();
			this.numSummons = 0;
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.GameStateChanged);
			eventManager.RegisterObserver(this, EventId.ProcBuff);
			eventManager.RegisterObserver(this, EventId.EntityKilled);
		}

		private bool VerifyBoardPosition(TransformComponent originalGridPos, Vector3 offset, float rotation, int troopWidth, TeamType teamType, out IntPosition validPosition)
		{
			bool result = true;
			Vector3 point = new Vector3(offset.x, 0f, offset.z);
			float y = (-rotation + 1.57079637f) * 57.29578f;
			Vector3 vector = Quaternion.Euler(0f, y, 0f) * point;
			int num = (int)Mathf.Round(vector.x);
			int num2 = (int)Mathf.Round(vector.z);
			validPosition = new IntPosition(Units.GridToBoardX(originalGridPos.X + num), Units.GridToBoardZ(originalGridPos.Z + num2));
			BoardCell boardCell = null;
			if (!Service.TroopController.ValidateTroopPlacement(validPosition, teamType, troopWidth, false, out boardCell, false))
			{
				validPosition = new IntPosition(Units.GridToBoardX(originalGridPos.X), Units.GridToBoardZ(originalGridPos.Z));
				result = false;
			}
			return result;
		}

		private void HandleSummon(BuffEventData data)
		{
			SummonDetailsVO summonDetailsVO = (SummonDetailsVO)data.BuffObj.Details;
			if (data.BuffObj.ProcCount > summonDetailsVO.MaxProc)
			{
				return;
			}
			if (this.numSummons >= GameConstants.MAX_SUMMONS_PER_BATTLE)
			{
				return;
			}
			List<SmartEntity> list;
			bool flag = this.visitorExecution.TryGetValue(data.Target, out list);
			if (list == null)
			{
				list = new List<SmartEntity>();
			}
			if (summonDetailsVO.VisitorUids == null || summonDetailsVO.VisitorUids.Length == 0)
			{
				Service.Logger.WarnFormat("No visitor uid for buff {0}", new object[]
				{
					summonDetailsVO.Uid
				});
				return;
			}
			int num = (data.BuffObj.ProcCount - 1) % summonDetailsVO.VisitorUids.Length;
			string text = summonDetailsVO.VisitorUids[num];
			TransformComponent transformComponent = data.Target.Get<TransformComponent>();
			TeamType teamType = data.Target.TeamComp.TeamType;
			if (!summonDetailsVO.SameTeam)
			{
				teamType = ((data.Target.TeamComp.TeamType != TeamType.Attacker) ? TeamType.Attacker : TeamType.Defender);
			}
			SmartEntity smartEntity = null;
			if (summonDetailsVO.VisitorType == VisitorType.Troop)
			{
				int num2 = (data.BuffObj.ProcCount - 1) % summonDetailsVO.SpawnPoints.Length;
				Vector3 offset = summonDetailsVO.SpawnPoints[num2];
				TroopTypeVO troopTypeVO = Service.StaticDataController.Get<TroopTypeVO>(text);
				IntPosition boardPosition;
				bool flag2 = this.VerifyBoardPosition(transformComponent, offset, transformComponent.Rotation, troopTypeVO.SizeX, teamType, out boardPosition);
				bool forceAllow = !flag2;
				int randomSpawnRadius = summonDetailsVO.RandomSpawnRadius;
				if (troopTypeVO.Type == TroopType.Phantom && randomSpawnRadius > 0)
				{
					forceAllow = true;
					BattleController battleController = Service.BattleController;
					RandSimSeed simSeed = Service.Rand.SimSeed;
					int num3 = (int)((battleController.Now + simSeed.SimSeedA) % (uint)randomSpawnRadius);
					int num4 = (int)((battleController.Now + simSeed.SimSeedB) % (uint)randomSpawnRadius);
					if (num3 % 2 == 0)
					{
						num3 *= -1;
					}
					if (num4 % 2 == 1)
					{
						num4 *= -1;
					}
					boardPosition.x = Mathf.Clamp(boardPosition.x + num3, -23, 23);
					boardPosition.z = Mathf.Clamp(boardPosition.z + num4, -23, 23);
				}
				smartEntity = Service.TroopController.SpawnTroop(troopTypeVO, teamType, boardPosition, TroopSpawnMode.Unleashed, true, forceAllow, summonDetailsVO.VisitorType);
				if (smartEntity == null)
				{
					Service.Logger.WarnFormat("could not spawn troop visitor {0} for buff {1}", new object[]
					{
						text,
						summonDetailsVO.Uid
					});
				}
				else
				{
					this.numSummons++;
				}
			}
			else if (summonDetailsVO.VisitorType == VisitorType.SpecialAttack)
			{
				Vector3 zero = Vector3.zero;
				zero.x = Units.BoardToWorldX(transformComponent.X);
				zero.z = Units.BoardToWorldX(transformComponent.Z);
				SpecialAttackTypeVO specialAttackType = Service.StaticDataController.Get<SpecialAttackTypeVO>(text);
				if (Service.SpecialAttackController.DeploySpecialAttack(specialAttackType, teamType, zero) == null)
				{
					Service.Logger.WarnFormat("could not spawn special attack {0} for buff {1}", new object[]
					{
						text,
						summonDetailsVO.Uid
					});
				}
				else
				{
					this.AddVisitorToValidate(data.BuffObj.BuffType, text, 1);
				}
			}
			else
			{
				Service.Logger.WarnFormat("Unhandled visitor type {0} for {1}", new object[]
				{
					summonDetailsVO.VisitorType,
					summonDetailsVO.Uid
				});
			}
			if (smartEntity != null)
			{
				if (summonDetailsVO.DieWithSummoner)
				{
					list.Add(smartEntity);
					if (!flag)
					{
						this.visitorExecution[data.Target] = list;
					}
				}
				if (summonDetailsVO.TargetSummoner && smartEntity.ShooterComp != null)
				{
					smartEntity.ShooterComp.Target = data.Target;
				}
				this.AddVisitorToValidate(data.BuffObj.BuffType, text, 1);
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.ProcBuff)
			{
				if (id != EventId.EntityKilled)
				{
					if (id == EventId.GameStateChanged)
					{
						IState currentState = Service.GameStateMachine.CurrentState;
						if (currentState is BattleStartState || currentState is BattlePlaybackState)
						{
							this.BattleInit();
						}
					}
				}
				else
				{
					SmartEntity key = cookie as SmartEntity;
					if (this.visitorExecution.ContainsKey(key))
					{
						List<SmartEntity> list = this.visitorExecution[key];
						if (list != null)
						{
							for (int i = 0; i < list.Count; i++)
							{
								Service.HealthController.KillEntity(list[i]);
							}
						}
						this.visitorExecution.Remove(key);
					}
				}
			}
			else
			{
				BuffEventData buffEventData = (BuffEventData)cookie;
				if (buffEventData.BuffObj.BuffType.Modify != BuffModify.Summon)
				{
					return EatResponse.NotEaten;
				}
				this.HandleSummon(buffEventData);
			}
			return EatResponse.NotEaten;
		}

		private void AddVisitorToValidate(BuffTypeVO buffType, string summonUid, int numVisitors)
		{
			if (buffType.AppliesOnlyToSelf())
			{
				CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
				int num = 0;
				currentBattle.NumVisitors.TryGetValue(summonUid, out num);
				currentBattle.NumVisitors[summonUid] = num + numVisitors;
			}
		}

		private void BattleInit()
		{
			if (this.visitorExecution != null)
			{
				this.visitorExecution.Clear();
			}
			this.numSummons = 0;
		}
	}
}
