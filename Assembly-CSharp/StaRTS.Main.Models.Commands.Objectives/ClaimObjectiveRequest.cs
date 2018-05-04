using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Objectives
{
	public class ClaimObjectiveRequest : PlayerIdRequest
	{
		public string PlanetId
		{
			get;
			set;
		}

		public string ObjectiveId
		{
			get;
			set;
		}

		public ClaimObjectiveRequest(string playerId, string planetId, string objectiveId)
		{
			this.PlanetId = planetId;
			base.PlayerId = playerId;
			this.ObjectiveId = objectiveId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			if (!string.IsNullOrEmpty(this.PlanetId) && !string.IsNullOrEmpty(this.ObjectiveId))
			{
				serializer.AddString("planetId", this.PlanetId);
				serializer.AddString("objectiveId", this.ObjectiveId);
			}
			return serializer.End().ToString();
		}
	}
}
