using StaRTS.Main.Models;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class SquadWarInfoScreenWarBoardTab : AbstractSquadWarInfoScreenTab
	{
		private const string TEXTURE = "TextureWarBoard";

		private const string GRID = "GridWarBoard";

		private const string ITEM = "ItemWarBoard";

		private const string ITEM_TEXTURE = "TextureItemWarBoard";

		private const string ITEM_TITLE = "LabelTitleWarBoard";

		private const string ITEM_BODY = "LabelBodyWarBoard";

		public SquadWarInfoScreenWarBoardTab(SquadWarInfoScreen parent, UXCheckbox tabButton, UXElement topGroup) : base(parent, tabButton, topGroup)
		{
			FactionType faction = Service.CurrentPlayer.Faction;
			string text = (faction != FactionType.Rebel) ? GameConstants.WAR_HELP_BASEEDIT_EMPIRE : GameConstants.WAR_HELP_BASEEDIT_REBEL;
			string[] array = text.Split(new char[]
			{
				'|'
			});
			if (array.Length < 2)
			{
				Service.Logger.WarnFormat("GameConstant [War Help BaseEdit {0}] is not formatted correctly: {1}", new object[]
				{
					faction,
					text
				});
				return;
			}
			base.PopulateBgTexture(array[0], "TextureWarBoard");
			base.PopulateGrid(array, "GridWarBoard", "ItemWarBoard", "TextureItemWarBoard", "LabelTitleWarBoard", "LabelBodyWarBoard");
		}
	}
}
