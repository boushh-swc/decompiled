using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public static class RelocationCostPerkUtils
	{
		public static int GetRelocationCostDiscount(List<string> perkEffectIds)
		{
			int num = 0;
			if (perkEffectIds != null)
			{
				StaticDataController staticDataController = Service.StaticDataController;
				int i = 0;
				int count = perkEffectIds.Count;
				while (i < count)
				{
					PerkEffectVO perkEffectVO = staticDataController.Get<PerkEffectVO>(perkEffectIds[i]);
					if (RelocationCostPerkUtils.CanApplyEffect(perkEffectVO))
					{
						num += perkEffectVO.RelocationDiscount;
					}
					i++;
				}
			}
			return num;
		}

		private static bool CanApplyEffect(PerkEffectVO perkEffectVO)
		{
			string type = perkEffectVO.Type;
			BuildingType perkBuilding = perkEffectVO.PerkBuilding;
			return type == "relocation" && perkBuilding == BuildingType.NavigationCenter;
		}
	}
}
