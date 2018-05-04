using StaRTS.Main.Models;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class SquadWarInfoScreenPreparationTab : AbstractSquadWarInfoScreenTab
	{
		private const string TEXTURE = "TexturePreparation";

		private const string GRID = "GridPreparation";

		private const string ITEM = "ItemPreparation";

		private const string ITEM_TEXTURE = "TextureItemPreparation";

		private const string ITEM_TITLE = "LabelTitlePreparation";

		private const string ITEM_BODY = "LabelBodyPreparation";

		public SquadWarInfoScreenPreparationTab(SquadWarInfoScreen parent, UXCheckbox tabButton, UXElement topGroup) : base(parent, tabButton, topGroup)
		{
			FactionType faction = Service.CurrentPlayer.Faction;
			string text = (faction != FactionType.Rebel) ? GameConstants.WAR_HELP_PREPARATION_EMPIRE : GameConstants.WAR_HELP_PREPARATION_REBEL;
			string[] array = text.Split(new char[]
			{
				'|'
			});
			if (array.Length < 2)
			{
				Service.Logger.WarnFormat("GameConstant [War Help Preparation {0}] is not formatted correctly: {1}", new object[]
				{
					faction,
					text
				});
				return;
			}
			base.PopulateBgTexture(array[0], "TexturePreparation");
			base.PopulateGrid(array, "GridPreparation", "ItemPreparation", "TextureItemPreparation", "LabelTitlePreparation", "LabelBodyPreparation");
		}
	}
}
