using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Bot
{
	public class BNSquadCenterHasSpace : BotNotifier
	{
		public override bool EvaluateUpdate()
		{
			int storage = Service.BuildingLookupController.SquadBuildingNodeList.Head.BuildingComp.BuildingType.Storage;
			int donatedTroopStorageUsedByWorldOwner = SquadUtils.GetDonatedTroopStorageUsedByWorldOwner();
			Service.BotRunner.Log("Squad Center Capacity: {0} > {1} : {2}", new object[]
			{
				storage,
				donatedTroopStorageUsedByWorldOwner,
				storage >= donatedTroopStorageUsedByWorldOwner
			});
			return storage > donatedTroopStorageUsedByWorldOwner;
		}
	}
}
