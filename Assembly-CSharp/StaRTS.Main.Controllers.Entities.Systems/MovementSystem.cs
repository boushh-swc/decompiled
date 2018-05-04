using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Entities.Systems
{
	public class MovementSystem : SimSystemBase
	{
		private EntityController entityController;

		private NodeList<MovementNode> nodeList;

		public override void AddToGame(Game game)
		{
			this.entityController = Service.EntityController;
			this.nodeList = this.entityController.GetNodeList<MovementNode>();
		}

		public override void RemoveFromGame(Game game)
		{
		}

		protected override void Update(uint dt)
		{
			Board board = Service.BoardController.Board;
			for (MovementNode movementNode = this.nodeList.Head; movementNode != null; movementNode = movementNode.Next)
			{
				SmartEntity smartEntity = (SmartEntity)movementNode.Entity;
				if (smartEntity.StateComp.CurState == EntityState.Moving && smartEntity.PathingComp.CurrentPath != null)
				{
					smartEntity.PathingComp.TimeOnSegment += dt;
					if ((ulong)smartEntity.PathingComp.TimeOnSegment > (ulong)((long)smartEntity.PathingComp.TimeToMove))
					{
						BoardCell boardCell = smartEntity.PathingComp.GetNextTile();
						if (boardCell == null)
						{
							if (smartEntity.DroidComp == null)
							{
								Service.ShooterController.StopMoving(smartEntity.StateComp);
							}
							Service.EventManager.SendEvent(EventId.TroopReachedPathEnd, smartEntity);
							smartEntity.PathingComp.CurrentPath = null;
						}
						else
						{
							smartEntity.TransformComp.X = boardCell.X;
							smartEntity.TransformComp.Z = boardCell.Z;
							board.MoveChild(smartEntity.BoardItemComp.BoardItem, smartEntity.TransformComp.CenterGridX(), smartEntity.TransformComp.CenterGridZ(), null, false, false);
							PathView pathView = smartEntity.PathingComp.PathView;
							BoardCell nextTurn = pathView.GetNextTurn();
							if (nextTurn.X == boardCell.X && nextTurn.Z == boardCell.Z)
							{
								pathView.AdvanceNextTurn();
							}
							boardCell = smartEntity.PathingComp.AdvanceNextTile();
							if (boardCell != null)
							{
								bool flag = smartEntity.TransformComp.X != boardCell.X && smartEntity.TransformComp.Z != boardCell.Z;
								smartEntity.PathingComp.TimeToMove += ((!flag) ? 1000 : 1414) * smartEntity.PathingComp.TimePerBoardCellMs / 1000;
							}
							else
							{
								DefenderComponent defenderComp = smartEntity.DefenderComp;
								if (defenderComp != null)
								{
									defenderComp.Patrolling = false;
								}
							}
						}
					}
				}
			}
		}
	}
}
