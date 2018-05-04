using StaRTS.Main.Views.Cameras;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXSlider : UXElement
	{
		private UXSliderComponent component;

		private UISprite sprite1;

		private UISprite sprite2;

		private float alpha;

		private float fAlpha;

		private float bAlpha;

		public float Value
		{
			get
			{
				return this.component.Value;
			}
			set
			{
				this.component.Value = value;
			}
		}

		public float Alpha
		{
			get
			{
				return this.alpha;
			}
			set
			{
				if (this.alpha != value)
				{
					this.alpha = value;
					if (this.sprite1 != null)
					{
						this.sprite1.alpha = this.fAlpha * this.alpha;
					}
					if (this.sprite2 != null)
					{
						this.sprite2.alpha = this.bAlpha * this.alpha;
					}
				}
			}
		}

		public UXSlider(UXCamera uxCamera, UXSliderComponent component) : base(uxCamera, component.gameObject, null)
		{
			this.component = component;
			this.alpha = 1f;
			this.sprite1 = null;
			this.sprite2 = null;
			if (this.root != null)
			{
				int i = 0;
				int childCount = this.root.transform.childCount;
				while (i < childCount)
				{
					UISprite x = this.root.transform.GetChild(i).gameObject.GetComponent<UISprite>();
					if (x != null)
					{
						if (!(this.sprite1 == null))
						{
							this.sprite2 = x;
							break;
						}
						this.sprite1 = x;
					}
					i++;
				}
			}
			if (this.sprite1 != null)
			{
				this.fAlpha = this.sprite1.alpha;
			}
			if (this.sprite2 != null)
			{
				this.bAlpha = this.sprite2.alpha;
			}
		}

		public override void InternalDestroyComponent()
		{
			this.component.Slider = null;
			UnityEngine.Object.Destroy(this.component);
		}
	}
}
