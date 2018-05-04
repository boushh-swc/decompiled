using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetResourcesRequest : PlayerIdRequest
	{
		public int Credits
		{
			get;
			set;
		}

		public int Materials
		{
			get;
			set;
		}

		public int Contraband
		{
			get;
			set;
		}

		public int Crystals
		{
			get;
			set;
		}

		public int Reputation
		{
			get;
			set;
		}

		public CheatSetResourcesRequest()
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.Credits = -1;
			this.Materials = -1;
			this.Contraband = -1;
			this.Crystals = -1;
			this.Reputation = -1;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			if (this.Credits >= 0)
			{
				serializer.Add<int>("credits", this.Credits);
			}
			if (this.Materials >= 0)
			{
				serializer.Add<int>("materials", this.Materials);
			}
			if (this.Contraband >= 0)
			{
				serializer.Add<int>("contraband", this.Contraband);
			}
			if (this.Crystals >= 0)
			{
				serializer.Add<int>("crystals", this.Crystals);
			}
			if (this.Reputation >= 0)
			{
				serializer.Add<int>("reputation", this.Reputation);
			}
			return serializer.End().ToString();
		}
	}
}
