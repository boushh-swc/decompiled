using Net.RichardLord.Ash.Core;
using StaRTS.DataStructures;
using StaRTS.Externals.Manimal;
using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.GameBoard;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Planets;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Crates;
using StaRTS.Main.Models.Commands.Episodes;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Models.Commands.Player.Store;
using StaRTS.Main.Models.Commands.Pvp;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Leaderboard;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Main.Views.UX.Tags;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace StaRTS.Main.Utils
{
	public static class GameUtils
	{
		private const int INFINITE_TROOP_CAPACITY = 9999;

		private const uint FRAME_COUNT_OFFSET_FOR_NEXT_TARGETING = 30u;

		private const string PERK_VO_PREFIX = "perk_";

		[CompilerGenerated]
		private static Comparison<BuildingTypeVO> <>f__mg$cache0;

		[CompilerGenerated]
		private static OnScreenModalResult <>f__mg$cache1;

		[CompilerGenerated]
		private static WipeCompleteDelegate <>f__mg$cache2;

		[CompilerGenerated]
		private static AbstractCommand<BuyLimitedEditionItemRequest, CrateDataResponse>.OnSuccessCallback <>f__mg$cache3;

		[CompilerGenerated]
		private static AbstractCommand<BuyCrateRequest, CrateDataResponse>.OnSuccessCallback <>f__mg$cache4;

		[CompilerGenerated]
		private static AbstractCommand<OpenCrateRequest, OpenCrateResponse>.OnSuccessCallback <>f__mg$cache5;

		[CompilerGenerated]
		private static AbstractCommand<OpenCrateRequest, OpenCrateResponse>.OnFailureCallback <>f__mg$cache6;

		[CompilerGenerated]
		private static AbstractCommand<PlayerIdRequest, EpisodeTaskClaimResponse>.OnSuccessCallback <>f__mg$cache7;

		[CompilerGenerated]
		private static OnScreenModalResult <>f__mg$cache8;

		[CompilerGenerated]
		private static Comparison<Building> <>f__mg$cache9;

		[CompilerGenerated]
		private static Comparison<Building> <>f__mg$cacheA;

		public static GamePlayer GetWorldOwner()
		{
			if (GameUtils.IsVisitingNeighbor())
			{
				return Service.NeighborVisitManager.NeighborPlayer;
			}
			return Service.CurrentPlayer;
		}

		public static bool IsVisitingNeighbor()
		{
			return Service.GameStateMachine.CurrentState is NeighborVisitState;
		}

		public static bool IsVisitingBase()
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			return currentState is NeighborVisitState || (currentState is BattleStartState && Service.SquadController.WarManager.GetCurrentStatus() == SquadWarStatusType.PhasePrep);
		}

		public static Dictionary<string, int> ListToMap(string[] list)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			if (list != null)
			{
				int i = 0;
				int num = list.Length;
				while (i < num)
				{
					string item = list[i];
					string key;
					int value;
					if (GameUtils.ParseMapString(item, out key, out value))
					{
						dictionary[key] = value;
					}
					i++;
				}
			}
			return dictionary;
		}

		public static double GetDelayJitter(double capValue, double baseValue, int attempt)
		{
			double num = Math.Min(capValue, baseValue * Math.Pow(2.0, (double)attempt));
			return (double)UnityEngine.Random.Range(0f, (float)num);
		}

		public static int CalculatePlayerVictoryRating(GamePlayer player)
		{
			int result = 0;
			if (player != null)
			{
				result = GameUtils.CalculateVictoryRating(player.AttacksWon, player.DefensesWon);
			}
			return result;
		}

		public static int CalculatePvpTargetVictoryRating(PvpTarget target)
		{
			int result = 0;
			if (target != null)
			{
				result = GameUtils.CalculateVictoryRating(target.PlayerAttacksWon, target.PlayerDefensesWon);
			}
			return result;
		}

		public static int CalculateBattleHistoryVictoryRating(LeaderboardBattleHistory history)
		{
			int result = 0;
			if (history != null)
			{
				result = GameUtils.CalculateVictoryRating(history.AttacksWon, history.DefensesWon);
			}
			return result;
		}

		public static int CalculateVictoryRating(int attacksWon, int defensesWon)
		{
			return attacksWon + defensesWon;
		}

		public static bool ParseCurrencyCostString(string costString, out CurrencyType type, out int amount)
		{
			type = CurrencyType.None;
			amount = -1;
			if (string.IsNullOrEmpty(costString))
			{
				Service.Logger.Error("ParseCurrencyCostString failed becuase cost string was null or empty");
				return false;
			}
			string[] array = costString.Split(new char[]
			{
				':'
			});
			if (array.Length <= 1)
			{
				Service.Logger.Error("ParseCurrencyCostString failed becuase cost string was invalid: " + costString);
				return false;
			}
			type = StringUtils.ParseEnum<CurrencyType>(array[0]);
			amount = Convert.ToInt32(array[1]);
			return true;
		}

		private static bool ParseMapString(string item, out string key, out int val)
		{
			val = 0;
			key = null;
			if (item != null)
			{
				int num = item.IndexOf(':');
				if (num >= 0)
				{
					int.TryParse(StringUtils.Substring(item, num + 1), out val);
					key = StringUtils.Substring(item, 0, num);
					return true;
				}
			}
			return false;
		}

		public static void ListToAdditiveMap(string[] list, Dictionary<string, int> map)
		{
			if (list != null && map != null)
			{
				int i = 0;
				int num = list.Length;
				while (i < num)
				{
					string item = list[i];
					string key;
					int num2;
					if (GameUtils.ParseMapString(item, out key, out num2))
					{
						int num3 = 0;
						if (map.ContainsKey(key))
						{
							num3 = map[key];
						}
						map[key] = num3 + num2;
					}
					i++;
				}
			}
		}

		public static string GetTransmissionHoloId(FactionType faction, string planetUId)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (TransmissionCharacterVO current in staticDataController.GetAll<TransmissionCharacterVO>())
			{
				if (current.Faction == faction && current.PlanetId == planetUId)
				{
					return current.CharacterId;
				}
			}
			string result = string.Empty;
			switch (faction)
			{
			case FactionType.Empire:
				result = "kosh_1";
				return result;
			case FactionType.Rebel:
				result = "jennica_1";
				return result;
			case FactionType.Smuggler:
				result = string.Empty;
				return result;
			}
			Service.Logger.Error("Unknown Faction: " + faction.ToString() + " GameUtils::GetBattleLogHoloId");
			return result;
		}

		public static string GetServerTransmissionMessageImage(FactionType faction, string planetUId)
		{
			string result = string.Empty;
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (TransmissionCharacterVO current in staticDataController.GetAll<TransmissionCharacterVO>())
			{
				if (current.Faction == faction && current.PlanetId == planetUId)
				{
					result = current.Image;
					break;
				}
			}
			return result;
		}

		public static int SquaredDistance(int fromX, int fromZ, int toX, int toZ)
		{
			return (toX - fromX) * (toX - fromX) + (toZ - fromZ) * (toZ - fromZ);
		}

		public static int GetSquaredDistanceToTarget(ShooterComponent shooterComp, SmartEntity target)
		{
			SmartEntity smartEntity = (SmartEntity)shooterComp.Entity;
			if (target == null || smartEntity == null)
			{
				return 2147483647;
			}
			TransformComponent transformComp = smartEntity.TransformComp;
			if (transformComp == null)
			{
				return 2147483647;
			}
			TransformComponent transformComp2 = target.TransformComp;
			if (transformComp2 == null)
			{
				return 2147483647;
			}
			int num = transformComp.CenterGridX();
			int num2 = transformComp.CenterGridZ();
			int fromX;
			int fromZ;
			if (shooterComp.IsMelee)
			{
				fromX = GameUtils.NearestPointOnRect(num, transformComp2.MinX(), transformComp2.MaxX());
				fromZ = GameUtils.NearestPointOnRect(num2, transformComp2.MinZ(), transformComp2.MaxZ());
			}
			else
			{
				fromX = transformComp2.CenterGridX();
				fromZ = transformComp2.CenterGridZ();
			}
			return GameUtils.SquaredDistance(fromX, fromZ, num, num2);
		}

		public static int NearestPointOnRect(int k, int minK, int maxK)
		{
			if (k < minK)
			{
				return minK;
			}
			if (k > maxK)
			{
				return maxK;
			}
			return k;
		}

		public static bool IsBuildingUpgradable(BuildingTypeVO buildingInfo)
		{
			return buildingInfo.Lvl > 0 && buildingInfo.Lvl < Service.BuildingUpgradeCatalog.GetMaxLevel(buildingInfo.UpgradeGroup).Lvl;
		}

		public static int GetBuildingEffectiveLevel(SmartEntity building)
		{
			BuildingComponent buildingComp = building.BuildingComp;
			BuildingTypeVO buildingType = buildingComp.BuildingType;
			if (ContractUtils.IsBuildingConstructing(building))
			{
				return 0;
			}
			return buildingType.Lvl;
		}

		public static bool IsBuildingTypeValidForBattleConditions(BuildingType type)
		{
			bool flag = type == BuildingType.Wall;
			bool flag2 = type == BuildingType.Rubble;
			bool flag3 = type == BuildingType.Blocker;
			bool flag4 = type == BuildingType.Clearable;
			return !flag && !flag2 && !flag3 && !flag4;
		}

		public static bool IsBuildingMovable(Entity building)
		{
			if (building == null)
			{
				return false;
			}
			BuildingType type = building.Get<BuildingComponent>().BuildingType.Type;
			return type != BuildingType.Clearable;
		}

		public static string GetEquivalentSlow(BuildingTypeVO currentType, FactionType faction)
		{
			if (currentType.Faction == faction)
			{
				return currentType.Uid;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			List<BuildingTypeVO> list = new List<BuildingTypeVO>();
			foreach (BuildingTypeVO current in staticDataController.GetAll<BuildingTypeVO>())
			{
				list.Add(current);
			}
			List<BuildingTypeVO> arg_7C_0 = list;
			if (GameUtils.<>f__mg$cache0 == null)
			{
				GameUtils.<>f__mg$cache0 = new Comparison<BuildingTypeVO>(GameUtils.SortBuildingByUID);
			}
			arg_7C_0.Sort(GameUtils.<>f__mg$cache0);
			return GameUtils.GetEquivalentFromPreSortedList(list, currentType, faction);
		}

		public static string GetEquivalentFromPreSortedList(List<BuildingTypeVO> sortedBuildings, BuildingTypeVO currentType, FactionType faction)
		{
			int count = sortedBuildings.Count;
			for (int i = 0; i < count; i++)
			{
				BuildingTypeVO buildingTypeVO = sortedBuildings[i];
				if (buildingTypeVO.Faction == faction && buildingTypeVO.Lvl == currentType.Lvl && buildingTypeVO.Type == currentType.Type && buildingTypeVO.SubType == currentType.SubType && ((buildingTypeVO.Type != BuildingType.Resource && buildingTypeVO.Type != BuildingType.Storage && buildingTypeVO.Type != BuildingType.Turret) || (buildingTypeVO.Type == BuildingType.Resource && buildingTypeVO.Currency == currentType.Currency) || (buildingTypeVO.Type == BuildingType.Storage && buildingTypeVO.Currency == currentType.Currency) || (buildingTypeVO.Type == BuildingType.Turret && buildingTypeVO.SubType == currentType.SubType)))
				{
					return buildingTypeVO.Uid;
				}
			}
			Service.Logger.WarnFormat("No equivalent building for {0} in faction {1}", new object[]
			{
				currentType.Uid,
				faction
			});
			return currentType.Uid;
		}

		public static int SortBuildingByUID(BuildingTypeVO bldg1, BuildingTypeVO bldg2)
		{
			return bldg2.Uid.CompareTo(bldg1.Uid);
		}

		public static int GetWorldOwnerTroopCount(string uid)
		{
			GamePlayer worldOwner = GameUtils.GetWorldOwner();
			return GameUtils.GetDeployableCount(uid, worldOwner.Inventory.Troop);
		}

		public static int GetWorldOwnerSpecialAttackCount(string uid)
		{
			GamePlayer worldOwner = GameUtils.GetWorldOwner();
			return GameUtils.GetDeployableCount(uid, worldOwner.Inventory.SpecialAttack);
		}

		public static int GetWorldOwnerHeroCount(string uid)
		{
			GamePlayer worldOwner = GameUtils.GetWorldOwner();
			return GameUtils.GetDeployableCount(uid, worldOwner.Inventory.Hero);
		}

		public static int GetWorldOwnerChampionCount(string uid)
		{
			GamePlayer worldOwner = GameUtils.GetWorldOwner();
			return GameUtils.GetDeployableCount(uid, worldOwner.Inventory.Champion);
		}

		public static int GetDeployableCountForUpgradeGroupSpecialAttack(SpecialAttackTypeVO specialAttack)
		{
			int num = 0;
			List<SpecialAttackTypeVO> upgradeGroupLevels = Service.StarshipUpgradeCatalog.GetUpgradeGroupLevels(specialAttack.UpgradeGroup);
			InventoryStorage specialAttack2 = Service.CurrentPlayer.Inventory.SpecialAttack;
			for (int i = 0; i < upgradeGroupLevels.Count; i++)
			{
				num += GameUtils.GetDeployableCount(upgradeGroupLevels[i].Uid, specialAttack2);
			}
			return num;
		}

		public static int GetDeployableCountForUpgradeGroupTroop(TroopTypeVO troop)
		{
			int num = 0;
			List<TroopTypeVO> upgradeGroupLevels = Service.TroopUpgradeCatalog.GetUpgradeGroupLevels(troop.TroopID);
			TroopType type = troop.Type;
			InventoryStorage storage;
			if (type != TroopType.Hero)
			{
				if (type != TroopType.Champion)
				{
					storage = Service.CurrentPlayer.Inventory.Troop;
				}
				else
				{
					storage = Service.CurrentPlayer.Inventory.Champion;
				}
			}
			else
			{
				storage = Service.CurrentPlayer.Inventory.Hero;
			}
			for (int i = 0; i < upgradeGroupLevels.Count; i++)
			{
				num += GameUtils.GetDeployableCount(upgradeGroupLevels[i].Uid, storage);
			}
			return num;
		}

		public static int GetDeployableCount(string uid, InventoryStorage storage)
		{
			return storage.GetItemAmount(uid);
		}

		public static void GetStarportTroopCounts(out int housingSpace, out int housingSpaceTotal)
		{
			GamePlayer worldOwner = GameUtils.GetWorldOwner();
			housingSpace = worldOwner.Inventory.Troop.GetTotalStorageAmount();
			housingSpaceTotal = worldOwner.Inventory.Troop.GetTotalStorageCapacity();
			if (housingSpaceTotal == -1)
			{
				housingSpaceTotal = 9999;
			}
		}

		public static void LogComponentsAsError(string message, Entity entity)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<ComponentBase> all = entity.GetAll();
			bool flag = false;
			int i = 0;
			int count = all.Count;
			while (i < count)
			{
				ComponentBase componentBase = all[i];
				bool flag2 = componentBase is AssetComponent;
				if (!flag2 || !flag)
				{
					stringBuilder.Append(componentBase.GetType().Name);
					if (flag2)
					{
						flag = true;
						stringBuilder.Append('=');
						stringBuilder.Append(((AssetComponent)componentBase).AssetName);
					}
					if (i < count - 1)
					{
						stringBuilder.Append(',');
					}
				}
				i++;
			}
			Service.Logger.ErrorFormat("{0} ({1}): {2}", new object[]
			{
				message,
				entity.ID,
				stringBuilder
			});
		}

		public static bool RectsIntersect(int left1, int right1, int top1, int bottom1, int left2, int right2, int top2, int bottom2)
		{
			return left1 < right2 && right1 > left2 && top1 < bottom2 && bottom1 > top2;
		}

		public static bool RectContainsRect(int left1, int right1, int top1, int bottom1, int left2, int right2, int top2, int bottom2)
		{
			return left2 >= left1 && right2 <= right1 && top2 >= top1 && bottom2 <= bottom1;
		}

		public static int CalcuateMedals(int AttackRating, int DefenseRating)
		{
			return AttackRating + DefenseRating;
		}

		public static string GetTimeLabelFromSeconds(int totalSeconds)
		{
			Lang lang = Service.Lang;
			int num = totalSeconds / 60;
			int num2 = totalSeconds - num * 60;
			int num3 = num / 60;
			num -= num3 * 60;
			int num4 = num3 / 24;
			num3 -= num4 * 24;
			int num5 = num4 / 7;
			num4 -= num5 * 7;
			if (num5 > 0)
			{
				return lang.Get("WEEKS_DAYS", new object[]
				{
					num5,
					num4
				});
			}
			if (num4 > 0)
			{
				return lang.Get("DAYS_HOURS", new object[]
				{
					num4,
					num3
				});
			}
			if (num3 > 0)
			{
				return lang.Get("HOURS_MINUTES", new object[]
				{
					num3,
					num
				});
			}
			if (num > 0)
			{
				return lang.Get("MINUTES_SECONDS", new object[]
				{
					num,
					num2
				});
			}
			return lang.Get("SECONDS", new object[]
			{
				num2
			});
		}

		public static CurrencyType GetCurrencyType(int credits, int materials, int contraband)
		{
			if (credits != 0)
			{
				return CurrencyType.Credits;
			}
			if (materials != 0)
			{
				return CurrencyType.Materials;
			}
			if (contraband != 0)
			{
				return CurrencyType.Contraband;
			}
			return CurrencyType.None;
		}

		public static bool CanAffordCrystals(int crystals)
		{
			return crystals <= Service.CurrentPlayer.CurrentCrystalsAmount;
		}

		public static bool CanAffordCredits(int credits)
		{
			return credits <= Service.CurrentPlayer.CurrentCreditsAmount;
		}

		public static bool CanAffordMaterials(int materials)
		{
			return materials <= Service.CurrentPlayer.CurrentMaterialsAmount;
		}

		public static bool CanAffordContraband(int contraband)
		{
			return contraband <= Service.CurrentPlayer.CurrentContrabandAmount;
		}

		public static bool CanAffordReputation(int reputation)
		{
			return reputation <= Service.CurrentPlayer.CurrentReputationAmount;
		}

		public static bool CanAffordCosts(int credits, int materials, int contraband, int crystals)
		{
			return GameUtils.CanAffordCrystals(crystals) && GameUtils.CanAffordCredits(credits) && GameUtils.CanAffordMaterials(materials) && GameUtils.CanAffordContraband(contraband);
		}

		public static void SpendCurrency(int credits, int materials, int contraband, int reputation, int crystals, bool playSound)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			EventManager eventManager = Service.EventManager;
			if (credits > 0)
			{
				currentPlayer.Inventory.ModifyCredits(-credits);
				if (playSound)
				{
					eventManager.SendEvent(EventId.AudibleCurrencySpent, CurrencyType.Credits);
				}
			}
			if (materials > 0)
			{
				currentPlayer.Inventory.ModifyMaterials(-materials);
				if (playSound)
				{
					eventManager.SendEvent(EventId.AudibleCurrencySpent, CurrencyType.Materials);
				}
			}
			if (contraband > 0)
			{
				currentPlayer.Inventory.ModifyContraband(-contraband);
				if (playSound)
				{
					eventManager.SendEvent(EventId.AudibleCurrencySpent, CurrencyType.Contraband);
				}
			}
			if (reputation > 0)
			{
				currentPlayer.Inventory.ModifyReputation(-reputation);
				if (playSound)
				{
					eventManager.SendEvent(EventId.AudibleCurrencySpent, CurrencyType.Reputation);
				}
			}
			if (crystals > 0)
			{
				currentPlayer.Inventory.ModifyCrystals(-crystals);
				if (playSound)
				{
					eventManager.SendEvent(EventId.AudibleCurrencySpent, CurrencyType.Crystals);
				}
			}
		}

		public static void SpendCurrencyWithMultiplier(int credits, int materials, int contraband, float multiplier, bool playSound)
		{
			GameUtils.MultiplyCurrency(multiplier, ref credits, ref materials, ref contraband);
			GameUtils.SpendCurrency(credits, materials, contraband, 0, 0, playSound);
		}

		public static void SpendCurrency(int credits, int materials, int contraband, bool playSound)
		{
			GameUtils.SpendCurrency(credits, materials, contraband, 0, 0, playSound);
		}

		public static void SpendHQScaledCurrency(string[] cost, bool playSound)
		{
			int credits;
			int materials;
			int contraband;
			int reputation;
			GameUtils.GetHQScaledCurrency(cost, out credits, out materials, out contraband, out reputation);
			GameUtils.SpendCurrency(credits, materials, contraband, reputation, 0, playSound);
		}

		public static void SpendCurrency(string[] cost, bool playSound)
		{
			int credits;
			int materials;
			int contraband;
			int reputation;
			int crystals;
			GameUtils.GetCurrencyCost(cost, out credits, out materials, out contraband, out reputation, out crystals);
			GameUtils.SpendCurrency(credits, materials, contraband, reputation, crystals, playSound);
		}

		public static void GetHQScaledCurrency(string[] cost, out int credits, out int materials, out int contraband, out int reputation)
		{
			int num = 0;
			credits = 0;
			materials = 0;
			contraband = 0;
			reputation = 0;
			Dictionary<string, int> hQScaledCostForPlayer = GameUtils.GetHQScaledCostForPlayer(cost);
			if (hQScaledCostForPlayer.TryGetValue("credits", out num))
			{
				credits = num;
			}
			if (hQScaledCostForPlayer.TryGetValue("materials", out num))
			{
				materials = num;
			}
			if (hQScaledCostForPlayer.TryGetValue("contraband", out num))
			{
				contraband = num;
			}
			if (hQScaledCostForPlayer.TryGetValue("reputation", out num))
			{
				reputation = num;
			}
		}

		public static void GetCurrencyCost(string[] cost, out int credits, out int materials, out int contraband, out int reputation, out int crystals)
		{
			credits = 0;
			materials = 0;
			contraband = 0;
			reputation = 0;
			crystals = 0;
			Dictionary<string, int> dictionary = GameUtils.ListToMap(cost);
			foreach (string current in dictionary.Keys)
			{
				if (current != null)
				{
					if (!(current == "credits"))
					{
						if (!(current == "materials"))
						{
							if (!(current == "contraband"))
							{
								if (!(current == "reputation"))
								{
									if (current == "crystals")
									{
										crystals = dictionary[current];
									}
								}
								else
								{
									reputation = dictionary[current];
								}
							}
							else
							{
								contraband = dictionary[current];
							}
						}
						else
						{
							materials = dictionary[current];
						}
					}
					else
					{
						credits = dictionary[current];
					}
				}
			}
		}

		public static bool SpendCrystals(int crystals)
		{
			if (crystals <= 0)
			{
				return false;
			}
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (crystals > currentPlayer.CurrentCrystalsAmount)
			{
				GameUtils.PromptToBuyCrystals();
				return false;
			}
			currentPlayer.Inventory.ModifyCrystals(-crystals);
			return true;
		}

		public static void PromptToBuyCrystals()
		{
			Lang lang = Service.Lang;
			string text = lang.Get("NOT_ENOUGH_CRYSTALS", new object[0]);
			string text2 = lang.Get("NOT_ENOUGH_CRYSTALS_BUY_MORE", new object[0]);
			bool arg_4B_0 = false;
			string arg_4B_1 = text;
			string arg_4B_2 = text2;
			if (GameUtils.<>f__mg$cache1 == null)
			{
				GameUtils.<>f__mg$cache1 = new OnScreenModalResult(GameUtils.OnBuyMoreCrystals);
			}
			AlertScreen.ShowModal(arg_4B_0, arg_4B_1, arg_4B_2, GameUtils.<>f__mg$cache1, null);
		}

		private static void OnBuyMoreCrystals(object result, object cookie)
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			if (currentState is GalaxyState)
			{
				ScreenController screenController = Service.ScreenController;
				screenController.CloseAll();
				GalaxyViewController arg_46_0 = Service.GalaxyViewController;
				bool arg_46_1 = true;
				if (GameUtils.<>f__mg$cache2 == null)
				{
					GameUtils.<>f__mg$cache2 = new WipeCompleteDelegate(GameUtils.ReturnToHomeCompleteNowBuyMoreCrystals);
				}
				arg_46_0.GoToHome(arg_46_1, GameUtils.<>f__mg$cache2, result);
				return;
			}
			if (result != null)
			{
				ScreenController screenController2 = Service.ScreenController;
				StoreScreen storeScreen = screenController2.GetHighestLevelScreen<StoreScreen>();
				if (storeScreen == null)
				{
					storeScreen = new StoreScreen();
					screenController2.AddScreen(storeScreen);
				}
				storeScreen.OpenStoreTab(StoreTab.Crystals);
				Service.EventManager.SendEvent(EventId.UINotEnoughHardCurrencyBuy, null);
			}
			else
			{
				Service.EventManager.SendEvent(EventId.UINotEnoughHardCurrencyClose, null);
			}
		}

		public static void OpenStoreWithTab(StoreTab tabToSelect)
		{
			StoreScreen storeScreen = new StoreScreen();
			storeScreen.OpenStoreTab(tabToSelect);
			Service.ScreenController.AddScreen(storeScreen);
		}

		public static void OpenInventoryCrateModal(CrateData crateData, OnScreenModalResult modalResult)
		{
			CrateInfoModalScreen crateInfoModalScreen = CrateInfoModalScreen.CreateForInventory(crateData);
			crateInfoModalScreen.OnModalResult = modalResult;
			crateInfoModalScreen.IsAlwaysOnTop = true;
			Service.ScreenController.AddScreen(crateInfoModalScreen, true, false);
		}

		public static void ReturnToHomeCompleteNowBuyMoreCrystals(object cookie)
		{
			GameUtils.OnBuyMoreCrystals(cookie, null);
		}

		public static int DroidCrystalCost(int droidIndex)
		{
			string[] array = GameConstants.DROID_CRYSTAL_COSTS.Split(new char[]
			{
				' '
			});
			int result;
			if (droidIndex < array.Length && int.TryParse(array[droidIndex], out result))
			{
				return result;
			}
			return -1;
		}

		public static int EpisodeTaskProgressToCrystals(int count, int target, EpisodeTaskActionVO taskActionVO)
		{
			if (taskActionVO == null)
			{
				Service.Logger.ErrorFormat("Failed to calculate percentile cost; taskActionVO is NULL!", new object[0]);
				return 0;
			}
			return GameUtils.CurrencyPercentile(count, target, taskActionVO.SkipCost);
		}

		public static int SecondsToCrystals(int seconds)
		{
			float baseValue = (float)seconds / 3600f;
			int cRYSTALS_SPEED_UP_COEFFICIENT = GameConstants.CRYSTALS_SPEED_UP_COEFFICIENT;
			int cRYSTALS_SPEED_UP_EXPONENT = GameConstants.CRYSTALS_SPEED_UP_EXPONENT;
			return GameUtils.CurrencyPow(baseValue, cRYSTALS_SPEED_UP_COEFFICIENT, cRYSTALS_SPEED_UP_EXPONENT);
		}

		public static int SecondsToCrystalsForPerk(int seconds)
		{
			float baseValue = (float)seconds / 3600f;
			int sQUADPERK_CRYSTALS_SPEED_UP_COEFFICIENT = GameConstants.SQUADPERK_CRYSTALS_SPEED_UP_COEFFICIENT;
			int sQUADPERK_CRYSTALS_SPEED_UP_EXPONENT = GameConstants.SQUADPERK_CRYSTALS_SPEED_UP_EXPONENT;
			return GameUtils.CurrencyPow(baseValue, sQUADPERK_CRYSTALS_SPEED_UP_COEFFICIENT, sQUADPERK_CRYSTALS_SPEED_UP_EXPONENT);
		}

		public static int SecondsToCrystalsForEpisodeTaskTimeGate(int seconds)
		{
			float baseValue = (float)seconds / 3600f;
			int cRYSTALS_SPEED_UP_TIME_GATE_COEFFICIENT = GameConstants.CRYSTALS_SPEED_UP_TIME_GATE_COEFFICIENT;
			int cRYSTALS_SPEED_UP_TIME_GATE_EXPONENT = GameConstants.CRYSTALS_SPEED_UP_TIME_GATE_EXPONENT;
			return GameUtils.CurrencyPow(baseValue, cRYSTALS_SPEED_UP_TIME_GATE_COEFFICIENT, cRYSTALS_SPEED_UP_TIME_GATE_EXPONENT);
		}

		public static int MultiCurrencyCrystalCost(Dictionary<CurrencyType, int> costMap)
		{
			int num = 0;
			int num2 = 0;
			if (costMap.TryGetValue(CurrencyType.Credits, out num2))
			{
				num += GameUtils.CreditsCrystalCost(num2);
			}
			if (costMap.TryGetValue(CurrencyType.Materials, out num2))
			{
				num += GameUtils.MaterialsCrystalCost(num2);
			}
			if (costMap.TryGetValue(CurrencyType.Contraband, out num2))
			{
				num += GameUtils.ContrabandCrystalCost(num2);
			}
			return num;
		}

		public static int CreditsCrystalCost(int credits)
		{
			int cREDITS_COEFFICIENT = GameConstants.CREDITS_COEFFICIENT;
			int cREDITS_EXPONENT = GameConstants.CREDITS_EXPONENT;
			return GameUtils.CurrencyPow((float)credits, cREDITS_COEFFICIENT, cREDITS_EXPONENT);
		}

		public static int MaterialsCrystalCost(int materials)
		{
			int aLLOY_COEFFICIENT = GameConstants.ALLOY_COEFFICIENT;
			int aLLOY_EXPONENT = GameConstants.ALLOY_EXPONENT;
			return GameUtils.CurrencyPow((float)materials, aLLOY_COEFFICIENT, aLLOY_EXPONENT);
		}

		public static int ContrabandCrystalCost(int contraband)
		{
			int cONTRABAND_COEFFICIENT = GameConstants.CONTRABAND_COEFFICIENT;
			int cONTRABAND_EXPONENT = GameConstants.CONTRABAND_EXPONENT;
			return GameUtils.CurrencyPow((float)contraband, cONTRABAND_COEFFICIENT, cONTRABAND_EXPONENT);
		}

		private static int CurrencyPercentile(int count, int target, int totalCost)
		{
			if (target == 0)
			{
				Service.Logger.ErrorFormat("Failed to calculate percentile cost; cannot divide by target 0!", new object[0]);
			}
			if (totalCost == 0)
			{
				Service.Logger.WarnFormat("In calculating percentile cost, total cost is 0!", new object[0]);
			}
			float num = (float)(target - count) / (float)target;
			return Mathf.CeilToInt(num * (float)totalCost);
		}

		public static int CurrencyPow(float baseValue, int coefficient, int exponent)
		{
			int cOEF_EXP_ACCURACY = GameConstants.COEF_EXP_ACCURACY;
			if (baseValue < 0f || coefficient <= 0 || exponent <= 0 || cOEF_EXP_ACCURACY <= 0)
			{
				return -1;
			}
			float num = (float)coefficient / (float)cOEF_EXP_ACCURACY;
			float p = (float)exponent / (float)cOEF_EXP_ACCURACY;
			return (int)Mathf.Ceil(num * Mathf.Pow(baseValue, p));
		}

		public static int CrystalCostToUpgradeAllWalls(int oneWallCost, int numWalls)
		{
			int num = oneWallCost * numWalls;
			int uPGRADE_ALL_WALLS_COEFFICIENT = GameConstants.UPGRADE_ALL_WALLS_COEFFICIENT;
			int uPGRADE_ALL_WALL_EXPONENT = GameConstants.UPGRADE_ALL_WALL_EXPONENT;
			int num2 = GameUtils.CurrencyPow((float)num, uPGRADE_ALL_WALLS_COEFFICIENT, uPGRADE_ALL_WALL_EXPONENT);
			return (int)Mathf.Ceil((float)num2 * GameConstants.UPGRADE_ALL_WALLS_CONVENIENCE_TAX);
		}

		public static int CrystalCostToInstantUpgrade(BuildingTypeVO nextBuildingInfo)
		{
			return GameUtils.CreditsCrystalCost(nextBuildingInfo.UpgradeCredits) + GameUtils.MaterialsCrystalCost(nextBuildingInfo.UpgradeMaterials) + GameUtils.ContrabandCrystalCost(nextBuildingInfo.UpgradeContraband) + GameUtils.SecondsToCrystals(nextBuildingInfo.Time);
		}

		public static void GetCrystalPacks(out int[] amounts, out int[] prices)
		{
			string[] array = GameConstants.CRYSTAL_PACK_AMOUNT.Split(new char[]
			{
				' '
			});
			string[] array2 = GameConstants.CRYSTAL_PACK_COST_USD.Split(new char[]
			{
				' '
			});
			int num = array.Length;
			if (num != array2.Length)
			{
				amounts = new int[0];
				prices = new int[0];
				return;
			}
			amounts = new int[num];
			prices = new int[num];
			for (int i = 0; i < num; i++)
			{
				int num2;
				int num3;
				if (!int.TryParse(array[i], out num2) || !int.TryParse(array2[i], out num3))
				{
					amounts = new int[0];
					prices = new int[0];
					return;
				}
				amounts[i] = num2;
				prices[i] = num3;
			}
		}

		public static void GetProtectionPacks(out int[] durations, out int[] crystals)
		{
			string[] array = GameConstants.PROTECTION_DURATION.Split(new char[]
			{
				' '
			});
			string[] array2 = GameConstants.PROTECTION_CRYSTAL_COSTS.Split(new char[]
			{
				' '
			});
			int num = array.Length;
			if (num != array2.Length)
			{
				durations = new int[0];
				crystals = new int[0];
				return;
			}
			durations = new int[num];
			crystals = new int[num];
			for (int i = 0; i < num; i++)
			{
				int num2;
				int num3;
				if (!int.TryParse(array[i], out num2) || !int.TryParse(array2[i], out num3))
				{
					durations = new int[0];
					crystals = new int[0];
					return;
				}
				durations[i] = num2;
				crystals[i] = num3;
			}
		}

		public static bool HasEnoughCurrencyStorage(CurrencyType currency, int amount)
		{
			bool flag = true;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (currency != CurrencyType.Credits)
			{
				if (currency != CurrencyType.Materials)
				{
					if (currency == CurrencyType.Contraband)
					{
						flag = (currentPlayer.CurrentContrabandAmount + amount <= currentPlayer.MaxContrabandAmount);
					}
				}
				else
				{
					flag = (currentPlayer.CurrentMaterialsAmount + amount <= currentPlayer.MaxMaterialsAmount);
				}
			}
			else
			{
				flag = (currentPlayer.CurrentCreditsAmount + amount <= currentPlayer.MaxCreditsAmount);
			}
			if (!flag)
			{
				GameUtils.ShowNotEnoughStorageMessage(currency);
			}
			return flag;
		}

		public static void ShowNotEnoughStorageMessage(CurrencyType currency)
		{
			Lang lang = Service.Lang;
			string instructions = lang.Get("NOT_ENOUGH_STORAGE", new object[]
			{
				lang.Get(currency.ToString().ToUpper(), new object[0])
			});
			Service.UXController.MiscElementsManager.ShowPlayerInstructionsError(instructions);
		}

		public static int GetProtectionTimeRemaining()
		{
			uint protectedUntil = Service.CurrentPlayer.ProtectedUntil;
			uint serverTime = Service.ServerAPI.ServerTime;
			if (protectedUntil > serverTime)
			{
				return (int)(protectedUntil - serverTime);
			}
			return 0;
		}

		public static bool BuyLEI(CurrentPlayer player, LimitedEditionItemVO itemVO)
		{
			bool flag = false;
			if (itemVO.Crystals > 0)
			{
				flag = GameUtils.SpendCrystals(itemVO.Crystals);
			}
			else
			{
				bool flag2 = player.CurrentCreditsAmount > itemVO.Credits && player.CurrentMaterialsAmount > itemVO.Materials && player.CurrentContrabandAmount > itemVO.Contraband;
				if (flag2)
				{
					GameUtils.SpendCurrency(itemVO.Credits, itemVO.Materials, itemVO.Contraband, true);
					flag = true;
				}
			}
			if (!flag)
			{
				return false;
			}
			ProcessingScreen.Show();
			BuyLimitedEditionItemRequest request = new BuyLimitedEditionItemRequest(itemVO.Uid);
			BuyLimitedEditionItemCommand buyLimitedEditionItemCommand = new BuyLimitedEditionItemCommand(request);
			AbstractCommand<BuyLimitedEditionItemRequest, CrateDataResponse> arg_B1_0 = buyLimitedEditionItemCommand;
			if (GameUtils.<>f__mg$cache3 == null)
			{
				GameUtils.<>f__mg$cache3 = new AbstractCommand<BuyLimitedEditionItemRequest, CrateDataResponse>.OnSuccessCallback(GameUtils.HandleCratePurchaseResponse);
			}
			arg_B1_0.AddSuccessCallback(GameUtils.<>f__mg$cache3);
			Service.ServerAPI.Sync(buyLimitedEditionItemCommand);
			return true;
		}

		public static bool BuyCrate(CurrentPlayer player, CrateVO crateVO)
		{
			if (!crateVO.Purchasable)
			{
				Service.Logger.ErrorFormat("Crate '{0}' is not purchasable", new object[]
				{
					crateVO.Uid
				});
				return false;
			}
			int num = (!player.ArmoryInfo.FirstCratePurchased) ? 0 : crateVO.Crystals;
			if (num > 0 && !GameUtils.SpendCrystals(num))
			{
				Service.EventManager.SendEvent(EventId.CrateStoreNotEnoughCurrency, crateVO.Uid);
				return false;
			}
			ProcessingScreen.Show();
			player.ArmoryInfo.FirstCratePurchased = true;
			BuyCrateRequest request = new BuyCrateRequest(crateVO.Uid);
			BuyCrateCommand buyCrateCommand = new BuyCrateCommand(request);
			AbstractCommand<BuyCrateRequest, CrateDataResponse> arg_B3_0 = buyCrateCommand;
			if (GameUtils.<>f__mg$cache4 == null)
			{
				GameUtils.<>f__mg$cache4 = new AbstractCommand<BuyCrateRequest, CrateDataResponse>.OnSuccessCallback(GameUtils.HandleCratePurchaseResponse);
			}
			arg_B3_0.AddSuccessCallback(GameUtils.<>f__mg$cache4);
			Service.ServerAPI.Sync(buyCrateCommand);
			Service.EventManager.SendEvent(EventId.CrateStorePurchase, crateVO.Uid);
			return true;
		}

		private static void HandleCratePurchaseResponse(CrateDataResponse response, object cookie)
		{
			if (response.CrateDataTO != null)
			{
				Service.EventManager.SendEvent(EventId.OpeningPurchasedCrate, null);
				CrateData crateDataTO = response.CrateDataTO;
				List<string> resolvedSupplyIdList = GameUtils.GetResolvedSupplyIdList(crateDataTO);
				Service.InventoryCrateRewardController.GrantInventoryCrateReward(resolvedSupplyIdList, response.CrateDataTO);
			}
		}

		public static void OpenCrate(CrateData crateData)
		{
			if (crateData != null)
			{
				crateData.Claimed = true;
				OpenCrateRequest request = new OpenCrateRequest(crateData.UId);
				OpenCrateCommand openCrateCommand = new OpenCrateCommand(request);
				AbstractCommand<OpenCrateRequest, OpenCrateResponse> arg_3E_0 = openCrateCommand;
				if (GameUtils.<>f__mg$cache5 == null)
				{
					GameUtils.<>f__mg$cache5 = new AbstractCommand<OpenCrateRequest, OpenCrateResponse>.OnSuccessCallback(GameUtils.CrateOpenSuccessCallback);
				}
				arg_3E_0.AddSuccessCallback(GameUtils.<>f__mg$cache5);
				AbstractCommand<OpenCrateRequest, OpenCrateResponse> arg_61_0 = openCrateCommand;
				if (GameUtils.<>f__mg$cache6 == null)
				{
					GameUtils.<>f__mg$cache6 = new AbstractCommand<OpenCrateRequest, OpenCrateResponse>.OnFailureCallback(GameUtils.CrateOpenFailureCallback);
				}
				arg_61_0.AddFailureCallback(GameUtils.<>f__mg$cache6);
				openCrateCommand.Context = crateData;
				ProcessingScreen.Show();
				Service.ServerAPI.Sync(openCrateCommand);
				Service.EventManager.SendEvent(EventId.InventoryCrateOpened, crateData.CrateId + "|" + crateData.UId);
			}
		}

		public static void CrateOpenSuccessCallback(OpenCrateResponse response, object cookie)
		{
			if (response.SupplyIDs != null)
			{
				List<string> supplyIDs = response.SupplyIDs;
				Service.InventoryCrateRewardController.GrantInventoryCrateReward(supplyIDs, (CrateData)cookie);
			}
		}

		public static void CrateOpenFailureCallback(uint status, object cookie)
		{
			ProcessingScreen.Hide();
			Service.Logger.Error("Failed to inventory open crate");
		}

		public static void ClaimCurrentEpisodeTask()
		{
			EpisodeTaskClaimCommand episodeTaskClaimCommand = new EpisodeTaskClaimCommand(new PlayerIdRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			AbstractCommand<PlayerIdRequest, EpisodeTaskClaimResponse> arg_3B_0 = episodeTaskClaimCommand;
			if (GameUtils.<>f__mg$cache7 == null)
			{
				GameUtils.<>f__mg$cache7 = new AbstractCommand<PlayerIdRequest, EpisodeTaskClaimResponse>.OnSuccessCallback(GameUtils.HandleEpisodeClaimResponse);
			}
			arg_3B_0.AddSuccessCallback(GameUtils.<>f__mg$cache7);
			Service.ServerAPI.Sync(episodeTaskClaimCommand);
		}

		private static void HandleEpisodeClaimResponse(EpisodeTaskClaimResponse response, object cookie)
		{
			if (response.CrateDataTO != null)
			{
				Service.EventManager.SendEvent(EventId.OpeningEpisodeTaskCrate, null);
				CrateData crateDataTO = response.CrateDataTO;
				List<string> resolvedSupplyIdList = GameUtils.GetResolvedSupplyIdList(crateDataTO);
				Service.InventoryCrateRewardController.GrantInventoryCrateReward(resolvedSupplyIdList, crateDataTO);
			}
		}

		public static List<string> GetResolvedSupplyIdList(CrateData crateData)
		{
			List<string> list = new List<string>();
			List<SupplyData> resolvedSupplies = crateData.ResolvedSupplies;
			int count = resolvedSupplies.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					list.Add(resolvedSupplies[i].SupplyId);
				}
			}
			return list;
		}

		public static void ShowCrateAwardModal(string awardedCrateUid)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			Dictionary<string, CrateData> available = currentPlayer.Prizes.Crates.Available;
			if (string.IsNullOrEmpty(awardedCrateUid) || !available.ContainsKey(awardedCrateUid))
			{
				Service.Logger.ErrorFormat("Cannot show crate reward modal, crate Id: {0} doesn't exist in inventory", new object[]
				{
					awardedCrateUid
				});
				return;
			}
			CrateData crateData = available[awardedCrateUid];
			CrateVO optional = staticDataController.GetOptional<CrateVO>(crateData.CrateId);
			if (optional == null)
			{
				Service.Logger.ErrorFormat("Cannot show crate reward modal, static data not found for crate Id: {0}", new object[]
				{
					awardedCrateUid
				});
				return;
			}
			CrateRewardModalScreen crateRewardModalScreen = new CrateRewardModalScreen(optional);
			crateRewardModalScreen.IsAlwaysOnTop = true;
			Service.ScreenController.AddScreen(crateRewardModalScreen, true, true);
		}

		public static bool BuyProtectionPackWithCrystals(int packNumber)
		{
			int[] array;
			int[] array2;
			GameUtils.GetProtectionPacks(out array, out array2);
			if (packNumber - 1 >= array.Length || packNumber - 1 >= array2.Length)
			{
				return false;
			}
			if (!GameUtils.SpendCrystals(array2[packNumber - 1]))
			{
				return false;
			}
			BuyResourceRequest request = BuyResourceRequest.MakeBuyProtectionRequest(packNumber);
			Service.ServerAPI.Enqueue(new BuyResourceCommand(request));
			uint time = ServerTime.Time;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (currentPlayer.ProtectedUntil == 0u)
			{
				currentPlayer.ProtectedUntil = time;
			}
			uint num = (uint)array[packNumber - 1];
			currentPlayer.ProtectedUntil += num * 60u;
			currentPlayer.AddProtectionCooldownUntil(packNumber, time + num * 60u * 6u);
			int currencyAmount = -array2[packNumber - 1];
			string itemType = "protection";
			string itemId = array[packNumber - 1].ToString();
			int itemCount = 1;
			string type = "damage_protection";
			string subType = "consumable";
			Service.DMOAnalyticsController.LogInAppCurrencyAction(currencyAmount, itemType, itemId, itemCount, type, subType);
			return true;
		}

		public static bool HandleSoftCurrencyFlow(object result, object cookie)
		{
			bool flag = result != null;
			if (flag)
			{
				if (cookie is CurrencyTag)
				{
					CurrencyTag currencyTag = (CurrencyTag)cookie;
					CurrencyType currency = currencyTag.Currency;
					int amount = currencyTag.Amount;
					int crystals = currencyTag.Crystals;
					string purchaseContext = currencyTag.PurchaseContext;
					if (!GameUtils.BuySoftCurrencyWithCrystals(currency, amount, crystals, purchaseContext, true))
					{
						flag = false;
					}
				}
				else if (cookie is MultiCurrencyTag)
				{
					MultiCurrencyTag multiCurrencyTag = (MultiCurrencyTag)cookie;
					string itemId = string.Empty;
					if (multiCurrencyTag.Cookie != null && multiCurrencyTag.Cookie is string)
					{
						itemId = (string)multiCurrencyTag.Cookie;
					}
					if (GameUtils.BuySoftCurrenciesWithCrystals(multiCurrencyTag.Credits, multiCurrencyTag.Materials, multiCurrencyTag.Contraband, multiCurrencyTag.Crystals, multiCurrencyTag.PurchaseContext, itemId))
					{
						flag = false;
					}
				}
			}
			return flag;
		}

		public static bool BuySoftCurrencyWithCrystals(CurrencyType currency, int amount, int crystals, string purchaseContext, bool softCurrencyFlow)
		{
			if (!GameUtils.SpendCrystals(crystals))
			{
				return false;
			}
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (currency != CurrencyType.Credits)
			{
				if (currency != CurrencyType.Materials)
				{
					if (currency == CurrencyType.Contraband)
					{
						currentPlayer.Inventory.ModifyContraband(amount);
					}
				}
				else
				{
					currentPlayer.Inventory.ModifyMaterials(amount);
				}
			}
			else
			{
				currentPlayer.Inventory.ModifyCredits(amount);
			}
			BuyResourceRequest buyResourceRequest = BuyResourceRequest.MakeBuyResourceRequest(currency, amount);
			if (!string.IsNullOrEmpty(purchaseContext))
			{
				buyResourceRequest.setPurchaseContext(purchaseContext);
			}
			Service.ServerAPI.Enqueue(new BuyResourceCommand(buyResourceRequest));
			int currencyAmount = -crystals;
			string itemType = string.Empty;
			string itemId = purchaseContext;
			if (softCurrencyFlow)
			{
				itemType = "soft_currency_flow";
				if (currency != CurrencyType.Credits)
				{
					if (currency != CurrencyType.Materials)
					{
						if (currency == CurrencyType.Contraband)
						{
							itemId = "contraband";
						}
					}
					else
					{
						itemId = "materials";
					}
				}
				else
				{
					itemId = "credits";
				}
			}
			string type = "currency_purchase";
			string subType = "durable";
			Service.DMOAnalyticsController.LogInAppCurrencyAction(currencyAmount, itemType, itemId, amount, type, subType);
			return true;
		}

		public static bool BuySoftCurrenciesWithCrystals(int credits, int materials, int contraband, int crystals, string purchaseContext, string itemId)
		{
			if (!GameUtils.SpendCrystals(crystals))
			{
				return false;
			}
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (credits > 0)
			{
				currentPlayer.Inventory.ModifyCredits(credits);
			}
			if (materials > 0)
			{
				currentPlayer.Inventory.ModifyMaterials(materials);
			}
			if (contraband > 0)
			{
				currentPlayer.Inventory.ModifyContraband(contraband);
			}
			BuyMultiResourceRequest request = new BuyMultiResourceRequest(credits, materials, contraband, purchaseContext);
			Service.ServerAPI.Enqueue(new BuyMultiResourceCommand(request));
			int currencyAmount = -crystals;
			string empty = string.Empty;
			int itemCount = 1;
			string type = "currency_purchase";
			string subType = "durable";
			Service.DMOAnalyticsController.LogInAppCurrencyAction(currencyAmount, empty, itemId, itemCount, type, subType);
			return true;
		}

		public static bool BuyNextDroid(bool allDroidsWereBusy)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (currentPlayer.CurrentDroidsAmount >= currentPlayer.MaxDroidsAmount)
			{
				return false;
			}
			int num = GameUtils.DroidCrystalCost(currentPlayer.CurrentDroidsAmount);
			if (!GameUtils.SpendCrystals(num))
			{
				return false;
			}
			currentPlayer.Inventory.ModifyDroids(1);
			BuyResourceRequest buyResourceRequest = BuyResourceRequest.MakeBuyDroidRequest(1);
			buyResourceRequest.setPurchaseContext((!allDroidsWereBusy) ? "droidHutUpgrade" : "allDroidsBusy");
			BuyResourceCommand command = new BuyResourceCommand(buyResourceRequest);
			Service.ServerAPI.Enqueue(command);
			int currencyAmount = -num;
			string itemType = "droid_hut";
			string analyticsDroidHutType = GameUtils.GetAnalyticsDroidHutType();
			int itemCount = 1;
			string type = (!currentPlayer.CampaignProgress.FueInProgress) ? "droid_upgrade" : "FUE_droid_upgrade";
			string subType = "durable";
			Service.DMOAnalyticsController.LogInAppCurrencyAction(currencyAmount, itemType, analyticsDroidHutType, itemCount, type, subType);
			Service.EventManager.SendEvent(EventId.DroidPurchaseCompleted, null);
			return true;
		}

		private static string GetAnalyticsDroidHutType()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			return currentPlayer.Faction.ToString().ToLower() + "DroidHut";
		}

		public static string GetBuildingPurchaseContext(BuildingTypeVO nextBuildingVO, BuildingTypeVO currentBuildingVO, bool isUpgrade, bool isSwap)
		{
			return GameUtils.GetBuildingPurchaseContext(nextBuildingVO, currentBuildingVO, isUpgrade, isSwap, null);
		}

		public static string GetBuildingPurchaseContext(BuildingTypeVO nextBuildingVO, BuildingTypeVO currentBuildingVO, bool isUpgrade, bool isSwap, PlanetVO selectedPlanet)
		{
			string value = StringUtils.ToLowerCaseUnderscoreSeperated(nextBuildingVO.Type.ToString());
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(value);
			if (selectedPlanet != null)
			{
				stringBuilder.Append("|");
				stringBuilder.Append(selectedPlanet.PlanetBIName);
			}
			if (isUpgrade)
			{
				stringBuilder.Append("|");
				stringBuilder.Append("upgrade");
			}
			if (isSwap)
			{
				stringBuilder.Append("|");
				stringBuilder.Append("cross");
			}
			stringBuilder.Append("|");
			stringBuilder.Append(nextBuildingVO.BuildingID);
			stringBuilder.Append("|");
			stringBuilder.Append(nextBuildingVO.Lvl);
			if (isSwap && currentBuildingVO != null)
			{
				stringBuilder.Append("|");
				stringBuilder.Append(currentBuildingVO.BuildingID);
			}
			return stringBuilder.ToString();
		}

		public static void OpenURL(string url)
		{
			if (Service.EnvironmentController.IsRestrictedProfile())
			{
				string message = Service.Lang.Get("RESTRICTED_WEB_ACCESS_WARNING", new object[0]);
				AlertScreen.ShowModal(false, null, message, null, null);
			}
			else
			{
				string text = Service.Lang.Get("EXIT_WARNING", new object[0]);
				bool arg_6C_0 = false;
				string arg_6C_1 = null;
				string arg_6C_2 = text;
				if (GameUtils.<>f__mg$cache8 == null)
				{
					GameUtils.<>f__mg$cache8 = new OnScreenModalResult(GameUtils.OnOpenURLModalResult);
				}
				AlertScreen.ShowModal(arg_6C_0, arg_6C_1, arg_6C_2, GameUtils.<>f__mg$cache8, url);
			}
		}

		private static void OnOpenURLModalResult(object result, object cookie)
		{
			if (result != null)
			{
				Application.OpenURL((string)cookie);
			}
		}

		public static void ToggleGameObjectViewVisibility(GameObjectViewComponent viewComp, bool visible)
		{
			if (viewComp != null && viewComp.MainTransform != null)
			{
				Transform mainTransform = viewComp.MainTransform;
				IEnumerator enumerator = mainTransform.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Transform transform = (Transform)enumerator.Current;
						transform.gameObject.SetActive(visible);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		public static string GetDeviceInfo()
		{
			return string.Format("Device: {0}-{1}, OS: {2}, Processor: {3}x{4}, Memory: {5}, Graphics: {6}-{7}-{8}", new object[]
			{
				SystemInfo.deviceModel,
				SystemInfo.deviceType,
				SystemInfo.operatingSystem,
				SystemInfo.processorType,
				SystemInfo.processorCount,
				SystemInfo.systemMemorySize,
				SystemInfo.graphicsDeviceVendor,
				SystemInfo.graphicsDeviceName,
				SystemInfo.graphicsDeviceVersion
			}).Trim();
		}

		public static long GetJavaEpochTime(DateTime time)
		{
			return Convert.ToInt64((time - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
		}

		public static long GetNowJavaEpochTime()
		{
			return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);
		}

		public static int CalculateDamagePercentage(HealthComponent healthComp)
		{
			return GameUtils.CalculateDamagePercentage(healthComp.Health, healthComp.MaxHealth);
		}

		public static int CalculateDamagePercentage(int currentHealth, int maxHealth)
		{
			float num = (float)currentHealth / (float)maxHealth;
			return (int)((1f - num) * 100f);
		}

		public static long CalculateResourceChecksum(int credits, int materials, int contraband, int crystals)
		{
			return 31L * (long)credits ^ 31L * (long)materials << 10 ^ 31L * (long)contraband << 20 ^ 31L * (long)crystals << 30;
		}

		public static string CreateInfoStringForChecksum(Contract additionalContract, bool instantContract, bool simulateTroopContractUpdate, ref int crystals)
		{
			ISupportController iSupportController = Service.ISupportController;
			List<Contract> list = null;
			List<Contract> list2 = null;
			iSupportController.GetEstimatedUpdatedContractListsForChecksum(simulateTroopContractUpdate, out list, out list2);
			int num = (list2 != null) ? list2.Count : 0;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			Map map = currentPlayer.Map;
			StaticDataController staticDataController = Service.StaticDataController;
			StringBuilder stringBuilder = new StringBuilder();
			List<Building> list3 = new List<Building>(map.Buildings);
			BaseLayoutToolController baseLayoutToolController = Service.BaseLayoutToolController;
			bool flag = baseLayoutToolController != null && baseLayoutToolController.ShouldChecksumLastSaveData();
			if (flag)
			{
				List<Building> arg_97_0 = list3;
				if (GameUtils.<>f__mg$cache9 == null)
				{
					GameUtils.<>f__mg$cache9 = new Comparison<Building>(GameUtils.CompareLastSavedBuildingLocation);
				}
				arg_97_0.Sort(GameUtils.<>f__mg$cache9);
			}
			else
			{
				List<Building> arg_C0_0 = list3;
				if (GameUtils.<>f__mg$cacheA == null)
				{
					GameUtils.<>f__mg$cacheA = new Comparison<Building>(GameUtils.CompareBuildingsByPosition);
				}
				arg_C0_0.Sort(GameUtils.<>f__mg$cacheA);
			}
			stringBuilder.Append("--Buildings--\n");
			int i = 0;
			int count = list3.Count;
			while (i < count)
			{
				Building building = list3[i];
				string uidOverride = null;
				uint timeOverride = 0u;
				bool flag2 = false;
				for (int j = 0; j < num; j++)
				{
					Contract contract = list2[j];
					if (contract.ContractTO.BuildingKey == building.Key && ContractUtils.IsBuildingType(contract.ContractTO.ContractType))
					{
						if (contract.DeliveryType == DeliveryType.UpgradeBuilding || contract.DeliveryType == DeliveryType.SwapBuilding)
						{
							uidOverride = contract.ProductUid;
						}
						if (contract.DeliveryType == DeliveryType.Building || contract.DeliveryType == DeliveryType.UpgradeBuilding || contract.DeliveryType == DeliveryType.SwapBuilding)
						{
							BuildingTypeVO buildingTypeVO = staticDataController.Get<BuildingTypeVO>(contract.ProductUid);
							if (buildingTypeVO.Type == BuildingType.Resource)
							{
								timeOverride = contract.ContractTO.EndTime;
							}
						}
						if (contract.DeliveryType == DeliveryType.ClearClearable)
						{
							flag2 = true;
							crystals += building.CurrentStorage;
						}
						break;
					}
				}
				if (!flag2)
				{
					if (instantContract && additionalContract != null && additionalContract.DeliveryType == DeliveryType.UpgradeBuilding && additionalContract.ContractTO.BuildingKey == building.Key)
					{
						uidOverride = additionalContract.ProductUid;
						timeOverride = 0u;
						additionalContract = null;
					}
					if (flag)
					{
						int buildingLastSavedX = baseLayoutToolController.GetBuildingLastSavedX(building.Key);
						int buildingLastSavedZ = baseLayoutToolController.GetBuildingLastSavedZ(building.Key);
						building.AddString(stringBuilder, uidOverride, timeOverride, buildingLastSavedX, buildingLastSavedZ);
					}
					else
					{
						building.AddString(stringBuilder, uidOverride, timeOverride);
					}
				}
				i++;
			}
			stringBuilder.Append("--Contracts--\n");
			if (list != null)
			{
				if (additionalContract != null)
				{
					list.Add(additionalContract);
				}
				list.Sort(new Comparison<Contract>(iSupportController.SortByEndTime));
				int k = 0;
				int count2 = list.Count;
				while (k < count2)
				{
					list[k].AddString(stringBuilder);
					k++;
				}
				List<ContractTO> uninitializedContractData = iSupportController.GetUninitializedContractData();
				if (uninitializedContractData != null)
				{
					uninitializedContractData.Sort(new Comparison<ContractTO>(iSupportController.SortContractTOByEndTime));
					int l = 0;
					int count3 = uninitializedContractData.Count;
					while (l < count3)
					{
						uninitializedContractData[l].AddString(stringBuilder);
						l++;
					}
				}
			}
			return stringBuilder.ToString();
		}

		public static long StringHash(string str)
		{
			int num = 7;
			for (int i = 0; i < str.Length; i++)
			{
				num = num * 31 + (int)str[i];
			}
			return (long)num;
		}

		public static int CompareBuildingsByPosition(Building a, Building b)
		{
			int num = a.X - b.X;
			if (num == 0)
			{
				return a.Z - b.Z;
			}
			return num;
		}

		public static int CompareLastSavedBuildingLocation(Building a, Building b)
		{
			BaseLayoutToolController baseLayoutToolController = Service.BaseLayoutToolController;
			int buildingLastSavedX = baseLayoutToolController.GetBuildingLastSavedX(a.Key);
			int buildingLastSavedX2 = baseLayoutToolController.GetBuildingLastSavedX(b.Key);
			int num = buildingLastSavedX - buildingLastSavedX2;
			if (num == 0)
			{
				int buildingLastSavedZ = baseLayoutToolController.GetBuildingLastSavedZ(a.Key);
				int buildingLastSavedZ2 = baseLayoutToolController.GetBuildingLastSavedZ(b.Key);
				return buildingLastSavedZ - buildingLastSavedZ2;
			}
			return num;
		}

		public static int GetTimeDifferenceSafe(uint timeA, uint timeB)
		{
			uint num;
			if (timeA > timeB)
			{
				num = timeA - timeB;
			}
			else
			{
				num = timeB - timeA;
			}
			if (num > 2147483647u)
			{
				Service.Logger.ErrorFormat("Attempted to get time difference but delta time {1} is too large.", new object[]
				{
					num
				});
				return 0;
			}
			return (int)(timeA - timeB);
		}

		public static uint GetModifiedTimeSafe(uint time, int modifier)
		{
			uint result;
			if (modifier >= 0)
			{
				result = time + (uint)modifier;
			}
			else
			{
				modifier = -modifier;
				if (time < (uint)modifier)
				{
					result = 0u;
				}
				else
				{
					result = time - (uint)modifier;
				}
			}
			return result;
		}

		public static FactionType GetOppositeFaction(FactionType faction)
		{
			if (faction == FactionType.Empire)
			{
				return FactionType.Rebel;
			}
			if (faction != FactionType.Rebel)
			{
				return FactionType.Invalid;
			}
			return FactionType.Empire;
		}

		public static bool IsPlanetCurrentOne(string uid)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			PlanetVO planet = currentPlayer.Map.Planet;
			return planet.Uid.Equals(uid);
		}

		public static bool IsMissionSpecOps(string missionId)
		{
			if (!string.IsNullOrEmpty(missionId))
			{
				StaticDataController staticDataController = Service.StaticDataController;
				CampaignMissionVO campaignMissionVO = staticDataController.Get<CampaignMissionVO>(missionId);
				return campaignMissionVO.BIContext == "campaign";
			}
			return false;
		}

		public static bool IsMissionRaidDefense(string missionId)
		{
			if (!string.IsNullOrEmpty(missionId))
			{
				StaticDataController staticDataController = Service.StaticDataController;
				CampaignMissionVO campaignMissionVO = staticDataController.Get<CampaignMissionVO>(missionId);
				return campaignMissionVO.MissionType == MissionType.RaidDefend;
			}
			return false;
		}

		public static bool IsMissionDefense(string missionId)
		{
			if (!string.IsNullOrEmpty(missionId))
			{
				StaticDataController staticDataController = Service.StaticDataController;
				CampaignMissionVO campaignMissionVO = staticDataController.Get<CampaignMissionVO>(missionId);
				return campaignMissionVO.MissionType == MissionType.Defend || campaignMissionVO.MissionType == MissionType.RaidDefend;
			}
			return false;
		}

		public static CampaignVO GetHighestUnlockedCampaign()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			StaticDataController staticDataController = Service.StaticDataController;
			int num = -1;
			CampaignVO campaignVO = null;
			foreach (CampaignVO current in staticDataController.GetAll<CampaignVO>())
			{
				if (!current.Timed && current.Faction == currentPlayer.Faction)
				{
					if (currentPlayer.CampaignProgress.HasCampaign(current) && current.UnlockOrder > num)
					{
						campaignVO = current;
						num = campaignVO.UnlockOrder;
					}
				}
			}
			return campaignVO;
		}

		public static bool HasAvailableTroops(bool isPvE, BattleTypeVO battle)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			Inventory inventory = currentPlayer.Inventory;
			if (inventory.Troop.GetTotalStorageAmount() > 0 || inventory.Champion.GetTotalStorageAmount() > 0 || inventory.SpecialAttack.GetTotalStorageAmount() > 0)
			{
				return true;
			}
			if (isPvE)
			{
				return battle != null && battle.OverridePlayerUnits;
			}
			return inventory.Hero.GetTotalStorageAmount() > 0 || SquadUtils.GetDonatedTroopStorageUsedByCurrentPlayer() > 0;
		}

		public static void ExitEditState()
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			if (currentState is EditBaseState)
			{
				HomeState.GoToHomeState(null, false);
			}
		}

		public static bool IsDeviceCountryInList(string countryList)
		{
			if (string.IsNullOrEmpty(countryList))
			{
				return false;
			}
			countryList = countryList.Replace(" ", string.Empty);
			countryList = countryList.ToLower();
			string[] array = countryList.Split(new char[]
			{
				'|'
			});
			string text = Service.EnvironmentController.GetDeviceCountryCode();
			text = text.ToLower();
			for (int i = 0; i < array.Length; i++)
			{
				if (text.Equals(array[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsBattleVersionSupported(string cmsVersion, string battleVersion)
		{
			return cmsVersion == Service.ContentManager.GetFileVersion("patches/base.json").ToString() && battleVersion == "30.0";
		}

		public static BoardCellDynamicArray TraverseSpiral(int radius, int centerX, int centerZ)
		{
			BoardController boardController = Service.BoardController;
			BoardCellDynamicArray result = new BoardCellDynamicArray(radius * radius);
			int num3;
			int num2;
			int num = num2 = (num3 = 0);
			int num4 = -1;
			int num5 = radius * 2 + 1;
			int num6 = num5 * num5;
			for (int i = 0; i < num6; i++)
			{
				if (-radius <= num2 && num2 <= radius && -radius <= num && num <= radius)
				{
					BoardCell clampedToBoardCellAt = boardController.Board.GetClampedToBoardCellAt(centerX + num2, centerZ + num, 1);
					if (clampedToBoardCellAt != null)
					{
						result.Add(clampedToBoardCellAt);
					}
				}
				if (num2 == num || (num2 < 0 && num2 == -num) || (num2 > 0 && num2 == 1 - num))
				{
					num5 = num3;
					num3 = -num4;
					num4 = num5;
				}
				num2 += num3;
				num += num4;
			}
			return result;
		}

		public static uint GetCurrentSimFrame()
		{
			return Service.SimTimeEngine.GetFrameCount();
		}

		public static void UpdateMinimumFrameCountForNextTargeting(ShooterComponent shooterComponent)
		{
			shooterComponent.MinimumFrameCountForNextTargeting = GameUtils.GetCurrentSimFrame() + 30u;
		}

		public static bool IsEligibleToFindTarget(ShooterComponent shooterComponent)
		{
			return shooterComponent != null && shooterComponent.MinimumFrameCountForNextTargeting <= GameUtils.GetCurrentSimFrame();
		}

		public static bool IsEntityDead(SmartEntity entity)
		{
			return entity.HealthComp == null || entity.HealthComp.IsDead();
		}

		public static Quaternion FindRelativeRotation(Quaternion a, Quaternion b)
		{
			return Quaternion.Inverse(b) * a;
		}

		public static Quaternion ApplyRelativeRotation(Quaternion a, Quaternion b)
		{
			return b * a;
		}

		public static bool IsEntityShieldGenerator(SmartEntity entity)
		{
			return entity.ShieldGeneratorComp != null;
		}

		public static void TryAndOpenAppropriateStorePage()
		{
			Application.OpenURL("market://details?id=com.lucasarts.starts_goo");
		}

		public static string GetTournamentPointIconName(string planetId)
		{
			if (string.IsNullOrEmpty(planetId))
			{
				return null;
			}
			PlanetVO optional = Service.StaticDataController.GetOptional<PlanetVO>(planetId);
			return (optional != null) ? optional.MedalIconName : null;
		}

		public static bool ConflictStartsInBadgePeriod(TournamentVO tournamentVO)
		{
			return tournamentVO.StartTimestamp - (int)ServerTime.Time < GameConstants.TOURNAMENT_HOURS_SHOW_BADGE * 3600;
		}

		public static string GetCurrencyIconName(string currencyName)
		{
			string result = null;
			if (currencyName == "contraband")
			{
				result = "icoContraband";
			}
			else if (currencyName == "crystals")
			{
				result = "icoCrystals";
			}
			else if (currencyName == "credits")
			{
				result = "icoCollectCredit";
			}
			else if (currencyName == "materials")
			{
				result = "icoMaterials";
			}
			else if (currencyName == "reputation")
			{
				result = "icoReputation";
			}
			return result;
		}

		public static string GetClearableAssetName(BuildingTypeVO buildingVO, PlanetVO planetVO)
		{
			string str = buildingVO.AssetName.Substring(0, buildingVO.AssetName.LastIndexOf("-") + 1);
			return str + planetVO.Abbreviation;
		}

		public static bool IsPvpTargetSearchFailureRequiresReload(uint commandStatus)
		{
			bool result = true;
			switch (commandStatus)
			{
			case 2100u:
			case 2101u:
			case 2102u:
			case 2103u:
			case 2104u:
			case 2105u:
			case 2107u:
				result = false;
				break;
			}
			return result;
		}

		public static void IndicateNewInventoryItems(RewardVO vo)
		{
			if (vo.CurrencyRewards != null)
			{
				Service.ServerPlayerPrefs.SetPref(ServerPref.NumInventoryCurrencyNotViewed, "1");
			}
			if (vo.TroopRewards != null || vo.SpecialAttackRewards != null || vo.HeroRewards != null)
			{
				Service.ServerPlayerPrefs.SetPref(ServerPref.NumInventoryTroopsNotViewed, "1");
			}
			Service.ServerAPI.Sync(new SetPrefsCommand(false));
			if (!GameUtils.IsAppLoading())
			{
				Service.EventManager.SendEvent(EventId.NumInventoryItemsNotViewedUpdated, null);
			}
		}

		public static void UpdateInventoryCrateBadgeCount(int delta)
		{
			if (delta != 0)
			{
				ServerPlayerPrefs serverPlayerPrefs = Service.ServerPlayerPrefs;
				int val = Convert.ToInt32(serverPlayerPrefs.GetPref(ServerPref.NumInventoryCratesNotViewed)) + delta;
				serverPlayerPrefs.SetPref(ServerPref.NumInventoryCratesNotViewed, Math.Max(0, val).ToString());
				Service.ServerAPI.Enqueue(new SetPrefsCommand(false));
			}
			if (!GameUtils.IsAppLoading())
			{
				Service.EventManager.SendEvent(EventId.NumInventoryItemsNotViewedUpdated, null);
			}
		}

		public static void AddRewardToInventory(RewardVO vo)
		{
			PrizeInventory prizes = Service.CurrentPlayer.Prizes;
			StaticDataController staticDataController = Service.StaticDataController;
			if (vo.CurrencyRewards != null)
			{
				int i = 0;
				int num = vo.CurrencyRewards.Length;
				while (i < num)
				{
					string[] array = vo.CurrencyRewards[i].Split(new char[]
					{
						':'
					});
					prizes.ModifyResourceAmount(array[0], Convert.ToInt32(array[1]));
					i++;
				}
			}
			if (vo.TroopRewards != null)
			{
				int j = 0;
				int num2 = vo.TroopRewards.Length;
				while (j < num2)
				{
					string[] array = vo.TroopRewards[j].Split(new char[]
					{
						':'
					});
					IUpgradeableVO upgradeableVO = staticDataController.Get<TroopTypeVO>(array[0]);
					prizes.ModifyTroopAmount(upgradeableVO.UpgradeGroup, Convert.ToInt32(array[1]));
					j++;
				}
			}
			if (vo.SpecialAttackRewards != null)
			{
				int k = 0;
				int num3 = vo.SpecialAttackRewards.Length;
				while (k < num3)
				{
					string[] array = vo.SpecialAttackRewards[k].Split(new char[]
					{
						':'
					});
					IUpgradeableVO upgradeableVO = staticDataController.Get<SpecialAttackTypeVO>(array[0]);
					prizes.ModifySpecialAttackAmount(upgradeableVO.UpgradeGroup, Convert.ToInt32(array[1]));
					k++;
				}
			}
			if (vo.HeroRewards != null)
			{
				int l = 0;
				int num4 = vo.HeroRewards.Length;
				while (l < num4)
				{
					string[] array = vo.HeroRewards[l].Split(new char[]
					{
						':'
					});
					IUpgradeableVO upgradeableVO = staticDataController.Get<TroopTypeVO>(array[0]);
					prizes.ModifyTroopAmount(upgradeableVO.UpgradeGroup, Convert.ToInt32(array[1]));
					l++;
				}
			}
			GameUtils.IndicateNewInventoryItems(vo);
		}

		public static int GetNumInventoryItemsNotViewed()
		{
			ServerPlayerPrefs serverPlayerPrefs = Service.ServerPlayerPrefs;
			int num = 0;
			num += Convert.ToInt32(serverPlayerPrefs.GetPref(ServerPref.NumInventoryItemsNotViewed));
			num += Convert.ToInt32(serverPlayerPrefs.GetPref(ServerPref.NumInventoryCratesNotViewed));
			num += Convert.ToInt32(serverPlayerPrefs.GetPref(ServerPref.NumInventoryTroopsNotViewed));
			return num + Convert.ToInt32(serverPlayerPrefs.GetPref(ServerPref.NumInventoryCurrencyNotViewed));
		}

		public static Transform FindAssetMetaDataTransform(GameObject gameObj, string searchName)
		{
			Transform result = null;
			if (gameObj != null && !string.IsNullOrEmpty(searchName))
			{
				AssetMeshDataMonoBehaviour component = gameObj.GetComponent<AssetMeshDataMonoBehaviour>();
				if (component != null)
				{
					int count = component.OtherGameObjects.Count;
					for (int i = 0; i < count; i++)
					{
						GameObject gameObject = component.OtherGameObjects[i];
						if (gameObject.name.Contains(searchName))
						{
							result = gameObject.transform;
							break;
						}
					}
				}
			}
			return result;
		}

		public static bool IsAppLoading()
		{
			return Service.GameStateMachine.CurrentState is ApplicationLoadState;
		}

		public static string GetSingleCurrencyItemAssetName(string currencyType)
		{
			CurrencyType currencyType2 = StringUtils.ParseEnum<CurrencyType>(currencyType);
			return GameUtils.GetSingleCurrencyItemAssetName(currencyType2);
		}

		public static string GetSingleCurrencyItemAssetName(CurrencyType currencyType)
		{
			switch (currencyType)
			{
			case CurrencyType.Credits:
				return "collectcredit-ani";
			case CurrencyType.Materials:
				return "collectmaterial-ani";
			case CurrencyType.Contraband:
				return "collectcontraband-ani";
			default:
				return "collectcredit-ani";
			}
		}

		public static string GetSingleCurrencyIconAssetName(string currencyType)
		{
			CurrencyType currencyType2 = StringUtils.ParseEnum<CurrencyType>(currencyType);
			return GameUtils.GetSingleCurrencyIconAssetName(currencyType2);
		}

		public static string GetSingleCurrencyIconAssetName(CurrencyType currencyType)
		{
			switch (currencyType)
			{
			case CurrencyType.Credits:
				return "currencyicon_neu-mod_credit";
			case CurrencyType.Materials:
				return "currencyicon_neu-mod_alloy";
			case CurrencyType.Contraband:
				return "currencyicon_neu-mod_contraband";
			case CurrencyType.Crystals:
				return "currencyicon_neu-mod_crystal";
			}
			return "currencyicon_neu-mod_credit";
		}

		public static string GetInventorySupplyIconAssetName(CrateSupplyVO crateSupply, out IGeometryVO config, out string supplyName, int hqLevel)
		{
			config = null;
			supplyName = null;
			if (crateSupply == null)
			{
				return null;
			}
			string rewardUid = crateSupply.RewardUid;
			if (rewardUid == null || rewardUid.Length <= 0)
			{
				return null;
			}
			InventoryCrateRewardController inventoryCrateRewardController = Service.InventoryCrateRewardController;
			RewardVO vo = inventoryCrateRewardController.GenerateRewardFromSupply(crateSupply, hqLevel);
			supplyName = Service.RewardManager.GetRewardString(vo, Service.Lang.Get("SupplyRewardFormat", new object[0]));
			SupplyType type = crateSupply.Type;
			if (type == SupplyType.Currency)
			{
				config = Service.StaticDataController.Get<CurrencyIconVO>(rewardUid);
				return GameUtils.GetSingleCurrencyIconAssetName(rewardUid);
			}
			if (type == SupplyType.Shard)
			{
				config = ArmoryUtils.GetCurrentEquipmentDataByID(crateSupply.RewardUid);
				return config.IconAssetName;
			}
			if (type == SupplyType.Troop || type == SupplyType.Hero)
			{
				TroopTypeVO troopTypeVO = Service.StaticDataController.Get<TroopTypeVO>(rewardUid);
				config = troopTypeVO;
				return troopTypeVO.AssetName;
			}
			if (type == SupplyType.SpecialAttack)
			{
				SpecialAttackTypeVO specialAttackTypeVO = Service.StaticDataController.Get<SpecialAttackTypeVO>(rewardUid);
				config = specialAttackTypeVO;
				return specialAttackTypeVO.AssetName;
			}
			config = null;
			supplyName = null;
			return null;
		}

		public static IGeometryVO GetIconVOFromObjective(ObjectiveVO obj, int objectiveHq)
		{
			GoalType goalType = Service.ObjectiveManager.GetGoalType(obj);
			return GameUtils.GetIconVOFromGoalType(goalType, obj.ObjIcon, objectiveHq);
		}

		public static IGeometryVO GetIconVOFromGoalType(GoalType objectiveType, string icon, int objectiveHq)
		{
			IGeometryVO geometryVO = null;
			StaticDataController staticDataController = Service.StaticDataController;
			switch (objectiveType)
			{
			case GoalType.Invalid:
				Service.Logger.ErrorFormat("Goal type not found for goal {0} {1} {2}", new object[]
				{
					objectiveType,
					icon,
					objectiveHq
				});
				break;
			case GoalType.Loot:
				geometryVO = UXUtils.GetDefaultCurrencyIconVO(icon);
				break;
			case GoalType.DestroyBuilding:
			case GoalType.DestroyBuildingType:
			case GoalType.DestroyBuildingID:
			case GoalType.ReceiveDonatedTroops:
			case GoalType.DonateTroopType:
			case GoalType.DonateTroopID:
			case GoalType.DonateTroop:
				geometryVO = staticDataController.GetOptional<BuildingTypeVO>(icon + objectiveHq);
				break;
			case GoalType.DeployTroop:
			case GoalType.DeployTroopType:
			case GoalType.DeployTroopID:
			case GoalType.TrainTroop:
			case GoalType.TrainTroopType:
			case GoalType.TrainTroopID:
				geometryVO = staticDataController.GetOptional<TroopTypeVO>(icon + objectiveHq);
				break;
			case GoalType.DeploySpecialAttack:
			case GoalType.DeploySpecialAttackID:
			case GoalType.TrainSpecialAttack:
			case GoalType.TrainSpecialAttackID:
				geometryVO = staticDataController.GetOptional<SpecialAttackTypeVO>(icon + objectiveHq);
				break;
			}
			if (geometryVO == null && objectiveHq != 1)
			{
				geometryVO = GameUtils.GetIconVOFromGoalType(objectiveType, icon, 1);
			}
			if (geometryVO == null)
			{
				Service.Logger.ErrorFormat("Could not find icon for type {1}, objIcon {2}, lvl {3}", new object[]
				{
					objectiveType,
					icon,
					objectiveHq
				});
			}
			return geometryVO;
		}

		public static int GetShardQualityNumeric(CrateSupplyVO supplyVO)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			SupplyType type = supplyVO.Type;
			if (type == SupplyType.ShardTroop || type == SupplyType.ShardSpecialAttack)
			{
				ShardVO shardVO = staticDataController.Get<ShardVO>(supplyVO.RewardUid);
				return (int)shardVO.Quality;
			}
			if (type != SupplyType.Shard)
			{
				return -1;
			}
			EquipmentVO currentEquipmentDataByID = ArmoryUtils.GetCurrentEquipmentDataByID(supplyVO.RewardUid);
			return (int)currentEquipmentDataByID.Quality;
		}

		public static int GetUpgradeShardsOwned(CrateSupplyVO supplyVO, CurrentPlayer player)
		{
			if (!player.Shards.ContainsKey(supplyVO.RewardUid))
			{
				return 0;
			}
			return player.Shards[supplyVO.RewardUid];
		}

		public static int GetUpgradeShardsRequired(CrateSupplyVO supplyVO, CurrentPlayer player, StaticDataController idc)
		{
			int level = 1;
			SupplyType type = supplyVO.Type;
			if (type != SupplyType.ShardTroop)
			{
				if (type != SupplyType.ShardSpecialAttack)
				{
					if (type != SupplyType.Shard)
					{
						Service.Logger.WarnFormat("Unexpected CrateSupply data passed.  No shards required for upgrade.{0} - {1}", new object[]
						{
							supplyVO.Uid,
							supplyVO.Type
						});
						return 101;
					}
					string rewardUid = supplyVO.RewardUid;
					if (player.UnlockedLevels.Equipment.Has(rewardUid))
					{
						level = player.UnlockedLevels.Equipment.GetNextLevel(rewardUid);
					}
					EquipmentVO byLevel = Service.EquipmentUpgradeCatalog.GetByLevel(rewardUid, level);
					if (byLevel == null)
					{
						return 0;
					}
					return byLevel.UpgradeShards;
				}
				else
				{
					string rewardUid2 = supplyVO.RewardUid;
					if (player.UnlockedLevels.Starships.Has(rewardUid2))
					{
						level = player.UnlockedLevels.Starships.GetNextLevel(rewardUid2);
					}
					ShardVO shardVO = idc.Get<ShardVO>(supplyVO.RewardUid);
					SpecialAttackTypeVO byLevel2 = Service.StarshipUpgradeCatalog.GetByLevel(shardVO.TargetGroupId, level);
					if (byLevel2 == null)
					{
						return 0;
					}
					return byLevel2.UpgradeShardCount;
				}
			}
			else
			{
				string rewardUid3 = supplyVO.RewardUid;
				if (player.UnlockedLevels.Troops.Has(rewardUid3))
				{
					level = player.UnlockedLevels.Troops.GetNextLevel(rewardUid3);
				}
				ShardVO shardVO2 = idc.Get<ShardVO>(supplyVO.RewardUid);
				TroopTypeVO byLevel3 = Service.TroopUpgradeCatalog.GetByLevel(shardVO2.TargetGroupId, level);
				if (byLevel3 == null)
				{
					return 0;
				}
				return byLevel3.UpgradeShardCount;
			}
		}

		public static string GetShardShopNameWithoutQuantity(CrateSupplyVO supply, StaticDataController idc)
		{
			string result = string.Empty;
			if (supply == null)
			{
				Service.Logger.Error("Argument cannot be null (ShardShopName)");
				return result;
			}
			SupplyType type = supply.Type;
			if (type != SupplyType.ShardSpecialAttack)
			{
				if (type != SupplyType.ShardTroop)
				{
					if (type == SupplyType.Shard)
					{
						EquipmentVO currentEquipmentDataByID = ArmoryUtils.GetCurrentEquipmentDataByID(supply.RewardUid);
						result = LangUtils.GetEquipmentDisplayName(currentEquipmentDataByID);
					}
				}
				else
				{
					ShardVO optional = idc.GetOptional<ShardVO>(supply.RewardUid);
					IDeployableVO deployableVOFromShard = Service.DeployableShardUnlockController.GetDeployableVOFromShard(optional);
					result = LangUtils.GetTroopDisplayName((TroopTypeVO)deployableVOFromShard);
				}
			}
			else
			{
				ShardVO optional2 = idc.GetOptional<ShardVO>(supply.RewardUid);
				IDeployableVO deployableVOFromShard2 = Service.DeployableShardUnlockController.GetDeployableVOFromShard(optional2);
				result = LangUtils.GetStarshipDisplayName((SpecialAttackTypeVO)deployableVOFromShard2);
			}
			return result;
		}

		public static string GetRewardSupplyName(CrateSupplyVO supply, int hqLevel)
		{
			InventoryCrateRewardController inventoryCrateRewardController = Service.InventoryCrateRewardController;
			SupplyType type = supply.Type;
			string result;
			if (type == SupplyType.ShardSpecialAttack || type == SupplyType.ShardTroop)
			{
				StaticDataController staticDataController = Service.StaticDataController;
				ShardVO optional = staticDataController.GetOptional<ShardVO>(supply.RewardUid);
				IDeployableVO deployableVOFromShard = Service.DeployableShardUnlockController.GetDeployableVOFromShard(optional);
				int rewardAmount = Service.InventoryCrateRewardController.GetRewardAmount(supply, hqLevel);
				result = GameUtils.GetShardUnlockSupplyName(deployableVOFromShard, optional, rewardAmount);
			}
			else
			{
				RewardVO vo = inventoryCrateRewardController.GenerateRewardFromSupply(supply, hqLevel);
				result = Service.RewardManager.GetRewardStringWithLevel(vo, Service.Lang.Get("SupplyRewardFormat", new object[0]));
			}
			return result;
		}

		public static IGeometryVO GetIconVOFromCrateSupply(CrateSupplyVO supply, int playerHq)
		{
			IGeometryVO geometryVO = null;
			string text = null;
			GameUtils.GetInventorySupplyIconAssetName(supply, out geometryVO, out text, playerHq);
			if (geometryVO != null)
			{
				return geometryVO;
			}
			return GameUtils.GetIconVOFromShardCrateSupply(supply, out text);
		}

		private static string GetShardUnlockSupplyName(IDeployableVO config, ShardVO shard, int amount)
		{
			Lang lang = Service.Lang;
			string text = null;
			if (config != null && shard != null)
			{
				if (shard.TargetType == "specialAttack")
				{
					text = LangUtils.GetStarshipDisplayName((SpecialAttackTypeVO)config);
				}
				else
				{
					text = LangUtils.GetTroopDisplayName((TroopTypeVO)config);
				}
			}
			if (amount <= 0)
			{
				return text;
			}
			string format = lang.Get("SupplyRewardFormat", new object[0]);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(format, text, lang.ThousandsSeparated(Convert.ToInt32(amount)));
			return stringBuilder.ToString();
		}

		private static IGeometryVO GetIconVOFromShardCrateSupply(CrateSupplyVO supply, out string supplyName)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			ShardVO optional = staticDataController.GetOptional<ShardVO>(supply.RewardUid);
			IDeployableVO deployableVOFromShard = Service.DeployableShardUnlockController.GetDeployableVOFromShard(optional);
			supplyName = GameUtils.GetShardUnlockSupplyName(deployableVOFromShard, optional, 0);
			return deployableVOFromShard;
		}

		public static bool HasUserFactionFlipped(CurrentPlayer player)
		{
			return player.NumIdentities > 1;
		}

		public static void SwapShaderIfNeeded(string[] swapList, Shader swapSrc, Material sharedMaterial)
		{
			string b = string.Empty;
			if (sharedMaterial.shader != null)
			{
				b = sharedMaterial.shader.name;
				int num = swapList.Length;
				for (int i = 0; i < num; i++)
				{
					if (swapList[i] == b)
					{
						sharedMaterial.shader = swapSrc;
						break;
					}
				}
			}
			else
			{
				Service.Logger.Warn("Material Shader NULL: " + sharedMaterial.name);
			}
		}

		public static bool SafeVOEqualityValidation(IValueObject lhs, IValueObject rhs)
		{
			return lhs == rhs || (rhs != null && lhs != null && lhs.Uid == rhs.Uid);
		}

		public static void MultiplyCurrency(float multiplier, ref int credits, ref int materials, ref int contraband)
		{
			if (multiplier != 1f)
			{
				credits = Mathf.FloorToInt((float)credits * multiplier);
				materials = Mathf.FloorToInt((float)materials * multiplier);
				contraband = Mathf.FloorToInt((float)contraband * multiplier);
			}
		}

		public static Dictionary<string, int> GetHQScaledCostForPlayer(string[] cost)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			int hqLevel = currentPlayer.Map.FindHighestHqLevel();
			return GameUtils.GetHQScaledCost(cost, hqLevel);
		}

		public static Dictionary<string, int> GetHQScaledCost(string[] cost, int hqLevel)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			StaticDataController staticDataController = Service.StaticDataController;
			int num = cost.Length;
			for (int i = 0; i < num; i++)
			{
				string[] array = cost[i].Split(new char[]
				{
					':'
				});
				if (array.Length == 2)
				{
					string key = array[0];
					string scalingUid = array[1];
					int hQScaledValue = GameUtils.GetHQScaledValue(staticDataController, scalingUid, hqLevel);
					if (hQScaledValue > 0)
					{
						dictionary[key] = hQScaledValue;
					}
				}
			}
			return dictionary;
		}

		private static int GetHQScaledValue(StaticDataController sdc, string scalingUid, int hqLevel)
		{
			CrateSupplyScaleVO crateSupplyScaleVO = sdc.Get<CrateSupplyScaleVO>(scalingUid);
			return crateSupplyScaleVO.GetHQScaling(hqLevel);
		}

		public static string GetSquadLevelUIDFromLevel(int level)
		{
			string str = "SquadLevel";
			return str + level.ToString();
		}

		public static string GetSquadLevelUIDFromSquad(Squad squad)
		{
			int level = 1;
			if (squad != null)
			{
				level = squad.Level;
			}
			else
			{
				Service.Logger.Warn("GameUtils.GetSquadLevelUIDFromSquad called with null Squad");
			}
			return GameUtils.GetSquadLevelUIDFromLevel(level);
		}

		public static int GetReputationCapacityForLevel(int squadCenterLevel)
		{
			string sQUADPERK_REPUTATION_MAX_LIMIT = GameConstants.SQUADPERK_REPUTATION_MAX_LIMIT;
			if (!string.IsNullOrEmpty(sQUADPERK_REPUTATION_MAX_LIMIT) && squadCenterLevel > 0)
			{
				string[] array = sQUADPERK_REPUTATION_MAX_LIMIT.Split(new char[]
				{
					' '
				});
				if (array.Length > squadCenterLevel - 1)
				{
					return Convert.ToInt32(array[squadCenterLevel - 1]);
				}
			}
			return 0;
		}

		public static int GetSquadLevelFromInvestedRep(int investedRep)
		{
			int num;
			if (Service.PerkManager != null)
			{
				num = Service.PerkManager.SquadLevelMax;
			}
			else
			{
				num = Service.StaticDataController.GetAll<SquadLevelVO>().Count;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			int result = 0;
			for (int i = 1; i <= num; i++)
			{
				string squadLevelUIDFromLevel = GameUtils.GetSquadLevelUIDFromLevel(i);
				SquadLevelVO squadLevelVO = staticDataController.Get<SquadLevelVO>(squadLevelUIDFromLevel);
				if (squadLevelVO.RepThreshold > investedRep)
				{
					break;
				}
				result = squadLevelVO.Level;
			}
			return result;
		}

		public static bool IsPerkCommandStatusFatal(uint status)
		{
			return status != 2504u && status != 2502u;
		}

		public static PerkVO GetPerkByGroupAndTier(string perkGroup, int perkTier)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			return staticDataController.Get<PerkVO>("perk_" + perkGroup + perkTier.ToString());
		}

		public static CrateData GetNextInventoryCrateToExpire(InventoryCrates crates, uint minThresholdTime)
		{
			Dictionary<string, CrateData> available = crates.Available;
			CrateData crateData = null;
			foreach (CrateData current in available.Values)
			{
				if (current.DoesExpire)
				{
					uint expiresTimeStamp = current.ExpiresTimeStamp;
					if (expiresTimeStamp != 0u && expiresTimeStamp >= minThresholdTime)
					{
						if (crateData == null)
						{
							crateData = current;
						}
						else if (crateData.ExpiresTimeStamp > expiresTimeStamp)
						{
							crateData = current;
						}
					}
				}
			}
			return crateData;
		}

		public static bool IsUnlockBlockingScreenOpen()
		{
			StoreScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<StoreScreen>();
			if (highestLevelScreen != null && !highestLevelScreen.IsShardShopModalVisible())
			{
				return true;
			}
			PrizeInventoryScreen highestLevelScreen2 = Service.ScreenController.GetHighestLevelScreen<PrizeInventoryScreen>();
			return highestLevelScreen2 != null || Service.InventoryCrateRewardController.IsCrateAnimationShowingOrPending;
		}

		public static bool CanShardUnlockCelebrationPlayImmediately()
		{
			StoreScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<StoreScreen>();
			return highestLevelScreen != null && highestLevelScreen.IsShardShopModalVisible();
		}

		public static void CloseStoreOrInventoryScreen()
		{
			StoreScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<StoreScreen>();
			if (highestLevelScreen != null)
			{
				highestLevelScreen.Close(null);
				return;
			}
			PrizeInventoryScreen highestLevelScreen2 = Service.ScreenController.GetHighestLevelScreen<PrizeInventoryScreen>();
			if (highestLevelScreen2 != null)
			{
				highestLevelScreen2.Close(null);
			}
		}

		public static ArmorType DeduceArmorType(SmartEntity target)
		{
			if (target.TroopComp != null)
			{
				return target.TroopComp.TroopType.ArmorType;
			}
			if (target.BuildingComp != null)
			{
				return target.BuildingComp.BuildingType.ArmorType;
			}
			if (target.TroopShieldComp != null && target.TroopShieldComp.IsActive())
			{
				return ArmorType.Shield;
			}
			return ArmorType.Invalid;
		}

		public static string GetVFXAssetForTarget(BuffTypeVO buffTypeVO, SmartEntity target)
		{
			if (string.IsNullOrEmpty(buffTypeVO.AssetProfile))
			{
				return buffTypeVO.AssetName;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			VFXProfileVO optional = staticDataController.GetOptional<VFXProfileVO>(buffTypeVO.AssetProfile);
			if (optional == null)
			{
				return null;
			}
			ArmorType armorType = GameUtils.DeduceArmorType(target);
			return optional.Values[(int)armorType];
		}

		public static bool HandleCrateSupplyRewardClicked(CrateSupplyVO crateSupply)
		{
			return GameUtils.HandleCrateSupplyRewardClicked(crateSupply, true);
		}

		public static bool HandleCrateSupplyRewardClicked(CrateSupplyVO crateSupply, bool showUpgrade)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			DeployableShardUnlockController deployableShardUnlockController = Service.DeployableShardUnlockController;
			IDeployableVO deployableVO = null;
			EquipmentVO equipmentVO = null;
			switch (crateSupply.Type)
			{
			case SupplyType.Shard:
				equipmentVO = ArmoryUtils.GetCurrentEquipmentDataByID(crateSupply.RewardUid);
				break;
			case SupplyType.Troop:
			case SupplyType.Hero:
				deployableVO = staticDataController.Get<TroopTypeVO>(crateSupply.RewardUid);
				break;
			case SupplyType.SpecialAttack:
				deployableVO = staticDataController.Get<SpecialAttackTypeVO>(crateSupply.RewardUid);
				break;
			case SupplyType.ShardTroop:
			case SupplyType.ShardSpecialAttack:
			{
				ShardVO shard = staticDataController.Get<ShardVO>(crateSupply.RewardUid);
				deployableVO = deployableShardUnlockController.GetDeployableVOFromShard(shard);
				break;
			}
			}
			SmartEntity availableTroopResearchLab = Service.BuildingLookupController.GetAvailableTroopResearchLab();
			if (deployableVO != null)
			{
				Service.EventManager.SendEvent(EventId.LootTableUnitInfoTapped, deployableVO.Uid);
				TroopUpgradeTag troopUpgradeTag = new TroopUpgradeTag(deployableVO, true);
				bool showUpgradeControls = showUpgrade && !string.IsNullOrEmpty(troopUpgradeTag.Troop.UpgradeShardUid);
				DeployableInfoScreen screen = new DeployableInfoScreen(troopUpgradeTag, null, showUpgradeControls, availableTroopResearchLab);
				Service.ScreenController.AddScreen(screen);
				return true;
			}
			if (equipmentVO != null)
			{
				EquipmentInfoScreen screen2 = new EquipmentInfoScreen(equipmentVO, null, availableTroopResearchLab, showUpgrade, false);
				Service.ScreenController.AddScreen(screen2);
				return true;
			}
			return false;
		}

		public static uint GetDaysSinceInstall()
		{
			uint firstLoginTime = Service.CurrentPlayer.FirstLoginTime;
			uint num = ServerTime.Time - firstLoginTime;
			return num / 86400u;
		}

		public static string GetShardSlotId(int index)
		{
			return "pool_" + index;
		}

		public static int GetShardCountToReachMaxLevel(string shardId, SupplyType type)
		{
			int result = 0;
			DeployableShardUnlockController deployableShardUnlockController = Service.DeployableShardUnlockController;
			StaticDataController staticDataController = Service.StaticDataController;
			if (type == SupplyType.ShardTroop)
			{
				ShardVO shardVO = staticDataController.Get<ShardVO>(shardId);
				string targetGroupId = shardVO.TargetGroupId;
				result = deployableShardUnlockController.CalcNumShardsForDeployableToReachMaxLevelInGroup<TroopTypeVO>(targetGroupId);
			}
			else if (type == SupplyType.ShardSpecialAttack)
			{
				ShardVO shardVO2 = staticDataController.Get<ShardVO>(shardId);
				string targetGroupId2 = shardVO2.TargetGroupId;
				result = deployableShardUnlockController.CalcNumShardsForDeployableToReachMaxLevelInGroup<SpecialAttackTypeVO>(targetGroupId2);
			}
			else if (type == SupplyType.Shard)
			{
				EquipmentUpgradeCatalog equipmentUpgradeCatalog = Service.EquipmentUpgradeCatalog;
				EquipmentVO maxLevel = equipmentUpgradeCatalog.GetMaxLevel(shardId);
				int maxLevel2 = (maxLevel == null) ? 0 : maxLevel.Lvl;
				result = ArmoryUtils.GetNumEquipmentShardsToReachLevel(equipmentUpgradeCatalog, shardId, 0, maxLevel2);
			}
			return result;
		}

		public static int GetSpentShardCount(string shardId, SupplyType supplyType)
		{
			int num = 0;
			DeployableShardUnlockController deployableShardUnlockController = Service.DeployableShardUnlockController;
			StaticDataController staticDataController = Service.StaticDataController;
			string text = string.Empty;
			DeliveryType deliveryType = DeliveryType.Invalid;
			if (supplyType == SupplyType.ShardTroop)
			{
				ShardVO shardVO = staticDataController.Get<ShardVO>(shardId);
				string targetType = shardVO.TargetType;
				text = shardVO.TargetGroupId;
				deliveryType = DeliveryType.UpgradeTroop;
				int upgradeLevelOfDeployable = deployableShardUnlockController.GetUpgradeLevelOfDeployable(targetType, text);
				num = deployableShardUnlockController.GetNumShardsForDeployableToReachLevelInGroup<TroopTypeVO>(0, upgradeLevelOfDeployable, text);
			}
			else if (supplyType == SupplyType.ShardSpecialAttack)
			{
				ShardVO shardVO2 = staticDataController.Get<ShardVO>(shardId);
				string targetType2 = shardVO2.TargetType;
				text = shardVO2.TargetGroupId;
				deliveryType = DeliveryType.UpgradeStarship;
				int upgradeLevelOfDeployable2 = deployableShardUnlockController.GetUpgradeLevelOfDeployable(targetType2, text);
				num = deployableShardUnlockController.GetNumShardsForDeployableToReachLevelInGroup<SpecialAttackTypeVO>(0, upgradeLevelOfDeployable2, text);
			}
			else if (supplyType == SupplyType.Shard)
			{
				LevelMap equipment = Service.CurrentPlayer.UnlockedLevels.Equipment;
				text = shardId;
				deliveryType = DeliveryType.UpgradeEquipment;
				int maxLevel = 0;
				if (equipment.Has(shardId))
				{
					maxLevel = equipment.GetLevel(shardId);
				}
				num = ArmoryUtils.GetNumEquipmentShardsToReachLevel(Service.EquipmentUpgradeCatalog, shardId, 0, maxLevel);
			}
			List<Contract> list = Service.ISupportController.FindAllContractsOfType(ContractType.Research);
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (list[i].DeliveryType == deliveryType)
				{
					string productUid = list[i].ProductUid;
					if (supplyType == SupplyType.ShardTroop)
					{
						TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(productUid);
						if (troopTypeVO.TroopID == text)
						{
							num += troopTypeVO.UpgradeShardCount;
							break;
						}
					}
					else if (supplyType == SupplyType.ShardSpecialAttack)
					{
						SpecialAttackTypeVO specialAttackTypeVO = staticDataController.Get<SpecialAttackTypeVO>(productUid);
						if (specialAttackTypeVO.SpecialAttackID == text)
						{
							num += specialAttackTypeVO.UpgradeShardCount;
							break;
						}
					}
					else if (supplyType == SupplyType.Shard)
					{
						EquipmentVO equipmentVO = staticDataController.Get<EquipmentVO>(productUid);
						if (equipmentVO.EquipmentID == text)
						{
							num += equipmentVO.UpgradeShards;
							break;
						}
					}
				}
			}
			return num;
		}
	}
}
