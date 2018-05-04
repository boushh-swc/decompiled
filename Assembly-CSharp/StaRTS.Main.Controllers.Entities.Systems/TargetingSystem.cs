using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Entities.Systems
{
	public class TargetingSystem : SimSystemBase
	{
		private NodeList<OffensiveTroopNode> offensiveTroopNodeList;

		private NodeList<DefensiveTroopNode> defensiveTroopNodeList;

		private NodeList<TurretNode> turretNodeList;

		private int flip;

		private TargetingController targetingController;

		public override void AddToGame(Game game)
		{
			this.targetingController = Service.TargetingController;
			EntityController entityController = Service.EntityController;
			this.offensiveTroopNodeList = entityController.GetNodeList<OffensiveTroopNode>();
			this.defensiveTroopNodeList = entityController.GetNodeList<DefensiveTroopNode>();
			this.turretNodeList = entityController.GetNodeList<TurretNode>();
			this.flip = 0;
		}

		public override void RemoveFromGame(Game game)
		{
		}

		protected override void Update(uint dt)
		{
			this.targetingController.Update(ref this.flip, new TargetingController.UpdateTarget(this.UpdateOffensiveTroopTarget), new TargetingController.UpdateTarget(this.UpdateDefensiveTroopTarget), new TargetingController.UpdateTarget(this.UpdateOffensiveTroopPeriodicUpdate));
			for (TurretNode turretNode = this.turretNodeList.Head; turretNode != null; turretNode = turretNode.Next)
			{
				this.targetingController.StopSearchIfTargetFound(turretNode.ShooterComp);
			}
		}

		private void OnTargetingDone(SmartEntity entity)
		{
			TroopComponent troopComp = entity.ShooterComp.Target.TroopComp;
			if (troopComp != null)
			{
				troopComp.TargetCount++;
			}
		}

		private void UpdateOffensiveTroopTarget(ref int numTargetingDone)
		{
			this.targetingController.UpdateNodes<OffensiveTroopNode>(this.offensiveTroopNodeList, ref numTargetingDone, new TargetingController.OnTargetingDone(this.OnTargetingDone), false);
		}

		private void UpdateDefensiveTroopTarget(ref int numTargetingDone)
		{
			this.targetingController.UpdateNodes<DefensiveTroopNode>(this.defensiveTroopNodeList, ref numTargetingDone, new TargetingController.OnTargetingDone(this.OnTargetingDone), false);
		}

		private void UpdateOffensiveTroopPeriodicUpdate(ref int numTargetingDone)
		{
			this.targetingController.UpdateNodes<OffensiveTroopNode>(this.offensiveTroopNodeList, ref numTargetingDone, new TargetingController.OnTargetingDone(this.OnTargetingDone), true);
		}
	}
}
