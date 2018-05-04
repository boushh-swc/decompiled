using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Squads;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class GetSquadInvitesResponse : AbstractResponse
	{
		public List<SquadInvite> SquadInvites
		{
			get;
			private set;
		}

		public GetSquadInvitesResponse()
		{
			this.SquadInvites = new List<SquadInvite>();
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
					SquadInvite squadInvite = new SquadInvite();
					squadInvite.FromObject(list[i]);
					this.SquadInvites.Add(squadInvite);
					i++;
				}
			}
			return this;
		}
	}
}
