using StaRTS.Externals.Manimal;
using StaRTS.Main.Models.Commands.TransferObjects;
using StaRTS.Main.Models.Player.World;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Construct
{
	public class BuildingConstructRequest : PlayerIdChecksumRequest
	{
		public string BuildingUid
		{
			get;
			set;
		}

		public Position Position
		{
			get;
			set;
		}

		public bool PayWithHardCurrency
		{
			get;
			set;
		}

		public string Tag
		{
			get;
			set;
		}

		protected override bool CalculateChecksumManually
		{
			get
			{
				return true;
			}
		}

		public BuildingConstructRequest(string buildingKey, string buildingUid, Position position, bool payWithHardCurrency, bool instant, string tag)
		{
			this.BuildingUid = buildingUid;
			this.Position = position;
			this.PayWithHardCurrency = payWithHardCurrency;
			this.Tag = tag;
			Contract contract = null;
			if (instant)
			{
				ContractTO contractTO = new ContractTO();
				contractTO.Uid = buildingUid;
				contractTO.BuildingKey = buildingKey;
				contractTO.EndTime = ServerTime.Time;
				contractTO.ContractType = ContractType.Build;
				contract = new Contract(buildingUid, DeliveryType.Building, 0, 0.0);
				contract.ContractTO = contractTO;
			}
			base.CalculateChecksum(contract, false, true);
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("buildingUid", this.BuildingUid);
			startedSerializer.AddObject<Position>("position", this.Position);
			startedSerializer.AddBool("payWithHardCurrency", this.PayWithHardCurrency);
			startedSerializer.AddString("tag", this.Tag);
			return startedSerializer.End().ToString();
		}
	}
}
