using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class RebelEmpireForkingStoryAction : AbstractStoryAction
	{
		private string reactionUID;

		public override string Reaction
		{
			get
			{
				return this.reactionUID;
			}
		}

		public RebelEmpireForkingStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
			char[] separator = new char[]
			{
				'|'
			};
			string[] array = vo.PrepareString.Split(separator, StringSplitOptions.None);
			if (array.Length < 2)
			{
				Service.Logger.Error("RebelEmpireForkingStoryAction lacking params: " + this.vo.Uid);
			}
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (currentPlayer.CampaignProgress.FueInProgress)
			{
				Service.Logger.Error("Cannot do forking actions in FUE only later guided experiences");
				this.Execute();
			}
			FactionType faction = currentPlayer.Faction;
			if (faction == FactionType.Rebel)
			{
				this.reactionUID = array[0];
			}
			else
			{
				this.reactionUID = array[1];
			}
		}

		public override void Prepare()
		{
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			this.parent.ChildComplete(this);
		}
	}
}
