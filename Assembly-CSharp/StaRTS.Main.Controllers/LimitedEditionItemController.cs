using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers
{
	public class LimitedEditionItemController : LimitedEditionItemControllerBase
	{
		public LimitedEditionItemController()
		{
			Service.LimitedEditionItemController = this;
		}

		protected override void UpdateValidItems()
		{
			base.UpdateValidItems<LimitedEditionItemVO>();
		}

		protected override bool IsValidForPlayer(ILimitedEditionItemVO vo, CurrentPlayer player)
		{
			return ((LimitedEditionItemVO)vo).Faction == player.Faction && base.IsValidForPlayer(vo, player);
		}
	}
}
