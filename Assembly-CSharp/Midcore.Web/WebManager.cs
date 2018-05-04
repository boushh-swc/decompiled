using StaRTS.Utils.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Midcore.Web
{
	public class WebManager
	{
		private const int MAX_CONNECTIONS = 20;

		private const string INVALID_URL = "Invalid url";

		private MonoBehaviour coroutineScript;

		private List<WWW> wwws;

		public WebManager(MonoBehaviour coroutineScript)
		{
			this.coroutineScript = coroutineScript;
			this.wwws = new List<WWW>();
		}

		public void CancelAll()
		{
			int i = 0;
			int count = this.wwws.Count;
			while (i < count)
			{
				this.wwws[i].Dispose();
				i++;
			}
			this.wwws.Clear();
		}

		public void Fetch(string url, WebAssetType assetType, int version, OnFetchDelegate onFetch, object cookie)
		{
			this.Fetch(url, null, null, assetType, version, onFetch, cookie);
		}

		public void Fetch(string url, Dictionary<string, string> requestFields, Dictionary<string, string> headers, WebAssetType assetType, int version, OnFetchDelegate onFetch, object cookie)
		{
			byte[] formData = null;
			if (requestFields != null)
			{
				WWWForm wWWForm = new WWWForm();
				foreach (KeyValuePair<string, string> current in requestFields)
				{
					wWWForm.AddField(current.Key, current.Value);
				}
				formData = wWWForm.data;
			}
			this.coroutineScript.StartCoroutine(this.FetchCoroutine(url, formData, headers, assetType, version, onFetch, cookie));
		}

		[DebuggerHidden]
		private IEnumerator FetchCoroutine(string url, byte[] formData, Dictionary<string, string> headers, WebAssetType assetType, int version, OnFetchDelegate onFetch, object cookie)
		{
			object obj = null;
			string text = null;
			if (string.IsNullOrEmpty(url))
			{
				text = "Invalid url";
			}
			else
			{
				while (this.wwws.Count >= 20)
				{
					yield return null;
				}
				WWW wWW;
				if (version != 0 && assetType == WebAssetType.Bundle)
				{
					wWW = WWW.LoadFromCacheOrDownload(url, version);
				}
				else
				{
					wWW = new WWW(url, formData, headers);
				}
				this.wwws.Add(wWW);
				yield return wWW;
				if (!this.wwws.Remove(wWW))
				{
					goto IL_222;
				}
				text = wWW.error;
				if (string.IsNullOrEmpty(text))
				{
					switch (assetType)
					{
					case WebAssetType.Bundle:
						obj = wWW.assetBundle;
						goto IL_1BB;
					case WebAssetType.StandaloneText:
						obj = wWW.text;
						goto IL_1BB;
					case WebAssetType.StandaloneBinary:
						obj = wWW.bytes;
						goto IL_1BB;
					}
					obj = null;
				}
				IL_1BB:
				wWW.Dispose();
			}
			if (object.ReferenceEquals(obj, null))
			{
				Service.Logger.WarnFormat("Failed to fetch asset {0} ({1})", new object[]
				{
					url,
					text
				});
			}
			onFetch(obj, text, cookie);
			IL_222:
			yield break;
		}
	}
}
