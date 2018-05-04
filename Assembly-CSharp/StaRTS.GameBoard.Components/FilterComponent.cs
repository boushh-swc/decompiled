using System;

namespace StaRTS.GameBoard.Components
{
	public class FilterComponent
	{
		public int categoryBits;

		public int maskBits;

		public int Category
		{
			get
			{
				return this.categoryBits;
			}
			set
			{
				this.categoryBits = value;
			}
		}

		public int Mask
		{
			get
			{
				return this.maskBits;
			}
			set
			{
				this.maskBits = value;
			}
		}

		public FilterComponent(int categoryBits, int maskBits)
		{
			this.categoryBits = categoryBits;
			this.maskBits = maskBits;
		}

		public FilterComponent(FilterComponent otherFilter)
		{
			this.Set(otherFilter);
		}

		public bool CollidesWith(FilterComponent otherFilter)
		{
			return otherFilter != null && ((this.categoryBits & otherFilter.maskBits) != 0 || (this.maskBits & otherFilter.categoryBits) != 0);
		}

		public void Merge(FilterComponent otherFilter)
		{
			if (otherFilter == null)
			{
				return;
			}
			this.categoryBits |= otherFilter.Category;
			this.maskBits |= otherFilter.Mask;
		}

		public void Set(FilterComponent otherFilter)
		{
			if (otherFilter == null)
			{
				this.categoryBits = 0;
				this.maskBits = 0;
				return;
			}
			this.categoryBits = otherFilter.categoryBits;
			this.maskBits = otherFilter.maskBits;
		}
	}
}
