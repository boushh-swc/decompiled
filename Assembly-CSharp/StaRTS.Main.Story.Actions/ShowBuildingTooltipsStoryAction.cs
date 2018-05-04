using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class ShowBuildingTooltipsStoryAction : AbstractStoryAction
	{
		private bool show;

		public ShowBuildingTooltipsStoryAction(StoryActionVO vo, IStoryReactor parent, bool show) : base(vo, parent)
		{
			this.show = show;
		}

		public override void Prepare()
		{
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			EntityController entityController = Service.EntityController;
			NodeList<HealthViewNode> nodeList = entityController.GetNodeList<HealthViewNode>();
			for (HealthViewNode healthViewNode = nodeList.Head; healthViewNode != null; healthViewNode = healthViewNode.Next)
			{
				healthViewNode.HealthView.SetEnabled(this.show);
				if (this.show)
				{
					Service.BuildingTooltipController.EnsureBuildingTooltip((SmartEntity)healthViewNode.Entity);
				}
			}
			NodeList<SupportViewNode> nodeList2 = Service.EntityController.GetNodeList<SupportViewNode>();
			for (SupportViewNode supportViewNode = nodeList2.Head; supportViewNode != null; supportViewNode = supportViewNode.Next)
			{
				supportViewNode.SupportView.SetEnabled(this.show);
				if (this.show)
				{
					Service.BuildingTooltipController.EnsureBuildingTooltip((SmartEntity)supportViewNode.Entity);
				}
			}
			NodeList<GeneratorViewNode> nodeList3 = Service.EntityController.GetNodeList<GeneratorViewNode>();
			for (GeneratorViewNode generatorViewNode = nodeList3.Head; generatorViewNode != null; generatorViewNode = generatorViewNode.Next)
			{
				if (this.show)
				{
					generatorViewNode.GeneratorView.SetEnabled(this.show);
					Service.ICurrencyController.UpdateGeneratorAccruedCurrency((SmartEntity)generatorViewNode.Entity);
				}
				else
				{
					generatorViewNode.GeneratorView.ShowCollectButton(false);
					generatorViewNode.GeneratorView.SetEnabled(this.show);
				}
			}
			this.parent.ChildComplete(this);
		}
	}
}
