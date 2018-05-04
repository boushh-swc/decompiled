using System;

namespace StaRTS.FX
{
	public class ShieldReason
	{
		public ShieldLoadReason Reason
		{
			get;
			private set;
		}

		public object Cookie
		{
			get;
			private set;
		}

		public ShieldReason(ShieldLoadReason reason, object cookie)
		{
			this.Reason = reason;
			this.Cookie = cookie;
		}
	}
}
