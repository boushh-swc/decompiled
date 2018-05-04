using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class SaleItemTypeVO : IValueObject
	{
		public static int COLUMN_title
		{
			get;
			private set;
		}

		public static int COLUMN_bonusMultiplier
		{
			get;
			private set;
		}

		public static int COLUMN_productId
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public double BonusMultiplier
		{
			get;
			set;
		}

		public string ProductId
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Title = row.TryGetString(SaleItemTypeVO.COLUMN_title);
			this.BonusMultiplier = (double)row.TryGetFloat(SaleItemTypeVO.COLUMN_bonusMultiplier, 1f);
			this.ProductId = row.TryGetString(SaleItemTypeVO.COLUMN_productId);
		}
	}
}
