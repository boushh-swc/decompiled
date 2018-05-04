using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Controllers
{
	public class RewardTag
	{
		public RewardVO Vo
		{
			get;
			set;
		}

		public RewardManager.SuccessCallback GlobalSuccess
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
