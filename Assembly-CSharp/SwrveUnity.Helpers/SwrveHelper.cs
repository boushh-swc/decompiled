using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SwrveUnity.Helpers
{
	public static class SwrveHelper
	{
		public static DateTime? Now = null;

		public static DateTime? UtcNow = null;

		private static MD5CryptoServiceProvider fakeReference = new MD5CryptoServiceProvider();

		private static Regex rgxNonAlphanumeric = new Regex("[^a-zA-Z0-9]");

		private static SHA1Managed sha1Managed = new SHA1Managed();

		public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static DateTime GetNow()
		{
			DateTime? now = SwrveHelper.Now;
			if (now.HasValue && SwrveHelper.Now.HasValue)
			{
				return SwrveHelper.Now.Value;
			}
			return DateTime.Now;
		}

		public static DateTime GetUtcNow()
		{
			DateTime? utcNow = SwrveHelper.UtcNow;
			if (utcNow.HasValue && SwrveHelper.UtcNow.HasValue)
			{
				return SwrveHelper.UtcNow.Value;
			}
			return DateTime.UtcNow;
		}

		public static void Shuffle<T>(this IList<T> list)
		{
			int i = list.Count;
			System.Random random = new System.Random();
			while (i > 1)
			{
				int index = random.Next(0, i) % i;
				i--;
				T value = list[index];
				list[index] = list[i];
				list[i] = value;
			}
		}

		public static byte[] MD5(string str)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			return SwrveMD5Core.GetHash(bytes);
		}

		public static string ApplyMD5(string str)
		{
			byte[] array = SwrveHelper.MD5(str);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		public static bool CheckBase64(string str)
		{
			string text = str.Trim();
			return text.Length % 4 == 0 && Regex.IsMatch(text, "^[a-zA-Z0-9\\+/]*={0,3}$", RegexOptions.None);
		}

		public static string CreateHMACMD5(string data, string key)
		{
			string result = null;
			if (SwrveHelper.fakeReference != null)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(data);
				byte[] bytes2 = Encoding.UTF8.GetBytes(key);
				using (HMACMD5 hMACMD = new HMACMD5(bytes2))
				{
					byte[] inArray = hMACMD.ComputeHash(bytes);
					result = Convert.ToBase64String(inArray);
				}
			}
			return result;
		}

		public static string sha1(byte[] bytes)
		{
			byte[] array = SwrveHelper.sha1Managed.ComputeHash(bytes);
			string text = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				byte value = array[i];
				text += Convert.ToInt32(value).ToString("x2");
			}
			return text;
		}

		public static long GetSeconds()
		{
			return (long)(DateTime.UtcNow - SwrveHelper.UnixEpoch).TotalSeconds;
		}

		public static long GetMilliseconds()
		{
			return (long)(DateTime.UtcNow - SwrveHelper.UnixEpoch).TotalMilliseconds;
		}

		public static string GetEventName(Dictionary<string, object> eventParameters)
		{
			string result = string.Empty;
			string text = (string)eventParameters["type"];
			switch (text)
			{
			case "session_start":
				result = "Swrve.session.start";
				break;
			case "session_end":
				result = "Swrve.session.end";
				break;
			case "buy_in":
				result = "Swrve.buy_in";
				break;
			case "iap":
				result = "Swrve.iap";
				break;
			case "event":
				result = (string)eventParameters["name"];
				break;
			case "purchase":
				result = "Swrve.user_purchase";
				break;
			case "currency_given":
				result = "Swrve.currency_given";
				break;
			case "user":
				result = "Swrve.user_properties_changed";
				break;
			}
			return result;
		}

		public static string EpochToFormat(long epochTime, string format)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return dateTime.AddMilliseconds((double)epochTime).ToString(format);
		}

		public static string FilterNonAlphanumeric(string str)
		{
			return SwrveHelper.rgxNonAlphanumeric.Replace(str, string.Empty);
		}

		public static bool IsNotOnDevice()
		{
			return !SwrveHelper.IsOnDevice();
		}

		public static bool IsOnDevice()
		{
			return SwrveHelper.IsAvailableOn(RuntimePlatform.Android);
		}

		public static bool IsAvailableOn(RuntimePlatform platform)
		{
			return Application.platform == platform;
		}
	}
}
