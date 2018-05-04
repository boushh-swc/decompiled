using StaRTS.Utils.Core;
using System;

namespace StaRTS.Assets
{
	public class MultiAssetInfo : AssetRequest
	{
		public RefCount RefCount
		{
			get;
			private set;
		}

		public AssetsCompleteDelegate OnComplete
		{
			get;
			private set;
		}

		public object CompleteCookie
		{
			get;
			private set;
		}

		public MultiAssetInfo(string assetName, AssetSuccessDelegate onSuccess, AssetFailureDelegate onFailure, object cookie, RefCount refCount, AssetsCompleteDelegate onComplete, object completeCookie) : base(AssetHandle.Invalid, assetName, onSuccess, onFailure, cookie)
		{
			this.RefCount = refCount;
			this.OnComplete = onComplete;
			this.CompleteCookie = completeCookie;
		}
	}
}
