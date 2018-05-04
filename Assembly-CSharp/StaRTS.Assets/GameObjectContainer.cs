using System;
using UnityEngine;

namespace StaRTS.Assets
{
	public class GameObjectContainer
	{
		public GameObject GameObj
		{
			get;
			private set;
		}

		public bool Flagged
		{
			get;
			set;
		}

		public GameObjectContainer(GameObject gameObject)
		{
			this.GameObj = gameObject;
			this.Flagged = false;
		}
	}
}
