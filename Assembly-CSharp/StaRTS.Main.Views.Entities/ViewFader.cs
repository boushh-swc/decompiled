using Net.RichardLord.Ash.Core;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.Entities
{
	public class ViewFader : IViewFrameTimeObserver
	{
		private List<AbstractFadingView> fadingEntities;

		private List<AbstractFadingView> completedFaders;

		public ViewFader()
		{
			this.fadingEntities = new List<AbstractFadingView>();
			this.completedFaders = new List<AbstractFadingView>();
		}

		public void FadeOut(Entity entity, float delay, float fadeTime, FadingDelegate onFadeStart, FadingDelegate onFadeComplete)
		{
			this.MaybeRegisterForViewTime();
			FadingEntity item = new FadingEntity(entity, delay, fadeTime, onFadeStart, onFadeComplete);
			this.fadingEntities.Add(item);
		}

		public void FadeOut(GameObject gameObj, float delay, float fadeTime, FadingDelegate onFadeStart, FadingDelegate onFadeComplete)
		{
			this.MaybeRegisterForViewTime();
			FadingGameObject item = new FadingGameObject(gameObj, delay, fadeTime, onFadeStart, onFadeComplete);
			this.fadingEntities.Add(item);
		}

		private void MaybeRegisterForViewTime()
		{
			if (this.fadingEntities.Count == 0)
			{
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			}
		}

		private void MaybeUnregisterFromViewTime()
		{
			if (this.fadingEntities.Count == 0)
			{
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
		}

		public void OnViewFrameTime(float dt)
		{
			int num = this.fadingEntities.Count;
			if (num != 0)
			{
				for (int i = 0; i < num; i++)
				{
					AbstractFadingView abstractFadingView = this.fadingEntities[i];
					if (abstractFadingView.Fade(dt))
					{
						this.fadingEntities.RemoveAt(i);
						i--;
						num--;
						this.completedFaders.Add(abstractFadingView);
					}
				}
				num = this.completedFaders.Count;
				if (num != 0)
				{
					this.MaybeUnregisterFromViewTime();
					for (int j = 0; j < num; j++)
					{
						this.completedFaders[j].Complete();
					}
					this.completedFaders.Clear();
				}
			}
		}
	}
}
