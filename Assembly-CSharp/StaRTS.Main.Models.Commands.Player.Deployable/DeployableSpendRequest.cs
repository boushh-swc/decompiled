using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Battle;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Player.Deployable
{
	public class DeployableSpendRequest : PlayerIdRequest
	{
		public List<DeploymentRecord> units
		{
			get;
			set;
		}

		public string BattleId
		{
			get;
			private set;
		}

		public bool SquadDeployed
		{
			get;
			private set;
		}

		public DeployableSpendRequest(string battleId, uint time, int boardX, int boardZ)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.BattleId = battleId;
			DeploymentRecord item = new DeploymentRecord(null, "SquadTroopPlaced", time, boardX, boardZ);
			this.units = new List<DeploymentRecord>();
			this.units.Add(item);
			this.SquadDeployed = true;
		}

		public DeployableSpendRequest(string battleId, List<DeploymentRecord> units)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.BattleId = battleId;
			this.units = new List<DeploymentRecord>(units);
			this.SquadDeployed = false;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("battleId", this.BattleId);
			serializer.AddBool("guildTroopsSpent", this.SquadDeployed);
			serializer.AddArray<DeploymentRecord>("units", this.units);
			return serializer.End().ToString();
		}
	}
}
