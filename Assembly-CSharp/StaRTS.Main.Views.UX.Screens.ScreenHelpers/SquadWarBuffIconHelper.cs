using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers
{
	public class SquadWarBuffIconHelper
	{
		public static void SetupBuffIcons(AbstractUXList list, string templateName, string squadId)
		{
			list.SetTemplateItem(templateName);
			list.Clear();
			StaticDataController staticDataController = Service.StaticDataController;
			List<SquadWarBuffBaseData> buffBases = Service.SquadController.WarManager.CurrentSquadWar.BuffBases;
			int i = 0;
			int count = buffBases.Count;
			while (i < count)
			{
				SquadWarBuffBaseData squadWarBuffBaseData = buffBases[i];
				if (squadWarBuffBaseData.OwnerId == squadId)
				{
					SquadWarBuffIconHelper.AddBuffIcon(list, squadWarBuffBaseData.BuffBaseId, i, staticDataController);
				}
				i++;
			}
			list.RepositionItems();
		}

		public static void SetupBuffIcons(AbstractUXList list, string templateName, List<string> buffBases)
		{
			list.SetTemplateItem(templateName);
			list.Clear();
			if (buffBases != null)
			{
				StaticDataController staticDataController = Service.StaticDataController;
				int i = 0;
				int count = buffBases.Count;
				while (i < count)
				{
					SquadWarBuffIconHelper.AddBuffIcon(list, buffBases[i], i, staticDataController);
					i++;
				}
				list.RepositionItems();
			}
		}

		private static void AddBuffIcon(AbstractUXList list, string buffBaseId, int order, StaticDataController dc)
		{
			WarBuffVO warBuffVO = dc.Get<WarBuffVO>(buffBaseId);
			UXElement uXElement = list.CloneTemplateItem(buffBaseId);
			UXSprite uXSprite = uXElement as UXSprite;
			if (uXSprite != null)
			{
				uXSprite.SpriteName = warBuffVO.BuffIcon;
			}
			list.AddItem(uXElement, order);
		}
	}
}
