using StaRTS.GameBoard.Components;
using StaRTS.Main.Utils;
using System;

namespace StaRTS.GameBoard
{
	public class ConstraintRegion
	{
		public int Left
		{
			get;
			set;
		}

		public int Right
		{
			get;
			set;
		}

		public int Top
		{
			get;
			set;
		}

		public int Bottom
		{
			get;
			set;
		}

		public FilterComponent FilterComponent
		{
			get;
			set;
		}

		public ConstraintRegion(int left, int right, int top, int bottom, FilterComponent filterComponent)
		{
			this.Left = left;
			this.Right = right;
			this.Top = top;
			this.Bottom = bottom;
			this.FilterComponent = filterComponent;
		}

		public bool Blocks(BoardItem item, int x, int z, int width, int depth)
		{
			return this.FilterComponent.CollidesWith(item.Filter) && !GameUtils.RectContainsRect(this.Left, this.Right, this.Top, this.Bottom, x, x + width, z, z + depth);
		}
	}
}
