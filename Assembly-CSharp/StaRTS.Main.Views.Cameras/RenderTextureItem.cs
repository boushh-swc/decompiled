using System;
using UnityEngine;

namespace StaRTS.Main.Views.Cameras
{
	public class RenderTextureItem
	{
		public RenderTexture RenderTexture
		{
			get;
			private set;
		}

		public int Width
		{
			get;
			private set;
		}

		public int Height
		{
			get;
			private set;
		}

		public bool InUse
		{
			get;
			set;
		}

		public RenderTextureItem(RenderTexture renderTexture, int width, int height)
		{
			this.RenderTexture = renderTexture;
			this.Width = width;
			this.Height = height;
			this.InUse = false;
		}
	}
}
