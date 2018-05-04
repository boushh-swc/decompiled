using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class ShowTranscriptStoryAction : AbstractStoryAction
	{
		private const int TEXT_STRING_ARG = 0;

		private const int TITLE_STRING_ARG = 1;

		public string Title
		{
			get
			{
				return this.prepareArgs[1];
			}
		}

		public string Text
		{
			get
			{
				return this.prepareArgs[0];
			}
		}

		public ShowTranscriptStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(2);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			Service.EventManager.SendEvent(EventId.ShowTranscript, this);
			this.parent.ChildComplete(this);
		}
	}
}
