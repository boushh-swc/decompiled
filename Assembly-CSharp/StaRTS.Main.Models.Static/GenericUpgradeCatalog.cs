using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using StaRTS.Utils.Diagnostics;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StaRTS.Main.Models.Static
{
	public abstract class GenericUpgradeCatalog<T> : AbstractUpgradeCatalog where T : IValueObject, IUpgradeableVO
	{
		public const int MIN_UPGRADE_LEVEL = 1;

		private Dictionary<string, List<T>> upgradeGroups;

		private Dictionary<string, T> maxLevels;

		private Dictionary<string, T> maxLevelsForRewards;

		[CompilerGenerated]
		private static Comparison<T> <>f__mg$cache0;

		public GenericUpgradeCatalog()
		{
			this.InitService();
			StaticDataController staticDataController = Service.StaticDataController;
			this.upgradeGroups = new Dictionary<string, List<T>>();
			this.maxLevels = new Dictionary<string, T>();
			this.maxLevelsForRewards = new Dictionary<string, T>();
			foreach (T current in staticDataController.GetAll<T>())
			{
				List<T> list;
				if (this.upgradeGroups.ContainsKey(current.UpgradeGroup))
				{
					list = this.upgradeGroups[current.UpgradeGroup];
				}
				else
				{
					list = new List<T>();
					this.upgradeGroups.Add(current.UpgradeGroup, list);
				}
				if (this.maxLevels.ContainsKey(current.UpgradeGroup))
				{
					T t = this.maxLevels[current.UpgradeGroup];
					if (current.Lvl > t.Lvl && current.PlayerFacing)
					{
						this.maxLevels[current.UpgradeGroup] = current;
					}
				}
				else
				{
					this.maxLevels[current.UpgradeGroup] = current;
				}
				if (this.maxLevelsForRewards.ContainsKey(current.UpgradeGroup))
				{
					T t2 = this.maxLevelsForRewards[current.UpgradeGroup];
					if (current.Lvl > t2.Lvl)
					{
						this.maxLevelsForRewards[current.UpgradeGroup] = current;
					}
				}
				else
				{
					this.maxLevelsForRewards[current.UpgradeGroup] = current;
				}
				list.Add(current);
			}
			foreach (KeyValuePair<string, List<T>> current2 in this.upgradeGroups)
			{
				List<T> arg_237_0 = current2.Value;
				if (GenericUpgradeCatalog<T>.<>f__mg$cache0 == null)
				{
					GenericUpgradeCatalog<T>.<>f__mg$cache0 = new Comparison<T>(GenericUpgradeCatalog<T>.SortByLevelAscending);
				}
				arg_237_0.Sort(GenericUpgradeCatalog<T>.<>f__mg$cache0);
				int arg_269_0 = current2.Value.Count;
				T t3 = this.maxLevels[current2.Key];
				if (arg_269_0 != t3.Lvl)
				{
					for (int i = 1; i < current2.Value.Count; i++)
					{
						T t4 = current2.Value[i];
						int arg_2B2_0 = t4.Lvl;
						T t5 = current2.Value[i - 1];
						if (arg_2B2_0 == t5.Lvl)
						{
							Logger arg_35F_0 = Service.Logger;
							string arg_35F_1 = "Duplicate levels in group {4}: {0} ({1}) AND {2} ({3})";
							object[] expr_2C7 = new object[5];
							int arg_2E6_1 = 0;
							T t6 = current2.Value[i];
							expr_2C7[arg_2E6_1] = t6.Uid;
							int arg_30B_1 = 1;
							T t7 = current2.Value[i];
							expr_2C7[arg_30B_1] = t7.Lvl;
							int arg_32D_1 = 2;
							T t8 = current2.Value[i - 1];
							expr_2C7[arg_32D_1] = t8.Uid;
							int arg_354_1 = 3;
							T t9 = current2.Value[i - 1];
							expr_2C7[arg_354_1] = t9.Lvl;
							expr_2C7[4] = current2.Key;
							arg_35F_0.ErrorFormat(arg_35F_1, expr_2C7);
						}
						else
						{
							T t10 = current2.Value[i];
							int arg_3A7_0 = t10.Lvl;
							T t11 = current2.Value[i - 1];
							if (arg_3A7_0 != t11.Lvl + 1)
							{
								Logger arg_454_0 = Service.Logger;
								string arg_454_1 = "In Group {4} expected {0} ({1}) to be one level higher than {2} ({3})";
								object[] expr_3BC = new object[5];
								int arg_3DB_1 = 0;
								T t12 = current2.Value[i];
								expr_3BC[arg_3DB_1] = t12.Uid;
								int arg_400_1 = 1;
								T t13 = current2.Value[i];
								expr_3BC[arg_400_1] = t13.Lvl;
								int arg_422_1 = 2;
								T t14 = current2.Value[i - 1];
								expr_3BC[arg_422_1] = t14.Uid;
								int arg_449_1 = 3;
								T t15 = current2.Value[i - 1];
								expr_3BC[arg_449_1] = t15.Lvl;
								expr_3BC[4] = current2.Key;
								arg_454_0.ErrorFormat(arg_454_1, expr_3BC);
							}
						}
					}
				}
			}
		}

		protected abstract void InitService();

		public List<T> GetUpgradeGroupLevels(string upgradeGroup)
		{
			return this.upgradeGroups[upgradeGroup];
		}

		public List<string> GetIDCollection()
		{
			return new List<string>(this.upgradeGroups.Keys);
		}

		public new T GetByLevel(string upgradeGroup, int level)
		{
			List<T> upgradeGroupLevels = this.GetUpgradeGroupLevels(upgradeGroup);
			int num = level - 1;
			if (num < upgradeGroupLevels.Count)
			{
				return upgradeGroupLevels[num];
			}
			return default(T);
		}

		public T GetByLevel(T vo, int level)
		{
			return this.GetByLevel(vo.UpgradeGroup, level);
		}

		public T GetNextLevel(T vo)
		{
			return this.GetByLevel(vo.UpgradeGroup, vo.Lvl + 1);
		}

		public T GetPrevLevel(T vo)
		{
			return this.GetByLevel(vo.UpgradeGroup, vo.Lvl - 1);
		}

		protected override IUpgradeableVO InternalGetByLevel(string upgradeGroup, int level)
		{
			return this.GetUpgradeGroupLevels(upgradeGroup)[level - 1];
		}

		public Dictionary<string, List<T>>.KeyCollection AllUpgradeGroups()
		{
			return this.upgradeGroups.Keys;
		}

		public T GetMaxLevel(string upgradeGroup)
		{
			return this.maxLevels[upgradeGroup];
		}

		public T GetMaxLevel(T upgradeable)
		{
			return this.GetMaxLevel(upgradeable.UpgradeGroup);
		}

		public T GetMaxRewardableLevel(string upgradeGroup)
		{
			return this.maxLevelsForRewards[upgradeGroup];
		}

		public T GetMaxRewardableLevel(T upgradeable)
		{
			return this.GetMaxRewardableLevel(upgradeable.UpgradeGroup);
		}

		public T GetMinLevel(string upgradeGroup)
		{
			return this.GetByLevel(upgradeGroup, 1);
		}

		public T GetMinLevel(T upgradeable)
		{
			return this.GetMinLevel(upgradeable.UpgradeGroup);
		}

		public bool CanUpgradeTo(LevelMap levelMap, T upgradeable)
		{
			return upgradeable.Lvl - levelMap.GetLevel(upgradeable.UpgradeGroup) == 1;
		}

		public static int SortByLevelAscending(T a, T b)
		{
			return a.Lvl - b.Lvl;
		}
	}
}
