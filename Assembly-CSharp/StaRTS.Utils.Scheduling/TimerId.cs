using System;

namespace StaRTS.Utils.Scheduling
{
	public class TimerId
	{
		public const uint INVALID = 0u;

		public static uint GetNext(ref uint idLast)
		{
			if ((idLast += 1u) == 0u)
			{
				throw new Exception("Timer id rollover has occurred");
			}
			return idLast;
		}
	}
}
