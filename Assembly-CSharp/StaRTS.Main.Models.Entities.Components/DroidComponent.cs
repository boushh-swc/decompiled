using Net.RichardLord.Ash.Core;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class DroidComponent : ComponentBase
	{
		public Entity Target
		{
			get;
			set;
		}

		public float Delay
		{
			get;
			set;
		}

		public bool AnimateTravel
		{
			get;
			set;
		}

		public DroidComponent()
		{
			this.Target = null;
			this.Delay = 0f;
			this.AnimateTravel = true;
		}
	}
}
