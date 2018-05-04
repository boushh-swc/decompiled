using Net.RichardLord.Ash.Core;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class TransformComponent : ComponentBase
	{
		public int X;

		public int Z;

		public float Rotation;

		public bool RotationInitialized;

		public float RotationVelocity;

		private int boardWidth;

		private int boardDepth;

		public int BoardWidth
		{
			get
			{
				return this.boardWidth;
			}
		}

		public int BoardDepth
		{
			get
			{
				return this.boardDepth;
			}
		}

		public TransformComponent(int boardX, int boardZ, float rotation, bool rotationInitialized, int boardWidth, int boardDepth)
		{
			this.X = boardX;
			this.Z = boardZ;
			this.Rotation = rotation;
			this.RotationInitialized = rotationInitialized;
			this.RotationVelocity = 0f;
			this.boardWidth = boardWidth;
			this.boardDepth = boardDepth;
		}

		public float CenterX()
		{
			return (float)this.X + (float)this.boardWidth / 2f;
		}

		public float CenterZ()
		{
			return (float)this.Z + (float)this.boardDepth / 2f;
		}

		public int CenterGridX()
		{
			return this.X + this.boardWidth / 2;
		}

		public int CenterGridZ()
		{
			return this.Z + this.boardDepth / 2;
		}

		public int MinX()
		{
			return this.X;
		}

		public int MaxX()
		{
			return this.X + this.boardWidth - 1;
		}

		public int MinZ()
		{
			return this.Z;
		}

		public int MaxZ()
		{
			return this.Z + this.boardDepth - 1;
		}
	}
}
