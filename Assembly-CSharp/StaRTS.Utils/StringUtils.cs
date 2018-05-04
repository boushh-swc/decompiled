using StaRTS.Utils.Core;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace StaRTS.Utils
{
	public static class StringUtils
	{
		private const string NUMERIC_CHARACTERS = "0123456789";

		private const string guidRegxPattern = "^(\\{){0,1}[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}(\\}){0,1}$";

		private static Regex guidRegx;

		private const string BASE64_REGEX = "^[a-zA-Z0-9\\+/]*={0,3}$";

		public static T ParseEnum<T>(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return default(T);
			}
			string text = StringUtils.ToPascalCase(name);
			T result;
			try
			{
				result = (T)((object)Enum.Parse(typeof(T), text));
			}
			catch
			{
				Service.Logger.Error(string.Format("Enum value '{0}' not found in {1}. Original Value: {2}", text, typeof(T), name));
				result = default(T);
			}
			return result;
		}

		public static string ToLowerCaseUnderscoreSeperated(string s)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int i = 0;
			int length = s.Length;
			while (i < length)
			{
				char c = s[i];
				bool flag = char.IsUpper(c);
				bool flag2 = false;
				if (i < s.Length - 1)
				{
					flag2 = char.IsUpper(s[i + 1]);
				}
				if (flag2 && !flag)
				{
					stringBuilder.Append(char.ToLower(c));
					stringBuilder.Append('_');
				}
				else if ((flag && flag2) || (flag && i == s.Length - 1))
				{
					stringBuilder.Append(c);
				}
				else
				{
					stringBuilder.Append(char.ToLower(c));
				}
				i++;
			}
			return stringBuilder.ToString();
		}

		public static string ToPascalCase(string s)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			int i = 0;
			int length = s.Length;
			while (i < length)
			{
				char c = s[i];
				bool flag2 = char.IsLetter(c);
				if (flag2)
				{
					if (!flag)
					{
						stringBuilder.Append(char.ToUpper(c));
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
				else if (char.IsNumber(c))
				{
					stringBuilder.Append(c);
				}
				flag = flag2;
				i++;
			}
			return stringBuilder.ToString();
		}

		public static string GetRomanNumeral(int number)
		{
			if (number <= 0)
			{
				return string.Empty;
			}
			if (number > 199)
			{
				return number.ToString();
			}
			int num = number / 100;
			number %= 100;
			string text = string.Empty;
			switch (number % 10)
			{
			case 1:
				text = "I";
				break;
			case 2:
				text = "II";
				break;
			case 3:
				text = "III";
				break;
			case 4:
				text = "IV";
				break;
			case 5:
				text = "V";
				break;
			case 6:
				text = "VI";
				break;
			case 7:
				text = "VII";
				break;
			case 8:
				text = "VIII";
				break;
			case 9:
				text = "IX";
				break;
			}
			if (number >= 10 && number <= 39)
			{
				string str = new string('X', number / 10);
				text = str + text;
			}
			else if (number >= 40 && number <= 49)
			{
				text = "XL" + text;
			}
			else if (number >= 50 && number <= 59)
			{
				text = "L" + text;
			}
			else if (number >= 60 && number <= 69)
			{
				text = "LX" + text;
			}
			else if (number >= 70 && number <= 79)
			{
				text = "LXX" + text;
			}
			else if (number >= 80 && number <= 89)
			{
				text = "LXXX" + text;
			}
			else if (number >= 90 && number <= 99)
			{
				text = "XC" + text;
			}
			if (num == 1)
			{
				text = "C" + text;
			}
			return text;
		}

		public static int GetIndexOfFirstNumericCharacter(string s)
		{
			return s.IndexOfAny("0123456789".ToCharArray());
		}

		public static string Substring(string s, int startIndex, int endIndex)
		{
			return s.Substring(startIndex, endIndex - startIndex);
		}

		public static string Substring(string s, int startIndex)
		{
			return s.Substring(startIndex);
		}

		public static bool IsBlank(string s)
		{
			return string.IsNullOrEmpty(s) || string.IsNullOrEmpty(s.Trim());
		}

		public static bool IsNotBlank(string s)
		{
			return !StringUtils.IsBlank(s);
		}

		public static bool IsEmpty(string s)
		{
			return string.IsNullOrEmpty(s);
		}

		public static bool IsNotEmpty(string s)
		{
			return !StringUtils.IsEmpty(s);
		}

		public static bool IsNull(string s)
		{
			return s == null;
		}

		public static bool IsNotNull(string s)
		{
			return !StringUtils.IsNull(s);
		}

		public static string GenerateRandom(uint length)
		{
			string text = "abcdefghijklmnopqrstuvwxyz0123456789";
			StringBuilder stringBuilder = new StringBuilder();
			Rand rand = Service.Rand;
			int num = 0;
			while ((long)num < (long)((ulong)length))
			{
				int index = rand.ViewRangeInt(0, text.Length);
				stringBuilder.Append(text[index]);
				num++;
			}
			return stringBuilder.ToString();
		}

		public static bool IsGuidValid(string guid)
		{
			if (StringUtils.guidRegx == null)
			{
				StringUtils.guidRegx = new Regex("^(\\{){0,1}[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}(\\}){0,1}$");
			}
			return StringUtils.guidRegx.IsMatch(guid);
		}

		public static bool IsBase64String(this string s)
		{
			s = s.Trim();
			return s.Length % 4 == 0 && Regex.IsMatch(s, "^[a-zA-Z0-9\\+/]*={0,3}$", RegexOptions.None);
		}
	}
}
