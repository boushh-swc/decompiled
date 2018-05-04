using StaRTS.Assets;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.FX
{
	public class LightSaberHitEffect
	{
		private const string HIT_FX_RBL_ID = "effect197";

		private const string HIT_FX_EMP_ID = "effect199";

		private const string ADJ_HIT_FX_ID = "effect198";

		private const string PARAM_FX_OBJECT = "fxObject";

		private const string PARAM_BUILDING_INDEX = "index";

		private const float HIT_POS_OFFSET = 1f;

		private const float HIT_ROT_OFFSET = 40f;

		private const float FLASH_FX_DELAY = 1f;

		private const float FLASH_FX_DURATION = 0.3f;

		private const float FX_HIT_DESTROY_DELAY = 1f;

		private const float FX_SWIRL_DESTROY_DELAY = 3f;

		private const float LARGE_VALUE = 10000f;

		private Transform heroTrans;

		private GameObject circularFx;

		private GameObject hitFx;

		private GameObject adjHitFx;

		private List<GameObject> targetLocators;

		private List<GameObject> adjescentLocators;

		private List<GameObject> fxInstances;

		private List<SmartEntity> targetBuildings;

		private EntityFlasher entityFlasher;

		private AssetHandle circularHitFxHandle;

		private AssetHandle hitFxHandle;

		private AssetHandle adjHitFxHandle;

		public LightSaberHitEffect(int radius, int gridX, int gridZ, Transform heroTrans, string centerFx, FactionType faction)
		{
			this.targetLocators = new List<GameObject>();
			this.adjescentLocators = new List<GameObject>();
			this.fxInstances = new List<GameObject>();
			this.targetBuildings = new List<SmartEntity>();
			this.entityFlasher = new EntityFlasher();
			this.heroTrans = heroTrans;
			if (heroTrans != null)
			{
				TargetingController.TraverseSpiralToFindTargets(radius, gridX, gridZ, new TargetingController.TargetValidator(this.FindClosest), null, false);
				StaticDataController staticDataController = Service.StaticDataController;
				string text = null;
				string cookie = null;
				EffectsTypeVO effectsTypeVO;
				if (faction == FactionType.Empire)
				{
					effectsTypeVO = staticDataController.Get<EffectsTypeVO>("effect199");
				}
				else
				{
					effectsTypeVO = staticDataController.Get<EffectsTypeVO>("effect197");
				}
				if (effectsTypeVO != null)
				{
					text = effectsTypeVO.AssetName;
				}
				effectsTypeVO = staticDataController.Get<EffectsTypeVO>("effect198");
				if (effectsTypeVO != null)
				{
					cookie = effectsTypeVO.AssetName;
				}
				if (centerFx != null)
				{
					Service.AssetManager.Load(ref this.circularHitFxHandle, centerFx, new AssetSuccessDelegate(this.OnLoadCircularFx), null, null);
				}
				if (this.targetLocators.Count > 0)
				{
					if (text != null)
					{
						Service.AssetManager.Load(ref this.hitFxHandle, text, new AssetSuccessDelegate(this.OnLoadHitFx), null, cookie);
					}
					else
					{
						Service.Logger.Error("LightSaber Hit Effect not found");
					}
				}
			}
		}

		public void StopFxAndDestroy()
		{
			if (this.circularFx != null)
			{
				ParticleSystem[] componentsInChildren = this.circularFx.GetComponentsInChildren<ParticleSystem>(true);
				ParticleSystem[] array = componentsInChildren;
				for (int i = 0; i < array.Length; i++)
				{
					ParticleSystem particleSystem = array[i];
					particleSystem.Stop(true);
				}
			}
			int j = 0;
			int count = this.fxInstances.Count;
			while (j < count)
			{
				this.StopPlayFx(this.fxInstances[j], false);
				j++;
			}
			this.entityFlasher.RemoveFlashingFromAllEntities();
			this.targetBuildings = null;
			Service.ViewTimerManager.CreateViewTimer(1f, false, new TimerDelegate(this.OnDestroyHitFxTimer), null);
			Service.ViewTimerManager.CreateViewTimer(3f, false, new TimerDelegate(this.OnDestroySwirlFxTimer), null);
		}

		private void OnLoadCircularFx(object asset, object cookie)
		{
			this.circularFx = (GameObject)asset;
			this.circularFx = UnityEngine.Object.Instantiate<GameObject>(this.circularFx);
			this.AttachCenterEffect();
		}

		private void OnLoadHitFx(object asset, object cookie)
		{
			this.hitFx = (GameObject)asset;
			string assetName = (string)cookie;
			Service.AssetManager.Load(ref this.adjHitFxHandle, assetName, new AssetSuccessDelegate(this.OnLoadAdjHitFx), null, null);
		}

		private void OnLoadAdjHitFx(object asset, object cookie)
		{
			this.adjHitFx = (GameObject)asset;
			GameObject value = null;
			GameObject cookie2 = null;
			GameObject cookie3 = null;
			float num = 1f / (float)this.targetLocators.Count;
			int i = 0;
			int count = this.targetLocators.Count;
			while (i < count)
			{
				if (this.hitFx != null && this.targetLocators[i] != null)
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					dictionary["fxObject"] = value;
					dictionary["index"] = i;
					value = this.InstantiateAndPlace(this.hitFx, this.targetLocators[i], i);
					Service.ViewTimerManager.CreateViewTimer(num * (float)i, false, new TimerDelegate(this.OnPlayMainFxTimer), dictionary);
				}
				if (this.adjHitFx != null)
				{
					int num2 = i + i;
					if (num2 < this.adjescentLocators.Count && this.adjescentLocators[num2] != null)
					{
						cookie2 = this.InstantiateAndPlace(this.adjHitFx, this.adjescentLocators[num2], i);
					}
					num2 = i + i + 1;
					if (num2 < this.adjescentLocators.Count && this.adjescentLocators[num2] != null)
					{
						cookie3 = this.InstantiateAndPlace(this.adjHitFx, this.adjescentLocators[num2], i);
					}
					Service.ViewTimerManager.CreateViewTimer(num * (float)i, false, new TimerDelegate(this.OnPlayFxTimer), cookie2);
					Service.ViewTimerManager.CreateViewTimer(num * (float)i, false, new TimerDelegate(this.OnPlayFxTimer), cookie3);
				}
				i++;
			}
		}

		private void AttachCenterEffect()
		{
			if (this.heroTrans != null && this.circularFx != null)
			{
				Transform transform = this.circularFx.transform;
				transform.parent = this.heroTrans;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
			}
		}

		private GameObject InstantiateAndPlace(GameObject hitFx, GameObject target, int index)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(hitFx);
			Transform transform = gameObject.transform;
			Transform transform2 = target.transform;
			gameObject.name = hitFx.name + index;
			this.fxInstances.Add(gameObject);
			transform.parent = transform2;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.Translate(Vector3.forward * 1f);
			Vector3 eulerAngles = transform.localRotation.eulerAngles;
			transform.localRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y - 40f, eulerAngles.z);
			return gameObject;
		}

		private void OnPlayMainFxTimer(uint id, object cookie)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)cookie;
			GameObject effect = (GameObject)dictionary["fxObject"];
			int num = (int)dictionary["index"];
			this.StopPlayFx(effect, true);
			if (this.targetBuildings != null && this.targetBuildings.Count > num)
			{
				SmartEntity entity = this.targetBuildings[num];
				this.entityFlasher.AddFlashing(entity, 0.3f, 1f);
			}
		}

		private void OnPlayFxTimer(uint id, object cookie)
		{
			GameObject effect = (GameObject)cookie;
			this.StopPlayFx(effect, true);
		}

		private void StopPlayFx(GameObject effect, bool play)
		{
			if (effect != null && effect.transform.childCount > 0)
			{
				Transform child = effect.transform.GetChild(0);
				if (child != null)
				{
					ParticleSystem component = child.GetComponent<ParticleSystem>();
					if (component != null)
					{
						if (play)
						{
							component.Play();
						}
						else
						{
							component.Stop();
						}
					}
				}
			}
		}

		private void OnDestroySwirlFxTimer(uint id, object cookie)
		{
			if (this.circularFx != null)
			{
				UnityEngine.Object.Destroy(this.circularFx);
				this.circularFx = null;
			}
			if (this.circularHitFxHandle != AssetHandle.Invalid)
			{
				Service.AssetManager.Unload(this.circularHitFxHandle);
				this.circularHitFxHandle = AssetHandle.Invalid;
			}
		}

		private void OnDestroyHitFxTimer(uint id, object cookie)
		{
			int i = 0;
			int count = this.fxInstances.Count;
			while (i < count)
			{
				if (this.fxInstances[i] != null)
				{
					UnityEngine.Object.Destroy(this.fxInstances[i]);
				}
				i++;
			}
			this.fxInstances.Clear();
			this.targetLocators = null;
			this.adjescentLocators = null;
			this.fxInstances = null;
			if (this.hitFxHandle != AssetHandle.Invalid)
			{
				Service.AssetManager.Unload(this.hitFxHandle);
				this.hitFxHandle = AssetHandle.Invalid;
			}
			if (this.adjHitFxHandle != AssetHandle.Invalid)
			{
				Service.AssetManager.Unload(this.adjHitFxHandle);
				this.adjHitFxHandle = AssetHandle.Invalid;
			}
		}

		private bool FindClosest(SmartEntity target, object cookie)
		{
			GameObjectViewComponent gameObjectViewComp = target.GameObjectViewComp;
			BuildingComponent buildingComp = target.BuildingComp;
			if (gameObjectViewComp != null && buildingComp != null)
			{
				if (this.targetBuildings.Contains(target))
				{
					return false;
				}
				if (gameObjectViewComp.HitLocators.Count > 0)
				{
					GameObject gameObject = null;
					int num = 0;
					int count = gameObjectViewComp.HitLocators.Count;
					float num2 = 10000f;
					for (int i = 0; i < count; i++)
					{
						if (gameObjectViewComp.HitLocators[i] != null)
						{
							Transform transform = gameObjectViewComp.HitLocators[i].transform;
							if (transform != null)
							{
								float num3 = Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(this.heroTrans.position.x, this.heroTrans.position.z));
								if (num3 < num2)
								{
									num2 = num3;
									gameObject = transform.gameObject;
									num = i;
								}
							}
						}
					}
					if (gameObject != null)
					{
						if (!this.targetLocators.Contains(gameObject))
						{
							this.targetLocators.Add(gameObject);
							this.targetBuildings.Add(target);
						}
						GameObject gameObject2;
						if (num > 0)
						{
							gameObject2 = gameObjectViewComp.HitLocators[num - 1];
						}
						else
						{
							gameObject2 = gameObjectViewComp.HitLocators[count - 1];
						}
						if (gameObject2 != null && !this.adjescentLocators.Contains(gameObject2))
						{
							this.adjescentLocators.Add(gameObject2);
						}
						if (num < count - 1)
						{
							gameObject2 = gameObjectViewComp.HitLocators[num + 1];
						}
						else
						{
							gameObject2 = gameObjectViewComp.HitLocators[0];
						}
						if (gameObject2 != null && !this.adjescentLocators.Contains(gameObject2))
						{
							this.adjescentLocators.Add(gameObject2);
						}
					}
				}
			}
			return false;
		}
	}
}
