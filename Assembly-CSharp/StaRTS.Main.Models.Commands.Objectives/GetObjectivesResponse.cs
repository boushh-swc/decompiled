using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Player.Objectives;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Objectives
{
	public class GetObjectivesResponse : AbstractResponse
	{
		public Dictionary<string, ObjectiveGroup> Groups;

		public override ISerializable FromObject(object obj)
		{
			this.Groups = new Dictionary<string, ObjectiveGroup>();
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				foreach (KeyValuePair<string, object> current in dictionary)
				{
					this.Groups.Add(current.Key, new ObjectiveGroup(current.Key).FromObject(current.Value) as ObjectiveGroup);
				}
			}
			return this;
		}
	}
}
