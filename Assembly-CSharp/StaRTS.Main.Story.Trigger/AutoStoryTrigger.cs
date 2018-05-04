using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class AutoStoryTrigger : AbstractStoryTrigger
	{
		public const string REQ_IF_PREF = "IF_PREF";

		public const string REQ_RESUME_STORY = "RESUME";

		private string savePrefName;

		private string saveValueReq;

		public AutoStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
			if (string.IsNullOrEmpty(vo.PrepareString))
			{
				Service.Logger.Error("AutoStoryTrigger: " + vo.Uid + " is missing prepare string");
			}
			if (this.prepareArgs.Length < 3)
			{
				Service.Logger.Error("AutoStoryTrigger: " + vo.Uid + " doesn't have enough arguments");
			}
			this.savePrefName = this.prepareArgs[1];
			this.saveValueReq = this.prepareArgs[2];
		}

		public override void Activate()
		{
			base.Activate();
			this.parent.SatisfyTrigger(this);
		}

		public override bool IsPreSatisfied()
		{
			SharedPlayerPrefs sharedPlayerPrefs = Service.SharedPlayerPrefs;
			string pref = sharedPlayerPrefs.GetPref<string>(this.savePrefName);
			return this.saveValueReq.Equals(pref) || (this.saveValueReq.Equals("false") && string.IsNullOrEmpty(pref));
		}
	}
}
