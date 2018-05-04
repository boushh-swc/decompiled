using System;

public interface IResourceFillable
{
	float CurrentFullnessPercentage
	{
		get;
		set;
	}

	float PreviousFullnessPercentage
	{
		get;
		set;
	}
}
