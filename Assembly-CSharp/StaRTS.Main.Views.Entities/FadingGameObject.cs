using System;
using UnityEngine;

namespace StaRTS.Main.Views.Entities
{
	public class FadingGameObject : AbstractFadingView
	{
		private GameObject gameObject;

		public GameObject GameObj
		{
			get
			{
				return this.gameObject;
			}
		}

		public FadingGameObject(GameObject gameObj, float delay, float fadeTime, FadingDelegate onStart, FadingDelegate onComplete) : base(gameObj, delay, fadeTime, onStart, onComplete)
		{
			this.gameObject = gameObj;
			base.InitData(this.gameObject, null);
		}
	}
}
