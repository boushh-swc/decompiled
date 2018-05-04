using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard.Components;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace StaRTS.Main.Utils
{
	public static class FXUtils
	{
		public const float SELECTION_OUTLINE_WIDTH = 0.00125f;

		public const string ATTACHMENT_RUBBLE = "rubble";

		public const string ATTACHMENT_FILL_STATE = "fillState";

		private const string FX_DESTRUCT_DEFAULT_UID = "fx_debris_{0}x{1}";

		private const string FX_VEHICLE_DESTRUCTION_UID = "fx_vehdebris_{0}x{1}";

		private const string FX_WALL_DESTRUCTION_UID = "effect176";

		private const string FX_RUBBLE_MODEL_UID = "rebelRubble{0}";

		private const string FX_WALL_RUBBLE_MODEL_UID = "rebelRubbleWall1";

		private const string FX_SHIELD_DESTRUCTION_UID = "fx_debris_6x6";

		private const int FX_MAX_SIZE = 6;

		private const string KEY_DELIMITER = "|";

		public static readonly Color SELECTION_OUTLINE_COLOR = new Color(0.482f, 0.831f, 0.996f, 1f);

		public static string MakeAssetKey(string attachmentKey, Entity entity)
		{
			return attachmentKey + "|" + entity.ID;
		}

		public static string MakeAssetKey(string assetName, Vector3 worldPos)
		{
			return assetName + "|" + worldPos.ToString();
		}

		public static string GetRubbleAssetNameForEntity(SmartEntity entity)
		{
			SizeComponent sizeComp = entity.SizeComp;
			int num = Units.BoardToGridX(sizeComp.Width);
			int num2 = Units.BoardToGridZ(sizeComp.Depth);
			string uid;
			if (entity.BuildingComp.BuildingType.Type == BuildingType.Wall)
			{
				uid = string.Format("rebelRubbleWall1", num, num2);
			}
			else
			{
				uid = string.Format("rebelRubble{0}", num, num2);
			}
			BuildingTypeVO buildingTypeVO = Service.StaticDataController.Get<BuildingTypeVO>(uid);
			return buildingTypeVO.AssetName;
		}

		public static string GetDebrisAssetNameForEntity(SmartEntity entity, bool isBuilding)
		{
			SizeComponent sizeComp = entity.SizeComp;
			int val = Units.BoardToGridX(sizeComp.Width);
			int val2 = Units.BoardToGridZ(sizeComp.Depth);
			int num = Math.Min(val, val2);
			string uid;
			if (isBuilding)
			{
				BuildingTypeVO buildingType = entity.BuildingComp.BuildingType;
				string format = (!string.IsNullOrEmpty(buildingType.DestructFX)) ? buildingType.DestructFX : "fx_debris_{0}x{1}";
				BuildingType type = buildingType.Type;
				if (type != BuildingType.Wall)
				{
					if (type != BuildingType.Turret)
					{
						if (type != BuildingType.ShieldGenerator)
						{
							uid = string.Format(format, num, num);
						}
						else
						{
							uid = "fx_debris_6x6";
						}
					}
					else
					{
						uid = ((buildingType.Faction != FactionType.Tusken) ? string.Format(format, num, num) : "effect176");
					}
				}
				else
				{
					uid = "effect176";
				}
			}
			else
			{
				uid = string.Format("fx_vehdebris_{0}x{1}", num, num);
			}
			EffectsTypeVO effectsTypeVO = Service.StaticDataController.Get<EffectsTypeVO>(uid);
			return effectsTypeVO.AssetName;
		}

		public static List<IAssetVO> GetEffectAssetTypes()
		{
			List<IAssetVO> list = new List<IAssetVO>();
			list.Add(FXUtils.GetAssetType<BuildingTypeVO>("rebelRubbleWall1"));
			list.Add(FXUtils.GetAssetType<EffectsTypeVO>("effect176"));
			for (int i = 1; i < 6; i++)
			{
				string uid = string.Format("fx_debris_{0}x{1}", i, i);
				list.Add(FXUtils.GetAssetType<EffectsTypeVO>(uid));
				string uid2 = string.Format("rebelRubble{0}", i);
				list.Add(FXUtils.GetAssetType<BuildingTypeVO>(uid2));
			}
			return list;
		}

		private static IAssetVO GetAssetType<T>(string uid) where T : IValueObject
		{
			return Service.StaticDataController.Get<T>(uid) as IAssetVO;
		}

		public static Color ConvertHexStringToColorObject(string hexColor)
		{
			Color grey = Color.grey;
			grey.a = 1f;
			if (!string.IsNullOrEmpty(hexColor) && hexColor.Length == 6)
			{
				grey.r = (float)int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier) / 255f;
				grey.g = (float)int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier) / 255f;
				grey.b = (float)int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier) / 255f;
			}
			else
			{
				Service.Logger.Warn("FXUtils: Invalid hex color: " + hexColor);
			}
			return grey;
		}
	}
}
