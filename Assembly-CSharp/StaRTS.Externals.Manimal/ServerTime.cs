using StaRTS.Utils.Core;
using System;

namespace StaRTS.Externals.Manimal
{
	public class ServerTime
	{
		public static uint Time
		{
			get
			{
				return Service.ServerAPI.ServerTime;
			}
		}
	}
}
