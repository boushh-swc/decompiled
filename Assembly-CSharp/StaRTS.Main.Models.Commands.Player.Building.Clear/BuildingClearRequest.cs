using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Clear
{
	public class BuildingClearRequest : PlayerIdChecksumRequest
	{
		public string InstanceId
		{
			get;
			set;
		}

		public bool PayWithHardCurrency
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("instanceId", this.InstanceId);
			startedSerializer.AddBool("payWithHardCurrency", this.PayWithHardCurrency);
			return startedSerializer.End().ToString();
		}
	}
}
