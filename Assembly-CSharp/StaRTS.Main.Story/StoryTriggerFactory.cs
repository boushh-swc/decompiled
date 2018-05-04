using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Story.Trigger;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Story
{
	public class StoryTriggerFactory
	{
		private const string AUTO = "Auto";

		private const string BATTLE_COMPLETE = "BattleComplete";

		private const string CLUSTER_AND = "ClusterAND";

		private const string CLUSTER_ORDER = "ClusterORDER";

		private const string EVENT_COUNTER = "EventCounter";

		private const string FACTION_CHOICE_COMPLETE = "FactionChoiceComplete";

		private const string GALAXY_MAP_OPEN = "GalaxyMapOpen";

		private const string GAME_STATE = "GameState";

		private const string HQ_LEVEL = "HQLevel";

		private const string MISSION_VICTORY = "MissionVictory";

		private const string PLANET_RELOCATE = "PlanetRelocate";

		private const string REPAIR_ALL_COMPLETE = "RepairAllComplete";

		private const string REPAIR_BUILDING_COMPLETE = "RepairBuildingComplete";

		private const string SCREEN_APPEARS = "ScreenAppears";

		private const string SCOUT_PLANET = "ScoutPlanet";

		private const string TIMER = "Timer";

		private const string UNLOCK_PLANET = "UnlockPlanet";

		private const string WORLD_LOAD = "WorldLoad";

		private const string WORLD_TRANSITION_COMPLETE = "WorldTransitionComplete";

		private const string SQUAD_MEMBER = "SquadMember";

		private const string BLOCKING_WAR_CLICK = "BlockingInitWarClick";

		private const string SQUAD_SIZE = "SquadSize";

		private const string SQUAD_ACTIVE_MEMBERS = "squadActiveMembers";

		private const string SQUAD_UI_OPEN = "squadUIOpen";

		public static IStoryTrigger GenerateStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent)
		{
			string triggerType = vo.TriggerType;
			switch (triggerType)
			{
			case "Auto":
				return new AutoStoryTrigger(vo, parent);
			case "BattleComplete":
				return new BattleCompleteStoryTrigger(vo, parent);
			case "ClusterAND":
				return new ClusterANDStoryTrigger(vo, parent);
			case "ClusterORDER":
				return new ClusterORDERStoryTrigger(vo, parent);
			case "EventCounter":
				return new EventCounterStoryTrigger(vo, parent);
			case "FactionChoiceComplete":
				return new FactionChangedStoryTrigger(vo, parent);
			case "GalaxyMapOpen":
				return new GalaxyMapOpenStoryTrigger(vo, parent);
			case "GameState":
				return new GameStateStoryTrigger(vo, parent);
			case "HQLevel":
				return new HQLevelStoryTrigger(vo, parent);
			case "MissionVictory":
				return new MissionVictoryStoryTrigger(vo, parent);
			case "PlanetRelocate":
				return new PlanetRelocateStoryTrigger(vo, parent);
			case "RepairAllComplete":
			case "RepairBuildingComplete":
				return new BuildingRepairStoryTrigger(vo, parent);
			case "ScreenAppears":
				return new ScreenAppearStoryTrigger(vo, parent);
			case "ScoutPlanet":
				return new ScoutPlanetTrigger(vo, parent);
			case "Timer":
				return new TimerTrigger(vo, parent);
			case "UnlockPlanet":
				return new UnlockPlanetTrigger(vo, parent);
			case "WorldLoad":
				return new WorldLoadStoryTrigger(vo, parent);
			case "WorldTransitionComplete":
				return new WorldTransitionCompleteStoryTrigger(vo, parent);
			case "SquadMember":
				return new SquadMemberStoryTrigger(vo, parent);
			case "BlockingInitWarClick":
				return new BlockingWarStoryTrigger(vo, parent);
			case "SquadSize":
				return new SquadSizeStoryTrigger(vo, parent);
			case "squadActiveMembers":
				return new SquadActiveMembersStoryTrigger(vo, parent);
			case "squadUIOpen":
				return new SquadUIOpenStoryTrigger(vo, parent);
			}
			Service.Logger.ErrorFormat("There is no entry in the StoryTriggerFactory for {0}", new object[]
			{
				vo.TriggerType
			});
			return new AbstractStoryTrigger(vo, parent);
		}

		public static IStoryTrigger DeserializeStoryTrigger(object data, ITriggerReactor parent)
		{
			Dictionary<string, object> dictionary = data as Dictionary<string, object>;
			if (!dictionary.ContainsKey("uid"))
			{
				Service.Logger.Error("Quest Deserialization Error: Trigger Uid not found.");
				return null;
			}
			string uid = dictionary["uid"] as string;
			StoryTriggerVO vo = Service.StaticDataController.Get<StoryTriggerVO>(uid);
			AbstractStoryTrigger abstractStoryTrigger = StoryTriggerFactory.GenerateStoryTrigger(vo, parent) as AbstractStoryTrigger;
			return abstractStoryTrigger.FromObject(dictionary) as AbstractStoryTrigger;
		}
	}
}
