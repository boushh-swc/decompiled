using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public static class ContractCostPerkUtils
	{
		public static float GetDiscountedCostMultiplier(BuildingTypeVO contractBuildingVO, List<string> perkEffectIds)
		{
			float num = 0f;
			if (perkEffectIds != null)
			{
				StaticDataController staticDataController = Service.StaticDataController;
				int i = 0;
				int count = perkEffectIds.Count;
				while (i < count)
				{
					PerkEffectVO perkEffectVO = staticDataController.Get<PerkEffectVO>(perkEffectIds[i]);
					if (ContractCostPerkUtils.CanApplyEffect(perkEffectVO, contractBuildingVO))
					{
						num += perkEffectVO.ContractDiscount;
					}
					i++;
				}
			}
			return 1f - num;
		}

		public static bool CanApplyEffect(PerkEffectVO perkEffectVO, BuildingTypeVO contractBuildingVO)
		{
			string type = perkEffectVO.Type;
			BuildingType perkBuilding = perkEffectVO.PerkBuilding;
			return contractBuildingVO != null && (type == "contractCost" && perkBuilding == contractBuildingVO.Type);
		}
	}
}
