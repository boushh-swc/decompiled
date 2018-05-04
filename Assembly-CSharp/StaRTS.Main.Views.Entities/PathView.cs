using StaRTS.GameBoard;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Utils;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Entities
{
	public class PathView
	{
		public const int MAX_TROOPS_PER_CELL = 9;

		public const int TROOPS_PER_ROW = 3;

		private int nextTurnIndex;

		public float TimeOnPathSegment;

		public PathingComponent PathComponent
		{
			get;
			protected set;
		}

		public Vector3 StartPos
		{
			get;
			set;
		}

		public float TimeToTarget
		{
			get;
			set;
		}

		public float Speed
		{
			get;
			set;
		}

		public float ClusterOffsetX
		{
			get;
			set;
		}

		public float ClusterOffsetZ
		{
			get;
			set;
		}

		public int NextTurnIndex
		{
			get
			{
				return this.nextTurnIndex;
			}
		}

		public PathView(PathingComponent pathing)
		{
			this.Reset(pathing);
		}

		public void Reset(PathingComponent pathing)
		{
			this.TimeToTarget = 0f;
			this.TimeOnPathSegment = 0f;
			this.Speed = 0f;
			this.StartPos = Vector3.zero;
			this.PathComponent = pathing;
			this.nextTurnIndex = -1;
			if (this.PathComponent != null)
			{
				uint num = this.PathComponent.Entity.ID % 9u;
				this.ClusterOffsetX = num % 3u / 3f;
				this.ClusterOffsetZ = num / 3u % 3u / 3f;
			}
			else
			{
				this.ClusterOffsetX = 0f;
				this.ClusterOffsetZ = 0f;
			}
		}

		public BoardCell GetPrevTurn()
		{
			if (this.PathComponent.CurrentPath == null)
			{
				return null;
			}
			return this.PathComponent.CurrentPath.GetTurn(this.nextTurnIndex - 1);
		}

		public BoardCell GetNextTurn()
		{
			if (this.PathComponent.CurrentPath == null)
			{
				return null;
			}
			return this.PathComponent.CurrentPath.GetTurn(this.nextTurnIndex);
		}

		public BoardCell GetNextNextTurn()
		{
			if (this.PathComponent.CurrentPath == null)
			{
				return null;
			}
			return this.PathComponent.CurrentPath.GetTurn(this.nextTurnIndex + 1);
		}

		public void GetTroopClusterOffset(ref float offsetX, ref float offsetZ)
		{
			offsetX = this.ClusterOffsetX;
			offsetZ = this.ClusterOffsetZ;
		}

		public void AdvanceNextTurn()
		{
			float num = 0f;
			float num2 = 0f;
			this.GetTroopClusterOffset(ref num, ref num2);
			float num3 = Units.BoardToWorldX((float)this.PathComponent.CurrentPath.TroopWidth / 2f);
			GameObjectViewComponent gameObjectViewComponent = this.PathComponent.Entity.Get<GameObjectViewComponent>();
			if (gameObjectViewComponent == null)
			{
				TransformComponent transformComponent = this.PathComponent.Entity.Get<TransformComponent>();
				this.StartPos = new Vector3(Units.BoardToWorldX(transformComponent.CenterX()), 0f, Units.BoardToWorldZ(transformComponent.CenterZ()));
			}
			else
			{
				this.StartPos = gameObjectViewComponent.MainTransform.position;
			}
			this.StartPos -= new Vector3(num3 + num, 0f, num3 + num2);
			if (this.nextTurnIndex < this.PathComponent.CurrentPath.TurnCount - 1)
			{
				this.nextTurnIndex++;
				this.TimeOnPathSegment = 0f;
				this.TimeToTarget = (float)(this.PathComponent.CurrentPath.GetTurnDistance(this.nextTurnIndex) * this.PathComponent.TimePerBoardCellMs) * 1E-06f;
				if (this.TimeToTarget == 0f)
				{
					this.Speed = 0f;
				}
				else
				{
					BoardCell turn = this.PathComponent.CurrentPath.GetTurn(this.nextTurnIndex - 1);
					BoardCell turn2 = this.PathComponent.CurrentPath.GetTurn(this.nextTurnIndex);
					float num4 = Units.BoardToWorldX(turn2.X - turn.X) + num;
					float num5 = Units.BoardToWorldZ(turn2.Z - turn.Z) + num2;
					this.Speed = Mathf.Sqrt(num4 * num4 + num5 * num5) / this.TimeToTarget;
				}
				TransformComponent transformComponent2 = this.PathComponent.Entity.Get<TransformComponent>();
				if (transformComponent2 != null)
				{
					transformComponent2.RotationVelocity = 0f;
				}
			}
		}
	}
}
