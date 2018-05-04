using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;
using UnityEngine;

namespace StaRTS.Main.Models.ValueObjects
{
	public class SummonDetailsVO : IValueObject
	{
		public static int COLUMN_visitorUids
		{
			get;
			private set;
		}

		public static int COLUMN_visitorType
		{
			get;
			private set;
		}

		public static int COLUMN_maxProc
		{
			get;
			private set;
		}

		public static int COLUMN_sameTeam
		{
			get;
			private set;
		}

		public static int COLUMN_dieWithSummoner
		{
			get;
			private set;
		}

		public static int COLUMN_targetSummoner
		{
			get;
			private set;
		}

		public static int COLUMN_spawnPoints
		{
			get;
			private set;
		}

		public static int COLUMN_randomSpawnRadius
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string[] VisitorUids
		{
			get;
			private set;
		}

		public VisitorType VisitorType
		{
			get;
			private set;
		}

		public int MaxProc
		{
			get;
			private set;
		}

		public bool SameTeam
		{
			get;
			private set;
		}

		public bool DieWithSummoner
		{
			get;
			private set;
		}

		public bool TargetSummoner
		{
			get;
			private set;
		}

		public Vector3[] SpawnPoints
		{
			get;
			private set;
		}

		public int RandomSpawnRadius
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.VisitorUids = row.TryGetStringArray(SummonDetailsVO.COLUMN_visitorUids);
			this.VisitorType = StringUtils.ParseEnum<VisitorType>(row.TryGetString(SummonDetailsVO.COLUMN_visitorType));
			this.MaxProc = row.TryGetInt(SummonDetailsVO.COLUMN_maxProc);
			this.SameTeam = row.TryGetBool(SummonDetailsVO.COLUMN_sameTeam);
			this.DieWithSummoner = row.TryGetBool(SummonDetailsVO.COLUMN_dieWithSummoner);
			this.TargetSummoner = row.TryGetBool(SummonDetailsVO.COLUMN_targetSummoner);
			this.SpawnPoints = row.TryGetVector3Array(SummonDetailsVO.COLUMN_spawnPoints);
			this.RandomSpawnRadius = row.TryGetInt(SummonDetailsVO.COLUMN_randomSpawnRadius);
		}
	}
}
