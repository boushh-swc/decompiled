using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Missions
{
	public class GetMissionMapResponse : AbstractResponse
	{
		public string BattleUid
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			this.BattleUid = (string)obj;
			return this;
		}
	}
}
