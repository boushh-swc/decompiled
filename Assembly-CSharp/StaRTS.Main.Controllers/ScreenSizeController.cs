using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class ScreenSizeController : IViewFrameTimeObserver
	{
		private float screenWidth;

		private float screenHeight;

		private bool resetGeometry;

		public float ScreenWidth
		{
			get
			{
				return this.screenWidth;
			}
		}

		public float ScreenHeight
		{
			get
			{
				return this.screenHeight;
			}
		}

		public ScreenSizeController()
		{
			Service.ScreenSizeController = this;
			this.screenWidth = (float)Screen.width;
			this.screenHeight = (float)Screen.height;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		public void OnViewFrameTime(float dt)
		{
			float num = (float)Screen.width;
			float num2 = (float)Screen.height;
			bool flag = num2 != this.screenHeight || num != this.screenWidth;
			if (flag)
			{
				Vector2 vector = new Vector2(num, num2);
				Service.EventManager.SendEvent(EventId.ScreenSizeChanged, vector);
				this.resetGeometry = true;
				this.screenWidth = num;
				this.screenHeight = num2;
			}
			else if (this.resetGeometry)
			{
				this.ResetGeometry();
				this.resetGeometry = false;
			}
		}

		public void ResetGeometry()
		{
			StoreScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<StoreScreen>();
			if (highestLevelScreen != null)
			{
				highestLevelScreen.ResetCurrentTab();
			}
			Service.EventManager.SendEvent(EventId.ForceGeometryReload, null);
		}
	}
}
