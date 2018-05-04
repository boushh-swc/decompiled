using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.ServerMessages
{
	public class DeltaMessage : AbstractMessage
	{
		private const string ATTACK_RATING_DELTA = "scalars.attackRating";

		private const string DEFENSE_RATING_DELTA = "scalars.defenseRating";

		public int CreditsDelta
		{
			get;
			private set;
		}

		public int MaterialsDelta
		{
			get;
			private set;
		}

		public int ContrabandDelta
		{
			get;
			private set;
		}

		public int AttackRatingDelta
		{
			get;
			private set;
		}

		public int DefenseRatingDelta
		{
			get;
			private set;
		}

		public override object MessageCookie
		{
			get
			{
				return this;
			}
		}

		public override EventId MessageEventId
		{
			get
			{
				return EventId.PvpNewBattleOccured;
			}
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("credits"))
			{
				this.CreditsDelta = Convert.ToInt32(dictionary["credits"]);
			}
			if (dictionary.ContainsKey("materials"))
			{
				this.MaterialsDelta = Convert.ToInt32(dictionary["materials"]);
			}
			if (dictionary.ContainsKey("contraband"))
			{
				this.ContrabandDelta = Convert.ToInt32(dictionary["contraband"]);
			}
			if (dictionary.ContainsKey("scalars.attackRating"))
			{
				this.AttackRatingDelta = Convert.ToInt32(dictionary["scalars.attackRating"]);
			}
			if (dictionary.ContainsKey("scalars.defenseRating"))
			{
				this.AttackRatingDelta = Convert.ToInt32(dictionary["scalars.defenseRating"]);
			}
			return this;
		}
	}
}
