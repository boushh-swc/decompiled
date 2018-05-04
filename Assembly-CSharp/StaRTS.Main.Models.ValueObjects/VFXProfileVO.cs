using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class VFXProfileVO : IValueObject
	{
		public static int COLUMN_default
		{
			get;
			private set;
		}

		public static int COLUMN_wall
		{
			get;
			private set;
		}

		public static int COLUMN_building
		{
			get;
			private set;
		}

		public static int COLUMN_storage
		{
			get;
			private set;
		}

		public static int COLUMN_resource
		{
			get;
			private set;
		}

		public static int COLUMN_turret
		{
			get;
			private set;
		}

		public static int COLUMN_HQ
		{
			get;
			private set;
		}

		public static int COLUMN_shield
		{
			get;
			private set;
		}

		public static int COLUMN_shieldGenerator
		{
			get;
			private set;
		}

		public static int COLUMN_infantry
		{
			get;
			private set;
		}

		public static int COLUMN_bruiserInfantry
		{
			get;
			private set;
		}

		public static int COLUMN_vehicle
		{
			get;
			private set;
		}

		public static int COLUMN_bruiserVehicle
		{
			get;
			private set;
		}

		public static int COLUMN_heroInfantry
		{
			get;
			private set;
		}

		public static int COLUMN_heroVehicle
		{
			get;
			private set;
		}

		public static int COLUMN_heroBruiserInfantry
		{
			get;
			private set;
		}

		public static int COLUMN_heroBruiserVehicle
		{
			get;
			private set;
		}

		public static int COLUMN_flierInfantry
		{
			get;
			private set;
		}

		public static int COLUMN_flierVehicle
		{
			get;
			private set;
		}

		public static int COLUMN_healerInfantry
		{
			get;
			private set;
		}

		public static int COLUMN_trap
		{
			get;
			private set;
		}

		public static int COLUMN_champion
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string[] Values
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			string text = row.TryGetString(VFXProfileVO.COLUMN_default);
			this.Values = new string[24];
			for (int i = 0; i < 24; i++)
			{
				this.Values[i] = text;
			}
			this.Values[1] = row.TryGetString(VFXProfileVO.COLUMN_wall, text);
			this.Values[2] = row.TryGetString(VFXProfileVO.COLUMN_building, text);
			this.Values[3] = row.TryGetString(VFXProfileVO.COLUMN_storage, text);
			this.Values[4] = row.TryGetString(VFXProfileVO.COLUMN_resource, text);
			this.Values[5] = row.TryGetString(VFXProfileVO.COLUMN_turret, text);
			this.Values[6] = row.TryGetString(VFXProfileVO.COLUMN_HQ, text);
			this.Values[7] = row.TryGetString(VFXProfileVO.COLUMN_shield, text);
			this.Values[8] = row.TryGetString(VFXProfileVO.COLUMN_shieldGenerator, text);
			this.Values[9] = row.TryGetString(VFXProfileVO.COLUMN_infantry, text);
			this.Values[10] = row.TryGetString(VFXProfileVO.COLUMN_bruiserInfantry, text);
			this.Values[11] = row.TryGetString(VFXProfileVO.COLUMN_vehicle, text);
			this.Values[12] = row.TryGetString(VFXProfileVO.COLUMN_bruiserVehicle, text);
			this.Values[13] = row.TryGetString(VFXProfileVO.COLUMN_heroInfantry, text);
			this.Values[14] = row.TryGetString(VFXProfileVO.COLUMN_heroVehicle, text);
			this.Values[15] = row.TryGetString(VFXProfileVO.COLUMN_heroBruiserInfantry, text);
			this.Values[16] = row.TryGetString(VFXProfileVO.COLUMN_heroBruiserVehicle, text);
			this.Values[17] = row.TryGetString(VFXProfileVO.COLUMN_flierInfantry, text);
			this.Values[18] = row.TryGetString(VFXProfileVO.COLUMN_flierVehicle, text);
			this.Values[19] = row.TryGetString(VFXProfileVO.COLUMN_healerInfantry, text);
			this.Values[20] = row.TryGetString(VFXProfileVO.COLUMN_trap, text);
			this.Values[21] = row.TryGetString(VFXProfileVO.COLUMN_champion, text);
		}
	}
}
