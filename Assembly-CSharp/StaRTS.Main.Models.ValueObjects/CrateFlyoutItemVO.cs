using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.ValueObjects
{
	public class CrateFlyoutItemVO : IValueObject
	{
		public static int COLUMN_planet
		{
			get;
			private set;
		}

		public static int COLUMN_minHQ
		{
			get;
			private set;
		}

		public static int COLUMN_maxHQ
		{
			get;
			private set;
		}

		public static int COLUMN_reqArmory
		{
			get;
			private set;
		}

		public static int COLUMN_listChanceString
		{
			get;
			private set;
		}

		public static int COLUMN_listDescString
		{
			get;
			private set;
		}

		public static int COLUMN_listIcons
		{
			get;
			private set;
		}

		public static int COLUMN_quantityString
		{
			get;
			private set;
		}

		public static int COLUMN_detailChanceString
		{
			get;
			private set;
		}

		public static int COLUMN_detailDescString
		{
			get;
			private set;
		}

		public static int COLUMN_crateSupplyUid
		{
			get;
			private set;
		}

		public static int COLUMN_detailTypeString
		{
			get;
			private set;
		}

		public static int COLUMN_tournamentTierDisplay3D
		{
			get;
			private set;
		}

		public static int COLUMN_showParametersList
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string[] PlanetIds
		{
			get;
			private set;
		}

		public int MinHQ
		{
			get;
			private set;
		}

		public int MaxHQ
		{
			get;
			private set;
		}

		public bool ReqArmory
		{
			get;
			private set;
		}

		public string ListChanceString
		{
			get;
			private set;
		}

		public string ListDescString
		{
			get;
			private set;
		}

		public string[] ListIcons
		{
			get;
			private set;
		}

		public string QuantityString
		{
			get;
			private set;
		}

		public string DetailChanceString
		{
			get;
			private set;
		}

		public string DetailDescString
		{
			get;
			private set;
		}

		public string CrateSupplyUid
		{
			get;
			private set;
		}

		public string DetailTypeStringId
		{
			get;
			private set;
		}

		public bool TournamentTierDisplay3D
		{
			get;
			private set;
		}

		protected string[] ShowParametersList
		{
			get;
			private set;
		}

		public List<CrateFlyoutDisplayType> ShowParams
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.PlanetIds = row.TryGetStringArray(CrateFlyoutItemVO.COLUMN_planet);
			this.MinHQ = row.TryGetInt(CrateFlyoutItemVO.COLUMN_minHQ);
			this.MaxHQ = row.TryGetInt(CrateFlyoutItemVO.COLUMN_maxHQ);
			this.ReqArmory = row.TryGetBool(CrateFlyoutItemVO.COLUMN_reqArmory);
			this.ListChanceString = row.TryGetString(CrateFlyoutItemVO.COLUMN_listChanceString);
			this.ListDescString = row.TryGetString(CrateFlyoutItemVO.COLUMN_listDescString);
			this.ListIcons = row.TryGetStringArray(CrateFlyoutItemVO.COLUMN_listIcons);
			this.QuantityString = row.TryGetString(CrateFlyoutItemVO.COLUMN_quantityString);
			this.DetailChanceString = row.TryGetString(CrateFlyoutItemVO.COLUMN_detailChanceString);
			this.DetailDescString = row.TryGetString(CrateFlyoutItemVO.COLUMN_detailDescString);
			this.CrateSupplyUid = row.TryGetString(CrateFlyoutItemVO.COLUMN_crateSupplyUid);
			this.DetailTypeStringId = row.TryGetString(CrateFlyoutItemVO.COLUMN_detailTypeString);
			this.TournamentTierDisplay3D = row.TryGetBool(CrateFlyoutItemVO.COLUMN_tournamentTierDisplay3D);
			this.ShowParametersList = row.TryGetStringArray(CrateFlyoutItemVO.COLUMN_showParametersList);
			if (this.ShowParametersList != null)
			{
				this.ShowParams = new List<CrateFlyoutDisplayType>();
				int i = 0;
				int num = this.ShowParametersList.Length;
				while (i < num)
				{
					CrateFlyoutDisplayType item = StringUtils.ParseEnum<CrateFlyoutDisplayType>(this.ShowParametersList[i]);
					this.ShowParams.Add(item);
					i++;
				}
			}
		}
	}
}
