using StaRTS.Assets;
using StaRTS.Main.Configs;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.Cameras;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.FX
{
	public class PlanetEffectController
	{
		private const string LOCATOR_PLANET_EFFECT = "locator_effect";

		private const string LOCATOR_PLANET_EMITTER = "locator_emitter";

		private const string HOTH_PLANET_UID = "planet21";

		private AssetHandle fxHandle;

		private List<GameObject> effectObjects;

		public PlanetEffectController()
		{
			Service.PlanetEffectController = this;
		}

		public void AttachEffects(GameObject gameObject, object cookie)
		{
			if (!HardwareProfile.IsLowEndDevice())
			{
				PlanetVO planetVO = cookie as PlanetVO;
				this.LoadPlanetFX(planetVO.Uid, this.GetEffectNames(planetVO.PlanetaryFX), gameObject);
			}
		}

		private List<string> GetEffectNames(string planetaryFx)
		{
			if (string.IsNullOrEmpty(planetaryFx))
			{
				return null;
			}
			List<string> list = new List<string>();
			StaticDataController staticDataController = Service.StaticDataController;
			char[] separator = new char[]
			{
				'|'
			};
			string[] array = planetaryFx.Split(separator);
			int i = 0;
			int num = array.Length;
			while (i < num)
			{
				EffectsTypeVO optional = staticDataController.GetOptional<EffectsTypeVO>(array[i]);
				if (optional == null || string.IsNullOrEmpty(optional.AssetName))
				{
					return null;
				}
				list.Add(optional.AssetName);
				i++;
			}
			return list;
		}

		private void LoadPlanetFX(string planetUid, List<string> effectNames, GameObject gameObject)
		{
			if (effectNames == null)
			{
				return;
			}
			int count = effectNames.Count;
			string assetName;
			if (count > 0)
			{
				assetName = effectNames[UnityEngine.Random.Range(0, count - 1)];
			}
			else
			{
				assetName = effectNames[0];
			}
			AssetManager assetManager = Service.AssetManager;
			assetManager.RegisterPreloadableAsset(assetName);
			switch (planetUid)
			{
			case "planet21":
				assetManager.Load(ref this.fxHandle, assetName, new AssetSuccessDelegate(this.OnLoadHothFx), null, gameObject);
				return;
				break;
			}
			assetManager.Load(ref this.fxHandle, assetName, new AssetSuccessDelegate(this.OnLoadPlanetFX), null, gameObject);
		}

		private void OnLoadPlanetFX(object asset, object cookie)
		{
			if (this.effectObjects == null)
			{
				this.effectObjects = new List<GameObject>();
			}
			GameObject item = UnityEngine.Object.Instantiate<GameObject>((GameObject)asset);
			this.effectObjects.Add(item);
		}

		private void OnLoadHothFx(object asset, object cookie)
		{
			if (this.effectObjects == null)
			{
				this.effectObjects = new List<GameObject>();
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((GameObject)asset);
			this.effectObjects.Add(gameObject);
			MainCamera mainCamera = Service.CameraManager.MainCamera;
			if (mainCamera == null)
			{
				return;
			}
			if (mainCamera.GameObj != null)
			{
				gameObject.transform.SetParent(mainCamera.GameObj.transform);
			}
		}

		public void UnloadAllFx()
		{
			if (this.effectObjects != null)
			{
				int i = 0;
				int count = this.effectObjects.Count;
				while (i < count)
				{
					UnityEngine.Object.Destroy(this.effectObjects[i]);
					i++;
				}
				this.effectObjects.Clear();
			}
			if (this.fxHandle != AssetHandle.Invalid)
			{
				Service.AssetManager.Unload(this.fxHandle);
				this.fxHandle = AssetHandle.Invalid;
			}
		}
	}
}
