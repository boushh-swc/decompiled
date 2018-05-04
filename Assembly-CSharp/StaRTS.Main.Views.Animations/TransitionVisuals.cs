using StaRTS.Assets;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Animations
{
	public class TransitionVisuals : UXFactory
	{
		private const string ASSET_ANCHOR = "Anchor3DAssets";

		private const string PLANET_LOADING_3D_PREFIX = "planet_loading_";

		private const string PLANET_LABEL = "PlanetLabel";

		private const string PLANET_ANCHOR = "PlanetAnchor";

		private const string PREFAB_LOAD_ERROR = "Invalid GameObject: ";

		private TransitionVisualsLoadedDelegate onLoadedCallback;

		private object onLoadedCookie;

		private AssetHandle assetHandle;

		private AssetHandle assetHandleModels;

		private AssetHandle assetHandlePlanet;

		private AssetHandle assetHandlePlanetGlow;

		private AssetHandle assetHandleStars;

		private PlanetVO planet;

		private AssetManager assetManager;

		private string modelAssetName;

		private GameObject planetAnchor;

		private bool onlyLoadUI;

		private GameObject planetGameObject;

		private GameObject planetGlowGameObject;

		private GameObject modelGameObject;

		private GameObject starsGameObject;

		private GameObject starsPrefab;

		private Material planetMaterial;

		private bool uiLoadReturned;

		private bool modelAssetLoadReturned;

		private bool planetAssetLoadReturned;

		private bool planetGlowAssetLoadReturned;

		private bool starfieldAssetLoadReturned;

		public TransitionVisuals(PlanetVO planet, TransitionVisualsLoadedDelegate onLoaded, object cookie, bool onlyLoadUI) : base(Service.CameraManager.UXSceneCamera)
		{
			if (planet == null)
			{
				planet = Service.StaticDataController.Get<PlanetVO>("planet1");
			}
			this.planet = planet;
			this.onLoadedCallback = onLoaded;
			this.onLoadedCookie = cookie;
			this.assetManager = Service.AssetManager;
			this.onlyLoadUI = onlyLoadUI;
			base.Load(ref this.assetHandle, planet.LoadingScreenAssetName, new UXFactoryLoadDelegate(this.UILoaded), new UXFactoryLoadDelegate(this.LoadingGUIFail), null);
			if (onlyLoadUI)
			{
				return;
			}
			this.modelAssetName = "planet_loading_" + planet.Abbreviation;
			this.assetManager.Load(ref this.assetHandleModels, this.modelAssetName, new AssetSuccessDelegate(this.ModelsLoaded), new AssetFailureDelegate(this.LoadingModelFail), null);
			this.assetManager.Load(ref this.assetHandlePlanet, planet.GalaxyAssetName, new AssetSuccessDelegate(this.PlanetLoaded), new AssetFailureDelegate(this.LoadingPlanetFail), null);
			this.assetManager.Load(ref this.assetHandlePlanetGlow, "fx_planetGlow", new AssetSuccessDelegate(this.PlanetGlowLoaded), new AssetFailureDelegate(this.LoadingPlanetGlowFail), null);
			this.assetManager.Load(ref this.assetHandleStars, "planets_starfield_non-moving", new AssetSuccessDelegate(this.OnStarsLoaded), new AssetFailureDelegate(this.OnStarsLoadFail), null);
		}

		private void UILoaded(object cookie)
		{
			this.Visible = true;
			UXLabel optionalElement = base.GetOptionalElement<UXLabel>("PlanetLabel");
			if (optionalElement != null)
			{
				optionalElement.Text = Service.Lang.Get(this.planet.LoadingScreenText, new object[0]);
			}
			this.uiLoadReturned = true;
			this.AssembleLoadingScene();
		}

		private void LoadingGUIFail(object cookie)
		{
			this.uiLoadReturned = true;
			this.AssembleLoadingScene();
		}

		private void ModelsLoaded(object asset, object cookie)
		{
			this.modelGameObject = (asset as GameObject);
			if (this.modelGameObject == null)
			{
				Service.Logger.Warn("Invalid GameObject: " + this.modelAssetName);
			}
			this.modelAssetLoadReturned = true;
			this.AssembleLoadingScene();
		}

		private void LoadingModelFail(object cookie)
		{
			this.modelAssetLoadReturned = true;
			this.AssembleLoadingScene();
		}

		private void PlanetLoaded(object asset, object cookie)
		{
			this.planetGameObject = (asset as GameObject);
			if (this.planetGameObject == null)
			{
				Service.Logger.Warn("Invalid GameObject: " + this.planet.GalaxyAssetName);
			}
			else
			{
				this.planetGameObject.SetActive(false);
			}
			this.planetAssetLoadReturned = true;
			this.AssembleLoadingScene();
		}

		private void LoadingPlanetFail(object cookie)
		{
			this.planetAssetLoadReturned = true;
			this.AssembleLoadingScene();
		}

		private void PlanetGlowLoaded(object asset, object cookie)
		{
			this.planetGlowGameObject = (asset as GameObject);
			this.planetGlowGameObject = UnityEngine.Object.Instantiate<GameObject>(this.planetGlowGameObject);
			if (this.planetGlowGameObject == null)
			{
				Service.Logger.Warn("Invalid GameObject: fx_planetGlow");
			}
			this.planetGlowAssetLoadReturned = true;
			this.AssembleLoadingScene();
		}

		private void LoadingPlanetGlowFail(object cookie)
		{
			this.planetGlowAssetLoadReturned = true;
			this.AssembleLoadingScene();
		}

		private void OnStarsLoaded(object asset, object cookie)
		{
			this.starsGameObject = (asset as GameObject);
			this.starsGameObject.SetActive(true);
			if (this.starsGameObject == null)
			{
				Service.Logger.Warn("Invalid GameObject: planets_starfield_non-moving");
			}
			this.starfieldAssetLoadReturned = true;
			this.AssembleLoadingScene();
		}

		private void OnStarsLoadFail(object cookie)
		{
			this.starfieldAssetLoadReturned = true;
			this.AssembleLoadingScene();
		}

		private void AssembleLoadingScene()
		{
			if (this.onlyLoadUI)
			{
				if (!this.uiLoadReturned)
				{
					return;
				}
				this.ExecuteCallBack();
			}
			if (!this.uiLoadReturned || !this.modelAssetLoadReturned || !this.planetAssetLoadReturned || !this.planetGlowAssetLoadReturned || !this.starfieldAssetLoadReturned)
			{
				return;
			}
			if (this.modelGameObject != null)
			{
				UnityUtils.SetLayerRecursively(this.modelGameObject, 9);
				Transform transform = this.modelGameObject.transform;
				transform.parent = base.Root.transform.parent;
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
				transform.localScale = Vector3.one;
				this.planetAnchor = UnityUtils.FindGameObject(this.modelGameObject, "PlanetAnchor");
			}
			if (this.planetAnchor != null)
			{
				Camera camera = Service.CameraManager.UXSceneCamera.Camera;
				if (this.planetGameObject != null)
				{
					this.planetGameObject.SetActive(true);
					UnityUtils.SetLayerRecursively(this.planetGameObject, 9);
					Transform transform2 = this.planetGameObject.transform;
					transform2.parent = this.planetAnchor.transform;
					transform2.localPosition = Vector3.zero;
					transform2.localRotation = Quaternion.identity;
					transform2.localScale = Vector3.one;
					this.planetMaterial = PlanetUtils.StopPlanetSpinning(this.planetGameObject);
					Service.GalaxyPlanetController.SetPlanetLighting(this.planetGameObject, this.planetMaterial, camera);
				}
				if (this.planetGlowGameObject != null)
				{
					this.planetGlowGameObject.SetActive(true);
					UnityUtils.SetLayerRecursively(this.planetGlowGameObject, 9);
					Transform transform3 = this.planetGlowGameObject.transform;
					transform3.parent = this.planetAnchor.transform;
					transform3.localPosition = Vector3.zero;
					transform3.localRotation = Quaternion.identity;
					transform3.localScale = Vector3.one;
					transform3.LookAt(camera.transform.position);
				}
			}
			if (this.starsGameObject != null)
			{
				UnityUtils.SetLayerRecursively(this.starsGameObject, 9);
				Transform transform4 = this.starsGameObject.transform;
				transform4.parent = base.Root.transform.parent;
				transform4.localPosition = Vector3.zero;
				transform4.localRotation = Quaternion.identity;
				transform4.localScale = Vector3.one;
			}
			this.ExecuteCallBack();
		}

		private void ExecuteCallBack()
		{
			Service.CameraManager.UXSceneCamera.Camera.enabled = true;
			if (this.onLoadedCallback != null)
			{
				this.onLoadedCallback(this.onLoadedCookie);
			}
		}

		public void Cleanup()
		{
			if (this.assetHandle != AssetHandle.Invalid)
			{
				base.Unload(this.assetHandle, this.planet.LoadingScreenAssetName);
				this.assetHandle = AssetHandle.Invalid;
			}
			if (this.modelGameObject != null)
			{
				UnityEngine.Object.Destroy(this.modelGameObject);
				this.modelGameObject = null;
			}
			if (this.assetHandleModels != AssetHandle.Invalid)
			{
				this.assetManager.Unload(this.assetHandleModels);
				this.assetHandleModels = AssetHandle.Invalid;
			}
			if (this.assetHandlePlanet != AssetHandle.Invalid)
			{
				this.assetManager.Unload(this.assetHandlePlanet);
				this.assetHandlePlanet = AssetHandle.Invalid;
			}
			if (this.assetHandlePlanetGlow != AssetHandle.Invalid)
			{
				this.assetManager.Unload(this.assetHandlePlanetGlow);
				this.assetHandlePlanetGlow = AssetHandle.Invalid;
			}
			if (this.starsGameObject != null)
			{
				UnityEngine.Object.Destroy(this.starsGameObject);
				this.starsGameObject = null;
			}
			if (this.assetHandleStars != AssetHandle.Invalid)
			{
				this.assetManager.Unload(this.assetHandleStars);
				this.assetHandleStars = AssetHandle.Invalid;
			}
			Service.CameraManager.UXSceneCamera.Camera.enabled = false;
			UnityUtils.DestroyMaterial(this.planetMaterial);
			base.DestroyFactory();
		}
	}
}
