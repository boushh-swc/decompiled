using StaRTS.Main.Models.Commands.Player.Fue;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class SaveProgressStoryAction : AbstractStoryAction
	{
		private const int QUEST_UID_ARG = 0;

		public SaveProgressStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(new int[]
			{
				0,
				1
			});
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			string text = this.vo.Uid;
			if (this.prepareArgs.Length > 0)
			{
				text = this.prepareArgs[0];
			}
			if (text == Service.CurrentPlayer.CurrentQuest)
			{
				Service.ServerAPI.Enabled = false;
				this.parent.ChildComplete(this);
				return;
			}
			if (!string.IsNullOrEmpty(text))
			{
				Service.ServerAPI.Enabled = true;
				Service.CurrentPlayer.CurrentQuest = text;
				Service.ServerAPI.Sync(new FueUpdateStateCommand());
				Service.ServerAPI.Enabled = false;
			}
			else
			{
				Service.Logger.Error("Please use the EndFUE command to end the FUE and not SaveProgress!");
			}
			this.parent.ChildComplete(this);
		}
	}
}
