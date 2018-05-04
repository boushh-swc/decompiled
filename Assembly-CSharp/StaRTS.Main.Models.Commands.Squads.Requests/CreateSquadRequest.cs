using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Squads.Requests
{
	public class CreateSquadRequest : PlayerIdRequest
	{
		private bool openSquad;

		private int minTrophy;

		private string icon;

		private string name;

		private string desc;

		public CreateSquadRequest(string name, string desc, string icon, int minTrophy, bool openSquad)
		{
			this.name = name;
			this.desc = desc;
			this.icon = icon;
			this.minTrophy = minTrophy;
			this.openSquad = openSquad;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddBool("openEnrollment", this.openSquad);
			serializer.Add<int>("minScoreAtEnrollment", this.minTrophy);
			serializer.AddString("icon", this.icon);
			serializer.AddString("name", this.name);
			serializer.AddString("description", this.desc);
			return serializer.End().ToString();
		}
	}
}
