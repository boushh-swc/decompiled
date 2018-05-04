using System;

namespace StaRTS.DataStructures
{
	public struct IntPosition
	{
		public int x;

		public int z;

		public static IntPosition Zero
		{
			get
			{
				return new IntPosition(0, 0);
			}
		}

		public IntPosition(int x, int z)
		{
			this.x = x;
			this.z = z;
		}

		public static IntPosition operator +(IntPosition ip1, IntPosition ip2)
		{
			return new IntPosition(ip1.x + ip2.x, ip1.z + ip2.z);
		}

		public static IntPosition operator -(IntPosition ip1, IntPosition ip2)
		{
			return new IntPosition(ip1.x - ip2.x, ip1.z - ip2.z);
		}

		public static IntPosition operator *(IntPosition ip, int multiplier)
		{
			return new IntPosition(ip.x * multiplier, ip.z * multiplier);
		}

		public static IntPosition operator *(IntPosition ip, uint multiplier)
		{
			return ip * (int)multiplier;
		}
	}
}
