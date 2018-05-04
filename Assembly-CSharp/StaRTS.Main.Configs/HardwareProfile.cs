using System;
using UnityEngine;

namespace StaRTS.Main.Configs
{
	public static class HardwareProfile
	{
		private const string DEVICE_IPOD5 = "iPod5,1";

		private const string DEVICE_IPHONE4_1 = "iPhone3,1";

		private const string DEVICE_IPHONE4_2 = "iPhone3,2";

		private const string DEVICE_IPHONE4_3 = "iPhone3,3";

		private const string DEVICE_IPHONE4S = "iPhone4,1";

		private const string DEVICE_IPAD2_1 = "iPad2,1";

		private const string DEVICE_IPAD2_2 = "iPad2,2";

		private const string DEVICE_IPAD2_3 = "iPad2,3";

		private const string DEVICE_IPAD2_4 = "iPad2,4";

		private const string DEVICE_IPADMINI_1 = "iPad2,5";

		private const string DEVICE_IPADMINI_2 = "iPad2,6";

		private const string DEVICE_IPADMINI_3 = "iPad2,7";

		private const string DEVICE_IPAD3_1 = "iPad3,1";

		private const string DEVICE_IPAD3_2 = "iPad3,2";

		private const string DEVICE_IPAD3_3 = "iPad3,3";

		public static bool IsLowEndDevice()
		{
			int hardwareProfile = PlayerSettings.GetHardwareProfile();
			return hardwareProfile != 1 && (hardwareProfile == 2 || true);
		}

		public static string GetDeviceModel()
		{
			return SystemInfo.deviceModel;
		}
	}
}
