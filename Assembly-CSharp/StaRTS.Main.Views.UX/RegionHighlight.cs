using StaRTS.Main.Utils;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX
{
	public class RegionHighlight : IViewFrameTimeObserver
	{
		private const float SHRINK_DURATION = 0.3f;

		private const float SHRINK_SIZE = 0.7f;

		private UXElement buttonHighlight;

		private Vector3 top;

		private Vector3 bottom;

		private Vector3 left;

		private Vector3 right;

		private Vector3 center;

		private MainCamera cam;

		private float screenWidth;

		private float screenHeight;

		private float age;

		private bool haveStartedAnim;

		public RegionHighlight(UXElement highlight, float boardX, float boardZ, int width, int depth)
		{
			this.buttonHighlight = highlight;
			this.buttonHighlight.Parent = Service.UXController.WorldAnchor;
			UnityUtils.SetLayerRecursively(highlight.Root, 8);
			this.buttonHighlight.UXCamera = Service.CameraManager.UXCamera;
			float num = (float)depth / 2f;
			float num2 = (float)width / 2f;
			this.top = new Vector3(Units.BoardToWorldX(boardX - num2), 0f, Units.BoardToWorldZ(boardZ - num));
			this.bottom = new Vector3(Units.BoardToWorldX(boardX + num2), 0f, Units.BoardToWorldZ(boardZ + num));
			this.left = new Vector3(Units.BoardToWorldX(boardX - num2), 0f, Units.BoardToWorldZ(boardZ + num));
			this.right = new Vector3(Units.BoardToWorldX(boardX + num2), 0f, Units.BoardToWorldZ(boardZ - num));
			this.center = (this.left + this.right) / 2f;
			this.cam = Service.CameraManager.MainCamera;
			this.age = 0f;
			this.UpdateHighlightPosition(0f);
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			this.buttonHighlight.Visible = true;
		}

		private void UpdateHighlightPosition(float dt)
		{
			if (!this.haveStartedAnim)
			{
				this.haveStartedAnim = true;
				Animator component = this.buttonHighlight.Root.GetComponent<Animator>();
				component.SetTrigger("Show");
			}
			float num = 1f;
			this.age += dt;
			if (this.age < 0.3f)
			{
				float num2 = 1f - this.age / 0.3f;
				num = 1f + 0.7f * num2;
			}
			Vector3 vector = this.cam.WorldPositionToScreenPoint(this.left);
			Vector3 vector2 = this.cam.WorldPositionToScreenPoint(this.right);
			Vector3 vector3 = this.cam.WorldPositionToScreenPoint(this.top);
			Vector3 vector4 = this.cam.WorldPositionToScreenPoint(this.bottom);
			this.screenWidth = vector2.x - vector.x;
			this.screenHeight = vector4.y - vector3.y;
			this.buttonHighlight.LocalPosition = this.cam.WorldPositionToScreenPoint(this.center);
			this.buttonHighlight.Width = this.screenWidth * num;
			this.buttonHighlight.Height = this.screenHeight * num;
			this.buttonHighlight.Visible = true;
		}

		public void OnViewFrameTime(float dt)
		{
			this.UpdateHighlightPosition(dt);
		}

		public void Destroy()
		{
			this.buttonHighlight = null;
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
		}
	}
}
