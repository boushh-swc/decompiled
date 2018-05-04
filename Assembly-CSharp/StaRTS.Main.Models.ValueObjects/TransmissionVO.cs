using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Utils;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace StaRTS.Main.Models.ValueObjects
{
	public class TransmissionVO : ITimestamped, IValueObject, ICallToAction
	{
		private const string SERVER_START_DATE = "postingDate";

		private const string SERVER_SOURCE_TYPE = "eventType";

		private const string SERVER_SOURCE_LEVEL = "level";

		private const string SERVER_SOURCE_UID = "uid";

		private const string SERVER_SOURCE_TIER = "tier";

		private const string SERVER_SOURCE_EMPIRE_NAME = "empireName";

		private const string SERVER_SOURCE_EMPIRE_SCORE = "empireScore";

		private const string SERVER_SOURCE_REBEL_NAME = "rebelName";

		private const string SERVER_SOURCE_REBEL_SCORE = "rebelScore";

		private const string SERVER_END_DATE = "_expirationDate";

		private const string CONFLICT_ACTION_TYPE = "conflictEnd";

		private const string WAR_BOARD_CTA = "gotowarboard";

		private const string CURRENT_SQUAD_NAME = "guildName";

		private const string SQUAD_LEVEL = "guildLevel";

		private const string SQUAD_LEVELUP_CTA = "squadlevelup";

		private const string CRATE_ID = "crateId";

		private const string DAILY_CRATE_REWARD_CTA = "dailycratereward";

		private const int DEFAULT_PRIORITY = 0;

		private const int BATTLE_PRIORITY = 1;

		private const int SQUAD_LEVEL_UP_PRIORITY = 2;

		private const int CONFLICT_PRIORITY = 3;

		private const int SQUAD_WAR_PRIORITY = 4;

		private const int DAILY_CRATE_REWARD_PRIORITY = 5;

		public static int COLUMN_titleText
		{
			get;
			private set;
		}

		public static int COLUMN_bodyText
		{
			get;
			private set;
		}

		public static int COLUMN_startDate
		{
			get;
			private set;
		}

		public static int COLUMN_endDate
		{
			get;
			private set;
		}

		public static int COLUMN_faction
		{
			get;
			private set;
		}

		public static int COLUMN_npc
		{
			get;
			private set;
		}

		public static int COLUMN_actionType
		{
			get;
			private set;
		}

		public static int COLUMN_actionData
		{
			get;
			private set;
		}

		public static int COLUMN_actionDisplay
		{
			get;
			private set;
		}

		public static int COLUMN_image
		{
			get;
			private set;
		}

		public static int COLUMN_btn1
		{
			get;
			private set;
		}

		public static int COLUMN_btn1action
		{
			get;
			private set;
		}

		public static int COLUMN_btn1data
		{
			get;
			private set;
		}

		public static int COLUMN_btn2
		{
			get;
			private set;
		}

		public static int COLUMN_btn2action
		{
			get;
			private set;
		}

		public static int COLUMN_btn2data
		{
			get;
			private set;
		}

		public static int COLUMN_transType
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string TitleText
		{
			get;
			set;
		}

		public string BodyText
		{
			get;
			set;
		}

		public int StartTime
		{
			get;
			set;
		}

		public int EndTime
		{
			get;
			set;
		}

		public FactionType Faction
		{
			get;
			set;
		}

		public string CharacterID
		{
			get;
			set;
		}

		public string Image
		{
			get;
			set;
		}

		public string Btn1
		{
			get;
			set;
		}

		public string Btn1Action
		{
			get;
			set;
		}

		public string Btn1Data
		{
			get;
			set;
		}

		public string Btn2
		{
			get;
			set;
		}

		public string Btn2Action
		{
			get;
			set;
		}

		public string Btn2Data
		{
			get;
			set;
		}

		public TransmissionType Type
		{
			get;
			set;
		}

		public int Priority
		{
			get;
			private set;
		}

		public string TransData
		{
			get;
			set;
		}

		public List<BattleEntry> AttackerData
		{
			get;
			private set;
		}

		public int TotalPvpRatingDelta
		{
			get;
			private set;
		}

		public string CurrentSquadName
		{
			get;
			private set;
		}

		public int SquadLevel
		{
			get;
			private set;
		}

		public string CrateId
		{
			get;
			set;
		}

		public string EmpireSquadName
		{
			get;
			set;
		}

		public int EmpireScore
		{
			get;
			set;
		}

		public string RebelSquadName
		{
			get;
			set;
		}

		public int RebelScore
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.TitleText = row.TryGetString(TransmissionVO.COLUMN_titleText);
			this.BodyText = row.TryGetString(TransmissionVO.COLUMN_bodyText);
			string text = row.TryGetString(TransmissionVO.COLUMN_startDate);
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					DateTime date = DateTime.ParseExact(text, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
					this.StartTime = DateUtils.GetSecondsFromEpoch(date);
				}
				catch (Exception)
				{
					this.StartTime = 0;
					Service.Logger.Warn("TransmissionVO:: Transmission Holonet CMS Start Date Format Error: " + this.Uid);
				}
			}
			else
			{
				this.StartTime = 0;
				Service.Logger.Warn("TransmissionVO:: Transmission Holonet CMS Start Date Not Specified For: " + this.Uid);
			}
			string text2 = row.TryGetString(TransmissionVO.COLUMN_endDate);
			if (!string.IsNullOrEmpty(text2))
			{
				try
				{
					DateTime date2 = DateTime.ParseExact(text2, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
					this.EndTime = DateUtils.GetSecondsFromEpoch(date2);
				}
				catch (Exception)
				{
					this.EndTime = 2147483647;
					Service.Logger.Warn("TransmissionVO:: Transmission Holonet CMS End Date Format Error: " + this.Uid);
				}
			}
			else
			{
				this.EndTime = 2147483647;
			}
			this.Faction = StringUtils.ParseEnum<FactionType>(row.TryGetString(TransmissionVO.COLUMN_faction));
			this.CharacterID = row.TryGetString(TransmissionVO.COLUMN_npc);
			this.Image = row.TryGetString(TransmissionVO.COLUMN_image);
			this.Btn1 = row.TryGetString(TransmissionVO.COLUMN_btn1);
			this.Btn1Action = row.TryGetString(TransmissionVO.COLUMN_btn1action);
			this.Btn1Data = row.TryGetString(TransmissionVO.COLUMN_btn1data);
			this.Btn2 = row.TryGetString(TransmissionVO.COLUMN_btn2);
			this.Btn2Action = row.TryGetString(TransmissionVO.COLUMN_btn2action);
			this.Btn2Data = row.TryGetString(TransmissionVO.COLUMN_btn2data);
			this.Type = StringUtils.ParseEnum<TransmissionType>(row.TryGetString(TransmissionVO.COLUMN_transType));
			this.Priority = 0;
		}

		public void InitBattleData(List<BattleEntry> battles)
		{
			this.Priority = 1;
			this.AttackerData = battles;
			this.TotalPvpRatingDelta = 0;
			int count = battles.Count;
			for (int i = 0; i < count; i++)
			{
				BattleEntry battleEntry = battles[i];
				BattleParticipant defender = battleEntry.Defender;
				int num = GameUtils.CalcuateMedals(defender.AttackRating, defender.DefenseRating);
				int num2 = GameUtils.CalcuateMedals(defender.AttackRating + defender.AttackRatingDelta, defender.DefenseRating + defender.DefenseRatingDelta);
				this.TotalPvpRatingDelta += num2 - num;
			}
		}

		public void ResetAttackerData()
		{
			if (this.AttackerData != null)
			{
				this.AttackerData.Clear();
				this.AttackerData = null;
			}
		}

		public static TransmissionVO CreateFromServerObject(object data)
		{
			TransmissionVO transmissionVO = new TransmissionVO();
			Dictionary<string, object> dictionary = data as Dictionary<string, object>;
			transmissionVO.Priority = 0;
			StringBuilder stringBuilder = new StringBuilder();
			if (dictionary.ContainsKey("postingDate"))
			{
				transmissionVO.StartTime = Convert.ToInt32((string)dictionary["postingDate"]);
			}
			if (dictionary.ContainsKey("eventType"))
			{
				string text = (string)dictionary["eventType"];
				transmissionVO.Type = StringUtils.ParseEnum<TransmissionType>(text);
				stringBuilder.Append(text);
			}
			if (dictionary.ContainsKey("tier"))
			{
				transmissionVO.Priority = 3;
				string text2 = (string)dictionary["tier"];
				transmissionVO.Btn1Data = text2;
				transmissionVO.Btn1Action = "conflictEnd";
				stringBuilder.Append(text2);
			}
			else if (dictionary.ContainsKey("level"))
			{
				string text3 = (string)dictionary["level"];
				transmissionVO.Btn1Data = text3;
				stringBuilder.Append(text3);
			}
			if (dictionary.ContainsKey("uid"))
			{
				string text4 = (string)dictionary["uid"];
				transmissionVO.TransData = text4;
				stringBuilder.Append(text4);
			}
			if (dictionary.ContainsKey("empireName"))
			{
				string text5 = (string)dictionary["empireName"];
				transmissionVO.EmpireSquadName = WWW.UnEscapeURL(text5);
				stringBuilder.Append(text5);
			}
			if (dictionary.ContainsKey("empireScore"))
			{
				int num = Convert.ToInt32(dictionary["empireScore"]);
				transmissionVO.EmpireScore = num;
				stringBuilder.Append(num);
			}
			if (dictionary.ContainsKey("rebelName"))
			{
				string text6 = (string)dictionary["rebelName"];
				transmissionVO.RebelSquadName = WWW.UnEscapeURL(text6);
				stringBuilder.Append(text6);
			}
			if (dictionary.ContainsKey("rebelScore"))
			{
				int num2 = Convert.ToInt32(dictionary["rebelScore"]);
				transmissionVO.RebelScore = num2;
				stringBuilder.Append(num2);
			}
			if (transmissionVO.Type == TransmissionType.WarStart || transmissionVO.Type == TransmissionType.WarEnded || transmissionVO.Type == TransmissionType.WarPreparation)
			{
				transmissionVO.Priority = 4;
				transmissionVO.Btn1Action = "gotowarboard";
			}
			if (dictionary.ContainsKey("guildName"))
			{
				string s = (string)dictionary["guildName"];
				transmissionVO.CurrentSquadName = WWW.UnEscapeURL(s);
			}
			if (dictionary.ContainsKey("guildLevel"))
			{
				transmissionVO.SquadLevel = Convert.ToInt32(dictionary["guildLevel"]);
				transmissionVO.Btn1Action = "squadlevelup";
				transmissionVO.Priority = 2;
			}
			if (dictionary.ContainsKey("crateId"))
			{
				transmissionVO.CrateId = (string)dictionary["crateId"];
				transmissionVO.Btn1Action = "dailycratereward";
				transmissionVO.Priority = 5;
			}
			transmissionVO.Uid = stringBuilder.ToString();
			transmissionVO.EndTime = 2147483647;
			return transmissionVO;
		}
	}
}
