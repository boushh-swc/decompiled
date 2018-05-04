using System;

namespace StaRTS.Externals.Manimal
{
	public enum DesyncType
	{
		CriticalCommandFail = 0,
		BatchMaxRetry = 1,
		CommandMaxRetry = 2,
		ReceiptVerificationFailed = 3
	}
}
