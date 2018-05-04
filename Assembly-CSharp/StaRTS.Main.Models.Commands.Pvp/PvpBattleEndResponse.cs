using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Pvp
{
	public class PvpBattleEndResponse : AbstractResponse
	{
		public BattleEntry BattleEntry
		{
			get;
			private set;
		}

		public Tournament TournamentData
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			this.BattleEntry = (new BattleEntry().FromObject(obj) as BattleEntry);
			this.BattleEntry.SetupExpendedTroops();
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null && dictionary.ContainsKey("attackerTournament"))
			{
				this.TournamentData = new Tournament();
				this.TournamentData.FromObject(dictionary["attackerTournament"]);
			}
			return this;
		}
	}
}
