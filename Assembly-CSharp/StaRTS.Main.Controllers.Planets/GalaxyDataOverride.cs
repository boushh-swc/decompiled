using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers.Planets
{
	public class GalaxyDataOverride : MonoBehaviour
	{
		private PlanetVO tatooinePlanetData;

		private PlanetVO dandoranPlanetData;

		private PlanetVO hothPlanetData;

		private PlanetVO erkitPlanetData;

		private PlanetVO yavinPlanetData;

		public float GalaxyAutoRotateSpeed;

		public float GalaxyPlanetForegroundUIAngle;

		public float GalaxyForegroundAngle;

		public float GalaxyForegroundPlateauAngle;

		public float GalaxyCameraHeightOffset;

		public float GalaxyCameraDistanceOffset;

		public float GalaxyEaseRotationTime;

		public float GalaxyEaseRotationTransitionTime;

		public float GalaxyInitialGalaxyZoomDist;

		public float GalaxyInitialGalaxyZoomTime;

		public float GalaxyPlanetViewHeight;

		public float GalaxyPlanetGalaxyZoomTime;

		public float GalaxyPlanetSwipeMinMove;

		public float GalaxyPlanetSwipeMaxTime;

		public float GalaxyPlanetSwipeTime;

		public string PlanetPositionX = "= Rotation Pos About Galaxy";

		public string PlanetPositionY = "= Distance from Center";

		public string PlanetPositionZ = "= Height Above Plane";

		public Vector3 TatooinePlanetPos;

		public Vector3 DandoranPlanetPos;

		public Vector3 HothPlanetPos;

		public Vector3 ErkitPlanetPos;

		public Vector3 YavinPlanetPos;

		public GalaxyDataOverride()
		{
			this.TatooinePlanetPos = Vector3.zero;
			this.DandoranPlanetPos = Vector3.zero;
			this.HothPlanetPos = Vector3.zero;
			this.ErkitPlanetPos = Vector3.zero;
			this.YavinPlanetPos = Vector3.zero;
			StaticDataController staticDataController = Service.StaticDataController;
			this.tatooinePlanetData = staticDataController.Get<PlanetVO>(GameConstants.TATOOINE_PLANET_UID);
			this.dandoranPlanetData = staticDataController.Get<PlanetVO>(GameConstants.DANDORAN_PLANET_UID);
			this.hothPlanetData = staticDataController.Get<PlanetVO>(GameConstants.HOTH_PLANET_UID);
			this.erkitPlanetData = staticDataController.Get<PlanetVO>(GameConstants.ERKIT_PLANET_UID);
			this.yavinPlanetData = staticDataController.Get<PlanetVO>(GameConstants.YAVIN_PLANET_UID);
			this.UpdateGalaxyValues();
			this.UpdatePlanetValues();
		}

		private void UpdatePlanetValues()
		{
			this.TatooinePlanetPos = this.tatooinePlanetData.GetGalaxyPositionAsVec3();
			this.DandoranPlanetPos = this.dandoranPlanetData.GetGalaxyPositionAsVec3();
			this.HothPlanetPos = this.hothPlanetData.GetGalaxyPositionAsVec3();
			this.ErkitPlanetPos = this.erkitPlanetData.GetGalaxyPositionAsVec3();
			this.YavinPlanetPos = this.yavinPlanetData.GetGalaxyPositionAsVec3();
		}

		private void UpdateGalaxyValues()
		{
			this.GalaxyAutoRotateSpeed = GameConstants.GALAXY_AUTO_ROTATE_SPEED;
			this.GalaxyForegroundAngle = GameConstants.GALAXY_PLANET_FOREGROUND_THRESHOLD;
			this.GalaxyForegroundPlateauAngle = GameConstants.GALAXY_PLANET_FOREGROUND_PLATEAU_THRESHOLD;
			this.GalaxyPlanetForegroundUIAngle = GameConstants.GALAXY_PLANET_FOREGROUND_UI_THRESHOLD;
			this.GalaxyCameraHeightOffset = GameConstants.GALAXY_CAMERA_HEIGHT_OFFSET;
			this.GalaxyCameraDistanceOffset = GameConstants.GALAXY_CAMERA_DISTANCE_OFFSET;
			this.GalaxyEaseRotationTime = GameConstants.GALAXY_EASE_ROTATION_TIME;
			this.GalaxyEaseRotationTransitionTime = GameConstants.GALAXY_EASE_ROTATION_TRANSITION_TIME;
			this.GalaxyInitialGalaxyZoomDist = GameConstants.GALAXY_INITIAL_GALAXY_ZOOM_DIST;
			this.GalaxyInitialGalaxyZoomTime = GameConstants.GALAXY_INITIAL_GALAXY_ZOOM_TIME;
			this.GalaxyPlanetViewHeight = GameConstants.GALAXY_PLANET_VIEW_HEIGHT;
			this.GalaxyPlanetGalaxyZoomTime = GameConstants.GALAXY_PLANET_GALAXY_ZOOM_TIME;
			this.GalaxyPlanetSwipeMinMove = GameConstants.GALAXY_PLANET_SWIPE_MIN_MOVE;
			this.GalaxyPlanetSwipeMaxTime = GameConstants.GALAXY_PLANET_SWIPE_MAX_TIME;
			this.GalaxyPlanetSwipeTime = GameConstants.GALAXY_PLANET_SWIPE_TIME;
		}

		private void Update()
		{
			this.PlanetPositionX = "= Rotation Pos About Galaxy";
			this.PlanetPositionY = "= Distance from Center";
			this.PlanetPositionZ = "= Height Above Plane";
			this.UpdateGalaxyValues();
			this.UpdatePlanetValues();
		}

		private void OnValidate()
		{
			this.tatooinePlanetData.SetGalaxyPositionFromVec3(this.TatooinePlanetPos);
			this.dandoranPlanetData.SetGalaxyPositionFromVec3(this.DandoranPlanetPos);
			this.hothPlanetData.SetGalaxyPositionFromVec3(this.HothPlanetPos);
			this.erkitPlanetData.SetGalaxyPositionFromVec3(this.ErkitPlanetPos);
			this.yavinPlanetData.SetGalaxyPositionFromVec3(this.YavinPlanetPos);
			Service.GalaxyViewController.UpdateGalaxyConstants();
		}
	}
}
