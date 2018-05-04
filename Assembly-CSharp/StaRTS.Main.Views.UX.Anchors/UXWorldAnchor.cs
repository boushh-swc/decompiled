using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Anchors
{
	public class UXWorldAnchor : UXAnchor
	{
		private const string ANCHOR_NAME = "World";

		public UXWorldAnchor() : base(Service.CameraManager.UXCamera)
		{
			GameObject root = new GameObject("World");
			base.Init(root, UIAnchor.Side.BottomLeft);
		}
	}
}
