using System;
using UnityEngine;

namespace NGUIExtensions
{
	public class ButtonTap : MonoBehaviour
	{
		public Vector3 downScale = new Vector3(0.95f, 0.95f, 0f);

		public float downAlpha = 0.8f;

		public float transitionTime = 0.25f;

		private Vector3 defaultScale = new Vector3(1f, 1f, 0f);

		private float defaultAlpha = 1f;

		public void OnPress(bool isDown)
		{
			float duration = this.transitionTime * 0.5f;
			TweenScale.Begin(base.gameObject, duration, this.downScale);
			TweenAlpha tweenAlpha = TweenAlpha.Begin(base.gameObject, duration, this.downAlpha);
			tweenAlpha.from = this.defaultAlpha;
			if (!isDown)
			{
				TweenScale.Begin(base.gameObject, duration, this.defaultScale);
				TweenAlpha.Begin(base.gameObject, duration, this.defaultAlpha);
				tweenAlpha.from = this.downAlpha;
			}
		}
	}
}
