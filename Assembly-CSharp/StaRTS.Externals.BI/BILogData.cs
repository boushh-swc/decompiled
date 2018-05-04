using System;
using System.Collections.Generic;

namespace StaRTS.Externals.BI
{
	public class BILogData
	{
		public string url;

		public byte[] postData;

		public Dictionary<string, string> headers;

		public BILogData()
		{
			this.url = string.Empty;
			this.postData = null;
			this.headers = null;
		}
	}
}
