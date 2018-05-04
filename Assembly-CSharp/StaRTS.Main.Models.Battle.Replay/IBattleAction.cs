using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Battle.Replay
{
	public interface IBattleAction : ISerializable
	{
		string ActionId
		{
			get;
		}

		uint Time
		{
			get;
		}
	}
}
