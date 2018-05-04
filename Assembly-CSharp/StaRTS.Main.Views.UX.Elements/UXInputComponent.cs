using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXInputComponent : MonoBehaviour
	{
		public UIInput NGUIInput
		{
			get;
			set;
		}

		public UXInput Input
		{
			get;
			set;
		}

		public string InitText
		{
			set
			{
				if (this.NGUIInput != null)
				{
					this.NGUIInput.defaultText = value;
				}
			}
		}

		public string Text
		{
			get
			{
				return (!(this.NGUIInput == null)) ? this.NGUIInput.value : null;
			}
			set
			{
				if (this.NGUIInput != null)
				{
					this.NGUIInput.value = value;
				}
			}
		}
	}
}
