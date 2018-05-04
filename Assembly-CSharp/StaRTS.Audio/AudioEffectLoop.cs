using StaRTS.Main.Models;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Audio
{
	public class AudioEffectLoop : IEventObserver, IViewFrameTimeObserver
	{
		private float duration;

		private List<StrIntPair> effectIds;

		public AudioEffectLoop(float duration, List<StrIntPair> effectIds)
		{
			if (duration <= 0f)
			{
				return;
			}
			this.duration = duration;
			this.effectIds = effectIds;
			this.PlayEffect();
		}

		private void PlayEffect()
		{
			AudioManager audioManager = Service.AudioManager;
			float num = audioManager.PlayAudio(audioManager.GetRandomClip(this.effectIds));
			if (num == -1f)
			{
				Service.EventManager.RegisterObserver(this, EventId.PlayedLoadedOnDemandAudio, EventPriority.Default);
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			}
			else
			{
				this.OnEffectPlayed(num);
			}
		}

		private void OnEffectPlayed(float clipLength)
		{
			if (clipLength == 0f)
			{
				return;
			}
			this.duration -= clipLength;
			if (this.duration > 0f)
			{
				Service.ViewTimerManager.CreateViewTimer(clipLength, false, new TimerDelegate(this.OnAudioEffectLoopTimer), null);
			}
		}

		private void OnAudioEffectLoopTimer(uint id, object cookie)
		{
			this.PlayEffect();
		}

		public void OnViewFrameTime(float dt)
		{
			this.duration -= dt;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.PlayedLoadedOnDemandAudio)
			{
				Service.EventManager.UnregisterObserver(this, id);
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
				float clipLength = (float)cookie;
				this.OnEffectPlayed(clipLength);
			}
			return EatResponse.NotEaten;
		}
	}
}
