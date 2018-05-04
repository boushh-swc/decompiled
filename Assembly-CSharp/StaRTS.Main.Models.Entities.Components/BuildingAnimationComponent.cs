using Net.RichardLord.Ash.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.Entities.Components
{
	public class BuildingAnimationComponent : ComponentBase
	{
		public bool BuildingUpgrading
		{
			get;
			set;
		}

		public bool Manufacturing
		{
			get;
			set;
		}

		public Animation Anim
		{
			get;
			set;
		}

		public List<ParticleSystem> ListOfParticleSystems
		{
			get;
			set;
		}

		public BuildingAnimationComponent(Animation anim, List<ParticleSystem> listOfParticleSystems)
		{
			this.Anim = anim;
			this.ListOfParticleSystems = listOfParticleSystems;
			this.BuildingUpgrading = false;
			this.Manufacturing = false;
		}
	}
}
