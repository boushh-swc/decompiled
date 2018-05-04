using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public interface ILimitedEditionItemVO : IValueObject
	{
		int StartTime
		{
			get;
			set;
		}

		int EndTime
		{
			get;
			set;
		}

		string[] AudienceConditions
		{
			get;
			set;
		}
	}
}
