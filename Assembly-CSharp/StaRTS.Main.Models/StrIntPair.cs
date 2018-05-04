using System;

namespace StaRTS.Main.Models
{
	public class StrIntPair
	{
		public string StrKey
		{
			get;
			private set;
		}

		public int IntVal
		{
			get;
			set;
		}

		public StrIntPair(string strKey, int intVal)
		{
			this.StrKey = strKey;
			this.IntVal = intVal;
		}
	}
}
