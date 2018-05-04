using Midcore.Web;
using StaRTS.Externals.BI;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UserInput;
using StaRTS.Utils.Core;
using StaRTS.Utils.Diagnostics;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class Engine : MonoBehaviour
	{
		private const string MAIN_SCENE_NAME = "MainScene";

		public const uint SIM_TIME_PER_FRAME = 33u;

		public const float VIEW_PHYSICS_TIME_PER_FRAME = 0.033f;

		public static int NumReloads;

		public WebManager WebManager;

		private bool reloadWait;

		private UserInputManager inputManager;

		private SimTimeEngine simTimeEngine;

		private ViewTimeEngine viewTimeEngine;

		private bool isPaused;

		private void Start()
		{
			this.isPaused = false;
			base.enabled = false;
			this.WebManager = new WebManager(this);
			this.ForceGarbageCollection(new Action(this.OnGarbageCollectedStart));
		}

		private void OnGarbageCollectedStart()
		{
			base.enabled = true;
			Application.runInBackground = true;
			Application.targetFrameRate = 30;
			Service.Engine = this;
			this.InitLogger();
			uint monoUsedSize = Profiler.GetMonoUsedSize();
			uint monoHeapSize = Profiler.GetMonoHeapSize();
			Service.Logger.DebugFormat("Managed memory on load: {0:F1}MB/{1:F1}MB", new object[]
			{
				monoUsedSize / 1048576f,
				monoHeapSize / 1048576f
			});
			this.reloadWait = false;
			this.inputManager = new TouchManager();
			this.simTimeEngine = new SimTimeEngine(33u);
			this.viewTimeEngine = new ViewTimeEngine(0.033f);
			new MainController();
		}

		private void Update()
		{
			this.inputManager.OnUpdate();
			this.simTimeEngine.OnUpdate();
			this.viewTimeEngine.OnUpdate();
		}

		private void InitLogger()
		{
			Logger logger = new Logger();
			UnityLogAppender unityLogAppender = null;
			logger.ErrorLevels = (LogLevel)3;
			logger.AddAppender(new BILogAppender(unityLogAppender));
			logger.Start();
		}

		public void Reload()
		{
			Service.Logger.Debug("Reloading the game...");
			Engine.NumReloads++;
			this.inputManager.UnregisterAll();
			this.simTimeEngine.UnregisterAll();
			this.viewTimeEngine.UnregisterAll();
			base.StopAllCoroutines();
			this.reloadWait = true;
			base.StartCoroutine(this.ReloadNow());
		}

		[DebuggerHidden]
		private IEnumerator ReloadNow()
		{
			if (this.reloadWait)
			{
				this.reloadWait = false;
				yield return null;
			}
			uint monoUsedSize = Profiler.GetMonoUsedSize();
			uint monoHeapSize = Profiler.GetMonoHeapSize();
			Service.Logger.DebugFormat("Managed memory before reload: {0:F1}MB/{1:F1}MB", new object[]
			{
				monoUsedSize / 1048576f,
				monoHeapSize / 1048576f
			});
			MainController.StaticReset();
			MainController.CleanupReferences();
			Service.ResetAll();
			this.ForceGarbageCollection(new Action(this.OnGarbageCollectedReload));
			yield break;
		}

		private void OnGarbageCollectedReload()
		{
			Application.LoadLevelAsync("MainScene");
		}

		public void ForceGarbageCollection(Action onComplete)
		{
			GarbageCollector garbageCollector = base.gameObject.GetComponent<GarbageCollector>();
			if (garbageCollector == null)
			{
				garbageCollector = base.gameObject.AddComponent<GarbageCollector>();
			}
			garbageCollector.RegisterCompleteCallback(onComplete);
		}

		public void OnApplicationPause(bool paused)
		{
			if (paused == this.isPaused)
			{
				return;
			}
			if (Service.EventManager != null)
			{
				this.isPaused = paused;
				Service.EventManager.SendEvent(EventId.ApplicationPauseToggled, paused);
			}
		}

		public void OnApplicationQuit()
		{
			if (Service.EventManager != null)
			{
				Service.EventManager.SendEvent(EventId.ApplicationQuit, null);
			}
		}
	}
}
