using System;

namespace StaRTS.Externals.Manimal.TransferObjects.Request
{
	public enum OnCompleteAction
	{
		Ok = 0,
		Skip = 1,
		Retry = 2,
		Desync = 3
	}
}
