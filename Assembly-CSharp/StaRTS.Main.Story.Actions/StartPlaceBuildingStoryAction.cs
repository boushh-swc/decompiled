using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class StartPlaceBuildingStoryAction : AbstractStoryAction
	{
		private const int ARG_BUILDING_UID = 0;

		private string buildingUid;

		public StartPlaceBuildingStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(1);
			this.buildingUid = this.prepareArgs[0];
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			BuildingTypeVO buildingTypeVO = Service.StaticDataController.Get<BuildingTypeVO>(this.buildingUid);
			if (buildingTypeVO == null)
			{
				Service.Logger.WarnFormat("buildingUiD {0} does not exist", new object[]
				{
					this.buildingUid
				});
			}
			else
			{
				Service.BuildingController.PrepareAndPurchaseNewBuilding(buildingTypeVO);
				Entity selectedBuilding = Service.BuildingController.SelectedBuilding;
				Service.UserInputInhibitor.AllowOnly(selectedBuilding);
				Service.UXController.MiscElementsManager.EnableConfirmGroupAcceptButton(true);
			}
			this.parent.ChildComplete(this);
		}
	}
}
