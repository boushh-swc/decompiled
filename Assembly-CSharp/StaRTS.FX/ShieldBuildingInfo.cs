using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.FX
{
	public class ShieldBuildingInfo
	{
		public Entity Building
		{
			get;
			private set;
		}

		public List<AssetHandle> AssetHandles
		{
			get;
			set;
		}

		public Dictionary<string, GameObject> EffectAssets
		{
			get;
			private set;
		}

		public List<ShieldReason> Reasons
		{
			get;
			private set;
		}

		public bool LoadComplete
		{
			get;
			set;
		}

		public GameObject Shield
		{
			get;
			set;
		}

		public ParticleSystem Spark
		{
			get;
			set;
		}

		public ParticleSystem Destruction
		{
			get;
			set;
		}

		public GameObject Generator
		{
			get;
			set;
		}

		public GameObject Top
		{
			get;
			set;
		}

		public Material DecalMaterial
		{
			get;
			set;
		}

		public Material ShieldMaterial
		{
			get;
			set;
		}

		public ShieldDissolve ShieldDisolveEffect
		{
			get;
			set;
		}

		public ShieldBuildingInfo(Entity building, List<string> effectUids)
		{
			this.Building = building;
			this.EffectAssets = new Dictionary<string, GameObject>();
			int i = 0;
			int count = effectUids.Count;
			while (i < count)
			{
				this.EffectAssets.Add(effectUids[i], null);
				i++;
			}
			this.Reasons = new List<ShieldReason>();
			this.LoadComplete = false;
			this.ShieldDisolveEffect = new ShieldDissolve();
		}

		public void PlayShieldDisolveEffect(bool turnOn, DissolveCompleteDelegate OnCompleteCallback)
		{
			this.ShieldDisolveEffect.Play(this.Shield, this.ShieldMaterial, turnOn, OnCompleteCallback, this);
		}
	}
}
