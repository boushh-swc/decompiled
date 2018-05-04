using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.Squads;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class BattleReplayShareScreen : AbstractSquadRequestScreen
	{
		public const string INSTRUCTIONS_LABEL = "LabelTroopIncrement";

		public const string BATTLE_REPLAY_SHARE = "BATTLE_REPLAY_SHARE";

		public const string BATTLE_REPLAY_SHARE_DEFAULT = "BATTLE_REPLAY_SHARE_DEFAULT";

		private BattleEntry battleEntry;

		public BattleReplayShareScreen(BattleEntry battleEntry)
		{
			this.battleEntry = battleEntry;
		}

		protected override void OnScreenLoaded()
		{
			base.OnScreenLoaded();
			base.GetElement<UXLabel>("LabelTroopIncrement").Visible = false;
			this.requestLabel.Text = this.lang.Get("BATTLE_REPLAY_SHARE", new object[0]);
			this.requestLabel.Visible = true;
			this.input.InitText(this.lang.Get("BATTLE_REPLAY_SHARE_DEFAULT", new object[0]));
			UXElement element = base.GetElement<UXElement>("InputFieldRequestMessage");
			Vector3 position = element.Position;
			float cellHeight = base.GetElement<UXGrid>("GridCurrentTroops").CellHeight;
			position.y += cellHeight;
			element.Position = position;
		}

		protected override void OnClicked(UXButton btn)
		{
			if (!base.CheckForValidInput())
			{
				return;
			}
			string text = this.input.Text;
			if (string.IsNullOrEmpty(text))
			{
				text = this.lang.Get("BATTLE_REPLAY_SHARE_DEFAULT", new object[0]);
			}
			SquadMsg message = SquadMsgUtils.CreateShareReplayMessage(text, this.battleEntry);
			SquadController squadController = Service.SquadController;
			squadController.TakeAction(message);
			this.Close(null);
		}
	}
}
