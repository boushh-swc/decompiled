using StaRTS.Main.Models;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class SquadWarInfoScreenRewardTab : AbstractSquadWarInfoScreenTab
	{
		private const string LABEL = "LabelReward";

		private const string TEXTURE = "TextureReward";

		public SquadWarInfoScreenRewardTab(SquadWarInfoScreen parent, UXCheckbox tabButton, UXElement topGroup) : base(parent, tabButton, topGroup)
		{
			FactionType faction = Service.CurrentPlayer.Faction;
			string text = (faction != FactionType.Rebel) ? GameConstants.WAR_HELP_REWARD_EMPIRE : GameConstants.WAR_HELP_REWARD_REBEL;
			string[] array = text.Split(new char[]
			{
				'|'
			});
			if (array.Length != 2)
			{
				Service.Logger.WarnFormat("GameConstant [War Help Reward {0}] is not formatted correctly: {1}", new object[]
				{
					faction,
					text
				});
				return;
			}
			base.PopulateBgTexture(array[0], "TextureReward");
			parent.GetElement<UXLabel>("LabelReward").Text = Service.Lang.Get(array[1], new object[0]);
		}
	}
}
