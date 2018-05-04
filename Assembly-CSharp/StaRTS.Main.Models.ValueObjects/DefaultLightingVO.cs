using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class DefaultLightingVO : IValueObject
	{
		public static int COLUMN_description
		{
			get;
			private set;
		}

		public static int COLUMN_buildingColorDark
		{
			get;
			private set;
		}

		public static int COLUMN_buildingColorLight
		{
			get;
			private set;
		}

		public static int COLUMN_unitColor
		{
			get;
			private set;
		}

		public static int COLUMN_shadowColor
		{
			get;
			private set;
		}

		public static int COLUMN_groundColor
		{
			get;
			private set;
		}

		public static int COLUMN_groundColorLight
		{
			get;
			private set;
		}

		public static int COLUMN_gridColor
		{
			get;
			private set;
		}

		public static int COLUMN_buildingGridColor
		{
			get;
			private set;
		}

		public static int COLUMN_wallGridColor
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string LightingColorDark
		{
			get;
			set;
		}

		public string LightingColorLight
		{
			get;
			set;
		}

		public string LightingColorMedian
		{
			get;
			set;
		}

		public string LightingColorShadow
		{
			get;
			set;
		}

		public string LightingColorGround
		{
			get;
			set;
		}

		public string LightingColorGroundLight
		{
			get;
			set;
		}

		public string LightingColorGrid
		{
			get;
			set;
		}

		public string LightingColorBuildingGrid
		{
			get;
			set;
		}

		public string LightingColorWallGrid
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Description = row.TryGetString(DefaultLightingVO.COLUMN_description);
			this.LightingColorDark = row.TryGetHexValueString(DefaultLightingVO.COLUMN_buildingColorDark);
			this.LightingColorLight = row.TryGetHexValueString(DefaultLightingVO.COLUMN_buildingColorLight);
			this.LightingColorMedian = row.TryGetHexValueString(DefaultLightingVO.COLUMN_unitColor);
			this.LightingColorShadow = row.TryGetHexValueString(DefaultLightingVO.COLUMN_shadowColor);
			this.LightingColorGround = row.TryGetHexValueString(DefaultLightingVO.COLUMN_groundColor);
			this.LightingColorGroundLight = row.TryGetHexValueString(DefaultLightingVO.COLUMN_groundColorLight);
			this.LightingColorGrid = row.TryGetHexValueString(DefaultLightingVO.COLUMN_gridColor);
			this.LightingColorBuildingGrid = row.TryGetHexValueString(DefaultLightingVO.COLUMN_buildingGridColor);
			this.LightingColorWallGrid = row.TryGetHexValueString(DefaultLightingVO.COLUMN_wallGridColor);
		}
	}
}
