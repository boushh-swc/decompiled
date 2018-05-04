using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Audio
{
	public class AudioFader : IViewFrameTimeObserver
	{
		private float originalVolume;

		private float age;

		private float constantTime;

		private AudioSource source;

		private AudioTypeVO nextType;

		public AudioFader(AudioSource source)
		{
			if (source == null)
			{
				Service.Logger.Warn("Source is null");
			}
			this.constantTime = GameConstants.FADE_OUT_CONSTANT_LENGTH;
			this.originalVolume = source.volume;
			this.source = source;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		public void FadeSound(float age)
		{
			if (this.source == null)
			{
				this.UnregisterFrameTimeObserver();
				return;
			}
			if (age <= this.constantTime)
			{
				float t = age / this.constantTime;
				this.source.volume = Mathf.Lerp(this.originalVolume, 0f, t);
			}
			else
			{
				this.source.Stop();
				this.source.volume = this.originalVolume;
				this.FadeOutComplete();
			}
		}

		public void QueueNextAudio(AudioTypeVO type)
		{
			this.nextType = type;
		}

		public void UnregisterFrameTimeObserver()
		{
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
		}

		public void FadeOutComplete()
		{
			AudioManager audioManager = Service.AudioManager;
			this.UnregisterFrameTimeObserver();
			audioManager.FadeOutComplete(this.source);
			if (this.nextType != null)
			{
				audioManager.PlayAudio(this.nextType.AssetName);
			}
			this.source = null;
			this.nextType = null;
		}

		public void OnViewFrameTime(float dt)
		{
			this.age += dt;
			this.FadeSound(this.age);
		}
	}
}
