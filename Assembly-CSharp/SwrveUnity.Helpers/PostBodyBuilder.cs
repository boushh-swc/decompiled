using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SwrveUnity.Helpers
{
	public class PostBodyBuilder
	{
		private const int ApiVersion = 2;

		private static readonly string Format = Regex.Replace("\n{{\n \"user\":\"{0}\",\n \"version\":{1},\n \"app_version\":\"{2}\",\n \"session_token\":\"{3}\",\n \"device_id\":\"{4}\",\n \"data\":[{5}]\n}}", "\\s", string.Empty);

		public static byte[] Build(string apiKey, int appId, string userId, string deviceId, string appVersion, long time, string events)
		{
			string text = PostBodyBuilder.CreateSessionToken(apiKey, appId, userId, time);
			string s = string.Format(PostBodyBuilder.Format, new object[]
			{
				userId,
				2,
				appVersion,
				text,
				deviceId,
				events
			});
			return Encoding.UTF8.GetBytes(s);
		}

		private static string CreateSessionToken(string apiKey, int appId, string userId, long time)
		{
			string text = SwrveHelper.ApplyMD5(string.Format("{0}{1}{2}", userId, time, apiKey));
			return string.Format("{0}={1}={2}={3}", new object[]
			{
				appId,
				userId,
				time,
				text
			});
		}
	}
}
