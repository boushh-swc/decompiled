using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers.Planets;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StaRTS.Main.Views.UX
{
	public static class UXUtils
	{
		private const float FRAME_DELAY = 0.04f;

		private const string APPEND_FORMAT = "{0} ({1})";

		private const string REMOVE_FORMAT = " ({0})";

		private const string START_COLOR = "[{0}]";

		private const string END_COLOR = "[-]";

		private const string COST_CREDIT_ICON = "CreditIcon";

		private const string COST_MATERIAL_ICON = "MaterialIcon";

		private const string COST_CONTRABAND_ICON = "ContrabandIcon";

		private const string COST_CRYSTAL_ICON = "CrystalIcon";

		private const string COST_EVENT_ICON = "EventIcon";

		private const string COST_REPUTATION_ICON = "ReputationIcon";

		private const string COST_LABEL = "Label";

		public const string PROTECTION = "protection";

		private const int REWARD_DEFAULT_NUM = 3;

		public const string COLOR_EMPIRE_STRING = "c0d0ff";

		public const string COLOR_REBEL_STRING = "f0dfc1";

		private const string UPLINK_ACTIVE_SPRITE = "icoVictoryPoint";

		private const string UPLINK_DISABLED_SPRITE = "AnimUplinkC";

		private const string CRATE_INVENTORY_CRATE_NO_EXPIRATION_TIMER = "CRATE_INVENTORY_CRATE_NO_EXPIRATION_TIMER";

		private const string CRATE_INVENTORY_CRATE_EXPIRATION_TIMER = "CRATE_INVENTORY_CRATE_EXPIRATION_TIMER";

		private const string CRATE_FLYOUT_LEI_EXPIRATION_TIMER = "CRATE_FLYOUT_LEI_EXPIRATION_TIMER";

		private const string COMPENSATION_TITLE = "comp_popup_title";

		private const string COMPENSATION_MESSAGE = "comp_popup_body";

		private const string COMPENSATION_CTA = "comp_popup_cta";

		private const string FRAGMENT_SPRITE_FORMAT = "icoDataFragQ{0}";

		private const string EXPIRES_IN = "expires_in";

		public static readonly Color COLOR_COST_CANNOT_AFFORD = new Color(0.956862748f, 0f, 0f);

		public static readonly Color COLOR_COST_LOCKED = new Color(0.156862751f, 0.156862751f, 0.156862751f);

		public static readonly Color COLOR_INACTIVE_TROOP_TAB = new Color(0.0392156877f, 0.4509804f, 0.5882353f);

		public static readonly Color COLOR_REPLAY_VICTORY = new Color(0.06666667f, 0.9490196f, 0.145098045f);

		public static readonly Color COLOR_REPLAY_DEFEAT = new Color(0.9490196f, 0.06666667f, 0.06666667f);

		public static readonly Color COLOR_LABEL_DISABLED = new Color(0.5019608f, 0.5019608f, 0.5019608f);

		public static readonly Color COLOR_ENABLED = Color.black;

		public static readonly Color COLOR_NAV_TAB_ENABLED = new Color(0f, 0f, 0f);

		public static readonly Color COLOR_NAV_TAB_DISABLED = new Color(0.784313738f, 0.9098039f, 1f);

		public static readonly Color COLOR_PERK_EFFECT_NOT_APPLIED = new Color(0.5019608f, 0.5019608f, 0.5019608f);

		public static readonly Color COLOR_CRATE_EXPIRE_LABEL_NORMAL = new Color(0.784313738f, 0.9098039f, 1f);

		public static readonly Color COLOR_CRATE_EXPIRE_LABEL_WARNING = new Color(0.956862748f, 0f, 0f);

		public static readonly Color COLOR_CRYSTALS_EXPIRE_LABEL_NORMAL = new Color(0.784313738f, 0.9098039f, 1f);

		public static readonly Color COLOR_CRYSTALS_EXPIRE_LABEL_WARNING = new Color(0.956862748f, 0f, 0f);

		public static readonly Color COLOR_SHARD_INPROGRESS = new Color(0.137254909f, 0.670588255f, 0.980392158f);

		public static readonly Color COLOR_SHARD_COMPLETE = new Color(0.380392164f, 0.65882355f, 0.164705887f);

		private static readonly Color UPLINK_ACTIVE_TINT = Color.white;

		private static readonly Color UPLINK_DISABLED_TINT = new Color(0.06f, 0.33f, 0.48f);

		public static string FormatNameToOriginalName(string currentName, string appendedName)
		{
			return (!string.IsNullOrEmpty(appendedName)) ? currentName.Replace(string.Format(" ({0})", appendedName), string.Empty) : currentName;
		}

		public static void AppendNameRecursively(GameObject gameObject, string name, bool root)
		{
			if (root)
			{
				gameObject.name = name;
			}
			else
			{
				gameObject.name = UXUtils.FormatAppendedName(gameObject.name, name);
			}
			Transform transform = gameObject.transform;
			int i = 0;
			int childCount = transform.childCount;
			while (i < childCount)
			{
				UXUtils.AppendNameRecursively(transform.GetChild(i).gameObject, name, false);
				i++;
			}
		}

		public static void HideChildrenRecursively(GameObject gameObject, bool isRoot)
		{
			if (!isRoot)
			{
				gameObject.SetActive(false);
			}
			Transform transform = gameObject.transform;
			int i = 0;
			int childCount = transform.childCount;
			while (i < childCount)
			{
				UXUtils.HideChildrenRecursively(transform.GetChild(i).gameObject, false);
				i++;
			}
		}

		public static string FormatAppendedName(string originalName, string appendedName)
		{
			return (!string.IsNullOrEmpty(appendedName)) ? string.Format("{0} ({1})", originalName, appendedName) : originalName;
		}

		public static void SetSpriteTopAnchorPoint(UXSprite sprite, int value)
		{
			if (sprite != null)
			{
				sprite.Root.GetComponent<UISprite>().topAnchor.absolute = value;
			}
		}

		public static Color GetCostColor(UXLabel label, bool canAfford, bool locked)
		{
			if (locked)
			{
				return UXUtils.COLOR_COST_LOCKED;
			}
			if (canAfford)
			{
				return label.OriginalTextColor;
			}
			return UXUtils.COLOR_COST_CANNOT_AFFORD;
		}

		public static Color GetWinResultColor(bool win)
		{
			return (!win) ? Color.red : Color.green;
		}

		public static void SetupMultiCostElements(UXFactory uxFactory, string[] costElementNames, string clonedParentName, string[] cost, int costMapCount)
		{
			int credits;
			int num;
			int num2;
			int num3;
			GameUtils.GetHQScaledCurrency(cost, out credits, out num, out num2, out num3);
			bool locked = false;
			string lockedText = null;
			UXUtils.SetupCostElements(uxFactory, costElementNames[0], clonedParentName, credits, num, num2, 0, 0, locked, lockedText);
			if (costMapCount == 2 && costElementNames.Length == 2)
			{
				if (num != 0 && num2 != 0)
				{
					num = 0;
				}
				UXUtils.SetupCostElements(uxFactory, costElementNames[1], clonedParentName, 0, num, num2, 0, 0, locked, lockedText);
			}
		}

		public static void SetupCostElements(UXFactory uxFactory, string costElementName, string clonedParentName, int credits, int materials, int contraband, int crystals, bool locked, string lockedText)
		{
			UXUtils.SetupCostElements(uxFactory, costElementName, clonedParentName, credits, materials, contraband, crystals, 0, locked, lockedText);
		}

		public static void SetupCostElements(UXFactory uxFactory, string costElementName, string clonedParentName, int credits, int materials, int contraband, int crystals, bool locked, string lockedText, int clampWidth)
		{
			UXUtils.SetupCostElements(uxFactory, costElementName, clonedParentName, credits, materials, contraband, crystals, 0, locked, lockedText, clampWidth);
		}

		public static void SetupCostElements(UXFactory uxFactory, string costElementName, string clonedParentName, int credits, int materials, int contraband, int crystals, int eventPoints, bool locked, string lockedText)
		{
			UXUtils.SetupCostElements(uxFactory, costElementName, clonedParentName, credits, materials, contraband, crystals, eventPoints, locked, lockedText, -1);
		}

		public static void SetupCostElements(UXFactory uxFactory, string costElementName, string clonedParentName, int credits, int materials, int contraband, int crystals, int eventPoints, bool locked, string lockedText, int clampWidth)
		{
			string costGroupStr = UXUtils.FormatAppendedName(costElementName, clonedParentName);
			string costElementStr = UXUtils.FormatAppendedName(costElementName + "Label", clonedParentName);
			string creditName = UXUtils.FormatAppendedName(costElementName + "CreditIcon", clonedParentName);
			string materialName = UXUtils.FormatAppendedName(costElementName + "MaterialIcon", clonedParentName);
			string contrabandName = UXUtils.FormatAppendedName(costElementName + "ContrabandIcon", clonedParentName);
			string crystalName = UXUtils.FormatAppendedName(costElementName + "CrystalIcon", clonedParentName);
			string eventPointsName = UXUtils.FormatAppendedName(costElementName + "EventIcon", clonedParentName);
			string reputationName = UXUtils.FormatAppendedName(costElementName + "ReputationIcon", clonedParentName);
			UXUtils.CommonSetupTasks(uxFactory, credits, materials, contraband, crystals, eventPoints, locked, lockedText, costGroupStr, creditName, materialName, contrabandName, crystalName, eventPointsName, reputationName, costElementStr, clampWidth);
		}

		public static void SetupSingleCostElement(UXFactory uxFactory, string costElementName, int credits, int materials, int contraband, int crystals, int eventPoints, bool locked, string lockedText)
		{
			string costElementStr = costElementName + "Label";
			string creditName = costElementName + "CreditIcon";
			string materialName = costElementName + "MaterialIcon";
			string contrabandName = costElementName + "ContrabandIcon";
			string crystalName = costElementName + "CrystalIcon";
			string eventPointsName = costElementName + "EventIcon";
			string reputationName = costElementName + "ReputationIcon";
			UXUtils.CommonSetupTasks(uxFactory, credits, materials, contraband, crystals, eventPoints, locked, lockedText, costElementName, creditName, materialName, contrabandName, crystalName, eventPointsName, reputationName, costElementStr, -1);
		}

		private static void CommonSetupTasks(UXFactory uxFactory, int credits, int materials, int contraband, int crystals, int eventPoints, bool locked, string lockedText, string costGroupStr, string creditName, string materialName, string contrabandName, string crystalName, string eventPointsName, string reputationName, string costElementStr, int clampWidth)
		{
			if (uxFactory == null)
			{
				return;
			}
			UXElement element = uxFactory.GetElement<UXElement>(costGroupStr);
			bool flag = credits != 0 || materials != 0 || contraband != 0 || crystals != 0 || eventPoints != 0 || !string.IsNullOrEmpty(lockedText);
			element.Visible = flag;
			if (flag)
			{
				UXSprite optionalElement = uxFactory.GetOptionalElement<UXSprite>(creditName);
				UXSprite optionalElement2 = uxFactory.GetOptionalElement<UXSprite>(materialName);
				UXSprite optionalElement3 = uxFactory.GetOptionalElement<UXSprite>(contrabandName);
				UXSprite optionalElement4 = uxFactory.GetOptionalElement<UXSprite>(crystalName);
				UXSprite optionalElement5 = uxFactory.GetOptionalElement<UXSprite>(eventPointsName);
				UXSprite optionalElement6 = uxFactory.GetOptionalElement<UXSprite>(reputationName);
				UXUtils.HideIcon(optionalElement6);
				UXSprite icon = null;
				int associatedIconWidth = 0;
				if (!uxFactory.HasElement<UXLabel>(costElementStr))
				{
					Service.Logger.Error("UXFactory missing needed label: " + costElementStr);
					return;
				}
				UXLabel element2 = uxFactory.GetElement<UXLabel>(costElementStr);
				element2.Root.GetComponent<UILabel>().overflowMethod = UILabel.Overflow.ResizeFreely;
				Lang lang = Service.Lang;
				if (credits != 0)
				{
					if (optionalElement != null)
					{
						associatedIconWidth = optionalElement.GetUIWidget.width;
						UXUtils.ShowIcon(optionalElement);
						icon = optionalElement;
					}
					UXUtils.HideIcon(optionalElement2);
					UXUtils.HideIcon(optionalElement3);
					UXUtils.HideIcon(optionalElement4);
					UXUtils.HideIcon(optionalElement5);
					element2.Text = lang.ThousandsSeparated(credits);
				}
				else if (materials != 0)
				{
					if (optionalElement2 != null)
					{
						associatedIconWidth = optionalElement2.GetUIWidget.width;
						UXUtils.ShowIcon(optionalElement2);
						icon = optionalElement2;
					}
					UXUtils.HideIcon(optionalElement);
					UXUtils.HideIcon(optionalElement3);
					UXUtils.HideIcon(optionalElement4);
					UXUtils.HideIcon(optionalElement5);
					element2.Text = lang.ThousandsSeparated(materials);
				}
				else if (contraband != 0)
				{
					if (optionalElement3 != null)
					{
						associatedIconWidth = optionalElement3.GetUIWidget.width;
						UXUtils.ShowIcon(optionalElement3);
						icon = optionalElement3;
					}
					UXUtils.HideIcon(optionalElement);
					UXUtils.HideIcon(optionalElement2);
					UXUtils.HideIcon(optionalElement4);
					UXUtils.HideIcon(optionalElement5);
					element2.Text = lang.ThousandsSeparated(contraband);
				}
				else if (crystals != 0)
				{
					if (optionalElement4 != null)
					{
						associatedIconWidth = optionalElement4.GetUIWidget.width;
						UXUtils.ShowIcon(optionalElement4);
						icon = optionalElement4;
					}
					UXUtils.HideIcon(optionalElement);
					UXUtils.HideIcon(optionalElement2);
					UXUtils.HideIcon(optionalElement3);
					UXUtils.HideIcon(optionalElement5);
					element2.Text = lang.ThousandsSeparated(crystals);
				}
				else if (eventPoints != 0)
				{
					if (optionalElement5 != null)
					{
						associatedIconWidth = optionalElement5.GetUIWidget.width;
						UXUtils.ShowIcon(optionalElement5);
						icon = optionalElement5;
					}
					UXUtils.HideIcon(optionalElement);
					UXUtils.HideIcon(optionalElement2);
					UXUtils.HideIcon(optionalElement3);
					UXUtils.HideIcon(optionalElement4);
					element2.Text = lang.ThousandsSeparated(eventPoints);
				}
				else
				{
					Vector3 localPosition = element2.LocalPosition;
					UXUtils.HideIconMinX(optionalElement, ref localPosition);
					UXUtils.HideIconMinX(optionalElement2, ref localPosition);
					UXUtils.HideIconMinX(optionalElement3, ref localPosition);
					UXUtils.HideIconMinX(optionalElement4, ref localPosition);
					UXUtils.HideIconMinX(optionalElement5, ref localPosition);
					element2.Text = lockedText;
					element2.TextColor = UXUtils.GetCostColor(element2, false, true);
					element2.LocalPosition = localPosition;
				}
				UXUtils.UpdateCostColor(element2, icon, credits, materials, contraband, crystals, eventPoints, locked);
				UXUtils.ClampUILabelWidth(element2, clampWidth, associatedIconWidth);
				UXUtils.CostGroupSetup(element);
			}
		}

		public static void SetupCurrencySprite(UXSprite currencySprite, CurrencyType type)
		{
			string text = "ico";
			switch (type)
			{
			case CurrencyType.Credits:
				text += "GalacticCredit";
				break;
			case CurrencyType.Materials:
				text += "Materials";
				break;
			case CurrencyType.Contraband:
				text += "Contraband";
				break;
			case CurrencyType.Crystals:
				text += "Crystals";
				break;
			}
			currencySprite.SpriteName = text;
		}

		private static void CostGroupSetup(UXElement costGroup)
		{
			if (costGroup != null)
			{
				Vector3 vector = UXUtils.CalculateElementSize(costGroup);
				Vector3 localPosition = costGroup.LocalPosition;
				costGroup.LocalPosition = new Vector3(-vector.x * 0.5f, localPosition.y, localPosition.z);
			}
		}

		public static void ClampUILabelWidth(UXLabel label, int pixelWidth, int associatedIconWidth)
		{
			if (pixelWidth > 0)
			{
				UILabel uILabel = (UILabel)label.GetUIWidget;
				int num = pixelWidth - associatedIconWidth;
				uILabel.overflowMethod = UILabel.Overflow.ShrinkContent;
				if (num < uILabel.width)
				{
					uILabel.SetDimensions(num, uILabel.height);
					uILabel.ResizeCollider();
				}
			}
		}

		public static void UpdateCostColor(UXLabel label, UXSprite icon, int credits, int materials, int contraband, int crystals, int eventPoints, bool locked)
		{
			UXUtils.UpdateCostColor(label, icon, credits, materials, contraband, crystals, eventPoints, 0, locked);
		}

		public static void UpdateCostColor(UXLabel label, UXSprite icon, int credits, int materials, int contraband, int crystals, int eventPoints, int episodePoints, bool locked)
		{
			Color costColor;
			if (credits != 0)
			{
				costColor = UXUtils.GetCostColor(label, GameUtils.CanAffordCredits(credits), locked);
			}
			else if (materials != 0)
			{
				costColor = UXUtils.GetCostColor(label, GameUtils.CanAffordMaterials(materials), locked);
			}
			else if (contraband != 0)
			{
				costColor = UXUtils.GetCostColor(label, GameUtils.CanAffordContraband(contraband), locked);
			}
			else if (crystals != 0)
			{
				costColor = UXUtils.GetCostColor(label, GameUtils.CanAffordCrystals(crystals), locked);
			}
			else if (episodePoints != 0)
			{
				costColor = UXUtils.GetCostColor(label, episodePoints <= 0, locked);
			}
			else if (locked)
			{
				costColor = UXUtils.GetCostColor(label, false, true);
			}
			else
			{
				costColor = UXUtils.GetCostColor(label, true, false);
			}
			label.TextColor = costColor;
			if (icon != null)
			{
				icon.Color = ((!locked) ? Color.white : costColor);
			}
		}

		private static void ShowIcon(UXSprite icon)
		{
			if (icon != null)
			{
				icon.Visible = true;
			}
		}

		private static void HideIcon(UXSprite icon)
		{
			if (icon != null)
			{
				icon.Visible = false;
			}
		}

		private static void HideIconMinX(UXSprite icon, ref Vector3 minP)
		{
			if (icon != null)
			{
				icon.Visible = false;
				minP.x = Mathf.Min(minP.x, icon.LocalPosition.x);
			}
		}

		public static Vector3 CalculateElementSize(UXElement element)
		{
			if (element.Root != null)
			{
				Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(element.Root.transform);
				return new Vector3(element.UXCamera.ScaleColliderHorizontally(bounds.size.x), element.UXCamera.ScaleColliderVertically(bounds.size.y), 0f);
			}
			return Vector3.zero;
		}

		public static bool ShouldShowHudBehindScreen(string assetName)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (UITypeVO current in staticDataController.GetAll<UITypeVO>())
			{
				if (current.AssetName == assetName)
				{
					return current.ShowHUDWhileDisplayed;
				}
			}
			return true;
		}

		public static bool ShouldHideHudAfterClosingScreen(string assetName)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (UITypeVO current in staticDataController.GetAll<UITypeVO>())
			{
				if (current.AssetName == assetName)
				{
					return current.HideHUDAfterClosing;
				}
			}
			return false;
		}

		public static string GetIconNameFromFactionType(FactionType factionType)
		{
			string result = "icoNeutral";
			if (factionType != FactionType.Empire)
			{
				if (factionType == FactionType.Rebel)
				{
					result = "icoRebel";
				}
			}
			else
			{
				result = "IcoEmpire";
			}
			return result;
		}

		public static string GetIconNameFromTroopClass(TroopRole troopClass)
		{
			string result = string.Empty;
			switch (troopClass)
			{
			case TroopRole.Striker:
				result = "ClassIcon_general";
				break;
			case TroopRole.Breacher:
				result = "ClassIcon_wallbreaker";
				break;
			case TroopRole.Looter:
				result = "ClassIcon_neutralizer";
				break;
			case TroopRole.Bruiser:
				result = "ClassIcon_meatshield";
				break;
			case TroopRole.Healer:
				result = "ClassIcon_medic";
				break;
			default:
				result = "ClassIcon_general";
				break;
			}
			return result;
		}

		public static string GetCurrencyItemAssetName(string currencyType)
		{
			return UXUtils.GetCurrencyItemAssetName(currencyType, 3);
		}

		public static string GetCurrencyItemAssetName(string currencyType, int num)
		{
			return currencyType + num;
		}

		public static CurrencyIconVO GetCurrencyIconVO(string iconType)
		{
			return Service.StaticDataController.Get<CurrencyIconVO>(iconType.ToLower());
		}

		public static CurrencyIconVO GetDefaultCurrencyIconVO(string iconType)
		{
			return Service.StaticDataController.Get<CurrencyIconVO>(iconType.ToLower() + 3);
		}

		public static CurrencyIconVO GetCurrencyIconVO(string iconType, int num)
		{
			return Service.StaticDataController.Get<CurrencyIconVO>(iconType.ToLower() + num);
		}

		public static void SetupGeometryForIcon(UXSprite itemIconSprite, string iconAssetName)
		{
			UXUtils.SetupGeometryForIcon(itemIconSprite, UXUtils.GetCurrencyIconVO(iconAssetName));
		}

		public static void SetupGeometryForIcon(UXSprite itemIconSprite, string iconAssetName, int num)
		{
			UXUtils.SetupGeometryForIcon(itemIconSprite, UXUtils.GetCurrencyIconVO(iconAssetName, num));
		}

		public static void SetupGeometryForIcon(UXSprite itemIconSprite, IGeometryVO data)
		{
			ProjectorConfig config = ProjectorUtils.GenerateGeometryConfig(data, itemIconSprite);
			UXUtils.SetupGeometryForIcon(itemIconSprite, config);
		}

		public static void SetupGeometryForIcon(UXSprite itemIconSprite, ProjectorConfig config)
		{
			itemIconSprite.SpriteName = "bkgClear";
			config.AnimPreference = AnimationPreference.NoAnimation;
			ProjectorUtils.GenerateProjector(config);
			itemIconSprite.GetUIWidget.enabled = false;
		}

		public static void TrySetupItemQualityView(CrateSupplyVO supplyData, UXElement unitElement, UXElement basicElement, UXElement advancedElement, UXElement eliteElement, UXElement defaultElement)
		{
			if (supplyData == null)
			{
				return;
			}
			unitElement.Visible = false;
			basicElement.Visible = false;
			advancedElement.Visible = false;
			eliteElement.Visible = false;
			bool flag = false;
			ShardQuality shardQuality = ShardQuality.Basic;
			if (supplyData.Type == SupplyType.ShardTroop || supplyData.Type == SupplyType.ShardSpecialAttack)
			{
				ShardVO optional = Service.StaticDataController.GetOptional<ShardVO>(supplyData.RewardUid);
				shardQuality = optional.Quality;
				flag = true;
			}
			else if (supplyData.Type == SupplyType.Shard)
			{
				EquipmentVO currentEquipmentDataByID = ArmoryUtils.GetCurrentEquipmentDataByID(supplyData.RewardUid);
				shardQuality = currentEquipmentDataByID.Quality;
				flag = true;
			}
			if (flag)
			{
				unitElement.Visible = (shardQuality == ShardQuality.Unit);
				basicElement.Visible = (shardQuality == ShardQuality.Basic);
				advancedElement.Visible = (shardQuality == ShardQuality.Advanced);
				eliteElement.Visible = (shardQuality == ShardQuality.Elite);
			}
			if (defaultElement != null)
			{
				defaultElement.Visible = !flag;
			}
		}

		public static void SetLeiExpirationTimerLabel(LimitedEditionItemVO leiVO, UXLabel expirationLabel, Lang lang)
		{
			expirationLabel.TextColor = UXUtils.COLOR_CRATE_EXPIRE_LABEL_NORMAL;
			int thresholdSeconds = GameConstants.CRATE_INVENTORY_LEI_EXPIRATION_TIMER_WARNING * 60;
			CountdownControl countdownControl = new CountdownControl(expirationLabel, lang.Get("CRATE_FLYOUT_LEI_EXPIRATION_TIMER", new object[0]), leiVO.EndTime);
			countdownControl.SetThreshold(thresholdSeconds, UXUtils.COLOR_CRATE_EXPIRE_LABEL_WARNING);
		}

		public static void SetCrateExpirationTimerLabel(CrateData crate, UXLabel expirationLabel, Lang lang)
		{
			if (!crate.DoesExpire)
			{
				expirationLabel.Text = lang.Get("CRATE_INVENTORY_CRATE_NO_EXPIRATION_TIMER", new object[0]);
				expirationLabel.TextColor = UXUtils.COLOR_CRATE_EXPIRE_LABEL_NORMAL;
			}
			else
			{
				int thresholdSeconds = GameConstants.CRATE_INVENTORY_EXPIRATION_TIMER_WARNING * 60;
				CountdownControl countdownControl = new CountdownControl(expirationLabel, lang.Get("CRATE_INVENTORY_CRATE_EXPIRATION_TIMER", new object[0]), (int)crate.ExpiresTimeStamp);
				countdownControl.SetThreshold(thresholdSeconds, UXUtils.COLOR_CRATE_EXPIRE_LABEL_WARNING);
			}
		}

		public static void SetCrateTargetedOfferTimerLabel(uint expiresAt, UXLabel expirationLabel, Lang lang)
		{
			expirationLabel.TextColor = UXUtils.COLOR_CRATE_EXPIRE_LABEL_NORMAL;
			int thresholdSeconds = GameConstants.CRATE_INVENTORY_EXPIRATION_TIMER_WARNING * 60;
			CountdownControl countdownControl = new CountdownControl(expirationLabel, lang.Get("expires_in", new object[0]), (int)expiresAt);
			countdownControl.SetThreshold(thresholdSeconds, UXUtils.COLOR_CRATE_EXPIRE_LABEL_WARNING);
		}

		public static void SetShardProgressBarValue(UXSlider slider, UXSprite sprite, float sliderValue)
		{
			slider.Value = ((sliderValue <= 1f) ? sliderValue : 1f);
			sprite.Color = ((sliderValue < 1f) ? UXUtils.COLOR_SHARD_INPROGRESS : UXUtils.COLOR_SHARD_COMPLETE);
		}

		public static float GetScreenRadiusFor3DVolume(Vector3 pos, Vector3 extents)
		{
			float num = (float)Screen.height;
			MainCamera mainCamera = Service.CameraManager.MainCamera;
			Vector3 vector = mainCamera.WorldPositionToScreenPoint(pos);
			Vector3 vector2 = mainCamera.Camera.transform.forward;
			vector2 = Quaternion.AngleAxis(90f, mainCamera.Camera.transform.up) * vector2;
			vector2 *= extents.magnitude;
			Vector3 vector3 = mainCamera.WorldPositionToScreenPoint(pos + vector2);
			Vector2 b = new Vector2(vector.x, num - vector.y);
			Vector2 a = new Vector2(vector3.x, num - vector3.y);
			return (a - b).magnitude;
		}

		public static Bounds CalculateAbsoluteWidgetBound(Transform trans)
		{
			if (trans != null)
			{
				UIWidget component = trans.GetComponent<UIWidget>();
				if (component != null)
				{
					Vector3 vector = new Vector3(3.40282347E+38f, 3.40282347E+38f, 3.40282347E+38f);
					Vector3 vector2 = new Vector3(-3.40282347E+38f, -3.40282347E+38f, -3.40282347E+38f);
					Vector3[] worldCorners = component.worldCorners;
					for (int i = 0; i < 4; i++)
					{
						vector2 = Vector3.Max(worldCorners[i], vector2);
						vector = Vector3.Min(worldCorners[i], vector);
					}
					Bounds result = new Bounds(vector, Vector3.zero);
					result.Encapsulate(vector2);
					return result;
				}
			}
			return new Bounds(Vector3.zero, Vector3.zero);
		}

		public static string WrapTextInColor(string input, string colorCode)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(input);
			if (!string.IsNullOrEmpty(colorCode))
			{
				stringBuilder.Insert(0, string.Format("[{0}]", colorCode));
				stringBuilder.Append("[-]");
			}
			return stringBuilder.ToString();
		}

		public static void SortListForTwoRowGrids(List<UXElement> list, UXGrid grid)
		{
			grid.MaxItemsPerLine = (list.Count + 1) / 2;
			int i = 1;
			int num = list.Count / 2;
			while (i <= num)
			{
				UXElement item = list[i];
				list.RemoveAt(i);
				list.Add(item);
				i++;
			}
		}

		public static void UpdateUplinkHelper(UXSprite sprite, bool active, bool hideIfInactive)
		{
			if (hideIfInactive)
			{
				sprite.Visible = active;
			}
			else if (active)
			{
				sprite.SpriteName = "icoVictoryPoint";
				sprite.Color = UXUtils.UPLINK_ACTIVE_TINT;
			}
			else
			{
				sprite.SpriteName = "AnimUplinkC";
				sprite.Color = UXUtils.UPLINK_DISABLED_TINT;
			}
		}

		public static bool IsElementObjective(UXElement element)
		{
			return element.Tag is ObjectiveViewData;
		}

		public static void HideGridQualityCards(UXGrid grid, string itemUid, string cardName)
		{
			foreach (int num in Enum.GetValues(typeof(ShardQuality)))
			{
				string name = string.Format(cardName, num);
				UXElement optionalSubElement = grid.GetOptionalSubElement<UXElement>(itemUid, name);
				if (optionalSubElement != null)
				{
					optionalSubElement.Visible = false;
				}
			}
		}

		public static void HideQualityCards(UXFactory factory, string defaultCardName, string qualityCardFormat)
		{
			UXElement optionalElement = factory.GetOptionalElement<UXElement>(defaultCardName);
			if (optionalElement != null)
			{
				optionalElement.Visible = false;
			}
			foreach (int num in Enum.GetValues(typeof(ShardQuality)))
			{
				string name = string.Format(qualityCardFormat, num);
				UXElement optionalElement2 = factory.GetOptionalElement<UXElement>(name);
				if (optionalElement2 != null)
				{
					optionalElement2.Visible = false;
				}
			}
		}

		public static UXElement SetCardQuality(UXFactory uxFactory, UXGrid grid, string itemUid, int qualityInt, string cardName)
		{
			string name = string.Format(cardName, qualityInt);
			UXElement optionalSubElement = grid.GetOptionalSubElement<UXElement>(itemUid, name);
			if (optionalSubElement == null)
			{
				return null;
			}
			UXUtils.HideGridQualityCards(grid, itemUid, cardName);
			uxFactory.RevertToOriginalNameRecursively(optionalSubElement.Root, itemUid);
			optionalSubElement.Visible = true;
			return optionalSubElement;
		}

		public static UXElement SetCardQuality(UXFactory uxFactory, UXGrid grid, string itemUid, int qualityInt, string cardName, string defaultCardName)
		{
			UXElement subElement = grid.GetSubElement<UXElement>(itemUid, defaultCardName);
			if (qualityInt == 0)
			{
				subElement.Visible = true;
				UXUtils.HideGridQualityCards(grid, itemUid, cardName);
				return null;
			}
			subElement.Visible = false;
			return UXUtils.SetCardQuality(uxFactory, grid, itemUid, qualityInt, cardName);
		}

		public static void SetupFragmentIconSprite(UXSprite fragmentSprite, int quality)
		{
			if (fragmentSprite != null)
			{
				fragmentSprite.Visible = true;
				fragmentSprite.SpriteName = string.Format("icoDataFragQ{0}", quality);
			}
		}

		public static bool IsVisibleInHierarchy(UXElement element)
		{
			return element != null && element.Visible && element.Root.activeInHierarchy;
		}

		public static void SetupAnimatedCharacter(UXSprite charSprite, string imageName, ref GeometryProjector geom)
		{
			if (charSprite != null && !string.IsNullOrEmpty(imageName))
			{
				StaticDataController staticDataController = Service.StaticDataController;
				TroopTypeVO vo = staticDataController.Get<TroopTypeVO>(imageName);
				if (geom == null)
				{
					ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(vo, charSprite);
					projectorConfig.AnimPreference = AnimationPreference.AnimationAlways;
					geom = ProjectorUtils.GenerateProjector(projectorConfig);
				}
			}
		}

		public static void SetupTargetedOfferCostUI(TargetedBundleVO currentOffer, UXLabel label, UXSprite sprite)
		{
			string[] cost = currentOffer.Cost;
			if (cost == null || cost.Length == 0)
			{
				Service.Logger.Error("SetupCurrencyCostOffer Cost was empty or null: " + currentOffer.Uid);
				return;
			}
			string costString = cost[0];
			CurrencyType type = CurrencyType.None;
			int num = -1;
			int credits = 0;
			int materials = 0;
			int contraband = 0;
			int crystals = 0;
			if (GameUtils.ParseCurrencyCostString(costString, out type, out num))
			{
				switch (type)
				{
				case CurrencyType.Credits:
					credits = num;
					break;
				case CurrencyType.Materials:
					materials = num;
					break;
				case CurrencyType.Contraband:
					contraband = num;
					break;
				case CurrencyType.Crystals:
					crystals = num;
					break;
				}
			}
			UXUtils.SetupSingleResourceUI(num, type, label, sprite, credits, materials, contraband, crystals);
		}

		public static void SetupSingleResourceUI(int amount, CurrencyType type, UXLabel label, UXSprite sprite, int credits, int materials, int contraband, int crystals)
		{
			label.Text = amount.ToString();
			UXUtils.SetupCurrencySprite(sprite, type);
			UXUtils.UpdateCostColor(label, null, credits, materials, contraband, crystals, 0, false);
		}

		public static void SetupSingleResourceUI(CostVO cost, UXLabel label, UXSprite sprite)
		{
			int amount = 0;
			CurrencyType type = CurrencyType.None;
			if (cost.Credits > 0)
			{
				type = CurrencyType.Credits;
				amount = cost.Credits;
			}
			if (cost.Materials > 0)
			{
				type = CurrencyType.Materials;
				amount = cost.Materials;
			}
			if (cost.Contraband > 0)
			{
				type = CurrencyType.Contraband;
				amount = cost.Contraband;
			}
			if (cost.Crystals > 0)
			{
				type = CurrencyType.Crystals;
				amount = cost.Crystals;
			}
			UXUtils.SetupSingleResourceUI(amount, type, label, sprite, cost.Credits, cost.Materials, cost.Contraband, cost.Crystals);
		}

		public static bool IsValidRewardItem(CrateFlyoutItemVO crateFlyoutItemVO, PlanetVO currentPlanetVO, int currentHqLevel)
		{
			if (crateFlyoutItemVO == null)
			{
				return false;
			}
			int minHQ = crateFlyoutItemVO.MinHQ;
			int maxHQ = crateFlyoutItemVO.MaxHQ;
			if ((minHQ > 0 && currentHqLevel < minHQ) || (maxHQ > 0 && currentHqLevel > maxHQ))
			{
				return false;
			}
			string[] planetIds = crateFlyoutItemVO.PlanetIds;
			if (currentPlanetVO != null && planetIds != null)
			{
				bool flag = false;
				int i = 0;
				int num = planetIds.Length;
				while (i < num)
				{
					if (planetIds[i] == currentPlanetVO.Uid)
					{
						flag = true;
						break;
					}
					i++;
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public static bool ShouldDisplayCrateFlyoutItem(CrateFlyoutItemVO flyoutVO, CrateFlyoutDisplayType displayType)
		{
			return flyoutVO != null && (flyoutVO.ShowParams == null || flyoutVO.ShowParams.Count < 1 || flyoutVO.ShowParams.Contains(displayType));
		}

		public static bool ShowCompensationMessage()
		{
			List<CrateData> compensationCrates = Service.CurrentPlayer.Prizes.GetCompensationCrates();
			if (compensationCrates.Count == 0)
			{
				return false;
			}
			uint pref = Service.SharedPlayerPrefs.GetPref<uint>("CompensationMsgLastViewTime");
			uint serverTime = Service.ServerAPI.ServerTime;
			if (serverTime - pref > 86400u)
			{
				AlertScreen alertScreen = AlertScreen.ShowModal(false, Service.Lang.Get("comp_popup_title", new object[0]), Service.Lang.Get("comp_popup_body", new object[0]), new OnScreenModalResult(UXUtils.OnGoToCrates), null);
				alertScreen.SetPrimaryLabelText(Service.Lang.Get("comp_popup_cta", new object[0]));
				Service.SharedPlayerPrefs.SetPref("CompensationMsgLastViewTime", serverTime.ToString());
				return true;
			}
			return false;
		}

		private static void OnGoToCrates(object result, object cookie)
		{
			if (result == null)
			{
				return;
			}
			bool flag = (bool)result;
			if (flag)
			{
				Service.HolonetController.CloseHolonet();
				ScreenBase screenBase = Service.UXController.HUD.CreatePrizeInventoryScreen();
				if (screenBase != null)
				{
					Service.ScreenController.AddScreen(screenBase);
				}
			}
		}
	}
}
