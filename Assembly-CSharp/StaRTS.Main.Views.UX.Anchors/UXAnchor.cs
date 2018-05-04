using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.UX.Elements;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Anchors
{
	public class UXAnchor : UXElement
	{
		public UXAnchor(UXCamera uxCamera) : base(uxCamera, null, null)
		{
		}

		protected void Init(GameObject root, UIAnchor.Side side)
		{
			this.root = root;
			if (root.GetComponent<UIAnchor>() != null)
			{
				throw new Exception("UXAnchor must not have a UIAnchor added already");
			}
			UIAnchor uIAnchor = root.AddComponent<UIAnchor>();
			uIAnchor.side = side;
			this.uxCamera.AddNewAnchor(root);
		}
	}
}
