using Net.RichardLord.Ash.Core;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.Entities.Components
{
	public class GameObjectViewComponent : ComponentBase
	{
		private GameObject mainGameObject;

		private Dictionary<string, GameObject> SecondaryGameObjects;

		private Dictionary<string, GameObjectAttachment> Attachments;

		private List<List<GameObject>> primaryGunLocators;

		private List<List<GameObject>> altGunLocators;

		private GameObject centerOfMass;

		private Transform centerOfMassTransform;

		public GameObject MainGameObject
		{
			get
			{
				return this.mainGameObject;
			}
			set
			{
				this.mainGameObject = value;
				this.MainTransform = ((!(this.mainGameObject == null)) ? this.mainGameObject.transform : null);
			}
		}

		public Transform MainTransform
		{
			get;
			private set;
		}

		public bool NeedsCollider
		{
			get;
			set;
		}

		public List<List<GameObject>> GunLocators
		{
			get;
			private set;
		}

		public List<GameObject> HitLocators
		{
			get;
			set;
		}

		public GameObject CenterOfMass
		{
			get
			{
				return this.centerOfMass;
			}
			set
			{
				this.centerOfMass = value;
				this.centerOfMassTransform = ((!(this.centerOfMass == null)) ? this.centerOfMass.transform : null);
			}
		}

		public bool IsFlashing
		{
			get;
			set;
		}

		public GameObject VehicleLocator
		{
			get;
			set;
		}

		public List<GameObject> EffectLocators
		{
			get;
			set;
		}

		public List<GameObject> EffectGameObjects
		{
			get;
			set;
		}

		public float TooltipHeightOffset
		{
			get;
			private set;
		}

		public float GameObjectHeight
		{
			get;
			private set;
		}

		public GameObjectViewComponent(GameObject gameObject, float tooltipHeightOffset)
		{
			this.MainGameObject = gameObject;
			this.primaryGunLocators = new List<List<GameObject>>();
			this.GunLocators = this.primaryGunLocators;
			this.HitLocators = new List<GameObject>();
			this.CenterOfMass = null;
			this.VehicleLocator = null;
			this.EffectLocators = new List<GameObject>();
			this.EffectGameObjects = new List<GameObject>();
			this.TooltipHeightOffset = tooltipHeightOffset;
			this.GameObjectHeight = UnityUtils.GetGameObjectBounds(gameObject).max.y;
		}

		private void PurgeAllAttachments()
		{
			if (this.Attachments != null)
			{
				List<string> list = new List<string>();
				foreach (string current in this.Attachments.Keys)
				{
					list.Add(current);
				}
				foreach (string current2 in list)
				{
					GameObject attachedGameObject = this.Attachments[current2].AttachedGameObject;
					if (attachedGameObject != null)
					{
						attachedGameObject.SetActive(false);
					}
					this.DetachGameObject(current2);
				}
			}
			Service.EventManager.SendEvent(EventId.ViewObjectsPurged, this.Entity);
		}

		public override void OnRemove()
		{
			this.PurgeAllAttachments();
			if (this.MainGameObject != null)
			{
				UnityEngine.Object.Destroy(this.MainGameObject);
				this.MainGameObject = null;
			}
		}

		public void SetXYZ(float x, float y, float z)
		{
			this.MainTransform.position = new Vector3(x, y, z);
			this.UpdateAllAttachments();
		}

		public void UpdateAllAttachments()
		{
			if (this.Attachments != null)
			{
				foreach (string current in this.Attachments.Keys)
				{
					this.UpdateAttachment(current);
				}
			}
		}

		public void SetOffset(string key, Vector3 offset)
		{
			if (this.Attachments.ContainsKey(key))
			{
				GameObjectAttachment gameObjectAttachment = this.Attachments[key];
				gameObjectAttachment.Offset = offset;
				this.UpdateAttachment(key);
			}
		}

		public void UpdateAttachment(string key)
		{
			GameObjectAttachment gameObjectAttachment = this.Attachments[key];
			if (gameObjectAttachment == null || gameObjectAttachment.AttachedGameObject == null || gameObjectAttachment.AttachedGameObject.transform == null)
			{
				return;
			}
			Vector3 vector = gameObjectAttachment.Offset;
			if (gameObjectAttachment.CenterOfMassPin && this.centerOfMassTransform != null)
			{
				vector += this.centerOfMassTransform.position;
			}
			else if (this.MainTransform != null)
			{
				vector += this.MainTransform.position;
			}
			if (gameObjectAttachment.FloorPin)
			{
				vector.y = gameObjectAttachment.Offset.y;
			}
			gameObjectAttachment.AttachedGameObject.transform.position = vector;
		}

		public void Rotate(float rads)
		{
			float angle = -rads * 57.29578f;
			this.MainTransform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
		}

		public void ComputeRotationToTarget(GameObjectViewComponent targetView, ref float rotation)
		{
			Vector3 position = this.MainTransform.position;
			Vector3 position2 = targetView.MainTransform.position;
			float y = position2.z - position.z;
			float x = position2.x - position.x;
			rotation = Mathf.Atan2(y, x);
		}

		public void AddSO(string key, GameObject obj)
		{
			if (this.SecondaryGameObjects == null)
			{
				this.SecondaryGameObjects = new Dictionary<string, GameObject>();
				this.SecondaryGameObjects.Add(key, obj);
			}
			else if (!this.SecondaryGameObjects.ContainsKey(key))
			{
				this.SecondaryGameObjects.Add(key, obj);
			}
			else
			{
				this.SecondaryGameObjects[key] = obj;
			}
		}

		public GameObject GetSO(string key)
		{
			if (this.SecondaryGameObjects != null && this.SecondaryGameObjects.ContainsKey(key))
			{
				return this.SecondaryGameObjects[key];
			}
			return null;
		}

		public void AttachGameObject(string key, GameObject obj, Vector3 offset, bool pinToFloor, bool pinToCenterOfMass)
		{
			if (this.Attachments == null)
			{
				this.Attachments = new Dictionary<string, GameObjectAttachment>();
			}
			else if (this.Attachments.ContainsKey(key))
			{
				return;
			}
			GameObjectAttachment gameObjectAttachment = new GameObjectAttachment();
			gameObjectAttachment.Key = key;
			gameObjectAttachment.AttachedGameObject = obj;
			gameObjectAttachment.Offset = offset;
			gameObjectAttachment.FloorPin = pinToFloor;
			gameObjectAttachment.CenterOfMassPin = pinToCenterOfMass;
			this.Attachments.Add(key, gameObjectAttachment);
			this.UpdateAttachment(key);
		}

		public void DetachGameObject(string key)
		{
			if (this.Attachments == null || !this.Attachments.ContainsKey(key))
			{
				return;
			}
			this.Attachments.Remove(key);
			if (this.Attachments.Count == 0)
			{
				this.Attachments = null;
			}
		}

		public bool HasAttachment(string key)
		{
			return this.Attachments != null && this.Attachments.ContainsKey(key);
		}

		public GameObject GetAttachedGameObject(string key)
		{
			GameObjectAttachment gameObjectAttachment;
			if (this.Attachments != null && this.Attachments.TryGetValue(key, out gameObjectAttachment))
			{
				return gameObjectAttachment.AttachedGameObject;
			}
			return null;
		}

		public List<List<GameObject>> SetupAlternateGunLocators()
		{
			if (this.altGunLocators == null)
			{
				this.altGunLocators = new List<List<GameObject>>();
			}
			return this.altGunLocators;
		}

		public void SwitchGunLocators(bool toAlt)
		{
			if (toAlt && this.altGunLocators != null && this.altGunLocators.Count > 0)
			{
				this.GunLocators = this.altGunLocators;
			}
			else
			{
				this.GunLocators = this.primaryGunLocators;
			}
		}
	}
}
