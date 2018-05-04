using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard.Components;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Squads.War;
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
	public class BuffController : IEventObserver
	{
		private const string BUFF_EFFECT_ID_FORMAT = "BuffEffect_{0}";

		private List<WarBuffVO> defenderWarBuffs;

		private List<WarBuffVO> attackerWarBuffs;

		private List<EquipmentEffectVO> attackerEquipmentBuffs;

		private List<EquipmentEffectVO> defenderEquipmentBuffs;

		private Dictionary<SmartEntity, Buff> shaderSwappedBuffs;

		protected List<Material> shaderSwappedMaterials;

		private Dictionary<int, Material> oldMaterials;

		public BuffController()
		{
			Service.BuffController = this;
			this.defenderWarBuffs = new List<WarBuffVO>();
			this.attackerWarBuffs = new List<WarBuffVO>();
			this.attackerEquipmentBuffs = new List<EquipmentEffectVO>();
			this.defenderEquipmentBuffs = new List<EquipmentEffectVO>();
			this.shaderSwappedBuffs = new Dictionary<SmartEntity, Buff>();
			this.shaderSwappedMaterials = new List<Material>();
			this.oldMaterials = new Dictionary<int, Material>();
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.TroopAbilityActivate, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.TroopAbilityDeactivate, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.EntityHit, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.AddedBuff, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.RemovingBuff, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.TroopPlacedOnBoard, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingPlacedOnBoard, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.SpecialAttackSpawned, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BattleLoadedForDefend, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.EquipmentBuffShaderApply, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.EquipmentBuffShaderRemove, EventPriority.Default);
		}

		public void ApplyActiveBuffs(SmartEntity target, BuffModify modify, ref int modifyValue, int modifyValueMax)
		{
			BuffComponent buffComp = target.BuffComp;
			if (buffComp != null)
			{
				buffComp.ApplyActiveBuffs(modify, ref modifyValue, modifyValueMax);
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.EntityHit:
				this.ApplyBuffFromBullet((Bullet)cookie);
				return EatResponse.NotEaten;
			case EventId.EntityHealthChanged:
			case EventId.EntityHitByBeam:
			{
				IL_1D:
				if (id == EventId.TroopAbilityActivate)
				{
					this.AddTroopAbilitySelfBuff((SmartEntity)cookie);
					return EatResponse.NotEaten;
				}
				if (id == EventId.TroopAbilityDeactivate)
				{
					this.RemoveTroopAbilitySelfBuff((SmartEntity)cookie);
					return EatResponse.NotEaten;
				}
				if (id == EventId.BuildingPlacedOnBoard)
				{
					SmartEntity entity = cookie as SmartEntity;
					this.ApplyEntityWarBuffs(entity);
					this.ApplyBuildingEquipmentBuffs(entity);
					return EatResponse.NotEaten;
				}
				if (id == EventId.TroopPlacedOnBoard)
				{
					SmartEntity smartEntity = cookie as SmartEntity;
					this.AddBuffsOnSpawn(smartEntity);
					this.ApplyEntityWarBuffs(smartEntity);
					this.ApplyTroopEquipmentBuffs(smartEntity);
					return EatResponse.NotEaten;
				}
				if (id == EventId.SpecialAttackSpawned)
				{
					SpecialAttack specialAttack = cookie as SpecialAttack;
					this.ApplySpecialAttackWarBuffs(specialAttack);
					this.ApplySpecialAttackEquipmentBuffs(specialAttack);
					return EatResponse.NotEaten;
				}
				if (id == EventId.BattleLoadedForDefend)
				{
					this.ApplyEquipmentBuffsToExistingBuildings();
					this.RemoveAllEquipmentBuffShader(GameConstants.EQUIPMENT_SHADER_DELAY_DEFENSE);
					return EatResponse.NotEaten;
				}
				if (id != EventId.EquipmentBuffShaderRemove)
				{
					return EatResponse.NotEaten;
				}
				int timeDelayInMS = (int)cookie;
				this.RemoveAllEquipmentBuffShader(timeDelayInMS);
				return EatResponse.NotEaten;
			}
			case EventId.AddedBuff:
			{
				BuffEventData buffEventData = (BuffEventData)cookie;
				this.OnAddedBuff(buffEventData.BuffObj, buffEventData.Target);
				return EatResponse.NotEaten;
			}
			case EventId.RemovingBuff:
			{
				BuffEventData buffEventData = (BuffEventData)cookie;
				this.OnRemovingBuff(buffEventData.BuffObj, buffEventData.Target);
				return EatResponse.NotEaten;
			}
			}
			goto IL_1D;
		}

		public static string MakeFXAttachmentTag(string buffId)
		{
			return string.Format("BuffEffect_{0}", buffId);
		}

		private void AddEntityToRecheckVisualAfterLoadList(SmartEntity entity, Buff buff)
		{
			BuildingComponent buildingComp = entity.BuildingComp;
			TroopComponent troopComp = entity.TroopComp;
			BuffComponent buffComp = entity.BuffComp;
			if (buildingComp != null)
			{
				buffComp.RegisterForVisualReAddOnBuilding(buff);
			}
			else if (troopComp != null)
			{
				buffComp.RegisterForVisualReAddOnTroop(buff);
			}
		}

		private void OnAddedBuff(Buff buff, SmartEntity target)
		{
			bool flag = false;
			if (target.GameObjectViewComp == null || target.GameObjectViewComp.MainGameObject == null)
			{
				flag = true;
			}
			string vFXAssetForTarget = GameUtils.GetVFXAssetForTarget(buff.BuffType, target);
			if (!string.IsNullOrEmpty(vFXAssetForTarget))
			{
				SizeComponent sizeComp = target.SizeComp;
				if (sizeComp != null)
				{
					if (!flag)
					{
						int num = (sizeComp.Width > sizeComp.Depth) ? sizeComp.Depth : sizeComp.Width;
						string assetName = string.Format(vFXAssetForTarget, num);
						Vector3 offset = BuffController.DeduceBuffAssetOffset(buff.BuffType, target);
						bool pinToCenterOfMass = buff.BuffType.OffsetType == BuffAssetOffset.Top;
						Service.FXManager.CreateAndAttachFXToEntity(target, assetName, BuffController.MakeFXAttachmentTag(buff.BuffType.BuffID), null, pinToCenterOfMass, offset, true);
					}
					else
					{
						this.AddEntityToRecheckVisualAfterLoadList(target, buff);
					}
				}
			}
			if (!string.IsNullOrEmpty(buff.BuffType.ShaderName))
			{
				bool flag2 = false;
				if (this.shaderSwappedBuffs.ContainsKey(target))
				{
					Buff buff2 = this.shaderSwappedBuffs[target];
					if (buff2.VisualPriority <= buff.VisualPriority)
					{
						this.shaderSwappedBuffs[target] = buff;
						flag2 = true;
					}
				}
				else
				{
					this.shaderSwappedBuffs.Add(target, buff);
					flag2 = true;
				}
				if (!flag2)
				{
					return;
				}
				if (!flag)
				{
					Shader shader = Service.AssetManager.Shaders.GetShader(this.shaderSwappedBuffs[target].BuffType.ShaderName);
					if (shader != null)
					{
						this.SwapShadersForAppliedBuff(target, shader);
					}
				}
				else
				{
					this.AddEntityToRecheckVisualAfterLoadList(target, this.shaderSwappedBuffs[target]);
				}
			}
		}

		private void SwapShadersForAppliedBuff(SmartEntity entity, Shader shader)
		{
			if (entity == null || shader == null)
			{
				Service.Logger.Error("SwapShadersForAppliedBuff: Missing Entity or Shader");
				return;
			}
			if (entity.GameObjectViewComp == null)
			{
				Service.Logger.Error("SwapShadersForAppliedBuff: entity.GameObjectViewComp null");
				return;
			}
			if (entity.GameObjectViewComp.MainGameObject == null)
			{
				Service.Logger.Error("SwapShadersForAppliedBuff: MainGameObject null");
				return;
			}
			AssetMeshDataMonoBehaviour component = entity.GameObjectViewComp.MainGameObject.GetComponent<AssetMeshDataMonoBehaviour>();
			if (component == null)
			{
				Service.Logger.Error("SwapShadersForAppliedBuff: assetData null");
				return;
			}
			if (component.SelectableGameObjects == null)
			{
				Service.Logger.Error("SwapShadersForAppliedBuff: assetData.SelectableGameObjects null");
				return;
			}
			int count = component.SelectableGameObjects.Count;
			for (int i = 0; i < count; i++)
			{
				GameObject gameObject = component.SelectableGameObjects[i];
				if (gameObject == null)
				{
					Service.Logger.ErrorFormat("No game object found for buff application {0}, skipping", new object[]
					{
						i
					});
				}
				else
				{
					Renderer component2 = gameObject.GetComponent<Renderer>();
					if (!(component2 == null))
					{
						if (component2.gameObject == null)
						{
							Service.Logger.ErrorFormat("No game object found on renderer for {0}", new object[]
							{
								gameObject.name
							});
						}
						else
						{
							Material sharedMaterial = component2.sharedMaterial;
							Material material = UnityUtils.EnsureMaterialCopy(component2);
							if (material == null)
							{
								Service.Logger.ErrorFormat("No material found on renderer for {0}, skipping buff application", new object[]
								{
									gameObject.name
								});
							}
							else
							{
								if (!this.oldMaterials.ContainsValue(sharedMaterial))
								{
									this.shaderSwappedMaterials.Add(material);
									material.shader = shader;
								}
								else
								{
									string name = material.name;
									int j = 0;
									int count2 = this.shaderSwappedMaterials.Count;
									while (j < count2)
									{
										Material material2 = this.shaderSwappedMaterials[j];
										if (!(material2 == null))
										{
											if (name == material2.name)
											{
												component2.sharedMaterial = material2;
												break;
											}
										}
										j++;
									}
								}
								if (!this.oldMaterials.ContainsKey(component2.gameObject.GetInstanceID()))
								{
									this.oldMaterials.Add(component2.gameObject.GetInstanceID(), sharedMaterial);
								}
							}
						}
					}
				}
			}
		}

		private void RestoreShadersAfterBuff(SmartEntity entity)
		{
			if (entity == null)
			{
				Service.Logger.Error("RestoreShadersAfterBuff: Missing Entity");
				return;
			}
			if (entity.BuffComp == null)
			{
				return;
			}
			if (entity.GameObjectViewComp == null || entity.GameObjectViewComp.MainGameObject == null)
			{
				Service.Logger.Error("RestoreShadersAfterBuff: Has No Game Object");
				return;
			}
			AssetMeshDataMonoBehaviour component = entity.GameObjectViewComp.MainGameObject.GetComponent<AssetMeshDataMonoBehaviour>();
			if (component != null)
			{
				int count = component.SelectableGameObjects.Count;
				for (int i = 0; i < count; i++)
				{
					Renderer component2 = component.SelectableGameObjects[i].GetComponent<Renderer>();
					if (component2 != null)
					{
						int instanceID = component2.gameObject.GetInstanceID();
						if (this.oldMaterials.ContainsKey(instanceID))
						{
							component2.sharedMaterial = this.oldMaterials[instanceID];
						}
					}
				}
			}
		}

		private void OnRemovingBuff(Buff buff, SmartEntity target)
		{
			string vFXAssetForTarget = GameUtils.GetVFXAssetForTarget(buff.BuffType, target);
			if (!string.IsNullOrEmpty(vFXAssetForTarget) && target.SizeComp != null)
			{
				Service.FXManager.StopParticlesAndRemoveAttachedFXFromEntity(target, BuffController.MakeFXAttachmentTag(buff.BuffType.BuffID));
			}
			if (!string.IsNullOrEmpty(buff.BuffType.ShaderName))
			{
				this.RestoreShadersAfterBuff(target);
				if (this.shaderSwappedBuffs.ContainsKey(target))
				{
					this.shaderSwappedBuffs.Remove(target);
				}
			}
		}

		private void AddTroopAbilitySelfBuff(SmartEntity troop)
		{
			BuffTypeVO troopAbilitySelfBuffType = this.GetTroopAbilitySelfBuffType(troop);
			if (troopAbilitySelfBuffType != null)
			{
				this.TryAddBuffStack(troop, troopAbilitySelfBuffType, troop.TroopComp.TroopType.ArmorType, BuffVisualPriority.SelfAbility, troop);
			}
		}

		private void RemoveTroopAbilitySelfBuff(SmartEntity troop)
		{
			BuffTypeVO troopAbilitySelfBuffType = this.GetTroopAbilitySelfBuffType(troop);
			if (troopAbilitySelfBuffType != null)
			{
				this.RemoveBuffStack(troop, troopAbilitySelfBuffType);
			}
		}

		private static Vector3 DeduceBuffAssetOffset(BuffTypeVO buffType, SmartEntity target)
		{
			if (target.TroopComp != null)
			{
				BuffAssetOffset offsetType = buffType.OffsetType;
				if (offsetType == BuffAssetOffset.Top)
				{
					return target.TroopComp.TroopType.BuffAssetOffset;
				}
				if (offsetType == BuffAssetOffset.Base)
				{
					return target.TroopComp.TroopType.BuffAssetBaseOffset;
				}
			}
			if (target.BuildingComp != null)
			{
				BuffAssetOffset offsetType2 = buffType.OffsetType;
				if (offsetType2 == BuffAssetOffset.Top)
				{
					return target.BuildingComp.BuildingType.BuffAssetOffset;
				}
				if (offsetType2 == BuffAssetOffset.Base)
				{
					return target.BuildingComp.BuildingType.BuffAssetBaseOffset;
				}
			}
			return Vector3.zero;
		}

		private void ApplyBuffFromBullet(Bullet bullet)
		{
			SmartEntity target = bullet.Target;
			ProjectileTypeVO projectileType = bullet.ProjectileType;
			if (target == null || target.TeamComp == null || projectileType == null)
			{
				return;
			}
			if (target.HealthComp == null || target.HealthComp.IsDead())
			{
				return;
			}
			string[] applyBuffs = projectileType.ApplyBuffs;
			if (applyBuffs == null || applyBuffs.Length == 0)
			{
				return;
			}
			ArmorType armorType = GameUtils.DeduceArmorType(target);
			if (armorType == ArmorType.Invalid)
			{
				return;
			}
			bool flag = bullet.Owner == target;
			bool flag2 = !flag && bullet.OwnerTeam == target.TeamComp.TeamType;
			StaticDataController staticDataController = Service.StaticDataController;
			int i = 0;
			int num = applyBuffs.Length;
			while (i < num)
			{
				BuffTypeVO buffTypeVO = staticDataController.Get<BuffTypeVO>(applyBuffs[i]);
				if (flag)
				{
					if (buffTypeVO.ApplyToSelf)
					{
						goto IL_FC;
					}
				}
				else if (flag2)
				{
					if (buffTypeVO.ApplyToAllies)
					{
						goto IL_FC;
					}
				}
				else if (buffTypeVO.ApplyToEnemies)
				{
					goto IL_FC;
				}
				IL_10D:
				i++;
				continue;
				IL_FC:
				this.TryAddBuffStack(target, buffTypeVO, armorType, BuffVisualPriority.Default, bullet.Owner);
				goto IL_10D;
			}
		}

		private void AddBuffsOnSpawn(SmartEntity target)
		{
			if (target.TroopComp == null)
			{
				return;
			}
			string[] spawnApplyBuffs = target.TroopComp.TroopType.SpawnApplyBuffs;
			if (spawnApplyBuffs == null || spawnApplyBuffs.Length == 0)
			{
				return;
			}
			ArmorType armorType = GameUtils.DeduceArmorType(target);
			if (armorType == ArmorType.Invalid)
			{
				return;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			int i = 0;
			int num = spawnApplyBuffs.Length;
			while (i < num)
			{
				BuffTypeVO buffTypeVO = staticDataController.Get<BuffTypeVO>(spawnApplyBuffs[i]);
				if (buffTypeVO.ApplyToSelf)
				{
					this.TryAddBuffStack(target, buffTypeVO, armorType, BuffVisualPriority.Default, target);
				}
				i++;
			}
		}

		private BuffTypeVO GetTroopAbilitySelfBuffType(SmartEntity troop)
		{
			if (troop == null || troop.TroopComp == null)
			{
				return null;
			}
			TroopAbilityVO abilityVO = troop.TroopComp.AbilityVO;
			if (abilityVO == null)
			{
				return null;
			}
			string selfBuff = abilityVO.SelfBuff;
			if (string.IsNullOrEmpty(selfBuff))
			{
				return null;
			}
			return Service.StaticDataController.Get<BuffTypeVO>(selfBuff);
		}

		private void TryAddBuffStack(SmartEntity target, BuffTypeVO buffType, ArmorType armorType, BuffVisualPriority visualPriority, SmartEntity originator)
		{
			if (target == null || buffType == null || !buffType.WillAffect(armorType))
			{
				return;
			}
			BuffComponent buffComponent = target.BuffComp;
			if (buffComponent == null)
			{
				buffComponent = new BuffComponent();
				target.Add(buffComponent);
			}
			else if (buffComponent.IsBuffPrevented(buffType))
			{
				return;
			}
			buffComponent.AddBuffStack(buffType, armorType, visualPriority, originator);
			buffComponent.RemoveBuffsCanceledBy(buffType);
		}

		private void RemoveBuffStack(SmartEntity target, BuffTypeVO buffType)
		{
			BuffComponent buffComp = target.BuffComp;
			if (buffComp == null)
			{
				return;
			}
			buffComp.RemoveBuffStack(buffType);
		}

		public void AddAttackerWarBuff(WarBuffVO warBuff)
		{
			this.attackerWarBuffs.Add(warBuff);
		}

		public void AddDefenderWarBuff(WarBuffVO warBuff)
		{
			this.defenderWarBuffs.Add(warBuff);
		}

		public void ClearWarBuffs()
		{
			this.defenderWarBuffs.Clear();
			this.attackerWarBuffs.Clear();
		}

		public List<WarBuffVO> GetListOfWarBuffsBasedOnTeam(TeamType team)
		{
			if (team == TeamType.Attacker)
			{
				return this.attackerWarBuffs;
			}
			if (team == TeamType.Defender)
			{
				return this.defenderWarBuffs;
			}
			return null;
		}

		public void AddAttackerEquipmentBuff(EquipmentEffectVO effectVO)
		{
			this.attackerEquipmentBuffs.Add(effectVO);
		}

		public void AddDefenderEquipmentBuff(EquipmentEffectVO effectVO)
		{
			this.defenderEquipmentBuffs.Add(effectVO);
		}

		public void ClearEquipmentBuffs()
		{
			this.attackerEquipmentBuffs.Clear();
			this.defenderEquipmentBuffs.Clear();
		}

		public List<EquipmentEffectVO> GetEquipmentBuffsForTeam(TeamType team)
		{
			if (team == TeamType.Attacker)
			{
				return this.attackerEquipmentBuffs;
			}
			if (team == TeamType.Defender)
			{
				return this.defenderEquipmentBuffs;
			}
			return null;
		}

		private void ApplySpecialAttackWarBuffs(SpecialAttack specialAttack)
		{
			TeamType teamType = specialAttack.TeamType;
			List<WarBuffVO> listOfWarBuffsBasedOnTeam = this.GetListOfWarBuffsBasedOnTeam(teamType);
			int num = 0;
			if (listOfWarBuffsBasedOnTeam != null)
			{
				num = listOfWarBuffsBasedOnTeam.Count;
			}
			if (num == 0)
			{
				return;
			}
			for (int i = 0; i < num; i++)
			{
				WarBuffVO warBuffVO = listOfWarBuffsBasedOnTeam[i];
				if (warBuffVO.BuffType == SquadWarBuffType.SpecialAttackDamage)
				{
					this.AddSpecialAttackDamageBuff(specialAttack, warBuffVO);
				}
			}
		}

		private void ApplySpecialAttackEquipmentBuffs(SpecialAttack specialAttack)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			List<EquipmentEffectVO> equipmentBuffsForTeam = this.GetEquipmentBuffsForTeam(specialAttack.TeamType);
			int num = (equipmentBuffsForTeam == null) ? 0 : equipmentBuffsForTeam.Count;
			for (int i = 0; i < num; i++)
			{
				EquipmentEffectVO equipmentEffectVO = equipmentBuffsForTeam[i];
				if (equipmentEffectVO.BuffUids != null && equipmentEffectVO.AffectedSpecialAttackIds != null)
				{
					int j = 0;
					int num2 = equipmentEffectVO.AffectedSpecialAttackIds.Length;
					while (j < num2)
					{
						if (equipmentEffectVO.AffectedSpecialAttackIds[j] == specialAttack.VO.SpecialAttackID)
						{
							int k = 0;
							int num3 = equipmentEffectVO.BuffUids.Length;
							while (k < num3)
							{
								BuffTypeVO buffVO = staticDataController.Get<BuffTypeVO>(equipmentEffectVO.BuffUids[k]);
								specialAttack.AddAppliedBuff(buffVO, BuffVisualPriority.Equipment);
								k++;
							}
							break;
						}
						j++;
					}
				}
			}
		}

		private void ApplyTroopEquipmentBuffs(SmartEntity entity)
		{
			if (entity.TeamComp == null || entity.TroopComp == null)
			{
				return;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			List<EquipmentEffectVO> equipmentBuffsForTeam = this.GetEquipmentBuffsForTeam(entity.TeamComp.TeamType);
			int num = (equipmentBuffsForTeam == null) ? 0 : equipmentBuffsForTeam.Count;
			for (int i = 0; i < num; i++)
			{
				EquipmentEffectVO equipmentEffectVO = equipmentBuffsForTeam[i];
				if (equipmentEffectVO.BuffUids != null && equipmentEffectVO.AffectedTroopIds != null)
				{
					string troopID = entity.TroopComp.TroopType.TroopID;
					int j = 0;
					int num2 = equipmentEffectVO.AffectedTroopIds.Length;
					while (j < num2)
					{
						if (equipmentEffectVO.AffectedTroopIds[j] == troopID)
						{
							int k = 0;
							int num3 = equipmentEffectVO.BuffUids.Length;
							while (k < num3)
							{
								BuffTypeVO buffType = staticDataController.Get<BuffTypeVO>(equipmentEffectVO.BuffUids[k]);
								this.TryAddBuffStack(entity, buffType, GameUtils.DeduceArmorType(entity), BuffVisualPriority.Equipment, entity);
								k++;
							}
							break;
						}
						j++;
					}
				}
			}
		}

		private void ApplyBuildingEquipmentBuffs(SmartEntity entity)
		{
			if (entity.TeamComp == null || entity.BuildingComp == null)
			{
				return;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			List<EquipmentEffectVO> equipmentBuffsForTeam = this.GetEquipmentBuffsForTeam(entity.TeamComp.TeamType);
			int num = (equipmentBuffsForTeam == null) ? 0 : equipmentBuffsForTeam.Count;
			for (int i = 0; i < num; i++)
			{
				EquipmentEffectVO equipmentEffectVO = equipmentBuffsForTeam[i];
				if (equipmentEffectVO.BuffUids != null && equipmentEffectVO.AffectedBuildingIds != null)
				{
					string buildingID = entity.BuildingComp.BuildingType.BuildingID;
					int j = 0;
					int num2 = equipmentEffectVO.AffectedBuildingIds.Length;
					while (j < num2)
					{
						if (equipmentEffectVO.AffectedBuildingIds[j] == buildingID)
						{
							int k = 0;
							int num3 = equipmentEffectVO.BuffUids.Length;
							while (k < num3)
							{
								BuffTypeVO buffType = staticDataController.Get<BuffTypeVO>(equipmentEffectVO.BuffUids[k]);
								this.TryAddBuffStack(entity, buffType, GameUtils.DeduceArmorType(entity), BuffVisualPriority.Equipment, entity);
								k++;
							}
							break;
						}
						j++;
					}
				}
			}
		}

		private void RemoveAllEquipmentBuffShader(int timeDelayInMS)
		{
			if (this.defenderEquipmentBuffs.Count == 0)
			{
				return;
			}
			float delay = (float)timeDelayInMS / 1000f;
			Service.ViewTimerManager.CreateViewTimer(delay, false, new TimerDelegate(this.RemoveBuffShaders), null);
		}

		private void RemoveBuffShaders(uint id, object cookie)
		{
			NodeList<BuildingNode> nodeList = Service.EntityController.GetNodeList<BuildingNode>();
			for (BuildingNode buildingNode = nodeList.Head; buildingNode != null; buildingNode = buildingNode.Next)
			{
				SmartEntity smartEntity = (SmartEntity)buildingNode.Entity;
				BuffComponent buffComp = smartEntity.BuffComp;
				if (buffComp != null)
				{
					if (buffComp.Buffs.Count != 0)
					{
						this.RestoreShadersAfterBuff(smartEntity);
						if (this.shaderSwappedBuffs.ContainsKey(smartEntity))
						{
							this.shaderSwappedBuffs.Remove(smartEntity);
						}
					}
				}
			}
			this.oldMaterials.Clear();
			this.shaderSwappedMaterials.Clear();
			this.shaderSwappedBuffs.Clear();
		}

		private void ApplyEquipmentBuffsToExistingBuildings()
		{
			if (this.defenderEquipmentBuffs.Count == 0)
			{
				return;
			}
			NodeList<BuildingNode> nodeList = Service.EntityController.GetNodeList<BuildingNode>();
			for (BuildingNode buildingNode = nodeList.Head; buildingNode != null; buildingNode = buildingNode.Next)
			{
				this.ApplyBuildingEquipmentBuffs((SmartEntity)buildingNode.Entity);
			}
			Service.EventManager.SendEvent(EventId.ShaderResetOnEntity, null);
		}

		private void ApplyEntityWarBuffs(SmartEntity entity)
		{
			TeamComponent teamComp = entity.TeamComp;
			if (teamComp == null)
			{
				return;
			}
			TeamType teamType = teamComp.TeamType;
			List<WarBuffVO> listOfWarBuffsBasedOnTeam = this.GetListOfWarBuffsBasedOnTeam(teamType);
			int num = 0;
			if (listOfWarBuffsBasedOnTeam != null)
			{
				num = listOfWarBuffsBasedOnTeam.Count;
			}
			if (num == 0)
			{
				return;
			}
			for (int i = 0; i < num; i++)
			{
				WarBuffVO warBuffVO = listOfWarBuffsBasedOnTeam[i];
				switch (warBuffVO.BuffType)
				{
				case SquadWarBuffType.ShieldRegeneration:
					this.AddShieldRegenerationWarBuff(entity, warBuffVO);
					break;
				case SquadWarBuffType.HealthRegeneration:
					this.AddHealthRegenerationWarBuff(entity, warBuffVO);
					break;
				case SquadWarBuffType.MaxHealth:
					this.AddMaxHealthWarBuff(entity, warBuffVO);
					break;
				case SquadWarBuffType.VehicleDamage:
					this.AddVehicleDamageBuff(entity, warBuffVO);
					break;
				case SquadWarBuffType.InfantryDamage:
					this.AddInfantryDamageBuff(entity, warBuffVO);
					break;
				}
			}
		}

		private void AddSpecialAttackDamageBuff(SpecialAttack specialAttack, WarBuffVO warBuffVO)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			BuffTypeVO buffVO = staticDataController.Get<BuffTypeVO>(warBuffVO.TroopBuffUid);
			specialAttack.AddAppliedBuff(buffVO, BuffVisualPriority.SquadWars);
		}

		private void AddInfantryDamageBuff(SmartEntity entity, WarBuffVO warBuffVO)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			TroopComponent troopComp = entity.TroopComp;
			BuffTypeVO buffTypeVO = null;
			if (troopComp != null)
			{
				TroopTypeVO troopTypeVO = (TroopTypeVO)troopComp.TroopType;
				if (troopTypeVO.Type == TroopType.Infantry)
				{
					buffTypeVO = staticDataController.Get<BuffTypeVO>(warBuffVO.TroopBuffUid);
				}
			}
			if (buffTypeVO != null)
			{
				this.TryAddBuffStack(entity, buffTypeVO, ArmorType.Infantry, BuffVisualPriority.SquadWars, entity);
			}
		}

		private void AddVehicleDamageBuff(SmartEntity entity, WarBuffVO warBuffVO)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			TroopComponent troopComp = entity.TroopComp;
			bool flag = false;
			BuffTypeVO buffType = null;
			if (troopComp != null)
			{
				buffType = staticDataController.Get<BuffTypeVO>(warBuffVO.TroopBuffUid);
				TroopTypeVO troopTypeVO = (TroopTypeVO)troopComp.TroopType;
				flag = (troopTypeVO.Type == TroopType.Vehicle);
			}
			if (flag)
			{
				this.TryAddBuffStack(entity, buffType, ArmorType.Vehicle, BuffVisualPriority.SquadWars, entity);
			}
		}

		private void AddMaxHealthWarBuff(SmartEntity entity, WarBuffVO warBuffVO)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			BuildingComponent buildingComp = entity.BuildingComp;
			if (buildingComp == null)
			{
				return;
			}
			bool flag = buildingComp.BuildingType.Type == BuildingType.HQ || buildingComp.BuildingType.Type == BuildingType.Wall;
			HealthComponent healthComp = entity.HealthComp;
			if (flag && healthComp != null)
			{
				BuffTypeVO buffType = staticDataController.Get<BuffTypeVO>(warBuffVO.BuildingBuffUid);
				this.TryAddBuffStack(entity, buffType, GameUtils.DeduceArmorType(entity), BuffVisualPriority.SquadWars, entity);
			}
		}

		private void AddHealthRegenerationWarBuff(SmartEntity entity, WarBuffVO warBuffVO)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			TroopComponent troopComp = entity.TroopComp;
			bool flag = false;
			BuffTypeVO buffType = null;
			if (troopComp != null)
			{
				buffType = staticDataController.Get<BuffTypeVO>(warBuffVO.TroopBuffUid);
				flag = (troopComp.TroopType.Type == TroopType.Infantry);
			}
			if (flag)
			{
				this.TryAddBuffStack(entity, buffType, ArmorType.Infantry, BuffVisualPriority.SquadWars, entity);
			}
		}

		private void AddShieldRegenerationWarBuff(SmartEntity entity, WarBuffVO warBuffVO)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			BuildingComponent buildingComp = entity.BuildingComp;
			ShieldGeneratorComponent shieldGeneratorComp = entity.ShieldGeneratorComp;
			TroopComponent troopComp = entity.TroopComp;
			TroopShieldHealthComponent troopShieldHealthComp = entity.TroopShieldHealthComp;
			BuffTypeVO buffType;
			if (buildingComp != null && shieldGeneratorComp != null)
			{
				buffType = staticDataController.Get<BuffTypeVO>(warBuffVO.BuildingBuffUid);
			}
			else
			{
				if (troopComp == null || troopShieldHealthComp == null)
				{
					return;
				}
				buffType = staticDataController.Get<BuffTypeVO>(warBuffVO.TroopBuffUid);
			}
			this.TryAddBuffStack(entity, buffType, ArmorType.Shield, BuffVisualPriority.SquadWars, entity);
			entity.BuffComp.SetBuildingLoadedEvent(EventId.ShieldStarted, new VisualReadyDelegate(this.BuildingShieldStarted));
		}

		private bool BuildingShieldStarted(EventId id, object cookie, SmartEntity target)
		{
			if (id == EventId.ShieldStarted)
			{
				SmartEntity smartEntity = (SmartEntity)cookie;
				if (target != smartEntity)
				{
					return false;
				}
			}
			return true;
		}
	}
}
