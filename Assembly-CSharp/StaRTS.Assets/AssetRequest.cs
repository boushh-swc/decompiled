using System;

namespace StaRTS.Assets
{
	public class AssetRequest
	{
		public AssetHandle Handle
		{
			get;
			private set;
		}

		public string AssetName
		{
			get;
			private set;
		}

		public AssetSuccessDelegate OnSuccess
		{
			get;
			private set;
		}

		public AssetFailureDelegate OnFailure
		{
			get;
			private set;
		}

		public object Cookie
		{
			get;
			private set;
		}

		public int DelayLoadFrameCount
		{
			get;
			set;
		}

		public AssetRequest(AssetHandle handle, string assetName, AssetSuccessDelegate onSuccess, AssetFailureDelegate onFailure, object cookie)
		{
			this.Handle = handle;
			this.AssetName = assetName;
			this.OnSuccess = onSuccess;
			this.OnFailure = onFailure;
			this.Cookie = cookie;
			this.DelayLoadFrameCount = 0;
		}
	}
}
