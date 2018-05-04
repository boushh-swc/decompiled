using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using StaRTS.GameBoard.Components;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Pooling;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.World.Targeting
{
	public class TargetReticle : IViewFrameTimeObserver
	{
		public const string VIEW_NAME = "TargetReticle {0}";

		public const string ASSET_NAME = "tac_reticle_quad";

		public const int SCALE_PERCENT = 140;

		public const float LOCKON_SCALE_FACTOR = 3f;

		public const float LOCKON_ROTATION = 180f;

		public const float LOCKON_SPIN_DURATION = 0.5f;

		public const float LOCKON_SCALE_DURATION = 0.3f;

		private GameObject gameObjectView;

		private TargetReticlePool targetIdentifier;

		private Entity pendingTarget;

		private bool assetReady;

		private float reticleScale = 1f;

		private string name = string.Empty;

		private float lockonMaxDuration;

		private float ageMS;

		public GameObject View
		{
			get
			{
				return this.gameObjectView;
			}
			set
			{
				this.gameObjectView = value;
				this.ViewTransform = ((!(this.gameObjectView == null)) ? this.gameObjectView.transform : null);
			}
		}

		public Transform ViewTransform
		{
			get;
			private set;
		}

		public TargetReticle()
		{
			AssetHandle assetHandle = AssetHandle.Invalid;
			Service.AssetManager.Load(ref assetHandle, "tac_reticle_quad", new AssetSuccessDelegate(this.OnLoad), new AssetFailureDelegate(this.OnFail), null);
		}

		public static TargetReticle CreateTargetReticlePoolObject(TargetReticlePool objectPool)
		{
			TargetReticle targetReticle = new TargetReticle();
			targetReticle.Construct(objectPool);
			return targetReticle;
		}

		public static void DeactivateTargetReticlePoolObject(TargetReticle targetReticle)
		{
			targetReticle.Deactivate();
		}

		public void Construct(TargetReticlePool objectPool)
		{
			this.targetIdentifier = objectPool;
			if (string.IsNullOrEmpty(this.name))
			{
				this.name = string.Format("TargetReticle {0}", this.targetIdentifier.Capacity);
			}
		}

		private void OnLoad(object asset, object cookie)
		{
			if (asset is GameObject)
			{
				this.assetReady = true;
				this.CreateView((GameObject)asset, this.targetIdentifier);
			}
		}

		private void OnFail(object cookie)
		{
			Service.Logger.Error("Failed to load target reticle");
		}

		private void CreateView(GameObject asset, TargetReticlePool objectPool)
		{
			this.View = asset;
			this.View.name = this.name;
			this.Deactivate();
			if (this.pendingTarget != null)
			{
				this.SetTarget(this.pendingTarget);
				this.pendingTarget = null;
			}
		}

		public void SetTarget(Entity target)
		{
			if (!this.assetReady)
			{
				this.pendingTarget = target;
				return;
			}
			TransformComponent transformComponent = target.Get<TransformComponent>();
			if (transformComponent == null)
			{
				return;
			}
			SizeComponent sizeComponent = target.Get<SizeComponent>();
			this.SetWorldTarget(transformComponent.CenterX(), transformComponent.CenterZ(), (float)sizeComponent.Width, (float)sizeComponent.Depth);
			this.StartLockon();
		}

		public void SetWorldTarget(float boardX, float boardZ, float width, float depth)
		{
			if (this.View == null)
			{
				return;
			}
			this.View.name = this.name;
			this.ViewTransform.position = new Vector3(Units.BoardToWorldX(boardX), 0.01f, Units.BoardToWorldZ(boardZ));
			this.reticleScale = 1.4f * Mathf.Max(Units.BoardToWorldX(width), Units.BoardToWorldZ(depth));
			this.ViewTransform.localScale = new Vector3(3f * this.reticleScale, 3f * this.reticleScale, 3f * this.reticleScale);
			this.ViewTransform.localEulerAngles = Vector3.zero;
			this.View.SetActive(true);
			this.StartLockon();
		}

		public void Deactivate()
		{
			if (this.View == null)
			{
				return;
			}
			this.View.SetActive(false);
		}

		private void StartLockon()
		{
			this.ageMS = 0f;
			this.lockonMaxDuration = Mathf.Max(0.5f, 0.3f);
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		private void FinishLockon()
		{
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
		}

		public void OnViewFrameTime(float dt)
		{
			this.ageMS += dt;
			float num = Mathf.Max(0f, (0.5f - this.ageMS) / 0.5f);
			float num2 = Mathf.Max(0f, (0.3f - this.ageMS) / 0.3f);
			if (this.ageMS >= this.lockonMaxDuration)
			{
				this.FinishLockon();
			}
			float num3 = 1f + num2 * 2f;
			float y = num * 180f;
			this.ViewTransform.localScale = new Vector3(num3 * this.reticleScale, num3 * this.reticleScale, num3 * this.reticleScale);
			this.ViewTransform.localEulerAngles = new Vector3(0f, y, 0f);
		}
	}
}
