using StaRTS.Assets;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.Planets
{
	public class Planet
	{
		private float THRASH_TIME;

		private float THRASH_DELTA_PERCENT;

		private const int THRASH_CAP = 20;

		public AssetHandle Handle;

		public AssetHandle EventEffectsHandle;

		public AssetHandle ParticleFXHandle;

		public AssetHandle PlanetGlowHandle;

		public PlanetFXState CurrentPlanetFXState;

		public Material planetMaterial;

		private TimedEventState tournamentState;

		private TournamentVO tournamentVO;

		private int actualPopulation;

		private int thrashValue;

		private float thrashTimer;

		private int thrashDelta;

		public GameObject PlanetGameObject
		{
			get;
			set;
		}

		public GameObject EventEffect
		{
			get;
			set;
		}

		public GameObject ParticleFX
		{
			get;
			set;
		}

		public GameObject PlanetGlowEffect
		{
			get;
			set;
		}

		public GameObject PlanetExplosions
		{
			get;
			set;
		}

		public ParticleSystem ParticleRings
		{
			get;
			set;
		}

		public float OriginalRingSize
		{
			get;
			set;
		}

		public PlanetVO VO
		{
			get;
			set;
		}

		public Vector3 ObjectExtents
		{
			get;
			set;
		}

		public TournamentVO Tournament
		{
			get
			{
				return this.tournamentVO;
			}
			set
			{
				this.tournamentVO = value;
				this.tournamentState = ((this.tournamentVO == null) ? TimedEventState.Invalid : TimedEventUtils.GetState(this.tournamentVO));
			}
		}

		public TimedEventState TournamentState
		{
			get
			{
				return this.tournamentState;
			}
		}

		public TimedEventCountdownHelper TournamentCountdown
		{
			get;
			set;
		}

		public bool IsCurrentPlanet
		{
			get;
			set;
		}

		public bool CurrentBackUIShown
		{
			get;
			set;
		}

		public bool IsForegrounded
		{
			get;
			set;
		}

		public List<SocialFriendData> FriendsOnPlanet
		{
			get;
			set;
		}

		public int NumSquadmatesOnPlanet
		{
			get;
			set;
		}

		public int ThrashingPopulation
		{
			get
			{
				return this.actualPopulation + this.thrashValue;
			}
			set
			{
				this.actualPopulation = value;
			}
		}

		public Planet(PlanetVO vo)
		{
			this.THRASH_TIME = GameConstants.GALAXY_PLANET_POPULATION_UPDATE_TIME;
			this.THRASH_DELTA_PERCENT = GameConstants.GALAXY_PLANET_POPULATION_COUNT_PERCENTAGE;
			this.VO = vo;
			this.thrashTimer = 0f;
			this.ThrashingPopulation = 0;
			this.thrashValue = 0;
			this.IsCurrentPlanet = false;
			this.CurrentBackUIShown = false;
			this.FriendsOnPlanet = new List<SocialFriendData>();
			this.CurrentPlanetFXState = PlanetFXState.Unknown;
			this.tournamentState = TimedEventState.Invalid;
		}

		public bool UpdatePlanetTournamentState()
		{
			bool result = false;
			TournamentVO activeTournamentOnPlanet = TournamentController.GetActiveTournamentOnPlanet(this.VO.Uid);
			if (activeTournamentOnPlanet != this.tournamentVO)
			{
				result = true;
			}
			else if (activeTournamentOnPlanet != null)
			{
				TimedEventState state = TimedEventUtils.GetState(this.tournamentVO);
				if (state != this.tournamentState)
				{
					result = true;
				}
			}
			this.Tournament = activeTournamentOnPlanet;
			return result;
		}

		public bool IsCurrentAndNeedsAnim()
		{
			return this.IsCurrentPlanet && !this.CurrentBackUIShown;
		}

		public void UpdateThrashingPopulation(float dt)
		{
			this.thrashTimer += dt;
			if (this.thrashTimer > this.THRASH_TIME)
			{
				this.thrashTimer = 0f;
				this.thrashDelta = (int)(this.THRASH_DELTA_PERCENT * (float)this.actualPopulation);
				this.thrashValue += Service.Rand.ViewRangeInt(-this.thrashDelta, this.thrashDelta);
				if (this.thrashValue > 20)
				{
					this.thrashValue = 20;
				}
				else if (this.thrashValue < -20)
				{
					this.thrashValue = -20;
					this.thrashValue = Mathf.Max(this.thrashValue, -this.actualPopulation);
				}
			}
		}

		public void DestroyParticles()
		{
			if (this.ParticleFX != null)
			{
				UnityEngine.Object.Destroy(this.ParticleFX);
				this.ParticleFX = null;
			}
			this.ParticleRings = null;
			this.PlanetExplosions = null;
		}

		public void Destroy()
		{
			if (this.planetMaterial != null)
			{
				UnityUtils.DestroyMaterial(this.planetMaterial);
				this.planetMaterial = null;
			}
			if (this.PlanetGameObject != null)
			{
				UnityEngine.Object.Destroy(this.PlanetGameObject);
				this.PlanetGameObject = null;
			}
			if (this.EventEffect != null)
			{
				UnityEngine.Object.Destroy(this.EventEffect);
				this.EventEffect = null;
			}
			if (this.PlanetGlowEffect != null)
			{
				UnityEngine.Object.Destroy(this.PlanetGlowEffect);
				this.PlanetGlowEffect = null;
			}
			this.DestroyParticles();
			this.Handle = AssetHandle.Invalid;
			this.PlanetGameObject = null;
			this.VO = null;
			this.ObjectExtents = Vector3.zero;
		}
	}
}
