using StaRTS.Main.Models;
using StaRTS.Utils;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.TrapConditions
{
	public static class TrapConditionFactory
	{
		private const string CONDITION_RADIUS = "radius";

		private const string CONDITION_INTRUDER_TYPE = "intrudertype";

		private const string CONDITION_ARMOR_NOT = "armornot";

		public static TrapCondition GenerateTrapCondition(string trapType, params string[] args)
		{
			if (trapType == "radius")
			{
				uint radius = Convert.ToUInt32(args[0]);
				return new RadiusTrapCondition(radius);
			}
			if (trapType == "intrudertype")
			{
				TroopType intruderType = StringUtils.ParseEnum<TroopType>(args[0]);
				return new IntruderTypeTrapCondition(intruderType);
			}
			if (trapType == "armornot")
			{
				List<ArmorType> list = new List<ArmorType>();
				int i = 0;
				int num = args.Length;
				while (i < num)
				{
					list.Add(StringUtils.ParseEnum<ArmorType>(args[i]));
					i++;
				}
				return new ArmorNotTrapCondition(list);
			}
			return null;
		}
	}
}
