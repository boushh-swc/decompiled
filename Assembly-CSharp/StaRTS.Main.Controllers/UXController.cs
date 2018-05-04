using StaRTS.Assets;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Anchors;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class UXController
	{
		public const int WORLD_DEPTH = -1;

		public const int UI_DEPTH = 0;

		public const int BUTTON_HIGHLIGHT_DEPTH = 9999;

		public const int DEBUG_CURSOR_DEPTH = 10000;

		public const int SCREEN_SINK_DEPTH = 10001;

		public const float WORLD_DEPTH_GRANULARITY = 10000f;

		private const string WORLD_UI_PANEL_NAME = "WorldUIPanel";

		private int waitingFor;

		public HUD HUD
		{
			get;
			private set;
		}

		public IntroCameraAnimation Intro
		{
			get;
			set;
		}

		public MiscElementsManager MiscElementsManager
		{
			get;
			private set;
		}

		public UXAnchor PerformanceAnchor
		{
			get;
			private set;
		}

		public UXAnchor WorldAnchor
		{
			get;
			private set;
		}

		public GameObject WorldUIParent
		{
			get;
			private set;
		}

		public UXController()
		{
			Service.UXController = this;
			this.HUD = null;
			this.PerformanceAnchor = new UXPerformanceAnchor();
			this.WorldAnchor = new UXWorldAnchor();
			this.WorldUIParent = UnityUtils.CreateChildGameObject("WorldUIPanel", this.WorldAnchor.Root);
			UIPanel uIPanel = this.WorldUIParent.AddComponent<UIPanel>();
			uIPanel.depth = -1;
			this.waitingFor = 1;
			this.MiscElementsManager = new MiscElementsManager(new AssetsCompleteDelegate(this.OnManagerComplete), null);
			if (Service.CurrentPlayer.HasNotCompletedFirstFueStep())
			{
				this.waitingFor++;
				this.Intro = new IntroCameraAnimation(new AssetsCompleteDelegate(this.OnManagerComplete), null);
			}
		}

		private void OnManagerComplete(object cookie)
		{
			if (--this.waitingFor == 0)
			{
				this.OnSharedElementsLoaded();
			}
		}

		private void OnSharedElementsLoaded()
		{
			this.HUD = new HUD();
			Service.ScreenController.AddScreen(this.HUD, false);
			this.HUD.Visible = false;
		}

		public void HideAll()
		{
			Service.ScreenController.HideAll();
			if (this.PerformanceAnchor != null)
			{
				this.PerformanceAnchor.Visible = false;
			}
			if (this.WorldAnchor != null)
			{
				this.WorldAnchor.Visible = false;
			}
		}

		public void RestoreVisibilityToAll()
		{
			Service.ScreenController.RestoreVisibilityToAll();
			if (this.PerformanceAnchor != null)
			{
				this.PerformanceAnchor.Visible = true;
			}
			if (this.WorldAnchor != null)
			{
				this.WorldAnchor.Visible = true;
			}
		}

		public int ComputeDepth(Vector3 worldPosition)
		{
			float num = worldPosition.x + worldPosition.z;
			float num2 = num - -276f;
			float num3 = 552f;
			return -1 - (int)(10000f * num2 / num3);
		}
	}
}
