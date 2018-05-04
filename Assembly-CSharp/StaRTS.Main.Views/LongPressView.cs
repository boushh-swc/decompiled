using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views
{
	public class LongPressView : IViewFrameTimeObserver
	{
		private const string LONG_PRESS_BKG = "SpriteLongPressBkg";

		private const string LONG_PRESS_FILL = "SpriteLongPressFill";

		private float preFadeDuration;

		private float fadeDuration;

		private float age;

		private UXElement longPress;

		private UXSprite longPressFill;

		private UXSprite longPressBkg;

		private bool started;

		private CameraManager cams;

		private UXController uxc;

		private ViewTimeEngine time;

		private bool registered;

		public LongPressView()
		{
			this.cams = Service.CameraManager;
			this.time = Service.ViewTimeEngine;
			this.uxc = Service.UXController;
			this.preFadeDuration = GameConstants.EDIT_LONG_PRESS_PRE_FADE;
			this.fadeDuration = GameConstants.EDIT_LONG_PRESS_FADE;
			this.longPress = this.uxc.MiscElementsManager.CreateLongPress("LongPress", this.uxc.WorldAnchor);
			this.longPress.Visible = false;
			this.longPressFill = this.uxc.MiscElementsManager.GetChildElementSprite(this.longPress, "SpriteLongPressFill");
			this.longPressBkg = this.uxc.MiscElementsManager.GetChildElementSprite(this.longPress, "SpriteLongPressBkg");
			this.registered = false;
		}

		public void StartTimer(Entity target)
		{
			Vector3 position = target.Get<GameObjectViewComponent>().MainTransform.position;
			this.StartTimer(position);
		}

		public void StartTimer(Vector3 worldPosition)
		{
			this.age = 0f;
			this.longPressFill.FillAmount = 0f;
			this.longPressFill.Alpha = 0f;
			this.longPressBkg.Alpha = 0f;
			if (!this.registered)
			{
				this.time.RegisterFrameTimeObserver(this);
				this.registered = true;
			}
			worldPosition.y = 0f;
			Vector3 vector = this.cams.MainCamera.WorldPositionToScreenPoint(worldPosition);
			this.longPress.LocalPosition = new Vector3(vector.x, vector.y, 0f);
			this.longPress.WidgetDepth = this.uxc.ComputeDepth(worldPosition);
			this.longPress.Visible = true;
		}

		public void KillTimer()
		{
			if (this.registered)
			{
				this.time.UnregisterFrameTimeObserver(this);
				this.longPress.Visible = false;
				this.registered = false;
				this.started = false;
			}
		}

		public void OnViewFrameTime(float dt)
		{
			this.age += dt;
			if (this.age < this.preFadeDuration)
			{
				return;
			}
			if (!this.started)
			{
				Service.EventManager.SendEvent(EventId.LongPressStarted, null);
				this.started = true;
			}
			if (this.age > this.preFadeDuration + this.fadeDuration)
			{
				this.UpdateFade(1f);
			}
			else
			{
				this.UpdateFade((this.age - this.preFadeDuration) / this.fadeDuration);
			}
			float proportion = (this.age - this.preFadeDuration) / (1f - this.preFadeDuration);
			this.UpdateFillAmount(proportion);
		}

		private void UpdateFillAmount(float proportion)
		{
			this.longPressFill.FillAmount = proportion;
		}

		private void UpdateFade(float proportion)
		{
			this.longPressFill.Alpha = proportion;
			this.longPressBkg.Alpha = proportion;
		}
	}
}
