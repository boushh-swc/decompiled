using StaRTS.Main.Models.Entities;
using System;

namespace StaRTS.Main.Views.Entities
{
	public class AddOnViewParams
	{
		public SmartEntity Entity
		{
			get;
			private set;
		}

		public string ParentName
		{
			get;
			private set;
		}

		public AddOnViewParams(SmartEntity entity, string parentName)
		{
			this.Entity = entity;
			this.ParentName = parentName;
		}
	}
}
