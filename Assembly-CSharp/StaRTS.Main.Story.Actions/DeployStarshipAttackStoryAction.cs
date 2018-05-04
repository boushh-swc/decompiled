using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Story.Actions
{
	public class DeployStarshipAttackStoryAction : AbstractStoryAction
	{
		private const int SHIP_UID_ARG = 0;

		private const int BOARDX_ARG = 1;

		private const int BOARDZ_ARG = 2;

		private int boardX;

		private int boardZ;

		public DeployStarshipAttackStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(3);
			this.boardX = Convert.ToInt32(this.prepareArgs[1]);
			this.boardZ = Convert.ToInt32(this.prepareArgs[2]);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			Vector3 zero = Vector3.zero;
			zero.x = Units.BoardToWorldX(this.boardX);
			zero.z = Units.BoardToWorldX(this.boardZ);
			StaticDataController staticDataController = Service.StaticDataController;
			SpecialAttackTypeVO specialAttackTypeVO = staticDataController.Get<SpecialAttackTypeVO>(this.prepareArgs[0]);
			SpecialAttack specialAttack = Service.SpecialAttackController.DeploySpecialAttack(specialAttackTypeVO, TeamType.Attacker, zero);
			if (specialAttack != null)
			{
				List<IAssetVO> assets = new List<IAssetVO>();
				ProjectileUtils.AddProjectileAssets(specialAttackTypeVO.ProjectileType, assets, staticDataController);
				Service.ProjectileViewManager.LoadProjectileAssetsAndCreatePools(assets);
				Service.EventManager.SendEvent(EventId.SpecialAttackDeployed, specialAttack);
			}
			this.parent.ChildComplete(this);
		}
	}
}
