using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Models.Static
{
	public abstract class AbstractUpgradeCatalog
	{
		protected virtual IUpgradeableVO InternalGetByLevel(string upgradeGroup, int level)
		{
			return null;
		}

		public IUpgradeableVO GetByLevel(string upgradeGroup, int level)
		{
			return this.InternalGetByLevel(upgradeGroup, level);
		}

		public IUpgradeableVO GetByLevel(IUpgradeableVO vo, int level)
		{
			return this.InternalGetByLevel(vo.UpgradeGroup, level);
		}
	}
}
