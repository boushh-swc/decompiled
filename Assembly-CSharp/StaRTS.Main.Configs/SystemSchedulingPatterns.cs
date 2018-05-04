using System;

namespace StaRTS.Main.Configs
{
	public static class SystemSchedulingPatterns
	{
		private const ushort ALTERNATIVE_1_0 = 21845;

		private const ushort ALTERNATIVE_1_1 = 43690;

		private const ushort ALTERNATIVE_3_0 = 4369;

		private const ushort ALL = 65535;

		public const ushort BATTLE = 65535;

		public const ushort TARGETING = 65535;

		public const ushort ATTACK = 43690;

		public const ushort MOVEMENT = 43690;

		public const ushort HEALER_TARGETING = 4369;

		public const ushort TRACKING = 21845;

		public const ushort AREA_TRIGGER = 21845;

		public const ushort DROID_RENDER = 65535;

		public const ushort GENERATOR_RENDER = 65535;

		public const ushort SUPPORT_RENDER = 65535;

		public const ushort TRANSPORT_RENDER = 65535;

		public static readonly ushort ENTITY_RENDER = (!HardwareProfile.IsLowEndDevice()) ? 65535 : 21845;

		public static readonly ushort TRACKING_RENDER = (!HardwareProfile.IsLowEndDevice()) ? 65535 : 4369;

		public static readonly ushort HEALTH_RENDER = (!HardwareProfile.IsLowEndDevice()) ? 65535 : 4369;
	}
}
