using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models
{
	public class SequencePair
	{
		public int[] GunSequence
		{
			get;
			private set;
		}

		public Dictionary<int, int> Sequences
		{
			get;
			private set;
		}

		public SequencePair(int[] gunSequence, Dictionary<int, int> sequences)
		{
			this.GunSequence = gunSequence;
			this.Sequences = sequences;
		}
	}
}
