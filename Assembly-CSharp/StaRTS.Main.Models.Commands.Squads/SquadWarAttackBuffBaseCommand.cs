using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class SquadWarAttackBuffBaseCommand : SquadGameActionCommand<SquadWarAttackBuffBaseRequest, BattleIdResponse>
	{
		public const string ACTION = "guild.war.attackBase.start";

		private const string WAR_ERROR_BUFF_BASE_OWNED = "WAR_ERROR_BUFF_BASE_OWNED";

		private const string WAR_ERROR_BUFF_BASE_IN_BATTLE = "WAR_ERROR_BUFF_BASE_IN_BATTLE";

		private const string WAR_ERROR_OPPONENT_IN_BATTLE = "WAR_ERROR_OPPONENT_IN_BATTLE";

		private const string WAR_ERROR_ENDED = "WAR_ERROR_ENDED";

		public SquadWarAttackBuffBaseCommand(SquadWarAttackBuffBaseRequest request) : base("guild.war.attackBase.start", request, new BattleIdResponse())
		{
		}

		public override OnCompleteAction OnFailure(uint status, object data)
		{
			if (SquadUtils.IsNotFatalServerError(status))
			{
				return base.EatFailure(status, data);
			}
			string text = null;
			switch (status)
			{
			case 2402u:
				text = "WAR_ERROR_BUFF_BASE_OWNED";
				break;
			case 2403u:
				text = "WAR_ERROR_BUFF_BASE_IN_BATTLE";
				break;
			case 2404u:
				text = "WAR_ERROR_OPPONENT_IN_BATTLE";
				break;
			default:
				if (status == 2414u)
				{
					text = "WAR_ERROR_ENDED";
					Service.SquadController.WarManager.EndSquadWar();
				}
				break;
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
