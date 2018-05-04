using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.ValueObjects
{
	public interface IAudioVO
	{
		List<StrIntPair> AudioCharge
		{
			get;
			set;
		}

		List<StrIntPair> AudioAttack
		{
			get;
			set;
		}

		List<StrIntPair> AudioDeath
		{
			get;
			set;
		}

		List<StrIntPair> AudioPlacement
		{
			get;
			set;
		}

		List<StrIntPair> AudioMovement
		{
			get;
			set;
		}

		List<StrIntPair> AudioMovementAway
		{
			get;
			set;
		}

		List<StrIntPair> AudioImpact
		{
			get;
			set;
		}

		List<StrIntPair> AudioTrain
		{
			get;
			set;
		}
	}
}
