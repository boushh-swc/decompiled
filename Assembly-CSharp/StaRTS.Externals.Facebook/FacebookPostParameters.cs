using System;
using System.Collections.Generic;

namespace StaRTS.Externals.Facebook
{
	public class FacebookPostParameters
	{
		public Dictionary<string, string[]> Properties;

		public Uri Link
		{
			get;
			set;
		}

		public string LinkName
		{
			get;
			set;
		}

		public string LinkCaption
		{
			get;
			set;
		}

		public string LinkDescription
		{
			get;
			set;
		}

		public Uri Picture
		{
			get;
			set;
		}

		public string MediaSource
		{
			get;
			set;
		}

		public string ActionName
		{
			get;
			set;
		}

		public string ActionLink
		{
			get;
			set;
		}

		public string Reference
		{
			get;
			set;
		}

		public FacebookPostParameters()
		{
			this.Link = null;
			this.LinkName = string.Empty;
			this.LinkCaption = string.Empty;
			this.LinkDescription = string.Empty;
			this.Picture = null;
			this.MediaSource = string.Empty;
			this.ActionName = string.Empty;
			this.ActionLink = string.Empty;
			this.Reference = string.Empty;
			this.Properties = null;
		}
	}
}
