using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using System;
using System.Runtime.InteropServices;

namespace StaRTS.Main.Controllers
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
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
