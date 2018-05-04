using System;
using System.Reflection;

namespace StaRTS.Main.Configs
{
	public class Secrets
	{
		public const string CLIENT_GAE_AUTH_HEADER = "54690BA3-45EF-4CEF-9A75-F30314596815";

		public static string GetSecretValue(string name)
		{
			name = name.ToUpper();
			FieldInfo field = typeof(Secrets).GetField(name);
			return (field != null) ? ((string)field.GetValue(null)) : string.Empty;
		}
	}
}
