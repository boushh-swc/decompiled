using System;

namespace StaRTS.Assets
{
	public class AssetProfilerFetchData
	{
		public string AssetName
		{
			get;
			private set;
		}

		public int FetchCount
		{
			get;
			set;
		}

		public float FetchTime
		{
			get;
			set;
		}

		public AssetProfilerFetchData(string name)
		{
			this.AssetName = name;
			this.FetchCount = 0;
			this.FetchTime = 0f;
		}
	}
}
