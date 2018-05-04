using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.World.Deploying;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers
{
	public class DeployerController
	{
		private AbstractDeployer[] deployers;

		public TroopDeployer TroopDeployer
		{
			get;
			private set;
		}

		public SpecialAttackDeployer SpecialAttackDeployer
		{
			get;
			private set;
		}

		public HeroDeployer HeroDeployer
		{
			get;
			private set;
		}

		public ChampionDeployer ChampionDeployer
		{
			get;
			private set;
		}

		public SquadTroopDeployer SquadTroopDeployer
		{
			get;
			private set;
		}

		public DeployerController()
		{
			Service.DeployerController = this;
			this.TroopDeployer = new TroopDeployer();
			this.SpecialAttackDeployer = new SpecialAttackDeployer();
			this.HeroDeployer = new HeroDeployer();
			this.ChampionDeployer = new ChampionDeployer();
			this.SquadTroopDeployer = new SquadTroopDeployer();
			this.deployers = new AbstractDeployer[]
			{
				this.TroopDeployer,
				this.SpecialAttackDeployer,
				this.HeroDeployer,
				this.ChampionDeployer,
				this.SquadTroopDeployer
			};
		}

		public void EnterTroopPlacementMode(TroopTypeVO troopType)
		{
			this.ExitAllDeployModesExcept(this.TroopDeployer);
			this.TroopDeployer.EnterPlacementMode(troopType);
		}

		public void EnterSpecialAttackPlacementMode(SpecialAttackTypeVO specialAttackType)
		{
			this.ExitAllDeployModesExcept(this.SpecialAttackDeployer);
			this.SpecialAttackDeployer.EnterPlacementMode(specialAttackType);
		}

		public void EnterHeroDeployMode(TroopTypeVO troopType)
		{
			this.ExitAllDeployModesExcept(this.HeroDeployer);
			this.HeroDeployer.EnterMode(troopType);
		}

		public void EnterChampionDeployMode(TroopTypeVO troopType)
		{
			this.ExitAllDeployModesExcept(this.ChampionDeployer);
			this.ChampionDeployer.EnterMode(troopType);
		}

		public void EnterSquadTroopPlacementMode()
		{
			this.ExitAllDeployModesExcept(this.SquadTroopDeployer);
			this.SquadTroopDeployer.EnterPlacementMode();
		}

		private void ExitAllDeployModesExcept(AbstractDeployer deployer)
		{
			int i = 0;
			int num = this.deployers.Length;
			while (i < num)
			{
				if (this.deployers[i] != deployer)
				{
					this.deployers[i].ExitMode();
				}
				i++;
			}
		}

		public void ExitAllDeployModes()
		{
			int i = 0;
			int num = this.deployers.Length;
			while (i < num)
			{
				this.deployers[i].ExitMode();
				i++;
			}
		}
	}
}
