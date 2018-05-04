using Net.RichardLord.Ash.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Models.Entities
{
	public class EntityRef : MonoBehaviour
	{
		public Entity Entity
		{
			get;
			set;
		}

		public EntityRef(Entity entity)
		{
			this.Entity = entity;
		}
	}
}
