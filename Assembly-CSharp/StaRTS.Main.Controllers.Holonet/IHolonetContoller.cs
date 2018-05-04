using System;

namespace StaRTS.Main.Controllers.Holonet
{
	public interface IHolonetContoller
	{
		HolonetControllerType ControllerType
		{
			get;
		}

		void PrepareContent(int lastTimeViewed);
	}
}
