using StaRTS.Main.Models.Entities;
using System;

namespace StaRTS.Main.Models
{
	public class BuffEventData
	{
		public Buff BuffObj
		{
			get;
			private set;
		}

		public SmartEntity Target
		{
			get;
			private set;
		}

		public BuffEventData(Buff buff, SmartEntity target)
		{
			this.BuffObj = buff;
			this.Target = target;
		}
	}
}
