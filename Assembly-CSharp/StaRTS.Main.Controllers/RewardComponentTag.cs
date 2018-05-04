using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Controllers
{
	public struct RewardComponentTag
	{
		public RewardType Type
		{
			get;
			set;
		}

		public string RewardAssetName
		{
			get;
			set;
		}

		public IGeometryVO RewardGeometryConfig
		{
			get;
			set;
		}

		public string RewardName
		{
			get;
			set;
		}

		public string Quantity
		{
			get;
			set;
		}

		public int Order
		{
			get;
			set;
		}
	}
}
