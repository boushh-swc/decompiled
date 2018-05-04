using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatScheduleDailyCrateRequest : PlayerIdRequest
	{
		public int RewardHour
		{
			get;
			private set;
		}

		public int RewardMinute
		{
			get;
			private set;
		}

		public CheatScheduleDailyCrateRequest(int rewardHour, int rewardMinute)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.RewardHour = rewardHour;
			this.RewardMinute = rewardMinute;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.Add<int>("rewardHour", this.RewardHour);
			serializer.Add<int>("rewardMinute", this.RewardMinute);
			return serializer.End().ToString();
		}
	}
}
