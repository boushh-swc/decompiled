using System;

namespace StaRTS.Externals.BI
{
	public interface IDeviceInfoController
	{
		void AddDeviceSpecificInfo(BILog log);

		string GetDeviceId();
	}
}
