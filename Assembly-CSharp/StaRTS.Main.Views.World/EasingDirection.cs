using System;
using UnityEngine;

namespace StaRTS.Main.Views.World
{
	public class EasingDirection
	{
		private const int EASING_COUNT_MAX = 3;

		private Vector3[] easingAmounts;

		private int easingCount;

		private int easingIndex;

		public EasingDirection()
		{
			this.easingAmounts = new Vector3[3];
			for (int i = 0; i < 3; i++)
			{
				this.easingAmounts[i] = Vector3.zero;
			}
			this.easingCount = 0;
			this.easingIndex = -1;
		}

		public void OnDrag(Vector3 amount)
		{
			this.easingIndex = (this.easingIndex + 1) % 3;
			this.easingAmounts[this.easingIndex] = amount;
			if (this.easingCount < 3)
			{
				this.easingCount++;
			}
		}

		public Vector3 CalculateAndReset()
		{
			Vector3 vector = Vector3.zero;
			for (int i = 0; i < this.easingCount; i++)
			{
				vector += this.easingAmounts[i];
				this.easingAmounts[i] = Vector3.zero;
			}
			this.easingCount = 0;
			this.easingIndex = -1;
			return vector;
		}
	}
}
