using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatEarnEquipmentShardCommand : GameCommand<CheatEarnEquipmentShardRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.equipment.earnShard";

		public CheatEarnEquipmentShardCommand(CheatEarnEquipmentShardRequest request) : base("cheats.equipment.earnShard", request, new DefaultResponse())
		{
		}
	}
}
