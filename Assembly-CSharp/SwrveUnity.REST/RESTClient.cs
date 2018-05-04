using ICSharpCode.SharpZipLib.GZip;
using SwrveUnity.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace SwrveUnity.REST
{
	public class RESTClient : IRESTClient
	{
		private const string CONTENT_ENCODING_HEADER_KEY = "CONTENT-ENCODING";

		private List<string> metrics = new List<string>();

		[DebuggerHidden]
		public virtual IEnumerator Get(string url, Action<RESTResponse> listener)
		{
			RESTClient.<Get>c__Iterator17 <Get>c__Iterator = new RESTClient.<Get>c__Iterator17();
			<Get>c__Iterator.url = url;
			<Get>c__Iterator.listener = listener;
			<Get>c__Iterator.<$>url = url;
			<Get>c__Iterator.<$>listener = listener;
			<Get>c__Iterator.<>f__this = this;
			return <Get>c__Iterator;
		}

		[DebuggerHidden]
		public virtual IEnumerator Post(string url, byte[] encodedData, Dictionary<string, string> headers, Action<RESTResponse> listener)
		{
			RESTClient.<Post>c__Iterator18 <Post>c__Iterator = new RESTClient.<Post>c__Iterator18();
			<Post>c__Iterator.headers = headers;
			<Post>c__Iterator.url = url;
			<Post>c__Iterator.encodedData = encodedData;
			<Post>c__Iterator.listener = listener;
			<Post>c__Iterator.<$>headers = headers;
			<Post>c__Iterator.<$>url = url;
			<Post>c__Iterator.<$>encodedData = encodedData;
			<Post>c__Iterator.<$>listener = listener;
			<Post>c__Iterator.<>f__this = this;
			return <Post>c__Iterator;
		}

		protected Dictionary<string, string> AddMetricsHeader(Dictionary<string, string> headers)
		{
			if (this.metrics.Count > 0)
			{
				string value = string.Join(";", this.metrics.ToArray());
				headers.Add("Swrve-Latency-Metrics", value);
				this.metrics.Clear();
			}
			return headers;
		}

		private void AddMetrics(string url, long wwwTime, bool error)
		{
			Uri uri = new Uri(url);
			url = string.Format("{0}{1}{2}", uri.Scheme, "://", uri.Authority);
			string item;
			if (error)
			{
				item = string.Format("u={0},c={1},c_error=1", url, wwwTime.ToString());
			}
			else
			{
				item = string.Format("u={0},c={1},sh={1},sb={1},rh={1},rb={1}", url, wwwTime.ToString());
			}
			this.metrics.Add(item);
		}

		protected void ProcessResponse(WWW www, long wwwTime, string url, Action<RESTResponse> listener)
		{
			try
			{
				WwwDeducedError wwwDeducedError = UnityWwwHelper.DeduceWwwError(www);
				if (wwwDeducedError == WwwDeducedError.NoError)
				{
					string text = null;
					bool flag = ResponseBodyTester.TestUTF8(www.bytes, out text);
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					string text2 = null;
					if (www.responseHeaders != null)
					{
						Dictionary<string, string>.Enumerator enumerator = www.responseHeaders.GetEnumerator();
						while (enumerator.MoveNext())
						{
							KeyValuePair<string, string> current = enumerator.Current;
							dictionary.Add(current.Key.ToUpper(), current.Value);
						}
						if (dictionary.ContainsKey("CONTENT-ENCODING"))
						{
							text2 = dictionary["CONTENT-ENCODING"];
						}
					}
					if (www.bytes != null && www.bytes.Length > 4 && text2 != null && string.Equals(text2, "gzip", StringComparison.OrdinalIgnoreCase) && text != null && (!text.StartsWith("{") || !text.EndsWith("}")) && (!text.StartsWith("[") || !text.EndsWith("]")))
					{
						int num = BitConverter.ToInt32(www.bytes, 0);
						if (num > 0)
						{
							byte[] array = new byte[num];
							using (MemoryStream memoryStream = new MemoryStream(www.bytes))
							{
								using (GZipInputStream gZipInputStream = new GZipInputStream(memoryStream))
								{
									gZipInputStream.Read(array, 0, array.Length);
									gZipInputStream.Dispose();
								}
								flag = ResponseBodyTester.TestUTF8(array, out text);
								memoryStream.Dispose();
							}
						}
					}
					if (flag)
					{
						this.AddMetrics(url, wwwTime, false);
						listener(new RESTResponse(text, dictionary));
					}
					else
					{
						this.AddMetrics(url, wwwTime, true);
						listener(new RESTResponse(WwwDeducedError.ApplicationErrorBody));
					}
				}
				else
				{
					this.AddMetrics(url, wwwTime, true);
					listener(new RESTResponse(wwwDeducedError));
				}
			}
			catch (Exception message)
			{
				SwrveLog.LogError(message);
			}
		}
	}
}
