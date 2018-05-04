using StaRTS.Utils.Animation.Anims;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Utils.Animation
{
	public class AnimController : IViewFrameTimeObserver
	{
		private List<Anim> anims;

		public AnimController()
		{
			Service.AnimController = this;
			this.anims = new List<Anim>();
		}

		public void Animate(Anim anim)
		{
			this.CompleteAnim(anim);
			if (anim.Delay > 0f)
			{
				anim.DelayTimer = Service.ViewTimerManager.CreateViewTimer(anim.Delay, false, new TimerDelegate(this.OnDelayComplete), anim);
			}
			else
			{
				anim.DelayTimer = 0u;
				this.InternalAnimate(anim);
			}
		}

		private void InternalAnimate(Anim anim)
		{
			this.anims.Add(anim);
			if (this.anims.Count == 1)
			{
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			}
			anim.Begin();
		}

		private void OnDelayComplete(uint id, object cookie)
		{
			Anim anim = cookie as Anim;
			anim.DelayTimer = 0u;
			this.InternalAnimate(anim);
		}

		private void CancelDelayedAnim(Anim anim)
		{
			Service.ViewTimerManager.KillViewTimer(anim.DelayTimer);
			anim.DelayTimer = 0u;
		}

		private void InternalCompleteAnim(Anim anim)
		{
			anim.Complete();
			if (this.anims.Count == 0)
			{
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
		}

		public void CompleteAnim(Anim anim)
		{
			if (anim.DelayTimer != 0u)
			{
				this.CancelDelayedAnim(anim);
			}
			if (anim.Playing)
			{
				this.anims.Remove(anim);
				this.InternalCompleteAnim(anim);
			}
		}

		private void CompleteAnimWithIndex(Anim anim, int index)
		{
			if (anim.DelayTimer != 0u)
			{
				this.CancelDelayedAnim(anim);
			}
			if (anim.Playing)
			{
				this.anims.RemoveAt(index);
				this.InternalCompleteAnim(anim);
			}
		}

		public void CompleteAndRemoveAnim(Anim anim)
		{
			this.CompleteAnim(anim);
			this.anims.Remove(anim);
			anim.Cleanup();
		}

		public void OnViewFrameTime(float dt)
		{
			int count = this.anims.Count;
			while (count-- != 0)
			{
				Anim anim = this.anims[count];
				anim.Tick(dt);
				bool flag = anim.Age >= anim.Duration;
				if (flag)
				{
					anim.Age = anim.Duration;
				}
				anim.Update(dt);
				if (flag)
				{
					this.CompleteAnimWithIndex(anim, count);
				}
			}
		}
	}
}
