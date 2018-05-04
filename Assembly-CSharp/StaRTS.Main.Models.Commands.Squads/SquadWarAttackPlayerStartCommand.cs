using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class SquadWarAttackPlayerStartCommand : SquadGameActionCommand<SquadWarAttackPlayerStartRequest, BattleIdResponse>
	{
		public const string ACTION = "guild.war.attackPlayer.start";

		private const string WAR_ERROR_OPPONENT_IN_BATTLE = "WAR_ERROR_OPPONENT_IN_BATTLE";

		private const string WAR_ERROR_OPPONENT_HAS_NO_POINTS = "WAR_ERROR_OPPONENT_HAS_NO_POINTS";

		private const string WAR_ERROR_ENDED = "WAR_ERROR_ENDED";

		public SquadWarAttackPlayerStartCommand(SquadWarAttackPlayerStartRequest request) : base("guild.war.attackPlayer.start", request, new BattleIdResponse())
		{
		}

		public override OnCompleteAction OnFailure(uint status, object data)
		{
			if (SquadUtils.IsNotFatalServerError(status))
			{
				return base.EatFailure(status, data);
			}
			string text = null;
			if (status == 2414u)
			{
				text = "WAR_ERROR_ENDED";
				Service.SquadController.WarManager.EndSquadWar();
			}
			if (text != null)
			{
				OnScreenModalResult onModalResult = new OnScreenModalResult(Service.UXController.HUD.OnSquadWarAttackResultCallback);
				AlertScreen.ShowModal(false, null, Service.Lang.Get(text, new object[0]), onModalResult, null);
				return OnCompleteAction.Ok;
			}
			return OnCompleteAction.Desync;
		}
	}
}
