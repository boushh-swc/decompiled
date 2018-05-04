using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Player.Raids
{
	public class RaidDefenseCompleteResponse : AbstractResponse
	{
		public string AwardedCrateUid
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("raidData"))
			{
				Dictionary<string, object> raidData = dictionary["raidData"] as Dictionary<string, object>;
				Service.CurrentPlayer.SetupRaidFromDictionary(raidData);
			}
			if (dictionary.ContainsKey("awardedCrateUid"))
			{
				this.AwardedCrateUid = (string)dictionary["awardedCrateUid"];
			}
			if (dictionary.ContainsKey("crates"))
			{
				Dictionary<string, object> obj2 = dictionary["crates"] as Dictionary<string, object>;
				bool flag = !string.IsNullOrEmpty(this.AwardedCrateUid);
				InventoryCrates crates = Service.CurrentPlayer.Prizes.Crates;
				if (flag)
				{
					crates.UpdateAndBadgeFromServerObject(obj2);
				}
				else
				{
					crates.FromObject(obj2);
				}
			}
			return this;
		}
	}
}
