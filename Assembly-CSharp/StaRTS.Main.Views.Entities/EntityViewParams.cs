using StaRTS.Main.Models.Entities;
using System;

namespace StaRTS.Main.Views.Entities
{
	public class EntityViewParams
	{
		public SmartEntity Entity
		{
			get;
			set;
		}

		public bool CreateCollider
		{
			get;
			set;
		}

		public EntityViewParams(SmartEntity entity, bool createCollider)
		{
			this.Entity = entity;
			this.CreateCollider = createCollider;
		}
	}
}
