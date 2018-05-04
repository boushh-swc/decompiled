using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Story.Trigger
{
	public class EventCounterStoryTrigger : AbstractStoryTrigger, IEventObserver
	{
		public const string REQUIRED_EVENTS_SYMBOL = "{REQ}";

		public const string COUNTED_EVENTS_SYMBOL = "{CNT}";

		public const string REMAINING_EVENTS_SYMBOL = "{REM}";

		private const string COUNTED_KEY = "cnt";

		private const int THRESHOLD_ARG = 0;

		private const int RELEVANT_EVENT_ARG = 1;

		private const int EVENT_DATA_ARG = 2;

		private const int UNINITIALIZED_COUNT = -1;

		private const string TROOP_DEPLOYED = "troopDeployed";

		private const string HERO_DEPLOYED = "heroDeployed";

		private const string CHAMPION_DEPLOYED = "championDeployed";

		private const string SPECIAL_ATTACK_DEPLOYED = "specialAttackDeployed";

		private const string BUILDING_KILLED = "buildingKilled";

		private const string TROOP_KILLED = "troopKilled";

		private const string BUTTON_CLICKED = "buttonClicked";

		private const string BUILDING_SELECTED = "buildingSelected";

		private const string UI_APPEARS = "uiAppears";

		private const string SCREEN_CLOSED = "screenClosed";

		private const string BUILDING_PLACED = "buildingPlaced";

		private const string CONTRACT_QUEUED = "contractQueued";

		private const string CONTRACT_STARTED = "contractStarted";

		private const string CONTRACT_COMPLETED = "contractCompleted";

		private const string INVENTORY_UPDATED = "inventoryUpdated";

		private const string DROID_PURCHASED = "droidPurchased";

		private const string TROOP_DONATION_TRACK_REWARDED = "donationTrackRewarded";

		private const string EQUIPMENT_UNLOCKED = "equipmentUnlocked";

		private int threshold;

		private int eventCount;

		private string[] eventData;

		public int RequiredEvents
		{
			get
			{
				return this.threshold;
			}
		}

		public int CountedEvents
		{
			get
			{
				return this.eventCount;
			}
		}

		public int RemainingEvents
		{
			get
			{
				return this.threshold - this.eventCount;
			}
		}

		public EventCounterStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
			this.eventCount = -1;
		}

		public override void Activate()
		{
			base.Activate();
			this.threshold = Convert.ToInt32(this.prepareArgs[0]);
			this.eventData = this.prepareArgs[2].Split(new char[]
			{
				','
			});
			if (this.eventCount == -1)
			{
				this.eventCount = 0;
			}
			EventId relevantEvent = this.GetRelevantEvent(this.prepareArgs[1]);
			Service.EventManager.RegisterObserver(this, relevantEvent, EventPriority.Default);
			base.UpdateAction();
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.ContractAdded:
			{
				ContractEventData contractEventData = (ContractEventData)cookie;
				if (contractEventData.Contract.ProductUid.Equals(this.eventData[0], StringComparison.InvariantCultureIgnoreCase))
				{
					this.CountEvent();
				}
				return EatResponse.NotEaten;
			}
			case EventId.ContractBacklogUpdated:
				IL_18:
				switch (id)
				{
				case EventId.ScreenClosing:
				{
					UXFactory uXFactory = cookie as UXFactory;
					string text = (uXFactory != null && !(uXFactory.Root == null)) ? uXFactory.Root.name : string.Empty;
					if (text.Equals(this.eventData[0], StringComparison.InvariantCultureIgnoreCase))
					{
						this.CountEvent();
					}
					return EatResponse.NotEaten;
				}
				case EventId.ScreenClosed:
				case EventId.ScreenOverlayClosing:
					IL_34:
					if (id != EventId.BuildingPurchaseSuccess)
					{
						if (id != EventId.BuildingSelected)
						{
							if (id != EventId.DroidPurchaseAnimationComplete)
							{
								if (id != EventId.EntityKilled)
								{
									if (id != EventId.TroopDeployed)
									{
										if (id == EventId.TroopDonationTrackRewardReceived)
										{
											goto IL_3C0;
										}
										if (id == EventId.SpecialAttackDeployed)
										{
											SpecialAttack specialAttack = (SpecialAttack)cookie;
											if (specialAttack.VO.Uid.Equals(this.eventData[0], StringComparison.InvariantCultureIgnoreCase))
											{
												this.CountEvent();
											}
											return EatResponse.NotEaten;
										}
										if (id == EventId.ButtonClicked)
										{
											string text2 = (string)cookie;
											if (text2.Equals(this.eventData[0], StringComparison.InvariantCultureIgnoreCase))
											{
												this.CountEvent();
											}
											return EatResponse.NotEaten;
										}
										if (id == EventId.ContractCompletedForStoryAction)
										{
											ContractTO contractTO = (ContractTO)cookie;
											if (contractTO.Uid.Equals(this.eventData[0], StringComparison.InvariantCultureIgnoreCase))
											{
												this.CountEvent();
											}
											return EatResponse.NotEaten;
										}
										if (id == EventId.InventoryResourceUpdated)
										{
											string text3 = (string)cookie;
											if (text3.Equals(this.eventData[0], StringComparison.InvariantCultureIgnoreCase))
											{
												this.CountEvent();
											}
											return EatResponse.NotEaten;
										}
										if (id != EventId.HeroDeployed && id != EventId.ChampionDeployed)
										{
											if (id != EventId.EquipmentUnlocked)
											{
												return EatResponse.NotEaten;
											}
											goto IL_3C0;
										}
									}
									Entity entity = (Entity)cookie;
									TroopComponent troopComponent = entity.Get<TroopComponent>();
									ITroopDeployableVO troopType = troopComponent.TroopType;
									if (troopType.Uid.Equals(this.eventData[0], StringComparison.InvariantCultureIgnoreCase))
									{
										this.CountEvent();
									}
									return EatResponse.NotEaten;
								}
								Entity entity2 = (Entity)cookie;
								string text4 = string.Empty;
								string a = this.prepareArgs[1];
								if (a == "troopKilled")
								{
									TroopComponent troopComponent2 = entity2.Get<TroopComponent>();
									if (troopComponent2 == null)
									{
										return EatResponse.NotEaten;
									}
									text4 = troopComponent2.TroopType.Uid;
								}
								else if (a == "buildingKilled")
								{
									BuildingComponent buildingComponent = entity2.Get<BuildingComponent>();
									if (buildingComponent == null)
									{
										return EatResponse.NotEaten;
									}
									BuildingTypeVO buildingType = buildingComponent.BuildingType;
									text4 = buildingType.Uid;
								}
								if (text4.Equals(this.eventData[0], StringComparison.InvariantCultureIgnoreCase))
								{
									this.CountEvent();
								}
								return EatResponse.NotEaten;
							}
							IL_3C0:
							this.CountEvent();
							return EatResponse.NotEaten;
						}
						Entity entity3 = (Entity)cookie;
						BuildingComponent buildingComponent2 = entity3.Get<BuildingComponent>();
						if (buildingComponent2 == null)
						{
							return EatResponse.NotEaten;
						}
						BuildingTypeVO buildingType2 = buildingComponent2.BuildingType;
						if (buildingType2.Uid.Equals(this.eventData[0], StringComparison.InvariantCultureIgnoreCase))
						{
							this.CountEvent();
						}
						return EatResponse.NotEaten;
					}
					else
					{
						Entity entity4 = (Entity)cookie;
						BuildingComponent buildingComponent3 = entity4.Get<BuildingComponent>();
						if (buildingComponent3 == null)
						{
							return EatResponse.NotEaten;
						}
						BuildingTypeVO buildingType3 = buildingComponent3.BuildingType;
						if (buildingType3.Uid.Equals(this.eventData[0], StringComparison.InvariantCultureIgnoreCase))
						{
							this.CountEvent();
						}
						return EatResponse.NotEaten;
					}
					break;
				case EventId.ScreenLoaded:
				{
					UXFactory uXFactory2 = cookie as UXFactory;
					string name = uXFactory2.Root.name;
					if (name.Equals(this.eventData[0], StringComparison.InvariantCultureIgnoreCase))
					{
						this.CountEvent();
					}
					return EatResponse.NotEaten;
				}
				}
				goto IL_34;
			case EventId.ContractStarted:
			{
				ContractEventData contractEventData2 = (ContractEventData)cookie;
				if (contractEventData2.Contract.ProductUid.Equals(this.eventData[0], StringComparison.InvariantCultureIgnoreCase))
				{
					this.CountEvent();
				}
				return EatResponse.NotEaten;
			}
			}
			goto IL_18;
		}

		private void CountEvent()
		{
			this.eventCount++;
			if (!this.EvaluateThreshold())
			{
				base.UpdateAction();
			}
		}

		private bool EvaluateThreshold()
		{
			if (this.eventCount >= this.threshold)
			{
				this.parent.SatisfyTrigger(this);
				return true;
			}
			return false;
		}

		private EventId GetRelevantEvent(string name)
		{
			switch (name)
			{
			case "heroDeployed":
				return EventId.HeroDeployed;
			case "championDeployed":
				return EventId.ChampionDeployed;
			case "specialAttackDeployed":
				return EventId.SpecialAttackDeployed;
			case "troopDeployed":
				return EventId.TroopDeployed;
			case "troopKilled":
			case "buildingKilled":
				return EventId.EntityKilled;
			case "buttonClicked":
				return EventId.ButtonClicked;
			case "buildingSelected":
				return EventId.BuildingSelected;
			case "uiAppears":
				return EventId.ScreenLoaded;
			case "buildingPlaced":
				return EventId.BuildingPurchaseSuccess;
			case "contractQueued":
				return EventId.ContractAdded;
			case "contractStarted":
				return EventId.ContractStarted;
			case "contractCompleted":
				return EventId.ContractCompletedForStoryAction;
			case "inventoryUpdated":
				return EventId.InventoryResourceUpdated;
			case "droidPurchased":
				return EventId.DroidPurchaseAnimationComplete;
			case "donationTrackRewarded":
				return EventId.TroopDonationTrackRewardReceived;
			case "screenClosed":
				return EventId.ScreenClosing;
			case "equipmentUnlocked":
				return EventId.EquipmentUnlocked;
			}
			Service.Logger.ErrorFormat("No event type associated with {0}", new object[]
			{
				name
			});
			return EventId.Nop;
		}

		public override void Destroy()
		{
			EventId relevantEvent = this.GetRelevantEvent(this.prepareArgs[1]);
			Service.EventManager.UnregisterObserver(this, relevantEvent);
			base.Destroy();
		}

		protected override void AddData(ref Serializer serializer)
		{
			base.AddData(ref serializer);
			serializer.Add<int>("cnt", this.eventCount);
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (!dictionary.ContainsKey("cnt"))
			{
				Service.Logger.ErrorFormat("Quest Deserialization Error: Could not find {0} property in trigger {1}", new object[]
				{
					"cnt",
					this.vo.Uid
				});
				return null;
			}
			this.eventCount = Convert.ToInt32(dictionary["cnt"]);
			return base.FromObject(obj);
		}
	}
}
