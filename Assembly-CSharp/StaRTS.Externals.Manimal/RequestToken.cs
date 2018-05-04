using System;

namespace StaRTS.Externals.Manimal
{
	public static class RequestToken
	{
		public static string Get()
		{
			return Guid.NewGuid().ToString();
		}
	}
}
