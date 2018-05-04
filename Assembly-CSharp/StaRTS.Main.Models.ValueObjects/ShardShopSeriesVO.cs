using StaRTS.Main.Controllers;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace StaRTS.Main.Models.ValueObjects
{
	public class ShardShopSeriesVO : IValueObject
	{
		private ShardShopPoolVO[] poolSlots;

		public static int COLUMN_uid
		{
			get;
			private set;
		}

		public static int COLUMN_startDate
		{
			get;
			private set;
		}

		public static int COLUMN_endDate
		{
			get;
			private set;
		}

		public static int COLUMN_pool_0
		{
			get;
			private set;
		}

		public static int COLUMN_pool_1
		{
			get;
			private set;
		}

		public static int COLUMN_pool_2
		{
			get;
			private set;
		}

		public static int COLUMN_pool_3
		{
			get;
			private set;
		}

		public static int COLUMN_pool_4
		{
			get;
			private set;
		}

		public static int COLUMN_infonetTitle
		{
			get;
			private set;
		}

		public static int COLUMN_shardShopDesc
		{
			get;
			private set;
		}

		public static int COLUMN_infonetTextureBG
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public DateTime StartDate
		{
			get;
			set;
		}

		public DateTime EndDate
		{
			get;
			set;
		}

		public string InfonetTitle
		{
			get;
			private set;
		}

		public string ShardShopDesc
		{
			get;
			private set;
		}

		public string ShardShopTextureBG
		{
			get;
			private set;
		}

		public string Pool0String
		{
			get;
			private set;
		}

		public string Pool1String
		{
			get;
			private set;
		}

		public string Pool2String
		{
			get;
			private set;
		}

		public string Pool3String
		{
			get;
			private set;
		}

		public string Pool4String
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			string text = row.TryGetString(ShardShopSeriesVO.COLUMN_startDate);
			if (!string.IsNullOrEmpty(text))
			{
				this.StartDate = DateTime.ParseExact(text, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
			}
			else
			{
				this.StartDate = DateTime.MinValue;
				Service.Logger.Warn("ShardShopSeries Start Date Not Specified");
			}
			string text2 = row.TryGetString(ShardShopSeriesVO.COLUMN_endDate);
			if (!string.IsNullOrEmpty(text2))
			{
				this.EndDate = DateTime.ParseExact(text2, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
			}
			else
			{
				this.EndDate = DateTime.MinValue;
				Service.Logger.Warn("ShardShopSeries End Date Not Specified");
			}
			this.Pool0String = row.TryGetString(ShardShopSeriesVO.COLUMN_pool_0, string.Empty);
			this.Pool1String = row.TryGetString(ShardShopSeriesVO.COLUMN_pool_1, string.Empty);
			this.Pool2String = row.TryGetString(ShardShopSeriesVO.COLUMN_pool_2, string.Empty);
			this.Pool3String = row.TryGetString(ShardShopSeriesVO.COLUMN_pool_3, string.Empty);
			this.Pool4String = row.TryGetString(ShardShopSeriesVO.COLUMN_pool_4, string.Empty);
			this.InfonetTitle = row.TryGetString(ShardShopSeriesVO.COLUMN_infonetTitle, string.Empty);
			this.ShardShopDesc = row.TryGetString(ShardShopSeriesVO.COLUMN_shardShopDesc, string.Empty);
			this.ShardShopTextureBG = row.TryGetString(ShardShopSeriesVO.COLUMN_infonetTextureBG, string.Empty);
		}

		public ShardShopPoolVO GetPool(int slotIndex)
		{
			if (this.poolSlots == null)
			{
				StaticDataController staticDataController = Service.StaticDataController;
				List<ShardShopPoolVO> list = new List<ShardShopPoolVO>();
				if (!string.IsNullOrEmpty(this.Pool0String))
				{
					list.Add(staticDataController.Get<ShardShopPoolVO>(this.Pool0String));
				}
				if (!string.IsNullOrEmpty(this.Pool1String))
				{
					list.Add(staticDataController.Get<ShardShopPoolVO>(this.Pool1String));
				}
				if (!string.IsNullOrEmpty(this.Pool2String))
				{
					list.Add(staticDataController.Get<ShardShopPoolVO>(this.Pool2String));
				}
				if (!string.IsNullOrEmpty(this.Pool3String))
				{
					list.Add(staticDataController.Get<ShardShopPoolVO>(this.Pool3String));
				}
				if (!string.IsNullOrEmpty(this.Pool4String))
				{
					list.Add(staticDataController.Get<ShardShopPoolVO>(this.Pool4String));
				}
				this.poolSlots = list.ToArray();
			}
			if (slotIndex >= this.poolSlots.Length)
			{
				return null;
			}
			return this.poolSlots[slotIndex];
		}
	}
}
