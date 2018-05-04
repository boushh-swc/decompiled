using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXSpriteComponent : MonoBehaviour
	{
		private string origSpriteName;

		public UISprite NGUISprite
		{
			get;
			set;
		}

		public UXSprite Sprite
		{
			get;
			set;
		}

		public string SpriteName
		{
			get
			{
				return (!(this.NGUISprite == null)) ? this.NGUISprite.spriteName : null;
			}
			set
			{
				if (this.NGUISprite != null)
				{
					if (this.origSpriteName == null)
					{
						this.origSpriteName = this.NGUISprite.spriteName;
					}
					this.NGUISprite.spriteName = value;
					if (!this.NGUISprite.isValid)
					{
						this.NGUISprite.spriteName = this.origSpriteName;
					}
				}
			}
		}

		public UIAtlas Atlas
		{
			get
			{
				return (!(this.NGUISprite == null)) ? this.NGUISprite.atlas : null;
			}
		}

		public Vector4 Border
		{
			get
			{
				return (!(this.NGUISprite == null)) ? this.NGUISprite.border : Vector4.zero;
			}
		}

		public float FillAmount
		{
			get
			{
				return (!(this.NGUISprite == null)) ? this.NGUISprite.fillAmount : 0f;
			}
			set
			{
				if (this.NGUISprite != null)
				{
					this.NGUISprite.fillAmount = value;
				}
			}
		}

		public float Alpha
		{
			get
			{
				return (!(this.NGUISprite == null)) ? this.NGUISprite.alpha : 0f;
			}
			set
			{
				if (this.NGUISprite != null)
				{
					this.NGUISprite.alpha = value;
				}
			}
		}

		public Color Color
		{
			get
			{
				return (!(this.NGUISprite == null)) ? this.NGUISprite.color : Color.white;
			}
			set
			{
				if (this.NGUISprite != null)
				{
					this.NGUISprite.color = value;
				}
			}
		}

		public bool SetAtlasAndSprite(UIAtlas atlas, string name)
		{
			if (atlas.GetSprite(name) == null)
			{
				return false;
			}
			this.NGUISprite.atlas = atlas;
			this.NGUISprite.spriteName = name;
			return true;
		}
	}
}
