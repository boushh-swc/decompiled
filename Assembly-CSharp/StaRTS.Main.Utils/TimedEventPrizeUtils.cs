using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player.SpecOps;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Utils
{
	public static class TimedEventPrizeUtils
	{
		private const string CONFLICT_PRIZE_CRATE_MULTIPLIER = "CONFLICT_PRIZE_CRATE_MULTIPLIER";

		private const string DATA_FRAG_ICON_QUALITY_FORMAT = "icoDataFragQ{0}";

		private static void GetCurrencyAmountToTransfer(string prizeID, ref int amountToTransfer, ref int amountRemaining)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			int num = 0;
			int num2 = 0;
			if (prizeID == "credits")
			{
				num = currentPlayer.CurrentCreditsAmount;
				num2 = currentPlayer.MaxCreditsAmount;
			}
			else if (prizeID == "materials")
			{
				num = currentPlayer.CurrentMaterialsAmount;
				num2 = currentPlayer.MaxMaterialsAmount;
			}
			else if (prizeID == "contraband")
			{
				num = currentPlayer.CurrentContrabandAmount;
				num2 = currentPlayer.MaxContrabandAmount;
			}
			else if (prizeID == "crystals")
			{
				num = currentPlayer.CurrentCrystalsAmount;
				num2 = 2147483647;
			}
			else if (prizeID == "reputation")
			{
				num = currentPlayer.CurrentReputationAmount;
				num2 = currentPlayer.MaxReputationAmount;
			}
			int num3 = num + amountToTransfer;
			if (num3 > num2)
			{
				amountRemaining = num3 - num2;
				amountToTransfer -= amountRemaining;
			}
		}

		public static int TransferPrizeFromInventory(PrizeType prizeType, string prizeID)
		{
			Lang lang = Service.Lang;
			StaticDataController staticDataController = Service.StaticDataController;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			PrizeInventory prizes = currentPlayer.Prizes;
			Inventory inventory = currentPlayer.Inventory;
			InventoryStorage inventoryStorage = null;
			IUpgradeableVO finalUnitFromPrize = TimedEventPrizeUtils.GetFinalUnitFromPrize(prizeType, prizeID);
			string text = null;
			string text2 = null;
			string id = null;
			int result = 0;
			int num = 0;
			switch (prizeType)
			{
			case PrizeType.Currency:
				num = prizes.GetResourceAmount(prizeID);
				TimedEventPrizeUtils.GetCurrencyAmountToTransfer(prizeID, ref num, ref result);
				if (num > 0)
				{
					inventory.ModifyItemAmount(prizeID, num);
					prizes.ModifyResourceAmount(prizeID, -num);
					text = "INVENTORY_REWARD_USED_CURRENCY";
					text2 = lang.Get(prizeID.ToUpper(), new object[0]);
				}
				else
				{
					id = "INVENTORY_NO_ROOM";
				}
				break;
			case PrizeType.Infantry:
			case PrizeType.Vehicle:
			case PrizeType.Mercenary:
				inventoryStorage = inventory.Troop;
				if (inventoryStorage.GetTotalStorageCapacity() >= inventoryStorage.GetTotalStorageAmount() + finalUnitFromPrize.Size)
				{
					num = 1;
					inventoryStorage.ModifyItemAmount(finalUnitFromPrize.Uid, num);
					prizes.ModifyTroopAmount(prizeID, -num);
					text = "INVENTORY_REWARD_USED_TROOP";
					text2 = LangUtils.GetTroopDisplayName((TroopTypeVO)finalUnitFromPrize);
					StorageSpreadUtils.UpdateAllStarportFullnessMeters();
				}
				else
				{
					id = "NOT_ENOUGH_HOUSING";
				}
				result = prizes.GetTroopAmount(prizeID);
				break;
			case PrizeType.Hero:
				inventoryStorage = inventory.Hero;
				if (!Service.BuildingLookupController.HasHeroCommand())
				{
					id = "INVENTORY_NO_HERO_COMMAND";
				}
				else if (inventoryStorage.GetTotalStorageCapacity() >= inventoryStorage.GetTotalStorageAmount() + finalUnitFromPrize.Size)
				{
					bool flag = false;
					foreach (KeyValuePair<string, InventoryEntry> current in inventoryStorage.GetInternalStorage())
					{
						if (current.Value.Amount > 0)
						{
							TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(current.Key);
							if (troopTypeVO.UpgradeGroup == finalUnitFromPrize.UpgradeGroup)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						flag = ContractUtils.HasExistingHeroContract(finalUnitFromPrize.UpgradeGroup);
					}
					if (!flag)
					{
						num = 1;
						inventoryStorage.ModifyItemAmount(finalUnitFromPrize.Uid, num);
						prizes.ModifyTroopAmount(prizeID, -num);
						text = "INVENTORY_REWARD_USED_HERO";
						text2 = LangUtils.GetTroopDisplayName((TroopTypeVO)finalUnitFromPrize);
						Service.EventManager.SendEvent(EventId.HeroMobilizedFromPrize, finalUnitFromPrize.Uid);
					}
					else
					{
						id = "INVENTORY_NO_ROOM_HERO_IN_QUEUE";
					}
				}
				else
				{
					id = "INVENTORY_NO_ROOM_HERO";
				}
				result = prizes.GetTroopAmount(prizeID);
				break;
			case PrizeType.SpecialAttack:
				inventoryStorage = inventory.SpecialAttack;
				if (!Service.BuildingLookupController.HasStarshipCommand())
				{
					id = "INVENTORY_NO_FLEET_COMMAND";
				}
				else if (inventoryStorage.GetTotalStorageCapacity() >= inventoryStorage.GetTotalStorageAmount() + finalUnitFromPrize.Size)
				{
					num = 1;
					inventoryStorage.ModifyItemAmount(finalUnitFromPrize.Uid, num);
					prizes.ModifySpecialAttackAmount(prizeID, -num);
					text = "INVENTORY_REWARD_USED_TROOP";
					text2 = LangUtils.GetStarshipDisplayName((SpecialAttackTypeVO)finalUnitFromPrize);
					Service.EventManager.SendEvent(EventId.StarshipMobilizedFromPrize, finalUnitFromPrize.Uid);
				}
				else
				{
					id = "NOT_ENOUGH_SPACE";
				}
				result = prizes.GetSpecialAttackAmount(prizeID);
				break;
			}
			if (num > 0)
			{
				InventoryTransferRequest request = new InventoryTransferRequest(prizeID, num);
				Service.ServerAPI.Enqueue(new InventoryTransferCommand(request));
			}
			else
			{
				string message = lang.Get(id, new object[0]);
				AlertScreen.ShowModal(false, null, message, null, null);
			}
			if (text != null)
			{
				string text3 = (prizeType != PrizeType.Hero) ? lang.Get("AMOUNT_AND_NAME", new object[]
				{
					num,
					text2
				}) : text2;
				string instructions = lang.Get(text, new object[]
				{
					text3
				});
				Service.UXController.MiscElementsManager.ShowPlayerInstructions(instructions, 1f, 2f);
			}
			return result;
		}

		public static IUpgradeableVO GetFinalUnitFromPrize(PrizeType prizeType, string prizeID)
		{
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			IUpgradeableVO result = null;
			TroopUpgradeCatalog troopUpgradeCatalog = Service.TroopUpgradeCatalog;
			switch (prizeType)
			{
			case PrizeType.Infantry:
			{
				int val = buildingLookupController.GetHighestLevelForBarracks();
				int lvl = troopUpgradeCatalog.GetMaxRewardableLevel(prizeID).Lvl;
				int level = Math.Min(val, lvl);
				result = troopUpgradeCatalog.GetByLevel(prizeID, level);
				break;
			}
			case PrizeType.Hero:
			{
				int val = buildingLookupController.GetHighestLevelForHeroCommands();
				int lvl = troopUpgradeCatalog.GetMaxRewardableLevel(prizeID).Lvl;
				int level = Math.Min(val, lvl);
				result = troopUpgradeCatalog.GetByLevel(prizeID, level);
				break;
			}
			case PrizeType.SpecialAttack:
			{
				StarshipUpgradeCatalog starshipUpgradeCatalog = Service.StarshipUpgradeCatalog;
				int val = buildingLookupController.GetHighestLevelForStarshipCommands();
				int lvl = starshipUpgradeCatalog.GetMaxRewardableLevel(prizeID).Lvl;
				int level = Math.Min(val, lvl);
				result = starshipUpgradeCatalog.GetByLevel(prizeID, level);
				break;
			}
			case PrizeType.Vehicle:
			{
				int val = buildingLookupController.GetHighestLevelForFactories();
				int lvl = troopUpgradeCatalog.GetMaxRewardableLevel(prizeID).Lvl;
				int level = Math.Min(val, lvl);
				result = troopUpgradeCatalog.GetByLevel(prizeID, level);
				break;
			}
			case PrizeType.Mercenary:
			{
				int val = buildingLookupController.GetHighestLevelForCantinas();
				int lvl = troopUpgradeCatalog.GetMaxRewardableLevel(prizeID).Lvl;
				int level = Math.Min(val, lvl);
				result = troopUpgradeCatalog.GetByLevel(prizeID, level);
				break;
			}
			}
			return result;
		}

		public static Dictionary<string, TournamentRewardsVO> GetTierRewardMap(string rewardGroupId)
		{
			Dictionary<string, TournamentRewardsVO> dictionary = new Dictionary<string, TournamentRewardsVO>();
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (TournamentRewardsVO current in staticDataController.GetAll<TournamentRewardsVO>())
			{
				if (current.TournamentRewardsId == rewardGroupId)
				{
					dictionary.Add(current.TournamentTier, current);
				}
			}
			return dictionary;
		}

		public static bool TrySetupConflictEndedRewardView(List<string> rewardUids, UXLabel label, UXSprite sprite)
		{
			if (rewardUids == null || rewardUids.Count == 0)
			{
				return false;
			}
			CrateVO optional = Service.StaticDataController.GetOptional<CrateVO>(rewardUids[0]);
			if (optional == null)
			{
				return false;
			}
			string uid = optional.Uid;
			label.Text = LangUtils.GetCrateDisplayName(uid);
			RewardUtils.SetCrateIcon(sprite, optional, AnimState.Closed);
			return true;
		}

		public static void TrySetupConflictItemRewardView(TournamentRewardsVO rewardGroup, UXLabel prizeLabel, UXSprite iconSprite, UXSprite crateSprite, UXElement unitElement, UXElement basicElement, UXElement advancedElement, UXElement eliteElement, UXLabel crateCountLabel, UXSprite dataFragIcon, UXLabel optionalUnitName)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			string text = string.Empty;
			FactionType faction = currentPlayer.Faction;
			if (faction != FactionType.Empire)
			{
				if (faction == FactionType.Rebel)
				{
					text = rewardGroup.RebelGuaranteedReward;
				}
			}
			else
			{
				text = rewardGroup.EmpireGuaranteedReward;
			}
			CrateSupplyVO optional = staticDataController.GetOptional<CrateSupplyVO>(text);
			Lang lang = Service.Lang;
			if (optional != null)
			{
				UXUtils.TrySetupItemQualityView(optional, unitElement, basicElement, advancedElement, eliteElement, null);
				int num = currentPlayer.Map.FindHighestHqLevel();
				IGeometryVO iconVOFromCrateSupply = GameUtils.GetIconVOFromCrateSupply(optional, num);
				if (iconVOFromCrateSupply != null)
				{
					ProjectorConfig projectorConfig = ProjectorUtils.GenerateGeometryConfig(iconVOFromCrateSupply, iconSprite, true);
					projectorConfig.AnimPreference = AnimationPreference.AnimationPreferred;
					ProjectorUtils.GenerateProjector(projectorConfig);
				}
				else
				{
					Service.Logger.ErrorFormat("Could not generate geometry for crate supply {0}", new object[]
					{
						optional.Uid
					});
				}
				if (iconVOFromCrateSupply != null)
				{
					int shardQualityNumeric = GameUtils.GetShardQualityNumeric(optional);
					if (shardQualityNumeric > -1)
					{
						dataFragIcon.SpriteName = string.Format("icoDataFragQ{0}", shardQualityNumeric);
						dataFragIcon.Visible = true;
					}
					else
					{
						dataFragIcon.Visible = false;
					}
				}
				InventoryCrateRewardController inventoryCrateRewardController = Service.InventoryCrateRewardController;
				int rewardAmount = inventoryCrateRewardController.GetRewardAmount(optional, num);
				if (rewardAmount > 1)
				{
					string text2 = lang.ThousandsSeparated(rewardAmount);
					prizeLabel.Text = lang.Get("CONFLICT_PRIZE_CRATE_MULTIPLIER", new object[]
					{
						text2
					});
				}
				else
				{
					prizeLabel.Visible = false;
				}
				if (optionalUnitName != null)
				{
					optionalUnitName.Text = inventoryCrateRewardController.GetCrateSupplyRewardName(optional);
				}
			}
			else
			{
				Service.Logger.ErrorFormat("Could not find crate supply {0} for faction {1}", new object[]
				{
					text,
					currentPlayer.Faction
				});
			}
			if (crateCountLabel != null)
			{
				crateCountLabel.Visible = false;
			}
			string[] crateRewardIds = rewardGroup.CrateRewardIds;
			CrateVO crateVO = null;
			if (crateRewardIds != null)
			{
				if (crateRewardIds.Length > 0)
				{
					crateVO = staticDataController.GetOptional<CrateVO>(rewardGroup.CrateRewardIds[0]);
				}
				if (crateCountLabel != null && crateRewardIds.Length > 1)
				{
					crateCountLabel.Visible = true;
					crateCountLabel.Text = lang.Get("CONFLICT_PRIZE_CRATE_MULTIPLIER", new object[]
					{
						crateRewardIds.Length
					});
				}
			}
			if (crateVO != null)
			{
				RewardUtils.SetCrateIcon(crateSprite, crateVO, AnimState.Idle);
			}
			else
			{
				Service.Logger.ErrorFormat("Missing crate reward meta data for tournament reward:{0}", new object[]
				{
					rewardGroup.Uid
				});
			}
		}
	}
}
