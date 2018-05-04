using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.FX
{
	public class AnimationEventManager
	{
		private const string FACTORY_SPARK = "FactorySpark";

		private const string DEPOT_SPARK = "StarShipDepotSpark";

		private const string EXITED_HYPERSPACE = "ExitedHyperspace";

		private const string HALFWAY_HYPERSPACE = "HalfwayHyperspace";

		private const string PLAY_CRATE_ANIM_VFX = "PlayCrateVFX";

		private const string CRATE_ANIM_REWARD_SHOW_DONE = "RewardShowComplete";

		private const string PLAY_CRATE_ANIM_AND_REWARD_SHOW_DONE = "PlayCrateVFXAndRewardShowComplete";

		private const string CRATE_IDLE_HOP_HOOK = "PlayCrateHopSound";

		private Dictionary<string, KeyValuePair<string, int>> animationEventMap;

		public AnimationEventManager()
		{
			Service.AnimationEventManager = this;
			this.animationEventMap = new Dictionary<string, KeyValuePair<string, int>>();
		}

		public KeyValuePair<string, int> ParseMessage(string message)
		{
			KeyValuePair<string, int> keyValuePair;
			if (this.animationEventMap.TryGetValue(message, out keyValuePair))
			{
				return keyValuePair;
			}
			char[] separator = new char[]
			{
				':'
			};
			string[] array = message.Split(separator);
			if (array.Length == 2)
			{
				keyValuePair = new KeyValuePair<string, int>(array[0], int.Parse(array[1]));
			}
			else
			{
				keyValuePair = new KeyValuePair<string, int>(string.Empty, 0);
				Service.Logger.Error("Incorrect Animation-Event Message");
			}
			this.animationEventMap.Add(message, keyValuePair);
			return keyValuePair;
		}

		public void ProcessAnimationEvent(EntityRef entityRefMB, string message)
		{
			if (message == "ExitedHyperspace")
			{
				if (Service.PlanetRelocationController == null)
				{
					return;
				}
				Service.PlanetRelocationController.HyperspaceComplete();
				return;
			}
			else
			{
				if (message == "PlayCrateVFXAndRewardShowComplete")
				{
					Service.EventManager.SendEvent(EventId.PlayCrateAnimVfx, true);
					Service.EventManager.SendEvent(EventId.CrateRewardAnimDone, true);
					return;
				}
				if (message == "PlayCrateVFX")
				{
					Service.EventManager.SendEvent(EventId.PlayCrateAnimVfx, null);
					return;
				}
				if (message == "RewardShowComplete")
				{
					Service.EventManager.SendEvent(EventId.CrateRewardAnimDone, null);
					return;
				}
				if (message == "PlayCrateHopSound")
				{
					Service.EventManager.SendEvent(EventId.CrateRewardIdleHop, null);
					return;
				}
				if (entityRefMB == null)
				{
					return;
				}
				if (entityRefMB.Entity == null)
				{
					return;
				}
				GameObjectViewComponent gameObjectViewComp = ((SmartEntity)entityRefMB.Entity).GameObjectViewComp;
				if (gameObjectViewComp != null)
				{
					if (string.IsNullOrEmpty(message))
					{
						return;
					}
					KeyValuePair<string, int> keyValuePair = this.ParseMessage(message);
					if (string.IsNullOrEmpty(keyValuePair.Key))
					{
						return;
					}
					if (keyValuePair.Key == "FactorySpark")
					{
						if (Service.BuildingAnimationController == null)
						{
							return;
						}
						Service.BuildingAnimationController.FactorySpark(keyValuePair.Value, gameObjectViewComp);
					}
					else if (keyValuePair.Key == "StarShipDepotSpark")
					{
						if (Service.BuildingAnimationController == null)
						{
							return;
						}
						Service.BuildingAnimationController.DepotSpark(keyValuePair.Value, gameObjectViewComp);
					}
				}
				return;
			}
		}
	}
}
