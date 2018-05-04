using System;
using System.Text;

public sealed class SwrveMD5Core
{
	private SwrveMD5Core()
	{
	}

	public static byte[] GetHash(string input, Encoding encoding)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
		}
		if (encoding == null)
		{
			throw new ArgumentNullException("encoding", "Unable to calculate hash over a string without a default encoding. Consider using the GetHash(string) overload to use UTF8 Encoding");
		}
		byte[] bytes = encoding.GetBytes(input);
		return SwrveMD5Core.GetHash(bytes);
	}

	public static byte[] GetHash(string input)
	{
		return SwrveMD5Core.GetHash(input, new UTF8Encoding());
	}

	public static string GetHashString(byte[] input)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
		}
		string text = BitConverter.ToString(SwrveMD5Core.GetHash(input));
		return text.Replace("-", string.Empty);
	}

	public static string GetHashString(string input, Encoding encoding)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
		}
		if (encoding == null)
		{
			throw new ArgumentNullException("encoding", "Unable to calculate hash over a string without a default encoding. Consider using the GetHashString(string) overload to use UTF8 Encoding");
		}
		byte[] bytes = encoding.GetBytes(input);
		return SwrveMD5Core.GetHashString(bytes);
	}

	public static string GetHashString(string input)
	{
		return SwrveMD5Core.GetHashString(input, new UTF8Encoding());
	}

	public static byte[] GetHash(byte[] input)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input", "Unable to calculate hash over null input data");
		}
		SwrveABCDStruct aBCD = default(SwrveABCDStruct);
		aBCD.A = 1732584193u;
		aBCD.B = 4023233417u;
		aBCD.C = 2562383102u;
		aBCD.D = 271733878u;
		int i;
		for (i = 0; i <= input.Length - 64; i += 64)
		{
			SwrveMD5Core.GetHashBlock(input, ref aBCD, i);
		}
		return SwrveMD5Core.GetHashFinalBlock(input, i, input.Length - i, aBCD, (long)input.Length * 8L);
	}

	internal static byte[] GetHashFinalBlock(byte[] input, int ibStart, int cbSize, SwrveABCDStruct ABCD, long len)
	{
		byte[] array = new byte[64];
		byte[] bytes = BitConverter.GetBytes(len);
		Array.Copy(input, ibStart, array, 0, cbSize);
		array[cbSize] = 128;
		if (cbSize < 56)
		{
			Array.Copy(bytes, 0, array, 56, 8);
			SwrveMD5Core.GetHashBlock(array, ref ABCD, 0);
		}
		else
		{
			SwrveMD5Core.GetHashBlock(array, ref ABCD, 0);
			array = new byte[64];
			Array.Copy(bytes, 0, array, 56, 8);
			SwrveMD5Core.GetHashBlock(array, ref ABCD, 0);
		}
		byte[] array2 = new byte[16];
		Array.Copy(BitConverter.GetBytes(ABCD.A), 0, array2, 0, 4);
		Array.Copy(BitConverter.GetBytes(ABCD.B), 0, array2, 4, 4);
		Array.Copy(BitConverter.GetBytes(ABCD.C), 0, array2, 8, 4);
		Array.Copy(BitConverter.GetBytes(ABCD.D), 0, array2, 12, 4);
		return array2;
	}

	internal static void GetHashBlock(byte[] input, ref SwrveABCDStruct ABCDValue, int ibStart)
	{
		uint[] array = SwrveMD5Core.Converter(input, ibStart);
		uint num = ABCDValue.A;
		uint num2 = ABCDValue.B;
		uint num3 = ABCDValue.C;
		uint num4 = ABCDValue.D;
		num = SwrveMD5Core.r1(num, num2, num3, num4, array[0], 7, 3614090360u);
		num4 = SwrveMD5Core.r1(num4, num, num2, num3, array[1], 12, 3905402710u);
		num3 = SwrveMD5Core.r1(num3, num4, num, num2, array[2], 17, 606105819u);
		num2 = SwrveMD5Core.r1(num2, num3, num4, num, array[3], 22, 3250441966u);
		num = SwrveMD5Core.r1(num, num2, num3, num4, array[4], 7, 4118548399u);
		num4 = SwrveMD5Core.r1(num4, num, num2, num3, array[5], 12, 1200080426u);
		num3 = SwrveMD5Core.r1(num3, num4, num, num2, array[6], 17, 2821735955u);
		num2 = SwrveMD5Core.r1(num2, num3, num4, num, array[7], 22, 4249261313u);
		num = SwrveMD5Core.r1(num, num2, num3, num4, array[8], 7, 1770035416u);
		num4 = SwrveMD5Core.r1(num4, num, num2, num3, array[9], 12, 2336552879u);
		num3 = SwrveMD5Core.r1(num3, num4, num, num2, array[10], 17, 4294925233u);
		num2 = SwrveMD5Core.r1(num2, num3, num4, num, array[11], 22, 2304563134u);
		num = SwrveMD5Core.r1(num, num2, num3, num4, array[12], 7, 1804603682u);
		num4 = SwrveMD5Core.r1(num4, num, num2, num3, array[13], 12, 4254626195u);
		num3 = SwrveMD5Core.r1(num3, num4, num, num2, array[14], 17, 2792965006u);
		num2 = SwrveMD5Core.r1(num2, num3, num4, num, array[15], 22, 1236535329u);
		num = SwrveMD5Core.r2(num, num2, num3, num4, array[1], 5, 4129170786u);
		num4 = SwrveMD5Core.r2(num4, num, num2, num3, array[6], 9, 3225465664u);
		num3 = SwrveMD5Core.r2(num3, num4, num, num2, array[11], 14, 643717713u);
		num2 = SwrveMD5Core.r2(num2, num3, num4, num, array[0], 20, 3921069994u);
		num = SwrveMD5Core.r2(num, num2, num3, num4, array[5], 5, 3593408605u);
		num4 = SwrveMD5Core.r2(num4, num, num2, num3, array[10], 9, 38016083u);
		num3 = SwrveMD5Core.r2(num3, num4, num, num2, array[15], 14, 3634488961u);
		num2 = SwrveMD5Core.r2(num2, num3, num4, num, array[4], 20, 3889429448u);
		num = SwrveMD5Core.r2(num, num2, num3, num4, array[9], 5, 568446438u);
		num4 = SwrveMD5Core.r2(num4, num, num2, num3, array[14], 9, 3275163606u);
		num3 = SwrveMD5Core.r2(num3, num4, num, num2, array[3], 14, 4107603335u);
		num2 = SwrveMD5Core.r2(num2, num3, num4, num, array[8], 20, 1163531501u);
		num = SwrveMD5Core.r2(num, num2, num3, num4, array[13], 5, 2850285829u);
		num4 = SwrveMD5Core.r2(num4, num, num2, num3, array[2], 9, 4243563512u);
		num3 = SwrveMD5Core.r2(num3, num4, num, num2, array[7], 14, 1735328473u);
		num2 = SwrveMD5Core.r2(num2, num3, num4, num, array[12], 20, 2368359562u);
		num = SwrveMD5Core.r3(num, num2, num3, num4, array[5], 4, 4294588738u);
		num4 = SwrveMD5Core.r3(num4, num, num2, num3, array[8], 11, 2272392833u);
		num3 = SwrveMD5Core.r3(num3, num4, num, num2, array[11], 16, 1839030562u);
		num2 = SwrveMD5Core.r3(num2, num3, num4, num, array[14], 23, 4259657740u);
		num = SwrveMD5Core.r3(num, num2, num3, num4, array[1], 4, 2763975236u);
		num4 = SwrveMD5Core.r3(num4, num, num2, num3, array[4], 11, 1272893353u);
		num3 = SwrveMD5Core.r3(num3, num4, num, num2, array[7], 16, 4139469664u);
		num2 = SwrveMD5Core.r3(num2, num3, num4, num, array[10], 23, 3200236656u);
		num = SwrveMD5Core.r3(num, num2, num3, num4, array[13], 4, 681279174u);
		num4 = SwrveMD5Core.r3(num4, num, num2, num3, array[0], 11, 3936430074u);
		num3 = SwrveMD5Core.r3(num3, num4, num, num2, array[3], 16, 3572445317u);
		num2 = SwrveMD5Core.r3(num2, num3, num4, num, array[6], 23, 76029189u);
		num = SwrveMD5Core.r3(num, num2, num3, num4, array[9], 4, 3654602809u);
		num4 = SwrveMD5Core.r3(num4, num, num2, num3, array[12], 11, 3873151461u);
		num3 = SwrveMD5Core.r3(num3, num4, num, num2, array[15], 16, 530742520u);
		num2 = SwrveMD5Core.r3(num2, num3, num4, num, array[2], 23, 3299628645u);
		num = SwrveMD5Core.r4(num, num2, num3, num4, array[0], 6, 4096336452u);
		num4 = SwrveMD5Core.r4(num4, num, num2, num3, array[7], 10, 1126891415u);
		num3 = SwrveMD5Core.r4(num3, num4, num, num2, array[14], 15, 2878612391u);
		num2 = SwrveMD5Core.r4(num2, num3, num4, num, array[5], 21, 4237533241u);
		num = SwrveMD5Core.r4(num, num2, num3, num4, array[12], 6, 1700485571u);
		num4 = SwrveMD5Core.r4(num4, num, num2, num3, array[3], 10, 2399980690u);
		num3 = SwrveMD5Core.r4(num3, num4, num, num2, array[10], 15, 4293915773u);
		num2 = SwrveMD5Core.r4(num2, num3, num4, num, array[1], 21, 2240044497u);
		num = SwrveMD5Core.r4(num, num2, num3, num4, array[8], 6, 1873313359u);
		num4 = SwrveMD5Core.r4(num4, num, num2, num3, array[15], 10, 4264355552u);
		num3 = SwrveMD5Core.r4(num3, num4, num, num2, array[6], 15, 2734768916u);
		num2 = SwrveMD5Core.r4(num2, num3, num4, num, array[13], 21, 1309151649u);
		num = SwrveMD5Core.r4(num, num2, num3, num4, array[4], 6, 4149444226u);
		num4 = SwrveMD5Core.r4(num4, num, num2, num3, array[11], 10, 3174756917u);
		num3 = SwrveMD5Core.r4(num3, num4, num, num2, array[2], 15, 718787259u);
		num2 = SwrveMD5Core.r4(num2, num3, num4, num, array[9], 21, 3951481745u);
		ABCDValue.A = num + ABCDValue.A;
		ABCDValue.B = num2 + ABCDValue.B;
		ABCDValue.C = num3 + ABCDValue.C;
		ABCDValue.D = num4 + ABCDValue.D;
	}

	private static uint r1(uint a, uint b, uint c, uint d, uint x, int s, uint t)
	{
		return b + SwrveMD5Core.LSR(a + ((b & c) | ((b ^ 4294967295u) & d)) + x + t, s);
	}

	private static uint r2(uint a, uint b, uint c, uint d, uint x, int s, uint t)
	{
		return b + SwrveMD5Core.LSR(a + ((b & d) | (c & (d ^ 4294967295u))) + x + t, s);
	}

	private static uint r3(uint a, uint b, uint c, uint d, uint x, int s, uint t)
	{
		return b + SwrveMD5Core.LSR(a + (b ^ c ^ d) + x + t, s);
	}

	private static uint r4(uint a, uint b, uint c, uint d, uint x, int s, uint t)
	{
		return b + SwrveMD5Core.LSR(a + (c ^ (b | (d ^ 4294967295u))) + x + t, s);
	}

	private static uint LSR(uint i, int s)
	{
		return i << s | i >> 32 - s;
	}

	private static uint[] Converter(byte[] input, int ibStart)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input", "Unable convert null array to array of uInts");
		}
		uint[] array = new uint[16];
		for (int i = 0; i < 16; i++)
		{
			array[i] = (uint)input[ibStart + i * 4];
			array[i] += (uint)((uint)input[ibStart + i * 4 + 1] << 8);
			array[i] += (uint)((uint)input[ibStart + i * 4 + 2] << 16);
			array[i] += (uint)((uint)input[ibStart + i * 4 + 3] << 24);
		}
		return array;
	}
}
