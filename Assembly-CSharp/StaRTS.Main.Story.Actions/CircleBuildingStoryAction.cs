using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Story.Actions
{
	public class CircleBuildingStoryAction : AbstractStoryAction
	{
		private const int BUILDING_ID_ARG = 0;

		private int boardX;

		private int boardZ;

		private int width;

		private int depth;

		private bool buildingFound;

		public CircleBuildingStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
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
			Entity entity = null;
			NodeList<BuildingNode> nodeList = Service.EntityController.GetNodeList<BuildingNode>();
			for (BuildingNode buildingNode = nodeList.Head; buildingNode != null; buildingNode = buildingNode.Next)
			{
				if (buildingNode.BuildingComp.BuildingType.BuildingID.Equals(this.prepareArgs[0], StringComparison.InvariantCultureIgnoreCase))
				{
					entity = buildingNode.Entity;
					this.buildingFound = true;
					break;
				}
			}
			if (entity != null)
			{
				this.width = Units.GridToBoardX(entity.Get<BuildingComponent>().BuildingType.SizeX);
				this.depth = Units.GridToBoardZ(entity.Get<BuildingComponent>().BuildingType.SizeY);
				this.boardX = entity.Get<BoardItemComponent>().BoardItem.BoardX + this.width / 2;
				this.boardZ = entity.Get<BoardItemComponent>().BoardItem.BoardZ + this.depth / 2;
			}
			if (this.buildingFound)
			{
				Service.BuildingController.HighlightBuilding(entity);
				Service.UXController.MiscElementsManager.HighlightRegion((float)this.boardX, (float)this.boardZ, this.width, this.depth);
				Vector3 zero = Vector3.zero;
				zero.x = Units.BoardToWorldX(this.boardX);
				zero.z = Units.BoardToWorldZ(this.boardZ);
				Service.WorldInitializer.View.PanToLocation(zero);
				Service.UserInputInhibitor.AllowOnly(entity);
			}
			this.parent.ChildComplete(this);
		}
	}
}
