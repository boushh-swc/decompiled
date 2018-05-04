using StaRTS.Main.Views.UX.Elements;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace StaRTS.Main.Views.UX
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct GameObjectElementPair
	{
		public GameObject PairObject
		{
			get;
			private set;
		}

		public UXElement PairElement
		{
			get;
			private set;
		}

		public GameObjectElementPair(GameObject obj, UXElement elm)
		{
			this = default(GameObjectElementPair);
			this.PairObject = obj;
			this.PairElement = elm;
		}
	}
}
