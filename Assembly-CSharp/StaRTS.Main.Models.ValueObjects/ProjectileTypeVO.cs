using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.ValueObjects
{
	public class ProjectileTypeVO : IAssetVO, IValueObject
	{
		private const int BEAM_EXTEND_FACTOR = 4;

		private const int BEAM_MIN_BULLET_LENGTH = 2;

		private const int BEAM_MAX_BULLET_LENGTH = 6;

		public List<int> DamageMultipliers;

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

		public static int COLUMN_bullet
		{
			get;
			private set;
		}

		public static int COLUMN_groundBullet
		{
			get;
			private set;
		}

		public static int COLUMN_chargeAssetName
		{
			get;
			private set;
		}

		public static int COLUMN_muzzleFlash
		{
			get;
			private set;
		}

		public static int COLUMN_hitSpark
		{
			get;
			private set;
		}

		public static int COLUMN_directional
		{
			get;
			private set;
		}

		public static int COLUMN_muzzleFlashFadeTime
		{
			get;
			private set;
		}

		public static int COLUMN_arcs
		{
			get;
			private set;
		}

		public static int COLUMN_arcHeight
		{
			get;
			private set;
		}

		public static int COLUMN_spinSpeed
		{
			get;
			private set;
		}

		public static int COLUMN_arcRange
		{
			get;
			private set;
		}

		public static int COLUMN_maxSpeed
		{
			get;
			private set;
		}

		public static int COLUMN_s1Time
		{
			get;
			private set;
		}

		public static int COLUMN_sTransition
		{
			get;
			private set;
		}

		public static int COLUMN_s2Time
		{
			get;
			private set;
		}

		public static int COLUMN_seeksTarget
		{
			get;
			private set;
		}

		public static int COLUMN_isDeflectable
		{
			get;
			private set;
		}

		public static int COLUMN_passThroughShield
		{
			get;
			private set;
		}

		public static int COLUMN_splashDamagePercentages
		{
			get;
			private set;
		}

		public static int COLUMN_widthSegments
		{
			get;
			private set;
		}

		public static int COLUMN_lengthSegments
		{
			get;
			private set;
		}

		public static int COLUMN_projectileLength
		{
			get;
			private set;
		}

		public static int COLUMN_applyBuffs
		{
			get;
			private set;
		}

		public static int COLUMN_beamDamage
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string BulletAssetName
		{
			get;
			set;
		}

		public string GroundBulletAssetName
		{
			get;
			set;
		}

		public string ChargeAssetName
		{
			get;
			set;
		}

		public string MuzzleFlashAssetName
		{
			get;
			set;
		}

		public string HitSparkAssetName
		{
			get;
			set;
		}

		public string AssetName
		{
			get
			{
				return this.BulletAssetName;
			}
			set
			{
			}
		}

		public float MuzzleFlashFadeTime
		{
			get;
			private set;
		}

		public bool Directional
		{
			get;
			set;
		}

		public bool Arcs
		{
			get;
			set;
		}

		public int ArcHeight
		{
			get;
			set;
		}

		public int ArcRange
		{
			get;
			set;
		}

		public int MaxSpeed
		{
			get;
			set;
		}

		public float SpinSpeed
		{
			get;
			private set;
		}

		public uint Stage1Duration
		{
			get;
			set;
		}

		public uint StageTransitionDuration
		{
			get;
			set;
		}

		public uint Stage2Duration
		{
			get;
			set;
		}

		public bool SeeksTarget
		{
			get;
			set;
		}

		public bool IsDeflectable
		{
			get;
			set;
		}

		public bool PassThroughShield
		{
			get;
			set;
		}

		public int BeamDamage
		{
			get;
			set;
		}

		public int SplashRadius
		{
			get;
			set;
		}

		public int[] SplashDamagePercentages
		{
			get;
			set;
		}

		public int[] BeamWidthSegments
		{
			get;
			private set;
		}

		public int[] BeamLengthSegments
		{
			get;
			private set;
		}

		public bool IsBeam
		{
			get;
			private set;
		}

		public int BeamBulletLength
		{
			get;
			private set;
		}

		public int BeamDamageLength
		{
			get;
			private set;
		}

		public int BeamInitialZeroes
		{
			get;
			private set;
		}

		public int BeamLifeLength
		{
			get;
			private set;
		}

		public int BeamEmitterLength
		{
			get;
			private set;
		}

		public string[] ApplyBuffs
		{
			get;
			private set;
		}

		public bool IsMultiStage
		{
			get
			{
				return this.Stage1Duration > 0u && this.ArcRange > 0 && this.ArcHeight > 0;
			}
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			int num = 24;
			this.DamageMultipliers = new List<int>(num);
			for (int i = 0; i < num; i++)
			{
				this.DamageMultipliers.Add(-1);
			}
			this.DamageMultipliers[1] = row.TryGetInt(ProjectileTypeVO.COLUMN_wall);
			this.DamageMultipliers[2] = row.TryGetInt(ProjectileTypeVO.COLUMN_building);
			this.DamageMultipliers[3] = row.TryGetInt(ProjectileTypeVO.COLUMN_storage);
			this.DamageMultipliers[4] = row.TryGetInt(ProjectileTypeVO.COLUMN_resource);
			this.DamageMultipliers[5] = row.TryGetInt(ProjectileTypeVO.COLUMN_turret);
			this.DamageMultipliers[6] = row.TryGetInt(ProjectileTypeVO.COLUMN_HQ);
			this.DamageMultipliers[7] = row.TryGetInt(ProjectileTypeVO.COLUMN_shield);
			this.DamageMultipliers[8] = row.TryGetInt(ProjectileTypeVO.COLUMN_shieldGenerator);
			this.DamageMultipliers[9] = row.TryGetInt(ProjectileTypeVO.COLUMN_infantry);
			this.DamageMultipliers[10] = row.TryGetInt(ProjectileTypeVO.COLUMN_bruiserInfantry);
			this.DamageMultipliers[11] = row.TryGetInt(ProjectileTypeVO.COLUMN_vehicle);
			this.DamageMultipliers[12] = row.TryGetInt(ProjectileTypeVO.COLUMN_bruiserVehicle);
			this.DamageMultipliers[13] = row.TryGetInt(ProjectileTypeVO.COLUMN_heroInfantry);
			this.DamageMultipliers[14] = row.TryGetInt(ProjectileTypeVO.COLUMN_heroVehicle);
			this.DamageMultipliers[15] = row.TryGetInt(ProjectileTypeVO.COLUMN_heroBruiserInfantry);
			this.DamageMultipliers[16] = row.TryGetInt(ProjectileTypeVO.COLUMN_heroBruiserVehicle);
			this.DamageMultipliers[17] = row.TryGetInt(ProjectileTypeVO.COLUMN_flierInfantry);
			this.DamageMultipliers[18] = row.TryGetInt(ProjectileTypeVO.COLUMN_flierVehicle);
			this.DamageMultipliers[19] = row.TryGetInt(ProjectileTypeVO.COLUMN_healerInfantry);
			this.DamageMultipliers[20] = row.TryGetInt(ProjectileTypeVO.COLUMN_trap);
			this.DamageMultipliers[21] = row.TryGetInt(ProjectileTypeVO.COLUMN_champion);
			this.BulletAssetName = row.TryGetString(ProjectileTypeVO.COLUMN_bullet);
			this.GroundBulletAssetName = row.TryGetString(ProjectileTypeVO.COLUMN_groundBullet);
			this.ChargeAssetName = row.TryGetString(ProjectileTypeVO.COLUMN_chargeAssetName);
			this.MuzzleFlashAssetName = row.TryGetString(ProjectileTypeVO.COLUMN_muzzleFlash);
			this.HitSparkAssetName = row.TryGetString(ProjectileTypeVO.COLUMN_hitSpark);
			this.Directional = row.TryGetBool(ProjectileTypeVO.COLUMN_directional);
			this.MuzzleFlashFadeTime = row.TryGetFloat(ProjectileTypeVO.COLUMN_muzzleFlashFadeTime);
			this.Arcs = row.TryGetBool(ProjectileTypeVO.COLUMN_arcs);
			this.ArcHeight = row.TryGetInt(ProjectileTypeVO.COLUMN_arcHeight);
			this.ArcRange = row.TryGetInt(ProjectileTypeVO.COLUMN_arcRange);
			this.MaxSpeed = row.TryGetInt(ProjectileTypeVO.COLUMN_maxSpeed);
			this.SpinSpeed = row.TryGetFloat(ProjectileTypeVO.COLUMN_spinSpeed, 0f);
			this.Stage1Duration = row.TryGetUint(ProjectileTypeVO.COLUMN_s1Time);
			this.StageTransitionDuration = row.TryGetUint(ProjectileTypeVO.COLUMN_sTransition);
			this.Stage2Duration = row.TryGetUint(ProjectileTypeVO.COLUMN_s2Time);
			this.SeeksTarget = row.TryGetBool(ProjectileTypeVO.COLUMN_seeksTarget);
			this.IsDeflectable = row.TryGetBool(ProjectileTypeVO.COLUMN_isDeflectable);
			this.PassThroughShield = row.TryGetBool(ProjectileTypeVO.COLUMN_passThroughShield);
			this.BeamDamage = row.TryGetInt(ProjectileTypeVO.COLUMN_beamDamage);
			this.SplashDamagePercentages = row.TryGetIntArray(ProjectileTypeVO.COLUMN_splashDamagePercentages);
			if (this.HasSplashDamage())
			{
				this.SplashRadius = this.SplashDamagePercentages.Length - 1;
			}
			else
			{
				this.SplashRadius = 0;
			}
			this.BeamWidthSegments = row.TryGetIntArray(ProjectileTypeVO.COLUMN_widthSegments);
			this.BeamLengthSegments = row.TryGetIntArray(ProjectileTypeVO.COLUMN_lengthSegments);
			this.IsBeam = (this.BeamWidthSegments != null && this.BeamWidthSegments.Length != 0 && this.BeamLengthSegments != null && this.BeamLengthSegments.Length != 0);
			this.BeamBulletLength = row.TryGetInt(ProjectileTypeVO.COLUMN_projectileLength, 0);
			if (this.IsBeam)
			{
				int num2 = this.BeamLengthSegments.Length;
				this.BeamEmitterLength = num2;
				this.BeamLifeLength = num2 * 4;
				this.BeamDamageLength = 0;
				for (int j = num2 - 1; j >= 0; j--)
				{
					int num3 = this.BeamLengthSegments[j];
					this.BeamLengthSegments[j] = ((num3 >= 0) ? num3 : 0);
					if (this.BeamDamageLength == 0 && num3 > 0)
					{
						this.BeamDamageLength = j + 1;
					}
				}
				this.BeamInitialZeroes = 0;
				for (int k = 0; k < num2; k++)
				{
					if (this.BeamLengthSegments[k] != 0)
					{
						break;
					}
					this.BeamInitialZeroes++;
				}
				if (this.BeamBulletLength < 2)
				{
					this.BeamBulletLength = 2;
				}
				else if (this.BeamBulletLength > 6)
				{
					Service.Logger.WarnFormat("Beam projectileLength too large ({0}) for {1} (max is {2})", new object[]
					{
						this.BeamBulletLength,
						this.Uid,
						6
					});
					this.BeamBulletLength = 6;
				}
			}
			else
			{
				this.BeamWidthSegments = null;
				this.BeamLengthSegments = null;
				this.BeamDamageLength = 0;
				this.BeamInitialZeroes = 0;
				this.BeamLifeLength = 0;
				this.BeamEmitterLength = 0;
				this.BeamBulletLength = 0;
			}
			this.ApplyBuffs = null;
			string text = row.TryGetString(ProjectileTypeVO.COLUMN_applyBuffs);
			if (!string.IsNullOrEmpty(text))
			{
				this.ApplyBuffs = text.Split(new char[]
				{
					','
				});
			}
		}

		public bool HasSplashDamage()
		{
			return this.SplashDamagePercentages != null && this.SplashDamagePercentages.Length > 0;
		}

		public int GetSplashDamagePercent(int distFromImpact)
		{
			int result = 0;
			if (distFromImpact < this.SplashDamagePercentages.Length)
			{
				result = this.SplashDamagePercentages[distFromImpact];
			}
			return result;
		}

		public string GetBulletAssetName(bool isOnGround)
		{
			return (!isOnGround) ? this.BulletAssetName : this.GroundBulletAssetName;
		}
	}
}
