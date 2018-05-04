using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Battle;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.ValueObjects
{
	public class BattleTypeVO : BattleDeploymentData, IAssetVO, IValueObject
	{
		public static int COLUMN_battleName
		{
			get;
			private set;
		}

		public static int COLUMN_assetName
		{
			get;
			private set;
		}

		public static int COLUMN_bundleName
		{
			get;
			private set;
		}

		public static int COLUMN_planet
		{
			get;
			private set;
		}

		public static int COLUMN_defenderName
		{
			get;
			private set;
		}

		public static int COLUMN_troopData
		{
			get;
			private set;
		}

		public static int COLUMN_specialAttackData
		{
			get;
			private set;
		}

		public static int COLUMN_heroData
		{
			get;
			private set;
		}

		public static int COLUMN_championData
		{
			get;
			private set;
		}

		public static int COLUMN_multipleHeroDeploys
		{
			get;
			private set;
		}

		public static int COLUMN_overridePlayerUnits
		{
			get;
			private set;
		}

		public static int COLUMN_battleTime
		{
			get;
			private set;
		}

		public static int COLUMN_encounterProfile
		{
			get;
			private set;
		}

		public static int COLUMN_BattleScript
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string BattleName
		{
			get;
			set;
		}

		public string AssetName
		{
			get;
			set;
		}

		public string BundleName
		{
			get;
			set;
		}

		public string Planet
		{
			get;
			set;
		}

		public string DefenderId
		{
			get;
			set;
		}

		public int BattleTime
		{
			get;
			set;
		}

		public bool MultipleHeroDeploys
		{
			get;
			set;
		}

		public bool OverridePlayerUnits
		{
			get;
			set;
		}

		public string EncounterProfile
		{
			get;
			set;
		}

		public string BattleScript
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.BattleName = row.TryGetString(BattleTypeVO.COLUMN_battleName);
			this.AssetName = row.TryGetString(BattleTypeVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(BattleTypeVO.COLUMN_bundleName);
			this.Planet = row.TryGetString(BattleTypeVO.COLUMN_planet);
			this.DefenderId = row.TryGetString(BattleTypeVO.COLUMN_defenderName);
			base.TroopData = this.GetData(row, BattleTypeVO.COLUMN_troopData);
			base.SpecialAttackData = this.GetData(row, BattleTypeVO.COLUMN_specialAttackData);
			base.HeroData = this.GetData(row, BattleTypeVO.COLUMN_heroData);
			base.ChampionData = this.GetData(row, BattleTypeVO.COLUMN_championData);
			this.MultipleHeroDeploys = row.TryGetBool(BattleTypeVO.COLUMN_multipleHeroDeploys);
			this.OverridePlayerUnits = row.TryGetBool(BattleTypeVO.COLUMN_overridePlayerUnits);
			this.BattleTime = row.TryGetInt(BattleTypeVO.COLUMN_battleTime);
			this.EncounterProfile = row.TryGetString(BattleTypeVO.COLUMN_encounterProfile);
			this.BattleScript = row.TryGetString(BattleTypeVO.COLUMN_BattleScript);
		}

		private Dictionary<string, int> GetData(Row row, int columnIndex)
		{
			ValueObjectController valueObjectController = Service.ValueObjectController;
			List<StrIntPair> strIntPairs = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(columnIndex));
			if (strIntPairs == null)
			{
				return null;
			}
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			int i = 0;
			int count = strIntPairs.Count;
			while (i < count)
			{
				StrIntPair strIntPair = strIntPairs[i];
				dictionary.Add(strIntPair.StrKey, strIntPair.IntVal);
				i++;
			}
			return dictionary;
		}
	}
}
