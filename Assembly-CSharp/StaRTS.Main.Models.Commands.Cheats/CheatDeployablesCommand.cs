using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatDeployablesCommand : GameCommand<CheatDeployablesRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.deployable.set";

		public CheatDeployablesCommand(CheatDeployablesRequest request) : base("cheats.deployable.set", request, new DefaultResponse())
		{
		}

		public static CheatDeployablesCommand SetDeployableAmount(string troopUid, int amount)
		{
			CheatDeployablesRequest request = new CheatDeployablesRequest(new Dictionary<string, int>
			{
				{
					troopUid,
					amount
				}
			});
			return new CheatDeployablesCommand(request);
		}
	}
}
