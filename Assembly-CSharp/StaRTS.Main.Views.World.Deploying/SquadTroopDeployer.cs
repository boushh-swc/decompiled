using StaRTS.DataStructures;
using StaRTS.Main.Utils;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.World.Deploying
{
	public class SquadTroopDeployer : AbstractDeployer
	{
		public bool EnterPlacementMode()
		{
			if (!Service.SquadTroopAttackController.Spawning)
			{
				this.EnterMode();
				return true;
			}
			return false;
		}

		public override EatResponse OnPress(GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			return EatResponse.NotEaten;
		}

		public override EatResponse OnDrag(GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			return EatResponse.NotEaten;
		}

		public override EatResponse OnRelease()
		{
			if (base.IsNotDraggedAndReleasingOwnPress() && !Service.SquadTroopAttackController.Spawning && !Service.SimTimeEngine.IsPaused() && !Service.BattleController.BattleEndProcessing && Service.BattleController.CanPlayerDeploySquadTroops())
			{
				IntPosition boardPos = Units.WorldToBoardIntDeployPosition(this.currentWorldPosition);
				Service.SquadTroopAttackController.DeploySquadTroops(boardPos);
			}
			return EatResponse.NotEaten;
		}
	}
}
