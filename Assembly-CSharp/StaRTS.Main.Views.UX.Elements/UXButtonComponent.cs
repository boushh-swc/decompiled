using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXButtonComponent : MonoBehaviour
	{
		public UIButton NGUIButton
		{
			get;
			set;
		}

		public UXButton Button
		{
			get;
			set;
		}

		private void OnClick()
		{
			if (this.Button != null)
			{
				this.Button.InternalOnClick();
			}
		}

		private void OnPress(bool isPressed)
		{
			if (this.Button != null)
			{
				if (isPressed)
				{
					this.Button.InternalOnPress();
				}
				else
				{
					this.Button.InternalOnRelease();
				}
			}
		}

		private void OnRelease()
		{
			if (this.Button != null)
			{
				this.Button.InternalOnRelease();
			}
		}
	}
}
