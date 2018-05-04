using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class StoryVO : IValueObject
	{
		public static int COLUMN_character
		{
			get;
			private set;
		}

		public static int COLUMN_side
		{
			get;
			private set;
		}

		public static int COLUMN_transcript
		{
			get;
			private set;
		}

		public static int COLUMN_audio_asset
		{
			get;
			private set;
		}

		public static int COLUMN_next_step
		{
			get;
			private set;
		}

		public static int COLUMN_next_step_data
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string Character
		{
			get;
			set;
		}

		public string Side
		{
			get;
			set;
		}

		public string Transcript
		{
			get;
			set;
		}

		public string AudioAsset
		{
			get;
			set;
		}

		public string NextStep
		{
			get;
			set;
		}

		public string NextStepData
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Character = row.TryGetString(StoryVO.COLUMN_character);
			this.Side = row.TryGetString(StoryVO.COLUMN_side);
			this.Transcript = row.TryGetString(StoryVO.COLUMN_transcript);
			this.AudioAsset = row.TryGetString(StoryVO.COLUMN_audio_asset);
			this.NextStep = row.TryGetString(StoryVO.COLUMN_next_step);
			this.NextStepData = row.TryGetString(StoryVO.COLUMN_next_step_data);
		}
	}
}
