using StaRTS.Main.Controllers;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.ValueObjects
{
	public class BuffTypeVO : IAssetVO, IValueObject
	{
		private const string BUFF_DEFLECT_UID = "buffDeflect";

		private const string ASSET_OFFSET_TOP = "Top";

		public const int NUM_ASSET_SIZES = 7;

		public static int COLUMN_assetName
		{
			get;
			private set;
		}

		public static int COLUMN_bundleName
		{
			get;
			private set;
		}

		public static int COLUMN_assetProfile
		{
			get;
			private set;
		}

		public static int COLUMN_lvl
		{
			get;
			private set;
		}

		public static int COLUMN_modifier
		{
			get;
			private set;
		}

		public static int COLUMN_value
		{
			get;
			private set;
		}

		public static int COLUMN_wall
		{
			get;
			private set;
		}

		public static int COLUMN_building
		{
			get;
			private set;
		}

		public static int COLUMN_storage
		{
			get;
			private set;
		}

		public static int COLUMN_resource
		{
			get;
			private set;
		}

		public static int COLUMN_turret
		{
			get;
			private set;
		}

		public static int COLUMN_HQ
		{
			get;
			private set;
		}

		public static int COLUMN_shield
		{
			get;
			private set;
		}

		public static int COLUMN_shieldGenerator
		{
			get;
			private set;
		}

		public static int COLUMN_infantry
		{
			get;
			private set;
		}

		public static int COLUMN_bruiserInfantry
		{
			get;
			private set;
		}

		public static int COLUMN_vehicle
		{
			get;
			private set;
		}

		public static int COLUMN_bruiserVehicle
		{
			get;
			private set;
		}

		public static int COLUMN_heroInfantry
		{
			get;
			private set;
		}

		public static int COLUMN_heroVehicle
		{
			get;
			private set;
		}

		public static int COLUMN_heroBruiserInfantry
		{
			get;
			private set;
		}

		public static int COLUMN_heroBruiserVehicle
		{
			get;
			private set;
		}

		public static int COLUMN_flierInfantry
		{
			get;
			private set;
		}

		public static int COLUMN_flierVehicle
		{
			get;
			private set;
		}

		public static int COLUMN_healerInfantry
		{
			get;
			private set;
		}

		public static int COLUMN_trap
		{
			get;
			private set;
		}

		public static int COLUMN_champion
		{
			get;
			private set;
		}

		public static int COLUMN_applyValueAs
		{
			get;
			private set;
		}

		public static int COLUMN_duration
		{
			get;
			private set;
		}

		public static int COLUMN_stack
		{
			get;
			private set;
		}

		public static int COLUMN_msFirstProc
		{
			get;
			private set;
		}

		public static int COLUMN_msPerProc
		{
			get;
			private set;
		}

		public static int COLUMN_isRefreshing
		{
			get;
			private set;
		}

		public static int COLUMN_target
		{
			get;
			private set;
		}

		public static int COLUMN_tags
		{
			get;
			private set;
		}

		public static int COLUMN_cancelTags
		{
			get;
			private set;
		}

		public static int COLUMN_preventTags
		{
			get;
			private set;
		}

		public static int COLUMN_audioAbilityEvent
		{
			get;
			private set;
		}

		public static int COLUMN_shaderOverride
		{
			get;
			private set;
		}

		public static int COLUMN_projectileAttachmentBundle
		{
			get;
			private set;
		}

		public static int COLUMN_muzzleAssetNameRebel
		{
			get;
			private set;
		}

		public static int COLUMN_muzzleAssetNameEmpire
		{
			get;
			private set;
		}

		public static int COLUMN_impactAssetNameRebel
		{
			get;
			private set;
		}

		public static int COLUMN_impactAssetNameEmpire
		{
			get;
			private set;
		}

		public static int COLUMN_assetOffsetType
		{
			get;
			private set;
		}

		public static int COLUMN_details
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string AssetName
		{
			get;
			set;
		}

		public string BundleName
		{
			get;
			set;
		}

		public string ShaderName
		{
			get;
			set;
		}

		public BuffAssetOffset OffsetType
		{
			get;
			set;
		}

		public string RebelMuzzleAssetName
		{
			get;
			set;
		}

		public string RebelImpactAssetName
		{
			get;
			set;
		}

		public string EmpireMuzzleAssetName
		{
			get;
			set;
		}

		public string EmpireImpactAssetName
		{
			get;
			set;
		}

		public string ProjectileAttachmentBundle
		{
			get;
			set;
		}

		public string BuffID
		{
			get;
			private set;
		}

		public int Lvl
		{
			get;
			private set;
		}

		public BuffModify Modify
		{
			get;
			private set;
		}

		public int[] Values
		{
			get;
			private set;
		}

		public BuffApplyAs ApplyAs
		{
			get;
			private set;
		}

		public int Duration
		{
			get;
			private set;
		}

		public uint MaxStacks
		{
			get;
			private set;
		}

		public int MillisecondsToFirstProc
		{
			get;
			private set;
		}

		public int MillisecondsPerProc
		{
			get;
			private set;
		}

		public bool IsRefreshing
		{
			get;
			private set;
		}

		public bool ApplyToSelf
		{
			get;
			private set;
		}

		public bool ApplyToAllies
		{
			get;
			private set;
		}

		public bool ApplyToEnemies
		{
			get;
			private set;
		}

		public HashSet<string> Tags
		{
			get;
			private set;
		}

		public HashSet<string> CancelTags
		{
			get;
			private set;
		}

		public HashSet<string> PreventTags
		{
			get;
			private set;
		}

		public List<StrIntPair> AudioAbilityEvent
		{
			get;
			private set;
		}

		public bool IsDeflector
		{
			get;
			private set;
		}

		public string Details
		{
			get;
			private set;
		}

		public string AssetProfile
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.AssetName = row.TryGetString(BuffTypeVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(BuffTypeVO.COLUMN_bundleName);
			this.ShaderName = row.TryGetString(BuffTypeVO.COLUMN_shaderOverride, string.Empty);
			this.RebelMuzzleAssetName = row.TryGetString(BuffTypeVO.COLUMN_muzzleAssetNameRebel);
			this.RebelImpactAssetName = row.TryGetString(BuffTypeVO.COLUMN_impactAssetNameRebel);
			this.EmpireMuzzleAssetName = row.TryGetString(BuffTypeVO.COLUMN_muzzleAssetNameEmpire);
			this.EmpireImpactAssetName = row.TryGetString(BuffTypeVO.COLUMN_impactAssetNameEmpire);
			this.ProjectileAttachmentBundle = row.TryGetString(BuffTypeVO.COLUMN_projectileAttachmentBundle);
			this.BuffID = row.Uid;
			this.Lvl = row.TryGetInt(BuffTypeVO.COLUMN_lvl, 1);
			this.Modify = StringUtils.ParseEnum<BuffModify>(row.TryGetString(BuffTypeVO.COLUMN_modifier));
			int num = row.TryGetInt(BuffTypeVO.COLUMN_value);
			this.Values = new int[24];
			for (int i = 0; i < 24; i++)
			{
				this.Values[i] = num;
			}
			this.Values[1] = row.TryGetInt(BuffTypeVO.COLUMN_wall, num);
			this.Values[2] = row.TryGetInt(BuffTypeVO.COLUMN_building, num);
			this.Values[3] = row.TryGetInt(BuffTypeVO.COLUMN_storage, num);
			this.Values[4] = row.TryGetInt(BuffTypeVO.COLUMN_resource, num);
			this.Values[5] = row.TryGetInt(BuffTypeVO.COLUMN_turret, num);
			this.Values[6] = row.TryGetInt(BuffTypeVO.COLUMN_HQ, num);
			this.Values[7] = row.TryGetInt(BuffTypeVO.COLUMN_shield, num);
			this.Values[8] = row.TryGetInt(BuffTypeVO.COLUMN_shieldGenerator, num);
			this.Values[9] = row.TryGetInt(BuffTypeVO.COLUMN_infantry, num);
			this.Values[10] = row.TryGetInt(BuffTypeVO.COLUMN_bruiserInfantry, num);
			this.Values[11] = row.TryGetInt(BuffTypeVO.COLUMN_vehicle, num);
			this.Values[12] = row.TryGetInt(BuffTypeVO.COLUMN_bruiserVehicle, num);
			this.Values[13] = row.TryGetInt(BuffTypeVO.COLUMN_heroInfantry, num);
			this.Values[14] = row.TryGetInt(BuffTypeVO.COLUMN_heroVehicle, num);
			this.Values[15] = row.TryGetInt(BuffTypeVO.COLUMN_heroBruiserInfantry, num);
			this.Values[16] = row.TryGetInt(BuffTypeVO.COLUMN_heroBruiserVehicle, num);
			this.Values[17] = row.TryGetInt(BuffTypeVO.COLUMN_flierInfantry, num);
			this.Values[18] = row.TryGetInt(BuffTypeVO.COLUMN_flierVehicle, num);
			this.Values[19] = row.TryGetInt(BuffTypeVO.COLUMN_healerInfantry, num);
			this.Values[20] = row.TryGetInt(BuffTypeVO.COLUMN_trap, num);
			this.Values[21] = row.TryGetInt(BuffTypeVO.COLUMN_champion, num);
			this.ApplyAs = StringUtils.ParseEnum<BuffApplyAs>(row.TryGetString(BuffTypeVO.COLUMN_applyValueAs));
			this.Duration = row.TryGetInt(BuffTypeVO.COLUMN_duration, -1);
			if (this.Duration < -1)
			{
				this.Duration = -1;
			}
			int num2 = row.TryGetInt(BuffTypeVO.COLUMN_stack, 0);
			if (num2 < 0)
			{
				num2 = 0;
			}
			this.MaxStacks = (uint)num2;
			this.MillisecondsToFirstProc = row.TryGetInt(BuffTypeVO.COLUMN_msFirstProc);
			this.MillisecondsPerProc = row.TryGetInt(BuffTypeVO.COLUMN_msPerProc);
			if (this.MillisecondsPerProc == 0)
			{
				this.MillisecondsPerProc = -1;
			}
			this.IsRefreshing = row.TryGetBool(BuffTypeVO.COLUMN_isRefreshing);
			this.ApplyToSelf = false;
			this.ApplyToAllies = false;
			this.ApplyToEnemies = false;
			string[] commaSeparatedStrings = this.GetCommaSeparatedStrings(row, BuffTypeVO.COLUMN_target);
			if (commaSeparatedStrings != null)
			{
				int j = 0;
				int num3 = commaSeparatedStrings.Length;
				while (j < num3)
				{
					switch (StringUtils.ParseEnum<BuffApplyTo>(commaSeparatedStrings[j]))
					{
					case BuffApplyTo.Self:
						this.ApplyToSelf = true;
						break;
					case BuffApplyTo.Allies:
						this.ApplyToAllies = true;
						break;
					case BuffApplyTo.Enemies:
						this.ApplyToEnemies = true;
						break;
					}
					j++;
				}
			}
			this.Tags = this.GetCommaSeparatedHashSet(row, BuffTypeVO.COLUMN_tags);
			this.CancelTags = this.GetCommaSeparatedHashSet(row, BuffTypeVO.COLUMN_cancelTags);
			this.PreventTags = this.GetCommaSeparatedHashSet(row, BuffTypeVO.COLUMN_preventTags);
			ValueObjectController valueObjectController = Service.ValueObjectController;
			this.AudioAbilityEvent = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(BuffTypeVO.COLUMN_audioAbilityEvent));
			this.OffsetType = StringUtils.ParseEnum<BuffAssetOffset>(row.TryGetString(BuffTypeVO.COLUMN_assetOffsetType, "Top"));
			this.IsDeflector = (this.Uid == "buffDeflect");
			this.Details = row.TryGetString(BuffTypeVO.COLUMN_details);
			this.AssetProfile = row.TryGetString(BuffTypeVO.COLUMN_assetProfile);
		}

		private string[] GetCommaSeparatedStrings(Row row, int columnIndex)
		{
			string text = row.TryGetString(columnIndex);
			return (!string.IsNullOrEmpty(text)) ? text.Split(new char[]
			{
				','
			}) : null;
		}

		private HashSet<string> GetCommaSeparatedHashSet(Row row, int columnIndex)
		{
			string text = row.TryGetString(columnIndex);
			return (!string.IsNullOrEmpty(text)) ? new HashSet<string>(text.Split(new char[]
			{
				','
			})) : new HashSet<string>();
		}

		public string GetImpactAssetNameBasedOnFaction(FactionType faction)
		{
			if (faction == FactionType.Empire)
			{
				return this.EmpireImpactAssetName;
			}
			if (faction != FactionType.Rebel)
			{
				return null;
			}
			return this.RebelImpactAssetName;
		}

		public string GetMuzzleAssetNameBasedOnFaction(FactionType faction)
		{
			if (faction == FactionType.Empire)
			{
				return this.EmpireMuzzleAssetName;
			}
			if (faction != FactionType.Rebel)
			{
				return null;
			}
			return this.RebelMuzzleAssetName;
		}

		public bool WillAffect(ArmorType armorType)
		{
			if (this.Modify == BuffModify.Nothing)
			{
				return true;
			}
			int num = this.Values[(int)armorType];
			switch (this.ApplyAs)
			{
			case BuffApplyAs.Relative:
				return num != 0;
			case BuffApplyAs.Absolute:
				return true;
			case BuffApplyAs.RelativePercent:
			case BuffApplyAs.RelativePercentOfMax:
				return num != 0;
			case BuffApplyAs.AbsolutePercent:
			case BuffApplyAs.AbsolutePercentOfMax:
				return num != 100;
			default:
				return false;
			}
		}

		public bool AppliesOnlyToSelf()
		{
			return !this.ApplyToAllies && !this.ApplyToEnemies && this.ApplyToSelf;
		}
	}
}
