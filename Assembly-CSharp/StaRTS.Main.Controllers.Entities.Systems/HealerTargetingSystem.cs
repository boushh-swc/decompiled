using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Entities.Systems
{
	public class HealerTargetingSystem : SimSystemBase
	{
		private NodeList<OffensiveHealerNode> offensiveHealerNodeList;

		private NodeList<DefensiveHealerNode> defensiveHealerNodeList;

		private int flip;

		private TargetingController targetingController;

		public override void AddToGame(Game game)
		{
			this.targetingController = Service.TargetingController;
			EntityController entityController = Service.EntityController;
			this.offensiveHealerNodeList = entityController.GetNodeList<OffensiveHealerNode>();
			this.defensiveHealerNodeList = entityController.GetNodeList<DefensiveHealerNode>();
			this.flip = 0;
		}

		public override void RemoveFromGame(Game game)
		{
		}

		protected override void Update(uint dt)
		{
			this.targetingController.Update(ref this.flip, new TargetingController.UpdateTarget(this.UpdateOffensiveHealerTarget), new TargetingController.UpdateTarget(this.UpdateDefensiveHealerTarget), new TargetingController.UpdateTarget(this.UpdateOffensiveTroopPeriodicUpdate));
		}

		private void OnTargetingDone(SmartEntity entity)
		{
		}

		private bool ShouldEvaluate(SmartEntity troopEntity)
		{
			return true;
		}

		private void UpdateOffensiveHealerTarget(ref int numTargetingDone)
		{
			this.targetingController.UpdateNodes<OffensiveHealerNode>(this.offensiveHealerNodeList, ref numTargetingDone, new TargetingController.OnTargetingDone(this.OnTargetingDone), false);
		}

		private void UpdateDefensiveHealerTarget(ref int numTargetingDone)
		{
			this.targetingController.UpdateNodes<DefensiveHealerNode>(this.defensiveHealerNodeList, ref numTargetingDone, new TargetingController.OnTargetingDone(this.OnTargetingDone), false);
		}

		private void UpdateOffensiveTroopPeriodicUpdate(ref int numTargetingDone)
		{
		}
	}
}
