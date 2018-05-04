using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Pvp
{
	public class PvpReleaseTargetcommand : GameActionCommand<PlayerIdChecksumRequest, DefaultResponse>
	{
		public const string ACTION = "player.pvp.releaseTarget";

		public PvpReleaseTargetcommand(PlayerIdChecksumRequest request) : base("player.pvp.releaseTarget", request, new DefaultResponse())
		{
		}
	}
}
