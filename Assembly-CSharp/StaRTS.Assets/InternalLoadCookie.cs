using System;

namespace StaRTS.Assets
{
	public class InternalLoadCookie
	{
		public string AssetName
		{
			get;
			private set;
		}

		public InternalLoadCookie(string assetName)
		{
			this.AssetName = assetName;
		}
	}
}
