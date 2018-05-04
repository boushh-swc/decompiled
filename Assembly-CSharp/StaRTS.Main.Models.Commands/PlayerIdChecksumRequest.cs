using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands
{
	public class PlayerIdChecksumRequest : PlayerIdRequest
	{
		private int credits;

		private int materials;

		private int contraband;

		private int crystals;

		private Contract additionalContract;

		private bool instantContract;

		private long resourceChecksum;

		private long infoChecksum;

		public long CS
		{
			get;
			private set;
		}

		protected virtual bool CalculateChecksumManually
		{
			get
			{
				return false;
			}
		}

		public string ChecksumInfoString
		{
			get;
			private set;
		}

		public PlayerIdChecksumRequest()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			base.PlayerId = currentPlayer.PlayerId;
			if (!this.CalculateChecksumManually)
			{
				this.CalculateChecksum(null, false, true);
			}
		}

		protected void CalculateChecksum(Contract additionalContract, bool instantContract, bool simulateTroopContractUpdate)
		{
			this.additionalContract = additionalContract;
			this.instantContract = instantContract;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			this.credits = currentPlayer.CurrentCreditsAmount;
			this.materials = currentPlayer.CurrentMaterialsAmount;
			this.contraband = currentPlayer.CurrentContrabandAmount;
			this.crystals = currentPlayer.CurrentCrystalsAmount;
			this.ChecksumInfoString = GameUtils.CreateInfoStringForChecksum(additionalContract, instantContract, simulateTroopContractUpdate, ref this.crystals);
			this.resourceChecksum = GameUtils.CalculateResourceChecksum(this.credits, this.materials, this.contraband, this.crystals);
			this.infoChecksum = GameUtils.StringHash(this.ChecksumInfoString);
			this.CS = (this.resourceChecksum ^ this.infoChecksum);
		}

		public void RecalculateChecksum()
		{
			this.CalculateChecksum(this.additionalContract, this.instantContract, true);
		}

		public void RecalculateChecksumAfterResourceChange(int crystalDelta)
		{
			this.crystals += crystalDelta;
			this.resourceChecksum = GameUtils.CalculateResourceChecksum(this.credits, this.materials, this.contraband, this.crystals);
			this.CS = (this.resourceChecksum ^ this.infoChecksum);
		}

		protected Serializer GetStartedSerializer()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.Add<long>("cs", this.CS);
			serializer.Add<int>("_credits", this.credits);
			serializer.Add<int>("_materials", this.materials);
			serializer.Add<int>("_contraband", this.contraband);
			serializer.Add<int>("_crystals", this.crystals);
			return serializer;
		}

		public override string ToJson()
		{
			return this.GetStartedSerializer().End().ToString();
		}
	}
}
