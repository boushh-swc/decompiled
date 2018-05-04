using StaRTS.Main.Controllers;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.ValueObjects
{
	public class TroopUniqueAbilityDescVO : IValueObject
	{
		public static int COLUMN_unitID
		{
			get;
			private set;
		}

		public static int COLUMN_abilityTitle1
		{
			get;
			private set;
		}

		public static int COLUMN_abilityDesc1
		{
			get;
			private set;
		}

		public static int COLUMN_abilityTitle2
		{
			get;
			private set;
		}

		public static int COLUMN_abilityDesc2
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string TroopID
		{
			get;
			private set;
		}

		public string AbilityTitle1
		{
			get;
			private set;
		}

		public string AbilityDesc1
		{
			get;
			private set;
		}

		public string AbilityTitle2
		{
			get;
			private set;
		}

		public string AbilityDesc2
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.TroopID = row.TryGetString(TroopUniqueAbilityDescVO.COLUMN_unitID);
			this.AbilityTitle1 = row.TryGetString(TroopUniqueAbilityDescVO.COLUMN_abilityTitle1);
			this.AbilityDesc1 = row.TryGetString(TroopUniqueAbilityDescVO.COLUMN_abilityDesc1);
			this.AbilityTitle2 = row.TryGetString(TroopUniqueAbilityDescVO.COLUMN_abilityTitle2);
			this.AbilityDesc2 = row.TryGetString(TroopUniqueAbilityDescVO.COLUMN_abilityDesc2);
			if (!string.IsNullOrEmpty(this.TroopID))
			{
				StaticDataController staticDataController = Service.StaticDataController;
				Dictionary<string, TroopTypeVO>.ValueCollection all = staticDataController.GetAll<TroopTypeVO>();
				foreach (TroopTypeVO current in all)
				{
					if (current.TroopID == this.TroopID)
					{
						current.UniqueAbilityDescVO = this;
					}
				}
			}
		}
	}
}
