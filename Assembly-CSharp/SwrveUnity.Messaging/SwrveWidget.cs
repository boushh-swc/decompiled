using System;
using UnityEngine;

namespace SwrveUnity.Messaging
{
	public abstract class SwrveWidget
	{
		public Point Position;

		public Point Size;

		public Texture2D Texture;

		public Rect Rect;

		public float AnimationScale = 1f;

		public SwrveWidget()
		{
			this.Position = new Point(0, 0);
			this.Size = new Point(0, 0);
		}

		public Point GetCenter(float w, float h, float Scale)
		{
			int x = (int)((double)(-(double)w * Scale) / 2.0);
			int y = (int)((double)(-(double)h * Scale) / 2.0);
			return new Point(x, y);
		}

		public Point GetCenteredPosition(float w, float h, float Scale, float FormatScale)
		{
			Point center = this.GetCenter(w, h, Scale);
			int x = (int)((float)center.X + (float)this.Position.X * FormatScale);
			int y = (int)((float)center.Y + (float)this.Position.Y * FormatScale);
			return new Point(x, y);
		}
	}
}
