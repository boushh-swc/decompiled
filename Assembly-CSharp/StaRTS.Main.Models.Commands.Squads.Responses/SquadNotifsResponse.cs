using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Squads.Responses
{
	public class SquadNotifsResponse : AbstractResponse
	{
		public List<object> Notifs
		{
			get;
			set;
		}

		public override ISerializable FromObject(object obj)
		{
			this.Notifs = (obj as List<object>);
			return this;
		}
	}
}
