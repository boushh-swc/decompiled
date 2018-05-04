using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers
{
	public class ScreenParticleFXCookie
	{
		public float Delay
		{
			get;
			set;
		}

		public uint DelayTimer
		{
			get;
			set;
		}

		public string ElementName
		{
			get;
			set;
		}

		public UXElement Element
		{
			get;
			set;
		}

		public ScreenParticleFXCookie(float delay, string elementName)
		{
			this.Delay = delay;
			this.DelayTimer = 0u;
			this.ElementName = elementName;
		}

		public void Destroy()
		{
			if (this.DelayTimer != 0u)
			{
				Service.Logger.Warn("Destroying the ScreenParticleFXCookie without killing the scheduled timer.");
			}
			this.Element = null;
		}
	}
}
