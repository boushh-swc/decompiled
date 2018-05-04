using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.TrapConditions;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Commands.Player.Building.Rearm;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StaRTS.Main.Utils
{
	public static class TrapUtils
	{
		[CompilerGenerated]
		private static OnScreenModalResult <>f__mg$cache0;

		[CompilerGenerated]
		private static OnScreenModalResult <>f__mg$cache1;

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
			if (eventType == TrapEventType.Turret)
			{
				string turretUid = trapType.TurretTED.TurretUid;
				TurretTypeVO turretTypeVO = staticDataController.Get<TurretTypeVO>(turretUid);
				return turretTypeVO.MaxAttackRange;
			}
			if (eventType != TrapEventType.SpecialAttack)
			{
				return 0u;
			}
			string specialAttackName = trapType.ShipTED.SpecialAttackName;
			SpecialAttackTypeVO specialAttackTypeVO = staticDataController.Get<SpecialAttackTypeVO>(specialAttackName);
			ProjectileTypeVO projectileType = specialAttackTypeVO.ProjectileType;
			return (uint)projectileType.SplashRadius;
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
			List<SmartEntity> rearmableTraps = TrapUtils.GetRearmableTraps();
			int i = 0;
			int count = rearmableTraps.Count;
			while (i < count)
			{
				TrapTypeVO trapTypeVO = staticDataController.Get<TrapTypeVO>(rearmableTraps[i].BuildingComp.BuildingType.TrapUid);
				creditsCost += trapTypeVO.RearmCreditsCost;
				materialsCost += trapTypeVO.RearmMaterialsCost;
				contrabandCost += trapTypeVO.RearmContrabandCost;
				i++;
			}
		}

		public static List<SmartEntity> GetRearmableTraps()
		{
			List<SmartEntity> list = new List<SmartEntity>();
			NodeList<TrapNode> trapNodeList = Service.BuildingLookupController.TrapNodeList;
			for (TrapNode trapNode = trapNodeList.Head; trapNode != null; trapNode = trapNode.Next)
			{
				SmartEntity smartEntity = (SmartEntity)trapNode.Entity;
				if (trapNode.TrapComp.CurrentState == TrapState.Spent && !ContractUtils.IsBuildingUpgrading(smartEntity))
				{
					list.Add(smartEntity);
				}
			}
			return list;
		}

		public static void RearmAllTraps()
		{
			int num;
			int num2;
			int num3;
			TrapUtils.GetRearmAllTrapsCost(out num, out num2, out num3);
			if (!GameUtils.CanAffordCredits(num) || !GameUtils.CanAffordMaterials(num2) || !GameUtils.CanAffordContraband(num3))
			{
				int arg_51_0 = num;
				int arg_51_1 = num2;
				int arg_51_2 = num3;
				string arg_51_3 = "Rearm_All_Traps";
				if (TrapUtils.<>f__mg$cache0 == null)
				{
					TrapUtils.<>f__mg$cache0 = new OnScreenModalResult(TrapUtils.OnPayMeForCurrencyResultForMultiTrap);
				}
				PayMeScreen.ShowIfNotEnoughCurrency(arg_51_0, arg_51_1, arg_51_2, arg_51_3, TrapUtils.<>f__mg$cache0);
				return;
			}
			List<SmartEntity> rearmableTraps = TrapUtils.GetRearmableTraps();
			int i = 0;
			int count = rearmableTraps.Count;
			while (i < count)
			{
				TrapUtils.RearmTrapForClient(rearmableTraps[i]);
				i++;
			}
			TrapUtils.SendRearmTrapServerCommand(rearmableTraps);
		}

		public static void RearmSingleTrap(SmartEntity selectedBuilding)
		{
			bool flag = TrapUtils.RearmTrapForClient(selectedBuilding);
			if (flag)
			{
				TrapUtils.SendRearmTrapServerCommand(new List<SmartEntity>
				{
					selectedBuilding
				});
			}
		}

		public static bool RearmTrapForClient(SmartEntity selectedBuilding)
		{
			BuildingTypeVO buildingType = selectedBuilding.BuildingComp.BuildingType;
			TrapTypeVO trapTypeVO = Service.StaticDataController.Get<TrapTypeVO>(buildingType.TrapUid);
			int rearmCreditsCost = trapTypeVO.RearmCreditsCost;
			int rearmMaterialsCost = trapTypeVO.RearmMaterialsCost;
			int rearmContrabandCost = trapTypeVO.RearmContrabandCost;
			if (!GameUtils.CanAffordCredits(rearmCreditsCost) || !GameUtils.CanAffordMaterials(rearmMaterialsCost))
			{
				int arg_73_0 = rearmCreditsCost;
				int arg_73_1 = rearmMaterialsCost;
				int arg_73_2 = rearmContrabandCost;
				string arg_73_3 = GameUtils.GetBuildingPurchaseContext(buildingType, null, false, false);
				if (TrapUtils.<>f__mg$cache1 == null)
				{
					TrapUtils.<>f__mg$cache1 = new OnScreenModalResult(TrapUtils.OnPayMeForCurrencyResultSingleTrap);
				}
				PayMeScreen.ShowIfNotEnoughCurrency(arg_73_0, arg_73_1, arg_73_2, arg_73_3, TrapUtils.<>f__mg$cache1);
				return false;
			}
			GameUtils.SpendCurrency(rearmCreditsCost, rearmMaterialsCost, rearmContrabandCost, true);
			Service.TrapController.SetTrapState(selectedBuilding.TrapComp, TrapState.Armed);
			selectedBuilding.BuildingComp.BuildingTO.CurrentStorage = 1;
			Service.UXController.HUD.ShowContextButtons(selectedBuilding);
			Service.BuildingController.RedrawRadiusForSelectedBuilding();
			return true;
		}

		public static void OnPayMeForCurrencyResultSingleTrap(object result, object cookie)
		{
			SmartEntity selectedBuilding = Service.BuildingController.SelectedBuilding;
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

		public static void SendRearmTrapServerCommand(List<SmartEntity> buildings)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < buildings.Count; i++)
			{
				list.Add(buildings[i].BuildingComp.BuildingTO.Key);
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
