using StaRTS.Assets;
using StaRTS.Externals.MobileConnectorAds;
using StaRTS.Main.Configs;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using StaRTS.Utils.Diagnostics;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Audio
{
	public class AudioManager
	{
		public const float WILL_PLAY_AFTER_LOAD = -1f;

		public const bool BYPASS_EFFECTS = true;

		public const bool BYPASS_LISTENER_EFFECTS = true;

		public const bool BYPASS_REVERB_ZONES = true;

		private const string AUDIO_OBJECT_NAME = "Audio";

		private const int MUSIC_PRIORITY = 0;

		private const int AMBIENCE_PRIORITY = 10;

		private const int DIALOGUE_PRIORITY = 20;

		private const int EFFECTS_PRIORITY = 128;

		private GameObject audioObject;

		private Dictionary<AudioCategory, AudioSource> audioSources;

		private Dictionary<AudioTypeVO, AudioData> loadedAudio;

		private Logger logger;

		private StaticDataController sdc;

		private AudioTypeVO[] lastPlayed;

		private bool[] battleAudioFlags;

		public Dictionary<AudioSource, AudioFader> fadeOutDictionary;

		private Dictionary<string, uint[]> audioRepeatTimers;

		public AudioManager(AssetsCompleteDelegate onComplete)
		{
			Service.AudioManager = this;
			this.logger = Service.Logger;
			this.sdc = Service.StaticDataController;
			new AudioEventManager(this);
			this.audioObject = new GameObject("Audio");
			this.audioObject.AddComponent<AudioListener>();
			this.audioObject.transform.position = Vector3.zero;
			this.audioSources = new Dictionary<AudioCategory, AudioSource>();
			this.audioSources[AudioCategory.Effect] = this.CreateAudioSource(128);
			this.audioSources[AudioCategory.Music] = this.CreateAudioSource(0);
			this.audioSources[AudioCategory.Ambience] = this.CreateAudioSource(10);
			this.audioSources[AudioCategory.Dialogue] = this.CreateAudioSource(20);
			this.loadedAudio = new Dictionary<AudioTypeVO, AudioData>();
			this.fadeOutDictionary = new Dictionary<AudioSource, AudioFader>();
			this.battleAudioFlags = new bool[8];
			this.lastPlayed = new AudioTypeVO[4];
			this.RefreshVolume();
			this.PreloadAudioAssets(onComplete);
		}

		public void CleanUp()
		{
			foreach (AudioFader current in this.fadeOutDictionary.Values)
			{
				current.UnregisterFrameTimeObserver();
			}
			foreach (AudioSource current2 in this.audioSources.Values)
			{
				current2.Stop();
			}
			this.ClearAudioRepeatTimers();
		}

		private AudioSource CreateAudioSource(int priority)
		{
			AudioSource audioSource = this.audioObject.AddComponent<AudioSource>();
			audioSource.bypassEffects = true;
			audioSource.bypassListenerEffects = true;
			audioSource.bypassReverbZones = true;
			audioSource.pitch = 1f;
			audioSource.priority = priority;
			return audioSource;
		}

		private bool IsPreloadable(AudioTypeVO vo)
		{
			AssetTypeVO optional = this.sdc.GetOptional<AssetTypeVO>("asset_" + vo.AssetName);
			return optional != null && optional.Category == AssetCategory.PreloadStandard;
		}

		private void PreloadAudioAssets(AssetsCompleteDelegate onComplete)
		{
			List<string> list = new List<string>();
			List<object> list2 = new List<object>();
			List<AssetHandle> list3 = new List<AssetHandle>();
			foreach (AudioTypeVO current in this.sdc.GetAll<AudioTypeVO>())
			{
				if (this.IsPreloadable(current) && !this.audioSources[current.Category].mute)
				{
					list.Add(current.AssetName);
					list2.Add(current);
					list3.Add(AssetHandle.Invalid);
				}
			}
			if (list.Count == 0)
			{
				if (onComplete != null)
				{
					onComplete(null);
				}
			}
			else
			{
				Service.AssetManager.MultiLoad(list3, list, new AssetSuccessDelegate(this.PreloadAudioAssetSuccess), null, list2, onComplete, null);
				int i = 0;
				int count = list3.Count;
				while (i < count)
				{
					AudioTypeVO vo = (AudioTypeVO)list2[i];
					AudioData orCreateAudioData = this.GetOrCreateAudioData(vo);
					orCreateAudioData.Handle = list3[i];
					i++;
				}
			}
		}

		private AudioData GetOrCreateAudioData(AudioTypeVO vo)
		{
			if (!this.loadedAudio.ContainsKey(vo))
			{
				this.loadedAudio[vo] = new AudioData();
			}
			return this.loadedAudio[vo];
		}

		private AudioData GetAudioData(AudioTypeVO vo)
		{
			if (this.loadedAudio.ContainsKey(vo))
			{
				return this.loadedAudio[vo];
			}
			Service.Logger.Warn("AudioData for " + vo.Uid + " does not exist.");
			return null;
		}

		private void AssignLoadedAudioClip(AudioTypeVO audioVO, AudioClip clip)
		{
			AudioData orCreateAudioData = this.GetOrCreateAudioData(audioVO);
			if (orCreateAudioData != null && clip != null)
			{
				orCreateAudioData.Clip = clip;
			}
		}

		private void PreloadAudioAssetSuccess(object asset, object cookie)
		{
			AudioTypeVO audioTypeVO = (AudioTypeVO)cookie;
			this.AssignLoadedAudioClip(audioTypeVO, asset as AudioClip);
			Service.EventManager.SendEvent(EventId.PreloadedAudioSuccess, audioTypeVO);
		}

		private void PreloadAudioAssetFailure(object cookie)
		{
			AudioTypeVO cookie2 = (AudioTypeVO)cookie;
			Service.EventManager.SendEvent(EventId.PreloadedAudioFailure, cookie2);
		}

		private void OnDemandAudioAssetSuccess(object asset, object cookie)
		{
			AudioTypeVO audioVO = (AudioTypeVO)cookie;
			AudioClip clip = asset as AudioClip;
			this.AssignLoadedAudioClip(audioVO, clip);
			float num = this.PlayAudioClip(audioVO);
			Service.EventManager.SendEvent(EventId.PlayedLoadedOnDemandAudio, num);
		}

		public string GetRandomClip(List<StrIntPair> clips)
		{
			if (clips == null || clips.Count == 0)
			{
				return null;
			}
			int num = 0;
			int count = clips.Count;
			for (int i = 0; i < count; i++)
			{
				num += clips[i].IntVal;
			}
			if (clips == null)
			{
				return null;
			}
			int num2 = Service.Rand.ViewRangeInt(0, num);
			string result = string.Empty;
			for (int j = 0; j < count; j++)
			{
				StrIntPair strIntPair = clips[j];
				int intVal = strIntPair.IntVal;
				if (num2 < intVal)
				{
					result = strIntPair.StrKey;
					break;
				}
				num2 -= intVal;
			}
			return result;
		}

		public float PlayAudio(string id)
		{
			if (!this.CanLoadAudio(id))
			{
				return 0f;
			}
			AudioTypeVO optional = this.sdc.GetOptional<AudioTypeVO>(id);
			AudioData orCreateAudioData = this.GetOrCreateAudioData(optional);
			if (orCreateAudioData != null && orCreateAudioData.Clip == null)
			{
				if (orCreateAudioData.Handle == AssetHandle.Invalid)
				{
					AssetHandle handle = AssetHandle.Invalid;
					Service.AssetManager.Load(ref handle, optional.AssetName, new AssetSuccessDelegate(this.OnDemandAudioAssetSuccess), null, optional);
					orCreateAudioData.Handle = handle;
				}
				return -1f;
			}
			return this.PlayAudioClip(optional);
		}

		public void PlayAudioRepeat(string id, int repeatCount, float delay, float interval)
		{
			if (repeatCount == 0)
			{
				this.logger.WarnFormat("Trying to play audio {0} with repeat count = 0, use PlayAudio", new object[]
				{
					id
				});
				return;
			}
			uint[] array = new uint[repeatCount];
			ViewTimerManager viewTimerManager = Service.ViewTimerManager;
			for (int i = 0; i < repeatCount; i++)
			{
				float delay2 = delay + (float)(i + 1) * interval;
				array[i] = viewTimerManager.CreateViewTimer(delay2, false, new TimerDelegate(this.PlayAudioDelayedCallback), id);
			}
			this.SetAudioRepeatTimers(id, array);
		}

		public void PlayAudioDelayed(string id, float delay)
		{
			this.SetAudioRepeatTimers(id, new uint[]
			{
				Service.ViewTimerManager.CreateViewTimer(delay, false, new TimerDelegate(this.PlayAudioDelayedCallback), id)
			});
		}

		private void PlayAudioDelayedCallback(uint id, object cookie)
		{
			this.PlayAudio((string)cookie);
		}

		public void SetAudioRepeatTimers(string id, uint[] timerIds)
		{
			if (id == null || timerIds == null)
			{
				return;
			}
			if (this.audioRepeatTimers == null)
			{
				this.audioRepeatTimers = new Dictionary<string, uint[]>();
			}
			this.audioRepeatTimers[id] = timerIds;
		}

		public void ClearAudioRepeatTimers()
		{
			if (this.audioRepeatTimers != null)
			{
				ViewTimerManager viewTimerManager = Service.ViewTimerManager;
				foreach (string current in this.audioRepeatTimers.Keys)
				{
					uint[] array = this.audioRepeatTimers[current];
					if (array != null)
					{
						int i = 0;
						int num = array.Length;
						while (i < num)
						{
							if (viewTimerManager != null)
							{
								viewTimerManager.KillViewTimer(array[i]);
							}
							array[i] = 0u;
							i++;
						}
					}
				}
				this.audioRepeatTimers.Clear();
				this.audioRepeatTimers = null;
			}
		}

		public bool LoadAudio(string id)
		{
			if (!this.CanLoadAudio(id))
			{
				return false;
			}
			AudioTypeVO optional = this.sdc.GetOptional<AudioTypeVO>(id);
			AudioData orCreateAudioData = this.GetOrCreateAudioData(optional);
			if (orCreateAudioData != null && orCreateAudioData.Clip == null && orCreateAudioData.Handle == AssetHandle.Invalid)
			{
				AssetHandle handle = AssetHandle.Invalid;
				Service.AssetManager.Load(ref handle, optional.AssetName, new AssetSuccessDelegate(this.PreloadAudioAssetSuccess), new AssetFailureDelegate(this.PreloadAudioAssetFailure), optional);
				orCreateAudioData.Handle = handle;
				return true;
			}
			return false;
		}

		private bool CanLoadAudio(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return false;
			}
			AudioTypeVO optional = this.sdc.GetOptional<AudioTypeVO>(id);
			if (optional == null)
			{
				this.logger.WarnFormat("Audio key not found: {0}", new object[]
				{
					id
				});
				return false;
			}
			return !this.audioSources[optional.Category].mute;
		}

		private float PlayAudioClip(AudioTypeVO audioVO)
		{
			if (audioVO.Category != AudioCategory.Effect)
			{
				AudioTypeVO audioTypeVO = this.lastPlayed[(int)audioVO.Category];
				if (audioTypeVO != audioVO)
				{
					this.UnloadLastPlayed(audioVO.Category);
				}
				else if (this.IsPlaying(audioVO.Category))
				{
					return 0f;
				}
			}
			AudioCategory category = audioVO.Category;
			AudioData audioData = this.GetAudioData(audioVO);
			if (audioData == null)
			{
				return 0f;
			}
			this.lastPlayed[(int)audioVO.Category] = audioVO;
			AudioSource audioSource = this.audioSources[category];
			switch (category)
			{
			case AudioCategory.Dialogue:
			case AudioCategory.Music:
			case AudioCategory.Ambience:
				if (this.fadeOutDictionary.ContainsKey(audioSource))
				{
					this.fadeOutDictionary[audioSource].QueueNextAudio(audioVO);
				}
				else
				{
					audioSource.clip = audioData.Clip;
					audioSource.loop = audioVO.Loop;
					audioSource.Play();
				}
				goto IL_F9;
			}
			audioSource.PlayOneShot(audioData.Clip);
			IL_F9:
			return audioData.Clip.length;
		}

		public bool IsPlaying(AudioCategory category)
		{
			return this.audioSources[category].isPlaying;
		}

		public bool IsPlaying(AudioCategory category, string id)
		{
			AudioTypeVO audioTypeVO = this.lastPlayed[(int)category];
			return this.audioSources[category].isPlaying && audioTypeVO != null && audioTypeVO.Uid == id;
		}

		public float GetClipLength(AudioCategory category)
		{
			if (this.audioSources[category].clip != null)
			{
				return this.audioSources[category].clip.length;
			}
			return 0f;
		}

		public void Stop(AudioCategory category)
		{
			if (category == AudioCategory.Ambience || category == AudioCategory.Music)
			{
				this.FadeOut(this.audioSources[category]);
			}
			else
			{
				this.audioSources[category].Stop();
			}
			this.UnloadLastPlayed(category);
		}

		public void Stop(AudioCategory category1, AudioCategory category2)
		{
			this.Stop(category1);
			this.Stop(category2);
		}

		public void FadeOutComplete(AudioSource source)
		{
			if (this.fadeOutDictionary.ContainsKey(source))
			{
				this.fadeOutDictionary.Remove(source);
			}
		}

		private void FadeOut(AudioSource source)
		{
			if (!this.fadeOutDictionary.ContainsKey(source))
			{
				this.fadeOutDictionary.Add(source, new AudioFader(source));
			}
		}

		private void UnloadLastPlayed(AudioCategory category)
		{
			AudioTypeVO audioTypeVO = this.lastPlayed[(int)category];
			if (audioTypeVO != null)
			{
				AudioData audioData = this.GetAudioData(audioTypeVO);
				this.UnloadAudio(audioData);
				this.loadedAudio.Remove(audioTypeVO);
				this.lastPlayed[(int)category] = null;
			}
		}

		private void UnloadAudio(AudioData data)
		{
			if (data != null && data.Handle != AssetHandle.Invalid)
			{
				Service.AssetManager.Unload(data.Handle);
				data.Handle = AssetHandle.Invalid;
				data.Clip = null;
			}
		}

		public void RefreshMusic()
		{
			float musicVolume = PlayerSettings.GetMusicVolume();
			this.SetVolume(AudioCategory.Music, musicVolume);
		}

		public void RefreshVolume()
		{
			float sfxVolume = PlayerSettings.GetSfxVolume();
			this.SetVolume(AudioCategory.Effect, sfxVolume);
			this.SetVolume(AudioCategory.Dialogue, sfxVolume);
			float musicVolume = PlayerSettings.GetMusicVolume();
			this.SetVolume(AudioCategory.Music, musicVolume);
			this.SetVolume(AudioCategory.Ambience, musicVolume);
		}

		public void SetAllVolume(float volume)
		{
			this.SetVolume(AudioCategory.Effect, volume);
			this.SetVolume(AudioCategory.Music, volume);
			this.SetVolume(AudioCategory.Ambience, volume);
			this.SetVolume(AudioCategory.Dialogue, volume);
		}

		public void SetVolume(AudioCategory category, float volume)
		{
			volume = Mathf.Clamp(volume, 0f, 1f);
			this.audioSources[category].volume = volume;
			bool flag = true;
			if (category == AudioCategory.Music)
			{
				bool flag2 = Service.EnvironmentController.IsMusicPlaying();
				bool flag3 = this.IsThirdPartyNativePluginActive();
				flag = (!flag2 && !flag3);
			}
			if (volume == 0f || (category == AudioCategory.Music && !flag))
			{
				this.audioSources[category].mute = true;
				this.audioSources[category].Stop();
				if (category == AudioCategory.Music || category == AudioCategory.Ambience || category == AudioCategory.Dialogue)
				{
					this.UnloadLastPlayed(category);
				}
				else if (category == AudioCategory.Effect)
				{
					this.UnloadEffectsAudio();
				}
			}
			else
			{
				this.audioSources[category].mute = false;
				if (category == AudioCategory.Music || category == AudioCategory.Ambience)
				{
					Service.EventManager.SendEvent(EventId.MusicUnmuted, null);
				}
			}
		}

		public void UnloadEffectsAudio()
		{
			foreach (KeyValuePair<AudioTypeVO, AudioData> current in this.loadedAudio)
			{
				if (current.Key.Category == AudioCategory.Effect)
				{
					this.UnloadAudio(current.Value);
				}
			}
		}

		public void ResetBattleAudioFlags()
		{
			int num = 8;
			for (int i = 0; i < num; i++)
			{
				this.battleAudioFlags[i] = false;
			}
		}

		public bool GetBattleAudioFlag(AudioCollectionType type)
		{
			return this.battleAudioFlags[(int)type];
		}

		public void SetBattleAudioFlag(AudioCollectionType type)
		{
			this.battleAudioFlags[(int)type] = true;
		}

		public void ToggleAllSounds(bool enabled)
		{
			if (enabled)
			{
				this.RefreshVolume();
			}
			else
			{
				this.SetAllVolume(0f);
			}
		}

		public bool IsThirdPartyNativePluginActive()
		{
			if (Service.MobileConnectorAdsController != null)
			{
				MobileConnectorAdsController mobileConnectorAdsController = Service.MobileConnectorAdsController;
				if (mobileConnectorAdsController.IsEnabled() && mobileConnectorAdsController.IsVideoAdActive)
				{
					return true;
				}
			}
			return false;
		}
	}
}
