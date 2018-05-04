using System;
using System.Collections;
using System.Collections.Generic;

namespace StaRTS.Utils
{
	public class ListUtils
	{
		public static List<T> ConvertArrayList<T>(ArrayList data)
		{
			List<T> list = new List<T>(data.Count);
			for (int i = 0; i < data.Count; i++)
			{
				list.Add((T)((object)data[i]));
			}
			return list;
		}

		public static void SortListBasedOnToString<T>(List<T> list)
		{
			list.Sort((T val1, T val2) => val1.ToString().CompareTo(val2.ToString()));
		}

		public static List<T> CreateAndFillListWithValue<T>(T val, int count)
		{
			List<T> list = new List<T>(count);
			for (int i = 0; i < count; i++)
			{
				list.Add(val);
			}
			return list;
		}
	}
}
