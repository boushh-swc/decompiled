using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Utils
{
	public class CostUtils
	{
		private const string COMBINE = "COMBINED_{0}_{1}";

		private const string CONVERT = "CONVERT_{0}";

		public static MissingCurrencyTypes CheckForMissingCurrency(CurrentPlayer player, CostVO cost)
		{
			if (cost == null)
			{
				return MissingCurrencyTypes.Invalid;
			}
			int num = 0;
			if (player.CurrentCrystalsAmount < cost.Crystals)
			{
				return MissingCurrencyTypes.Hard;
			}
			if (player.CurrentCreditsAmount < cost.Credits)
			{
				num++;
			}
			if (player.CurrentContrabandAmount < cost.Contraband)
			{
				num++;
			}
			if (player.CurrentMaterialsAmount < cost.Materials)
			{
				num++;
			}
			if (player.CurrentReputationAmount < cost.Reputation)
			{
				num++;
			}
			if (num == 1)
			{
				return MissingCurrencyTypes.Soft;
			}
			if (num > 1)
			{
				return MissingCurrencyTypes.MultipleSoft;
			}
			return MissingCurrencyTypes.None;
		}

		public static bool HasRequiredCurrency(CurrentPlayer player, CostVO cost)
		{
			return CostUtils.CheckForMissingCurrency(player, cost) == MissingCurrencyTypes.None;
		}

		public static bool DeductCost(CurrentPlayer player, CostVO cost)
		{
			if (!CostUtils.HasRequiredCurrency(player, cost))
			{
				return false;
			}
			if (cost.Credits != 0 || cost.Materials != 0 || cost.Contraband != 0 || cost.Reputation != 0 || cost.Crystals != 0)
			{
				GameUtils.SpendCurrency(cost.Credits, cost.Materials, cost.Contraband, cost.Reputation, cost.Crystals, true);
			}
			return true;
		}

		public static CostVO Combine(CostVO a, CostVO b)
		{
			return new CostVO
			{
				Uid = string.Format("COMBINED_{0}_{1}", a.Uid, b.Uid),
				Credits = a.Credits + b.Credits,
				Materials = a.Materials + b.Materials,
				Contraband = a.Contraband + b.Contraband,
				Reputation = a.Reputation + b.Reputation,
				Crystals = a.Crystals + b.Crystals
			};
		}

		public static CostVO CombineCurrenciesForShards(CostVO cost)
		{
			int numCurrencyTypes = CostUtils.GetNumCurrencyTypes(cost);
			CostVO result = cost;
			if (numCurrencyTypes > 1)
			{
				result = CostUtils.ConvertToCrystalsForShardShop(cost);
			}
			return result;
		}

		public static CostVO ConvertToCrystalsForShardShop(CostVO original)
		{
			if (original.Reputation > 0)
			{
				Service.Logger.ErrorFormat("Cannot convert REPUTATION cost to CRYSTAL cost: CostVO uid: {0}", new object[]
				{
					original.Uid
				});
				return original;
			}
			CostVO costVO = new CostVO();
			costVO.Uid = string.Format("CONVERT_{0}", original.Uid);
			costVO.Crystals = original.Crystals;
			costVO.Crystals += CostUtils.ConvertToCrystalsForShardShop(CurrencyType.Credits, original.Credits);
			costVO.Crystals += CostUtils.ConvertToCrystalsForShardShop(CurrencyType.Materials, original.Materials);
			costVO.Crystals += CostUtils.ConvertToCrystalsForShardShop(CurrencyType.Contraband, original.Contraband);
			return costVO;
		}

		public static int ConvertToCrystalsForShardShop(CurrencyType type, int amount)
		{
			if (amount == 0)
			{
				return 0;
			}
			int coefficient;
			int exponent;
			switch (type)
			{
			case CurrencyType.Credits:
				coefficient = GameConstants.SHARD_SHOP_CREDITS_COEFFICIENT;
				exponent = GameConstants.SHARD_SHOP_CREDITS_EXPONENT;
				break;
			case CurrencyType.Materials:
				coefficient = GameConstants.SHARD_SHOP_ALLOY_COEFFICIENT;
				exponent = GameConstants.SHARD_SHOP_ALLOY_EXPONENT;
				break;
			case CurrencyType.Contraband:
				coefficient = GameConstants.SHARD_SHOP_CONTRABAND_COEFFICIENT;
				exponent = GameConstants.SHARD_SHOP_CONTRABAND_EXPONENT;
				break;
			default:
				coefficient = 197290;
				exponent = 68900;
				break;
			}
			return GameUtils.CurrencyPow((float)amount, coefficient, exponent);
		}

		private static int GetNumCurrencyTypes(CostVO costVO)
		{
			int num = 0;
			if (costVO.Credits > 0)
			{
				num++;
			}
			if (costVO.Materials > 0)
			{
				num++;
			}
			if (costVO.Contraband > 0)
			{
				num++;
			}
			if (costVO.Reputation > 0)
			{
				num++;
			}
			if (costVO.Crystals > 0)
			{
				num++;
			}
			return num;
		}
	}
}
