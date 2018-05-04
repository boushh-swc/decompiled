using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models
{
	public class SpecialAttackTrapEventData : ITrapEventData
	{
		public string SpecialAttackName
		{
			get;
			private set;
		}

		public ITrapEventData Init(string rawData)
		{
			if (string.IsNullOrEmpty(rawData))
			{
				Service.Logger.Error("All SpecialAttack Traps must list the uid of the special attack");
				return null;
			}
			this.SpecialAttackName = rawData.TrimEnd(new char[]
			{
				' '
			});
			return this;
		}
	}
}
