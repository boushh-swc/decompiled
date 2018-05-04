using StaRTS.Main.Models.Entities;
using System;

namespace StaRTS.Main.Models
{
	public class EntityHealthChangedData
	{
		public SmartEntity Source;

		public SmartEntity Target;

		public int Damage;

		public int RawDamage;

		public bool IsFromBeam;

		public EntityHealthChangedData(SmartEntity source, SmartEntity target, int damage, int rawDamage, bool isFromBeam)
		{
			this.Source = source;
			this.Target = target;
			this.Damage = damage;
			this.RawDamage = rawDamage;
			this.IsFromBeam = isFromBeam;
		}
	}
}
