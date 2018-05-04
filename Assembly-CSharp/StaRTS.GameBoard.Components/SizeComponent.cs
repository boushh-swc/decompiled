using Net.RichardLord.Ash.Core;
using System;
using UnityEngine;

namespace StaRTS.GameBoard.Components
{
	public class SizeComponent : ComponentBase
	{
		public int Width;

		public int Depth;

		public SizeComponent(int boardWidth, int boardDepth)
		{
			this.Width = boardWidth;
			this.Depth = boardDepth;
		}

		public Vector3 ToVector3(float height)
		{
			return new Vector3((float)this.Width, height, (float)this.Depth);
		}

		public Vector3 ToVector3()
		{
			return this.ToVector3(0f);
		}
	}
}
