using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class PlayAudioStoryAction : AbstractStoryAction
	{
		private const int AUDIO_CLIP_ARG = 0;

		public PlayAudioStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
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
			if (LangUtils.ShouldPlayVOClips())
			{
				Service.AudioManager.PlayAudio(this.prepareArgs[0]);
			}
			this.parent.ChildComplete(this);
		}
	}
}
