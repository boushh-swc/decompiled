using StaRTS.Main.Controllers;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.ValueObjects
{
	public class CivilianTypeVO : IAssetVO, IAudioVO, IHealthVO, ISpeedVO, IValueObject
	{
		public int Acceleration;

		public int Credits;

		public int Materials;

		public int Contraband;

		public int Xp;

		public int SizeX;

		public int SizeY;

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

		public static int COLUMN_faction
		{
			get;
			private set;
		}

		public static int COLUMN_credits
		{
			get;
			private set;
		}

		public static int COLUMN_materials
		{
			get;
			private set;
		}

		public static int COLUMN_contraband
		{
			get;
			private set;
		}

		public static int COLUMN_health
		{
			get;
			private set;
		}

		public static int COLUMN_maxSpeed
		{
			get;
			private set;
		}

		public static int COLUMN_newRotationSpeed
		{
			get;
			private set;
		}

		public static int COLUMN_size
		{
			get;
			private set;
		}

		public static int COLUMN_xp
		{
			get;
			private set;
		}

		public static int COLUMN_sizex
		{
			get;
			private set;
		}

		public static int COLUMN_sizey
		{
			get;
			private set;
		}

		public static int COLUMN_audioCharge
		{
			get;
			private set;
		}

		public static int COLUMN_audioAttack
		{
			get;
			private set;
		}

		public static int COLUMN_audioDeath
		{
			get;
			private set;
		}

		public static int COLUMN_audioPlacement
		{
			get;
			private set;
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

		public string Uid
		{
			get;
			set;
		}

		public FactionType Faction
		{
			get;
			set;
		}

		public int Health
		{
			get;
			set;
		}

		public int MaxSpeed
		{
			get;
			set;
		}

		public int RotationSpeed
		{
			get;
			set;
		}

		public int TrainingTime
		{
			get;
			set;
		}

		public int Size
		{
			get;
			set;
		}

		public List<StrIntPair> AudioPlacement
		{
			get;
			set;
		}

		public List<StrIntPair> AudioCharge
		{
			get;
			set;
		}

		public List<StrIntPair> AudioAttack
		{
			get;
			set;
		}

		public List<StrIntPair> AudioDeath
		{
			get;
			set;
		}

		public List<StrIntPair> AudioMovement
		{
			get;
			set;
		}

		public List<StrIntPair> AudioMovementAway
		{
			get;
			set;
		}

		public List<StrIntPair> AudioImpact
		{
			get;
			set;
		}

		public List<StrIntPair> AudioTrain
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.AssetName = row.TryGetString(CivilianTypeVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(CivilianTypeVO.COLUMN_bundleName);
			this.Uid = row.Uid;
			this.Faction = StringUtils.ParseEnum<FactionType>(row.TryGetString(CivilianTypeVO.COLUMN_faction));
			this.Credits = row.TryGetInt(CivilianTypeVO.COLUMN_credits);
			this.Materials = row.TryGetInt(CivilianTypeVO.COLUMN_materials);
			this.Contraband = row.TryGetInt(CivilianTypeVO.COLUMN_contraband);
			this.Health = row.TryGetInt(CivilianTypeVO.COLUMN_health);
			this.MaxSpeed = row.TryGetInt(CivilianTypeVO.COLUMN_maxSpeed);
			this.RotationSpeed = row.TryGetInt(CivilianTypeVO.COLUMN_newRotationSpeed);
			this.Size = row.TryGetInt(CivilianTypeVO.COLUMN_size);
			this.Xp = row.TryGetInt(CivilianTypeVO.COLUMN_xp);
			this.SizeX = row.TryGetInt(CivilianTypeVO.COLUMN_sizex);
			this.SizeY = row.TryGetInt(CivilianTypeVO.COLUMN_sizey);
			ValueObjectController valueObjectController = Service.ValueObjectController;
			this.AudioCharge = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(CivilianTypeVO.COLUMN_audioCharge));
			this.AudioAttack = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(CivilianTypeVO.COLUMN_audioAttack));
			this.AudioDeath = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(CivilianTypeVO.COLUMN_audioDeath));
			this.AudioPlacement = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(CivilianTypeVO.COLUMN_audioPlacement));
			if (this.RotationSpeed == 0)
			{
				Service.Logger.ErrorFormat("Missing rotation speed for civilianTypeVO {0}", new object[]
				{
					this.Uid
				});
			}
		}
	}
}
