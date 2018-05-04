using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.World
{
	public class TrapRadiusView : RadiusView
	{
		private const string ASSET_NAME = "fx_trap_radius";

		private const string TRIGGER_RADIUS = "fx_trap_trigger_radius";

		private const string DAMAGE_RADIUS = "fx_trap_damage_radius";

		private ParticleSystem triggerRadius;

		private ParticleSystem damageRadius;

		public TrapRadiusView() : base("fx_trap_radius")
		{
		}

		protected override void SetupAssetOnLoad()
		{
			base.TryFindParticleSystem(ref this.triggerRadius, "fx_trap_trigger_radius");
			base.TryFindParticleSystem(ref this.damageRadius, "fx_trap_damage_radius");
		}

		protected override bool SetupParticlesOnShow(Entity entity)
		{
			BuildingTypeVO buildingType = entity.Get<BuildingComponent>().BuildingType;
			if (buildingType.Type == BuildingType.Trap && entity.Get<TrapComponent>().CurrentState != TrapState.Spent && !ContractUtils.IsBuildingUpgrading(entity) && !ContractUtils.IsBuildingConstructing(entity))
			{
				TrapTypeVO trapType = Service.StaticDataController.Get<TrapTypeVO>(buildingType.TrapUid);
				uint trapMaxRadius = TrapUtils.GetTrapMaxRadius(trapType);
				uint trapAttackRadius = TrapUtils.GetTrapAttackRadius(trapType);
				base.SetupParticleSystemWithRange(this.triggerRadius, 5f, trapMaxRadius);
				base.SetupParticleSystemWithRange(this.damageRadius, 5f, trapAttackRadius);
				return true;
			}
			return false;
		}
	}
}
