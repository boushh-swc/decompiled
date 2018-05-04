using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class TransmissionCharacterVO : IValueObject
	{
		public static int COLUMN_planetId
		{
			get;
			private set;
		}

		public static int COLUMN_faction
		{
			get;
			private set;
		}

		public static int COLUMN_characterName
		{
			get;
			private set;
		}

		public static int COLUMN_characterId
		{
			get;
			private set;
		}

		public static int COLUMN_image
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string PlanetId
		{
			get;
			set;
		}

		public FactionType Faction
		{
			get;
			set;
		}

		public string CharacterName
		{
			get;
			set;
		}

		public string CharacterId
		{
			get;
			set;
		}

		public string Image
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.PlanetId = row.TryGetString(TransmissionCharacterVO.COLUMN_planetId);
			this.CharacterName = row.TryGetString(TransmissionCharacterVO.COLUMN_characterName);
			this.CharacterId = row.TryGetString(TransmissionCharacterVO.COLUMN_characterId);
			this.Image = row.TryGetString(TransmissionCharacterVO.COLUMN_image);
			this.Faction = StringUtils.ParseEnum<FactionType>(row.TryGetString(TransmissionCharacterVO.COLUMN_faction));
		}
	}
}
