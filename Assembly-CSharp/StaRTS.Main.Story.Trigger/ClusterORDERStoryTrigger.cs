using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class ClusterORDERStoryTrigger : AbstractStoryTrigger, ITriggerReactor
	{
		private const int TRIGGER_LIST_ARG = 0;

		private string[] uids;

		private int childIndex;

		private IStoryTrigger currentTrigger;

		private StaticDataController sdc;

		public ClusterORDERStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
		}

		public override void Activate()
		{
			this.sdc = Service.StaticDataController;
			this.uids = this.prepareArgs[0].Split(new char[]
			{
				','
			});
			this.childIndex = -1;
			this.ActivateNextChild();
		}

		private void ActivateNextChild()
		{
			this.childIndex++;
			if (this.childIndex < this.uids.Length)
			{
				StoryTriggerVO vo = this.sdc.Get<StoryTriggerVO>(this.uids[this.childIndex]);
				this.currentTrigger = StoryTriggerFactory.GenerateStoryTrigger(vo, this);
				this.currentTrigger.Activate();
			}
			else
			{
				this.parent.SatisfyTrigger(this);
			}
		}

		public void SatisfyTrigger(IStoryTrigger trigger)
		{
			this.ActivateNextChild();
		}

		public override void Destroy()
		{
			if (this.currentTrigger != null)
			{
				this.currentTrigger.Destroy();
				this.currentTrigger = null;
			}
			this.uids = null;
			base.Destroy();
		}
	}
}
