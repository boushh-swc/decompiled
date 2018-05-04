using StaRTS.DataStructures;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.World.Deploying
{
	public class SpecialAttackDeployer : AbstractDeployer
	{
		private SpecialAttackTypeVO currentSpecialAttackType;

		public SpecialAttackDeployer()
		{
			this.currentSpecialAttackType = null;
		}

		public bool EnterPlacementMode(SpecialAttackTypeVO specialAttackType)
		{
			if (this.currentSpecialAttackType != null)
			{
				this.ExitMode();
			}
			this.currentSpecialAttackType = specialAttackType;
			this.EnterMode();
			return true;
		}

		public override void ExitMode()
		{
			base.ExitMode();
			this.currentSpecialAttackType = null;
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
			if (base.IsNotDraggedAndReleasingOwnPress())
			{
				this.DeploySpecialAttack();
			}
			return EatResponse.NotEaten;
		}

		private SpecialAttack DeploySpecialAttack()
		{
			if (Service.SimTimeEngine.IsPaused())
			{
				return null;
			}
			BattleController battleController = Service.BattleController;
			if (battleController.BattleEndProcessing)
			{
				return null;
			}
			if (battleController.GetPlayerDeployableSpecialAttackCount(this.currentSpecialAttackType.Uid) == 0)
			{
				Service.EventManager.SendEvent(EventId.TroopNotPlacedInvalidTroop, this.currentWorldPosition);
				return null;
			}
			if (this.currentSpecialAttackType != null)
			{
				TeamType teamType = TeamType.Attacker;
				if (battleController.GetCurrentBattle().Type == BattleType.PveDefend)
				{
					teamType = TeamType.Defender;
				}
				SpecialAttack specialAttack = Service.SpecialAttackController.DeploySpecialAttack(this.currentSpecialAttackType, teamType, this.currentWorldPosition, true);
				if (specialAttack != null)
				{
					IntPosition boardPosition = Units.WorldToBoardIntPosition(this.currentWorldPosition);
					battleController.OnSpecialAttackDeployed(this.currentSpecialAttackType.Uid, teamType, boardPosition);
					Service.EventManager.SendEvent(EventId.SpecialAttackDeployed, specialAttack);
					return specialAttack;
				}
			}
			return null;
		}
	}
}
