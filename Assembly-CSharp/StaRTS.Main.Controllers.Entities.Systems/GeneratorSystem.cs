using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Entities.Systems
{
	public class GeneratorSystem : ViewSystemBase
	{
		private const float VIEW_UPDATE_TIME = 1f;

		private const float FULLNESS_UPDATE_TIME = 5f;

		private EntityController entityController;

		private ICurrencyController currencyController;

		private PostBattleRepairController postBattleRepairController;

		private NodeList<GeneratorViewNode> nodeList;

		private StorageEffects storageEffects;

		private float timeSinceViewUpdate;

		private float timeSinceFullnessUpdate;

		public override void AddToGame(Game game)
		{
			this.entityController = Service.EntityController;
			this.currencyController = Service.ICurrencyController;
			this.postBattleRepairController = Service.PostBattleRepairController;
			this.nodeList = this.entityController.GetNodeList<GeneratorViewNode>();
			this.storageEffects = Service.StorageEffects;
			for (GeneratorViewNode generatorViewNode = this.nodeList.Head; generatorViewNode != null; generatorViewNode = generatorViewNode.Next)
			{
				if (generatorViewNode.GeneratorView != null)
				{
					generatorViewNode.GeneratorView.SetEnabled(true);
				}
			}
		}

		public override void RemoveFromGame(Game game)
		{
			for (GeneratorViewNode generatorViewNode = this.nodeList.Head; generatorViewNode != null; generatorViewNode = generatorViewNode.Next)
			{
				if (generatorViewNode.GeneratorView != null)
				{
					generatorViewNode.GeneratorView.ShowCollectButton(false);
					generatorViewNode.GeneratorView.SetEnabled(false);
				}
			}
			this.entityController = null;
			this.nodeList = null;
		}

		protected override void Update(float dt)
		{
			if (this.nodeList == null)
			{
				return;
			}
			this.timeSinceViewUpdate += dt;
			this.timeSinceFullnessUpdate += dt;
			if (this.timeSinceViewUpdate >= 1f)
			{
				bool flag = false;
				if (this.timeSinceFullnessUpdate >= 5f)
				{
					flag = true;
					this.timeSinceFullnessUpdate = 0f;
				}
				for (GeneratorViewNode generatorViewNode = this.nodeList.Head; generatorViewNode != null; generatorViewNode = generatorViewNode.Next)
				{
					if (!this.postBattleRepairController.IsEntityInRepair(generatorViewNode.Entity))
					{
						if (generatorViewNode.GeneratorView != null && !generatorViewNode.GeneratorView.Enabled)
						{
							generatorViewNode.GeneratorView.SetEnabled(true);
						}
						this.currencyController.UpdateGeneratorAccruedCurrency((SmartEntity)generatorViewNode.Entity);
						if (flag)
						{
							this.storageEffects.UpdateFillState(generatorViewNode.Entity, generatorViewNode.BuildingComp.BuildingType);
						}
					}
				}
				this.timeSinceViewUpdate = 0f;
			}
		}
	}
}
