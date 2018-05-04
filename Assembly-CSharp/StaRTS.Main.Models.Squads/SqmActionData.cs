using StaRTS.Main.Controllers.Squads;
using System;

namespace StaRTS.Main.Models.Squads
{
	public class SqmActionData
	{
		public SquadAction Type;

		public SquadController.ActionCallback Callback;

		public object Cookie;
	}
}
