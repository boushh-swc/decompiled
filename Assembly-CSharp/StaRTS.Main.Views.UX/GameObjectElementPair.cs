using StaRTS.Main.Views.UX.Elements;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX
{
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
			this.PairObject = obj;
			this.PairElement = elm;
		}
	}
}
