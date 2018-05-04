using System;

namespace StaRTS.Externals.BI
{
	public interface ILogCreator
	{
		BILogData CreateWWWDataFromBILog(BILog log);
	}
}
