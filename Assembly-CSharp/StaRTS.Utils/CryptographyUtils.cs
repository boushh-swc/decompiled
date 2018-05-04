using System;
using System.Security.Cryptography;
using System.Text;

namespace StaRTS.Utils
{
	public class CryptographyUtils
	{
		public static byte[] ComputeHmacHash(string algorithm, string secret, string plainText)
		{
			if (algorithm == "HmacSHA256")
			{
				HMAC hMAC = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
				return hMAC.ComputeHash(Encoding.UTF8.GetBytes(plainText));
			}
			throw new ArgumentException(string.Format("Unknown algorithm {0}", algorithm));
		}

		public static string ComputeMD5Hash(string input)
		{
			MD5 mD = MD5.Create();
			byte[] bytes = Encoding.ASCII.GetBytes(input);
			byte[] array = mD.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}
	}
}
