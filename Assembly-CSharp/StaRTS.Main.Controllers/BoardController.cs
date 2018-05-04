using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard;
using StaRTS.GameBoard.Components;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers
{
	public class BoardController
	{
		public const int DEPLOYABLE_BOARD_RADIUS = 23;

		private Board board;

		public Board Board
		{
			get
			{
				return this.board;
			}
		}

		public BoardController()
		{
			Service.BoardController = this;
			this.Initialize();
		}

		private void Initialize()
		{
			this.board = new Board(46, 42, 0u);
			this.board.AddConstraintRegion(new ConstraintRegion(-21, 21, -21, 21, CollisionFilters.BUILDABLE_AREA));
		}

		public int GetPriorityQueueSize()
		{
			return this.Board.BoardSize * this.Board.BoardSize;
		}

		public void RemoveEntity(Entity entity, bool removeSpawnProtection)
		{
			BoardItemComponent boardItemComponent = entity.Get<BoardItemComponent>();
			if (boardItemComponent == null)
			{
				return;
			}
			BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
			this.board.RemoveChild(boardItemComponent.BoardItem, buildingComponent != null && buildingComponent.BuildingType.Type != BuildingType.Blocker, buildingComponent != null);
			if (buildingComponent != null)
			{
				FlagStamp flagStamp = boardItemComponent.BoardItem.FlagStamp;
				if (flagStamp == null)
				{
					return;
				}
				flagStamp.Clear();
				if (!removeSpawnProtection)
				{
					uint num = 4u;
					if (buildingComponent.BuildingType.AllowDefensiveSpawn)
					{
						num |= 32u;
					}
					flagStamp.Fill(num);
				}
				this.board.AddFlagStamp(flagStamp);
			}
			Service.EventManager.SendEvent(EventId.BuildingRemovedFromBoard, entity);
		}

		public void ResetBoard()
		{
			this.Initialize();
		}
	}
}
