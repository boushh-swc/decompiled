using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetWarRatingRequest : PlayerIdRequest
	{
		public string SquadId;

		public int Rating
		{
			get;
			private set;
		}

		public CheatSetWarRatingRequest(string squadId, int rating)
		{
			this.SquadId = squadId;
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.Rating = rating;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("guildId", this.SquadId);
			serializer.AddString("rating", this.Rating.ToString());
			return serializer.End().ToString();
		}
	}
}
