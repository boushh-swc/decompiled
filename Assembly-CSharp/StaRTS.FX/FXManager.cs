using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.FX
{
	public class FXManager
	{
		public delegate void AttachedFXLoadedCallback(GameObject instance, Entity parentEntity, float value);

		private const string PARAM_ASSET_NAME = "assetName";

		private const string PARAM_COORDS = "coords";

		private const string PARAM_ROTATION = "rotation";

		private const string PARAM_ENTITY = "entity";

		private const string PARAM_GAME_OBJECT = "gameObject";

		private const string PARAM_ATTACHMENT_KEY = "attachmentKey";

		private const string PARAM_ASSET_KEY = "assetKey";

		private const string PARAM_CALLBACK = "callback";

		private const string PARAM_CALLBACK_VALUE = "value";

		private const string PARAM_CENTER_OF_MASS = "centerOfMass";

		private const string PARAM_APPLY_ROTATION = "rotation";

		private const string DESTRUCTION_FX_NAME = "DestructionFX {0}";

		private const float DESTRUCTION_LIFETIME = 10f;

		private const float DELAY_BEFORE_ATTACHED_PARTICLE_FX_REMOVAL = 5f;

		private List<GameObject> destructionInstances;

		private float speed;

		private List<GameObject> fxInstances;

		private Dictionary<string, uint> activeTimers;

		private Dictionary<string, AssetHandle> assetHandles;

		public FXManager()
		{
			Service.FXManager = this;
			this.destructionInstances = new List<GameObject>();
			this.fxInstances = new List<GameObject>();
			this.activeTimers = new Dictionary<string, uint>();
			this.assetHandles = new Dictionary<string, AssetHandle>();
			this.ResetSpeed();
		}

		public void CreateDestructionFX(SmartEntity entity, bool isBuilding)
		{
			string debrisAssetNameForEntity = FXUtils.GetDebrisAssetNameForEntity(entity, isBuilding);
			if (debrisAssetNameForEntity != null)
			{
				GameObjectViewComponent gameObjectViewComp = entity.GameObjectViewComp;
				if (gameObjectViewComp == null)
				{
					return;
				}
				Vector3 position = gameObjectViewComp.MainTransform.position;
				Vector3 vector = new Vector3(position.x, 0f, position.z);
				if (!isBuilding)
				{
					GameObject centerOfMass = gameObjectViewComp.CenterOfMass;
					if (centerOfMass != null)
					{
						Vector3 position2 = centerOfMass.transform.position;
						vector = new Vector3(position2.x, position2.y, position2.z);
					}
				}
				string text = FXUtils.MakeAssetKey(debrisAssetNameForEntity, vector);
				if (!this.EnsureUniqueFX(text, debrisAssetNameForEntity, vector))
				{
					return;
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary["assetName"] = debrisAssetNameForEntity;
				dictionary["coords"] = vector;
				dictionary["assetKey"] = text;
				AssetHandle value = AssetHandle.Invalid;
				Service.AssetManager.Load(ref value, debrisAssetNameForEntity, new AssetSuccessDelegate(this.OnDestructionFXLoaded), null, dictionary);
				this.assetHandles.Add(text, value);
			}
		}

		private void OnDestructionFXLoaded(object asset, object cookie)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)cookie;
			Vector3 vector = (Vector3)dictionary["coords"];
			GameObject original = asset as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original);
			this.UpdateSpeed(gameObject);
			gameObject.name = string.Format("DestructionFX {0}", this.destructionInstances.Count);
			gameObject.transform.position = new Vector3(vector.x, vector.y, vector.z);
			gameObject.SetActive(true);
			this.destructionInstances.Add(gameObject);
			dictionary["gameObject"] = gameObject;
			uint value = Service.ViewTimerManager.CreateViewTimer(10f, false, new TimerDelegate(this.OnDestructionTimer), dictionary);
			string key = (string)dictionary["assetKey"];
			this.activeTimers.Add(key, value);
		}

		private void OnDestructionTimer(uint id, object cookie)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)cookie;
			GameObject gameObject = (GameObject)dictionary["gameObject"];
			string text = (string)dictionary["assetKey"];
			this.destructionInstances.Remove(gameObject);
			UnityEngine.Object.Destroy(gameObject);
			this.UnloadByAssetKey(text);
			this.activeTimers.Remove(text);
		}

		public void CreateRubbleAtEntityPosition(SmartEntity entity)
		{
			string rubbleAssetNameForEntity = FXUtils.GetRubbleAssetNameForEntity(entity);
			if (rubbleAssetNameForEntity != null)
			{
				GameObjectViewComponent gameObjectViewComp = entity.GameObjectViewComp;
				Vector3 position = gameObjectViewComp.MainTransform.position;
				float y = UnityEngine.Random.Range(0f, 360f);
				Quaternion rotation = Quaternion.Euler(0f, y, 0f);
				this.CreateFXAtPosition(rubbleAssetNameForEntity, position, rotation);
			}
		}

		public void CreateFXAtPosition(string assetName, Vector3 worldPos)
		{
			this.CreateFXAtPosition(assetName, worldPos, Quaternion.identity);
		}

		public void CreateFXAtPosition(string assetName, Vector3 worldPos, Quaternion rotation)
		{
			string text = FXUtils.MakeAssetKey(assetName, worldPos);
			if (!this.EnsureUniqueFX(text, assetName, worldPos))
			{
				return;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["assetName"] = assetName;
			dictionary["coords"] = worldPos;
			dictionary["rotation"] = rotation;
			AssetHandle value = AssetHandle.Invalid;
			Service.AssetManager.Load(ref value, assetName, new AssetSuccessDelegate(this.OnFXWithPositionLoaded), null, dictionary);
			this.assetHandles.Add(text, value);
		}

		private void OnFXWithPositionLoaded(object asset, object cookie)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)cookie;
			string assetName = (string)dictionary["assetName"];
			Vector3 position = (Vector3)dictionary["coords"];
			Quaternion rotation = (Quaternion)dictionary["rotation"];
			GameObject gameObject = this.InitFXInstanceOnLoad(asset, assetName);
			gameObject.transform.position = position;
			gameObject.transform.rotation = rotation;
		}

		public void CreateAndAttachRubbleToEntity(Entity entity)
		{
			SmartEntity smartEntity = (SmartEntity)entity;
			string rubbleAssetNameForEntity = FXUtils.GetRubbleAssetNameForEntity(smartEntity);
			if (rubbleAssetNameForEntity != null)
			{
				GameObjectViewComponent gameObjectViewComp = smartEntity.GameObjectViewComp;
				GameUtils.ToggleGameObjectViewVisibility(gameObjectViewComp, false);
				Service.AssetManager.RegisterPreloadableAsset(rubbleAssetNameForEntity);
				this.CreateAndAttachFXToEntity(entity, rubbleAssetNameForEntity, "rubble", null, false, Vector3.zero, true);
			}
		}

		public void RemoveAttachedRubbleFromEntity(Entity entity)
		{
			GameObjectViewComponent viewComp = entity.Get<GameObjectViewComponent>();
			GameUtils.ToggleGameObjectViewVisibility(viewComp, true);
			this.RemoveAttachedFXFromEntity(entity, "rubble");
		}

		public void CreateAndAttachFXToEntity(Entity entity, string assetName, string attachmentKey, FXManager.AttachedFXLoadedCallback callback, bool pinToCenterOfMass, Vector3 offset, bool applyRotation)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["assetName"] = assetName;
			dictionary["entity"] = entity;
			dictionary["attachmentKey"] = attachmentKey;
			dictionary["assetKey"] = FXUtils.MakeAssetKey(attachmentKey, entity);
			dictionary["callback"] = callback;
			dictionary["centerOfMass"] = pinToCenterOfMass;
			dictionary["coords"] = offset;
			dictionary["rotation"] = applyRotation;
			this.CreateAndAttachFXEntityInternal(assetName, dictionary);
		}

		public void CreateAndAttachFXToEntity(Entity entity, string assetName, string attachmentKey, FXManager.AttachedFXLoadedCallback callback, float value, bool pinToCenterOfMass, Vector3 offset)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["assetName"] = assetName;
			dictionary["entity"] = entity;
			dictionary["attachmentKey"] = attachmentKey;
			dictionary["assetKey"] = FXUtils.MakeAssetKey(attachmentKey, entity);
			dictionary["callback"] = callback;
			dictionary["value"] = value;
			dictionary["centerOfMass"] = pinToCenterOfMass;
			dictionary["coords"] = offset;
			dictionary["rotation"] = true;
			this.CreateAndAttachFXEntityInternal(assetName, dictionary);
		}

		private void CreateAndAttachFXEntityInternal(string assetName, Dictionary<string, object> cookie)
		{
			string text = (string)cookie["assetKey"];
			string text2 = (string)cookie["attachmentKey"];
			if (this.activeTimers.ContainsKey(text))
			{
				Service.ViewTimerManager.TriggerKillViewTimer(this.activeTimers[text]);
				this.activeTimers.Remove(text);
			}
			SmartEntity smartEntity = (SmartEntity)cookie["entity"];
			GameObjectViewComponent gameObjectViewComp = smartEntity.GameObjectViewComp;
			if (gameObjectViewComp != null && !gameObjectViewComp.HasAttachment(text2))
			{
				if (!this.EnsureUniqueFX(text, assetName, text2))
				{
					return;
				}
				AssetManager assetManager = Service.AssetManager;
				assetManager.RegisterPreloadableAsset(assetName);
				AssetHandle value = AssetHandle.Invalid;
				assetManager.Load(ref value, assetName, new AssetSuccessDelegate(this.OnAttachedFXLoaded), null, cookie);
				this.assetHandles.Add(text, value);
			}
		}

		private void OnAttachedFXLoaded(object asset, object cookie)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)cookie;
			string assetName = (string)dictionary["assetName"];
			SmartEntity smartEntity = (SmartEntity)dictionary["entity"];
			if (smartEntity == null || smartEntity.GameObjectViewComp == null)
			{
				if (Service.AssetManager.IsAssetCloned(assetName))
				{
					UnityEngine.Object.Destroy(asset as GameObject);
				}
				string assetKey = (string)dictionary["assetKey"];
				this.UnloadByAssetKey(assetKey);
				return;
			}
			string key = (string)dictionary["attachmentKey"];
			bool pinToCenterOfMass = (bool)dictionary["centerOfMass"];
			bool flag = (bool)dictionary["rotation"];
			Vector3 offset = (Vector3)dictionary["coords"];
			GameObject gameObject = this.InitFXInstanceOnLoad(asset, assetName);
			if (gameObject != null)
			{
				GameObjectViewComponent gameObjectViewComp = smartEntity.GameObjectViewComp;
				gameObjectViewComp.AttachGameObject(key, gameObject, offset, false, pinToCenterOfMass);
				if (flag)
				{
					gameObject.transform.rotation = gameObjectViewComp.MainTransform.rotation;
				}
			}
			FXManager.AttachedFXLoadedCallback attachedFXLoadedCallback = (FXManager.AttachedFXLoadedCallback)dictionary["callback"];
			if (attachedFXLoadedCallback != null)
			{
				float value = 0f;
				if (dictionary.ContainsKey("value"))
				{
					value = (float)dictionary["value"];
				}
				attachedFXLoadedCallback(gameObject, smartEntity, value);
			}
		}

		private GameObject InitFXInstanceOnLoad(object asset, string assetName)
		{
			GameObject gameObject;
			if (Service.AssetManager.IsAssetCloned(assetName))
			{
				gameObject = (asset as GameObject);
			}
			else
			{
				gameObject = UnityEngine.Object.Instantiate<GameObject>(asset as GameObject);
			}
			gameObject.SetActive(true);
			this.fxInstances.Add(gameObject);
			return gameObject;
		}

		public void StopParticlesAndRemoveAttachedFXFromEntity(Entity entity, string attachmentKey)
		{
			if (entity == null)
			{
				return;
			}
			GameObjectViewComponent gameObjectViewComponent = entity.Get<GameObjectViewComponent>();
			if (gameObjectViewComponent == null)
			{
				return;
			}
			string text = FXUtils.MakeAssetKey(attachmentKey, entity);
			GameObject attachedGameObject = gameObjectViewComponent.GetAttachedGameObject(attachmentKey);
			if (attachedGameObject != null)
			{
				float delay = 0f;
				ParticleSystem[] componentsInChildren = attachedGameObject.GetComponentsInChildren<ParticleSystem>(true);
				if (componentsInChildren != null && componentsInChildren.Length > 0)
				{
					delay = 5f;
					int i = 0;
					int num = componentsInChildren.Length;
					while (i < num)
					{
						componentsInChildren[i].Stop(false);
						i++;
					}
				}
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary["entity"] = entity;
				dictionary["attachmentKey"] = attachmentKey;
				dictionary["assetKey"] = text;
				dictionary["gameObject"] = attachedGameObject;
				if (this.activeTimers.ContainsKey(text))
				{
					Service.ViewTimerManager.KillViewTimer(this.activeTimers[text]);
					this.activeTimers.Remove(text);
				}
				uint value = Service.ViewTimerManager.CreateViewTimer(delay, false, new TimerDelegate(this.RemoveAttachedFXFromEntityAfterDelay), dictionary);
				this.activeTimers.Add(text, value);
			}
			else
			{
				this.UnloadByAssetKey(text);
			}
		}

		private void RemoveAttachedFXFromEntityAfterDelay(uint id, object cookie)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)cookie;
			SmartEntity smartEntity = (SmartEntity)dictionary["entity"];
			string text = (string)dictionary["assetKey"];
			this.activeTimers.Remove(text);
			if (smartEntity != null && smartEntity.GameObjectViewComp != null)
			{
				string attachmentKey = (string)dictionary["attachmentKey"];
				this.RemoveAttachedFXFromEntity(smartEntity, attachmentKey);
			}
			else
			{
				GameObject gameObject = (GameObject)dictionary["gameObject"];
				this.fxInstances.Remove(gameObject);
				UnityEngine.Object.Destroy(gameObject);
				this.UnloadByAssetKey(text);
			}
		}

		public void RemoveAttachedFXFromEntity(Entity entity, string attachmentKey)
		{
			GameObjectViewComponent gameObjectViewComponent = null;
			if (entity != null)
			{
				gameObjectViewComponent = entity.Get<GameObjectViewComponent>();
			}
			if (gameObjectViewComponent != null)
			{
				GameObject attachedGameObject = gameObjectViewComponent.GetAttachedGameObject(attachmentKey);
				if (attachedGameObject != null)
				{
					gameObjectViewComponent.DetachGameObject(attachmentKey);
					this.fxInstances.Remove(attachedGameObject);
					UnityEngine.Object.Destroy(attachedGameObject);
				}
			}
			string assetKey = FXUtils.MakeAssetKey(attachmentKey, entity);
			this.UnloadByAssetKey(assetKey);
		}

		public void Reset()
		{
			foreach (KeyValuePair<string, AssetHandle> current in this.assetHandles)
			{
				Service.AssetManager.Unload(current.Value);
			}
			this.assetHandles.Clear();
			foreach (KeyValuePair<string, uint> current2 in this.activeTimers)
			{
				Service.ViewTimerManager.KillViewTimer(current2.Value);
			}
			this.activeTimers.Clear();
			int i = 0;
			int count = this.destructionInstances.Count;
			while (i < count)
			{
				if (this.destructionInstances[i] != null)
				{
					UnityEngine.Object.Destroy(this.destructionInstances[i]);
				}
				i++;
			}
			this.destructionInstances.Clear();
			int j = 0;
			int count2 = this.fxInstances.Count;
			while (j < count2)
			{
				if (this.fxInstances[j] != null)
				{
					UnityEngine.Object.Destroy(this.fxInstances[j]);
				}
				j++;
			}
			this.fxInstances.Clear();
			this.ResetSpeed();
		}

		public void SetSpeed(float speed)
		{
			this.speed = speed;
			for (int i = 0; i < this.destructionInstances.Count; i++)
			{
				this.UpdateSpeed(this.destructionInstances[i]);
			}
		}

		private void ResetSpeed()
		{
			this.speed = 1f;
		}

		private void UpdateSpeed(GameObject destructionInstance)
		{
			if (destructionInstance == null)
			{
				return;
			}
			ParticleSystem component = destructionInstance.GetComponent<ParticleSystem>();
			component.main.simulationSpeed = this.speed;
		}

		private void UnloadByAssetKey(string assetKey)
		{
			AssetHandle handle;
			if (this.assetHandles.TryGetValue(assetKey, out handle))
			{
				Service.AssetManager.Unload(handle);
				this.assetHandles.Remove(assetKey);
			}
		}

		private bool EnsureUniqueFX(string assetKey, string assetName, Vector3 coords)
		{
			return !this.assetHandles.ContainsKey(assetKey);
		}

		private bool EnsureUniqueFX(string assetKey, string assetName, string attachmentKey)
		{
			if (this.assetHandles.ContainsKey(assetKey))
			{
				Service.Logger.WarnFormat("Attempted to add duplicate attachment: key:{0} asset:{1} at {2}", new object[]
				{
					assetKey,
					assetName,
					attachmentKey
				});
				return false;
			}
			return true;
		}
	}
}
