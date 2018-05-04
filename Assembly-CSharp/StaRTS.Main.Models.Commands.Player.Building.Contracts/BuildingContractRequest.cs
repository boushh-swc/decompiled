using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Contracts
{
	public class BuildingContractRequest : PlayerIdChecksumRequest
	{
		protected string buildingKey;

		protected string tag;

		protected override bool CalculateChecksumManually
		{
			get
			{
				return true;
			}
		}

		public BuildingContractRequest()
		{
		}

		public BuildingContractRequest(string buildingKey, bool isCancelOrBuyout, string tag)
		{
			this.buildingKey = buildingKey;
			this.tag = tag;
			bool simulateTroopContractUpdate = !isCancelOrBuyout;
			base.CalculateChecksum(null, false, simulateTroopContractUpdate);
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("instanceId", this.buildingKey);
			if (!string.IsNullOrEmpty(this.tag))
			{
				startedSerializer.AddString("tag", this.tag);
			}
			return startedSerializer.End().ToString();
		}
	}
}
