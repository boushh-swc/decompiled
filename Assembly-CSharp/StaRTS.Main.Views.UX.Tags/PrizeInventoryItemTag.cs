using StaRTS.Main.Models;
using StaRTS.Main.Views.UX.Elements;
using System;

namespace StaRTS.Main.Views.UX.Tags
{
	public class PrizeInventoryItemTag
	{
		public string PrizeID
		{
			get;
			set;
		}

		public PrizeType PrizeType
		{
			get;
			set;
		}

		public UXElement TileElement
		{
			get;
			set;
		}

		public UXElement MainElement
		{
			get;
			set;
		}

		public UXLabel InfoLabel
		{
			get;
			set;
		}

		public UXLabel CountLabel
		{
			get;
			set;
		}

		public string IconAssetName
		{
			get;
			set;
		}
	}
}
