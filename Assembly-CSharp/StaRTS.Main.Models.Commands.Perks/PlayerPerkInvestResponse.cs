using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Perks
{
	public class PlayerPerkInvestResponse : AbstractResponse
	{
		private const string PERK_STATUS = "perkStatus";

		private const string SQUAD_LEVEL = "guildLevel";

		private const string SQUAD_TOTAL_INVESTMENT = "totalRepInvested";

		private const string NEW_REP_INVESTED = "newRepInvested";

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			if (dictionary.ContainsKey("perkStatus"))
			{
				currentSquad.UpdateSquadPerks(dictionary["perkStatus"]);
			}
			if (dictionary.ContainsKey("guildLevel") && dictionary.ContainsKey("totalRepInvested"))
			{
				int squadLevel = Convert.ToInt32(dictionary["guildLevel"]);
				int totalRepInvested = Convert.ToInt32(dictionary["totalRepInvested"]);
				currentSquad.UpdateSquadLevel(squadLevel, totalRepInvested);
			}
			if (dictionary.ContainsKey("newRepInvested"))
			{
				Service.SquadController.StateManager.NumRepDonatedInSession += Convert.ToInt32(dictionary["newRepInvested"]);
			}
			Service.EventManager.SendEvent(EventId.SquadPerkUpdated, null);
			return this;
		}
	}
}
