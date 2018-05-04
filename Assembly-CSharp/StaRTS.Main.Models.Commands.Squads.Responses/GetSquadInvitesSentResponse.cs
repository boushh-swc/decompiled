using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Squads.Responses
{
	public class GetSquadInvitesSentResponse : AbstractResponse
	{
		public List<string> PlayersWithPendingInvites
		{
			get;
			private set;
		}

		public GetSquadInvitesSentResponse()
		{
			this.PlayersWithPendingInvites = new List<string>();
		}

		public override ISerializable FromObject(object obj)
		{
			List<object> list = obj as List<object>;
			if (list != null)
			{
				int i = 0;
				int count = list.Count;
				while (i < count)
				{
					this.PlayersWithPendingInvites.Add(list[i] as string);
					i++;
				}
			}
			return this;
		}
	}
}
