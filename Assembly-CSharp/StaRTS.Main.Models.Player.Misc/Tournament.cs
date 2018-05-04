using StaRTS.Main.Models.Commands.Tournament;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player.Misc
{
	public class Tournament : AbstractTimedEvent
	{
		public int Rating
		{
			get;
			set;
		}

		public List<string> RedeemedRewards
		{
			get;
			set;
		}

		public TournamentRank CurrentRank
		{
			get;
			set;
		}

		public TournamentRank FinalRank
		{
			get;
			set;
		}

		public int BestTier
		{
			get;
			set;
		}

		public Tournament()
		{
			this.RedeemedRewards = new List<string>();
			this.CurrentRank = new TournamentRank();
			this.FinalRank = new TournamentRank();
		}

		public override ISerializable FromObject(object obj)
		{
			base.FromObject(obj);
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				if (base.Collected)
				{
					this.FinalRank.FromObject(obj);
				}
				if (dictionary.ContainsKey("rating"))
				{
					this.Rating = Convert.ToInt32(dictionary["rating"]);
				}
				if (dictionary.ContainsKey("bestTier"))
				{
					this.BestTier = Convert.ToInt32(dictionary["bestTier"]);
				}
				this.RedeemedRewards.Clear();
				if (dictionary.ContainsKey("redeemedRewards"))
				{
					List<object> list = dictionary["redeemedRewards"] as List<object>;
					if (list != null)
					{
						int count = list.Count;
						for (int i = 0; i < count; i++)
						{
							this.RedeemedRewards.Add(Convert.ToString(list[i]));
						}
					}
				}
			}
			return this;
		}

		public void Sync(Tournament tournament)
		{
			if (tournament == null)
			{
				return;
			}
			if (base.Uid != tournament.Uid)
			{
				Service.Logger.Error("Trying to sync mismatched tournament data.");
				return;
			}
			base.Collected = tournament.Collected;
			this.Rating = tournament.Rating;
			this.FinalRank = tournament.FinalRank;
			this.RedeemedRewards = new List<string>(tournament.RedeemedRewards);
			this.BestTier = tournament.BestTier;
		}

		public void UpdateRatingAndCurrentRank(object obj)
		{
			if (obj != null)
			{
				Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
				if (dictionary != null && dictionary.ContainsKey("value"))
				{
					this.Rating = Convert.ToInt32(dictionary["value"]);
				}
				this.CurrentRank.FromObject(obj);
			}
		}
	}
}
