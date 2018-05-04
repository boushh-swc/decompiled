using StaRTS.Utils.Core;
using StaRTS.Utils.Diagnostics;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Midcore.Web
{
	public class OAuthHelper
	{
		public const string SCOPE_STORAGE_READ_ONLY = "https://www.googleapis.com/auth/devstorage.read_only";

		public const int EXPIRATION = 3600;

		private const string API_ENDPOINT = "https://www.googleapis.com/oauth2/v3/token";

		private const string CLAIM_SET = "{{\"iss\":\"{0}\",\"scope\":\"{1}\",\"aud\":\"{2}\",\"iat\":{3},\"exp\":{4}}}";

		private const string CLAIM_HEADER = "{\"alg\":\"RS256\",\"typ\":\"JWT\"}";

		private const string ASSERTION_TYPE = "http://oauth.net/grant_type/jwt/1.0/bearer";

		private WebManager webManager;

		private string serviceAccountEmail;

		private byte[] serviceAccountKey;

		private string scope;

		private Action<string> onComplete;

		private string currentToken;

		private bool isRequestPending;

		private Logger logger;

		public OAuthHelper(WebManager webManager, string serviceAccountEmail, byte[] serviceAccountKey, string scope)
		{
			this.webManager = webManager;
			this.serviceAccountEmail = serviceAccountEmail;
			this.serviceAccountKey = serviceAccountKey;
			this.scope = scope;
			this.logger = Service.Logger;
		}

		public void RequestToken(Action<string> onComplete, bool rebuildPrivateKey)
		{
			if (this.isRequestPending)
			{
				return;
			}
			this.isRequestPending = true;
			this.onComplete = onComplete;
			long timeInSeconds = this.GetTimeInSeconds();
			string s = string.Format("{{\"iss\":\"{0}\",\"scope\":\"{1}\",\"aud\":\"{2}\",\"iat\":{3},\"exp\":{4}}}", new object[]
			{
				this.serviceAccountEmail,
				this.scope,
				"https://www.googleapis.com/oauth2/v3/token",
				timeInSeconds,
				timeInSeconds + 3600L
			});
			string text = string.Format("{0}.{1}", Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"alg\":\"RS256\",\"typ\":\"JWT\"}")), Convert.ToBase64String(Encoding.UTF8.GetBytes(s)));
			X509Certificate2 x509Certificate = new X509Certificate2(this.serviceAccountKey, "notasecret", X509KeyStorageFlags.Exportable);
			RSACryptoServiceProvider rSACryptoServiceProvider = x509Certificate.PrivateKey as RSACryptoServiceProvider;
			if (rebuildPrivateKey)
			{
				RSACryptoServiceProvider rSACryptoServiceProvider2 = new RSACryptoServiceProvider();
				rSACryptoServiceProvider2.FromXmlString(x509Certificate.PrivateKey.ToXmlString(true));
				rSACryptoServiceProvider = rSACryptoServiceProvider2;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			byte[] inArray = rSACryptoServiceProvider.SignData(bytes, "SHA256");
			string arg = Convert.ToBase64String(inArray);
			string value = string.Format("{0}.{1}", text, arg);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary["grant_type"] = "assertion";
			dictionary["assertion_type"] = "http://oauth.net/grant_type/jwt/1.0/bearer";
			dictionary["assertion"] = value;
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			dictionary2["Content-Type"] = "application/x-www-form-urlencoded";
			this.webManager.Fetch("https://www.googleapis.com/oauth2/v3/token", dictionary, dictionary2, WebAssetType.StandaloneBinary, 0, new OnFetchDelegate(this.OnRequestComplete), null);
		}

		private void OnRequestComplete(object asset, string error, object cookie)
		{
			byte[] array = asset as byte[];
			if (array == null)
			{
				this.logger.Error("A token response was not received.");
				return;
			}
			string @string = Encoding.UTF8.GetString(array);
			string text = "\"access_token\": \"";
			int num = @string.IndexOf(text);
			if (num < 0)
			{
				this.logger.ErrorFormat("An invalid token response was received: {0}", new object[]
				{
					@string
				});
				return;
			}
			num += text.Length;
			int num2 = @string.IndexOf("\"", num);
			string obj = @string.Substring(num, num2 - num);
			if (this.onComplete != null)
			{
				this.onComplete(obj);
			}
			this.isRequestPending = false;
		}

		private long GetTimeInSeconds()
		{
			DateTime d = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return (long)(DateTime.UtcNow - d).TotalSeconds;
		}
	}
}
