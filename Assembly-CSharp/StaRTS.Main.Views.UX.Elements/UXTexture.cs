using StaRTS.Assets;
using StaRTS.Main.Views.Cameras;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXTexture : UXElement
	{
		private UXTextureComponent component;

		private AssetManager assetManager;

		private string assetName;

		private AssetHandle assetHandle;

		private bool textureHasBeenSet;

		private UXSprite spinner;

		private Action onLoadCompleteCallback;

		private Action onLoadFailedCallback;

		public Texture MainTexture
		{
			get
			{
				return this.component.MainTexture;
			}
			set
			{
				this.DestroyCurrentTexture();
				this.textureHasBeenSet = true;
				this.component.MainTexture = value;
			}
		}

		public string AssetName
		{
			get
			{
				return this.assetName;
			}
		}

		public UXTexture(UXCamera uxCamera, UXTextureComponent component) : base(uxCamera, component.gameObject, null)
		{
			this.textureHasBeenSet = false;
			this.assetManager = Service.AssetManager;
			this.component = component;
			this.onLoadCompleteCallback = null;
		}

		public override void InternalDestroyComponent()
		{
			this.component.Texture = null;
			UnityEngine.Object.Destroy(this.component);
		}

		public void DeferTextureForLoad(string assetName)
		{
			this.assetName = assetName;
			this.UnloadCurrentTexture();
			this.spinner = Service.UXController.MiscElementsManager.GetHolonetLoader(this);
			this.spinner.LocalPosition = Vector3.zero;
		}

		public void LoadDeferred()
		{
			this.assetManager.Load(ref this.assetHandle, this.assetName, new AssetSuccessDelegate(this.OnLoadSuccess), null, null);
		}

		public void LoadTexture(string assetName)
		{
			this.LoadTexture(assetName, null);
		}

		public void LoadTexture(string assetName, Action onLoadComplete)
		{
			this.LoadTexture(assetName, onLoadComplete, null);
		}

		public void LoadTexture(string assetName, Action onLoadComplete, Action onLoadFail)
		{
			this.onLoadCompleteCallback = onLoadComplete;
			this.onLoadFailedCallback = onLoadFail;
			if (assetName == this.assetName || this.component == null || this.component.gameObject == null)
			{
				return;
			}
			this.component.gameObject.SetActive(false);
			this.UnloadCurrentTexture();
			this.assetName = assetName;
			this.assetManager.Load(ref this.assetHandle, assetName, new AssetSuccessDelegate(this.OnLoadSuccess), new AssetFailureDelegate(this.OnLoadFailure), null);
		}

		private void OnLoadFailure(object cookie)
		{
			if (this.onLoadFailedCallback != null)
			{
				this.onLoadFailedCallback();
			}
		}

		private void OnLoadSuccess(object asset, object cookie)
		{
			if (this.onLoadCompleteCallback != null)
			{
				this.onLoadCompleteCallback();
			}
			if (this.component != null)
			{
				this.MainTexture = (Texture2D)asset;
				this.component.gameObject.SetActive(true);
				if (this.spinner != null)
				{
					this.spinner.OnDestroyElement();
					UnityEngine.Object.Destroy(this.spinner.Root);
					this.spinner = null;
				}
			}
			else
			{
				this.UnloadCurrentTexture();
				this.DestroyCurrentTexture();
			}
		}

		private void DestroyCurrentTexture()
		{
			if (this.textureHasBeenSet && this.component.MainTexture != null)
			{
				this.component.MainTexture = null;
			}
		}

		private void UnloadCurrentTexture()
		{
			if (this.assetHandle != AssetHandle.Invalid)
			{
				this.assetManager.Unload(this.assetHandle);
				this.assetHandle = AssetHandle.Invalid;
				this.assetName = null;
			}
		}

		public override void OnDestroyElement()
		{
			this.onLoadCompleteCallback = null;
			this.UnloadCurrentTexture();
			this.DestroyCurrentTexture();
			base.OnDestroyElement();
		}
	}
}
