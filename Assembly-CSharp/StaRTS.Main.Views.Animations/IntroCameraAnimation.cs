using StaRTS.Assets;
using StaRTS.Main.Models;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.UserInput;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Animations
{
	public class IntroCameraAnimation : UXFactory, IUserInputObserver, IViewFrameTimeObserver
	{
		private const string SKIP_BUTTON_CONTAINER = "ContainerBtnSkipIntro";

		private const string SKIP_TEXT = "LabelBtnSkipIntro";

		private const string INTRO_TEXT_1 = "LabelChapterNumber";

		private const string INTRO_TEXT_2 = "LabelChapterName";

		private const string INTRO_TEXT_3 = "LabelBody";

		private const string LOGO_SPRITE = "logo";

		private const float START_DELAY = 1.5f;

		private AssetsCompleteDelegate onCompleteCallback;

		private object onCompleteCookie;

		private Animation animation;

		private bool animating;

		private bool finishing;

		private bool modifiedCameras;

		private float skipLeft;

		private float skipBottom;

		private Vector2 lastScreenPosition;

		private AssetHandle assetHandle;

		private float oldNearClipPlane;

		public IntroCameraAnimation(AssetsCompleteDelegate onCompleteCallback, object onCompleteCookie) : base(Service.CameraManager.UXSceneCamera)
		{
			this.onCompleteCallback = onCompleteCallback;
			this.onCompleteCookie = onCompleteCookie;
			this.Visible = false;
			base.Load(ref this.assetHandle, "gui_introAnimation", new UXFactoryLoadDelegate(this.Loaded), new UXFactoryLoadDelegate(this.Loaded), null);
		}

		private void Loaded(object cookie)
		{
			bool flag = base.IsLoaded();
			if (flag)
			{
				this.animation = base.Root.GetComponent<Animation>();
				if (this.animation != null)
				{
					this.animation.playAutomatically = false;
				}
			}
			if (this.onCompleteCallback != null)
			{
				this.onCompleteCallback(this.onCompleteCookie);
			}
			string locale = Service.Lang.Locale;
			string vO_BLACKLIST = GameConstants.VO_BLACKLIST;
			Lang lang = Service.Lang;
			base.GetElement<UXLabel>("LabelChapterNumber").Text = lang.Get("INTRO_TEXT_1", new object[0]);
			base.GetElement<UXLabel>("LabelChapterName").Text = lang.Get("INTRO_TEXT_2", new object[0]);
			UXLabel element = base.GetElement<UXLabel>("LabelBody");
			element.UseFontSharpening = false;
			element.Text = LangUtils.ProcessStringWithNewlines(lang.Get("INTRO_TEXT_3", new object[0]));
			base.GetElement<UXLabel>("LabelBtnSkipIntro").Text = lang.Get("s_SKIP", new object[0]);
			if (vO_BLACKLIST.Contains(locale))
			{
				base.GetElement<UXSprite>("logo").Visible = false;
			}
		}

		public void Start()
		{
			Service.BILoggingController.TrackGameAction("text_crawl", "start", null, null);
			this.StopNow();
			this.animating = true;
			this.finishing = false;
			if (this.animation == null)
			{
				this.modifiedCameras = false;
				this.FinishUp(false);
			}
			else
			{
				CameraManager cameraManager = Service.CameraManager;
				cameraManager.MainCamera.Camera.enabled = false;
				cameraManager.UXCamera.Camera.enabled = false;
				Camera camera = this.uxCamera.Camera;
				this.oldNearClipPlane = camera.nearClipPlane;
				camera.fieldOfView = 90f;
				camera.nearClipPlane = 0.01f;
				camera.orthographic = false;
				camera.enabled = true;
				UXElement element = base.GetElement<UXElement>("ContainerBtnSkipIntro");
				float x = (float)Screen.width * 0.5f;
				float num = (float)Screen.height * 0.5f;
				float z = num;
				element.LocalPosition = new Vector3(x, num, z);
				this.skipLeft = (float)Screen.width - element.Width;
				this.skipBottom = (float)Screen.height - element.Height;
				Service.UserInputManager.RegisterObserver(this, UserInputLayer.Screen);
				this.modifiedCameras = true;
				this.Visible = true;
				this.animation.Play();
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
				Service.EventManager.SendEvent(EventId.IntroStarted, null);
			}
		}

		public EatResponse OnPress(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			this.lastScreenPosition = screenPosition;
			return EatResponse.Eaten;
		}

		public EatResponse OnDrag(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			this.lastScreenPosition = screenPosition;
			return EatResponse.Eaten;
		}

		public EatResponse OnRelease(int id)
		{
			if (this.modifiedCameras && this.animating && !this.finishing && this.lastScreenPosition.x >= this.skipLeft && this.lastScreenPosition.x < (float)Screen.width && this.lastScreenPosition.y >= this.skipBottom && this.lastScreenPosition.y < (float)Screen.height)
			{
				this.FinishUp(true);
			}
			return EatResponse.Eaten;
		}

		public EatResponse OnScroll(float delta, Vector2 screenPosition)
		{
			return EatResponse.Eaten;
		}

		public void FinishUp(bool didSkip)
		{
			if (!this.finishing)
			{
				string action = (!didSkip) ? "finish" : "skip";
				Service.BILoggingController.TrackGameAction("text_crawl", action, null, null);
				this.finishing = true;
				Service.EventManager.SendEvent(EventId.IntroComplete, null);
				Service.CameraManager.WipeCamera.StartLinearWipe(WipeTransition.FromIntroToBase, 1.57079637f, new WipeCompleteDelegate(this.OnWipeComplete), null);
			}
		}

		public void StopNow()
		{
			if (this.animating)
			{
				this.animating = false;
				this.Visible = false;
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
				if (this.modifiedCameras)
				{
					this.modifiedCameras = false;
					Service.UserInputManager.UnregisterObserver(this, UserInputLayer.Screen);
					Camera camera = this.uxCamera.Camera;
					camera.nearClipPlane = this.oldNearClipPlane;
					camera.orthographic = true;
					camera.enabled = false;
				}
				base.DestroyFactory();
			}
		}

		private void OnWipeComplete(object cookie)
		{
			this.StopNow();
			if (this.assetHandle != AssetHandle.Invalid)
			{
				base.Unload(this.assetHandle, "gui_introAnimation");
				this.assetHandle = AssetHandle.Invalid;
			}
		}

		public void OnViewFrameTime(float dt)
		{
			if (!this.animation.isPlaying)
			{
				this.FinishUp(false);
			}
		}
	}
}
