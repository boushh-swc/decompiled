using StaRTS.Main.Views.Cameras;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXSprite : UXElement
	{
		public const string CLEAR_SPRITE_NAME = "bkgClear";

		private UXSpriteComponent component;

		public string SpriteName
		{
			get
			{
				return this.component.SpriteName;
			}
			set
			{
				this.component.SpriteName = value;
			}
		}

		public UIAtlas Atlas
		{
			get
			{
				return this.component.Atlas;
			}
		}

		public Vector4 Border
		{
			get
			{
				return this.component.Border * this.uxCamera.Scale;
			}
		}

		public float Alpha
		{
			get
			{
				return this.component.Alpha;
			}
			set
			{
				this.component.Alpha = value;
			}
		}

		public Color Color
		{
			get
			{
				return this.component.Color;
			}
			set
			{
				this.component.Color = value;
			}
		}

		public float FillAmount
		{
			get
			{
				return this.component.FillAmount;
			}
			set
			{
				this.component.FillAmount = value;
			}
		}

		public UXSprite(UXCamera uxCamera, UXSpriteComponent component) : base(uxCamera, component.gameObject, null)
		{
			this.component = component;
		}

		public override void InternalDestroyComponent()
		{
			this.component.Sprite = null;
			UnityEngine.Object.Destroy(this.component);
		}

		public override void OnDestroyElement()
		{
			if (Service.ProjectorManager != null)
			{
				Service.ProjectorManager.DestroyProjector(this);
			}
		}

		public bool SetAtlasAndSprite(UIAtlas atlas, string name)
		{
			return this.component.SetAtlasAndSprite(atlas, name);
		}
	}
}
