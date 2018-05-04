using System;

namespace StaRTS.Utils.Scheduling
{
	public interface ISimTimeProvider
	{
		uint Now
		{
			get;
		}
	}
}
