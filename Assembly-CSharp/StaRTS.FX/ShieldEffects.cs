using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.FX
{
	public class ShieldEffects
	{
		private const string SHIELD_EMPIRE_FX_UID = "effect160";

		private const string SHIELD_REBEL_FX_UID = "effect161";

		private const string SPARK_EMPIRE_FX_UID = "effect39";

		private const string SPARK_REBEL_FX_UID = "effect166";

		private const string DESTROY_FX_UID = "fx_debris_6x6";

		private const string IMPACT_EMPIRE_FX_UID = "effect157";

		private const string IMPACT_REBEL_FX_UID = "effect165";

		private const string GENERATOR_EMPIRE_FX_UID = "effect159";

		private const string GENERATOR_REBEL_FX_UID = "effect164";

		private const string TOP_EMPIRE_FX_UID = "effect158";

		private const string TOP_REBEL_FX_UID = "effect167";

		private const string SHIELD_RENDERER_GO = "shieldMesh";

		private const string SHIELD_REBEL_CORE = "shield_beam_root";

		private const string SHIELD_DECAL_GO = "shieldGroundMesh";

		private const float SHIELD_VERTICAL_LIMIT = 5f;

		public const float SHIELD_WORLD_RADIUS_SCALE_FACTOR = 3f;

		private const float SHIELD_ASSET_SCALE_FACTOR = 1f;

		private const float SHIELD_CORE_ASSET_SCALE_FACTOR = 0.2425f;

		private const float SHIELD_CORE_VERTICAL_OFFSET = 3.8f;

		private const float SHIELD_VERTICAL_TAPER = 9f;

		private const string SHIELD_ATTACH = "shield";

		private const string SPARK_ATTACH = "spark";

		private const string DESTROY_ATTACH = "destroy";

		private const string DOME_ATTACH = "dome";

		private const string GENERATOR_ATTACH = "beam";

		private const string TOP_ATTACH = "top";

		private const float INITIAL_DELAY = 0.5f;

		private const float DELAY_INTERVAL = 0.65f;

		private const string IMPACT_EMPIRE_POOL_NAME = "ImpactEmpirePool";

		private const string IMPACT_REBEL_POOL_NAME = "ImpactRebelPool";

		private const string IMPACT_EMPIRE_GO = "shield_sparkle";

		private const string IMPACT_REBEL_GO = "shield_sparkle";

		private const float IMPACT_CLEANUP_DELAY = 1.2f;

		private Dictionary<Entity, uint> delayTimers;

		private Dictionary<FactionType, List<GameObject>> impactPool;

		private Dictionary<Entity, ShieldBuildingInfo> buildings;

		public ShieldEffects()
		{
			Service.ShieldEffects = this;
			this.delayTimers = new Dictionary<Entity, uint>();
			this.buildings = new Dictionary<Entity, ShieldBuildingInfo>();
			this.impactPool = new Dictionary<FactionType, List<GameObject>>();
			this.impactPool.Add(FactionType.Empire, new List<GameObject>());
			this.impactPool.Add(FactionType.Rebel, new List<GameObject>());
		}

		private FactionType GetFactionType(Entity building)
		{
			FactionType factionType = building.Get<BuildingComponent>().BuildingType.Faction;
			if (factionType != FactionType.Empire)
			{
				factionType = FactionType.Rebel;
			}
			return factionType;
		}

		private ShieldBuildingInfo LoadEffectsForBuilding(Entity building)
		{
			FactionType factionType = this.GetFactionType(building);
			List<string> list = new List<string>();
			if (factionType == FactionType.Empire)
			{
				list.Add("effect160");
				list.Add("effect39");
				list.Add("effect157");
				list.Add("effect159");
				list.Add("effect158");
			}
			else
			{
				list.Add("effect161");
				list.Add("effect166");
				list.Add("effect165");
				list.Add("effect164");
				list.Add("effect167");
			}
			list.Add("fx_debris_6x6");
			List<string> list2 = new List<string>();
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				list2.Add(this.GetAssetType(list[i]).AssetName);
				i++;
			}
			ShieldBuildingInfo shieldBuildingInfo = new ShieldBuildingInfo(building, list);
			List<object> list3 = new List<object>();
			List<AssetHandle> list4 = new List<AssetHandle>();
			int j = 0;
			int count2 = list.Count;
			while (j < count2)
			{
				list3.Add(new KeyValuePair<string, ShieldBuildingInfo>(list[j], shieldBuildingInfo));
				list4.Add(AssetHandle.Invalid);
				j++;
			}
			Service.AssetManager.MultiLoad(list4, list2, new AssetSuccessDelegate(this.OnEffectLoaded), null, list3, new AssetsCompleteDelegate(this.OnEffectsComplete), shieldBuildingInfo);
			shieldBuildingInfo.AssetHandles = list4;
			this.buildings.Add(building, shieldBuildingInfo);
			return shieldBuildingInfo;
		}

		private void OnEffectLoaded(object asset, object cookie)
		{
			GameObject value = asset as GameObject;
			KeyValuePair<string, ShieldBuildingInfo> keyValuePair = (KeyValuePair<string, ShieldBuildingInfo>)cookie;
			string key = keyValuePair.Key;
			ShieldBuildingInfo value2 = keyValuePair.Value;
			value2.EffectAssets[key] = value;
		}

		private void OnEffectsComplete(object cookie)
		{
			ShieldBuildingInfo shieldBuildingInfo = (ShieldBuildingInfo)cookie;
			shieldBuildingInfo.LoadComplete = true;
			Entity building = shieldBuildingInfo.Building;
			int i = 0;
			int count = shieldBuildingInfo.Reasons.Count;
			while (i < count)
			{
				ShieldReason shieldReason = shieldBuildingInfo.Reasons[i];
				switch (shieldReason.Reason)
				{
				case ShieldLoadReason.CreateEffect:
					this.CreateEffect(building);
					break;
				case ShieldLoadReason.ApplyHitEffect:
					this.ApplyHitEffect(building, (Vector3)shieldReason.Cookie);
					break;
				case ShieldLoadReason.UpdateShieldScale:
					this.UpdateShieldScale(building);
					break;
				case ShieldLoadReason.PlayEffect:
					this.PlayEffect(building, (bool)shieldReason.Cookie, 0f);
					break;
				case ShieldLoadReason.StopEffect:
					this.StopEffect(building);
					break;
				case ShieldLoadReason.PowerDownShieldEffect:
					this.PowerDownShieldEffect(building);
					break;
				case ShieldLoadReason.PlayDestructionEffect:
					this.PlayDestructionEffect(building);
					break;
				}
				i++;
			}
			shieldBuildingInfo.Reasons.Clear();
		}

		private ShieldBuildingInfo GetShieldBuildingInfo(Entity building, ShieldLoadReason reason, object cookie)
		{
			ShieldBuildingInfo shieldBuildingInfo;
			if (!this.buildings.ContainsKey(building))
			{
				if (reason == ShieldLoadReason.StopEffect)
				{
					return null;
				}
				shieldBuildingInfo = this.LoadEffectsForBuilding(building);
			}
			else
			{
				shieldBuildingInfo = this.buildings[building];
				if (shieldBuildingInfo.LoadComplete)
				{
					return shieldBuildingInfo;
				}
				if (reason == ShieldLoadReason.StopEffect)
				{
					this.CleanupShield(building);
					return null;
				}
			}
			if (reason != ShieldLoadReason.CreateEffect && shieldBuildingInfo.Reasons.Count == 0)
			{
				shieldBuildingInfo.Reasons.Add(new ShieldReason(ShieldLoadReason.CreateEffect, null));
			}
			shieldBuildingInfo.Reasons.Add(new ShieldReason(reason, cookie));
			return null;
		}

		private GameObject GetImpactFromPool(ShieldBuildingInfo info, FactionType faction)
		{
			List<GameObject> list = this.impactPool[faction];
			GameObject gameObject;
			if (list.Count != 0)
			{
				int index = list.Count - 1;
				gameObject = list[index];
				list.RemoveAt(index);
				return gameObject;
			}
			GameObject gameObject2;
			string name;
			string name2;
			if (faction == FactionType.Empire)
			{
				gameObject2 = info.EffectAssets["effect157"];
				name = "ImpactEmpirePool";
				name2 = "shield_sparkle";
			}
			else
			{
				gameObject2 = info.EffectAssets["effect165"];
				name = "ImpactRebelPool";
				name2 = "shield_sparkle";
			}
			if (gameObject2 == null)
			{
				return null;
			}
			GameObject gameObject3 = new GameObject(name);
			gameObject = UnityEngine.Object.Instantiate<GameObject>(gameObject2);
			gameObject.SetActive(false);
			gameObject.transform.parent = gameObject3.transform;
			return gameObject.transform.Find(name2).gameObject;
		}

		private void ReturnImpactToPool(FactionType faction, GameObject impact)
		{
			impact.transform.parent.gameObject.SetActive(false);
			this.impactPool[faction].Add(impact);
		}

		public void ApplyHitEffect(Entity building, Vector3 worldPosition)
		{
			GameObjectViewComponent gameObjectViewComponent = building.Get<GameObjectViewComponent>();
			if (gameObjectViewComponent == null)
			{
				return;
			}
			ShieldBuildingInfo shieldBuildingInfo = this.GetShieldBuildingInfo(building, ShieldLoadReason.ApplyHitEffect, worldPosition);
			if (shieldBuildingInfo == null)
			{
				return;
			}
			FactionType factionType = this.GetFactionType(building);
			GameObject gameObject = this.GetImpactFromPool(shieldBuildingInfo, factionType);
			if (gameObject == null)
			{
				return;
			}
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			gameObject = gameObject.transform.parent.gameObject;
			gameObject.transform.position = worldPosition;
			gameObject.transform.LookAt(gameObjectViewComponent.MainTransform);
			gameObject.SetActive(true);
			component.Play();
			ImpactCookie impactCookie = new ImpactCookie();
			impactCookie.ImpactGameObject = component.gameObject;
			impactCookie.ImpactFaction = factionType;
			Service.ViewTimerManager.CreateViewTimer(1.2f, false, new TimerDelegate(this.CleanUpHitEffect), impactCookie);
		}

		private void CleanUpHitEffect(uint id, object cookie)
		{
			ImpactCookie impactCookie = cookie as ImpactCookie;
			GameObject impactGameObject = impactCookie.ImpactGameObject;
			this.ReturnImpactToPool(impactCookie.ImpactFaction, impactGameObject);
		}

		private GameObject InstantiateEffect(ShieldBuildingInfo info, string effectUid)
		{
			GameObject gameObject = info.EffectAssets[effectUid];
			return (!(gameObject == null)) ? UnityEngine.Object.Instantiate<GameObject>(gameObject) : null;
		}

		private GameObject InstantiateShield(ShieldBuildingInfo info, FactionType faction)
		{
			string effectUid = (faction != FactionType.Empire) ? "effect161" : "effect160";
			return this.InstantiateEffect(info, effectUid);
		}

		private GameObject InstantiateGenerator(ShieldBuildingInfo info, FactionType faction)
		{
			string effectUid = (faction != FactionType.Empire) ? "effect164" : "effect159";
			return this.InstantiateEffect(info, effectUid);
		}

		private GameObject InstantiateTop(ShieldBuildingInfo info, FactionType faction)
		{
			string effectUid = (faction != FactionType.Empire) ? "effect167" : "effect158";
			return this.InstantiateEffect(info, effectUid);
		}

		private GameObject InstantiateSpark(ShieldBuildingInfo info, FactionType faction)
		{
			string effectUid = (faction != FactionType.Empire) ? "effect166" : "effect39";
			return this.InstantiateEffect(info, effectUid);
		}

		public void CreateEffect(Entity building)
		{
			SmartEntity smartEntity = (SmartEntity)building;
			if (smartEntity.ShieldGeneratorComp == null)
			{
				return;
			}
			ShieldBuildingInfo shieldBuildingInfo = this.GetShieldBuildingInfo(building, ShieldLoadReason.CreateEffect, null);
			if (shieldBuildingInfo == null)
			{
				return;
			}
			GameObjectViewComponent gameObjectViewComponent = building.Get<GameObjectViewComponent>();
			Vector3 position = gameObjectViewComponent.MainTransform.position;
			Vector3 zero = Vector3.zero;
			GameObject gameObject = null;
			FactionType factionType = this.GetFactionType(building);
			GameObject gameObject2 = this.InstantiateShield(shieldBuildingInfo, factionType);
			GameObject gameObject3 = this.InstantiateGenerator(shieldBuildingInfo, factionType);
			GameObject gameObject4 = this.InstantiateTop(shieldBuildingInfo, factionType);
			GameObject gameObject5 = this.InstantiateSpark(shieldBuildingInfo, factionType);
			if (gameObject2 != null)
			{
				Vector3 position2 = position;
				position2.y = 0f;
				gameObject2.transform.position = position2;
				gameObject = gameObject2.transform.FindChild("shieldGroundMesh").gameObject;
			}
			if (gameObject3 != null)
			{
				gameObject3.SetActive(false);
			}
			if (gameObject4 != null)
			{
				gameObject4.SetActive(false);
			}
			if (gameObject5 != null)
			{
				gameObject5.transform.position = position;
				gameObject5.transform.Rotate(Vector3.up, 270f);
				gameObject5.SetActive(false);
			}
			GameObject gameObject6 = this.InstantiateEffect(shieldBuildingInfo, "fx_debris_6x6");
			gameObject6.transform.position = position;
			gameObject6.SetActive(false);
			Transform transform = gameObject2.transform.Find("shieldMesh");
			GameObject gameObject7 = transform.gameObject;
			Material shieldMaterial = UnityUtils.EnsureMaterialCopy(gameObject7.GetComponent<Renderer>());
			shieldBuildingInfo.Shield = transform.gameObject;
			shieldBuildingInfo.Spark = gameObject5.transform.GetChild(0).GetComponent<ParticleSystem>();
			shieldBuildingInfo.Destruction = gameObject6.transform.GetChild(0).GetComponent<ParticleSystem>();
			shieldBuildingInfo.Generator = gameObject3;
			shieldBuildingInfo.Top = gameObject4;
			shieldBuildingInfo.DecalMaterial = UnityUtils.EnsureMaterialCopy(gameObject.GetComponent<Renderer>());
			shieldBuildingInfo.ShieldMaterial = shieldMaterial;
			gameObject2.SetActive(false);
			gameObjectViewComponent.AttachGameObject("shield", gameObject2, Vector3.zero, true, false);
			gameObjectViewComponent.AttachGameObject("spark", gameObject5, Vector3.zero, true, false);
			gameObjectViewComponent.AttachGameObject("destroy", gameObject6, Vector3.zero, true, false);
			gameObjectViewComponent.AttachGameObject("beam", gameObject3, Vector3.zero, true, false);
			gameObjectViewComponent.AttachGameObject("top", gameObject4, zero, false, false);
			Transform transform2 = gameObject2.transform.Find("shieldMesh");
			gameObjectViewComponent.AddSO("dome", transform2.gameObject);
			this.UpdateShieldScale(building);
		}

		public void UpdateShieldScale(Entity building)
		{
			ShieldBuildingInfo shieldBuildingInfo = this.GetShieldBuildingInfo(building, ShieldLoadReason.UpdateShieldScale, null);
			if (shieldBuildingInfo == null)
			{
				return;
			}
			if (!building.Has<ShieldGeneratorComponent>())
			{
				return;
			}
			GameObject shield = shieldBuildingInfo.Shield;
			int currentRadius = building.Get<ShieldGeneratorComponent>().CurrentRadius;
			float num = 1f;
			if ((float)currentRadius > 5f)
			{
				num = 1f - ((float)currentRadius - 5f) / 9f;
			}
			float x = 1f * (float)currentRadius;
			float z = 1f * (float)currentRadius;
			float num2 = 1f * ((float)currentRadius * num);
			shield.transform.parent.localScale = new Vector3(x, num2, z);
			GameObject top = shieldBuildingInfo.Top;
			GameObjectViewComponent gameObjectViewComponent = building.Get<GameObjectViewComponent>();
			Vector3 position = gameObjectViewComponent.MainTransform.position;
			Vector3 b = new Vector3(0f, num2 * 3f, 0f);
			top.transform.position = position + b;
			GameObject generator = shieldBuildingInfo.Generator;
			generator.transform.position = position + new Vector3(0f, 3.8f, 0f);
			FactionType factionType = this.GetFactionType(building);
			if (factionType != FactionType.Empire)
			{
				float num3 = (float)currentRadius * num * 0.2425f;
				GameObject gameObject = generator.transform.Find("shield_beam_root").gameObject;
				gameObject.transform.localScale = new Vector3(num3, num3, num3);
				top.transform.localScale = new Vector3(num3, num3, num3);
				Rand rand = Service.Rand;
				generator.transform.eulerAngles = new Vector3(0f, rand.ViewRangeFloat(-30f, 30f), 0f);
				top.transform.rotation = generator.transform.rotation;
				shield.transform.eulerAngles = new Vector3(0f, rand.ViewRangeFloat(60f, 120f), 0f);
			}
		}

		private void PlayDelayedEffect(uint timerId, object cookie)
		{
			Entity entity = (Entity)cookie;
			if (entity != null && ((SmartEntity)entity).ShieldGeneratorComp != null)
			{
				this.PlayEffect(entity, false, 0f);
			}
			this.delayTimers.Remove(entity);
		}

		private void PlayEffect(Entity building, bool idle, float delay)
		{
			if (delay > 0f)
			{
				if (!this.delayTimers.ContainsKey(building))
				{
					uint value = Service.ViewTimerManager.CreateViewTimer(delay, false, new TimerDelegate(this.PlayDelayedEffect), building);
					this.delayTimers.Add(building, value);
				}
				return;
			}
			ShieldBuildingInfo shieldBuildingInfo = this.GetShieldBuildingInfo(building, ShieldLoadReason.PlayEffect, idle);
			if (shieldBuildingInfo == null)
			{
				return;
			}
			if (shieldBuildingInfo.Generator != null)
			{
				shieldBuildingInfo.Generator.SetActive(true);
			}
			if (shieldBuildingInfo.Top != null)
			{
				shieldBuildingInfo.Top.SetActive(true);
			}
			if (shieldBuildingInfo.Shield != null)
			{
				shieldBuildingInfo.Shield.transform.parent.gameObject.SetActive(true);
			}
			shieldBuildingInfo.PlayShieldDisolveEffect(true, new DissolveCompleteDelegate(this.DestroyDissolver));
			if (shieldBuildingInfo.DecalMaterial != null)
			{
				new ShieldDecal(shieldBuildingInfo.DecalMaterial, true);
			}
			if (!idle && shieldBuildingInfo.Spark != null)
			{
				ParticleSystem spark = shieldBuildingInfo.Spark;
				spark.transform.root.gameObject.SetActive(true);
				spark.Play();
			}
			if (!idle)
			{
				Service.EventManager.SendEvent(EventId.ShieldStarted, building);
			}
		}

		public void PlayAllEffects(bool idle)
		{
			float num = (!idle) ? 0.5f : 0f;
			float num2 = (!idle) ? 0.65f : 0f;
			int num3 = 0;
			NodeList<ShieldGeneratorNode> shieldGeneratorNodeList = Service.BuildingLookupController.ShieldGeneratorNodeList;
			for (ShieldGeneratorNode shieldGeneratorNode = shieldGeneratorNodeList.Head; shieldGeneratorNode != null; shieldGeneratorNode = shieldGeneratorNode.Next)
			{
				if (!Service.BaseLayoutToolController.IsBuildingStashed(shieldGeneratorNode.Entity))
				{
					float delay = num + (float)num3 * num2;
					this.PlayEffect(shieldGeneratorNode.Entity, idle, delay);
					num3++;
				}
			}
		}

		public void StopEffect(Entity building)
		{
			ShieldBuildingInfo shieldBuildingInfo = this.GetShieldBuildingInfo(building, ShieldLoadReason.StopEffect, null);
			if (shieldBuildingInfo == null)
			{
				return;
			}
			shieldBuildingInfo.PlayShieldDisolveEffect(false, new DissolveCompleteDelegate(this.PowerDownShieldEffectComplete));
		}

		public void PowerDownShieldEffect(Entity building)
		{
			ShieldBuildingInfo shieldBuildingInfo = this.GetShieldBuildingInfo(building, ShieldLoadReason.PowerDownShieldEffect, null);
			if (shieldBuildingInfo == null)
			{
				return;
			}
			shieldBuildingInfo.PlayShieldDisolveEffect(true, new DissolveCompleteDelegate(this.PowerDownShieldEffectComplete));
		}

		private void PowerDownShieldEffectComplete(GameObject shield, bool death, ShieldBuildingInfo info)
		{
			this.InternalPowerDown(info);
		}

		private void InternalPowerDown(ShieldBuildingInfo info)
		{
			if (info.Spark != null)
			{
				info.Spark.transform.root.gameObject.SetActive(false);
			}
			if (info.Generator != null)
			{
				info.Generator.SetActive(false);
			}
			if (info.Top != null)
			{
				info.Top.SetActive(false);
			}
			if (info.Shield != null)
			{
				info.Shield.transform.parent.gameObject.SetActive(false);
			}
		}

		private void DestroyDissolver(GameObject shield, bool death, ShieldBuildingInfo info)
		{
			if (death && shield != null)
			{
				shield.transform.parent.gameObject.SetActive(false);
			}
		}

		public void PlayDestructionEffect(Entity building)
		{
			ShieldBuildingInfo shieldBuildingInfo = this.GetShieldBuildingInfo(building, ShieldLoadReason.PlayDestructionEffect, null);
			if (shieldBuildingInfo == null)
			{
				return;
			}
			shieldBuildingInfo.PlayShieldDisolveEffect(false, new DissolveCompleteDelegate(this.PowerDownShieldEffectComplete));
			if (shieldBuildingInfo.DecalMaterial != null)
			{
				new ShieldDecal(shieldBuildingInfo.DecalMaterial, false);
			}
		}

		public void CleanupShield(Entity building)
		{
			if (!this.buildings.ContainsKey(building))
			{
				return;
			}
			ShieldBuildingInfo shieldBuildingInfo = this.buildings[building];
			GameObjectViewComponent gameObjectViewComponent = building.Get<GameObjectViewComponent>();
			if (shieldBuildingInfo.Shield != null)
			{
				GameObject shield = shieldBuildingInfo.Shield;
				UnityEngine.Object.Destroy(shield.transform.root.gameObject);
				shieldBuildingInfo.Shield = null;
				gameObjectViewComponent.DetachGameObject("shield");
			}
			if (shieldBuildingInfo.Spark != null)
			{
				ParticleSystem spark = shieldBuildingInfo.Spark;
				UnityEngine.Object.Destroy(spark.transform.root.gameObject);
				shieldBuildingInfo.Spark = null;
				gameObjectViewComponent.DetachGameObject("spark");
			}
			if (shieldBuildingInfo.Destruction != null)
			{
				ParticleSystem destruction = shieldBuildingInfo.Destruction;
				UnityEngine.Object.Destroy(destruction.transform.root.gameObject);
				shieldBuildingInfo.Destruction = null;
				gameObjectViewComponent.DetachGameObject("destroy");
			}
			if (shieldBuildingInfo.Generator != null)
			{
				GameObject generator = shieldBuildingInfo.Generator;
				UnityEngine.Object.Destroy(generator.transform.root.gameObject);
				shieldBuildingInfo.Generator = null;
				gameObjectViewComponent.DetachGameObject("beam");
			}
			if (shieldBuildingInfo.Top != null)
			{
				GameObject top = shieldBuildingInfo.Top;
				UnityEngine.Object.Destroy(top.transform.root.gameObject);
				shieldBuildingInfo.Top = null;
				gameObjectViewComponent.DetachGameObject("top");
			}
			if (shieldBuildingInfo.DecalMaterial != null)
			{
				UnityUtils.DestroyMaterial(shieldBuildingInfo.DecalMaterial);
				shieldBuildingInfo.DecalMaterial = null;
			}
			if (shieldBuildingInfo.ShieldMaterial != null)
			{
				UnityUtils.DestroyMaterial(shieldBuildingInfo.ShieldMaterial);
				shieldBuildingInfo.ShieldMaterial = null;
			}
			if (this.delayTimers.ContainsKey(building))
			{
				uint id = this.delayTimers[building];
				Service.ViewTimerManager.KillViewTimer(id);
				this.delayTimers.Remove(building);
			}
			int i = 0;
			int count = shieldBuildingInfo.AssetHandles.Count;
			while (i < count)
			{
				Service.AssetManager.Unload(shieldBuildingInfo.AssetHandles[i]);
				i++;
			}
			shieldBuildingInfo.AssetHandles = null;
			this.buildings.Remove(building);
		}

		public void Cleanup()
		{
			foreach (uint current in this.delayTimers.Values)
			{
				Service.ViewTimerManager.KillViewTimer(current);
			}
			this.delayTimers.Clear();
			foreach (ShieldBuildingInfo current2 in this.buildings.Values)
			{
				if (current2.Shield != null)
				{
					UnityEngine.Object.Destroy(current2.Shield.transform.root.gameObject);
					current2.Shield = null;
				}
				if (current2.Spark != null)
				{
					UnityEngine.Object.Destroy(current2.Spark.transform.root.gameObject);
					current2.Spark = null;
				}
				if (current2.Destruction != null)
				{
					UnityEngine.Object.Destroy(current2.Destruction.transform.root.gameObject);
					current2.Destruction = null;
				}
				if (current2.Generator != null)
				{
					UnityEngine.Object.Destroy(current2.Generator.transform.root.gameObject);
					current2.Generator = null;
				}
				if (current2.Top != null)
				{
					UnityEngine.Object.Destroy(current2.Top.transform.root.gameObject);
					current2.Top = null;
				}
				if (current2.DecalMaterial != null)
				{
					UnityUtils.DestroyMaterial(current2.DecalMaterial);
					current2.DecalMaterial = null;
				}
				if (current2.ShieldMaterial != null)
				{
					UnityUtils.DestroyMaterial(current2.ShieldMaterial);
					current2.ShieldMaterial = null;
				}
				if (current2.ShieldDisolveEffect != null)
				{
					current2.ShieldDisolveEffect.Cleanup();
					current2.ShieldDisolveEffect = null;
				}
				int i = 0;
				int count = current2.AssetHandles.Count;
				while (i < count)
				{
					Service.AssetManager.Unload(current2.AssetHandles[i]);
					i++;
				}
				current2.AssetHandles = null;
			}
			this.buildings.Clear();
		}

		public List<IAssetVO> GetEffectAssetTypes()
		{
			return new List<IAssetVO>
			{
				this.GetAssetType("effect160"),
				this.GetAssetType("effect161"),
				this.GetAssetType("effect39"),
				this.GetAssetType("effect166"),
				this.GetAssetType("effect157"),
				this.GetAssetType("effect165"),
				this.GetAssetType("effect159"),
				this.GetAssetType("effect164"),
				this.GetAssetType("effect158"),
				this.GetAssetType("effect167"),
				this.GetAssetType("fx_debris_6x6")
			};
		}

		private IAssetVO GetAssetType(string uid)
		{
			return Service.StaticDataController.Get<EffectsTypeVO>(uid);
		}
	}
}
