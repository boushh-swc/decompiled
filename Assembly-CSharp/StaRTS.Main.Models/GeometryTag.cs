using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.Projectors;
using System;

namespace StaRTS.Main.Models
{
	public sealed class GeometryTag
	{
		public IGeometryVO geometry;

		public ProjectorConfig projector;

		public string assetName;

		public string tooltipText;

		public BattleEntry battle;

		public ActiveArmory armory;

		public GeometryTag(IGeometryVO geometry, ProjectorConfig projector, BattleEntry battle)
		{
			this.Init(geometry, projector, null, null, battle, null);
		}

		public GeometryTag(IGeometryVO geometry, ProjectorConfig projector, ActiveArmory armory)
		{
			this.Init(geometry, projector, null, null, null, armory);
		}

		public GeometryTag(IGeometryVO geometry, string tooltipText, BattleEntry battle)
		{
			this.Init(geometry, null, null, tooltipText, battle, null);
		}

		public GeometryTag(IGeometryVO geometry, string assetName)
		{
			this.Init(geometry, null, assetName, null, null, null);
		}

		private void Init(IGeometryVO geometry, ProjectorConfig projector, string assetName, string tooltipText, BattleEntry battle, ActiveArmory armory)
		{
			this.geometry = geometry;
			this.projector = projector;
			this.assetName = assetName;
			this.tooltipText = tooltipText;
			this.battle = battle;
			this.armory = armory;
		}
	}
}
