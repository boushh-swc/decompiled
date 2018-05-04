using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models
{
	public class TurretTrapEventData : ITrapEventData
	{
		public string TurretUid
		{
			get;
			private set;
		}

		public string TurretAnimatorName
		{
			get;
			private set;
		}

		public ITrapEventData Init(string rawData)
		{
			if (string.IsNullOrEmpty(rawData))
			{
				Service.Logger.Error("All Turret Traps must list the uid of the turret and the animator name");
				return null;
			}
			if (!rawData.Contains(","))
			{
				Service.Logger.ErrorFormat("Turret Trap Data must have 2 comma-delimited values for turret type and turret animator game object.  ({0})", new object[]
				{
					rawData
				});
				return null;
			}
			string[] array = rawData.TrimEnd(new char[]
			{
				' '
			}).Split(new char[]
			{
				','
			});
			if (array.Length > 2)
			{
				Service.Logger.ErrorFormat("Turret Trap Data must have exactly 2 values, one for turret type and one for turret animator game object.  ({0})", new object[]
				{
					rawData
				});
				return null;
			}
			this.TurretUid = array[0];
			this.TurretAnimatorName = array[1];
			return this;
		}
	}
}
