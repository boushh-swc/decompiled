using System;

namespace StaRTS.Utils
{
	public class IntMath
	{
		public const int ACCURACY = 1024;

		public const int FRAC_TRUNCATE = 3;

		public const int SQRT_2_NORM = 1448;

		private const int DIST_HI_SCALAR = 1007;

		private const int DIST_LO_SCALAR = 441;

		private const int ATANLUT_ACCURACY = 128;

		public const int ATANLUT_PI = 16384;

		private static readonly ushort[] atanLUT = new ushort[]
		{
			0,
			326,
			652,
			978,
			1303,
			1629,
			1954,
			2279,
			2604,
			2929,
			3253,
			3577,
			3900,
			4223,
			4545,
			4867,
			5188,
			5509,
			5829,
			6148,
			6467,
			6784,
			7101,
			7418,
			7733,
			8047,
			8361,
			8673,
			8985,
			9296,
			9605,
			9914,
			10221,
			10527,
			10832,
			11136,
			11439,
			11740,
			12040,
			12339,
			12637,
			12933,
			13228,
			13522,
			13814,
			14105,
			14394,
			14682,
			14968,
			15253,
			15537,
			15819,
			16100,
			16379,
			16656,
			16932,
			17206,
			17479,
			17750,
			18020,
			18288,
			18554,
			18819,
			19083,
			19344,
			19604,
			19862,
			20119,
			20374,
			20627,
			20879,
			21129,
			21378,
			21624,
			21870,
			22113,
			22355,
			22595,
			22834,
			23070,
			23306,
			23539,
			23771,
			24001,
			24230,
			24457,
			24682,
			24906,
			25128,
			25349,
			25568,
			25785,
			26001,
			26215,
			26427,
			26638,
			26848,
			27056,
			27262,
			27467,
			27670,
			27871,
			28072,
			28270,
			28467,
			28663,
			28857,
			29050,
			29241,
			29430,
			29619,
			29805,
			29991,
			30175,
			30357,
			30538,
			30718,
			30896,
			31073,
			31248,
			31423,
			31595,
			31767,
			31937,
			32106,
			32273,
			32439,
			32604,
			32768,
			32930
		};

		private static readonly short[] sinLUT = new short[]
		{
			0,
			8,
			17,
			26,
			35,
			44,
			53,
			62,
			71,
			80,
			89,
			98,
			107,
			115,
			124,
			133,
			142,
			151,
			160,
			169,
			177,
			186,
			195,
			204,
			212,
			221,
			230,
			239,
			247,
			256,
			265,
			273,
			282,
			290,
			299,
			307,
			316,
			324,
			333,
			341,
			350,
			358,
			366,
			375,
			383,
			391,
			400,
			408,
			416,
			424,
			432,
			440,
			448,
			456,
			464,
			472,
			480,
			488,
			496,
			504,
			512,
			519,
			527,
			535,
			542,
			550,
			557,
			565,
			572,
			580,
			587,
			594,
			601,
			609,
			616,
			623,
			630,
			637,
			644,
			651,
			658,
			665,
			671,
			678,
			685,
			691,
			698,
			704,
			711,
			717,
			724,
			730,
			736,
			742,
			748,
			754,
			760,
			766,
			772,
			778,
			784,
			790,
			795,
			801,
			806,
			812,
			817,
			823,
			828,
			833,
			838,
			843,
			848,
			853,
			858,
			863,
			868,
			873,
			877,
			882,
			886,
			891,
			895,
			899,
			904,
			908,
			912,
			916,
			920,
			924,
			928,
			931,
			935,
			939,
			942,
			946,
			949,
			952,
			955,
			959,
			962,
			965,
			968,
			971,
			973,
			976,
			979,
			981,
			984,
			986,
			989,
			991,
			993,
			995,
			997,
			999,
			1001,
			1003,
			1005,
			1006,
			1008,
			1009,
			1011,
			1012,
			1014,
			1015,
			1016,
			1017,
			1018,
			1019,
			1020,
			1020,
			1021,
			1022,
			1022,
			1023,
			1023,
			1023,
			1023,
			1023,
			1024
		};

