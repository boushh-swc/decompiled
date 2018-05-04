using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Entities.Components
{
	public class SecondaryTargetsComponent : ComponentBase
	{
		public LinkedList<Entity> WallTargets;

		public LinkedList<Entity> AlternateTargets;

		public Entity CurrentWallTarget;

		public Entity CurrentAlternateTarget;

		public Entity ObstacleTarget;

		public Point ObstacleTargetPoint;

		public SecondaryTargetsComponent()
		{
			this.AlternateTargets = null;
			this.CurrentAlternateTarget = null;
			this.ObstacleTarget = null;
			this.CurrentWallTarget = null;
		}
	}
}
