using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class CivilianComponent : WalkerComponent
	{
		public CivilianTypeVO CivilianType
		{
			get;
			set;
		}

		public CivilianComponent(CivilianTypeVO civilianType) : base(civilianType.AssetName, civilianType)
		{
			this.CivilianType = civilianType;
		}
	}
}
