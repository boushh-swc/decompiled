using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Squads
{
	public abstract class AbstractSquadWarBoardInfoElement : AbstractSquadWarBoardElement, IEventObserver
	{
		private const int WIDGET_DEPTH = -1;

		private const float VISIBILITY_FADE_START = 14f;

		private const float VISIBILITY_FADE_END = 18f;

		public AbstractSquadWarBoardInfoElement(string assetName, Transform transformToTrack) : base(assetName)
		{
			this.transformToTrack = transformToTrack;
		}

		protected abstract void SetupView();

		protected abstract void UpdateView();

		protected override void OnScreenLoaded(object cookie)
		{
			base.OnScreenLoaded(cookie);
			base.WidgetDepth = -1;
			this.SetupView();
			this.UpdateVisibility();
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			this.OnViewFrameTime(0f);
		}

		public void UpdateVisibility()
		{
			float num = this.transformToTrack.position.z;
			num = Mathf.Clamp(num, 14f, 18f);
			float proportion = 1f - (num - 14f) / 4f;
			this.FadeElements(proportion);
		}

		protected virtual void FadeElements(float proportion)
		{
		}

		public override void Destroy()
		{
			base.Destroy();
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
		}
	}
}
