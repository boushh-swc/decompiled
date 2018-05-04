using StaRTS.Main.Utils.Events;
using System;

namespace StaRTS.Main.Models
{
	public class HoloCommand
	{
		public EventId EventId
		{
			get;
			set;
		}

		public object Cookie
		{
			get;
			set;
		}
	}
}
