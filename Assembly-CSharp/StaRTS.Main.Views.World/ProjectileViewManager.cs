using StaRTS.Assets;
using StaRTS.FX;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.World
{
	public class ProjectileViewManager : IViewFrameTimeObserver, IEventObserver
	{
		private const float DEFLECT_VARIANCE = 6f;

		private const float DEFLECT_TROOP_SIZE_ADJUST = 1.33f;

		private Stack<ProjectileView> projectilePool;

		private List<ProjectileView> activeProjectiles;

		private Dictionary<string, EmitterPool> emitters;

		private Dictionary<string, MeshPool> meshes;

		private Dictionary<string, AssetHandle> loadedAssetNames;

		private HashSet<IAssetVO> loadedAssets;

		private Dictionary<GameObject, SmartEntity> emitterGameObjectToEntity;

		private HashSet<SmartEntity> associatedEmitterEntitySet;

		private MutableIterator miter;

		private float speed;

		public const float BULLET_HEIGHT = 2f;

		public const int INITIAL_POOL_SIZE = 50;

		public const int INITIAL_FINISHED_POOL_SIZE = 10;

		private const float HIT_FX_POSITION_FUDGE = 0.2f;

		public ProjectileViewManager()
		{
			Service.ProjectileViewManager = this;
			this.projectilePool = new Stack<ProjectileView>(50);
			this.activeProjectiles = new List<ProjectileView>(50);
			this.emitters = new Dictionary<string, EmitterPool>();
			this.meshes = new Dictionary<string, MeshPool>();
			this.emitterGameObjectToEntity = new Dictionary<GameObject, SmartEntity>();
			this.associatedEmitterEntitySet = new HashSet<SmartEntity>();
			this.loadedAssetNames = new Dictionary<string, AssetHandle>();
			this.loadedAssets = new HashSet<IAssetVO>();
			this.miter = new MutableIterator();
			this.speed = 1f;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.ShooterWarmingUp, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.EntityHitByBeam, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.EntityKilled, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ShooterStoppedAttacking, EventPriority.Default);
		}

		public void SetSpeed(float speed)
		{
			this.speed = speed;
		}

		private void OnProjectileLoaded(object asset, object cookie)
		{
			string key = cookie as string;
			GameObject gameObject = asset as GameObject;
			GameObject gameObject2 = null;
			MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
			if (component != null)
			{
				gameObject2 = component.gameObject;
			}
			ParticleSystem particleSystem = gameObject.GetComponent<ParticleSystem>();
			if (gameObject2 == null && particleSystem == null)
			{
				IEnumerator enumerator = gameObject.transform.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Transform transform = (Transform)enumerator.Current;
						GameObject gameObject3 = transform.gameObject;
						ParticleSystem component2 = gameObject3.GetComponent<ParticleSystem>();
						if (component2 != null)
						{
							particleSystem = component2;
						}
						if (gameObject3.GetComponent<MeshRenderer>())
						{
							gameObject2 = gameObject3;
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
			if (particleSystem != null)
			{
				gameObject.SetActive(false);
				if (!this.emitters.ContainsKey(key))
				{
					this.emitters.Add(key, EmitterPool.CreateEmitterPool(particleSystem, new EmitterReturnedToPool(this.EmitterReturnedToPool)));
				}
			}
			if (gameObject2 != null && !this.meshes.ContainsKey(key))
			{
				this.meshes.Add(key, new MeshPool(gameObject2));
			}
		}

		private void EmitterReturnedToPool(object emitterObj)
		{
			if (emitterObj != null)
			{
				if (emitterObj is GameObject)
				{
					GameObject gameObject = (GameObject)emitterObj;
					if (gameObject != null)
					{
						Dictionary<GameObject, SmartEntity> dictionary = this.emitterGameObjectToEntity;
						SmartEntity smartEntity;
						if (dictionary.TryGetValue(gameObject, out smartEntity))
						{
							dictionary.Remove(gameObject);
							bool flag = false;
							foreach (SmartEntity current in dictionary.Values)
							{
								if (current == smartEntity)
								{
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								this.associatedEmitterEntitySet.Remove(smartEntity);
							}
							Service.DerivedTransformationManager.RemoveDerivedTransformation(gameObject);
						}
					}
					else
					{
						Service.Logger.Error("Emitter pool game object is being destroyed.");
					}
				}
			}
			else
			{
				Service.Logger.Error("Null emitterObject is being returned.");
			}
		}

		private void RemoveEmittersForEntity(SmartEntity smartEntity)
		{
			if (smartEntity != null && this.associatedEmitterEntitySet.Contains(smartEntity))
			{
				List<GameObject> list = new List<GameObject>();
				foreach (KeyValuePair<GameObject, SmartEntity> current in this.emitterGameObjectToEntity)
				{
					if (current.Value == smartEntity)
					{
						GameObject key = current.Key;
						if (key != null)
						{
							Service.DerivedTransformationManager.RemoveDerivedTransformation(key);
							MultipleEmittersPool.StopChildEmitters(key);
							list.Add(key);
						}
						else
						{
							Service.Logger.Error("ProjectileViewManager: Emitter objects are being destroyed");
						}
					}
				}
				int i = 0;
				int count = list.Count;
				while (i < count)
				{
					this.emitterGameObjectToEntity.Remove(list[i]);
					i++;
				}
				this.associatedEmitterEntitySet.Remove(smartEntity);
			}
		}

		private void AssociateEntityWithEmitterObject(SmartEntity smartEntity, GameObject emitterObject)
		{
			if (smartEntity != null && emitterObject != null)
			{
				this.emitterGameObjectToEntity.Add(emitterObject, smartEntity);
				this.associatedEmitterEntitySet.Add(smartEntity);
			}
		}

		public void LoadProjectileAssetsAndCreatePools(List<IAssetVO> assets)
		{
			if (assets == null || assets.Count == 0)
			{
				return;
			}
			HashSet<string> hashSet = null;
			int i = 0;
			int count = assets.Count;
			while (i < count)
			{
				IAssetVO assetVO = assets[i];
				if (this.loadedAssets.Add(assetVO))
				{
					List<string> assetNames = ProjectileUtils.GetAssetNames(assetVO);
					int j = 0;
					int count2 = assetNames.Count;
					while (j < count2)
					{
						string text = assetNames[j];
						if (!this.loadedAssetNames.ContainsKey(text))
						{
							if (hashSet == null)
							{
								hashSet = new HashSet<string>();
								hashSet.Add(text);
							}
							else if (!hashSet.Contains(text))
							{
								hashSet.Add(text);
							}
						}
						j++;
					}
				}
				i++;
			}
			if (hashSet != null)
			{
				List<string> list = new List<string>(hashSet);
				List<object> list2 = new List<object>();
				List<AssetHandle> list3 = new List<AssetHandle>();
				int k = 0;
				int count3 = list.Count;
				while (k < count3)
				{
					list2.Add(list[k]);
					list3.Add(AssetHandle.Invalid);
					k++;
				}
				Service.AssetManager.MultiLoad(list3, list, new AssetSuccessDelegate(this.OnProjectileLoaded), null, list2, null, null);
				int l = 0;
				int count4 = list.Count;
				while (l < count4)
				{
					this.loadedAssetNames.Add(list[l], list3[l]);
					l++;
				}
			}
		}

		public void UnloadProjectileAssetsAndPools()
		{
			AssetManager assetManager = Service.AssetManager;
			foreach (KeyValuePair<string, AssetHandle> current in this.loadedAssetNames)
			{
				assetManager.Unload(current.Value);
			}
			this.CleanupAssetsAndPools();
		}

		private void CleanupAssetsAndPools()
		{
			this.loadedAssets.Clear();
			this.loadedAssetNames.Clear();
			foreach (KeyValuePair<string, MeshPool> current in this.meshes)
			{
				current.Value.Destroy();
			}
			this.meshes.Clear();
			foreach (KeyValuePair<string, EmitterPool> current2 in this.emitters)
			{
				current2.Value.Destroy();
			}
			this.emitters.Clear();
			this.emitterGameObjectToEntity.Clear();
			this.associatedEmitterEntitySet.Clear();
		}

		private void LoadBulletAssets(Bullet bullet)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			List<IAssetVO> list = new List<IAssetVO>();
			ProjectileUtils.AddProjectileAssets(bullet.ProjectileType, list, staticDataController);
			if (bullet.AppliedBuffs != null)
			{
				int i = 0;
				int count = bullet.AppliedBuffs.Count;
				while (i < count)
				{
					list.Add(bullet.AppliedBuffs[i].BuffType);
					i++;
				}
			}
			this.LoadProjectileAssetsAndCreatePools(list);
		}

		private EmitterPool GetEmitter(string name)
		{
			if (name != null && this.emitters.ContainsKey(name))
			{
				return this.emitters[name];
			}
			return null;
		}

		public ProjectileView SpawnProjectile(Bullet bullet)
		{
			return this.SpawnProjectile(bullet, false);
		}

		private ProjectileView SpawnProjectile(Bullet bullet, bool isOnGround)
		{
			if (!ProjectileUtils.AreAllBulletAssetsLoaded(bullet, this.loadedAssets))
			{
				this.LoadBulletAssets(bullet);
			}
			bool isDeflection = bullet.IsDeflection;
			ProjectileTypeVO projectileType = bullet.ProjectileType;
			GameObject gunLocator = bullet.GunLocator;
			Vector3 vector;
			if (!isDeflection && gunLocator != null)
			{
				vector = gunLocator.transform.position;
			}
			else
			{
				vector = bullet.SpawnWorldLocation;
			}
			Vector3 targetWorldLocation = bullet.TargetWorldLocation;
			if (isOnGround)
			{
				vector.y = 0f;
				targetWorldLocation.y = 0f;
			}
			else if (projectileType.IsBeam || (bullet.Owner != null && bullet.Target != null && bullet.Target.ShieldBorderComp != null))
			{
				targetWorldLocation.y = vector.y;
			}
			ProjectileView projectileView = (this.projectilePool.Count <= 0) ? new ProjectileView() : this.projectilePool.Pop();
			projectileView.Init(projectileType, bullet, isOnGround);
			if (!isDeflection && !string.IsNullOrEmpty(projectileType.MuzzleFlashAssetName))
			{
				this.StartDirectionalEmitter(bullet.Owner, gunLocator, vector, targetWorldLocation, projectileType.MuzzleFlashAssetName, projectileType.MuzzleFlashFadeTime);
				int num = 0;
				if (bullet.AppliedBuffs != null)
				{
					num = bullet.AppliedBuffs.Count;
				}
				FactionType ownerFaction = bullet.OwnerFaction;
				for (int i = 0; i < num; i++)
				{
					string muzzleAssetNameBasedOnFaction = bullet.AppliedBuffs[i].BuffType.GetMuzzleAssetNameBasedOnFaction(ownerFaction);
					if (!string.IsNullOrEmpty(muzzleAssetNameBasedOnFaction))
					{
						this.StartDirectionalEmitter(bullet.Owner, gunLocator, vector, targetWorldLocation, muzzleAssetNameBasedOnFaction, projectileType.MuzzleFlashFadeTime);
					}
				}
			}
			GameObject gameObject = null;
			float num2 = bullet.TravelTime / 1000f;
			GameObject gameObject2 = null;
			ParticleSystem particleSystem = null;
			string bulletAssetName = projectileType.GetBulletAssetName(isOnGround);
			EmitterPool emitter = this.GetEmitter(bulletAssetName);
			if (emitter != null)
			{
				float num3 = num2;
				float delayPostEmitterStop = 0f;
				if (projectileType.IsBeam)
				{
					num3 = num2 * (float)(projectileType.BeamEmitterLength - projectileType.BeamInitialZeroes) / (float)(projectileType.BeamLifeLength - projectileType.BeamInitialZeroes);
					delayPostEmitterStop = num2 - num3;
				}
				if (emitter is MultipleEmittersPool)
				{
					MultipleEmittersPool multipleEmittersPool = (MultipleEmittersPool)emitter;
					gameObject = multipleEmittersPool.GetEmitterRoot();
					if (gameObject != null)
					{
						multipleEmittersPool.StopEmissionAndReturnToPool(gameObject, num3, delayPostEmitterStop);
					}
				}
				else if (emitter is SingleEmitterPool)
				{
					SingleEmitterPool singleEmitterPool = (SingleEmitterPool)emitter;
					particleSystem = singleEmitterPool.GetEmitter();
					if (particleSystem != null)
					{
						singleEmitterPool.StopEmissionAndReturnToPool(particleSystem, num3, delayPostEmitterStop);
					}
				}
				if (this.meshes.ContainsKey(bulletAssetName))
				{
					gameObject2 = this.meshes[bulletAssetName].GetMesh();
				}
			}
			Transform targetTransform = this.GetTargetTransform(bullet, vector, targetWorldLocation);
			if (gameObject2 != null && particleSystem != null)
			{
				projectileView.InitWithMeshAndEmitter(num2, vector, targetWorldLocation, gameObject2, particleSystem, targetTransform);
			}
			else if (gameObject2 != null && gameObject == null)
			{
				projectileView.InitWithMesh(num2, vector, targetWorldLocation, gameObject2, targetTransform);
			}
			else if (particleSystem != null)
			{
				projectileView.InitWithEmitter(num2, vector, targetWorldLocation, particleSystem, targetTransform);
			}
			else if (gameObject != null)
			{
				projectileView.InitWithEmitters(num2, vector, targetWorldLocation, gameObject, gameObject2, targetTransform);
			}
			else
			{
				projectileView.InitWithoutBullet(num2, vector, targetWorldLocation, targetTransform);
			}
			this.activeProjectiles.Add(projectileView);
			if (!isOnGround && !string.IsNullOrEmpty(projectileType.GroundBulletAssetName))
			{
				this.SpawnProjectile(bullet, true);
			}
			return projectileView;
		}

		private void SpawnChargeEmitter(SmartEntity owner)
		{
			ShooterComponent shooterComp = owner.ShooterComp;
			if (shooterComp == null)
			{
				return;
			}
			ProjectileTypeVO projectileType = shooterComp.ShooterVO.ProjectileType;
			if (string.IsNullOrEmpty(projectileType.ChargeAssetName))
			{
				return;
			}
			GameObjectViewComponent gameObjectViewComp = owner.GameObjectViewComp;
			if (gameObjectViewComp == null || gameObjectViewComp.GunLocators.Count == 0 || gameObjectViewComp.GunLocators[0].Count == 0)
			{
				return;
			}
			GameObject gameObject = gameObjectViewComp.GunLocators[0][0];
			if (gameObject == null)
			{
				return;
			}
			Vector3 position = gameObject.transform.position;
			this.StartDirectionalEmitter(owner, gameObject, position, Vector3.zero, projectileType.ChargeAssetName, projectileType.MuzzleFlashFadeTime);
		}

		private void StartDirectionalEmitter(SmartEntity owner, GameObject gunLocator, Vector3 startLocation, Vector3 endLocation, string assetName, float fadeTime)
		{
			EmitterPool emitter = this.GetEmitter(assetName);
			if (emitter != null)
			{
				if (emitter is MultipleEmittersPool)
				{
					MultipleEmittersPool multipleEmittersPool = (MultipleEmittersPool)emitter;
					GameObject emitterRoot = multipleEmittersPool.GetEmitterRoot();
					float num = 0f;
					if (emitterRoot != null)
					{
						ParticleSystem[] allEmitters = MultipleEmittersPool.GetAllEmitters(emitterRoot);
						Transform transform = emitterRoot.transform;
						transform.position = startLocation;
						if (gunLocator != null)
						{
							DerivedTransformationObject derivedTransformationObject = new DerivedTransformationObject(gunLocator, Vector3.zero, true);
							Service.DerivedTransformationManager.AddDerivedTransformation(emitterRoot, derivedTransformationObject);
							this.AssociateEntityWithEmitterObject(owner, emitterRoot);
						}
						else
						{
							GameObjectViewComponent gameObjectViewComponent = null;
							if (owner != null)
							{
								gameObjectViewComponent = owner.GameObjectViewComp;
							}
							if (gameObjectViewComponent == null)
							{
								transform.LookAt(endLocation);
							}
							else
							{
								transform.rotation = gameObjectViewComponent.MainTransform.rotation;
							}
						}
						emitterRoot.SetActive(true);
						ParticleSystem[] array = allEmitters;
						for (int i = 0; i < array.Length; i++)
						{
							ParticleSystem particleSystem = array[i];
							if (particleSystem != null)
							{
								if (particleSystem.main.duration > num)
								{
									num = particleSystem.main.duration;
								}
								this.PlayParticle(particleSystem);
							}
						}
						multipleEmittersPool.StopEmissionAndReturnToPool(emitterRoot, num, fadeTime);
					}
				}
				else if (emitter is SingleEmitterPool)
				{
					SingleEmitterPool singleEmitterPool = (SingleEmitterPool)emitter;
					ParticleSystem emitter2 = singleEmitterPool.GetEmitter();
					if (emitter2 != null)
					{
						this.PlayParticleAt(emitter2, startLocation);
						singleEmitterPool.StopEmissionAndReturnToPool(emitter2, emitter2.main.startLifetime.constant, fadeTime);
					}
				}
			}
		}

		private Transform GetTargetTransform(Bullet bullet, Vector3 startLocation, Vector3 endLocation)
		{
			Transform transform = null;
			if (bullet.ProjectileType.SeeksTarget && bullet.Target != null)
			{
				GameObjectViewComponent gameObjectViewComp = bullet.Target.GameObjectViewComp;
				if (gameObjectViewComp != null)
				{
					if (gameObjectViewComp.HitLocators.Count > 0)
					{
						Vector3 vector = startLocation - endLocation;
						int num = (int)(Mathf.Atan2(vector.z, vector.x) * 57.29578f);
						int num2 = 4 - num / 45;
						if (gameObjectViewComp.HitLocators != null && num2 < gameObjectViewComp.HitLocators.Count && gameObjectViewComp.HitLocators[num2] != null)
						{
							transform = gameObjectViewComp.HitLocators[num2].transform;
						}
					}
					else if (gameObjectViewComp.CenterOfMass != null)
					{
						transform = gameObjectViewComp.CenterOfMass.transform;
					}
					if (transform == null)
					{
						transform = gameObjectViewComp.MainTransform;
					}
				}
			}
			return transform;
		}

		private void ShowImpactFXForBeam(SmartEntity owner, SmartEntity target)
		{
			if (owner != null && target != null)
			{
				int i = 0;
				int count = this.activeProjectiles.Count;
				while (i < count)
				{
					ProjectileView projectileView = this.activeProjectiles[i];
					Bullet bullet = projectileView.Bullet;
					if (bullet.Owner == owner && bullet.Target == target)
					{
						if (!bullet.AppliedBeamHitFXThisSegment)
						{
							bullet.AppliedBeamHitFXThisSegment = true;
							GameObjectViewComponent gameObjectViewComp = target.GameObjectViewComp;
							if (gameObjectViewComp != null)
							{
								GameObject centerOfMass = gameObjectViewComp.CenterOfMass;
								Vector3 position;
								if (centerOfMass == null)
								{
									position = gameObjectViewComp.MainTransform.position;
									position.y = projectileView.TargetLocation.y;
								}
								else
								{
									position = centerOfMass.transform.position;
								}
								projectileView.SetTargetLocation(position);
								this.ShowImpactFXForBulletOwner(projectileView, owner);
							}
						}
						break;
					}
					i++;
				}
			}
		}

		private void ShowImpactFXForBulletOwner(ProjectileView view, SmartEntity owner)
		{
			this.ShowImpactFXForView(view);
			Bullet bullet = view.Bullet;
			List<Buff> appliedBuffs = bullet.AppliedBuffs;
			int num = (appliedBuffs == null) ? 0 : appliedBuffs.Count;
			FactionType ownerFaction = bullet.OwnerFaction;
			for (int i = 0; i < num; i++)
			{
				string impactAssetNameBasedOnFaction = bullet.AppliedBuffs[i].BuffType.GetImpactAssetNameBasedOnFaction(ownerFaction);
				if (!string.IsNullOrEmpty(impactAssetNameBasedOnFaction))
				{
					this.ShowImpactFXForView(view, impactAssetNameBasedOnFaction);
				}
			}
			Service.EventManager.SendEvent(EventId.ProjectileViewImpacted, owner);
		}

		private void ShowImpactFXForView(ProjectileView view)
		{
			this.ShowImpactFXForView(view, view.ProjectileType.HitSparkAssetName);
		}

		private void ShowImpactFXForView(ProjectileView view, string assetName)
		{
			EmitterPool emitter = this.GetEmitter(assetName);
			if (emitter == null)
			{
				return;
			}
			Vector3 vector = view.TargetLocation;
			if (view.TargetTracker != null)
			{
				vector = view.TargetTracker.position;
			}
			else if (view.MeshTracker != null)
			{
				vector = view.MeshTracker.position;
			}
			bool directional = view.ProjectileType.Directional;
			bool seeksTarget = view.ProjectileType.SeeksTarget;
			if (directional)
			{
				Vector3 a = view.StartLocation - vector;
				a.Normalize();
				vector += a * 0.2f;
			}
			if (emitter is MultipleEmittersPool)
			{
				MultipleEmittersPool multipleEmittersPool = (MultipleEmittersPool)emitter;
				GameObject emitterRoot = multipleEmittersPool.GetEmitterRoot();
				float num = 0f;
				if (emitterRoot != null)
				{
					emitterRoot.SetActive(true);
					ParticleSystem[] allEmitters = MultipleEmittersPool.GetAllEmitters(emitterRoot);
					Transform transform = emitterRoot.transform;
					transform.position = vector;
					if (directional)
					{
						transform.LookAt(view.StartLocation);
					}
					ParticleSystem[] array = allEmitters;
					for (int i = 0; i < array.Length; i++)
					{
						ParticleSystem particleSystem = array[i];
						if (particleSystem.main.duration > num)
						{
							num = particleSystem.main.duration;
						}
						this.PlayParticle(particleSystem);
					}
					if (seeksTarget && view.Bullet.Target != null && view.Bullet.Target.GameObjectViewComp != null && !GameUtils.IsEntityDead(view.Bullet.Target))
					{
						SmartEntity target = view.Bullet.Target;
						Vector3 positionOffset = emitterRoot.transform.position - target.GameObjectViewComp.MainTransform.position;
						DerivedTransformationObject derivedTransformationObject = new DerivedTransformationObject(target.GameObjectViewComp.MainGameObject, positionOffset, false);
						Service.DerivedTransformationManager.AddDerivedTransformation(emitterRoot, derivedTransformationObject);
						this.AssociateEntityWithEmitterObject(target, emitterRoot);
					}
					multipleEmittersPool.StopEmissionAndReturnToPool(emitterRoot, num, 0f);
				}
			}
			else if (emitter is SingleEmitterPool)
			{
				SingleEmitterPool singleEmitterPool = (SingleEmitterPool)emitter;
				ParticleSystem emitter2 = singleEmitterPool.GetEmitter();
				if (emitter2 != null)
				{
					if (directional)
					{
						this.PlayParticleLookingAt(emitter2, vector, view.StartLocation);
					}
					else
					{
						this.PlayParticleAt(emitter2, vector);
					}
					singleEmitterPool.StopEmissionAndReturnToPool(emitter2, emitter2.main.startLifetime.constant, 0f);
				}
			}
		}

		private void PlayParticle(ParticleSystem particle)
		{
			UnityUtils.PlayParticleEmitter(particle);
		}

		private void PlayParticleAt(ParticleSystem particle, Vector3 targetLocation)
		{
			particle.transform.position = targetLocation;
			this.PlayParticle(particle);
		}

		private void PlayParticleLookingAt(ParticleSystem particle, Vector3 targetLocation, Vector3 lookAt)
		{
			Transform transform = particle.transform;
			transform.position = targetLocation;
			transform.LookAt(lookAt);
			this.PlayParticle(particle);
		}

		private void ReturnProjectileViewToPool(ProjectileView view)
		{
			string bulletAssetName = view.ProjectileType.GetBulletAssetName(view.IsOnGround);
			if (view.Emitter != null)
			{
				EmitterPool emitter = this.GetEmitter(bulletAssetName);
				if (emitter != null && emitter is SingleEmitterPool)
				{
					SingleEmitterPool singleEmitterPool = (SingleEmitterPool)emitter;
					singleEmitterPool.StopEmissionAndReturnToPool(view.Emitter, 0f, 0f);
				}
			}
			if (view.MeshTracker != null && view.MeshTracker.gameObject != null && this.meshes.ContainsKey(bulletAssetName))
			{
				this.meshes[bulletAssetName].ReturnToPool(view.MeshTracker.gameObject);
			}
			view.OnReturnToPool();
			this.projectilePool.Push(view);
		}

		private BuffTypeVO ShouldDeflectProjectile(ProjectileView view, float distSquared)
		{
			SmartEntity target = view.Bullet.Target;
			if (target == null || target.TroopComp == null || !target.TroopComp.IsAbilityModeActive)
			{
				return null;
			}
			Bullet bullet = view.Bullet;
			if (bullet.IsDeflection)
			{
				return null;
			}
			if (!bullet.ProjectileType.IsDeflectable)
			{
				return null;
			}
			TroopComponent troopComp = target.TroopComp;
			ITroopDeployableVO troopType = troopComp.TroopType;
			float num = (float)(troopType.SizeX + troopType.SizeY) * 0.5f;
			num += 1.33f;
			float num2 = num * 0.5f * 3f;
			if (distSquared > num2 * num2)
			{
				return null;
			}
			string selfBuff = troopComp.AbilityVO.SelfBuff;
			if (string.IsNullOrEmpty(selfBuff))
			{
				return null;
			}
			return Service.StaticDataController.GetOptional<BuffTypeVO>(selfBuff);
		}

		private Vector3 ChooseDeflectionTarget(Vector3 v)
		{
			Rand rand = Service.Rand;
			v.x += rand.ViewRangeFloat(-6f, 6f);
			v.y += rand.ViewRangeFloat(-6f, 6f);
			v.z += rand.ViewRangeFloat(-6f, 6f);
			return v;
		}

		public void OnViewFrameTime(float dt)
		{
			this.miter.Init(this.activeProjectiles);
			while (this.miter.Active())
			{
				Vector3 vector = Vector3.zero;
				Vector3 vector2 = Vector3.zero;
				ProjectileView projectileView = this.activeProjectiles[this.miter.Index];
				float distSquared;
				bool flag = projectileView.Update(dt * this.speed, out distSquared);
				BuffTypeVO buffTypeVO = this.ShouldDeflectProjectile(projectileView, distSquared);
				bool flag2 = buffTypeVO != null && buffTypeVO.IsDeflector;
				if (flag || flag2)
				{
					Bullet bullet = projectileView.Bullet;
					if (flag2)
					{
						vector = projectileView.ViewPath.CurrentLocation;
						vector2 = this.ChooseDeflectionTarget(projectileView.StartLocation);
						Vector3 vector3 = Vector3.Normalize(vector2 - vector);
						vector3 *= (float)(bullet.ProjectileType.MaxSpeed * 3 * GameConstants.DEFLECTION_VELOCITY_PERCENT) * 0.01f;
						vector3 *= (float)GameConstants.DEFLECTION_DURATION_MS * 0.001f;
						vector2 = vector + vector3;
					}
					else if (!bullet.ProjectileType.IsBeam)
					{
						this.ShowImpactFXForBulletOwner(projectileView, bullet.Owner);
					}
					this.ReturnProjectileViewToPool(projectileView);
					this.activeProjectiles.RemoveAt(this.miter.Index);
					this.miter.OnRemove(this.miter.Index);
					if (flag2)
					{
						bullet.ChangeToDeflection((uint)GameConstants.DEFLECTION_DURATION_MS, vector, vector2);
						this.SpawnProjectile(bullet);
						Service.EventManager.SendEvent(EventId.ProjectileViewDeflected, buffTypeVO);
					}
				}
				this.miter.Next();
			}
			this.miter.Reset();
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.ShooterWarmingUp:
				this.SpawnChargeEmitter((SmartEntity)cookie);
				return EatResponse.NotEaten;
			case EventId.ShooterClipUsed:
			{
				IL_18:
				if (id == EventId.EntityKilled)
				{
					goto IL_5C;
				}
				if (id != EventId.EntityHitByBeam)
				{
					return EatResponse.NotEaten;
				}
				Bullet bullet = (Bullet)cookie;
				this.ShowImpactFXForBeam(bullet.Owner, bullet.Target);
				return EatResponse.NotEaten;
			}
			case EventId.ShooterStoppedAttacking:
				goto IL_5C;
			}
			goto IL_18;
			IL_5C:
			this.RemoveEmittersForEntity((SmartEntity)cookie);
			return EatResponse.NotEaten;
		}

		public void Destroy()
		{
			this.CleanupAssetsAndPools();
			this.projectilePool.Clear();
			this.activeProjectiles.Clear();
			this.projectilePool = null;
			this.activeProjectiles = null;
			this.loadedAssets = null;
			this.loadedAssetNames = null;
			this.meshes = null;
			this.emitters = null;
			this.emitterGameObjectToEntity = null;
			this.associatedEmitterEntitySet = null;
		}
	}
}
