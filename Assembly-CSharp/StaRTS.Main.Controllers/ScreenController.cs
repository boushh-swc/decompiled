using StaRTS.Assets;
using StaRTS.Main.Configs;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class ScreenController : IViewFrameTimeObserver, IEventObserver
	{
		private List<ScreenInfo> screens;

		private Dictionary<string, GameObjectContainer> cache;

		private List<AssetRequest> loadQueue;

		private Queue<ScreenInfo> queuedScreens;

		private ScreenInfo scrim;

		public ScreenController()
		{
			Service.ScreenController = this;
			this.screens = new List<ScreenInfo>();
			this.cache = new Dictionary<string, GameObjectContainer>();
			this.loadQueue = new List<AssetRequest>();
			this.queuedScreens = new Queue<ScreenInfo>();
			this.scrim = null;
			Service.EventManager.RegisterObserver(this, EventId.UpdateScrim, EventPriority.Default);
		}

		public void AddScreen(UXElement screen, bool modal, QueueScreenBehavior subType)
		{
			this.AddScreen(screen, modal, true, subType);
		}

		public void AddScreen(UXElement screen, bool modal)
		{
			this.AddScreen(screen, modal, true, QueueScreenBehavior.Default);
		}

		public void AddScreen(UXElement screen, bool modal, bool visibleScrim)
		{
			this.AddScreen(screen, modal, visibleScrim, QueueScreenBehavior.Default);
		}

		public void AddScreen(ScreenBase screen, QueueScreenBehavior subType)
		{
			this.AddScreen(screen, true, subType);
			if (!UXUtils.ShouldShowHudBehindScreen(screen.AssetName))
			{
				Service.UXController.HUD.Visible = false;
			}
		}

		public ScreenInfo AddScreen(UXElement screen, bool modal, bool visibleScrim, QueueScreenBehavior subType)
		{
			ScreenBase highestLevelScreen = this.GetHighestLevelScreen<ScreenBase>();
			if (highestLevelScreen != null)
			{
				AlertScreen alertScreen = highestLevelScreen as AlertScreen;
				if (alertScreen != null && alertScreen.IsFatal)
				{
					screen.Visible = false;
					return null;
				}
			}
			ScreenBase screenBase = screen as ScreenBase;
			AlertScreen alertScreen2 = screen as AlertScreen;
			ScreenInfo screenInfo = new ScreenInfo(screen, modal, visibleScrim, subType);
			if (!this.HandleScreenQueue(screenInfo))
			{
				if (highestLevelScreen != null && highestLevelScreen.IsAlwaysOnTop && this.screens.Count > 0)
				{
					if ((screenBase != null && screenBase.IsAlwaysOnTop) || (alertScreen2 != null && alertScreen2.IsFatal))
					{
						this.screens.Add(screenInfo);
						Service.EventManager.SendEvent(EventId.NewTopScreen, null);
					}
					else
					{
						this.screens.Insert(this.screens.Count - 1, screenInfo);
					}
				}
				else
				{
					this.screens.Add(screenInfo);
					Service.EventManager.SendEvent(EventId.NewTopScreen, null);
				}
				screen.Visible = true;
				this.UpdateScrimAndDepths();
				Service.UserInputManager.ResetLastScreenPosition();
			}
			return screenInfo;
		}

		private bool HandleScreenQueue(ScreenInfo screen)
		{
			bool flag = false;
			if ((screen.QueueBehavior == QueueScreenBehavior.Queue || screen.QueueBehavior == QueueScreenBehavior.QueueAndDeferTillClosed) && (this.queuedScreens.Count > 1 || this.IsModalDialogActive()))
			{
				flag = true;
			}
			if (!flag)
			{
				flag = this.ShouldForceScreenToQueue();
			}
			if (flag)
			{
				this.AddScreenInfoToQueue(screen);
			}
			return flag;
		}

		private void AddScreenInfoToQueue(ScreenInfo screen)
		{
			screen.OnEnqueued();
			this.queuedScreens.Enqueue(screen);
			ScreenBase screenBase = screen.Screen as ScreenBase;
			if (screenBase != null)
			{
				screenBase.OnScreenAddedToQueue();
			}
		}

		private bool ShouldForceScreenToQueue()
		{
			for (int i = this.screens.Count - 1; i >= 0; i--)
			{
				if (this.screens[i].ShouldDefer())
				{
					return true;
				}
			}
			return false;
		}

		private bool ShouldDequeueScreen(ScreenInfo screen)
		{
			bool flag = !this.ShouldForceScreenToQueue();
			if (flag)
			{
				flag = (!screen.HasQueueBehavior() || !this.IsModalDialogActive());
			}
			return flag;
		}

		private void PopAndShowNextQueuedScreen()
		{
			if (this.queuedScreens.Count >= 1 && this.ShouldDequeueScreen(this.queuedScreens.Peek()))
			{
				ScreenInfo screenInfo = this.queuedScreens.Dequeue();
				screenInfo.OnDequeued();
				screenInfo = this.AddScreen(screenInfo.Screen, screenInfo.IsModal, screenInfo.VisibleScrim, screenInfo.QueueBehavior);
				if (screenInfo != null)
				{
					ScreenBase screenBase = screenInfo.Screen as ScreenBase;
					if (screenBase != null)
					{
						screenBase.OnScreeenPoppedFromQueue();
					}
					if (screenInfo.QueueBehavior == QueueScreenBehavior.Default && !screenInfo.IsModal)
					{
						this.PopAndShowNextQueuedScreen();
					}
				}
			}
		}

		public bool IsModalDialogActive()
		{
			for (int i = this.screens.Count - 1; i >= 0; i--)
			{
				if (this.screens[i].IsModal)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsFatalAlertActive()
		{
			for (int i = this.screens.Count - 1; i >= 0; i--)
			{
				AlertScreen alertScreen = this.screens[i].Screen as AlertScreen;
				if (alertScreen != null && alertScreen.IsFatal)
				{
					return true;
				}
			}
			return false;
		}

		public void AddScreen(ScreenBase screen)
		{
			this.AddScreen(screen, true);
			if (!UXUtils.ShouldShowHudBehindScreen(screen.AssetName))
			{
				Service.UXController.HUD.Visible = false;
			}
		}

		public void RemoveScreen(ScreenBase screen)
		{
			if (this.RemoveScreenHelper(screen))
			{
				this.UpdateScrimAndDepths();
			}
			this.PopAndShowNextQueuedScreen();
		}

		public void RecalculateHudVisibility()
		{
			HUD hUD = Service.UXController.HUD;
			if (!hUD.ReadyToToggleVisiblity)
			{
				return;
			}
			bool flag = true;
			for (int i = this.screens.Count - 1; i >= 0; i--)
			{
				ScreenBase screenBase = this.screens[i].Screen as ScreenBase;
				if (screenBase != null)
				{
					if (!screenBase.IsClosing)
					{
						flag = UXUtils.ShouldShowHudBehindScreen(screenBase.AssetName);
						if (!flag)
						{
							break;
						}
					}
					else
					{
						flag = !UXUtils.ShouldHideHudAfterClosingScreen(screenBase.AssetName);
						if (!flag)
						{
							break;
						}
					}
				}
			}
			if (hUD.Visible != flag)
			{
				hUD.Visible = flag;
			}
		}

		public void RecalculateCurrencyTrayVisibility()
		{
			UXController uXController = Service.UXController;
			if (uXController == null || !uXController.MiscElementsManager.IsLoaded())
			{
				return;
			}
			bool flag = false;
			for (int i = this.screens.Count - 1; i >= 0; i--)
			{
				ScreenBase screenBase = this.screens[i].Screen as ScreenBase;
				if (screenBase != null && screenBase.IsLoaded() && screenBase.ShowCurrencyTray)
				{
					flag = true;
					uXController.MiscElementsManager.DetachCurrencyTray();
					screenBase.UpdateCurrencyTrayAttachment();
					break;
				}
			}
			if (!flag)
			{
				uXController.MiscElementsManager.DetachCurrencyTray();
			}
		}

		public void LogAllScreens()
		{
			for (int i = this.screens.Count - 1; i >= 0; i--)
			{
				if (this.screens[i].Screen != null)
				{
					Service.Logger.Warn("Active Screen: " + this.screens[i].Screen.Root.name);
				}
			}
		}

		public T GetHighestLevelScreen<T>() where T : UXElement
		{
			for (int i = this.screens.Count - 1; i >= 0; i--)
			{
				if (this.screens[i].Screen is T)
				{
					return this.screens[i].Screen as T;
				}
			}
			return (T)((object)null);
		}

		public T FindElement<T>(string elementName) where T : UXElement
		{
			if (this.screens.Count == 0)
			{
				return (T)((object)null);
			}
			for (int i = this.screens.Count - 1; i >= 0; i--)
			{
				if (this.screens[i].Screen is UXFactory)
				{
					UXFactory uXFactory = this.screens[i].Screen as UXFactory;
					if (uXFactory.HasElement<T>(elementName))
					{
						return uXFactory.GetElement<T>(elementName);
					}
				}
			}
			return (T)((object)null);
		}

		public void PreloadAndCacheScreens(AssetsCompleteDelegate onComplete, object cookie)
		{
			int num = AssetConstants.GUI_PRELOADED_SCREENS.Length;
			if (num == 0)
			{
				onComplete(cookie);
				return;
			}
			List<string> list = new List<string>();
			List<object> list2 = new List<object>();
			List<AssetHandle> list3 = new List<AssetHandle>();
			for (int i = 0; i < num; i++)
			{
				string item = AssetConstants.GUI_PRELOADED_SCREENS[i];
				list.Add(item);
				list2.Add(item);
				list3.Add(AssetHandle.Invalid);
			}
			Service.AssetManager.MultiLoad(list3, list, new AssetSuccessDelegate(this.PreloadSuccess), null, list2, onComplete, cookie);
		}

		private void PreloadSuccess(object asset, object cookie)
		{
			string text = (string)cookie;
			bool flag = false;
			string[] gUI_CACHED_SCREENS = AssetConstants.GUI_CACHED_SCREENS;
			int i = 0;
			int num = gUI_CACHED_SCREENS.Length;
			while (i < num)
			{
				if (gUI_CACHED_SCREENS[i] == text)
				{
					flag = true;
					break;
				}
				i++;
			}
			if (flag)
			{
				GameObject gameObject = Service.AssetManager.CloneGameObject(asset as GameObject);
				gameObject.SetActive(false);
				this.cache.Add(text, new GameObjectContainer(gameObject));
			}
		}

		public bool LoadCachedScreen(ref AssetHandle assetHandle, string assetName, AssetSuccessDelegate onSuccess, AssetFailureDelegate onFailure, object cookie)
		{
			if (this.cache.ContainsKey(assetName))
			{
				assetHandle = AssetHandle.FirstUser;
				AssetRequest item = new AssetRequest(assetHandle, assetName, onSuccess, onFailure, cookie);
				this.loadQueue.Add(item);
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
				return true;
			}
			return false;
		}

		public bool UnloadCachedScreen(string assetName)
		{
			if (this.cache.ContainsKey(assetName))
			{
				int i = 0;
				int count = this.loadQueue.Count;
				while (i < count)
				{
					if (this.loadQueue[i].AssetName == assetName)
					{
						this.LoadQueueRemoveAt(i);
						break;
					}
					i++;
				}
				GameObjectContainer gameObjectContainer = this.cache[assetName];
				GameObject gameObj = gameObjectContainer.GameObj;
				gameObj.name = assetName;
				gameObj.SetActive(false);
				gameObjectContainer.Flagged = false;
				return true;
			}
			return false;
		}

		public void OnViewFrameTime(float dt)
		{
			int num = this.loadQueue.Count - 1;
			AssetRequest assetRequest = this.loadQueue[num];
			this.LoadQueueRemoveAt(num);
			string assetName = assetRequest.AssetName;
			GameObjectContainer gameObjectContainer = this.cache[assetName];
			GameObject gameObj = gameObjectContainer.GameObj;
			gameObj.SetActive(true);
			if (gameObjectContainer.Flagged)
			{
				Service.Logger.Error("Cannot use a cached screen multiple times: " + gameObj.name);
				if (assetRequest.OnFailure != null)
				{
					assetRequest.OnFailure(assetRequest.Cookie);
				}
			}
			else
			{
				gameObjectContainer.Flagged = true;
				if (assetRequest.OnSuccess != null)
				{
					assetRequest.OnSuccess(gameObj, assetRequest.Cookie);
				}
			}
		}

		private void LoadQueueRemoveAt(int i)
		{
			this.loadQueue.RemoveAt(i);
			if (this.loadQueue.Count == 0)
			{
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
		}

		private bool RemoveScreenHelper(UXElement screen)
		{
			int i = 0;
			int count = this.screens.Count;
			while (i < count)
			{
				ScreenInfo screenInfo = this.screens[i];
				if (screenInfo.Screen == screen)
				{
					this.RestoreDepths(i);
					this.screens.RemoveAt(i);
					if (i == this.screens.Count)
					{
						Service.EventManager.SendEvent(EventId.NewTopScreen, null);
					}
					return true;
				}
				i++;
			}
			return false;
		}

		private void UpdateScrimAndDepths()
		{
			bool flag = false;
			if (this.scrim != null)
			{
				this.RemoveScreenHelper(this.scrim.Screen);
				this.scrim = null;
			}
			for (int i = this.screens.Count - 1; i >= 0; i--)
			{
				if (this.screens[i].IsModal || this.screens[i].IsPersistentAndOpen())
				{
					UXElement screen = Service.UXController.MiscElementsManager.ShowScrim(true, this.screens[i].VisibleScrim);
					this.scrim = new ScreenInfo(screen, false);
					this.screens.Insert(i, this.scrim);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Service.UXController.MiscElementsManager.ShowScrim(false);
			}
			this.AdjustDepths();
		}

		public void AdjustDepths()
		{
			int num = 0;
			int i = 0;
			int count = this.screens.Count;
			while (i < count)
			{
				int num2 = 0;
				ScreenInfo screenInfo = this.screens[i];
				UXElement screen = screenInfo.Screen;
				if (screen.Root != null)
				{
					int depth = screenInfo.Depth;
					if (depth != num)
					{
						this.AddDepthRecursively(screen.Root, num, depth, ref num2);
						screenInfo.Depth = num;
						screenInfo.ScreenPanelThickness = num2;
					}
					else
					{
						num2 += screenInfo.ScreenPanelThickness;
					}
				}
				num += num2 + 1;
				i++;
			}
		}

		private void RestoreDepths(int i)
		{
			int num = 0;
			int num2 = 0;
			ScreenInfo screenInfo = this.screens[i];
			UXElement screen = screenInfo.Screen;
			if (screen.Root != null)
			{
				int depth = screenInfo.Depth;
				this.AddDepthRecursively(screen.Root, num, depth, ref num2);
				screenInfo.Depth = num;
			}
		}

		public void HideAll()
		{
			int i = 0;
			int count = this.screens.Count;
			while (i < count)
			{
				ScreenInfo screenInfo = this.screens[i];
				screenInfo.WasVisible = screenInfo.Screen.Visible;
				screenInfo.Screen.Visible = false;
				i++;
			}
			if (this.scrim != null)
			{
				this.scrim.WasVisible = this.scrim.Screen.Visible;
				this.scrim.Screen.Visible = false;
			}
		}

		public void RestoreVisibilityToAll()
		{
			int i = 0;
			int count = this.screens.Count;
			while (i < count)
			{
				ScreenInfo screenInfo = this.screens[i];
				screenInfo.Screen.Visible = screenInfo.WasVisible;
				i++;
			}
			this.scrim.Screen.Visible = this.scrim.WasVisible;
		}

		private bool IsScreenAutoCloseable(ScreenInfo screenInfo)
		{
			bool result = false;
			if (screenInfo.Screen is ScreenBase && !(screenInfo.Screen is HUD) && !(screenInfo.Screen is PersistentAnimatedScreen))
			{
				result = true;
			}
			return result;
		}

		public void CloseAll()
		{
			List<ScreenBase> list = new List<ScreenBase>();
			int i = 0;
			int count = this.screens.Count;
			while (i < count)
			{
				if (this.IsScreenAutoCloseable(this.screens[i]))
				{
					list.Add(this.screens[i].Screen as ScreenBase);
				}
				i++;
			}
			int count2 = this.queuedScreens.Count;
			int num = 0;
			while (num < count2 && this.queuedScreens.Count > 0)
			{
				ScreenInfo screenInfo = this.queuedScreens.Dequeue();
				if (this.IsScreenAutoCloseable(screenInfo))
				{
					list.Add(screenInfo.Screen as ScreenBase);
				}
				num++;
			}
			for (int j = list.Count - 1; j >= 0; j--)
			{
				list[j].CloseNoTransition(null);
			}
		}

		private void AddDepthRecursively(GameObject gameObject, int newDepth, int restoreDepth, ref int maxDepth)
		{
			UIPanel component = gameObject.GetComponent<UIPanel>();
			if (component != null)
			{
				int num = component.depth - restoreDepth;
				if (num > maxDepth)
				{
					maxDepth = num;
				}
				component.depth += newDepth - restoreDepth;
			}
			Transform transform = gameObject.transform;
			int i = 0;
			int childCount = transform.childCount;
			while (i < childCount)
			{
				this.AddDepthRecursively(transform.GetChild(i).gameObject, newDepth, restoreDepth, ref maxDepth);
				i++;
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.UpdateScrim)
			{
				this.UpdateScrimAndDepths();
			}
			return EatResponse.NotEaten;
		}
	}
}
