using Net.RichardLord.Ash.Core;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class SpawnComponent : ComponentBase
	{
		public VisitorType VisitorType
		{
			get;
			protected set;
		}

		public SpawnComponent(VisitorType visitorType)
		{
			this.VisitorType = visitorType;
		}

		public bool IsSummoned()
		{
			return this.VisitorType != VisitorType.Invalid;
		}
	}
}
