using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.Projectors;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Animations
{
	public class SupplyCrateTag
	{
		public RenderTexture RenderTexture;

		public ProjectorConfig Config;

		public GeometryProjector Projector;

		public CrateSupplyVO CrateSupply;

		public ShardQuality ShardQuailty;

		public EquipmentVO Equipment;

		public ShardVO UnlockShard;

		public void CleanUp()
		{
			this.RenderTexture = null;
			this.Config = null;
			if (this.Projector != null)
			{
				this.Projector.Destroy();
			}
			this.Projector = null;
			this.CrateSupply = null;
		}
	}
}
