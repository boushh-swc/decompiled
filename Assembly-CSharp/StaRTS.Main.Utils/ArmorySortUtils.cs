using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StaRTS.Main.Utils
{
	public static class ArmorySortUtils
	{
		public delegate int SortEquipmentDelegate(SortableEquipment a, SortableEquipment b);

		[CompilerGenerated]
		private static ArmorySortUtils.SortEquipmentDelegate <>f__mg$cache0;

		[CompilerGenerated]
		private static ArmorySortUtils.SortEquipmentDelegate <>f__mg$cache1;

		[CompilerGenerated]
		private static ArmorySortUtils.SortEquipmentDelegate <>f__mg$cache2;

		[CompilerGenerated]
		private static ArmorySortUtils.SortEquipmentDelegate <>f__mg$cache3;

		[CompilerGenerated]
		private static ArmorySortUtils.SortEquipmentDelegate <>f__mg$cache4;

		[CompilerGenerated]
		private static ArmorySortUtils.SortEquipmentDelegate <>f__mg$cache5;

		[CompilerGenerated]
		private static ArmorySortUtils.SortEquipmentDelegate <>f__mg$cache6;

		[CompilerGenerated]
		private static ArmorySortUtils.SortEquipmentDelegate <>f__mg$cache7;

		[CompilerGenerated]
		private static ArmorySortUtils.SortEquipmentDelegate <>f__mg$cache8;

		public static void SortWithPriorityList(List<SortableEquipment> list, List<EquipmentSortMethod> priorityList)
		{
			list.Sort((SortableEquipment a, SortableEquipment b) => ArmorySortUtils.SortWithList(a, b, priorityList));
		}

		public static List<EquipmentVO> RemoveWrapper(List<SortableEquipment> sortableEquipmentList)
		{
			List<EquipmentVO> list = new List<EquipmentVO>();
			int i = 0;
			int count = sortableEquipmentList.Count;
			while (i < count)
			{
				list.Add(sortableEquipmentList[i].Equipment);
				i++;
			}
			return list;
		}

		public static int SortWithList(SortableEquipment a, SortableEquipment b, List<EquipmentSortMethod> sortingPriority)
		{
			Dictionary<EquipmentSortMethod, ArmorySortUtils.SortEquipmentDelegate> dictionary = ArmorySortUtils.CreateSortDictionary();
			int i = 0;
			int count = sortingPriority.Count;
			while (i < count)
			{
				ArmorySortUtils.SortEquipmentDelegate sortEquipmentDelegate = dictionary[sortingPriority[i]];
				int num = sortEquipmentDelegate(a, b);
				if (num != 0)
				{
					return num;
				}
				i++;
			}
			return 0;
		}

		private static Dictionary<EquipmentSortMethod, ArmorySortUtils.SortEquipmentDelegate> CreateSortDictionary()
		{
			Dictionary<EquipmentSortMethod, ArmorySortUtils.SortEquipmentDelegate> dictionary = new Dictionary<EquipmentSortMethod, ArmorySortUtils.SortEquipmentDelegate>();
			Dictionary<EquipmentSortMethod, ArmorySortUtils.SortEquipmentDelegate> arg_25_0 = dictionary;
			EquipmentSortMethod arg_25_1 = EquipmentSortMethod.UnlockedEquipment;
			if (ArmorySortUtils.<>f__mg$cache0 == null)
			{
				ArmorySortUtils.<>f__mg$cache0 = new ArmorySortUtils.SortEquipmentDelegate(ArmorySortUtils.SortByUnlockedEquipment);
			}
			arg_25_0.Add(arg_25_1, ArmorySortUtils.<>f__mg$cache0);
			Dictionary<EquipmentSortMethod, ArmorySortUtils.SortEquipmentDelegate> arg_49_0 = dictionary;
			EquipmentSortMethod arg_49_1 = EquipmentSortMethod.RequirementsMet;
			if (ArmorySortUtils.<>f__mg$cache1 == null)
			{
				ArmorySortUtils.<>f__mg$cache1 = new ArmorySortUtils.SortEquipmentDelegate(ArmorySortUtils.SortByRequirementsMet);
			}
			arg_49_0.Add(arg_49_1, ArmorySortUtils.<>f__mg$cache1);
			Dictionary<EquipmentSortMethod, ArmorySortUtils.SortEquipmentDelegate> arg_6D_0 = dictionary;
			EquipmentSortMethod arg_6D_1 = EquipmentSortMethod.CurrentPlanet;
			if (ArmorySortUtils.<>f__mg$cache2 == null)
			{
				ArmorySortUtils.<>f__mg$cache2 = new ArmorySortUtils.SortEquipmentDelegate(ArmorySortUtils.SortByCurrentPlanet);
			}
			arg_6D_0.Add(arg_6D_1, ArmorySortUtils.<>f__mg$cache2);
			Dictionary<EquipmentSortMethod, ArmorySortUtils.SortEquipmentDelegate> arg_91_0 = dictionary;
			EquipmentSortMethod arg_91_1 = EquipmentSortMethod.CapacitySize;
			if (ArmorySortUtils.<>f__mg$cache3 == null)
			{
				ArmorySortUtils.<>f__mg$cache3 = new ArmorySortUtils.SortEquipmentDelegate(ArmorySortUtils.SortByCapacitySize);
			}
			arg_91_0.Add(arg_91_1, ArmorySortUtils.<>f__mg$cache3);
			Dictionary<EquipmentSortMethod, ArmorySortUtils.SortEquipmentDelegate> arg_B5_0 = dictionary;
			EquipmentSortMethod arg_B5_1 = EquipmentSortMethod.Quality;
			if (ArmorySortUtils.<>f__mg$cache4 == null)
			{
				ArmorySortUtils.<>f__mg$cache4 = new ArmorySortUtils.SortEquipmentDelegate(ArmorySortUtils.SortByQuality);
			}
			arg_B5_0.Add(arg_B5_1, ArmorySortUtils.<>f__mg$cache4);
			Dictionary<EquipmentSortMethod, ArmorySortUtils.SortEquipmentDelegate> arg_D9_0 = dictionary;
			EquipmentSortMethod arg_D9_1 = EquipmentSortMethod.Alphabetical;
			if (ArmorySortUtils.<>f__mg$cache5 == null)
			{
				ArmorySortUtils.<>f__mg$cache5 = new ArmorySortUtils.SortEquipmentDelegate(ArmorySortUtils.SortAlphabetically);
			}
			arg_D9_0.Add(arg_D9_1, ArmorySortUtils.<>f__mg$cache5);
			Dictionary<EquipmentSortMethod, ArmorySortUtils.SortEquipmentDelegate> arg_FD_0 = dictionary;
			EquipmentSortMethod arg_FD_1 = EquipmentSortMethod.EmptyEquipment;
			if (ArmorySortUtils.<>f__mg$cache6 == null)
			{
				ArmorySortUtils.<>f__mg$cache6 = new ArmorySortUtils.SortEquipmentDelegate(ArmorySortUtils.SortByEmptyEquipment);
			}
			arg_FD_0.Add(arg_FD_1, ArmorySortUtils.<>f__mg$cache6);
			Dictionary<EquipmentSortMethod, ArmorySortUtils.SortEquipmentDelegate> arg_121_0 = dictionary;
			EquipmentSortMethod arg_121_1 = EquipmentSortMethod.DecrementingIndex;
			if (ArmorySortUtils.<>f__mg$cache7 == null)
			{
				ArmorySortUtils.<>f__mg$cache7 = new ArmorySortUtils.SortEquipmentDelegate(ArmorySortUtils.SortByDecrementingIndex);
			}
			arg_121_0.Add(arg_121_1, ArmorySortUtils.<>f__mg$cache7);
			Dictionary<EquipmentSortMethod, ArmorySortUtils.SortEquipmentDelegate> arg_145_0 = dictionary;
			EquipmentSortMethod arg_145_1 = EquipmentSortMethod.IncrementingEmptyIndex;
			if (ArmorySortUtils.<>f__mg$cache8 == null)
			{
				ArmorySortUtils.<>f__mg$cache8 = new ArmorySortUtils.SortEquipmentDelegate(ArmorySortUtils.SortByIncrementingEmptyIndex);
			}
			arg_145_0.Add(arg_145_1, ArmorySortUtils.<>f__mg$cache8);
			return dictionary;
		}

		private static int SortByUnlockedEquipment(SortableEquipment a, SortableEquipment b)
		{
			bool flag = a.Player.UnlockedLevels.Equipment.Levels.ContainsKey(a.Equipment.EquipmentID);
			bool flag2 = b.Player.UnlockedLevels.Equipment.Levels.ContainsKey(b.Equipment.EquipmentID);
			if (flag && !flag2)
			{
				return -1;
			}
			if (flag2 && !flag)
			{
				return 1;
			}
			return 0;
		}

		private static int SortByRequirementsMet(SortableEquipment a, SortableEquipment b)
		{
			bool flag = ArmoryUtils.IsBuildingRequirementMet(a.Equipment);
			bool flag2 = ArmoryUtils.IsBuildingRequirementMet(b.Equipment);
			if (flag && !flag2)
			{
				return -1;
			}
			if (flag2 && !flag)
			{
				return 1;
			}
			return 0;
		}

		private static int SortByCurrentPlanet(SortableEquipment a, SortableEquipment b)
		{
			bool flag = ArmoryUtils.IsEquipmentOnValidPlanet(a.Player, a.Equipment);
			bool flag2 = ArmoryUtils.IsEquipmentOnValidPlanet(b.Player, b.Equipment);
			if (flag && !flag2)
			{
				return -1;
			}
			if (flag2 && !flag)
			{
				return 1;
			}
			return 0;
		}

		private static int SortByCapacitySize(SortableEquipment a, SortableEquipment b)
		{
			int num = a.Equipment.Size - b.Equipment.Size;
			if (num != 0)
			{
				return num;
			}
			return 0;
		}

		private static int SortByQuality(SortableEquipment a, SortableEquipment b)
		{
			int num = b.Equipment.Quality - a.Equipment.Quality;
			if (num != 0)
			{
				return num;
			}
			return 0;
		}

		private static int SortAlphabetically(SortableEquipment a, SortableEquipment b)
		{
			return string.Compare(a.Equipment.EquipmentID, b.Equipment.EquipmentID);
		}

		private static int SortByEmptyEquipment(SortableEquipment a, SortableEquipment b)
		{
			if (a.HasEquipment() && !b.HasEquipment())
			{
				return 1;
			}
			if (b.HasEquipment() && !a.HasEquipment())
			{
				return -1;
			}
			return 0;
		}

		private static int SortByDecrementingIndex(SortableEquipment a, SortableEquipment b)
		{
			int num = b.IncrementingIndex - a.IncrementingIndex;
			if (num != 0)
			{
				return num;
			}
			return 0;
		}

		private static int SortByIncrementingEmptyIndex(SortableEquipment a, SortableEquipment b)
		{
			int num = a.EmptyIndex - b.EmptyIndex;
			if (num != 0)
			{
				return num;
			}
			return 0;
		}
	}
}
