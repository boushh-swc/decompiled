using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Bot
{
	public class BotRunner : IViewFrameTimeObserver, IBotNotifierParent
	{
		public Dictionary<string, object> BotProperties;

		public bool Performing;

		public BotNotifier NextNotifier
		{
			get;
			set;
		}

		public BotRunner()
		{
			Service.BotRunner = this;
			this.BotProperties = new Dictionary<string, object>();
			this.Performing = false;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		public BotPerformer GenerateRequesterScript(int battleCount)
		{
			BotPerformer botPerformer = new BPSetInteger().Init(new KeyValuePair<string, int>("requesting", 1));
			BotPerformer botPerformer2 = new BPSetInteger().Init(new KeyValuePair<string, int>("battles", battleCount));
			BotNotifier botNotifier = new BNWhilePropNonZero().Init("requesting");
			BotPerformer nextPerformer = new BPStartNotifier().Init(botNotifier);
			BotNotifier botNotifier2 = new BNWhilePropNonZero().Init("battles");
			BotNotifier botNotifier3 = new BNSquadCenterHasSpace();
			botPerformer.NextPerformer = botPerformer2;
			botPerformer2.NextPerformer = nextPerformer;
			botNotifier.AddNotifier(botNotifier2);
			botNotifier2.AddNotifier(botNotifier3);
			BotPerformer botPerformer3 = new BPSendTroopRequest();
			BotPerformer nextPerformer2 = new BPDelay().Init(10f);
			botNotifier3.SuccessPerformer = botPerformer3;
			botPerformer3.NextPerformer = nextPerformer2;
			BotPerformer botPerformer4 = new BPDelay().Init(1f);
			botNotifier3.FailurePerformer = botPerformer4;
			BotPerformer botPerformer5 = new BPStartRandomPvp();
			BotPerformer botPerformer6 = new BPDelay().Init(5f);
			BotPerformer botPerformer7 = new BPDeploySquadTroops();
			BotPerformer botPerformer8 = new BPDelay().Init(2f);
			BotPerformer botPerformer9 = new BPEndRandomPvp();
			BotPerformer nextPerformer3 = new BPDecrementInteger().Init("battles");
			botPerformer4.NextPerformer = botPerformer5;
			botPerformer5.NextPerformer = botPerformer6;
			botPerformer6.NextPerformer = botPerformer7;
			botPerformer7.NextPerformer = botPerformer8;
			botPerformer8.NextPerformer = botPerformer9;
			botPerformer9.NextPerformer = nextPerformer3;
			BotPerformer botPerformer10 = new BPSetInteger().Init(new KeyValuePair<string, int>("requesting", 0));
			botNotifier2.FailurePerformer = botPerformer10;
			botNotifier.FailurePerformer = botPerformer10;
			BotPerformer nextPerformer4 = new BPStopNotifier().Init(botNotifier);
			botPerformer10.NextPerformer = nextPerformer4;
			return botPerformer;
		}

		public BotPerformer GenerateDonatorScript()
		{
			BotPerformer botPerformer = new BPSetInteger().Init(new KeyValuePair<string, int>("donating", 1));
			BotNotifier botNotifier = new BNWhilePropNonZero().Init("donating");
			BotPerformer nextPerformer = new BPStartNotifier().Init(botNotifier);
			botPerformer.NextPerformer = nextPerformer;
			BotNotifier botNotifier2 = new BNSquadRequestOpen().Init("trooprequest");
			botNotifier.AddNotifier(botNotifier2);
			BotPerformer botPerformer2 = new BPTrainSoldiers().Init(4);
			BotPerformer botPerformer3 = new BPDelay().Init(1f);
			BotPerformer botPerformer4 = new BPDonateTroops().Init("trooprequest");
			BotPerformer botPerformer5 = new BPDelay().Init(6f);
			botNotifier2.UpdatePerformer = botPerformer2;
			botNotifier2.FailurePerformer = botPerformer5;
			botPerformer2.NextPerformer = botPerformer3;
			botPerformer3.NextPerformer = botPerformer4;
			botPerformer4.NextPerformer = botPerformer5;
			return botPerformer;
		}

		public void PerformScript(BotPerformer start)
		{
			start.Perform();
		}

		public void AddNotifier(BotNotifier notifier)
		{
			if (this.NextNotifier == null)
			{
				this.NextNotifier = notifier;
				this.NextNotifier.Parent = this;
				return;
			}
			this.NextNotifier.AddNotifier(notifier);
		}

		public void OnViewFrameTime(float dt)
		{
			if (this.NextNotifier == null)
			{
				return;
			}
			if (this.Performing)
			{
				return;
			}
			this.NextNotifier.Update();
		}

		public void Log(string message, params object[] args)
		{
			message = string.Format(message, args);
			Service.Logger.DebugFormat("<color=cyan>BOT | {0}</color>", new object[]
			{
				message
			});
		}
	}
}
