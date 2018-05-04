using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public static class ContractTimePerkUtils
	{
		public static float GetTimeReductionMultiplier(BuildingTypeVO contractBuildingVO, List<string> perkEffectIds)
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
					if (ContractTimePerkUtils.CanApplyEffect(perkEffectVO, contractBuildingVO))
					{
						num += perkEffectVO.ContractTimeReduction;
					}
					i++;
				}
			}
			return 1f - num;
		}

		private static bool CanApplyEffect(PerkEffectVO perkEffectVO, BuildingTypeVO contractBuildingVO)
		{
			string type = perkEffectVO.Type;
			BuildingType perkBuilding = perkEffectVO.PerkBuilding;
			return contractBuildingVO != null && (type == "contractTime" && perkBuilding == contractBuildingVO.Type);
		}
	}
}
