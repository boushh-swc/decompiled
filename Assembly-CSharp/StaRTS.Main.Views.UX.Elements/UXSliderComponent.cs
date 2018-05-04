using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXSliderComponent : MonoBehaviour
	{
		public UISlider NGUISlider
		{
			get;
			set;
		}

		public UXSlider Slider
		{
			get;
			set;
		}

		public float Value
		{
			get
			{
				return (!(this.NGUISlider == null)) ? this.NGUISlider.value : 0f;
			}
			set
			{
				if (this.NGUISlider != null)
				{
					this.NGUISlider.value = value;
				}
			}
		}
	}
}
