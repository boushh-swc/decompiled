using StaRTS.Assets;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Planets;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Story;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.Planets;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers.Planets
{
	public class GalaxyViewController : IEventObserver, IViewFrameTimeObserver
	{
		private const float MIN_EASE_DIFF = 0.01f;

		private const float NEAR_CLIP_PLANE = 0.1f;

		private const float FAR_CLIP_PLANE = 5000f;

		private const float DEFAULT_FOV = 10f;

		private const int ROT_EASING_MAX = 60;

		private const float GRID_Z_OFFSET = -8f;

		private const string STAR_CAMERA = "StarCamera";

		private const string BG_STAR = "bgStars";

		private const float MIN_HEIGHT_DIV = 2f;

		private GameObject stars;

		private GameObject bgStars;

		private GameObject spiral;

		private Transform spiralTransform;

		private GameObject galaxyOverlayGrid;

		private float zoomTime;

		private float zoomDist;

		private Vector3 galaxyPosOffset;

		private Vector3 galaxyStarsOffset;

		private Vector3 galaxyToStarAdjust;

		private Vector2 galaxyOffsetScreen;

		private AssetHandle galaxySpiralHandle;

		private AssetHandle galaxyStarsHandle;

		private AssetHandle galaxyGridHandle;

		private bool initialZoomComplete;

		private bool softSnappedToPlanet;

		private bool galaxyBeingDragged;

		private bool galaxyReleased;

		private bool galaxyClosing;

		private bool ignoreSwipes;

		private float easeTargetRotation;

		private float planetStartRotation;

		private float planetTargetRotaion;

		private float easeVelocity;

		private float[] rotationEasing;

		private int easeIndex;

		private float cachedCameraFOV;

		private Vector3 foregroundPos;

		private float cachedWorldNearCliplPlane;

		private float cachedWorldFarClipPlane;

		private Vector3 cachedCameraLookAt;

		private Vector3 planetViewLookPos;

		private float planetViewDist;

		private float objectiveViewDist;

		private Quaternion planetViewLookAtRotation;

		private float foregroundPlateauAngle;

		private float planetViewCameraHeight;

		private float objectiveViewCameraHeight;

		private PlanetDetailsScreen planetScreen;

		private CampaignScreenSection planetSection;

		private string planetViewUID;

		private GalaxyViewState galaxyViewState;

		private float objectiveOffset;

		private bool doesObjectivesNeedUpdate;

		private GalaxyManipulator galaxyManip;

		private GameObject galaxyDataOverrider;

		private CameraManager cameraManager;

		private Vector3 planetLook;

		private Vector3 cameraOffset;

		private bool loadGalaxyGrid;

		private GalaxyTransitionData currentTranstion;

		public GalaxyViewController()
		{
			EventManager eventManager = Service.EventManager;
			this.doesObjectivesNeedUpdate = false;
			this.cameraManager = Service.CameraManager;
			MainCamera mainCamera = this.cameraManager.MainCamera;
			Service.GalaxyViewController = this;
			this.galaxyPosOffset = new Vector3(0f, 10000f, 0f);
			this.galaxyStarsOffset = new Vector3(10000f, 10000f, 0f);
			this.galaxyToStarAdjust = new Vector3(10000f, 0f, 0f);
			this.galaxyOffsetScreen = mainCamera.WorldPositionToScreenPoint(Vector3.zero);
			eventManager.RegisterObserver(this, EventId.WorldLoadComplete, EventPriority.Default);
			this.galaxySpiralHandle = AssetHandle.Invalid;
			this.galaxyStarsHandle = AssetHandle.Invalid;
			this.galaxyGridHandle = AssetHandle.Invalid;
			this.galaxyViewState = GalaxyViewState.Loading;
			this.galaxyBeingDragged = false;
			this.galaxyReleased = false;
			this.galaxyManip = new GalaxyManipulator();
			this.planetViewLookAtRotation = Quaternion.identity;
			this.cachedCameraFOV = 10f;
			this.rotationEasing = new float[60];
			this.easeIndex = 0;
			this.loadGalaxyGrid = false;
			this.cameraOffset = Vector3.zero;
			this.planetLook = Vector3.zero;
			this.currentTranstion = new GalaxyTransitionData();
			this.currentTranstion.Reset();
			this.InitForegroundPos();
		}

		public void UpdateGalaxyConstants()
		{
			GalaxyPlanetController galaxyPlanetController = Service.GalaxyPlanetController;
			if (galaxyPlanetController.AreAllPlanetsLoaded())
			{
				MainCamera mainCamera = this.cameraManager.MainCamera;
				this.InitForegroundPos();
				this.planetViewCameraHeight = GameConstants.GALAXY_PLANET_VIEW_HEIGHT;
				float gALAXY_CAMERA_DISTANCE_OFFSET = GameConstants.GALAXY_CAMERA_DISTANCE_OFFSET;
				float gALAXY_CAMERA_HEIGHT_OFFSET = GameConstants.GALAXY_CAMERA_HEIGHT_OFFSET;
				this.planetViewLookAtRotation = Quaternion.identity;
				mainCamera.ResetAndStopRotation();
				mainCamera.ResetHarness(this.cachedCameraLookAt);
				mainCamera.ForceCameraMoveFinish();
				Vector3 a = new Vector3(gALAXY_CAMERA_DISTANCE_OFFSET, gALAXY_CAMERA_HEIGHT_OFFSET, gALAXY_CAMERA_DISTANCE_OFFSET);
				Vector3 harnessPosition = a - mainCamera.CurrentCameraPosition;
				mainCamera.SetRotationHarnessPosition(this.galaxyPosOffset);
				mainCamera.SetHarnessPosition(harnessPosition);
				mainCamera.SetLookAtPositionImmediately(this.galaxyPosOffset, this.planetViewLookAtRotation);
				galaxyPlanetController.UpdatePlanetConstants();
			}
		}

		private void InitForegroundPos()
		{
			this.foregroundPlateauAngle = Mathf.Min(GameConstants.GALAXY_PLANET_FOREGROUND_THRESHOLD, GameConstants.GALAXY_PLANET_FOREGROUND_PLATEAU_THRESHOLD);
			this.foregroundPos = new Vector3(this.galaxyOffsetScreen.x, this.galaxyOffsetScreen.y + (float)Screen.height / 2f, 0f);
		}

		private void AdjustCameraPosition(float dist, float height, Vector3 look)
		{
			MainCamera mainCamera = this.cameraManager.MainCamera;
			this.cameraOffset.Set(dist, height, dist);
			mainCamera.SetHarnessPosition(this.cameraOffset - mainCamera.CurrentCameraPosition);
			mainCamera.SetLookAtPositionImmediately(look, this.planetViewLookAtRotation);
		}

		private void SetupCamera()
		{
			MainCamera mainCamera = this.cameraManager.MainCamera;
			this.cachedWorldNearCliplPlane = mainCamera.Camera.nearClipPlane;
			this.cachedWorldFarClipPlane = mainCamera.Camera.farClipPlane;
			mainCamera.RotateAboutPoint = this.galaxyPosOffset;
			this.cachedCameraLookAt = mainCamera.CurrentLookatPosition;
			mainCamera.Camera.clearFlags = CameraClearFlags.Depth;
			this.cachedCameraFOV = mainCamera.Camera.fieldOfView;
			mainCamera.SetFov(this.cameraManager.StarsCamera.fieldOfView);
			mainCamera.SetRotationFeel(CameraFeel.Medium);
			this.planetViewLookAtRotation = Quaternion.identity;
			float num = GameConstants.GALAXY_CAMERA_DISTANCE_OFFSET + this.zoomDist;
			float gALAXY_CAMERA_HEIGHT_OFFSET = GameConstants.GALAXY_CAMERA_HEIGHT_OFFSET;
			Vector3 a = new Vector3(num, gALAXY_CAMERA_HEIGHT_OFFSET, num);
			Vector3 harnessPosition = a - mainCamera.CurrentCameraPosition;
			mainCamera.SetRotationHarnessPosition(this.galaxyPosOffset);
			mainCamera.SetHarnessPosition(harnessPosition);
			mainCamera.SetLookAtPositionImmediately(this.galaxyPosOffset, this.planetViewLookAtRotation);
			mainCamera.SetClipPlanes(0.1f, 5000f);
			mainCamera.GroundOffset = 10000f;
		}

		private void InitGalaxy(float initialZoomDist)
		{
			this.softSnappedToPlanet = false;
			this.galaxyReleased = false;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			Service.WorldInitializer.View.ResetCameraImmediate();
			this.cameraManager.MainCamera.ForceCameraMoveFinish();
			this.zoomDist = initialZoomDist;
			this.zoomTime = 0f;
			this.stars.SetActive(true);
			this.bgStars.SetActive(true);
			this.cameraManager.StarsCamera.enabled = true;
			this.SetupCamera();
			this.galaxyManip.Enable();
			Service.GameStateMachine.SetState(new GalaxyState());
		}

		public void GoToPlanetView(string planetUID, CampaignScreenSection section)
		{
			EventManager eventManager = Service.EventManager;
			PlanetVO optional = Service.StaticDataController.GetOptional<PlanetVO>(planetUID);
			if (optional == null)
			{
				Service.Logger.Warn("Planet Details Screen: '" + planetUID + "' not found.");
				return;
			}
			if (!optional.PlayerFacing)
			{
				Service.Logger.Warn("Planet Details Screen: '" + planetUID + "' PlayerFacing == false");
				return;
			}
			if (this.IsPlanetDetailsScreenOpen())
			{
				GalaxyPlanetController galaxyPlanetController = Service.GalaxyPlanetController;
				this.SwitchToPlanet(galaxyPlanetController.GetPlanet(planetUID));
				return;
			}
			Service.UserInputInhibitor.DenyAll();
			if (this.spiral != null)
			{
				this.spiral.SetActive(true);
			}
			Service.UXController.HUD.SetSquadScreenAlwaysOnTop(true);
			Service.UXController.MiscElementsManager.HideEventsTickerView();
			this.planetViewCameraHeight = GameConstants.GALAXY_PLANET_VIEW_HEIGHT;
			this.galaxyClosing = false;
			this.planetSection = section;
			this.planetViewUID = planetUID;
			this.galaxyViewState = GalaxyViewState.PlanetTransitionInstantStart;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			Service.UXController.MiscElementsManager.PrepareGalaxyPlanetUI();
			Service.GalaxyPlanetController.InitPlanets();
			Service.UXController.MiscElementsManager.SetGalaxyCloseButtonVisible(false);
			eventManager.SendEvent(EventId.GalaxyGoToPlanetView, null);
		}

		public void GoToGalaxyView()
		{
			this.GoToGalaxyView(Service.CurrentPlayer.Planet.Uid);
		}

		public void GoToGalaxyView(string planetUID)
		{
			EventManager eventManager = Service.EventManager;
			if (string.IsNullOrEmpty(planetUID))
			{
				planetUID = Service.CurrentPlayer.Planet.Uid;
			}
			PlanetVO optional = Service.StaticDataController.GetOptional<PlanetVO>(planetUID);
			if (optional == null)
			{
				Service.Logger.Warn("GoToGalaxyView: '" + planetUID + "' not found.");
				return;
			}
			if (!optional.PlayerFacing)
			{
				Service.Logger.Warn("GoToGalaxyView: '" + planetUID + "' PlayerFacing == false");
				return;
			}
			this.initialZoomComplete = false;
			this.galaxyClosing = false;
			this.planetScreen = null;
			this.planetViewUID = string.Empty;
			this.ignoreSwipes = false;
			this.galaxyViewState = GalaxyViewState.Loading;
			if (this.spiral != null)
			{
				this.spiral.SetActive(true);
			}
			this.planetViewCameraHeight = GameConstants.GALAXY_PLANET_VIEW_HEIGHT;
			Service.GalaxyViewController.ResetCameraForBase();
			Service.WorldInitializer.View.ResetCameraImmediate();
			this.cameraManager.MainCamera.ForceCameraMoveFinish();
			this.cameraManager.WipeCamera.StartEllipticalWipe(WipeTransition.FromBaseToGalaxy, 0.5f, 0.5f, null, null);
			Service.UXController.MiscElementsManager.SetGalaxyCloseButtonVisible(true);
			this.InitGalaxy(GameConstants.GALAXY_INITIAL_GALAXY_ZOOM_DIST);
			Service.UXController.MiscElementsManager.PrepareGalaxyPlanetUI();
			Service.GalaxyPlanetController.InitPlanets(planetUID);
			this.InitGalaxyGrid();
			this.InitInitialCameraPos();
			eventManager.SendEvent(EventId.GalaxyGoToGalaxyView, null);
		}

		private void InitInitialCameraPos()
		{
			MainCamera mainCamera = this.cameraManager.MainCamera;
			this.AdjustCameraPosition(GameConstants.GALAXY_CAMERA_DISTANCE_OFFSET, GameConstants.GALAXY_CAMERA_HEIGHT_OFFSET, this.galaxyPosOffset);
			Vector3 initialPlanetPosition = Service.GalaxyPlanetController.InitialPlanetPosition;
			Vector2 galaxyScreenPos = this.GetGalaxyScreenPos(initialPlanetPosition);
			Vector3 positionOnGalaxyPlane = this.GetPositionOnGalaxyPlane(galaxyScreenPos);
			Vector3 foregroundPositonOnGalaxyPlane = this.GetForegroundPositonOnGalaxyPlane();
			float num = this.CalculateGalaxyRotation(positionOnGalaxyPlane, foregroundPositonOnGalaxyPlane);
			num += mainCamera.RotationSpring.Position.x;
			mainCamera.UpdateRotationImmediatelyTo(num);
		}

		public void GoToHome()
		{
			this.GoToHome(true, null, null);
		}

		public void GoToHome(bool playWipe, WipeCompleteDelegate completeCallback, object completeCookie)
		{
			if (!this.galaxyClosing && Service.GameStateMachine.CurrentState is GalaxyState)
			{
				this.ClearEasing();
				if (this.planetScreen != null)
				{
					this.planetScreen.CloseNoTransition(null);
				}
				this.galaxyClosing = true;
				if (playWipe)
				{
					this.cameraManager.WipeCamera.StartEllipticalWipe(WipeTransition.FromGalaxyToBase, 0.5f, 0.5f, completeCallback, completeCookie);
				}
				this.UnregisterForPlanetScreenEvents();
				this.initialZoomComplete = false;
				this.planetScreen = null;
				this.planetViewUID = string.Empty;
				this.ignoreSwipes = false;
				MainCamera mainCamera = this.cameraManager.MainCamera;
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
				this.softSnappedToPlanet = false;
				mainCamera.GroundOffset = 0f;
				mainCamera.ResetAndStopRotation();
				mainCamera.ResetHarness(this.cachedCameraLookAt);
				mainCamera.ForceCameraMoveFinish();
				mainCamera.SetClipPlanes(this.cachedWorldNearCliplPlane, this.cachedWorldFarClipPlane);
				mainCamera.Camera.clearFlags = CameraClearFlags.Color;
				mainCamera.SetFov(this.cachedCameraFOV);
				this.stars.SetActive(false);
				this.galaxyManip.Disable();
				this.ClearForegroundUI();
				Service.UXController.MiscElementsManager.HideGalaxyPlanetUI();
				HomeState.GoToHomeState(null, false);
				Service.WorldInitializer.View.ZoomIn();
				Service.GalaxyPlanetController.DestroyAllPlanets();
				Service.UXController.MiscElementsManager.SetGalaxyCloseButtonVisible(false);
				this.DestroyGalaxyGrid();
			}
		}

		public void ResetCameraForBase()
		{
			if (!this.galaxyClosing && this.galaxyViewState != GalaxyViewState.Loading)
			{
				this.ClearEasing();
				MainCamera mainCamera = this.cameraManager.MainCamera;
				this.galaxyClosing = true;
				Service.UXController.HUD.SetSquadScreenAlwaysOnTop(false);
				this.softSnappedToPlanet = false;
				this.planetScreen = null;
				this.planetViewUID = string.Empty;
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
				mainCamera.SetFov(this.cachedCameraFOV);
				this.stars.SetActive(false);
				this.galaxyManip.Disable();
				Service.UXController.MiscElementsManager.HideGalaxyPlanetUI();
				Service.GalaxyPlanetController.DestroyAllPlanets();
				this.DestroyGalaxyGrid();
				this.ClearForegroundUI();
			}
		}

		private bool IsDraggable()
		{
			return this.galaxyViewState == GalaxyViewState.ManualRotate;
		}

		public float UpdateGalaxyRotation(Vector3 curPos, Vector3 prevPos, Vector3 start)
		{
			float num = 0f;
			if (this.IsDraggable())
			{
				this.galaxyBeingDragged = true;
				this.softSnappedToPlanet = false;
				if (Service.GalaxyPlanetController.ForegroundedPlanet != null)
				{
					this.galaxyViewState = GalaxyViewState.ManualRotate;
				}
				num = this.CalculateGalaxyRotation(curPos, prevPos);
				this.rotationEasing[this.easeIndex] = num;
				this.easeIndex = (this.easeIndex + 1) % 60;
				num = this.CalculateGalaxyRotation(curPos, start);
				this.cameraManager.MainCamera.UpdateRotationImmediatelyBy(num);
			}
			return num;
		}

		private float CalculateGalaxyRotation(Vector3 curPos, Vector3 prevPos)
		{
			Vector3 rhs = curPos;
			Vector3 lhs = prevPos;
			float num = Mathf.Atan2(rhs.z * lhs.x - lhs.z * rhs.x, Vector3.Dot(lhs, rhs));
			return 57.29578f * num;
		}

		private Vector3 GetPlanetPositionOnGalaxyPlane(Planet planet)
		{
			Vector2 planetScreenPos = this.GetPlanetScreenPos(planet);
			return this.GetPositionOnGalaxyPlane(planetScreenPos);
		}

		private Vector3 GetPositionOnGalaxyPlane(Vector2 screenPos)
		{
			MainCamera mainCamera = this.cameraManager.MainCamera;
			Vector3 screenPosition = new Vector3(screenPos.x, screenPos.y, 0f);
			Vector3 zero = Vector3.zero;
			mainCamera.GetGroundPosition(screenPosition, ref zero);
			return zero;
		}

		private Vector3 GetForegroundPositonOnGalaxyPlane()
		{
			MainCamera mainCamera = this.cameraManager.MainCamera;
			Vector3 zero = Vector3.zero;
			mainCamera.GetGroundPosition(this.foregroundPos, ref zero);
			return zero;
		}

		private void AttemptSoftSnapToCurrentPlanet()
		{
			if (!this.softSnappedToPlanet && !this.galaxyBeingDragged)
			{
				MainCamera mainCamera = this.cameraManager.MainCamera;
				Planet foregroundedPlanet = Service.GalaxyPlanetController.ForegroundedPlanet;
				if ((this.galaxyViewState == GalaxyViewState.ManualRotate || this.galaxyViewState == GalaxyViewState.PlanetTransitionInstantStart) && foregroundedPlanet != null)
				{
					this.galaxyReleased = false;
					this.ClearEasing();
					mainCamera.RotationSpring.StopMoving();
					this.softSnappedToPlanet = true;
					this.easeVelocity = 0f;
					Vector3 planetPositionOnGalaxyPlane = this.GetPlanetPositionOnGalaxyPlane(foregroundedPlanet);
					Vector3 foregroundPositonOnGalaxyPlane = this.GetForegroundPositonOnGalaxyPlane();
					this.easeTargetRotation = this.CalculateGalaxyRotation(planetPositionOnGalaxyPlane, foregroundPositonOnGalaxyPlane);
					this.easeTargetRotation += mainCamera.RotationSpring.Position.x;
				}
			}
		}

		private void ClearEasing()
		{
			for (int i = 0; i < 60; i++)
			{
				this.rotationEasing[i] = 0f;
			}
			this.easeIndex = 0;
		}

		public void OnTouchReleased(GalaxySwipeType type)
		{
			if (!this.ignoreSwipes)
			{
				if (type != GalaxySwipeType.SwipeRight)
				{
					if (type == GalaxySwipeType.SwipeLeft)
					{
						this.TransitionToNextPlanet();
					}
				}
				else
				{
					this.TransitionToPrevPlanet();
				}
			}
			this.ignoreSwipes = false;
			this.galaxyReleased = true;
		}

		private void ActivateGrid()
		{
			if (this.galaxyOverlayGrid != null)
			{
				this.galaxyOverlayGrid.SetActive(true);
			}
		}

		private void DeactivateGrid()
		{
			if (this.galaxyOverlayGrid != null)
			{
				this.galaxyOverlayGrid.SetActive(false);
			}
		}

		private void RegisterForPlanetScreenEvents()
		{
			Service.EventManager.RegisterObserver(this, EventId.InventoryCrateCollectionClosed, EventPriority.Default);
		}

		private void UnregisterForPlanetScreenEvents()
		{
			Service.EventManager.UnregisterObserver(this, EventId.InventoryCrateCollectionClosed);
		}

		public void TranstionPlanetToGalaxy()
		{
			MainCamera mainCamera = this.cameraManager.MainCamera;
			GalaxyPlanetController galaxyPlanetController = Service.GalaxyPlanetController;
			this.UnregisterForPlanetScreenEvents();
			this.zoomTime = 0f;
			this.planetTargetRotaion = 0f;
			this.planetStartRotation = 0f;
			this.planetScreen = null;
			this.bgStars.SetActive(true);
			this.spiral.SetActive(true);
			this.ActivateGrid();
			this.planetViewLookAtRotation = Quaternion.identity;
			galaxyPlanetController.ActivateAllPlanetRings();
			this.ShowPlanetLockedUI();
			Service.GalaxyPlanetController.ClearForegroundedPlanet();
			this.currentTranstion.Reset();
			this.currentTranstion.TransitionDuration = GameConstants.GALAXY_PLANET_GALAXY_ZOOM_TIME;
			Vector3 startlook = Vector3.zero;
			if (this.galaxyViewState == GalaxyViewState.LeftView)
			{
				this.galaxyViewState = GalaxyViewState.PlanetTransitionFromLeftTowardGalaxy;
				this.currentTranstion.SetTransitionDistance(this.objectiveViewDist, GameConstants.GALAXY_CAMERA_DISTANCE_OFFSET);
				this.currentTranstion.SetTransitionHeight(this.objectiveViewCameraHeight, GameConstants.GALAXY_CAMERA_HEIGHT_OFFSET);
				startlook = this.GetObjectiveDetailOffset(mainCamera, 0f);
			}
			else
			{
				this.galaxyViewState = GalaxyViewState.PlanetTransitionTowardGalaxy;
				this.currentTranstion.SetTransitionDistance(this.planetViewDist, GameConstants.GALAXY_CAMERA_DISTANCE_OFFSET);
				this.currentTranstion.SetTransitionHeight(this.planetViewCameraHeight, GameConstants.GALAXY_CAMERA_HEIGHT_OFFSET);
				startlook = this.planetViewLookPos;
			}
			this.currentTranstion.SetTransitionLookAt(startlook, this.galaxyPosOffset);
			Service.UXController.MiscElementsManager.SetGalaxyCloseButtonVisible(true);
			Service.UXController.MiscElementsManager.ShowEventsTickerView();
		}

		private bool IsDoingInitialZoom()
		{
			return this.zoomDist > 0f;
		}

		private void UpdateStarCamera()
		{
			MainCamera mainCamera = this.cameraManager.MainCamera;
			if (this.cameraManager.StarsCamera == null)
			{
				return;
			}
			Transform transform = this.cameraManager.StarsCamera.transform;
			if (transform == null)
			{
				return;
			}
			transform.position = mainCamera.Camera.transform.position + this.galaxyToStarAdjust;
			transform.LookAt(this.galaxyStarsOffset);
		}

		private void UpdateSpiral(float dt)
		{
			if (this.spiralTransform != null)
			{
				this.spiralTransform.Rotate(Vector3.up, dt * GameConstants.GALAXY_AUTO_ROTATE_SPEED);
			}
		}

		private void SetObjectivesVariables()
		{
			this.objectiveViewDist = this.planetViewDist - 5f;
			this.objectiveViewCameraHeight = this.planetViewCameraHeight - 4f;
		}

		private Vector3 GetObjectiveDetailOffset(MainCamera camera, float rotation)
		{
			Transform transform = camera.Camera.transform;
			Vector3 vector = transform.right * 2f + transform.up * 0.5f;
			if (rotation != 0f)
			{
				vector = Quaternion.AngleAxis(rotation, Vector3.up) * vector;
			}
			return this.planetViewLookPos + vector;
		}

		public void OnViewFrameTime(float dt)
		{
			MainCamera mainCamera = this.cameraManager.MainCamera;
			GalaxyPlanetController galaxyPlanetController = Service.GalaxyPlanetController;
			this.UpdateSpiral(dt);
			this.UpdateStarCamera();
			if (this.IsDoingInitialZoom())
			{
				if (!mainCamera.IsStillMoving())
				{
					this.zoomTime += dt;
					this.zoomDist = Mathf.Lerp(GameConstants.GALAXY_INITIAL_GALAXY_ZOOM_DIST, 0f, this.zoomTime / GameConstants.GALAXY_INITIAL_GALAXY_ZOOM_TIME);
					this.AdjustCameraPosition(GameConstants.GALAXY_CAMERA_DISTANCE_OFFSET + this.zoomDist, GameConstants.GALAXY_CAMERA_HEIGHT_OFFSET, this.galaxyPosOffset);
				}
			}
			else
			{
				if (!this.initialZoomComplete)
				{
					this.initialZoomComplete = true;
					Service.EventManager.SendEvent(EventId.GalaxyViewMapOpenComplete, null);
				}
				if (this.galaxyReleased)
				{
					this.galaxyReleased = false;
					this.galaxyBeingDragged = false;
				}
				switch (this.galaxyViewState)
				{
				case GalaxyViewState.Loading:
					if (galaxyPlanetController.AreAllPlanetsLoaded())
					{
						this.galaxyViewState = GalaxyViewState.ManualRotate;
					}
					break;
				case GalaxyViewState.ManualRotate:
					this.AttemptSoftSnapToCurrentPlanet();
					break;
				case GalaxyViewState.PlanetTransitionWithinGalaxy:
				{
					this.zoomTime += dt;
					float num = this.zoomTime / GameConstants.GALAXY_PLANET_SWIPE_TIME;
					num = Mathf.Min(num, 1f);
					num = Mathf.Sin(num * 3.14159274f * 0.5f);
					float rotation = Mathf.LerpAngle(this.planetStartRotation, this.planetTargetRotaion, num);
					mainCamera.UpdateRotationImmediatelyTo(rotation);
					if (num >= 1f)
					{
						this.galaxyViewState = GalaxyViewState.ManualRotate;
					}
					break;
				}
				case GalaxyViewState.PlanetTransitionTowardCamera:
					if (this.planetScreen != null && this.planetScreen.IsLoaded())
					{
						bool flag = this.InterpolatePlanetCamera(dt, this.currentTranstion);
						if (flag)
						{
							this.planetScreen.UpdateCurrentPlanet(galaxyPlanetController.ForegroundedPlanet);
							this.galaxyViewState = GalaxyViewState.PlanetView;
						}
					}
					break;
				case GalaxyViewState.PlanetTransitionTowardGalaxy:
				{
					bool flag2 = this.InterpolatePlanetCamera(dt, this.currentTranstion);
					if (flag2)
					{
						this.galaxyViewState = GalaxyViewState.ManualRotate;
						Service.EventManager.SendEvent(EventId.ReturnToGalaxyViewMapComplete, null);
					}
					break;
				}
				case GalaxyViewState.PlanetTransitionTowardLeft:
					if (this.planetScreen != null && this.planetScreen.IsLoaded())
					{
						bool flag3 = this.InterpolatePlanetCamera(dt, this.currentTranstion);
						if (flag3)
						{
							this.galaxyViewState = GalaxyViewState.LeftView;
							if (this.doesObjectivesNeedUpdate)
							{
								this.doesObjectivesNeedUpdate = false;
								this.planetScreen.UpdateCurrentPlanet(galaxyPlanetController.ForegroundedPlanet);
							}
						}
					}
					break;
				case GalaxyViewState.PlanetTransitionFromLeftTowardGalaxy:
				{
					bool flag4 = this.InterpolatePlanetCamera(dt, this.currentTranstion);
					if (flag4)
					{
						this.galaxyViewState = GalaxyViewState.ManualRotate;
						Service.EventManager.SendEvent(EventId.ReturnToGalaxyViewMapComplete, null);
					}
					break;
				}
				case GalaxyViewState.PlanetTransitionTowardCenter:
					if (this.planetScreen != null && this.planetScreen.IsLoaded())
					{
						bool flag5 = this.InterpolatePlanetCamera(dt, this.currentTranstion);
						if (flag5)
						{
							this.galaxyViewState = GalaxyViewState.PlanetView;
						}
					}
					break;
				case GalaxyViewState.PlanetTransitionInstantStart:
					if (galaxyPlanetController.AreAllPlanetsLoaded() && this.planetScreen == null)
					{
						galaxyPlanetController.SetForegroundedPlanet(this.planetViewUID);
						this.planetScreen = new PlanetDetailsScreen(galaxyPlanetController.ForegroundedPlanet, false);
						this.planetScreen.currentSection = this.planetSection;
						Service.ScreenController.AddScreen(this.planetScreen, true, false);
						this.RegisterForPlanetScreenEvents();
					}
					if (this.planetScreen != null && this.planetScreen.IsLoaded() && !this.softSnappedToPlanet)
					{
						this.InstantlySetCameraForPlanetView();
						this.planetScreen.Transition();
						this.AdjustCameraPosition(this.planetViewDist, this.planetViewCameraHeight, this.planetViewLookPos);
						this.galaxyViewState = GalaxyViewState.PlanetView;
						galaxyPlanetController.DeactivatePlanetRings(galaxyPlanetController.ForegroundedPlanet);
						if (!Service.CurrentPlayer.CampaignProgress.FueInProgress)
						{
							Service.UserInputInhibitor.AllowAll();
						}
					}
					break;
				}
				if (galaxyPlanetController.AreAllPlanetsLoaded())
				{
					this.UpdatePlanetUI(dt);
				}
				if (this.CanRotatePlanetToFront())
				{
					float totalTime = GameConstants.GALAXY_EASE_ROTATION_TIME;
					if (this.galaxyViewState == GalaxyViewState.PlanetTransitionPanTo)
					{
						totalTime = GameConstants.GALAXY_EASE_ROTATION_TRANSITION_TIME;
					}
					this.EaseGalaxyRotation(dt, totalTime);
					if (this.IsEaseDone() && this.galaxyViewState == GalaxyViewState.PlanetTransitionPanTo)
					{
						this.galaxyViewState = GalaxyViewState.ManualRotate;
						Service.EventManager.SendEvent(EventId.GalaxyStatePanToPlanetComplete, galaxyPlanetController.ForegroundedPlanet.VO.Uid);
					}
				}
			}
			galaxyPlanetController.UpdatePlanets();
		}

		private bool InterpolatePlanetCamera(float dt, GalaxyTransitionData transitionData)
		{
			MainCamera mainCamera = this.cameraManager.MainCamera;
			this.zoomTime += dt;
			float num = this.zoomTime / transitionData.TransitionDuration;
			num = Mathf.Min(num, 1f);
			num = Mathf.Sin(num * 3.14159274f * 0.5f);
			if (transitionData.Instant)
			{
				num = 1f;
			}
			float dist = Mathf.Lerp(transitionData.StartViewDistance, transitionData.EndViewDistance, num);
			float height = Mathf.Lerp(transitionData.StartViewHeight, transitionData.EndViewHeight, num);
			this.planetLook = Vector3.Lerp(transitionData.StartViewLookAt, transitionData.EndViewLookAt, num);
			if (transitionData.StartViewRotation != transitionData.EndViewRotation)
			{
				float rotation = Mathf.LerpAngle(transitionData.StartViewRotation, transitionData.EndViewRotation, num);
				mainCamera.UpdateRotationImmediatelyTo(rotation);
			}
			this.AdjustCameraPosition(dist, height, this.planetLook);
			return num >= 1f;
		}

		private void InitFinalPlanetPositions()
		{
			Planet foregroundedPlanet = Service.GalaxyPlanetController.ForegroundedPlanet;
			if (foregroundedPlanet != null)
			{
				MainCamera mainCamera = this.cameraManager.MainCamera;
				this.ClearEasing();
				this.easeVelocity = 0f;
				Vector3 planetPositionOnGalaxyPlane = this.GetPlanetPositionOnGalaxyPlane(foregroundedPlanet);
				Vector3 foregroundPositonOnGalaxyPlane = this.GetForegroundPositonOnGalaxyPlane();
				float num = this.CalculateGalaxyRotation(planetPositionOnGalaxyPlane, foregroundPositonOnGalaxyPlane);
				num += mainCamera.RotationSpring.Position.x;
				mainCamera.UpdateRotationImmediatelyTo(num);
			}
		}

		private void InstantlySetCameraForPlanetView()
		{
			this.InitGalaxy(0f);
			this.InitGalaxyGrid();
			this.InitFinalPlanetPositions();
			this.planetViewLookAtRotation = this.planetScreen.GetTransitionLookOffset();
		}

		private bool CanRotatePlanetToFront()
		{
			return this.softSnappedToPlanet;
		}

		private bool IsEaseDone()
		{
			MainCamera mainCamera = this.cameraManager.MainCamera;
			float num = Mathf.Abs(this.easeTargetRotation - mainCamera.RotationSpring.Position.x);
			return num <= 0.01f;
		}

		public bool IsPlanetDetailsScreenOpen()
		{
			return Service.GameStateMachine.CurrentState is GalaxyState && (this.galaxyViewState == GalaxyViewState.PlanetView || this.galaxyViewState == GalaxyViewState.PlanetTransitionInstantStart);
		}

		public bool IsPlanetDetailsScreenOpeningOrOpen()
		{
			return Service.GameStateMachine.CurrentState is GalaxyState || this.galaxyViewState == GalaxyViewState.PlanetTransitionInstantStart;
		}

		private void EaseGalaxyRotation(float dt, float totalTime)
		{
			MainCamera mainCamera = this.cameraManager.MainCamera;
			float rotation = Mathf.SmoothDampAngle(mainCamera.RotationSpring.Position.x, this.easeTargetRotation, ref this.easeVelocity, totalTime);
			mainCamera.UpdateRotationImmediatelyTo(rotation);
		}

		public void OnGalaxyObjectClicked(GameObject galaxyObject)
		{
			PlanetRef component = galaxyObject.GetComponent<PlanetRef>();
			if (component != null && !this.IsDoingInitialZoom() && this.IsDraggable())
			{
				string uid = component.Planet.VO.Uid;
				if (PlanetIntroStoryUtil.ShouldPlanetIntroStoryBePlayed(uid))
				{
					PlanetIntroStoryUtil.PlayPlanetIntroStoryChain(uid);
				}
				else
				{
					this.HandlePlanetClicked(component.Planet);
				}
			}
		}

		public float GetPlanetScaleFactor(Planet planet)
		{
			float result = 1f;
			if (planet != null)
			{
				float num = this.CalculateGalaxyRotation(this.GetPlanetPositionOnGalaxyPlane(planet), this.GetForegroundPositonOnGalaxyPlane());
				num = Mathf.Abs(num);
				num -= this.foregroundPlateauAngle * (1f - Mathf.Min(num / this.foregroundPlateauAngle, 1f));
				result = 1f - num / GameConstants.GALAXY_PLANET_FOREGROUND_THRESHOLD;
			}
			return result;
		}

		private void ClearForegroundUI()
		{
			Service.UXController.MiscElementsManager.ClearPlanetFrontUI();
		}

		private void AttachForegroundUIToPlanet(Planet planet)
		{
			Service.UXController.MiscElementsManager.AttachPlanetFrontUI(planet);
		}

		private void ShowForegroundPlanetUI(Planet planet)
		{
			Service.UXController.MiscElementsManager.ShowPlanetFrontUI(planet);
		}

		public void HideBackgroundPlanetUI(Planet planet)
		{
			Service.UXController.MiscElementsManager.HidePlanetBackUI(planet);
		}

		public void ShowBackgroundPlanetUI(Planet planet)
		{
			Service.UXController.MiscElementsManager.ShowPlanetBackUI(planet);
		}

		private Vector2 GetGalaxyScreenPos(Vector3 pos)
		{
			MainCamera mainCamera = this.cameraManager.MainCamera;
			pos = mainCamera.WorldPositionToScreenPoint(new Vector3(pos.x, this.galaxyPosOffset.y, pos.z));
			Vector2 result = new Vector2(pos.x, (float)Screen.height - pos.y);
			return result;
		}

		private Vector2 GetPlanetScreenPos(Planet planet)
		{
			Vector3 position = planet.PlanetGameObject.transform.position;
			return this.GetGalaxyScreenPos(position);
		}

		private bool IsPlanetForegrounded(Planet planet)
		{
			float f = this.CalculateGalaxyRotation(this.GetPlanetPositionOnGalaxyPlane(planet), this.GetForegroundPositonOnGalaxyPlane());
			return Mathf.Abs(f) <= GameConstants.GALAXY_PLANET_FOREGROUND_THRESHOLD;
		}

		private bool IsPlanetUIForegrounded(Planet planet)
		{
			float f = this.CalculateGalaxyRotation(this.GetPlanetPositionOnGalaxyPlane(planet), this.GetForegroundPositonOnGalaxyPlane());
			return Mathf.Abs(f) <= GameConstants.GALAXY_PLANET_FOREGROUND_UI_THRESHOLD;
		}

		private float GetPlanetDistToForeground(Planet planet)
		{
			float f = this.CalculateGalaxyRotation(this.GetPlanetPositionOnGalaxyPlane(planet), this.GetForegroundPositonOnGalaxyPlane());
			return Mathf.Abs(f);
		}

		private bool Transitioning()
		{
			return this.galaxyViewState == GalaxyViewState.PlanetTransitionTowardCamera || this.galaxyViewState == GalaxyViewState.PlanetTransitionTowardGalaxy || this.galaxyViewState == GalaxyViewState.PlanetTransitionInstantStart || this.galaxyViewState == GalaxyViewState.PlanetView || this.galaxyViewState == GalaxyViewState.PlanetTransitionFromLeftTowardGalaxy || this.galaxyViewState == GalaxyViewState.PlanetTransitionTowardLeft || this.galaxyViewState == GalaxyViewState.PlanetTransitionTowardCenter || this.galaxyViewState == GalaxyViewState.LeftView;
		}

		private void UpdatePlanetUI(float dt)
		{
			GalaxyPlanetController galaxyPlanetController = Service.GalaxyPlanetController;
			Planet planet = galaxyPlanetController.ForegroundedPlanet;
			Planet planet2 = planet;
			Planet planet3 = null;
			List<Planet> list = new List<Planet>();
			galaxyPlanetController.ClearForegroundPlanetStatus();
			galaxyPlanetController.BuildPlanetList(new GalaxyPlanetController.PlanetBooleanDelegate(this.IsPlanetForegrounded), ref list);
			if (!this.Transitioning())
			{
				planet = null;
				if (list.Count == 1)
				{
					planet = list[0];
					planet.IsForegrounded = true;
				}
				else if (list.Count > 1)
				{
					planet = list[0];
					planet.IsForegrounded = true;
					float num = this.GetPlanetDistToForeground(list[0]);
					int count = list.Count;
					for (int i = 1; i < count; i++)
					{
						Planet planet4 = list[i];
						planet4.IsForegrounded = true;
						float planetDistToForeground = this.GetPlanetDistToForeground(planet4);
						if (num > planetDistToForeground)
						{
							num = planetDistToForeground;
							planet = planet4;
						}
					}
				}
				galaxyPlanetController.ForegroundedPlanet = planet;
			}
			if (planet != null)
			{
				if (this.galaxyViewState == GalaxyViewState.PlanetTransitionTowardCamera)
				{
					this.ClearForegroundUI();
				}
				else if (!this.Transitioning())
				{
					if (this.IsPlanetUIForegrounded(planet))
					{
						planet3 = planet;
						this.ShowForegroundPlanetUI(planet);
						this.AttachForegroundUIToPlanet(planet);
					}
					else
					{
						Service.UXController.MiscElementsManager.PanBetweenPlanets();
					}
				}
				planet.UpdateThrashingPopulation(dt);
			}
			else
			{
				this.ClearForegroundUI();
			}
			if (planet2 != planet && this.galaxyBeingDragged)
			{
				this.ignoreSwipes = true;
			}
			int count2 = galaxyPlanetController.PlanetsWithActiveEvents.Count;
			for (int j = 0; j < count2; j++)
			{
				Planet planet5 = galaxyPlanetController.PlanetsWithActiveEvents[j];
				if (planet3 != planet5 && !this.Transitioning())
				{
					this.ShowBackgroundPlanetUI(planet5);
				}
				else
				{
					this.HideBackgroundPlanetUI(planet5);
				}
			}
		}

		public void SwitchToObjectiveDetails(bool newPlanet)
		{
			MainCamera mainCamera = this.cameraManager.MainCamera;
			this.zoomTime = 0f;
			Service.UXController.MiscElementsManager.HideEventsTickerView();
			this.galaxyViewState = GalaxyViewState.PlanetTransitionTowardLeft;
			this.spiral.SetActive(false);
			this.currentTranstion.Reset();
			this.currentTranstion.TransitionDuration = GameConstants.GALAXY_PLANET_GALAXY_ZOOM_TIME;
			this.currentTranstion.SetTransitionDistance(this.planetViewDist, this.objectiveViewDist);
			this.currentTranstion.SetTransitionHeight(this.planetViewCameraHeight, this.objectiveViewCameraHeight);
			float rotation = 0f;
			if (newPlanet)
			{
				this.doesObjectivesNeedUpdate = true;
				rotation = this.planetTargetRotaion - this.planetStartRotation;
				this.currentTranstion.SetTransitionRotation(this.planetStartRotation, this.planetTargetRotaion);
			}
			this.currentTranstion.SetTransitionLookAt(this.planetViewLookPos, this.GetObjectiveDetailOffset(mainCamera, rotation));
			float prevAngle = this.PrepareCameraForDistanceCalculation(this.planetViewLookPos);
			this.planetViewLookAtRotation = this.planetScreen.GetTransitionLookOffset();
			this.ResetCameraAfterDistanceCalculation(prevAngle);
		}

		public void SwitchFromObjectiveDetails()
		{
			MainCamera mainCamera = this.cameraManager.MainCamera;
			this.zoomTime = 0f;
			Service.UXController.MiscElementsManager.HideEventsTickerView();
			this.galaxyViewState = GalaxyViewState.PlanetTransitionTowardCenter;
			this.spiral.SetActive(true);
			this.currentTranstion.Reset();
			this.currentTranstion.TransitionDuration = GameConstants.GALAXY_PLANET_GALAXY_ZOOM_TIME;
			this.currentTranstion.SetTransitionDistance(this.objectiveViewDist, this.planetViewDist);
			this.currentTranstion.SetTransitionHeight(this.objectiveViewCameraHeight, this.planetViewCameraHeight);
			this.currentTranstion.SetTransitionLookAt(this.GetObjectiveDetailOffset(mainCamera, 0f), this.planetViewLookPos);
		}

		private void SwitchToPlanet(Planet planet)
		{
			this.SwitchToPlanet(planet, false);
		}

		public bool IsInPlanetScreen()
		{
			return this.planetScreen != null;
		}

		private void SwitchToPlanet(Planet planet, bool instant)
		{
			bool flag = this.planetScreen != null;
			this.zoomTime = 0f;
			this.planetViewLookAtRotation = Quaternion.identity;
			this.AdjustCameraPosition(GameConstants.GALAXY_CAMERA_DISTANCE_OFFSET, GameConstants.GALAXY_CAMERA_HEIGHT_OFFSET, this.galaxyPosOffset);
			Service.GalaxyPlanetController.UpdatePlanets();
			this.RotatePlanetToForeground(planet, false);
			this.currentTranstion.Reset();
			if (flag)
			{
				if (!this.InObjectivesState())
				{
					this.galaxyViewState = GalaxyViewState.PlanetTransitionTowardCamera;
					this.planetScreen.Transition();
				}
				else
				{
					this.planetScreen.Transition();
					this.SwitchToObjectiveDetails(true);
				}
			}
			else
			{
				this.ClearForegroundUI();
				this.galaxyViewState = GalaxyViewState.PlanetTransitionWithinGalaxy;
			}
			this.currentTranstion.Instant = instant;
		}

		private bool InObjectivesState()
		{
			return this.galaxyViewState == GalaxyViewState.LeftView || this.galaxyViewState == GalaxyViewState.PlanetTransitionFromLeftTowardGalaxy || this.galaxyViewState == GalaxyViewState.PlanetTransitionTowardCenter || this.galaxyViewState == GalaxyViewState.PlanetTransitionTowardLeft;
		}

		private void SetupTransitionTowardCamera()
		{
			this.currentTranstion.TransitionDuration = GameConstants.GALAXY_PLANET_GALAXY_ZOOM_TIME;
			this.currentTranstion.SetTransitionDistance(GameConstants.GALAXY_CAMERA_DISTANCE_OFFSET, this.planetViewDist);
			this.currentTranstion.SetTransitionHeight(GameConstants.GALAXY_CAMERA_HEIGHT_OFFSET, this.planetViewCameraHeight);
			this.currentTranstion.SetTransitionRotation(this.planetStartRotation, this.planetTargetRotaion);
			this.currentTranstion.SetTransitionLookAt(this.galaxyPosOffset, this.planetViewLookPos);
		}

		public Planet TransitionToNextPlanet()
		{
			GalaxyPlanetController galaxyPlanetController = Service.GalaxyPlanetController;
			Planet planet = galaxyPlanetController.TransitionToPlanet(true);
			this.SwitchToPlanet(planet);
			return planet;
		}

		public void TransitionToPlanet(Planet planet, bool instant)
		{
			GalaxyPlanetController galaxyPlanetController = Service.GalaxyPlanetController;
			galaxyPlanetController.ForegroundedPlanet = planet;
			this.SwitchToPlanet(planet, instant);
			if (instant)
			{
				this.OnViewFrameTime(0f);
			}
		}

		public Planet TransitionToPrevPlanet()
		{
			GalaxyPlanetController galaxyPlanetController = Service.GalaxyPlanetController;
			Planet planet = galaxyPlanetController.TransitionToPlanet(false);
			this.SwitchToPlanet(planet);
			return planet;
		}

		public void PanToPlanet(Planet planet)
		{
			this.galaxyViewState = GalaxyViewState.PlanetTransitionPanTo;
			this.RotatePlanetToForeground(planet, true);
		}

		public void RotatePlanetToForeground(Planet planet, bool soft)
		{
			if (planet != null)
			{
				Service.GalaxyPlanetController.ForegroundedPlanet = planet;
				this.softSnappedToPlanet = soft;
				MainCamera mainCamera = this.cameraManager.MainCamera;
				this.ClearEasing();
				mainCamera.RotationSpring.StopMoving();
				float num = this.CalculateGalaxyRotation(this.GetPlanetPositionOnGalaxyPlane(planet), this.GetForegroundPositonOnGalaxyPlane()) + mainCamera.RotationSpring.Position.x;
				if (soft)
				{
					this.easeTargetRotation = num;
					this.easeVelocity = 0f;
				}
				else
				{
					this.planetStartRotation = mainCamera.RotationSpring.Position.x;
					this.planetTargetRotaion = num;
				}
			}
		}

		public bool ShouldAnimateCurrent(Planet planet)
		{
			bool result = false;
			if (planet != null)
			{
				result = (planet.IsCurrentAndNeedsAnim() && !this.Transitioning());
			}
			return result;
		}

		public void ReturnPlanetScreenToMainSelect()
		{
			if (this.planetScreen != null && this.planetScreen.IsLoaded())
			{
				this.planetScreen.CloseSubScreenAndReturnToMainSelect();
			}
		}

		public void StartPlanetTransition()
		{
			GalaxyPlanetController galaxyPlanetController = Service.GalaxyPlanetController;
			this.zoomTime = 0f;
			if (this.galaxyViewState != GalaxyViewState.PlanetTransitionInstantStart)
			{
				this.DeactivateGrid();
				galaxyPlanetController.DeactivatePlanetRings(galaxyPlanetController.ForegroundedPlanet);
			}
			this.HidePlanetLockedUI();
			this.planetViewLookPos = galaxyPlanetController.GetCurrentPlanetPosition();
			float prevAngle = this.PrepareCameraForDistanceCalculation(this.planetViewLookPos);
			float planetFrustumDistance = this.planetScreen.GetPlanetFrustumDistance(galaxyPlanetController.ForegroundedPlanet.ObjectExtents.y);
			this.planetViewDist = planetFrustumDistance + galaxyPlanetController.ForegroundedPlanet.VO.Radius;
			float num = this.planetViewDist / 2f;
			if (this.planetViewCameraHeight > num)
			{
				this.planetViewCameraHeight = num;
			}
			else if (this.planetViewCameraHeight < -num)
			{
				this.planetViewCameraHeight = -num;
			}
			this.planetViewDist = Mathf.Sqrt(this.planetViewDist * this.planetViewDist - this.planetViewCameraHeight * this.planetViewCameraHeight);
			this.SetObjectivesVariables();
			if (this.galaxyViewState == GalaxyViewState.PlanetTransitionTowardCamera)
			{
				this.currentTranstion.Reset();
				this.SetupTransitionTowardCamera();
				this.planetViewLookAtRotation = this.planetScreen.GetTransitionLookOffset();
			}
			this.ResetCameraAfterDistanceCalculation(prevAngle);
		}

		private void ResetCameraAfterDistanceCalculation(float prevAngle)
		{
			this.cameraManager.MainCamera.UpdateRotationImmediatelyTo(prevAngle);
		}

		private float PrepareCameraForDistanceCalculation(Vector3 planetPos)
		{
			float x = this.cameraManager.MainCamera.RotationSpring.Position.x;
			Vector2 galaxyScreenPos = this.GetGalaxyScreenPos(planetPos);
			Vector3 positionOnGalaxyPlane = this.GetPositionOnGalaxyPlane(galaxyScreenPos);
			Vector3 foregroundPositonOnGalaxyPlane = this.GetForegroundPositonOnGalaxyPlane();
			float num = this.CalculateGalaxyRotation(positionOnGalaxyPlane, foregroundPositonOnGalaxyPlane);
			num += x;
			this.cameraManager.MainCamera.UpdateRotationImmediatelyTo(num);
			return x;
		}

		private void GoToPlanetScreen()
		{
			this.ClearForegroundUI();
			Service.UXController.MiscElementsManager.HideEventsTickerView();
			PlanetDetailsScreen screen = new PlanetDetailsScreen(Service.GalaxyPlanetController.ForegroundedPlanet, true);
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			this.planetScreen = screen;
			PlanetVO vO = this.planetScreen.CurrentPlanet.VO;
			Service.ScreenController.AddScreen(screen, true, false);
			this.RegisterForPlanetScreenEvents();
			string cookie = vO.PlanetBIName.ToLower() + " | " + currentPlayer.GetPlanetStatus(vO.Uid);
			Service.EventManager.SendEvent(EventId.GalaxyPlanetTapped, cookie);
		}

		public void OpenPlanetDetailsForPlanet(string planetUID)
		{
			Planet planet = Service.GalaxyPlanetController.GetPlanet(planetUID);
			if (planet == null)
			{
				return;
			}
			this.HandlePlanetClicked(planet);
		}

		private void HidePlanetLockedUI()
		{
			Service.UXController.MiscElementsManager.HidePlanetLockedUI();
		}

		private void ShowPlanetLockedUI()
		{
			Service.UXController.MiscElementsManager.ShowPlanetLockedUI();
		}

		private void HandlePlanetClicked(Planet clickedPlanet)
		{
			this.ClearForegroundUI();
			Service.UXController.MiscElementsManager.SetGalaxyCloseButtonVisible(false);
			this.galaxyBeingDragged = false;
			this.RotatePlanetToForeground(clickedPlanet, false);
			this.galaxyViewState = GalaxyViewState.PlanetTransitionTowardCamera;
			if (this.planetScreen == null)
			{
				this.GoToPlanetScreen();
			}
		}

		private void InitGalaxyView()
		{
			Service.EventManager.UnregisterObserver(this, EventId.WorldLoadComplete);
			Service.AssetManager.Load(ref this.galaxyStarsHandle, "planets_starfield", new AssetSuccessDelegate(this.OnStarsLoaded), null, null);
			Service.AssetManager.Load(ref this.galaxySpiralHandle, "planets_galaxy", new AssetSuccessDelegate(this.OnSpiralLoaded), null, null);
		}

		private void InitGalaxyGrid()
		{
			if (this.loadGalaxyGrid)
			{
				Service.AssetManager.Load(ref this.galaxyGridHandle, "planets_galaxygrid", new AssetSuccessDelegate(this.OnGridLoaded), null, null);
			}
		}

		private void OnGridLoaded(object asset, object cookie)
		{
			MainCamera mainCamera = this.cameraManager.MainCamera;
			this.galaxyOverlayGrid = (GameObject)asset;
			Transform transform = this.galaxyOverlayGrid.transform;
			transform.SetParent(mainCamera.MainRotCameraHarness.transform);
			transform.rotation = Quaternion.Euler(0f, mainCamera.Camera.transform.rotation.eulerAngles.y, 0f);
			transform.position = this.galaxyPosOffset;
			transform.localPosition += new Vector3(0f, 0f, -8f);
			switch (this.galaxyViewState)
			{
			case GalaxyViewState.PlanetTransitionInstantStart:
			case GalaxyViewState.PlanetView:
				this.DeactivateGrid();
				return;
			}
			this.ActivateGrid();
		}

		private void DestroyGalaxyGrid()
		{
			if (this.galaxyOverlayGrid != null)
			{
				UnityEngine.Object.Destroy(this.galaxyOverlayGrid);
				this.galaxyOverlayGrid = null;
			}
			if (this.galaxyGridHandle != AssetHandle.Invalid)
			{
				Service.AssetManager.Unload(this.galaxyGridHandle);
				this.galaxyGridHandle = AssetHandle.Invalid;
			}
		}

		private void OnGalaxyStarsSpiralLoaded()
		{
			this.spiralTransform.parent = this.stars.transform;
			this.spiralTransform.localPosition = Vector3.zero;
		}

		private void OnSpiralLoaded(object asset, object cookie)
		{
			this.spiral = (GameObject)asset;
			if (this.spiral != null)
			{
				this.spiralTransform = this.spiral.transform;
				if (this.stars != null)
				{
					this.OnGalaxyStarsSpiralLoaded();
				}
			}
		}

		private void OnStarsLoaded(object asset, object cookie)
		{
			this.stars = UnityEngine.Object.Instantiate<GameObject>(asset as GameObject);
			Transform transform = this.stars.transform;
			transform.position = this.galaxyStarsOffset;
			this.cameraManager.StarsCamera = transform.FindChild("StarCamera").GetComponent<Camera>();
			this.bgStars = transform.FindChild("bgStars").gameObject;
			this.stars.SetActive(false);
			if (this.stars != null && this.spiral != null)
			{
				this.OnGalaxyStarsSpiralLoaded();
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.WorldLoadComplete)
			{
				if (id == EventId.InventoryCrateCollectionClosed)
				{
					this.GoToHome();
				}
			}
			else
			{
				this.InitGalaxyView();
			}
			return EatResponse.NotEaten;
		}
	}
}
