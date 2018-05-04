using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class WalkerComponent : AssetComponent
	{
		public ISpeedVO SpeedVO;

		public ISpeedVO OriginalSpeedVO
		{
			get;
			set;
		}

		public WalkerComponent(string assetName, ISpeedVO speedVO) : base(assetName)
		{
			this.OriginalSpeedVO = speedVO;
			this.SetVOData(speedVO);
		}

		public void SetVOData(ISpeedVO speedVO)
		{
			this.SpeedVO = speedVO;
		}
	}
}
