using System;

namespace StaRTS.Externals.Manimal
{
	public static class RequestId
	{
		public static uint Id
		{
			get;
			private set;
		}

		public static void Reset()
		{
			RequestId.Id = 1u;
		}

		public static uint Get()
		{
			return RequestId.Id += 1u;
		}
	}
}
