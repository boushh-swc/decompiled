using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class IfPrefGateStoryAction : AbstractStoryAction
	{
		private string reactionUID;

		public override string Reaction
		{
			get
			{
				return this.reactionUID;
			}
		}

		public IfPrefGateStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
			this.reactionUID = string.Empty;
			if (string.IsNullOrEmpty(vo.PrepareString))
			{
			}
			if (this.prepareArgs.Length < 2)
			{
			}
			SharedPlayerPrefs sharedPlayerPrefs = Service.SharedPlayerPrefs;
			string pref = sharedPlayerPrefs.GetPref<string>(this.prepareArgs[0]);
			string text = this.prepareArgs[1];
			if (text.Equals(pref) || (text.Equals("false") && string.IsNullOrEmpty(pref)))
			{
				this.reactionUID = vo.Reaction;
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
