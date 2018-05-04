using StaRTS.Assets;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.Animations;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class TextCrawlStoryAction : AbstractStoryAction
	{
		private TextCrawlAnimation anim;

		public TextCrawlStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		private void OnAnimationDoneLoading(object cookie)
		{
			this.parent.ChildPrepared(this);
		}

		public override void Prepare()
		{
			char[] separator = new char[]
			{
				'|'
			};
			string[] array = this.vo.PrepareString.Split(separator, StringSplitOptions.None);
			string chapterNumber = array[0];
			string chapterName = array[1];
			string chapterBody = array[2];
			this.anim = new TextCrawlAnimation(chapterNumber, chapterName, chapterBody, new AssetsCompleteDelegate(this.OnAnimationDoneLoading), null, new TextCrawlAnimation.TextCrawlAnimationCompleteDelegate(this.OnAnimationComplete));
		}

		private void OnAnimationComplete()
		{
			this.parent.ChildComplete(this);
		}

		public override void Execute()
		{
			this.anim.Start();
			base.Execute();
		}
	}
}
