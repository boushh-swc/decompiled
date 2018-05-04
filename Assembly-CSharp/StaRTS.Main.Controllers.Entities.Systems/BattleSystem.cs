using Net.RichardLord.Ash.Core;
using StaRTS.Audio;
using StaRTS.Main.Configs;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Entities.Systems
{
	public class BattleSystem : SimSystemBase
	{
		private EntityController entityController;

		private BattleController battleController;

		private TroopAbilityController troopAbilityController;

		private AudioManager audioManager;

		private SpecialAttackController specialAttackController;

		private SquadTroopAttackController squadTroopAttackController;

		private NodeList<BuildingNode> buildingNodeList;

		private NodeList<TroopNode> troopNodeList;

		private const uint SMALL_AUDIO_RESET_DELTA = 33u;

		private const uint LARGE_AUDIO_RESET_DELTA = 330u;

		private uint audioResetDeltaMax;

		private uint audioResetDeltaAccumulator;

		public override void AddToGame(Game game)
		{
			this.entityController = Service.EntityController;
			this.battleController = Service.BattleController;
			this.troopAbilityController = Service.TroopAbilityController;
			this.audioManager = Service.AudioManager;
			this.specialAttackController = Service.SpecialAttackController;
			this.squadTroopAttackController = Service.SquadTroopAttackController;
			this.buildingNodeList = this.entityController.GetNodeList<BuildingNode>();
			this.troopNodeList = this.entityController.GetNodeList<TroopNode>();
			this.audioResetDeltaMax = ((!HardwareProfile.IsLowEndDevice()) ? 33u : 330u);
			this.audioResetDeltaAccumulator = this.audioResetDeltaMax;
		}

		public override void RemoveFromGame(Game game)
		{
			this.audioManager.ResetBattleAudioFlags();
		}

		private void HandleDeferredUserInuput()
		{
			this.troopAbilityController.ProcessPendingAbilities();
		}

		protected override void Update(uint dt)
		{
			this.HandleDeferredUserInuput();
			this.battleController.UpdateBattleTime(dt);
			if (this.audioResetDeltaAccumulator >= this.audioResetDeltaMax)
			{
				this.audioManager.ResetBattleAudioFlags();
				this.audioResetDeltaAccumulator = 0u;
			}
			else
			{
				this.audioResetDeltaAccumulator += dt;
			}
			int num = 0;
			for (BuildingNode buildingNode = this.buildingNodeList.Head; buildingNode != null; buildingNode = buildingNode.Next)
			{
				num += this.battleController.GetHealth(buildingNode.BuildingComp.BuildingType.Type, buildingNode.HealthComp);
			}
			bool flag = true;
			for (TroopNode troopNode = this.troopNodeList.Head; troopNode != null; troopNode = troopNode.Next)
			{
				if (troopNode.TeamComp.TeamType == TeamType.Attacker && !troopNode.HealthComp.IsDead() && !troopNode.TroopComp.TroopType.IsHealer)
				{
					flag = false;
					break;
				}
			}
			if (flag && this.specialAttackController.HasUnexpendedSpecialAttacks())
			{
				flag = false;
			}
			this.battleController.UpdateCurrentHealth(num);
			if (flag)
			{
				this.battleController.OnAllTroopsDead();
			}
			TeamType type = (!this.battleController.GetCurrentBattle().IsRaidDefense()) ? TeamType.Attacker : TeamType.Defender;
			this.squadTroopAttackController.UpdateSquadTroopSpawnQueue(type);
		}
	}
}
