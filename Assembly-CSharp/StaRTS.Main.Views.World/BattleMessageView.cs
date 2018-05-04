using StaRTS.Main.Controllers;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Anchors;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.World
{
	public class BattleMessageView : IViewFrameTimeObserver
	{
		private const float HEIGHT_OFFSET = 5f;

		private const float DELAY = 1.25f;

		private const float FADE_TIME = 0.5f;

		private UXLabel label;

		private float curTime;

		private bool registered;

		public BattleMessageView(string labelName)
		{
			UXController uXController = Service.UXController;
			UXAnchor worldAnchor = uXController.WorldAnchor;
			this.label = uXController.MiscElementsManager.CreateGameBoardLabel(labelName, worldAnchor);
			this.label.Pivot = UIWidget.Pivot.Bottom;
			this.label.Visible = false;
			this.registered = false;
		}

		public void Show(Vector3 worldPosition, bool error, string message)
		{
			worldPosition.y = 5f;
			Vector3 vector = Service.CameraManager.MainCamera.WorldPositionToScreenPoint(worldPosition);
			this.label.LocalPosition = new Vector3(vector.x, vector.y, 0f);
			this.label.WidgetDepth = Service.UXController.ComputeDepth(worldPosition);
			this.label.Visible = true;
			this.label.Text = message;
			this.label.TextColor = UXUtils.GetCostColor(this.label, !error, false);
			this.curTime = 0f;
			if (!this.registered)
			{
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
				this.registered = true;
			}
		}

		public void HideImmediately()
		{
			if (this.curTime - 1.25f < 0f)
			{
				this.curTime = 0.5f;
			}
		}

		public void OnViewFrameTime(float dt)
		{
			this.curTime += dt;
			float num = this.curTime - 1.25f;
			if (num < 0f)
			{
				return;
			}
			bool flag = num >= 0.5f;
			num = ((!flag) ? (1f - num / 0.5f) : 0f);
			this.label.TextColor = new Color(this.label.TextColor.r, this.label.TextColor.g, this.label.TextColor.b, num);
			if (flag)
			{
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
				this.label.Visible = false;
				this.registered = false;
			}
		}
	}
}
