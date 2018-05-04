using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Squads.Requests
{
	public class GetSquadNotifsRequest : PlayerIdRequest
	{
		public uint Since
		{
			get;
			set;
		}

		public string BattleVersion
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.Add<uint>("since", this.Since);
			serializer.AddString("battleVersion", this.BattleVersion);
			return serializer.End().ToString();
		}
	}
}
