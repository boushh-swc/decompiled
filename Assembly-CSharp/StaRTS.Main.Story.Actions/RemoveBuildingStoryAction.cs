using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Story.Actions
{
	public class RemoveBuildingStoryAction : AbstractStoryAction
	{
		private const int BOARDX_ARG = 0;

		private const int BOARDZ_ARG = 1;

		private int boardX;

		private int boardZ;

		private ViewFader fader;

		private Entity buildingToRemove;

		public RemoveBuildingStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(2);
			this.boardX = Convert.ToInt32(this.prepareArgs[0]);
			this.boardZ = Convert.ToInt32(this.prepareArgs[1]);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			BoardCell cellAt = Service.BoardController.Board.GetCellAt(this.boardX, this.boardZ);
			if (cellAt == null)
			{
				Service.Logger.ErrorFormat("Story {0} is attempting to remove a building at {1}, {2}, but there is no cell.", new object[]
				{
					this.vo.Uid,
					this.boardX,
					this.boardZ
				});
				this.parent.ChildComplete(this);
				return;
			}
			if (cellAt.Children == null)
			{
				return;
			}
			this.buildingToRemove = null;
			for (LinkedListNode<BoardItem> linkedListNode = cellAt.Children.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				Entity data = linkedListNode.Value.Data;
				BuildingComponent buildingComponent = data.Get<BuildingComponent>();
				if (buildingComponent != null)
				{
					this.buildingToRemove = data;
					break;
				}
			}
			if (this.buildingToRemove != null)
			{
				this.fader = new ViewFader();
				this.fader.FadeOut(this.buildingToRemove, 0f, 1f, null, new FadingDelegate(this.OnEntityFadeComplete));
				Service.EventManager.SendEvent(EventId.BuildingRemovedFromBoard, this.buildingToRemove);
			}
		}

		private void OnEntityFadeComplete(object fadedObject)
		{
			BuildingType type = this.buildingToRemove.Get<BuildingComponent>().BuildingType.Type;
			Service.EntityFactory.DestroyEntity(this.buildingToRemove, true, false);
			Service.EventManager.SendEvent(EventId.PostBuildingEntityKilled, type);
			this.buildingToRemove = null;
			this.fader = null;
			this.parent.ChildComplete(this);
		}
	}
}
