using System;
using System.Collections.Generic;
using System.Linq;

public static class EnumHelper
{
	public static string[] GetEnumAsStringArray<T>()
	{
		Type typeFromHandle = typeof(T);
		int enumLength = EnumHelper.GetEnumLength<T>();
		string[] array = new string[enumLength];
		int num = 0;
		foreach (object current in Enum.GetValues(typeFromHandle))
		{
			array[num] = current.ToString();
			num++;
		}
		return array;
	}

	public static string[] GetEnumArrayAsStringArray<T>(T[] enumArray)
	{
		string[] array = new string[enumArray.Length];
		for (int i = 0; i < enumArray.Length; i++)
		{
			array[i] = enumArray[i].ToString();
		}
		return array;
	}

	public static T[] GetStringArrayAsEnumArray<T>(string[] stringArray)
	{
		T[] array = new T[stringArray.Length];
		for (int i = 0; i < stringArray.Length; i++)
		{
			array[i] = EnumHelper.GetEnumValFromString<T>(stringArray[i]);
		}
		return array;
	}

	public static int GetEnumLength<T>()
	{
		Type typeFromHandle = typeof(T);
		return Enum.GetNames(typeFromHandle).Length;
	}

	public static List<T> GetEnumAsList<T>()
	{
		return Enum.GetValues(typeof(T)).Cast<T>().ToList<T>();
	}

	public static T[] GetEnumAsArray<T>()
	{
		return Enum.GetValues(typeof(T)).Cast<T>().ToArray<T>();
	}

	public static T GetEnumValFromString<T>(string stringVal)
	{
		return EnumHelper.GetEnumValFromString<T>(stringVal, default(T));
	}

	public static T GetEnumValFromString<T>(string stringVal, T defaultValue)
	{
		T result = defaultValue;
		if (stringVal != null && Enum.IsDefined(typeof(T), stringVal))
		{
			result = (T)((object)Enum.Parse(typeof(T), stringVal));
		}
		return result;
	}
}
