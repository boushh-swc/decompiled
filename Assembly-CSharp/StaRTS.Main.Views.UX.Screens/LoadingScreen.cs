using StaRTS.Assets;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class LoadingScreen : UXFactory
	{
		private const string LOADING_LABEL = "LabelLoading";

		private const string LOADING_PLAYERID_LABEL = "LabelPlayerID";

		private const string LOADING_BAR = "PBarLoading";

		private const string LOADING_LOGO_ASIAN = "SpriteLogoAsian";

		private const string LOADING_LOGO = "SpriteLogo";

		private const string LOADING_BACKGROUND = "SpriteBkg";

		private const string LOADING_DIMMER = "SpriteDimmer";

		private const string LOGO_DEFAULT = "LoadingLogo";

		private const string LOGO_RU = "SWC_Logo_Russian_B";

		private const string LOGO_CN = "SWC_Logo_Chinese_SC";

		private const string LOGO_TW = "SWC_Logo_Chinese_TC";

		private const string LOGO_KR = "SWC_Logo_Korean";

		private AssetHandle assetHandle;

		private UXSlider loadingSlider;

		private UXLabel playerIdLabel;

		public LoadingScreen() : base(Service.CameraManager.UXSceneCamera)
		{
			base.Load(ref this.assetHandle, "gui_loading_screen", new UXFactoryLoadDelegate(this.LoadSuccess), null, null);
		}

		private void OnWipeComplete(object cookie)
		{
			this.uxCamera.Camera.enabled = false;
			base.DestroyFactory();
		}

		private void LoadSuccess(object cookie)
		{
			this.uxCamera.Camera.enabled = true;
			this.InitSlider();
			this.InitLogo();
			this.playerIdLabel = base.GetElement<UXLabel>("LabelPlayerID");
			this.playerIdLabel.Text = string.Empty;
		}

		private void InitLogo()
		{
			UXSprite element = base.GetElement<UXSprite>("SpriteLogo");
			UXSprite element2 = base.GetElement<UXSprite>("SpriteLogoAsian");
			string locale = Service.Lang.Locale;
			if (locale == "ru_RU")
			{
				element.SpriteName = "SWC_Logo_Russian_B";
				element.Visible = true;
				element2.Visible = false;
			}
			else if (locale == "zh_CN")
			{
				element.Visible = false;
				element2.Visible = true;
				element2.SpriteName = "SWC_Logo_Chinese_SC";
			}
			else if (locale == "zh_TW")
			{
				element.Visible = false;
				element2.Visible = true;
				element2.SpriteName = "SWC_Logo_Chinese_TC";
			}
			else if (locale == "ko_KR")
			{
				element.Visible = false;
				element2.Visible = true;
				element2.SpriteName = "SWC_Logo_Korean";
			}
			else
			{
				element.SpriteName = "LoadingLogo";
				element.Visible = true;
				element2.Visible = false;
			}
		}

		private void InitSlider()
		{
			UXLabel element = base.GetElement<UXLabel>("LabelLoading");
			element.Text = Service.Lang.Get("LOADING", new object[0]);
			this.loadingSlider = base.GetElement<UXSlider>("PBarLoading");
		}

		public override void SetupRootCollider()
		{
		}

		public override void OnDestroyElement()
		{
			if (this.assetHandle != AssetHandle.Invalid)
			{
				this.UnloadSpriteAtlas("SpriteBkg");
				this.UnloadSpriteAtlas("SpriteDimmer");
				this.UnloadSpriteAtlas("SpriteLogo");
				this.UnloadSpriteAtlas("SpriteLogoAsian");
				base.Unload(this.assetHandle, "gui_loading_screen");
				this.assetHandle = AssetHandle.Invalid;
			}
			base.OnDestroyElement();
		}

		private void UnloadSpriteAtlas(string spriteName)
		{
			UXSprite element = base.GetElement<UXSprite>(spriteName);
			UnityUtils.UnloadMaterial(element.Atlas.spriteMaterial);
		}

		public void Progress(float percentage, string description)
		{
			if (base.IsLoaded())
			{
				this.loadingSlider.Value = percentage * 0.01f;
			}
		}

		public void SetPlayerId(string playerId)
		{
			if (!base.IsLoaded())
			{
				return;
			}
			if (this.playerIdLabel.Text == playerId)
			{
				return;
			}
			this.playerIdLabel.Text = playerId;
		}

		public void Fade()
		{
			if (Service.CurrentPlayer.HasNotCompletedFirstFueStep())
			{
				this.OnWipeComplete(null);
			}
			else
			{
				Service.CameraManager.WipeCamera.StartLinearWipe(WipeTransition.FromIntroToBase, 1.57079637f, new WipeCompleteDelegate(this.OnWipeComplete), null);
				Service.WorldInitializer.View.SetIsoVantage(CameraFeel.Medium);
			}
		}
	}
}
