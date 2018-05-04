using Net.RichardLord.Ash.Core;
using StaRTS.DataStructures;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models
{
	public class SpatialIndex
	{
		private SmartEntityPriorityList nearnessToBuildings;

		private List<EntityElementPriorityPair> turretsInRange;

		private List<EntityElementPriorityPair> areaTriggerBuildingsInRange;

		public bool AlreadyScannedBuildingsToAttack
		{
			get;
			set;
		}

		public bool AlreadyScannedTurretsInRange
		{
			get;
			set;
		}

		public bool AlreadyScannedAreaTriggerBuildingsInRange
		{
			get;
			set;
		}

		public SpatialIndex()
		{
			this.nearnessToBuildings = new SmartEntityPriorityList();
			this.turretsInRange = new List<EntityElementPriorityPair>();
			this.areaTriggerBuildingsInRange = new List<EntityElementPriorityPair>();
			this.AlreadyScannedBuildingsToAttack = false;
			this.AlreadyScannedTurretsInRange = false;
			this.AlreadyScannedAreaTriggerBuildingsInRange = false;
		}

		public void ResetTurretScanedFlag()
		{
			this.AlreadyScannedTurretsInRange = false;
		}

		public void AddBuildingsToAttack(SmartEntity entity, int nearness)
		{
			this.nearnessToBuildings.Add(entity, nearness);
		}

		public void AddTurretsInRangeOf(Entity entity, int distanceSquared, int nearness)
		{
			ShooterComponent shooterComponent = entity.Get<ShooterComponent>();
			if (shooterComponent == null)
			{
				return;
			}
			if (Service.ShooterController.InRange(distanceSquared, shooterComponent))
			{
				this.turretsInRange.Add(new EntityElementPriorityPair(entity, nearness));
			}
		}

		public void AddAreaTriggerBuildingsInRangeOf(Entity entity, int distanceSquared, int nearness)
		{
			AreaTriggerComponent areaTriggerComponent = entity.Get<AreaTriggerComponent>();
			if (areaTriggerComponent == null)
			{
				return;
			}
			if ((long)distanceSquared <= (long)((ulong)areaTriggerComponent.RangeSquared))
			{
				this.areaTriggerBuildingsInRange.Add(new EntityElementPriorityPair(entity, nearness));
			}
		}

		public List<EntityElementPriorityPair> GetTurretsInRangeOf()
		{
			return this.turretsInRange;
		}

		public List<EntityElementPriorityPair> GetArareaTriggerBuildingsInRange()
		{
			return this.areaTriggerBuildingsInRange;
		}

		public SmartEntityPriorityList GetBuildingsToAttack()
		{
			return this.nearnessToBuildings;
		}
	}
}
