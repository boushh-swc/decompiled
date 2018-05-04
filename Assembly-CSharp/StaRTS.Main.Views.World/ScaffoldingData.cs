using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.World
{
	public class ScaffoldingData
	{
		public Entity Building
		{
			get;
			private set;
		}

		public int Offset
		{
			get;
			private set;
		}

		public bool Flip
		{
			get;
			private set;
		}

		public AssetHandle Handle
		{
			get;
			set;
		}

		public GameObject GameObj
		{
			get;
			set;
		}

		public ScaffoldingData(Entity building, int offset, bool flip)
		{
			this.Handle = AssetHandle.Invalid;
			this.Building = building;
			this.Offset = offset;
			this.Flip = flip;
		}
	}
}
