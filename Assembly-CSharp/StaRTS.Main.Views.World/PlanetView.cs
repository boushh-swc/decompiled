using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using StaRTS.Main.Configs;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.World.Targeting;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.World
{
	public class PlanetView
	{
		public const float MAP_CAMERA_DEF_Y = 200f;

		public const float MAP_CAMERA_DEFAULT_ZOOM_LEVEL = 0.7f;

		private const float TOP_DOWN_STEEPNESS = 0.6f;

		private const float DEFAULT_GROUND_SIZE = 500f;

		private const string LOCATOR_NEAR_L = "locator_lowerLeft";

		private const string LOCATOR_NEAR_R = "locator_lowerRight";

		private const string LOCATOR_FAR_L = "locator_upperLeft";

		private const string LOCATOR_FAR_R = "locator_upperRight";

		private const string GRID_ASSET = "visible_grid_plane";

		private static readonly Vector3 MAP_CORNER_NEAR_L = new Vector3(-0.43f, 0f, -0.02f);

		private static readonly Vector3 MAP_CORNER_NEAR_R = new Vector3(-0.02f, 0f, -0.43f);

		private static readonly Vector3 MAP_CORNER_FAR_L = new Vector3(0.03f, 0f, 0.53f);

		private static readonly Vector3 MAP_CORNER_FAR_R = new Vector3(0.53f, 0f, 0.03f);

		private static readonly Vector3[] DEFAULT_CORNER_LOCATORS = new Vector3[]
		{
			PlanetView.MAP_CORNER_NEAR_L * 500f,
			PlanetView.MAP_CORNER_NEAR_R * 500f,
			PlanetView.MAP_CORNER_FAR_L * 500f,
			PlanetView.MAP_CORNER_FAR_R * 500f
		};

		private AssetHandle planetAssetHandle;

		private PlanetVO currentPlanet;

		private MapManipulator mapManipulator;

		private float yToHypotenuse;

		private Vector3[] cornerLocators;

		private Scaffolding scaffolding;

		private SpawnProtectionView spawnProtection;

		private TargetIdentifier targetIdentifier;

		private HeroIdentifier heroIdentifier;

		private GameObject worldGrid;

		private AssetHandle gridAssetHandle;

		private GameObject groundAsset;

		public Scaffolding Scaffolding
		{
			get
			{
				return this.scaffolding;
			}
		}

		public TargetIdentifier TargetIdentifier
		{
			get
			{
				return this.targetIdentifier;
			}
		}

		public HeroIdentifier HeroIdentifier
		{
			get
			{
				return this.heroIdentifier;
			}
		}

		public SpawnProtectionView SpawnProtection
		{
			get
			{
				return this.spawnProtection;
			}
		}

		public PlanetView()
		{
			this.mapManipulator = null;
			this.scaffolding = new Scaffolding();
			this.targetIdentifier = new TargetIdentifier();
			this.heroIdentifier = new HeroIdentifier();
			this.spawnProtection = new SpawnProtectionView();
			this.Reset();
		}

		public void Prepare(PlanetVO planet, AssetsCompleteDelegate onLoadComplete, object cookie)
		{
			if (planet == this.currentPlanet)
			{
				onLoadComplete(cookie);
				return;
			}
			AssetManager assetManager = Service.AssetManager;
			if (this.planetAssetHandle != AssetHandle.Invalid)
			{
				assetManager.Unload(this.planetAssetHandle);
				this.planetAssetHandle = AssetHandle.Invalid;
			}
			this.currentPlanet = planet;
			this.Reset();
			List<string> list = new List<string>();
			string text = (!HardwareProfile.IsLowEndDevice()) ? planet.AssetName : (planet.AssetName + "-lod1");
			list.Add(text);
			assetManager.RegisterPreloadableAsset(text);
			List<object> list2 = new List<object>();
			list2.Add(this.currentPlanet);
			List<AssetHandle> list3 = new List<AssetHandle>();
			list3.Add(AssetHandle.Invalid);
			assetManager.MultiLoad(list3, list, new AssetSuccessDelegate(this.AssetSuccess), null, list2, onLoadComplete, cookie);
			this.planetAssetHandle = list3[0];
		}

		public void StartMapManipulation()
		{
			this.mapManipulator = new MapManipulator(this.yToHypotenuse, this.cornerLocators);
		}

		public void PanToLocation(Vector3 worldLocation)
		{
			this.mapManipulator.PanToLocation(worldLocation);
		}

		public void ZoomIn()
		{
			this.mapManipulator.ZoomIn(false);
		}

		public void ZoomOut()
		{
			this.mapManipulator.ZoomOut(false);
		}

		public void ZoomOutImmediate()
		{
			this.mapManipulator.ZoomOut(true);
		}

		public void ZoomTo(float amount)
		{
			this.mapManipulator.ZoomTo(amount, false);
		}

		public void ResetCameraImmediate()
		{
			this.mapManipulator.ResetCameraPositionImmediatly();
		}

		private void AssetSuccess(object asset, object cookie)
		{
			GameObject gameObject = asset as GameObject;
			PlanetVO planetVO = cookie as PlanetVO;
			bool flag = HardwareProfile.IsLowEndDevice();
			if (planetVO != null && !flag)
			{
				Service.PlanetEffectController.AttachEffects(gameObject, planetVO);
			}
			Transform transform = gameObject.transform;
			transform.position = Vector3.zero;
			transform.rotation = Quaternion.AngleAxis(-90f, Vector3.up);
			if (!flag && transform.FindChild("terrainInnerMesh"))
			{
				Service.TerrainBlendController.IndexTerrainMesh(gameObject);
			}
			this.groundAsset = gameObject;
			Vector3 vector;
			Vector3 vector2;
			Vector3 vector3;
			Vector3 vector4;
			Vector3[] array;
			if (!this.TryGetLocator(gameObject, "locator_lowerLeft", out vector) || !this.TryGetLocator(gameObject, "locator_lowerRight", out vector2) || !this.TryGetLocator(gameObject, "locator_upperLeft", out vector3) || !this.TryGetLocator(gameObject, "locator_upperRight", out vector4))
			{
				Service.Logger.Warn("Unable to find corner locators");
				array = PlanetView.DEFAULT_CORNER_LOCATORS;
			}
			else
			{
				array = new Vector3[]
				{
					vector,
					vector2,
					vector3,
					vector4
				};
			}
			this.SetCornerLocators(array);
		}

		private bool TryGetLocator(GameObject parent, string name, out Vector3 v)
		{
			GameObject gameObject = UnityUtils.FindGameObject(parent, name);
			if (gameObject == null)
			{
				v = Vector3.zero;
				return false;
			}
			v = gameObject.transform.position;
			return true;
		}

		public void SetIsoVantage(CameraFeel cameraFeel)
		{
			this.SetVantage(false, cameraFeel, true, false, Vector3.zero);
		}

		public void SetEditModeVantage(bool topDown)
		{
			bool keepFocus = false;
			Vector3 focus = Vector3.zero;
			Entity selectedBuilding = Service.BuildingController.SelectedBuilding;
			if (selectedBuilding != null)
			{
				GameObjectViewComponent gameObjectViewComponent = selectedBuilding.Get<GameObjectViewComponent>();
				if (gameObjectViewComponent != null)
				{
					keepFocus = true;
					focus = gameObjectViewComponent.MainTransform.position;
				}
			}
			this.SetVantage(topDown, CameraFeel.Fast, false, keepFocus, focus);
		}

		private void SetVantage(bool topDown, CameraFeel cameraFeel, bool reset, bool keepFocus, Vector3 focus)
		{
			MainCamera mainCamera = Service.CameraManager.MainCamera;
			float num = 200f;
			float num2 = -num;
			float num3 = -num;
			if (topDown)
			{
				num2 *= 0.6f;
				num3 *= 0.6f;
			}
			this.SetYToHypotenuse(Mathf.Sqrt(1f + (num2 * num2 + num3 * num3) / (num * num)));
			num = Mathf.Sqrt(3f * num * num - num2 * num2 - num3 * num3);
			if (!reset)
			{
				num2 += mainCamera.CurrentLookatAnchor.x;
				num3 += mainCamera.CurrentLookatAnchor.z;
			}
			mainCamera.AnchorCamera(new Vector3(num2, num, num3));
			if (cameraFeel != CameraFeel.NoChange)
			{
				mainCamera.SetCameraFeel(cameraFeel);
			}
			if (reset)
			{
				mainCamera.AnchorLookat(Vector3.zero);
			}
			if (keepFocus)
			{
				mainCamera.KeepFocus(focus);
			}
			if (this.mapManipulator != null)
			{
				this.mapManipulator.OnVantageSwitch(topDown);
			}
		}

		public void DrawWorldGrid()
		{
			if (this.gridAssetHandle == AssetHandle.Invalid)
			{
				Service.AssetManager.Load(ref this.gridAssetHandle, "visible_grid_plane", new AssetSuccessDelegate(this.OnGridSuccess), null, null);
			}
		}

		private void OnGridSuccess(object asset, object cookie)
		{
			this.worldGrid = (asset as GameObject);
			this.worldGrid.transform.localPosition = Vector3.zero;
			GameObject gameObject = UnityUtils.FindGameObject(this.worldGrid, "userGrid");
			gameObject.transform.localPosition = new Vector3(0f, 0.06f, 0f);
			LightingEffectsController lightingEffectsController = Service.LightingEffectsController;
			Color currentLightingColor = lightingEffectsController.GetCurrentLightingColor(LightingColorType.GridColor);
			gameObject.GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", currentLightingColor);
		}

		public void DestroyWorldGrid()
		{
			if (this.gridAssetHandle != AssetHandle.Invalid)
			{
				if (this.worldGrid != null)
				{
					UnityEngine.Object.Destroy(this.worldGrid);
					this.worldGrid = null;
				}
				Service.AssetManager.Unload(this.gridAssetHandle);
				this.gridAssetHandle = AssetHandle.Invalid;
			}
		}

		private void SetYToHypotenuse(float yToHypotenuse)
		{
			this.yToHypotenuse = yToHypotenuse;
			if (this.mapManipulator != null)
			{
				this.mapManipulator.YToHypotenuse = yToHypotenuse;
			}
		}

		private void SetCornerLocators(Vector3[] cornerLocators)
		{
			this.cornerLocators = cornerLocators;
			if (this.mapManipulator != null)
			{
				this.mapManipulator.CornerLocators = cornerLocators;
			}
		}

		public void ComputeCornerLocators()
		{
			Vector3 vector;
			Vector3 vector2;
			Vector3 vector3;
			Vector3 vector4;
			Vector3[] array;
			if (!this.TryGetLocator(this.groundAsset, "locator_lowerLeft", out vector) || !this.TryGetLocator(this.groundAsset, "locator_lowerRight", out vector2) || !this.TryGetLocator(this.groundAsset, "locator_upperLeft", out vector3) || !this.TryGetLocator(this.groundAsset, "locator_upperRight", out vector4))
			{
				Service.Logger.Warn("Unable to find corner locators");
				array = PlanetView.DEFAULT_CORNER_LOCATORS;
			}
			else
			{
				array = new Vector3[]
				{
					vector,
					vector2,
					vector3,
					vector4
				};
			}
			this.SetCornerLocators(array);
		}

		private void Reset()
		{
			this.SetYToHypotenuse(MathUtils.SQRT3);
			this.SetCornerLocators(PlanetView.DEFAULT_CORNER_LOCATORS);
			if (this.groundAsset != null)
			{
				UnityEngine.Object.Destroy(this.groundAsset);
				this.groundAsset = null;
				Service.PlanetEffectController.UnloadAllFx();
			}
		}
	}
}
