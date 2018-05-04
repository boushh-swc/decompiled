using StaRTS.Main.Models.Squads;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Squads
{
	public interface ISquadMsgDisplay
	{
		void ProcessNewMessages(List<SquadMsg> messages);

		void RemoveMessage(SquadMsg msg);

		void Destroy();
	}
}
