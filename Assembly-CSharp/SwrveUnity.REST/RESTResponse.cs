using SwrveUnity.Helpers;
using System;
using System.Collections.Generic;

namespace SwrveUnity.REST
{
	public class RESTResponse
	{
		public readonly string Body;

		public readonly WwwDeducedError Error;

		public readonly Dictionary<string, string> Headers;

		public RESTResponse(string body)
		{
			this.Body = body;
		}

		public RESTResponse(string body, Dictionary<string, string> headers) : this(body)
		{
			this.Headers = headers;
		}

		public RESTResponse(WwwDeducedError error)
		{
			this.Error = error;
		}
	}
}
