using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXLabelComponent : MonoBehaviour
	{
		public UILabel NGUILabel
		{
			get;
			set;
		}

		public UXLabel Label
		{
			get;
			set;
		}

		public string Text
		{
			get
			{
				return (!(this.NGUILabel == null)) ? this.NGUILabel.text : null;
			}
			set
			{
				if (this.NGUILabel != null)
				{
					this.NGUILabel.text = value;
				}
			}
		}

		public Color TextColor
		{
			get
			{
				return (!(this.NGUILabel == null)) ? this.NGUILabel.color : Color.white;
			}
			set
			{
				if (this.NGUILabel != null)
				{
					this.NGUILabel.color = value;
				}
			}
		}

		public int FontSize
		{
			get
			{
				return (!(this.NGUILabel == null)) ? this.NGUILabel.fontSize : 0;
			}
			set
			{
				if (this.NGUILabel != null)
				{
					this.NGUILabel.fontSize = value;
				}
			}
		}

		public Font Font
		{
			get
			{
				return (!(this.NGUILabel == null)) ? this.NGUILabel.trueTypeFont : null;
			}
			set
			{
				if (this.NGUILabel != null)
				{
					this.NGUILabel.trueTypeFont = value;
				}
			}
		}

		public float TotalHeight
		{
			get
			{
				return (!(this.NGUILabel == null)) ? this.NGUILabel.localSize.y : 0f;
			}
		}

		public float LineHeight
		{
			get
			{
				return (!(this.NGUILabel == null)) ? ((float)this.NGUILabel.fontSize) : 0f;
			}
		}

		public float TextWidth
		{
			get
			{
				return (!(this.NGUILabel == null)) ? ((float)this.NGUILabel.width) : 0f;
			}
		}

		public UIWidget.Pivot Pivot
		{
			get
			{
				return (!(this.NGUILabel == null)) ? this.NGUILabel.pivot : UIWidget.Pivot.Center;
			}
			set
			{
				if (this.NGUILabel != null)
				{
					this.NGUILabel.pivot = value;
				}
			}
		}

		public bool UseFontSharpening
		{
			get
			{
				return this.NGUILabel == null || this.NGUILabel.UseFontSharpening;
			}
			set
			{
				if (this.NGUILabel != null)
				{
					this.NGUILabel.UseFontSharpening = value;
				}
			}
		}
	}
}
