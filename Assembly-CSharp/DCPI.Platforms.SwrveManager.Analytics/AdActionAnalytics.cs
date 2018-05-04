using System;
using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class AdActionAnalytics : GameAnalytics
	{
		private string _creative;

		private string _placement;

		private string _type;

		private string _referralStoreVersion;

		private IDictionary<string, object> _custom;

		public string Creative
		{
			get
			{
				return this._creative;
			}
		}

		public string Placement
		{
			get
			{
				return this._placement;
			}
		}

		public string Type
		{
			get
			{
				return this._type;
			}
		}

		public string ReferralStoreVersion
		{
			get
			{
				return this._referralStoreVersion;
			}
		}

		public IDictionary<string, object> Custom
		{
			get
			{
				return this._custom;
			}
		}

		public AdActionAnalytics(string creative, string placement, string type)
		{
			this.InitAdActionAnalytics(creative, placement, type, null, null);
		}

		public AdActionAnalytics(string creative, string placement, string type, string referralStoreVersion)
		{
			this.InitAdActionAnalytics(creative, placement, type, referralStoreVersion, null);
		}

		public AdActionAnalytics(string creative, string placement, string type, string referralStoreVersion, IDictionary<string, object> custom)
		{
			this.InitAdActionAnalytics(creative, placement, type, referralStoreVersion, custom);
		}

		private void InitAdActionAnalytics(string creative, string placement, string type, string referralStoreVersion, IDictionary<string, object> custom)
		{
			this._creative = creative;
			this._placement = placement;
			this._type = type;
			this._referralStoreVersion = referralStoreVersion;
			if (custom != null)
			{
				this._custom = new Dictionary<string, object>(custom);
			}
		}

		public override string GetSwrveEvent()
		{
			return "ad_action";
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("creative", this._creative);
			dictionary.Add("placement", this._placement);
			dictionary.Add("type", this._type);
			if (!string.IsNullOrEmpty(this._referralStoreVersion))
			{
				dictionary.Add("referralstore_version", this._referralStoreVersion);
			}
			if (this._custom != null)
			{
				foreach (KeyValuePair<string, object> current in this._custom)
				{
					dictionary.Add(current.Key, current.Value);
				}
			}
			return dictionary;
		}
	}
}
