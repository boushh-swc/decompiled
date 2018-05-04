using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UserInput;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.World.Deploying
{
	public abstract class AbstractDeployer : IUserInputObserver
	{
		protected const int FINGER_ID = 0;

		private const string SPAWN_EFFECT_ID_PREFIX = "SpawnEffect";

		protected Vector3 currentWorldPosition;

		protected Vector2 pressScreenPosition;

		protected bool dragged;

		protected PlanetView worldView;

		public AbstractDeployer()
		{
			this.Reset();
			this.worldView = Service.WorldInitializer.View;
		}

		protected virtual bool EnterMode()
		{
			this.Reset();
			Service.UserInputManager.RegisterObserver(this, UserInputLayer.World);
			return true;
		}

		public virtual void ExitMode()
		{
			this.Reset();
			Service.UserInputManager.UnregisterObserver(this, UserInputLayer.World);
		}

		private void Reset()
		{
			this.currentWorldPosition = Vector3.zero;
			this.pressScreenPosition = -Vector2.one;
			this.dragged = false;
		}

		public EatResponse OnPress(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			if (id != 0)
			{
				return EatResponse.NotEaten;
			}
			if (!Service.UserInputInhibitor.IsDeployAllowed())
			{
				return EatResponse.NotEaten;
			}
			this.currentWorldPosition = groundPosition;
			this.pressScreenPosition = screenPosition;
			this.dragged = false;
			return this.OnPress(target, screenPosition, groundPosition);
		}

		public EatResponse OnDrag(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			if (id != 0)
			{
				return EatResponse.NotEaten;
			}
			if (!this.dragged && CameraUtils.HasDragged(screenPosition, this.pressScreenPosition))
			{
				this.dragged = true;
			}
			return this.OnDrag(target, screenPosition, groundPosition);
		}

		public EatResponse OnRelease(int id)
		{
			if (id != 0)
			{
				return EatResponse.NotEaten;
			}
			EatResponse result = this.OnRelease();
			this.pressScreenPosition = -Vector2.one;
			return result;
		}

		public EatResponse OnScroll(float delta, Vector2 screenPosition)
		{
			return EatResponse.NotEaten;
		}

		protected void PlaySpawnEffect(SmartEntity entity)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(entity.TroopComp.TroopType.Uid);
			string spawnEffectUid = troopTypeVO.SpawnEffectUid;
			if (!string.IsNullOrEmpty(spawnEffectUid))
			{
				EffectsTypeVO effectsTypeVO = Service.StaticDataController.Get<EffectsTypeVO>(spawnEffectUid);
				Service.FXManager.CreateAndAttachFXToEntity(entity, effectsTypeVO.AssetName, "SpawnEffect" + entity.ID.ToString(), null, false, Vector3.zero, true);
				Service.BattleController.CameraShakeObj.Shake(0.5f, 0.25f);
			}
		}

		protected bool IsNotDraggedAndReleasingOwnPress()
		{
			return !this.dragged && this.pressScreenPosition.x >= 0f && this.pressScreenPosition.y >= 0f;
		}

		public abstract EatResponse OnPress(GameObject target, Vector2 screenPosition, Vector3 groundPosition);

		public abstract EatResponse OnDrag(GameObject target, Vector2 screenPosition, Vector3 groundPosition);

		public abstract EatResponse OnRelease();
	}
}
