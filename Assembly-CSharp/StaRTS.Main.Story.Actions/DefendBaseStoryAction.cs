using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class DefendBaseStoryAction : AbstractStoryAction
	{
		private const int DEFENSE_ENCOUNTER_UID_ARG = 0;

		private CampaignMissionVO encounterVO;

		public DefendBaseStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(1);
			StaticDataController staticDataController = Service.StaticDataController;
			this.encounterVO = staticDataController.Get<CampaignMissionVO>(this.prepareArgs[0]);
			BattleInitializationData data = BattleInitializationData.CreateFromDefensiveCampaignMissionVO(this.encounterVO.Uid);
			BattleStartState.GoToBattleStartState(data, null);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			Service.DefensiveBattleController.StartDefenseMission(this.encounterVO);
			this.parent.ChildComplete(this);
		}
	}
}
