using StaRTS.Assets;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Animations
{
	public class DroidMoment
	{
		private static readonly Vector3 RIG_OFFSET = new Vector3(2000f, 1000f, 0f);

		private const string MODEL_ASSET = "workerdroidui_neu-ani";

		private const string ASSET_STAGER = "AssetStager";

		private const string ASSET_CAMERA = "GUIAssetCamera3D";

		private const int ANIM_SHOW = 0;

		private const int ANIM_EXIT = 1;

		private const int ANIM_EXITHAPPY = 2;

		private const float EXIT_ANIMATION_TIME = 3f;

		private const int STATE_IDLE = 0;

		private const int STATE_SHOWING = 1;

		private const int STATE_HIDING = 2;

		private int state;

		private GameObject rigGameObject;

		private AssetHandle rigHandle;

		private GameObject modelGameObject;

		private AssetHandle modelHandle;

		private Animator rigAnim;

		private Animator modelAnim;

		public DroidMoment()
		{
			this.state = 0;
			this.rigAnim = null;
			this.modelAnim = null;
			AssetManager assetManager = Service.AssetManager;
			assetManager.Load(ref this.rigHandle, "gui_3d_asset_camera_rig", new AssetSuccessDelegate(this.OnAssetLoadSuccess), null, true);
			assetManager.Load(ref this.modelHandle, "workerdroidui_neu-ani", new AssetSuccessDelegate(this.OnAssetLoadSuccess), null, false);
		}

		private void OnAssetLoadSuccess(object asset, object cookie)
		{
			GameObject gameObject = asset as GameObject;
			bool flag = (bool)cookie;
			if (flag)
			{
				gameObject = Service.AssetManager.CloneGameObject(gameObject);
				this.rigGameObject = gameObject;
			}
			else
			{
				this.modelGameObject = gameObject;
			}
			gameObject.transform.position = DroidMoment.RIG_OFFSET;
			gameObject.SetActive(false);
			if (this.rigGameObject != null && this.modelGameObject != null)
			{
				GameObject gameObject2 = UnityUtils.FindGameObject(this.rigGameObject, "AssetStager");
				GameObject gameObject3 = UnityUtils.FindGameObject(this.rigGameObject, "GUIAssetCamera3D");
				if (gameObject2 != null && gameObject3 != null)
				{
					this.rigAnim = this.rigGameObject.GetComponent<Animator>();
					this.modelAnim = this.modelGameObject.GetComponent<Animator>();
					this.rigGameObject.transform.rotation = Quaternion.AngleAxis(180f, Vector3.up);
					Transform transform = this.modelGameObject.transform;
					transform.parent = gameObject2.transform;
					transform.localPosition = Vector3.zero;
					gameObject3.GetComponent<Camera>().depth = 1f;
				}
				if (this.rigHandle == AssetHandle.Invalid || this.modelHandle == AssetHandle.Invalid || this.rigAnim == null || this.modelAnim == null || this.state == 2)
				{
					this.DestroyDroidMoment();
				}
				else
				{
					this.rigAnim.enabled = true;
					this.modelAnim.enabled = true;
					this.rigGameObject.SetActive(true);
					this.modelGameObject.SetActive(true);
					this.ShowDroidMoment();
				}
			}
		}

		public void ShowDroidMoment()
		{
			if (this.rigAnim != null && this.modelAnim != null)
			{
				this.StartAnimation(0);
			}
			this.state = 1;
		}

		public void HideDroidMoment(bool happy)
		{
			if (Service.CurrentPlayer.CampaignProgress.FueInProgress)
			{
				Service.UserInputInhibitor.DenyAll();
			}
			if (this.rigAnim != null && this.modelAnim != null)
			{
				this.StartAnimation((!happy) ? 1 : 2);
				Service.ViewTimerManager.CreateViewTimer(3f, false, new TimerDelegate(this.OnExitAnimationFinishedTimer), null);
				this.state = 2;
			}
			else
			{
				this.DestroyDroidMoment();
			}
		}

		private void StartAnimation(int animId)
		{
			if (this.rigAnim != null && this.modelAnim != null)
			{
				string text = null;
				if (animId == 0)
				{
					text = "Show";
				}
				else if (animId == 1)
				{
					text = "Exit";
				}
				else if (animId == 2)
				{
					text = "ExitHappy";
				}
				if (text != null)
				{
					this.rigAnim.SetTrigger(text);
					this.modelAnim.SetInteger("Motivation", animId);
				}
			}
		}

		private void OnExitAnimationFinishedTimer(uint id, object cookie)
		{
			this.DestroyDroidMoment();
		}

		public void DestroyDroidMoment()
		{
			if (Service.CurrentPlayer.CampaignProgress.FueInProgress)
			{
				Service.UserInputInhibitor.AllowAll();
			}
			Service.EventManager.SendEvent(EventId.DroidPurchaseAnimationComplete, null);
			if (this.modelGameObject != null)
			{
				UnityEngine.Object.Destroy(this.modelGameObject);
				this.modelGameObject = null;
			}
			if (this.rigGameObject != null)
			{
				UnityEngine.Object.Destroy(this.rigGameObject);
				this.rigGameObject = null;
			}
			AssetManager assetManager = Service.AssetManager;
			if (this.modelHandle != AssetHandle.Invalid)
			{
				assetManager.Unload(this.modelHandle);
				this.modelHandle = AssetHandle.Invalid;
			}
			if (this.rigHandle != AssetHandle.Invalid)
			{
				assetManager.Unload(this.rigHandle);
				this.rigHandle = AssetHandle.Invalid;
			}
			this.state = 0;
		}
	}
}
