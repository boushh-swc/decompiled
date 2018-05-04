using StaRTS.Main.Models.Static;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class GameDataStartupTask : StartupTask
	{
		public GameDataStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			Service.EventManager.SendEvent(EventId.InitializeGameDataStart, null);
			new TroopUpgradeCatalog();
			new StarshipUpgradeCatalog();
			new BuildingUpgradeCatalog();
			new EquipmentUpgradeCatalog();
			new CombatEncounterController();
			new HoloController();
			new EpisodeController();
			new EpisodeTaskManager();
			base.Complete();
			Service.EventManager.SendEvent(EventId.InitializeGameDataEnd, null);
		}
	}
}
