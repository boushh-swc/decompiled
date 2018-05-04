using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.TrapConditions;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Commands.Player.Building.Rearm;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Utils
{
	public static class TrapUtils
	{
		public static uint GetTrapMaxRadius(TrapTypeVO trapType)
		{
			List<TrapCondition> parsedTrapConditions = trapType.ParsedTrapConditions;
			for (int i = 0; i < parsedTrapConditions.Count; i++)
			{
				if (parsedTrapConditions[i] is RadiusTrapCondition)
				{
					return ((RadiusTrapCondition)parsedTrapConditions[i]).Radius;
				}
			}
			return 0u;
		}

		public static uint GetTrapAttackRadius(TrapTypeVO trapType)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			TrapEventType eventType = trapType.EventType;
			if (eventType == TrapEventType.SpecialAttack)
			{
				string specialAttackName = trapType.ShipTED.SpecialAttackName;
				SpecialAttackTypeVO specialAttackTypeVO = staticDataController.Get<SpecialAttackTypeVO>(specialAttackName);
				ProjectileTypeVO projectileType = specialAttackTypeVO.ProjectileType;
				return (uint)projectileType.SplashRadius;
			}
			if (eventType != TrapEventType.Turret)
			{
				return 0u;
			}
			string turretUid = trapType.TurretTED.TurretUid;
			TurretTypeVO turretTypeVO = staticDataController.Get<TurretTypeVO>(turretUid);
			return turretTypeVO.MaxAttackRange;
		}

		public static List<AddOnMapping> ParseAddons(string rawString)
		{
			List<AddOnMapping> list = null;
			if (!string.IsNullOrEmpty(rawString))
			{
				string text = rawString.TrimEnd(new char[]
				{
					' '
				});
				string[] array = text.Split(new char[]
				{
					' '
				});
				int i = 0;
				int num = array.Length;
				while (i < num)
				{
					string[] array2 = array[i].Split(new char[]
					{
						':'
					});
					if (list == null)
					{
						list = new List<AddOnMapping>();
					}
					list.Add(new AddOnMapping(array2[0], array2[1]));
					i++;
				}
			}
			return list;
		}

		public static List<TrapCondition> ParseConditions(string rawString)
		{
			List<TrapCondition> list = new List<TrapCondition>();
			string text = rawString.Replace(" ", string.Empty);
			string[] array = text.Split(new char[]
			{
				'&'
			});
			int i = 0;
			int num = array.Length;
			while (i < num)
			{
				string text2 = array[i];
				string[] array2 = text2.Split(new char[]
				{
					'('
				});
				array2[0] = array2[0].ToLower();
				array2[1] = array2[1].Substring(0, array2[1].Length - 1);
				string[] args = array2[1].Split(new char[]
				{
					','
				});
				list.Add(TrapConditionFactory.GenerateTrapCondition(array2[0], args));
				i++;
			}
			return list;
		}

		public static ITrapEventData ParseEventData(TrapEventType type, string rawData)
		{
			if (type == TrapEventType.SpecialAttack)
			{
				return new SpecialAttackTrapEventData().Init(rawData);
			}
			if (type != TrapEventType.Turret)
			{
				return null;
			}
			return new TurretTrapEventData().Init(rawData);
		}

		public static void GetRearmAllTrapsCost(out int creditsCost, out int materialsCost, out int contrabandCost)
		{
			creditsCost = 0;
			materialsCost = 0;
			contrabandCost = 0;
			StaticDataController staticDataController = Service.StaticDataController;
			List<Entity> rearmableTraps = TrapUtils.GetRearmableTraps();
			int i = 0;
			int count = rearmableTraps.Count;
			while (i < count)
			{
				TrapTypeVO trapTypeVO = staticDataController.Get<TrapTypeVO>(rearmableTraps[i].Get<BuildingComponent>().BuildingType.TrapUid);
				creditsCost += trapTypeVO.RearmCreditsCost;
				materialsCost += trapTypeVO.RearmMaterialsCost;
				contrabandCost += trapTypeVO.RearmContrabandCost;
				i++;
			}
		}

		public static List<Entity> GetRearmableTraps()
		{
			List<Entity> list = new List<Entity>();
			NodeList<TrapNode> trapNodeList = Service.BuildingLookupController.TrapNodeList;
			for (TrapNode trapNode = trapNodeList.Head; trapNode != null; trapNode = trapNode.Next)
			{
				if (trapNode.TrapComp.CurrentState == TrapState.Spent && !ContractUtils.IsBuildingUpgrading(trapNode.Entity))
				{
					list.Add(trapNode.Entity);
				}
			}
			return list;
		}

		public static void RearmAllTraps()
		{
			int credits;
			int materials;
			int contraband;
			TrapUtils.GetRearmAllTrapsCost(out credits, out materials, out contraband);
			if (!GameUtils.CanAffordCredits(credits) || !GameUtils.CanAffordMaterials(materials) || !GameUtils.CanAffordContraband(contraband))
			{
				PayMeScreen.ShowIfNotEnoughCurrency(credits, materials, contraband, "Rearm_All_Traps", new OnScreenModalResult(TrapUtils.OnPayMeForCurrencyResultForMultiTrap));
				return;
			}
			List<Entity> rearmableTraps = TrapUtils.GetRearmableTraps();
			int i = 0;
			int count = rearmableTraps.Count;
			while (i < count)
			{
				TrapUtils.RearmTrapForClient(rearmableTraps[i]);
				i++;
			}
			TrapUtils.SendRearmTrapServerCommand(rearmableTraps);
		}

		public static void RearmSingleTrap(Entity selectedBuilding)
		{
			bool flag = TrapUtils.RearmTrapForClient(selectedBuilding);
			if (flag)
			{
				TrapUtils.SendRearmTrapServerCommand(new List<Entity>
				{
					selectedBuilding
				});
			}
		}

		public static bool RearmTrapForClient(Entity selectedBuilding)
		{
			BuildingTypeVO buildingType = selectedBuilding.Get<BuildingComponent>().BuildingType;
			TrapTypeVO trapTypeVO = Service.StaticDataController.Get<TrapTypeVO>(buildingType.TrapUid);
			int rearmCreditsCost = trapTypeVO.RearmCreditsCost;
			int rearmMaterialsCost = trapTypeVO.RearmMaterialsCost;
			int rearmContrabandCost = trapTypeVO.RearmContrabandCost;
			if (!GameUtils.CanAffordCredits(rearmCreditsCost) || !GameUtils.CanAffordMaterials(rearmMaterialsCost))
			{
				PayMeScreen.ShowIfNotEnoughCurrency(rearmCreditsCost, rearmMaterialsCost, rearmContrabandCost, GameUtils.GetBuildingPurchaseContext(buildingType, null, false, false), new OnScreenModalResult(TrapUtils.OnPayMeForCurrencyResultSingleTrap));
				return false;
			}
			GameUtils.SpendCurrency(rearmCreditsCost, rearmMaterialsCost, rearmContrabandCost, true);
			Service.TrapController.SetTrapState(selectedBuilding.Get<TrapComponent>(), TrapState.Armed);
			selectedBuilding.Get<BuildingComponent>().BuildingTO.CurrentStorage = 1;
			Service.UXController.HUD.ShowContextButtons(selectedBuilding);
			Service.BuildingController.RedrawRadiusForSelectedBuilding();
			return true;
		}

		public static void OnPayMeForCurrencyResultSingleTrap(object result, object cookie)
		{
			Entity selectedBuilding = Service.BuildingController.SelectedBuilding;
			if (GameUtils.HandleSoftCurrencyFlow(result, cookie))
			{
				TrapUtils.RearmSingleTrap(selectedBuilding);
			}
		}

		public static void OnPayMeForCurrencyResultForMultiTrap(object result, object cookie)
		{
			if (GameUtils.HandleSoftCurrencyFlow(result, cookie))
			{
				TrapUtils.RearmAllTraps();
			}
		}

		public static void SendRearmTrapServerCommand(List<Entity> buildings)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < buildings.Count; i++)
			{
				list.Add(buildings[i].Get<BuildingComponent>().BuildingTO.Key);
			}
			RearmTrapCommand command = new RearmTrapCommand(new RearmTrapRequest
			{
				BuildingIds = list,
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			Service.ServerAPI.Sync(command);
		}

		public static bool IsCurrentPlayerInDefensiveBattle(IState gameState)
		{
			bool flag = gameState is BattleStartState || gameState is BattlePlayState;
			string playerId = Service.CurrentPlayer.PlayerId;
			return (gameState is BattlePlaybackState && Service.BattlePlaybackController.CurrentBattleEntry.DefenderID == playerId) || (flag && Service.BattleController.CurrentPlayerTeamType == TeamType.Defender);
		}
	}
}
