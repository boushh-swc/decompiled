using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class ShopStickerViewModule
	{
		private const string CRYSTALS_TAB_GROUP = "WidgetCrystalsNormal";

		private const string CRYSTAL_SPECIAL_TAB_GROUP = "PanelCrystalsSpecial";

		private const string TEXTURE_HOLDER_CRYSTALS_SPECIAL = "TextureCrystalsIconSpecial";

		private const string TEXTURE_CRYSTALS_SPECIAL = "storeicon_lg_crystals_special";

		private const string CRYSTAL_STORE_SALE_EXPIRATION_TIMER = "crystal_sale_sticker_expiration_timer";

		private const string CRYSTAL_JEWEL_PARTICLES = "particlesJewelCrystals";

		private const string SPECIAL_SUFFIX = "Special";

		private const string STICKER_TAB_TEXTURE_FORMAT = "Texture{0}Icon";

		private const string STICKER_TAB_WIDGET_FORMAT = "Widget{0}Special";

		private const string STICKER_TAB_BANNER_GRADIENT_FORMAT = "SpriteBannerGradient{0}Special";

		private const string STICKER_TAB_BANNER_FORMAT = "SpriteBanner{0}Special";

		private const string STICKER_TAB_DEALS_LABEL_FORMAT = "LabelSee{0}Deals";

		private const string STICKER_TAB_DEALS_STROKE_FORMAT = "SpriteSee{0}DealsStroke";

		private const string STICKER_TAB_DEALS_ICON_FORMAT = "SpriteSee{0}DealsIcon";

		private const string STICKER_TAB_BG_STROKE_FORMAT = "SpriteBkgStrokeTab{0}";

		private const string STICKER_TAB_DETAIL_RIGHT_FORMAT = "SpriteBkgDetailRightTab{0}";

		private const string STICKER_TAB_DETAIL_LEFT_FORMAT = "SpriteBkgDetailLeftTab{0}";

		private const string STICKER_TAB_INNER_GLOW_FORMAT = "SpriteBkgInnerGlowTab{0}";

		private const float DETAILS_ALPHA = 0.5f;

		private const float INNER_GLOW_ALPHA = 0.4f;

		private static readonly StoreTab[] STICKER_TABS = new StoreTab[]
		{
			StoreTab.Fragments,
			StoreTab.Treasure,
			StoreTab.Structures,
			StoreTab.Turrets,
			StoreTab.Crystals
		};

		private StoreScreen parentStoreScreen;

		private Dictionary<StoreTab, JewelControl> tabStickers;

		private Dictionary<string, Color> orginalUIColors;

		private Dictionary<string, string> orginalTextureNames;

		public ShopStickerViewModule(StoreScreen parentStoreScreen)
		{
			this.parentStoreScreen = parentStoreScreen;
			this.tabStickers = new Dictionary<StoreTab, JewelControl>();
			this.orginalUIColors = new Dictionary<string, Color>();
			this.orginalTextureNames = new Dictionary<string, string>();
		}

		public void SetupStickers()
		{
			int num = ShopStickerViewModule.STICKER_TABS.Length;
			for (int i = 0; i < num; i++)
			{
				this.SetupSticker(ShopStickerViewModule.STICKER_TABS[i]);
			}
		}

		public void CheckForStickerExpiration()
		{
			int num = ShopStickerViewModule.STICKER_TABS.Length;
			int serverTime = (int)Service.ServerAPI.ServerTime;
			for (int i = 0; i < num; i++)
			{
				StoreTab storeTab = ShopStickerViewModule.STICKER_TABS[i];
				int endTime = this.tabStickers[storeTab].EndTime;
				if (endTime > 0 && serverTime >= endTime)
				{
					this.SetupSticker(storeTab);
				}
			}
		}

		private void AddOriginalUIColor(string id, Color color)
		{
			if (!this.orginalUIColors.ContainsKey(id))
			{
				this.orginalUIColors.Add(id, color);
			}
		}

		private void RestoreOriginalSpriteColor(string id, UXSprite sprite)
		{
			if (sprite != null && this.orginalUIColors.ContainsKey(id))
			{
				sprite.Color = this.orginalUIColors[id];
			}
		}

		private void RestoreOriginalLabelColor(string id, UXLabel label)
		{
			if (label != null && this.orginalUIColors.ContainsKey(id))
			{
				label.TextColor = this.orginalUIColors[id];
			}
		}

		private void SetupStickerVisuals(StoreTab tab, StickerVO vo)
		{
			string text = string.Format("Texture{0}Icon", tab.ToString());
			string name = string.Format("Widget{0}Special", tab.ToString());
			string text2 = string.Format("SpriteBannerGradient{0}Special", tab.ToString());
			string text3 = string.Format("SpriteBanner{0}Special", tab.ToString());
			string text4 = string.Format("LabelSee{0}Deals", tab.ToString());
			string text5 = string.Format("SpriteSee{0}DealsStroke", tab.ToString());
			string text6 = string.Format("SpriteSee{0}DealsIcon", tab.ToString());
			string text7 = string.Format("SpriteBkgStrokeTab{0}", tab.ToString());
			string text8 = string.Format("SpriteBkgDetailRightTab{0}", tab.ToString());
			string text9 = string.Format("SpriteBkgDetailLeftTab{0}", tab.ToString());
			string text10 = string.Format("SpriteBkgInnerGlowTab{0}", tab.ToString());
			UXTexture optionalElement = this.parentStoreScreen.GetOptionalElement<UXTexture>(text);
			FactionType faction = Service.CurrentPlayer.Faction;
			if (optionalElement != null)
			{
				string text11 = string.Empty;
				if (faction == FactionType.Rebel)
				{
					text11 = vo.TextureOverrideAssetNameRebel;
				}
				else
				{
					text11 = vo.TextureOverrideAssetNameEmpire;
				}
				if (!string.IsNullOrEmpty(text11))
				{
					string assetName = optionalElement.AssetName;
					this.orginalTextureNames.Add(text, assetName);
					optionalElement.LoadTexture(text11);
				}
			}
			UXElement optionalElement2 = this.parentStoreScreen.GetOptionalElement<UXElement>(name);
			if (optionalElement2 != null)
			{
				optionalElement2.Visible = true;
			}
			UXSprite optionalElement3 = this.parentStoreScreen.GetOptionalElement<UXSprite>(text2);
			if (optionalElement3 != null && !string.IsNullOrEmpty(vo.GradientColor))
			{
				this.AddOriginalUIColor(text2, optionalElement3.Color);
				optionalElement3.Color = FXUtils.ConvertHexStringToColorObject(vo.GradientColor);
			}
			if (string.IsNullOrEmpty(vo.MainColor))
			{
				return;
			}
			Color color = FXUtils.ConvertHexStringToColorObject(vo.MainColor);
			UXSprite optionalElement4 = this.parentStoreScreen.GetOptionalElement<UXSprite>(text3);
			if (optionalElement4 != null)
			{
				this.AddOriginalUIColor(text3, optionalElement4.Color);
				optionalElement4.Color = color;
			}
			UXLabel optionalElement5 = this.parentStoreScreen.GetOptionalElement<UXLabel>(text4);
			if (optionalElement5 != null)
			{
				this.AddOriginalUIColor(text4, optionalElement5.TextColor);
				optionalElement5.TextColor = color;
			}
			UXSprite optionalElement6 = this.parentStoreScreen.GetOptionalElement<UXSprite>(text5);
			if (optionalElement6 != null)
			{
				this.AddOriginalUIColor(text5, optionalElement6.Color);
				optionalElement6.Color = color;
			}
			UXSprite optionalElement7 = this.parentStoreScreen.GetOptionalElement<UXSprite>(text6);
			if (optionalElement7 != null)
			{
				this.AddOriginalUIColor(text6, optionalElement7.Color);
				optionalElement7.Color = color;
			}
			UXSprite optionalElement8 = this.parentStoreScreen.GetOptionalElement<UXSprite>(text7);
			if (optionalElement8 != null)
			{
				this.AddOriginalUIColor(text7, optionalElement8.Color);
				optionalElement8.Color = color;
			}
			Color color2 = new Color(color.r, color.g, color.b, 0.5f);
			UXSprite optionalElement9 = this.parentStoreScreen.GetOptionalElement<UXSprite>(text8);
			if (optionalElement9 != null)
			{
				this.AddOriginalUIColor(text8, optionalElement9.Color);
				optionalElement9.Color = color2;
			}
			UXSprite optionalElement10 = this.parentStoreScreen.GetOptionalElement<UXSprite>(text9);
			if (optionalElement10 != null)
			{
				this.AddOriginalUIColor(text9, optionalElement10.Color);
				optionalElement10.Color = color2;
			}
			UXSprite optionalElement11 = this.parentStoreScreen.GetOptionalElement<UXSprite>(text10);
			if (optionalElement11 != null)
			{
				this.AddOriginalUIColor(text10, optionalElement11.Color);
				optionalElement11.Color = new Color(color.r, color.g, color.b, 0.4f);
			}
		}

		private void HideStickerVisuals(StoreTab tab)
		{
			string text = string.Format("Texture{0}Icon", tab.ToString());
			string name = string.Format("Widget{0}Special", tab.ToString());
			string text2 = string.Format("SpriteBannerGradient{0}Special", tab.ToString());
			string text3 = string.Format("SpriteBanner{0}Special", tab.ToString());
			string text4 = string.Format("LabelSee{0}Deals", tab.ToString());
			string text5 = string.Format("SpriteSee{0}DealsStroke", tab.ToString());
			string text6 = string.Format("SpriteSee{0}DealsIcon", tab.ToString());
			string text7 = string.Format("SpriteBkgStrokeTab{0}", tab.ToString());
			string text8 = string.Format("SpriteBkgDetailRightTab{0}", tab.ToString());
			string text9 = string.Format("SpriteBkgDetailLeftTab{0}", tab.ToString());
			string text10 = string.Format("SpriteBkgInnerGlowTab{0}", tab.ToString());
			UXTexture optionalElement = this.parentStoreScreen.GetOptionalElement<UXTexture>(text);
			if (optionalElement != null && this.orginalTextureNames.ContainsKey(text))
			{
				optionalElement.LoadTexture(this.orginalTextureNames[text]);
			}
			UXElement optionalElement2 = this.parentStoreScreen.GetOptionalElement<UXElement>(name);
			if (optionalElement2 != null)
			{
				optionalElement2.Visible = false;
			}
			UXSprite optionalElement3 = this.parentStoreScreen.GetOptionalElement<UXSprite>(text2);
			this.RestoreOriginalSpriteColor(text2, optionalElement3);
			UXSprite optionalElement4 = this.parentStoreScreen.GetOptionalElement<UXSprite>(text3);
			this.RestoreOriginalSpriteColor(text3, optionalElement4);
			UXLabel optionalElement5 = this.parentStoreScreen.GetOptionalElement<UXLabel>(text4);
			this.RestoreOriginalLabelColor(text4, optionalElement5);
			UXSprite optionalElement6 = this.parentStoreScreen.GetOptionalElement<UXSprite>(text5);
			this.RestoreOriginalSpriteColor(text5, optionalElement6);
			UXSprite optionalElement7 = this.parentStoreScreen.GetOptionalElement<UXSprite>(text6);
			this.RestoreOriginalSpriteColor(text6, optionalElement7);
			UXSprite optionalElement8 = this.parentStoreScreen.GetOptionalElement<UXSprite>(text7);
			this.RestoreOriginalSpriteColor(text7, optionalElement8);
			UXSprite optionalElement9 = this.parentStoreScreen.GetOptionalElement<UXSprite>(text8);
			this.RestoreOriginalSpriteColor(text8, optionalElement9);
			UXSprite optionalElement10 = this.parentStoreScreen.GetOptionalElement<UXSprite>(text9);
			this.RestoreOriginalSpriteColor(text9, optionalElement10);
			UXSprite optionalElement11 = this.parentStoreScreen.GetOptionalElement<UXSprite>(text10);
			this.RestoreOriginalSpriteColor(text10, optionalElement11);
		}

		private void SetupTabSticker(JewelControl jewel, StoreTab tab)
		{
			if (this.parentStoreScreen.IsTabLocked(tab))
			{
				this.HideStickerVisuals(tab);
				return;
			}
			StickerType stickerTypeFromStoreTab = this.GetStickerTypeFromStoreTab(tab);
			StickerController stickerController = Service.StickerController;
			StickerVO storeStickerToDisplay = stickerController.GetStoreStickerToDisplay(stickerTypeFromStoreTab);
			jewel.EndTime = 0;
			if (storeStickerToDisplay == null)
			{
				jewel.Cleanup();
				this.HideStickerVisuals(tab);
				return;
			}
			jewel.EndTime = storeStickerToDisplay.EndTime;
			this.SetupStickerVisuals(tab, storeStickerToDisplay);
		}

		private void SetupCrystalsSticker(JewelControl jewel)
		{
			StickerController stickerController = Service.StickerController;
			StickerVO storeStickerToDisplay = stickerController.GetStoreStickerToDisplay(StickerType.CrystalShop);
			Lang lang = Service.Lang;
			bool flag = storeStickerToDisplay != null && this.parentStoreScreen.HasInAppPurchaseItems();
			this.parentStoreScreen.GetElement<UXElement>("PanelCrystalsSpecial").Visible = false;
			this.parentStoreScreen.GetElement<UXElement>("WidgetCrystalsNormal").Visible = true;
			jewel.EndTime = 0;
			if (!flag)
			{
				jewel.Cleanup();
				return;
			}
			jewel.EndTime = storeStickerToDisplay.EndTime;
			string assetName = "storeicon_lg_crystals_special";
			string factionBasedTextureAsset = stickerController.GetFactionBasedTextureAsset(storeStickerToDisplay);
			if (!string.IsNullOrEmpty(factionBasedTextureAsset))
			{
				assetName = factionBasedTextureAsset;
			}
			this.parentStoreScreen.GetElement<UXTexture>("TextureCrystalsIconSpecial").LoadTexture(assetName, new Action(this.OnCrystalsSpecialTextureLoaded));
			jewel.Text = lang.Get(storeStickerToDisplay.LabelText, new object[0]);
			string iconAsset = storeStickerToDisplay.IconAsset;
			bool flag2 = !string.IsNullOrEmpty(iconAsset);
			if (flag2)
			{
				jewel.Icon = iconAsset;
			}
			this.parentStoreScreen.GetElement<UXElement>("particlesJewelCrystals").Visible = flag2;
			if (jewel.TimerLabel != null)
			{
				jewel.TimerLabel.TextColor = UXUtils.COLOR_CRYSTALS_EXPIRE_LABEL_NORMAL;
				CountdownControl countdownControl = new CountdownControl(jewel.TimerLabel, lang.Get("crystal_sale_sticker_expiration_timer", new object[0]), storeStickerToDisplay.EndTime);
				countdownControl.SetThreshold(GameConstants.CRYSTAL_STORE_SALE_EXPIRATION_TIMER_WARNING, UXUtils.COLOR_CRYSTALS_EXPIRE_LABEL_WARNING);
			}
		}

		private void OnCrystalsSpecialTextureLoaded()
		{
			this.parentStoreScreen.GetElement<UXElement>("PanelCrystalsSpecial").Visible = true;
			this.parentStoreScreen.GetElement<UXElement>("WidgetCrystalsNormal").Visible = false;
		}

		private void SetupSticker(StoreTab tab)
		{
			JewelControl jewelControl;
			if (this.tabStickers.ContainsKey(tab))
			{
				jewelControl = this.tabStickers[tab];
			}
			else
			{
				string name = tab.ToString();
				string parentName = tab.ToString() + "Special";
				jewelControl = JewelControl.CreateSticker(this.parentStoreScreen, name, parentName);
				if (jewelControl == null)
				{
					return;
				}
				this.tabStickers.Add(tab, jewelControl);
			}
			if (tab == StoreTab.Crystals)
			{
				this.SetupCrystalsSticker(jewelControl);
			}
			else
			{
				this.SetupTabSticker(jewelControl, tab);
			}
		}

		private StickerType GetStickerTypeFromStoreTab(StoreTab tab)
		{
			StickerType result = StickerType.Invalid;
			switch (tab)
			{
			case StoreTab.Treasure:
				result = StickerType.TreasureShop;
				break;
			case StoreTab.Crystals:
				result = StickerType.CrystalShop;
				break;
			case StoreTab.Fragments:
				result = StickerType.FragmentShop;
				break;
			case StoreTab.Structures:
				result = StickerType.StructureShop;
				break;
			}
			return result;
		}
	}
}
