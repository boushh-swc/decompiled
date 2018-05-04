using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Elements;
using System;

namespace StaRTS.Main.Views.UX.Tags
{
	public class TroopTrainingTag : TroopUpgradeTag
	{
		public bool AutoQueuing
		{
			get;
			set;
		}

		public UXLabel QueueCountLabel
		{
			get;
			set;
		}

		public UXButton TroopButton
		{
			get;
			set;
		}

		public UXButton QueueButton
		{
			get;
			set;
		}

		public UXLabel CostLabel
		{
			get;
			set;
		}

		public UXSprite Dimmer
		{
			get;
			set;
		}

		public GeometryProjector Projector
		{
			get;
			set;
		}

		public TroopTrainingTag(IDeployableVO troop, bool reqMet) : base(troop, reqMet)
		{
		}
	}
}
