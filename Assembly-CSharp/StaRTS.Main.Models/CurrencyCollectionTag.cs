using Net.RichardLord.Ash.Core;
using System;

namespace StaRTS.Main.Models
{
	public class CurrencyCollectionTag
	{
		public Entity Building
		{
			get;
			set;
		}

		public CurrencyType Type
		{
			get;
			set;
		}

		public int Delta
		{
			get;
			set;
		}
	}
}
