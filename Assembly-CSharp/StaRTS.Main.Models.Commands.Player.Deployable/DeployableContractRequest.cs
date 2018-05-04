using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Deployable
{
	public class DeployableContractRequest : PlayerIdChecksumRequest
	{
		public string ConstructorBuildingId
		{
			get;
			private set;
		}

		public string UnitTypeUid
		{
			get;
			private set;
		}

		public int Quantity
		{
			get;
			private set;
		}

		public DeployableContractRequest(string constructorBuildingId, string unitTypeUid, int quantity)
		{
			this.ConstructorBuildingId = constructorBuildingId;
			this.UnitTypeUid = unitTypeUid;
			this.Quantity = quantity;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("constructor", this.ConstructorBuildingId);
			startedSerializer.AddString("unitTypeId", this.UnitTypeUid);
			startedSerializer.Add<int>("quantity", this.Quantity);
			return startedSerializer.End().ToString();
		}
	}
}
