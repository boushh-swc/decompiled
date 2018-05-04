using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Squads.Requests
{
	public class EditSquadRequest : PlayerIdRequest
	{
		public bool OpenSquad
		{
			get;
			set;
		}

		public int MinTrophy
		{
			get;
			set;
		}

		public string Icon
		{
			get;
			set;
		}

		public string Desc
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddBool("openEnrollment", this.OpenSquad);
			serializer.Add<int>("minScoreAtEnrollment", this.MinTrophy);
			serializer.AddString("icon", this.Icon);
			serializer.AddString("description", this.Desc);
			return serializer.End().ToString();
		}
	}
}
