using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public static class TroopRequestPerkUtils
	{
		public static int GetTroopRequestPerkTimeReduction(List<string> perkEffectIds)
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
					string type = perkEffectVO.Type;
					BuildingType perkBuilding = perkEffectVO.PerkBuilding;
					if (type == "troopRequestTime" && perkBuilding == BuildingType.Squad)
					{
						num += perkEffectVO.TroopRequestTimeDiscount;
					}
					i++;
				}
			}
			return num;
		}
	}
}
