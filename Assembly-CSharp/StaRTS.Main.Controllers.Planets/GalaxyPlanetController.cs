using StaRTS.Assets;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Planets;
using StaRTS.Main.Models.Planets;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.UX;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers.Planets
{
	public class GalaxyPlanetController
	{
		public delegate bool PlanetBooleanDelegate(Planet planet);

		private const string PLANETARY_LIGHTING_DIRECTION = "_LightDir";

		private const float PLANET_GLOW_SCALE = 1.15f;

		private const string DEFAULT_PLANETID_IF_NO_LOGIN = "";

		private const string UNSCOUTED_MESH = "planetgalaxy_neu-mod_unscoutedgrid";

		private const string REBEL_EVENT_EFFECT = "xwingplanet_rbl-ani";

		private const string EMPIRE_EVENT_EFFECT = "tiefighterplanet_emp-ani";

		private const string EVENT_EFFECT_ANIM_NAME = "Idle";

		private const string EVENT_PARTICLE_FX_UPCOMING_NAME = "fx_planet_Imminent";

		private const string EVENT_PARTICLE_FX_ACTIVE_NAME = "fx_planet_Conflict";

		private const string RING_PARTICLE_NAME = "Rings";

		private const string EXPLOSION_PARTICLE_NAME = "planetExplosions";

		public const string PLANET_GLOW_NAME = "fx_planetGlow";

		private static readonly Vector3 Global_LightDir = new Vector3(-119f, 57.4f, -144.3f);

		private static readonly Vector3 PLANET_GLOW_OFFSET = new Vector3(0f, 0f, -9f);

		private List<Planet> planetsWithActiveEvents;

		private List<Planet> planetsWithEvent;

		private int loadedPlanetsCount;

		private int planetParticleCount;

		private int planetEffectsCount;

		private int planetIndex;

		private Planet currentlyForegroundedPlanet;

		private bool updatePlanetPopulations;

		private string eventEffectsAssetName;

		private List<Planet> planets;

		private Dictionary<string, Planet> planetLookupDict;

		public List<Planet> PlanetsWithActiveEvents
		{
			get
			{
				return this.planetsWithActiveEvents;
			}
		}

		public Vector3 InitialPlanetPosition
		{
			get;
			private set;
		}

		public Planet ForegroundedPlanet
		{
			get
			{
				return this.currentlyForegroundedPlanet;
			}
			set
			{
				this.currentlyForegroundedPlanet = value;
				this.UpdatePlanetIndex();
			}
		}

		public GalaxyPlanetController()
		{
			Service.GalaxyPlanetController = this;
			this.currentlyForegroundedPlanet = null;
			this.planetsWithActiveEvents = new List<Planet>();
			this.planetsWithEvent = new List<Planet>();
			this.loadedPlanetsCount = 0;
			this.planetParticleCount = 0;
			this.planetEffectsCount = 0;
			this.updatePlanetPopulations = true;
			this.planets = new List<Planet>();
			this.planetLookupDict = new Dictionary<string, Planet>();
		}

		public void ClearPlanetsWithEvents()
		{
			this.planetsWithEvent.Clear();
			this.planetsWithActiveEvents.Clear();
		}

		public void ClearForegroundedPlanet()
		{
			this.currentlyForegroundedPlanet = null;
		}

		public void UpdatePlanetConstants()
		{
			int count = this.planets.Count;
			for (int i = 0; i < count; i++)
			{
				Planet planet = this.planets[i];
				planet.PlanetGameObject.transform.position = this.GetPlanetVect3Position(planet.VO.Angle, planet.VO.Radius, -planet.VO.HeightOffset);
			}
		}

		public Planet GetPlanet(string planetUID)
		{
			if (this.planetLookupDict.ContainsKey(planetUID))
			{
				return this.planetLookupDict[planetUID];
			}
			Service.Logger.Error("No Planet found with UID: " + planetUID);
			return null;
		}

		public bool AreAllPlanetsLoaded()
		{
			return this.planets.Count == this.loadedPlanetsCount && this.loadedPlanetsCount > 0;
		}

		public bool AreAllEffectsAndParticlesLoaded()
		{
			int count = this.planetsWithEvent.Count;
			int count2 = this.planets.Count;
			return count == this.planetParticleCount && count2 == this.planetEffectsCount;
		}

		public void SetForegroundedPlanet(string planetUID)
		{
			Planet planet = this.GetPlanet(planetUID);
			this.currentlyForegroundedPlanet = planet;
			this.UpdatePlanetIndex();
		}

		public Vector3 GetCurrentPlanetPosition()
		{
			Vector3 result = Vector3.zero;
			if (this.currentlyForegroundedPlanet != null)
			{
				result = this.currentlyForegroundedPlanet.PlanetGameObject.transform.position;
			}
			return result;
		}

		public void UpdatePlanetPopulation(string planetUID, int population)
		{
			int count = this.planets.Count;
			for (int i = 0; i < count; i++)
			{
				Planet planet = this.planets[i];
				if (planet.VO.Uid == planetUID)
				{
					planet.ThrashingPopulation = population;
					planet.VO.Population = population;
					break;
				}
			}
		}

		public void ClearForegroundPlanetStatus()
		{
			int count = this.planets.Count;
			for (int i = 0; i < count; i++)
			{
				Planet planet = this.planets[i];
				planet.IsForegrounded = false;
			}
		}

		public void BuildPlanetList(GalaxyPlanetController.PlanetBooleanDelegate addToList, ref List<Planet> planetList)
		{
			int count = this.planets.Count;
			for (int i = 0; i < count; i++)
			{
				Planet planet = this.planets[i];
				if (addToList(planet))
				{
					planetList.Add(planet);
				}
			}
		}

		public List<Planet> GetListOfUnlockedPlanets()
		{
			List<Planet> list = null;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (this.AreAllPlanetsLoaded())
			{
				list = new List<Planet>();
				int count = this.planets.Count;
				for (int i = 0; i < count; i++)
				{
					if (currentPlayer.IsPlanetUnlocked(this.planets[i].VO.Uid))
					{
						list.Add(this.planets[i]);
					}
				}
			}
			return list;
		}

		public void InitPlanets()
		{
			this.InitPlanets(Service.CurrentPlayer.Planet.Uid);
		}

		public void InitPlanets(string startPlanetUid)
		{
			PlanetStatsCommand planetStatsCommand = null;
			PlanetStatsRequest planetStatsRequest = null;
			AssetManager assetManager = Service.AssetManager;
			if (this.updatePlanetPopulations)
			{
				this.updatePlanetPopulations = false;
				planetStatsRequest = new PlanetStatsRequest();
				planetStatsCommand = new PlanetStatsCommand(planetStatsRequest);
			}
			this.planetIndex = 0;
			this.planetsWithEvent.Clear();
			this.planetsWithActiveEvents.Clear();
			List<PlanetVO> allPlayerFacingPlanets = PlanetUtils.GetAllPlayerFacingPlanets();
			int i = 0;
			int count = allPlayerFacingPlanets.Count;
			while (i < count)
			{
				Planet planet = new Planet(allPlayerFacingPlanets[i]);
				AssetHandle handle = AssetHandle.Invalid;
				if (planet.VO.Uid == startPlanetUid)
				{
					this.currentlyForegroundedPlanet = planet;
					this.InitialPlanetPosition = this.GetPlanetVect3Position(planet.VO.Angle, planet.VO.Radius, -planet.VO.HeightOffset);
				}
				if (planetStatsRequest != null)
				{
					planetStatsRequest.AddPlanetID(planet.VO.Uid);
				}
				else
				{
					planet.ThrashingPopulation = planet.VO.Population;
				}
				planet.IsCurrentPlanet = (startPlanetUid == planet.VO.Uid);
				this.planets.Add(planet);
				this.planetLookupDict[planet.VO.Uid] = planet;
				planet.Tournament = null;
				TournamentVO activeTournamentOnPlanet = TournamentController.GetActiveTournamentOnPlanet(planet.VO.Uid);
				if (activeTournamentOnPlanet != null)
				{
					TimedEventState state = TimedEventUtils.GetState(activeTournamentOnPlanet);
					if (state == TimedEventState.Live)
					{
						this.planetsWithEvent.Add(planet);
						planet.Tournament = activeTournamentOnPlanet;
						this.planetsWithActiveEvents.Add(planet);
					}
					else if (state == TimedEventState.Upcoming)
					{
						this.planetsWithEvent.Add(planet);
					}
				}
				planet.UpdatePlanetTournamentState();
				assetManager.Load(ref handle, planet.VO.GalaxyAssetName, new AssetSuccessDelegate(this.OnPlanetLoaded), null, planet);
				planet.Handle = handle;
				i++;
			}
			this.UpdatePlanetsFriendData();
			this.UpdatePlanetsSquadData();
			if (planetStatsCommand != null)
			{
				Service.ServerAPI.Sync(planetStatsCommand);
			}
			this.planets.Sort(new Comparison<Planet>(this.ComparePlanetOrder));
		}

		private int ComparePlanetOrder(Planet a, Planet b)
		{
			if (a == b)
			{
				return 0;
			}
			return (a.VO.Order >= b.VO.Order) ? -1 : 1;
		}

		private void UpdatePlanetIndex()
		{
			int count = this.planets.Count;
			int i;
			for (i = 0; i < count; i++)
			{
				if (this.currentlyForegroundedPlanet == this.planets[i])
				{
					break;
				}
			}
			if (i < count)
			{
				this.planetIndex = i;
			}
		}

		public Planet TransitionToPlanet(bool next)
		{
			this.planetIndex += ((!next) ? 1 : -1);
			if (this.planetIndex < 0)
			{
				this.planetIndex = this.planets.Count - 1;
			}
			else if (this.planetIndex >= this.planets.Count)
			{
				this.planetIndex = 0;
			}
			return this.planets[this.planetIndex];
		}

		public void UpdatePlanetsFriendData()
		{
			foreach (Planet current in this.planetLookupDict.Values)
			{
				current.FriendsOnPlanet.Clear();
			}
			List<SocialFriendData> friends = Service.ISocialDataController.Friends;
			if (friends != null)
			{
				int i = 0;
				int count = friends.Count;
				while (i < count)
				{
					if (friends[i].PlayerData != null)
					{
						string text = friends[i].PlayerData.Planet;
						if (text.Equals(string.Empty))
						{
							text = "planet1";
						}
						if (this.planetLookupDict.ContainsKey(text))
						{
							this.planetLookupDict[text].FriendsOnPlanet.Add(friends[i]);
						}
					}
					i++;
				}
			}
		}

		private void UpdatePlanetsSquadData()
		{
			foreach (KeyValuePair<string, Planet> current in this.planetLookupDict)
			{
				current.Value.NumSquadmatesOnPlanet = 0;
			}
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			if (currentSquad != null)
			{
				List<SquadMember> memberList = currentSquad.MemberList;
				for (int i = 0; i < memberList.Count; i++)
				{
					if (memberList[i].MemberID != currentPlayer.PlayerId)
					{
						string text = memberList[i].Planet;
						if (text.Equals(string.Empty))
						{
							text = "planet1";
						}
						if (this.planetLookupDict.ContainsKey(text))
						{
							this.planetLookupDict[text].NumSquadmatesOnPlanet++;
						}
					}
				}
			}
		}

		private void BillboardPlanetObject(GameObject obj)
		{
			if (obj != null)
			{
				MainCamera mainCamera = Service.CameraManager.MainCamera;
				obj.transform.LookAt(mainCamera.Camera.transform.position);
			}
		}

		private void OffsetPlanetObject(GameObject obj, Vector3 offset)
		{
			if (obj != null)
			{
				obj.transform.localPosition = offset;
			}
		}

		private void RebuildPlanetEvent(Planet planet)
		{
			GalaxyViewController galaxyViewController = Service.GalaxyViewController;
			MiscElementsManager miscElementsManager = Service.UXController.MiscElementsManager;
			this.UpdateEventEffectStatus(planet);
			AssetManager assetManager = Service.AssetManager;
			if (planet.ParticleFXHandle != AssetHandle.Invalid)
			{
				assetManager.Unload(planet.ParticleFXHandle);
				planet.ParticleFXHandle = AssetHandle.Invalid;
			}
			planet.DestroyParticles();
			this.planetParticleCount = Mathf.Max(0, --this.planetParticleCount);
			if (planet.Tournament == null)
			{
				if (this.planetsWithActiveEvents.Contains(planet))
				{
					this.planetsWithActiveEvents.Remove(planet);
				}
				if (this.planetsWithEvent.Contains(planet))
				{
					this.planetsWithEvent.Remove(planet);
				}
				galaxyViewController.HideBackgroundPlanetUI(planet);
			}
			else
			{
				AssetHandle particleFXHandle = AssetHandle.Invalid;
				TimedEventState tournamentState = planet.TournamentState;
				TimedEventState timedEventState = tournamentState;
				if (timedEventState != TimedEventState.Upcoming)
				{
					if (timedEventState != TimedEventState.Live)
					{
						if (this.planetsWithEvent.Contains(planet))
						{
							this.planetsWithEvent.Remove(planet);
						}
						if (this.planetsWithActiveEvents.Contains(planet))
						{
							this.planetsWithActiveEvents.Remove(planet);
						}
					}
					else
					{
						if (!this.planetsWithEvent.Contains(planet))
						{
							this.planetsWithEvent.Add(planet);
						}
						if (!this.planetsWithActiveEvents.Contains(planet))
						{
							this.planetsWithActiveEvents.Add(planet);
						}
						assetManager.Load(ref particleFXHandle, "fx_planet_Conflict", new AssetSuccessDelegate(this.OnEventParticleFXLoaded), null, planet);
					}
				}
				else
				{
					if (!this.planetsWithEvent.Contains(planet))
					{
						this.planetsWithEvent.Add(planet);
					}
					assetManager.Load(ref particleFXHandle, "fx_planet_Imminent", new AssetSuccessDelegate(this.OnEventParticleFXLoaded), null, planet);
				}
				planet.ParticleFXHandle = particleFXHandle;
				miscElementsManager.DestroyPlanetBackgroundUI(planet);
				miscElementsManager.CreatePlanetBackgroundUI(planet);
			}
			miscElementsManager.UpdateFrontPlanetEventTimer();
		}

		public void UpdatePlanets()
		{
			int count = this.planets.Count;
			bool flag = this.AreAllPlanetsLoaded();
			GalaxyViewController galaxyViewController = Service.GalaxyViewController;
			for (int i = 0; i < count; i++)
			{
				Planet planet = this.planets[i];
				if (flag && planet.UpdatePlanetTournamentState())
				{
					this.RebuildPlanetEvent(planet);
				}
				this.BillboardPlanetObject(planet.EventEffect);
				this.BillboardPlanetObject(planet.PlanetGameObject);
				this.BillboardPlanetObject(planet.PlanetExplosions);
				this.BillboardPlanetObject(planet.PlanetGlowEffect);
				if (planet.PlanetGameObject != null && galaxyViewController.IsInPlanetScreen())
				{
					planet.PlanetGlowEffect.transform.localScale = Vector3.one;
					this.OffsetPlanetObject(planet.PlanetGlowEffect, Vector3.zero);
				}
				else if (planet.PlanetGlowEffect != null)
				{
					planet.PlanetGlowEffect.transform.localScale = Vector3.one * 1.15f;
					this.OffsetPlanetObject(planet.PlanetGlowEffect, GalaxyPlanetController.PLANET_GLOW_OFFSET);
				}
				if (planet.planetMaterial != null)
				{
					this.SetPlanetLighting(planet.PlanetGameObject, planet.planetMaterial, Service.CameraManager.MainCamera.Camera);
				}
			}
		}

		public void DestroyAllPlanets()
		{
			AssetManager assetManager = Service.AssetManager;
			this.planetIndex = 0;
			int count = this.planets.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.planets[i].Handle != AssetHandle.Invalid)
				{
					assetManager.Unload(this.planets[i].Handle);
					this.planets[i].Handle = AssetHandle.Invalid;
				}
				if (this.planets[i].EventEffectsHandle != AssetHandle.Invalid)
				{
					assetManager.Unload(this.planets[i].EventEffectsHandle);
					this.planets[i].EventEffectsHandle = AssetHandle.Invalid;
				}
				if (this.planets[i].ParticleFXHandle != AssetHandle.Invalid)
				{
					assetManager.Unload(this.planets[i].ParticleFXHandle);
					this.planets[i].ParticleFXHandle = AssetHandle.Invalid;
				}
				if (this.planets[i].PlanetGlowHandle != AssetHandle.Invalid)
				{
					assetManager.Unload(this.planets[i].PlanetGlowHandle);
					this.planets[i].PlanetGlowHandle = AssetHandle.Invalid;
				}
				this.planets[i].Destroy();
			}
			Service.UXController.MiscElementsManager.DestroyAllPlanetBackgroundUI();
			Service.UXController.MiscElementsManager.DestroyPlanetLockedUI();
			this.planets.Clear();
			this.loadedPlanetsCount = 0;
			this.planetParticleCount = 0;
			this.planetEffectsCount = 0;
		}

		public bool DoesPlanetHaveEvent(Planet planet)
		{
			return this.planetsWithEvent.Contains(planet);
		}

		public void UpdateEventEffectStatus(Planet planet)
		{
			GameObject eventEffect = planet.EventEffect;
			if (eventEffect != null)
			{
				if (planet.TournamentState == TimedEventState.Live)
				{
					eventEffect.SetActive(true);
					Animation component = planet.EventEffect.GetComponent<Animation>();
					component.Play("Idle");
				}
				else
				{
					eventEffect.SetActive(false);
				}
			}
		}

		private void UpdateAllPlanetEffects()
		{
			int count = this.planets.Count;
			for (int i = 0; i < count; i++)
			{
				this.UpdateEventEffectStatus(this.planets[i]);
			}
		}

		private void ActivateAllPlanetParticleFX()
		{
			int count = this.planetsWithEvent.Count;
			for (int i = 0; i < count; i++)
			{
				PlanetFXState currentPlanetFXState = this.planetsWithEvent[i].CurrentPlanetFXState;
				if (currentPlanetFXState != PlanetFXState.AllFXOff)
				{
					if (currentPlanetFXState != PlanetFXState.RingsOff)
					{
						this.ActivatePlanetParticleFX(this.planetsWithEvent[i]);
					}
					else
					{
						this.ActivatePlanetParticleFX(this.planetsWithEvent[i]);
						this.DeactivatePlanetRings(this.planetsWithEvent[i]);
					}
				}
				else
				{
					this.DeactivatePlanetParticleFX(this.planetsWithEvent[i]);
				}
			}
		}

		public void ActivatePlanetParticleFX(Planet planet)
		{
			GameObject particleFX = planet.ParticleFX;
			planet.CurrentPlanetFXState = PlanetFXState.AllFXOn;
			if (particleFX != null)
			{
				particleFX.SetActive(true);
			}
		}

		public void DeactivatePlanetParticleFX(Planet planet)
		{
			GameObject particleFX = planet.ParticleFX;
			planet.CurrentPlanetFXState = PlanetFXState.AllFXOff;
			if (particleFX != null)
			{
				particleFX.SetActive(false);
			}
		}

		public void ActivateAllPlanetRings()
		{
			int count = this.planetsWithEvent.Count;
			for (int i = 0; i < count; i++)
			{
				this.ActivatePlanetRings(this.planetsWithEvent[i]);
			}
		}

		public void ActivatePlanetRings(Planet planet)
		{
			if (planet.Tournament != null)
			{
				TimedEventState tournamentState = planet.TournamentState;
				if (tournamentState == TimedEventState.Live || tournamentState == TimedEventState.Upcoming)
				{
					ParticleSystem particleRings = planet.ParticleRings;
					planet.CurrentPlanetFXState = PlanetFXState.AllFXOn;
					if (particleRings != null)
					{
						particleRings.gameObject.SetActive(true);
					}
				}
			}
		}

		public void DeactivatePlanetRings(Planet planet)
		{
			ParticleSystem particleRings = planet.ParticleRings;
			if (planet.CurrentPlanetFXState != PlanetFXState.AllFXOff)
			{
				planet.CurrentPlanetFXState = PlanetFXState.RingsOff;
			}
			if (particleRings != null)
			{
				particleRings.gameObject.SetActive(false);
			}
		}

		private Vector3 GetPlanetVect3Position(float angle, float radius, float heightOffset)
		{
			Vector3 result = new Vector3(0f, 10000f, 0f);
			float f = 0.0174532924f * angle;
			float x = radius * Mathf.Cos(f);
			float z = radius * Mathf.Sin(f);
			result.x = x;
			result.y += heightOffset;
			result.z = z;
			return result;
		}

		private void CheckForEffectAndFXDoneLoading()
		{
			if (this.AreAllEffectsAndParticlesLoaded())
			{
				this.ActivateAllPlanetParticleFX();
				this.UpdateAllPlanetEffects();
				Service.UXController.MiscElementsManager.ShowEventsTickerView();
			}
		}

		private void OnEventEffectsLoaded(object asset, object cookie)
		{
			if (Service.ScreenController.IsFatalAlertActive())
			{
				return;
			}
			MainCamera mainCamera = Service.CameraManager.MainCamera;
			Planet planet = (Planet)cookie;
			GameObject gameObject = (GameObject)asset;
			GameObject expr_2A = gameObject;
			expr_2A.name += planet.VO.Uid;
			gameObject.transform.parent = planet.PlanetGameObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			planet.EventEffect = gameObject;
			gameObject.transform.LookAt(mainCamera.Camera.transform.position);
			this.planetEffectsCount++;
			this.CheckForEffectAndFXDoneLoading();
		}

		private void OnEventParticleFXLoaded(object asset, object cookie)
		{
			if (Service.ScreenController.IsFatalAlertActive())
			{
				return;
			}
			MainCamera mainCamera = Service.CameraManager.MainCamera;
			Planet planet = (Planet)cookie;
			GameObject gameObject = (GameObject)asset;
			gameObject = UnityEngine.Object.Instantiate<GameObject>(gameObject);
			GameObject expr_31 = gameObject;
			expr_31.name += planet.VO.Uid;
			gameObject.transform.parent = planet.PlanetGameObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.SetActive(false);
			planet.ParticleFX = gameObject;
			Transform transform = gameObject.transform;
			Transform transform2 = transform.FindChild("Rings");
			Transform transform3 = transform.FindChild("planetExplosions");
			if (transform3 != null)
			{
				planet.PlanetExplosions = transform3.gameObject;
			}
			planet.ParticleRings = transform2.GetComponent<ParticleSystem>();
			planet.OriginalRingSize = planet.ParticleRings.startSize;
			gameObject.transform.LookAt(mainCamera.Camera.transform.position);
			this.planetParticleCount++;
			this.CheckForEffectAndFXDoneLoading();
		}

		private void OnPlanetGlowEffectLoaded(object asset, object cookie)
		{
			if (Service.ScreenController.IsFatalAlertActive())
			{
				return;
			}
			MainCamera mainCamera = Service.CameraManager.MainCamera;
			Planet planet = (Planet)cookie;
			GameObject original = (GameObject)asset;
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original);
			gameObject.SetActive(true);
			GameObject expr_38 = gameObject;
			expr_38.name += planet.VO.Uid;
			gameObject.transform.parent = planet.PlanetGameObject.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one * 1.15f;
			planet.PlanetGlowEffect = gameObject;
			gameObject.transform.LookAt(mainCamera.Camera.transform.position);
			this.loadedPlanetsCount++;
			if (this.AreAllPlanetsLoaded())
			{
				Service.EventManager.SendEvent(EventId.PlanetsLoadingComplete, null);
			}
		}

		private void OnParticleFXLoadFailed(object cookie)
		{
			Service.Logger.Error("Failed to Load Particle FX For Planet");
			this.planetParticleCount++;
		}

		private void OnEffectLoadFailed(object cookie)
		{
			Service.Logger.Error("Failed to Load Effect For Planet");
			this.planetEffectsCount++;
		}

		private void OnPlanetLoaded(object asset, object cookie)
		{
			if (Service.ScreenController.IsFatalAlertActive())
			{
				return;
			}
			MainCamera mainCamera = Service.CameraManager.MainCamera;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			AssetManager assetManager = Service.AssetManager;
			AssetHandle eventEffectsHandle = AssetHandle.Invalid;
			AssetHandle particleFXHandle = AssetHandle.Invalid;
			AssetHandle planetGlowHandle = AssetHandle.Invalid;
			Planet planet = (Planet)cookie;
			GameObject gameObject = (GameObject)asset;
			planet.PlanetGameObject = gameObject;
			Transform transform = gameObject.transform;
			transform.position = this.GetPlanetVect3Position(planet.VO.Angle, planet.VO.Radius, -planet.VO.HeightOffset);
			planet.planetMaterial = PlanetUtils.GetPlanetMaterial(gameObject);
			Bounds gameObjectLocalBounds = UnityUtils.GetGameObjectLocalBounds(gameObject, false);
			planet.ObjectExtents = gameObjectLocalBounds.extents;
			PlanetRef planetRef = gameObject.AddComponent<PlanetRef>();
			planetRef.Planet = planet;
			BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
			boxCollider.size = gameObjectLocalBounds.size;
			Service.UXController.MiscElementsManager.CreatePlanetBackgroundUI(planet);
			planet.planetMaterial = PlanetUtils.GetPlanetMaterial(gameObject);
			if (!currentPlayer.IsPlanetUnlocked(planet.VO.Uid))
			{
				Service.UXController.MiscElementsManager.CreatePlanetLockedUI(planet);
				planet.planetMaterial.SetFloat("_Bright", 0.35f);
			}
			assetManager.Load(ref planetGlowHandle, "fx_planetGlow", new AssetSuccessDelegate(this.OnPlanetGlowEffectLoaded), null, planet);
			planet.PlanetGlowHandle = planetGlowHandle;
			string text = string.Empty;
			if (currentPlayer.Faction == FactionType.Empire)
			{
				text = "tiefighterplanet_emp-ani";
			}
			else if (currentPlayer.Faction == FactionType.Rebel)
			{
				text = "xwingplanet_rbl-ani";
			}
			if (!string.IsNullOrEmpty(text))
			{
				assetManager.Load(ref eventEffectsHandle, text, new AssetSuccessDelegate(this.OnEventEffectsLoaded), new AssetFailureDelegate(this.OnEffectLoadFailed), planet);
			}
			if (this.DoesPlanetHaveEvent(planet))
			{
				TimedEventState tournamentState = planet.TournamentState;
				TimedEventState timedEventState = tournamentState;
				if (timedEventState != TimedEventState.Upcoming)
				{
					if (timedEventState == TimedEventState.Live)
					{
						assetManager.Load(ref particleFXHandle, "fx_planet_Conflict", new AssetSuccessDelegate(this.OnEventParticleFXLoaded), new AssetFailureDelegate(this.OnParticleFXLoadFailed), planet);
					}
				}
				else
				{
					assetManager.Load(ref particleFXHandle, "fx_planet_Imminent", new AssetSuccessDelegate(this.OnEventParticleFXLoaded), new AssetFailureDelegate(this.OnParticleFXLoadFailed), planet);
				}
			}
			planet.EventEffectsHandle = eventEffectsHandle;
			planet.ParticleFXHandle = particleFXHandle;
			transform.LookAt(mainCamera.Camera.transform.position);
		}

		public void SetPlanetLighting(GameObject planet, Material material, Camera camera)
		{
			if (material == null || planet == null || camera == null)
			{
				Service.Logger.Warn("SetPlanetLighting invalid");
				return;
			}
			Vector3 position = camera.transform.position;
			Quaternion rotation = camera.transform.rotation;
			camera.transform.LookAt(planet.transform.position);
			Vector3 v = camera.transform.TransformDirection(GalaxyPlanetController.Global_LightDir);
			material.SetVector("_LightDir", v);
			camera.transform.position = position;
			camera.transform.rotation = rotation;
		}
	}
}
