using StaRTS.Main.Models;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers
{
	public class TroopTabHelper : AbstractTabHelper
	{
		private const string FILTER_PANEL = "FilterPanel";

		private const string FILTER_GRID = "FilterGrid";

		private const string FILTER_TEMPLATE = "FilterTemplate";

		private const string FILTER_BUTTON = "BtnFilter";

		private const string FILTER_BUTTON_LABEL = "LabelBtnFilter";

		public TroopTabHelper() : base("FilterPanel", "FilterGrid", "FilterTemplate", "BtnFilter", "LabelBtnFilter")
		{
		}

		public TroopTab ConvertTroopRoleToTab(TroopRole role)
		{
			TroopTab result = TroopTab.All;
			switch (role)
			{
			case TroopRole.Generic:
				result = TroopTab.Generic;
				break;
			case TroopRole.Striker:
				result = TroopTab.Striker;
				break;
			case TroopRole.Breacher:
				result = TroopTab.Breacher;
				break;
			case TroopRole.Looter:
				result = TroopTab.Looter;
				break;
			case TroopRole.Bruiser:
				result = TroopTab.Bruiser;
				break;
			case TroopRole.Healer:
				result = TroopTab.Healer;
				break;
			case TroopRole.Destroyer:
				result = TroopTab.Destroyer;
				break;
			default:
				Service.Logger.WarnFormat("Cannot convert TroopRole {0} to TroopTab", new object[]
				{
					role
				});
				break;
			}
			return result;
		}

		public TroopTab ConvertTroopTypeToTab(TroopType type)
		{
			TroopTab result = TroopTab.All;
			switch (type)
			{
			case TroopType.Infantry:
				result = TroopTab.Infantry;
				break;
			case TroopType.Vehicle:
				result = TroopTab.Vehicle;
				break;
			case TroopType.Mercenary:
				result = TroopTab.Mercenary;
				break;
			case TroopType.Hero:
				result = TroopTab.Hero;
				break;
			default:
				Service.Logger.WarnFormat("Cannot convert TroopType {0} to TroopTab", new object[]
				{
					type
				});
				break;
			}
			return result;
		}
	}
}
