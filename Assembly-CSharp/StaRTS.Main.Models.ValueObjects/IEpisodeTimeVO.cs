using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public interface IEpisodeTimeVO
	{
		DateTime StartTime
		{
			get;
			set;
		}

		DateTime EndTime
		{
			get;
			set;
		}

		int Priority
		{
			get;
			set;
		}
	}
}
