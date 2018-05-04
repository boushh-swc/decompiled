using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class TransportTypeVO : IAssetVO, IValueObject
	{
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

		public static int COLUMN_transportID
		{
			get;
			private set;
		}

		public static int COLUMN_transportName
		{
			get;
			private set;
		}

		public static int COLUMN_capacity
		{
			get;
			private set;
		}

		public static int COLUMN_maxSpeed
		{
			get;
			private set;
		}

		public static int COLUMN_lvl
		{
			get;
			private set;
		}

		public static int COLUMN_type
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

		public string transportID
		{
			get;
			set;
		}

		public string transportName
		{
			get;
			set;
		}

		public int MaxSpeed
		{
			get;
			set;
		}

		public int Capacity
		{
			get;
			set;
		}

		public int Level
		{
			get;
			set;
		}

		public TransportType TransportType
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.AssetName = row.TryGetString(TransportTypeVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(TransportTypeVO.COLUMN_bundleName);
			this.Uid = row.Uid;
			this.Faction = StringUtils.ParseEnum<FactionType>(row.TryGetString(TransportTypeVO.COLUMN_faction));
			this.transportID = row.TryGetString(TransportTypeVO.COLUMN_transportID);
			this.transportName = row.TryGetString(TransportTypeVO.COLUMN_transportName);
			this.Capacity = row.TryGetInt(TransportTypeVO.COLUMN_capacity);
			this.MaxSpeed = row.TryGetInt(TransportTypeVO.COLUMN_maxSpeed);
			this.Level = row.TryGetInt(TransportTypeVO.COLUMN_lvl);
			this.TransportType = StringUtils.ParseEnum<TransportType>(row.TryGetString(TransportTypeVO.COLUMN_type));
			this.SizeX = row.TryGetInt(TransportTypeVO.COLUMN_sizex);
			this.SizeY = row.TryGetInt(TransportTypeVO.COLUMN_sizey);
		}
	}
}
