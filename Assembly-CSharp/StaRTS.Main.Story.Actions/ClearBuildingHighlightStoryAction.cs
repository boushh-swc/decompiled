using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class ClearBuildingHighlightStoryAction : AbstractStoryAction
	{
		private const int BUILDING_ID_ARG = 0;

		public ClearBuildingHighlightStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(1);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			NodeList<BuildingNode> nodeList = Service.EntityController.GetNodeList<BuildingNode>();
			for (BuildingNode buildingNode = nodeList.Head; buildingNode != null; buildingNode = buildingNode.Next)
			{
				if (buildingNode.BuildingComp.BuildingType.BuildingID.Equals(this.prepareArgs[0], StringComparison.InvariantCultureIgnoreCase))
				{
					Entity entity = buildingNode.Entity;
					Service.BuildingController.ClearBuildingHighlight(entity);
					Service.UXController.MiscElementsManager.HideHighlight();
					break;
				}
			}
			this.parent.ChildComplete(this);
		}
	}
}
