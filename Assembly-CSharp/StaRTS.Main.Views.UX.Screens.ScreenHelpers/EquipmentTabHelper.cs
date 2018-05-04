using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers
{
	public class EquipmentTabHelper : AbstractTabHelper
	{
		private const string GRID_FILTER = "FilterGrid";

		private const string TEMPLATE_FILTER = "FilterTemplate";

		private const string PANEL_FILTER = "FilterPanel";

		private const string BUTTON_FILTER = "BtnFilter";

		private const string LABEL_FILTER_BUTTON = "LabelBtnFilter";

		public EquipmentTabHelper() : base("FilterPanel", "FilterGrid", "FilterTemplate", "BtnFilter", "LabelBtnFilter")
		{
		}

		public bool IsEquipmentValidForTab(EquipmentVO vo, EquipmentTab selectedTab)
		{
			if (selectedTab == EquipmentTab.All)
			{
				return true;
			}
			TroopUpgradeCatalog troopUpgradeCatalog = Service.TroopUpgradeCatalog;
			StaticDataController staticDataController = Service.StaticDataController;
			if (vo.EffectUids == null)
			{
				return false;
			}
			int i = 0;
			int num = vo.EffectUids.Length;
			while (i < num)
			{
				EquipmentEffectVO optional = staticDataController.GetOptional<EquipmentEffectVO>(vo.EffectUids[i]);
				if (optional == null)
				{
					Service.Logger.Error("CMS Error: EffectUids is empty for EquipmentEffectVO " + optional.Uid);
					return false;
				}
				if (selectedTab != EquipmentTab.Structures)
				{
					if (selectedTab != EquipmentTab.Troops)
					{
						if (selectedTab == EquipmentTab.Heroes)
						{
							if (optional.AffectedTroopIds != null && optional.AffectedTroopIds.Length > 0)
							{
								int j = 0;
								int num2 = optional.AffectedTroopIds.Length;
								while (j < num2)
								{
									if (troopUpgradeCatalog.GetMinLevel(optional.AffectedTroopIds[j]).Type == TroopType.Hero)
									{
										return true;
									}
									j++;
								}
							}
						}
					}
					else if (optional.AffectedTroopIds != null && optional.AffectedTroopIds.Length > 0)
					{
						int k = 0;
						int num3 = optional.AffectedTroopIds.Length;
						while (k < num3)
						{
							if (troopUpgradeCatalog.GetMinLevel(optional.AffectedTroopIds[k]).Type != TroopType.Hero)
							{
								return true;
							}
							k++;
						}
					}
				}
				else if (optional.AffectedBuildingIds != null && optional.AffectedBuildingIds.Length > 0)
				{
					return true;
				}
				i++;
			}
			return false;
		}
	}
}
