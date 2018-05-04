using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class BattleScriptVO : IValueObject
	{
		public static int COLUMN_scripts
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string Scripts
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Scripts = row.TryGetString(BattleScriptVO.COLUMN_scripts);
		}
	}
}
