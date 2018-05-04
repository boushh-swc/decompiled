using StaRTS.Main.Story;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Player.Fue
{
	public class FueUpdateStateRequest : PlayerIdChecksumRequest
	{
		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			List<IStoryTrigger> activeSaveTriggers = Service.CurrentPlayer.ActiveSaveTriggers;
			startedSerializer.AddString("FueUid", Service.CurrentPlayer.CurrentQuest);
			startedSerializer.AddArray<IStoryTrigger>("triggers", activeSaveTriggers);
			return startedSerializer.End().ToString();
		}
	}
}
