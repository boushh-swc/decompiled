using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Collect
{
	public class BuildingCollectCommand : GameActionCommand<BuildingCollectRequest, DefaultResponse>
	{
		public const string ACTION = "player.building.collect";

		public BuildingCollectCommand(BuildingCollectRequest request) : base("player.building.collect", request, new DefaultResponse())
		{
		}
	}
}
