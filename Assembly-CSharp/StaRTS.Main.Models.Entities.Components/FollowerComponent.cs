using Net.RichardLord.Ash.Core;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class FollowerComponent : ComponentBase
	{
		public Entity Child
		{
			get;
			set;
		}

		public FollowerComponent(Entity child)
		{
			this.Child = child;
		}
	}
}
