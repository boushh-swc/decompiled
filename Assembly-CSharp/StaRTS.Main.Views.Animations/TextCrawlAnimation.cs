using StaRTS.Assets;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.World;
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
	public class TextCrawlAnimation : UXFactory, IBackButtonHandler, IUserInputObserver, IViewFrameTimeObserver
	{
		public delegate void TextCrawlAnimationCompleteDelegate();

		private const string SKIP_BUTTON_CONTAINER = "ContainerBtnSkipIntro";

		private const string SKIP_TEXT_BUTTON_LABEL = "LabelBtnSkipIntro";

		private const string CHAPTER_NUMBER_LABEL = "LabelChapterNumber";

		private const string CHAPTER_NAME_LABEL = "LabelChapterName";

		private const string CHAPTER_BODY_LABEL = "LabelBody";

		private const string LOGO_SPRITE = "logo";

		private const string SKIP_STRING = "s_SKIP";

		private const float START_DELAY = 1.5f;

		private string chapterNumber;

		private string chapterName;

		private string chapterBody;

		private AssetsCompleteDelegate onLoadingCompleteCallback;

		private TextCrawlAnimation.TextCrawlAnimationCompleteDelegate onAnimationCompleteCallback;

		private object onLoadingCompleteCookie;

		private Animation animation;

		private bool animating;

		private bool finishing;

		private bool modifiedCameras;

		private float skipLeft;

		private float skipBottom;

		private Vector2 lastScreenPosition;

		private AssetHandle assetHandle;

		private float oldNearClipPlane;

		public TextCrawlAnimation(string chapterNumber, string chapterName, string chapterBody, AssetsCompleteDelegate onLoadingCompleteCallback, object onLoadingCompleteCookie, TextCrawlAnimation.TextCrawlAnimationCompleteDelegate onAnimationCompleteCallback) : base(Service.CameraManager.UXSceneCamera)
		{
			this.chapterNumber = chapterNumber;
			this.chapterName = chapterName;
			this.chapterBody = chapterBody;
			this.onLoadingCompleteCallback = onLoadingCompleteCallback;
			this.onLoadingCompleteCookie = onLoadingCompleteCookie;
			this.onAnimationCompleteCallback = onAnimationCompleteCallback;
			this.Visible = false;
			Service.EventManager.SendEvent(EventId.PurgeHomeStateRUFTask, null);
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
			string vO_BLACKLIST = GameConstants.VO_BLACKLIST;
			Lang lang = Service.Lang;
			string locale = lang.Locale;
			base.GetElement<UXLabel>("LabelChapterNumber").Text = lang.Get(this.chapterNumber, new object[0]);
			base.GetElement<UXLabel>("LabelChapterName").Text = lang.Get(this.chapterName, new object[0]);
			UXLabel element = base.GetElement<UXLabel>("LabelBody");
			element.UseFontSharpening = false;
			element.Text = LangUtils.ProcessStringWithNewlines(lang.Get(this.chapterBody, new object[0]));
			base.GetElement<UXLabel>("LabelBtnSkipIntro").Text = lang.Get("s_SKIP", new object[0]);
			if (vO_BLACKLIST.Contains(locale))
			{
				base.GetElement<UXSprite>("logo").Visible = false;
			}
			if (this.onLoadingCompleteCallback != null)
			{
				this.onLoadingCompleteCallback(this.onLoadingCompleteCookie);
			}
		}

		public void Start()
		{
			EventManager eventManager = Service.EventManager;
			this.StopNow();
			this.animating = true;
			this.finishing = false;
			Service.IBackButtonManager.RegisterBackButtonHandler(this);
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
				eventManager.SendEvent(EventId.TextCrawlStarted, null);
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

		public bool HandleBackButtonPress()
		{
			bool result = false;
			if (this.modifiedCameras && this.animating && !this.finishing)
			{
				result = true;
				this.FinishUp(true);
			}
			return result;
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

		private void FinishUp(bool didSkip)
		{
			if (!this.finishing)
			{
				Service.IBackButtonManager.UnregisterBackButtonHandler(this);
				this.StopNow();
				this.finishing = true;
				Service.EventManager.SendEvent(EventId.TextCrawlComplete, null);
				HomeState.GoToHomeState(new TransitionCompleteDelegate(this.OnWipeComplete), true);
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
					CameraManager cameraManager = Service.CameraManager;
					cameraManager.MainCamera.Camera.enabled = true;
					cameraManager.UXCamera.Camera.enabled = true;
					Service.UserInputManager.UnregisterObserver(this, UserInputLayer.Screen);
					Camera camera = this.uxCamera.Camera;
					camera.nearClipPlane = this.oldNearClipPlane;
					camera.orthographic = true;
					camera.enabled = false;
				}
				base.DestroyFactory();
			}
		}

		public void OnWipeComplete()
		{
			this.StopNow();
			if (this.assetHandle != AssetHandle.Invalid)
			{
				base.Unload(this.assetHandle, "gui_introAnimation");
				this.assetHandle = AssetHandle.Invalid;
			}
			if (this.onAnimationCompleteCallback != null)
			{
				this.onAnimationCompleteCallback();
			}
		}

		public void OnViewFrameTime(float dt)
		{
			if (!this.animation.isPlaying)
			{
				this.FinishUp(false);
			}
			else
			{
				CameraManager cameraManager = Service.CameraManager;
				if (cameraManager.MainCamera.Camera.enabled)
				{
					cameraManager.MainCamera.Camera.enabled = false;
					cameraManager.UXCamera.Camera.enabled = false;
					Camera camera = this.uxCamera.Camera;
					this.oldNearClipPlane = camera.nearClipPlane;
					camera.fieldOfView = 90f;
					camera.nearClipPlane = 0.01f;
					camera.orthographic = false;
					camera.enabled = true;
				}
			}
		}
	}
}
