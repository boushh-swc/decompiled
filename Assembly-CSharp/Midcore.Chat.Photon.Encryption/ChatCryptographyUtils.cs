using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Midcore.Chat.Photon.Encryption
{
	public static class ChatCryptographyUtils
	{
		private const string MESSAGE = "msg";

		private const string IV = "iv";

		private static RijndaelManaged symmetricKey;

		public static void StaticInit()
		{
			if (ChatCryptographyUtils.symmetricKey != null)
			{
				ChatCryptographyUtils.symmetricKey = null;
			}
			ChatCryptographyUtils.symmetricKey = new RijndaelManaged();
		}

		public static string GetSHA256Hash(string plainText)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(plainText);
			SHA256 sHA = SHA256.Create();
			return Convert.ToBase64String(sHA.ComputeHash(bytes));
		}

		public static string GetEncryptedMessageWithIV(string plainText, string key)
		{
			if (string.IsNullOrEmpty(plainText))
			{
				Service.Logger.Error("Failed to encrypt message, Input string is empty");
				return string.Empty;
			}
			if (string.IsNullOrEmpty(key))
			{
				Service.Logger.Error("Failed to encrypt message, key is empty");
				return string.Empty;
			}
			if (!key.IsBase64String())
			{
				Service.Logger.Error("Failed to encrypt message, key is not base64. key:" + key);
				return string.Empty;
			}
			string result;
			try
			{
				byte[] bytes = Encoding.UTF8.GetBytes(plainText);
				byte[] rgbKey = Convert.FromBase64String(key);
				ChatCryptographyUtils.symmetricKey.GenerateIV();
				byte[] iV = ChatCryptographyUtils.symmetricKey.IV;
				ChatCryptographyUtils.symmetricKey.Mode = CipherMode.CBC;
				ICryptoTransform transform = ChatCryptographyUtils.symmetricKey.CreateEncryptor(rgbKey, iV);
				MemoryStream memoryStream = new MemoryStream();
				CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
				cryptoStream.Write(bytes, 0, bytes.Length);
				cryptoStream.FlushFinalBlock();
				byte[] inArray = memoryStream.ToArray();
				string val = Convert.ToBase64String(inArray);
				string val2 = Convert.ToBase64String(iV);
				Serializer serializer = Serializer.Start();
				serializer.AddString("msg", val);
				serializer.AddString("iv", val2);
				string text = serializer.End().ToString();
				result = text;
			}
			catch (Exception ex)
			{
				Service.Logger.ErrorFormat("Failed to encrypt message: {0}, error: {1}", new object[]
				{
					plainText,
					ex.Message
				});
				result = string.Empty;
			}
			return result;
		}

		public static string DecryptMessageWithIV(string messageAndIV, string key)
		{
			if (string.IsNullOrEmpty(messageAndIV))
			{
				Service.Logger.Error("Failed to decrypt message, messageAndIV is empty");
				return string.Empty;
			}
			if (string.IsNullOrEmpty(key))
			{
				Service.Logger.Error("Failed to encrypt message, key is empty");
				return string.Empty;
			}
			string text = string.Empty;
			string text2 = string.Empty;
			object obj = new JsonParser(messageAndIV).Parse();
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				if (dictionary.ContainsKey("msg"))
				{
					text = Convert.ToString(dictionary["msg"]);
				}
				if (dictionary.ContainsKey("iv"))
				{
					text2 = Convert.ToString(dictionary["iv"]);
				}
			}
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2))
			{
				Service.Logger.ErrorFormat("Failed to decrypt message: {0}", new object[]
				{
					messageAndIV
				});
				return string.Empty;
			}
			if (!text.IsBase64String() || !key.IsBase64String() || !text2.IsBase64String())
			{
				Service.Logger.Error("Photon Chat: Convert failed for non Base64 string.  messageAndIV:" + messageAndIV);
				return string.Empty;
			}
			string result;
			try
			{
				byte[] array = Convert.FromBase64String(text.Replace(' ', '+'));
				byte[] array2 = new byte[array.Length];
				byte[] rgbKey = Convert.FromBase64String(key);
				byte[] rgbIV = Convert.FromBase64String(text2);
				ChatCryptographyUtils.symmetricKey.Mode = CipherMode.CBC;
				ICryptoTransform transform = ChatCryptographyUtils.symmetricKey.CreateDecryptor(rgbKey, rgbIV);
				MemoryStream stream = new MemoryStream(array);
				CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read);
				int count = cryptoStream.Read(array2, 0, array2.Length);
				string text3 = Encoding.UTF8.GetString(array2, 0, count).TrimEnd(new char[1]);
				result = text3;
			}
			catch (Exception ex)
			{
				Service.Logger.WarnFormat("Failed to decrypt messageAndIV: {0}, error: {1}", new object[]
				{
					messageAndIV,
					ex.Message
				});
				result = string.Empty;
			}
			return result;
		}
	}
}
