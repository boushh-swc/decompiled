using StaRTS.Main.Controllers;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class ShardShopPoolVO : IValueObject
	{
		private CostVO[] costs;

		public static int COLUMN_rebelCrateSupplyPool
		{
			get;
			private set;
		}

		public static int COLUMN_empireCrateSupplyPool
		{
			get;
			private set;
		}

		public static int COLUMN_costs
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string RebelCrateSupplyPool
		{
			get;
			private set;
		}

		public string EmpireCrateSupplyPool
		{
			get;
			private set;
		}

		public string[] CostStrings
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.RebelCrateSupplyPool = row.TryGetString(ShardShopPoolVO.COLUMN_rebelCrateSupplyPool);
			this.EmpireCrateSupplyPool = row.TryGetString(ShardShopPoolVO.COLUMN_empireCrateSupplyPool);
			this.CostStrings = row.TryGetStringArray(ShardShopPoolVO.COLUMN_costs);
		}

		public CostVO GetCost(int index)
		{
			if (this.costs == null)
			{
				this.PopulateCosts();
			}
			if (index >= this.costs.Length)
			{
				Service.Logger.ErrorFormat("Cannot determine cost index {0} of cost schedule for {1} (strings: {2})", new object[]
				{
					index,
					this.Uid,
					string.Join(",", this.CostStrings)
				});
				return null;
			}
			return this.costs[index];
		}

		public int GetTotalQuantity()
		{
			if (this.costs == null)
			{
				this.PopulateCosts();
			}
			return this.costs.Length;
		}

		private void PopulateCosts()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			this.costs = new CostVO[this.CostStrings.Length];
			for (int i = 0; i < this.costs.Length; i++)
			{
				this.costs[i] = CostUtils.CombineCurrenciesForShards(staticDataController.Get<CostVO>(this.CostStrings[i]));
			}
		}
	}
}
