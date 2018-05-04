using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace DCPI.Platforms.SwrveManager.Utils
{
	public static class SwrveManagerUtils
	{
		private static readonly string ENCRYPTION_ALGORITHM;

		private static readonly string ANDI_TYPE;

		private const int KEY_SIZE = 256;

		private const int ITERATIONS = 32768;

		private const string CIPHER_ALOGRITHM = "AES/CBC/PKCS5Padding";

		private const string KEY_ALOGRITHM = "PBKDF2WithHmacSHA1";

		private const string KEY_SPEC_ALOGRITHM = "AES";

		private const string ENCODING = "UTF-8";

		private static KeyParameter secretKeyParameter;

		private static string saltString;

		private static Type andiType;

		public static Type ANDIType
		{
			get
			{
				return SwrveManagerUtils.andiType;
			}
		}

		public static KeyParameter aesKey
		{
			get
			{
				return SwrveManagerUtils.secretKeyParameter;
			}
		}

		static SwrveManagerUtils()
		{
			SwrveManagerUtils.ENCRYPTION_ALGORITHM = "AES";
			SwrveManagerUtils.ANDI_TYPE = "Disney.ANDI.ANDI";
			SwrveManagerUtils.secretKeyParameter = null;
			SwrveManagerUtils.saltString = string.Empty;
			SwrveManagerUtils.andiType = null;
		}

		public static string GetIsJailBroken()
		{
			string result = string.Empty;
			if (Application.platform == RuntimePlatform.Android)
			{
				result = SwrveManagerUtilsAndroid.GetIsJailBroken();
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				result = SwrveManagerUtilsiOS.GetIsJailBroken();
			}
			return result;
		}

		public static string GetIsLat()
		{
			string result = string.Empty;
			if (Application.platform == RuntimePlatform.Android)
			{
				result = SwrveManagerUtilsAndroid.GetIsLat().ToString();
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				result = SwrveManagerUtilsiOS.GetIsLat().ToString();
			}
			return result;
		}

		public static string GetAdvertiserID()
		{
			string result = string.Empty;
			if (Application.platform == RuntimePlatform.Android)
			{
				result = SwrveManagerUtilsAndroid.GetGIDA();
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				result = SwrveManagerUtilsiOS.GetIDFA();
			}
			return result;
		}

		public static bool RegisterGCMDevice(string gameObject, string senderId, string appTitle, string iconId, string materialIconId, string largeIconId, int accentColor, string appGroupName)
		{
			bool result = false;
			if (Application.platform == RuntimePlatform.Android)
			{
				result = SwrveManagerUtilsAndroid.RegisterGCMDevice(gameObject, senderId, appTitle, iconId, materialIconId, largeIconId, accentColor, appGroupName);
			}
			return result;
		}

		public static bool IsAndiAvailable()
		{
			if (SwrveManagerUtils.andiType != null)
			{
				return true;
			}
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			int i = 0;
			int num = assemblies.Length;
			while (i < num)
			{
				Assembly assembly = assemblies[i];
				SwrveManagerUtils.andiType = assembly.GetType(SwrveManagerUtils.ANDI_TYPE);
				if (SwrveManagerUtils.andiType != null)
				{
					return true;
				}
				i++;
			}
			return false;
		}

		public static bool IsAndiInitialized()
		{
			bool result = false;
			if (SwrveManagerUtils.andiType == null)
			{
				return result;
			}
			string value = (string)SwrveManagerUtils.andiType.GetMethod("GetAndiu").Invoke(null, null);
			if (!string.IsNullOrEmpty(value))
			{
				result = true;
			}
			return result;
		}

		public static string AESEncrypt(string saltString, string plainText)
		{
			if (SwrveManagerUtils.secretKeyParameter == null)
			{
				SwrveManagerUtils.secretKeyParameter = SwrveManagerUtils.KeyGen(saltString);
			}
			string text = string.Empty;
			IBufferedCipher cipher = CipherUtilities.GetCipher(SwrveManagerUtils.ENCRYPTION_ALGORITHM + "/CBC/PKCS5PADDING");
			byte[] array = new byte[cipher.GetBlockSize()];
			SecureRandom instance = SecureRandom.GetInstance("SHA1PRNG");
			instance.NextBytes(array);
			ParametersWithIV parametersWithIV = new ParametersWithIV(SwrveManagerUtils.secretKeyParameter, array);
			cipher.Init(true, parametersWithIV);
			int num = array.Length;
			byte[] array2 = cipher.DoFinal(Encoding.UTF8.GetBytes(plainText));
			Debug.Log("AESEncrypt:: IV as string: " + Convert.ToBase64String(parametersWithIV.GetIV()));
			Debug.Log("AESEncrypt:: encryptedByte as string: " + Convert.ToBase64String(array2));
			byte[] array3 = new byte[num + array2.Length];
			Array.Copy(parametersWithIV.GetIV(), 0, array3, 0, num);
			Array.Copy(array2, 0, array3, num, array2.Length);
			text = Convert.ToBase64String(array3, Base64FormattingOptions.None);
			Debug.Log("AESEncrypt:: encryptedString: " + text);
			return text;
		}

		public static string GetRSAEncryptedKey()
		{
			string result = string.Empty;
			if (SwrveManagerUtils.secretKeyParameter == null)
			{
				Debug.LogError("### SwrveManagerUtils::GetRSAEncryptedKey: secretKeyParameter is null! There is nothing to encrypt");
				return result;
			}
			try
			{
				string text = (Resources.Load("pub") as TextAsset).text;
				text = text.Replace("-----BEGIN PUBLIC KEY-----", string.Empty).Replace("-----END PUBLIC KEY-----", string.Empty);
				result = SwrveManagerUtils.RSAEncrypt(text, SwrveManagerUtils.secretKeyParameter);
			}
			catch (Exception ex)
			{
				Debug.LogError("### SwrveManagerUtils::GetRSAEncryptedKey: " + ex.Message);
			}
			return result;
		}

		public static string RSAEncrypt(string pemStreamText, KeyParameter secretKeyParameter)
		{
			string result = string.Empty;
			try
			{
				StreamReader reader = new StreamReader(new MemoryStream(Convert.FromBase64String(pemStreamText)));
				Org.BouncyCastle.OpenSsl.PemReader pemReader = new Org.BouncyCastle.OpenSsl.PemReader(reader);
				PemObject pemObject = pemReader.ReadPemObject();
				RsaKeyParameters parameters;
				if (pemObject != null)
				{
					AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(pemObject.Content);
					parameters = (RsaKeyParameters)asymmetricKeyParameter;
				}
				else
				{
					parameters = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(pemStreamText));
				}
				byte[] key = secretKeyParameter.GetKey();
				IBufferedCipher cipher = CipherUtilities.GetCipher("RSA/ECB/OAEPWithSHA_1AndMGF1Padding");
				cipher.Init(true, parameters);
				byte[] inArray = SwrveManagerUtils.BlockCipher(key, cipher, true);
				result = Convert.ToBase64String(inArray, Base64FormattingOptions.None);
			}
			catch (Exception ex)
			{
				Debug.LogError("### SwrveManagerUtils::RSAEncrypt: " + ex.Message);
			}
			return result;
		}

		public static byte[] BlockCipher(byte[] bytes, IBufferedCipher cipher, bool isEncrypt)
		{
			if ((bytes.Length <= 62 && isEncrypt) || (bytes.Length <= 128 && !isEncrypt))
			{
				return cipher.DoFinal(bytes);
			}
			byte[] suffix = new byte[0];
			byte[] prefix = new byte[0];
			int num = (!isEncrypt) ? 128 : 62;
			byte[] array = new byte[num];
			for (int i = 0; i < bytes.Length; i++)
			{
				if (i > 0 && i % num == 0)
				{
					suffix = cipher.DoFinal(array);
					prefix = SwrveManagerUtils.AppendBytes(prefix, suffix);
					int num2 = num;
					if (i + num > bytes.Length)
					{
						num2 = bytes.Length - i;
					}
					array = new byte[num2];
				}
				array[i % num] = bytes[i];
			}
			suffix = cipher.DoFinal(array);
			return SwrveManagerUtils.AppendBytes(prefix, suffix);
		}

		public static byte[] AppendBytes(byte[] prefix, byte[] suffix)
		{
			byte[] array = new byte[prefix.Length + suffix.Length];
			for (int i = 0; i < prefix.Length; i++)
			{
				array[i] = prefix[i];
			}
			for (int j = 0; j < suffix.Length; j++)
			{
				array[j + prefix.Length] = suffix[j];
			}
			return array;
		}

		private static KeyParameter KeyGen(string salt)
		{
			string text = Guid.NewGuid().ToString();
			SwrveManagerUtils.saltString = salt;
			byte[] bytes = Encoding.UTF8.GetBytes(SwrveManagerUtils.saltString);
			char[] password = text.ToCharArray();
			byte[] password2 = PbeParametersGenerator.Pkcs12PasswordToBytes(password);
			IDigest digest = new Sha1Digest();
			PbeParametersGenerator pbeParametersGenerator = new Pkcs12ParametersGenerator(digest);
			pbeParametersGenerator.Init(password2, bytes, 32768);
			return (KeyParameter)pbeParametersGenerator.GenerateDerivedParameters("AES", 256);
		}
	}
}
