using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Anchors
{
	public class UXPerformanceAnchor : UXAnchor
	{
		private const string ANCHOR_NAME = "Performance";

		public UXPerformanceAnchor() : base(Service.CameraManager.UXCamera)
		{
			GameObject root = new GameObject("Performance");
			base.Init(root, UIAnchor.Side.BottomRight);
		}
	}
}
