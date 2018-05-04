using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Models.Entities.Components
{
	public class TroopShieldViewComponent : ComponentBase
	{
		private const string SHIELD_EFFECT_ANCHOR = "bnd_shield";

		private const string ANIM_PROP_MOTIVATOR = "Motivation";

		private const int ANIM_SHIELD_ON = 1;

		private const int ANIM_SHIELD_OFF = 0;

		private const float ANIM_SHIELD_ON_DELAY = 0.1f;

		private AssetHandle assetHandle;

		private Animator anim;

		public TroopShieldViewComponent(TroopTypeVO troop)
		{
			string shieldAssetName = troop.ShieldAssetName;
			string assetName = Service.StaticDataController.Get<EffectsTypeVO>(shieldAssetName).AssetName;
			Service.AssetManager.Load(ref this.assetHandle, assetName, new AssetSuccessDelegate(this.OnEffectLoaded), null, true);
		}

		private void OnEffectLoaded(object asset, object cookie)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(asset as GameObject);
			GameObject mainGameObject = ((SmartEntity)this.Entity).GameObjectViewComp.MainGameObject;
			Transform transform = GameUtils.FindAssetMetaDataTransform(mainGameObject, "bnd_shield");
			if (transform != null)
			{
				Transform transform2 = gameObject.transform;
				transform2.parent = transform;
				transform2.localPosition = Vector3.zero;
				transform2.localScale = Vector3.one;
			}
			this.anim = gameObject.GetComponent<Animator>();
			Service.ViewTimerManager.CreateViewTimer(0.1f, false, new TimerDelegate(this.ActivateShieldCallback), null);
		}

		private void ActivateShieldCallback(uint id, object cookie)
		{
			SmartEntity smartEntity = this.Entity as SmartEntity;
			if (smartEntity != null && smartEntity.StateComp != null && !smartEntity.StateComp.IsRunning)
			{
				this.PlayActivateAnimation();
			}
		}

		public override void OnRemove()
		{
			if (this.assetHandle != AssetHandle.Invalid)
			{
				Service.AssetManager.Unload(this.assetHandle);
				this.assetHandle = AssetHandle.Invalid;
			}
		}

		public void PlayActivateAnimation()
		{
			if (this.anim != null && this.anim.isActiveAndEnabled)
			{
				this.anim.gameObject.transform.localRotation = Quaternion.identity;
				this.anim.SetInteger("Motivation", 1);
			}
		}

		public void PlayDeactivateAnimation()
		{
			if (this.anim != null)
			{
				this.anim.SetInteger("Motivation", 0);
			}
		}
	}
}
