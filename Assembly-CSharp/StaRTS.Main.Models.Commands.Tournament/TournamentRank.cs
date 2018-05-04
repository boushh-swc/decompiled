using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Tournament
{
	public class TournamentRank : AbstractResponse
	{
		public const double MAX_PERCENTILE = 100.0;

		private const double MIN_PERCENTILE = 0.01;

		public double Percentile
		{
			get;
			set;
		}

		public string TierUid
		{
			get;
			set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				bool flag = false;
				if (dictionary.ContainsKey("percentile"))
				{
					this.Percentile = 100.0 - Convert.ToDouble(dictionary["percentile"]);
					if (this.Percentile <= 0.01)
					{
						this.Percentile = 0.01;
					}
					flag = true;
				}
				else
				{
					this.Percentile = 100.0;
					Service.Logger.Error("PERCENTILE value not found in TournamentRank response");
				}
				if (dictionary.ContainsKey("tier"))
				{
					this.TierUid = Convert.ToString(dictionary["tier"]);
				}
				else if (flag)
				{
					this.TierUid = this.GetTierIdForPercentage(this.Percentile);
				}
			}
			return this;
		}

		private string GetTierIdForPercentage(double percentile)
		{
			TournamentTierVO tournamentTierVO = null;
			StaticDataController staticDataController = Service.StaticDataController;
			foreach (TournamentTierVO current in staticDataController.GetAll<TournamentTierVO>())
			{
				if (percentile <= (double)current.Percentage && (tournamentTierVO == null || tournamentTierVO.Percentage > current.Percentage))
				{
					tournamentTierVO = current;
				}
			}
			return (tournamentTierVO != null) ? tournamentTierVO.Uid : null;
		}
	}
}
