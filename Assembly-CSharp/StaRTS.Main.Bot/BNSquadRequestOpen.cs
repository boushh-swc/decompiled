using StaRTS.Main.Models.Squads;
using StaRTS.Main.Views.UX.Squads;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Bot
{
	public class BNSquadRequestOpen : BotNotifier, ISquadMsgDisplay
	{
		public override BotNotifier Init(object arg)
		{
			Service.SquadController.MsgManager.RegisterObserver(this);
			Service.BotRunner.BotProperties.Add((string)arg, null);
			return base.Init(arg);
		}

		public override bool EvaluateUpdate()
		{
			Service.BotRunner.Log(this.arg + " != null : " + (Service.BotRunner.BotProperties[(string)this.arg] != null), new object[0]);
			return Service.BotRunner.BotProperties[(string)this.arg] != null;
		}

		public void ProcessNewMessages(List<SquadMsg> messages)
		{
			int i = 0;
			int count = messages.Count;
			while (i < count)
			{
				this.ProcessMessage(messages[i]);
				i++;
			}
		}

		protected bool ProcessMessage(SquadMsg msg)
		{
			if (msg.RequestData == null)
			{
				return false;
			}
			Service.BotRunner.Log(string.Concat(new object[]
			{
				"Setting property ",
				this.arg,
				" to ",
				msg
			}), new object[0]);
			Service.BotRunner.BotProperties[(string)this.arg] = msg;
			return true;
		}

		public void RemoveMessage(SquadMsg msg)
		{
		}

		public void Destroy()
		{
		}
	}
}
