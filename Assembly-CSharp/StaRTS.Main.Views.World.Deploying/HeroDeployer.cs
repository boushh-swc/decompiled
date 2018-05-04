using Net.RichardLord.Ash.Core;
using StaRTS.DataStructures;
using StaRTS.GameBoard;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.World.Deploying
{
	public class HeroDeployer : AbstractDeployer
	{
		public TroopTypeVO CurrentTroopType
		{
			get;
			private set;
		}

		public bool CanDeploy
		{
			get;
			private set;
		}

		public HeroDeployer()
		{
			this.CurrentTroopType = null;
			this.CanDeploy = true;
		}

		public bool EnterMode(TroopTypeVO troopType)
		{
			if (this.CurrentTroopType != null)
			{
				this.ExitMode();
			}
			this.CurrentTroopType = troopType;
			this.EnterMode();
			return true;
		}

		public override void ExitMode()
		{
			this.CanDeploy = true;
			base.ExitMode();
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
				this.DeployHero(this.CurrentTroopType);
			}
			return EatResponse.NotEaten;
		}

		private SmartEntity DeployHero(TroopTypeVO troopType)
		{
			if (Service.SimTimeEngine.IsPaused())
			{
				return null;
			}
			if (!this.CanDeploy)
			{
				return null;
			}
			BattleController battleController = Service.BattleController;
			if (battleController.BattleEndProcessing)
			{
				return null;
			}
			if (battleController.GetPlayerDeployableHeroCount(troopType.Uid) == 0)
			{
				this.CanDeploy = false;
				Service.EventManager.SendEvent(EventId.TroopNotPlacedInvalidTroop, this.currentWorldPosition);
				return null;
			}
			TeamType teamType = TeamType.Attacker;
			if (battleController.GetCurrentBattle().Type == BattleType.PveDefend)
			{
				teamType = TeamType.Defender;
			}
			IntPosition intPosition = Units.WorldToBoardIntDeployPosition(this.currentWorldPosition);
			TroopController troopController = Service.TroopController;
			if (battleController.GetCurrentBattle().IsRaidDefense())
			{
				Entity entity = null;
				BoardCell boardCell = null;
				if (!troopController.FinalizeSafeBoardPosition(troopType, ref entity, ref intPosition, ref boardCell, TeamType.Attacker, TroopSpawnMode.Unleashed, false))
				{
					Service.EventManager.SendEvent(EventId.TroopNotPlacedInvalidArea, intPosition);
					return null;
				}
			}
			SmartEntity smartEntity = troopController.SpawnHero(troopType, teamType, intPosition, false);
			if (smartEntity != null)
			{
				base.PlaySpawnEffect(smartEntity);
				this.CanDeploy = false;
				battleController.OnHeroDeployed(troopType.Uid, teamType, intPosition);
				Service.EventManager.SendEvent(EventId.HeroDeployed, smartEntity);
			}
			return smartEntity;
		}
	}
}
