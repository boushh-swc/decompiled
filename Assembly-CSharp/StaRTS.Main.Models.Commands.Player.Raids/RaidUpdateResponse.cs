using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Player.Raids
{
	public class RaidUpdateResponse : AbstractResponse
	{
		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> raidData = obj as Dictionary<string, object>;
			Service.CurrentPlayer.SetupRaidFromDictionary(raidData);
			return this;
		}
	}
}
