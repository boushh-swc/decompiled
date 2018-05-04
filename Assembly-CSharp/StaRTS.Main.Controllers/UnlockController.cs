using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers
{
	public class UnlockController
	{
		public const int BUILDING = 0;

		public const int UPGRADING = 1;

		private CurrentPlayer currentPlayer;

		private StaticDataController dataController;

		private EventManager eventManager;

		public UnlockController()
		{
			Service.UnlockController = this;
			this.currentPlayer = Service.CurrentPlayer;
			this.dataController = Service.StaticDataController;
			this.eventManager = Service.EventManager;
		}

		public bool CanDeployableBeUpgraded(IDeployableVO currentVO, IDeployableVO vo)
		{
			string text = null;
			string text2 = null;
			return this.CanDeployableBeUpgraded(currentVO, vo, out text, out text2);
		}

		public bool CanDeployableBeUpgraded(IDeployableVO currentVO, IDeployableVO vo, out string requirementText, out string shortRequirementText)
		{
			requirementText = null;
			shortRequirementText = null;
			BuildingTypeVO buildingTypeVO = null;
			bool flag = false;
			if (this.RequiresUnlockByEventReward(vo))
			{
				if (this.IsMinLevelUnlocked(vo))
				{
					if (vo is TroopTypeVO)
					{
						if (!this.VerifyUpgradeData(currentVO, vo))
						{
							return false;
						}
						TroopTypeVO vo2 = (TroopTypeVO)currentVO;
						TroopTypeVO vo3 = (TroopTypeVO)vo;
						flag = (this.IsTroopUnlockedByRequirement(vo2, ref buildingTypeVO) && this.IsTroopUnlockedByRequirement(vo3, ref buildingTypeVO));
					}
					else if (vo is SpecialAttackTypeVO)
					{
						if (!this.VerifyUpgradeData(currentVO, vo))
						{
							return false;
						}
						SpecialAttackTypeVO vo4 = (SpecialAttackTypeVO)currentVO;
						SpecialAttackTypeVO vo5 = (SpecialAttackTypeVO)vo;
						flag = (this.IsSpecialAttackUnlockedByRequirement(vo4, ref buildingTypeVO) && this.IsSpecialAttackUnlockedByRequirement(vo5, ref buildingTypeVO));
					}
				}
				else
				{
					requirementText = Service.Lang.Get("UNLOCK_IN_EVENT_ONLY", new object[0]);
				}
				if (!string.IsNullOrEmpty(currentVO.UpgradeShardUid))
				{
					shortRequirementText = LangUtils.GetShardLockedDeployableString(currentVO);
				}
				else
				{
					Service.Logger.ErrorFormat("Deployable has unlockedByEvent but not upgradeShardUid {0} {1} {2}", new object[]
					{
						currentVO.GetType(),
						currentVO.UpgradeGroup,
						currentVO.Lvl
					});
				}
			}
			else
			{
				flag = (this.IsUnlockedByRequirement(currentVO, 1, out buildingTypeVO) && this.IsUnlockedByRequirement(vo, 1, out buildingTypeVO));
			}
			if (!flag && requirementText == null && buildingTypeVO != null)
			{
				requirementText = Service.Lang.Get("BUILDING_REQUIREMENT", new object[]
				{
					buildingTypeVO.Lvl,
					LangUtils.GetBuildingDisplayName(buildingTypeVO)
				});
				shortRequirementText = requirementText;
			}
			return flag;
		}

		private bool VerifyUpgradeData(IUpgradeableVO currentVO, IUpgradeableVO nextVO)
		{
			if (currentVO == null || nextVO == null)
			{
				Service.Logger.ErrorFormat("Deployable upgrade data is null '{0}' '{1}'", new object[]
				{
					(currentVO != null) ? currentVO.GetType().ToString() : "null",
					(nextVO != null) ? nextVO.GetType().ToString() : "null"
				});
				return false;
			}
			if (currentVO.GetType() != nextVO.GetType())
			{
				Service.Logger.ErrorFormat("Deployable upgrade type differs from next type: {0} {1} {2} vs {3} {4} {5}", new object[]
				{
					currentVO.GetType(),
					currentVO.UpgradeGroup,
					currentVO.Lvl,
					nextVO.GetType(),
					nextVO.UpgradeGroup,
					nextVO.Lvl
				});
				return false;
			}
			return true;
		}

		public bool IsMinLevelUnlocked(IUpgradeableVO vo)
		{
			IUpgradeableVO vo2 = null;
			if (vo is TroopTypeVO)
			{
				vo2 = Service.TroopUpgradeCatalog.GetMinLevel((TroopTypeVO)vo);
			}
			else if (vo is SpecialAttackTypeVO)
			{
				vo2 = Service.StarshipUpgradeCatalog.GetMinLevel((SpecialAttackTypeVO)vo);
			}
			BuildingTypeVO buildingTypeVO = null;
			return this.IsUnlocked(vo2, 1, out buildingTypeVO);
		}

		public bool IsTroopUnlocked(GamePlayer player, TroopTypeVO vo)
		{
			BuildingTypeVO buildingTypeVO = null;
			return this.IsUnlocked(player, vo, 1, out buildingTypeVO);
		}

		public bool IsSpecialAttackUnlocked(GamePlayer player, SpecialAttackTypeVO vo)
		{
			BuildingTypeVO buildingTypeVO = null;
			return this.IsUnlocked(player, vo, 1, out buildingTypeVO);
		}

		public bool IsUnlocked(IUpgradeableVO vo, int buildOrUpgrade, out BuildingTypeVO req)
		{
			return this.IsUnlocked(this.currentPlayer, vo, 1, out req);
		}

		public bool IsUpgradeableUnlocked(IUpgradeableVO vo)
		{
			BuildingTypeVO buildingTypeVO = null;
			return this.IsUnlocked(this.currentPlayer, vo, 1, out buildingTypeVO);
		}

		private bool IsUnlocked(GamePlayer player, IUpgradeableVO vo, int buildOrUpgrade, out BuildingTypeVO req)
		{
			req = null;
			if (this.RequiresUnlockByEventReward(vo))
			{
				return this.HasUnlockedByReward(player, vo);
			}
			return this.IsUnlockedByRequirement(vo, buildOrUpgrade, out req);
		}

		public bool HasUnlockedByReward(GamePlayer player, IUpgradeableVO vo)
		{
			bool result = false;
			if (vo is BuildingTypeVO)
			{
				result = this.HasUnlockedBuildingByReward(player, vo);
			}
			else if (vo is TroopTypeVO)
			{
				result = this.HasUnlockedTroopByReward(player, vo);
			}
			else if (vo is SpecialAttackTypeVO)
			{
				result = this.HasUnlockedSpecialAttackByReward(player, vo);
			}
			return result;
		}

		public bool IsUnlockedByRequirement(IUpgradeableVO vo, int buildOrUpgrade)
		{
			BuildingTypeVO buildingTypeVO = null;
			return this.IsUnlockedByRequirement(vo, buildOrUpgrade, out buildingTypeVO);
		}

		private bool IsUnlockedByRequirement(IUpgradeableVO vo, int buildOrUpgrade, out BuildingTypeVO req)
		{
			bool result = false;
			req = null;
			if (vo is BuildingTypeVO)
			{
				result = this.IsBuildingUnlockedByRequirement((BuildingTypeVO)vo, buildOrUpgrade, ref req);
			}
			else if (vo is TroopTypeVO)
			{
				result = this.IsTroopUnlockedByRequirement((TroopTypeVO)vo, ref req);
			}
			else if (vo is SpecialAttackTypeVO)
			{
				result = this.IsSpecialAttackUnlockedByRequirement((SpecialAttackTypeVO)vo, ref req);
			}
			else if (vo is EquipmentVO)
			{
				result = this.IsEquipmentUnlockedByRequirement((EquipmentVO)vo, ref req);
			}
			return result;
		}

		private bool IsBuildingUnlockedByRequirement(BuildingTypeVO vo, int buildOrUpgrade, ref BuildingTypeVO req)
		{
			bool flag = true;
			if (buildOrUpgrade == 0)
			{
				vo = Service.BuildingUpgradeCatalog.GetMinLevel(vo);
			}
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			StaticDataController staticDataController = Service.StaticDataController;
			if (!string.IsNullOrEmpty(vo.BuildingRequirement))
			{
				req = staticDataController.Get<BuildingTypeVO>(vo.BuildingRequirement);
				flag = buildingLookupController.HasConstructedBuilding(req);
			}
			if (flag && !string.IsNullOrEmpty(vo.BuildingRequirement2))
			{
				req = staticDataController.Get<BuildingTypeVO>(vo.BuildingRequirement2);
				flag = buildingLookupController.HasConstructedBuilding(req);
			}
			return flag;
		}

		private bool IsTroopUnlockedByRequirement(TroopTypeVO vo, ref BuildingTypeVO req)
		{
			if (vo != null && !string.IsNullOrEmpty(vo.BuildingRequirement))
			{
				BuildingLookupController buildingLookupController = Service.BuildingLookupController;
				req = buildingLookupController.GetTroopUnlockRequirement(vo);
				return req == null || buildingLookupController.HasConstructedBuilding(req);
			}
			return true;
		}

		private bool IsSpecialAttackUnlockedByRequirement(SpecialAttackTypeVO vo, ref BuildingTypeVO req)
		{
			if (vo != null && !string.IsNullOrEmpty(vo.BuildingRequirement))
			{
				BuildingLookupController buildingLookupController = Service.BuildingLookupController;
				req = buildingLookupController.GetStarshipUnlockRequirement(vo);
				return req == null || buildingLookupController.HasConstructedBuilding(req);
			}
			return true;
		}

		private bool IsEquipmentUnlockedByRequirement(EquipmentVO vo, ref BuildingTypeVO req)
		{
			if (vo != null && !string.IsNullOrEmpty(vo.BuildingRequirement))
			{
				BuildingLookupController buildingLookupController = Service.BuildingLookupController;
				req = buildingLookupController.GetEquipmentUnlockRequirement(vo);
				return req == null || buildingLookupController.HasConstructedBuilding(req);
			}
			return true;
		}

		public bool RequiresUnlockByEventReward(IUpgradeableVO vo)
		{
			return vo != null && vo.UnlockedByEvent;
		}

		public bool HasUnlockedBuildingByReward(GamePlayer player, IUpgradeableVO vo)
		{
			return this.HasUnlockedByReward(vo, player.UnlockedLevels.Buildings);
		}

		public bool HasUnlockedBuildingByReward(IUpgradeableVO vo)
		{
			return this.HasUnlockedByReward(vo, this.currentPlayer.UnlockedLevels.Buildings);
		}

		public bool HasUnlockedTroopByReward(GamePlayer player, IUpgradeableVO vo)
		{
			return this.HasUnlockedByReward(vo, player.UnlockedLevels.Troops);
		}

		public bool HasUnlockedTroopByReward(IUpgradeableVO vo)
		{
			return this.HasUnlockedByReward(vo, this.currentPlayer.UnlockedLevels.Troops);
		}

		public bool HasUnlockedSpecialAttackByReward(GamePlayer player, IUpgradeableVO vo)
		{
			return this.HasUnlockedByReward(vo, player.UnlockedLevels.Starships);
		}

		public bool HasUnlockedSpecialAttackByReward(IUpgradeableVO vo)
		{
			return this.HasUnlockedByReward(vo, this.currentPlayer.UnlockedLevels.Starships);
		}

		private bool HasUnlockedByReward(IUpgradeableVO vo, LevelMap levels)
		{
			bool result = false;
			if (this.RequiresUnlockByEventReward(vo))
			{
				result = (levels.Has(vo) && levels.GetLevel(vo.UpgradeGroup) >= vo.Lvl);
			}
			return result;
		}

		public int GetCurrentLevelUnlockedByReward(IUpgradeableVO vo)
		{
			int result = 0;
			if (this.RequiresUnlockByEventReward(vo))
			{
				LevelMap levelMap = null;
				if (vo is BuildingTypeVO)
				{
					levelMap = this.currentPlayer.UnlockedLevels.Buildings;
				}
				else if (vo is TroopTypeVO)
				{
					levelMap = this.currentPlayer.UnlockedLevels.Troops;
				}
				else if (vo is SpecialAttackTypeVO)
				{
					levelMap = this.currentPlayer.UnlockedLevels.Starships;
				}
				if (levelMap != null)
				{
					result = ((!levelMap.Has(vo)) ? 0 : levelMap.GetLevel(vo.UpgradeGroup));
				}
			}
			return result;
		}

		public void GrantUnlockByReward(IUpgradeableVO vo)
		{
			if (vo is BuildingTypeVO)
			{
				this.currentPlayer.UnlockedLevels.Buildings.SetLevel(vo);
			}
			else if (vo is TroopTypeVO)
			{
				this.currentPlayer.UnlockedLevels.Troops.SetLevel(vo);
			}
			else if (vo is SpecialAttackTypeVO)
			{
				this.currentPlayer.UnlockedLevels.Starships.SetLevel(vo);
			}
		}

		public void GrantBuildingUnlockReward(string[] buildingUnlocks)
		{
			if (buildingUnlocks != null)
			{
				LevelMap buildings = this.currentPlayer.UnlockedLevels.Buildings;
				int i = 0;
				int num = buildingUnlocks.Length;
				while (i < num)
				{
					BuildingTypeVO level = this.dataController.Get<BuildingTypeVO>(buildingUnlocks[i]);
					buildings.SetLevel(level);
					i++;
				}
				this.eventManager.SendEvent(EventId.InventoryUnlockUpdated, null);
			}
		}

		public void GrantTroopUnlockReward(string[] troopUnlocks)
		{
			if (troopUnlocks != null)
			{
				LevelMap troops = this.currentPlayer.UnlockedLevels.Troops;
				int i = 0;
				int num = troopUnlocks.Length;
				while (i < num)
				{
					TroopTypeVO level = this.dataController.Get<TroopTypeVO>(troopUnlocks[i]);
					troops.SetLevel(level);
					i++;
				}
				this.eventManager.SendEvent(EventId.InventoryUnlockUpdated, null);
			}
		}

		public void GrantSpecialAttackUnlockReward(string[] specialAttackUnlocks)
		{
			if (specialAttackUnlocks != null)
			{
				LevelMap starships = this.currentPlayer.UnlockedLevels.Starships;
				int i = 0;
				int num = specialAttackUnlocks.Length;
				while (i < num)
				{
					SpecialAttackTypeVO level = this.dataController.Get<SpecialAttackTypeVO>(specialAttackUnlocks[i]);
					starships.SetLevel(level);
					i++;
				}
				this.eventManager.SendEvent(EventId.InventoryUnlockUpdated, null);
			}
		}
	}
}