		public static int FastDist(int x1, int y1, int x2, int y2)
		{
			int num = (x2 <= x1) ? (x1 - x2) : (x2 - x1);
			int num2 = (y2 <= y1) ? (y1 - y2) : (y2 - y1);
			int num3;
			int num4;
			if (num > num2)
			{
				num3 = num;
				num4 = num2;
			}
			else
			{
				num3 = num2;
				num4 = num;
			}
			int num5 = num3 * 1007 + num4 * 441;
			if (num3 < num4 * 16)
			{
				num5 -= num3 * 40;
			}
			return num5;
		}

		public static int Normalize(int oldMin, int oldMax, int val)
		{
			if (val <= oldMin)
			{
				return 0;
			}
			if (val >= oldMax)
			{
				return 1024;
			}
			if (oldMax == oldMin)
			{
				return 1024;
			}
			return (val - oldMin) * 1024 / (oldMax - oldMin);
		}

		public static int Normalize(int oldMin, int oldMax, int val, int newMin, int newMax)
		{
			if (oldMin == oldMax)
			{
				return newMax;
			}
			return val * (newMax - newMin) / (oldMax - oldMin);
		}

		public static int FloatStrToInt(string floatStr)
		{
			int num = 0;
			int length = floatStr.Length;
			int i = 0;
			if (length == 0)
			{
				return 0;
			}
			while (i < length)
			{
				if (floatStr[i] == '.')
				{
					break;
				}
				int num2 = (int)char.GetNumericValue(floatStr, i);
				if (num2 != -1)
				{
					num *= 10;
					num += num2;
				}
				i++;
			}
			num *= 1024;
			if (i != length)
			{
				int num3 = 0;
				int num4 = 1000;
				i++;
				int num5 = 0;
				while (i < length && num5 < 3)
				{
					int num6 = (int)char.GetNumericValue(floatStr, i);
					if (num6 != -1)
					{
						num3 *= 10;
						num4 /= 10;
						num3 += num6;
					}
					i++;
					num5++;
				}
				num += num3 * num4 * 1024 / 1000;
			}
			return (floatStr[0] != '-') ? num : (-num);
		}

		public static int GetPercent(int percent, int total)
		{
			return (int)((long)total * (long)percent * 1024L / 100L / 1024L);
		}

		public static int Atan2Lookup(int x, int y)
		{
			if (y == 0)
			{
				return (x < 0) ? 16384 : 0;
			}
			int num = 0;
			if (y < 0)
			{
				x = -x;
				y = -y;
				num += 4;
			}
			if (x <= 0)
			{
				int num2 = x;
				x = y;
				y = -num2;
				num += 2;
			}
			if (x <= y)
			{
				int num2 = y - x;
				x += y;
				y = num2;
				num++;
			}
			num *= 4096;
			return num + (IntMath.atanLUT[y * 128 / x] >> 3);
		}

		public static int sinLookup(int twiceAngle)
		{
			if (twiceAngle <= 180)
			{
				return (int)IntMath.sinLUT[twiceAngle];
			}
			if (twiceAngle <= 360)
			{
				return (int)IntMath.sinLUT[360 - twiceAngle];
			}
			if (twiceAngle < 540)
			{
				return (int)(-(int)IntMath.sinLUT[twiceAngle - 360]);
			}
			return (int)(-(int)IntMath.sinLUT[720 - twiceAngle]);
		}

		public static int cosLookup(int twiceAngle)
		{
			if (twiceAngle <= 180)
			{
				return (int)IntMath.sinLUT[180 - twiceAngle];
			}
			if (twiceAngle <= 360)
			{
				return (int)(-(int)IntMath.sinLUT[twiceAngle - 180]);
			}
			if (twiceAngle < 540)
			{
				return (int)(-(int)IntMath.sinLUT[540 - twiceAngle]);
			}
			return (int)IntMath.sinLUT[twiceAngle - 540];
		}
	}
}
