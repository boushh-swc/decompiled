using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXTextureComponent : MonoBehaviour
	{
		public UITexture NGUITexture
		{
			get;
			set;
		}

		public UXTexture Texture
		{
			get;
			set;
		}

		public Texture MainTexture
		{
			get
			{
				if (this.NGUITexture != null)
				{
					return this.NGUITexture.mainTexture;
				}
				return null;
			}
			set
			{
				if (this.NGUITexture != null)
				{
					this.NGUITexture.mainTexture = value;
				}
			}
		}
	}
}
