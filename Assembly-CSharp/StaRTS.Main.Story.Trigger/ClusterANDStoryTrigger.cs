using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Story.Trigger
{
	public class ClusterANDStoryTrigger : AbstractStoryTrigger, ITriggerReactor
	{
		private const int TRIGGER_LIST_ARG = 0;

		private Dictionary<IStoryTrigger, bool> children;

		public ClusterANDStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
		}

		public override void Activate()
		{
			StaticDataController staticDataController = Service.StaticDataController;
			this.children = new Dictionary<IStoryTrigger, bool>();
			string[] array = this.prepareArgs[0].Split(new char[]
			{
				','
			});
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string uid = array2[i];
				StoryTriggerVO vo = staticDataController.Get<StoryTriggerVO>(uid);
				IStoryTrigger storyTrigger = StoryTriggerFactory.GenerateStoryTrigger(vo, this);
				this.children.Add(storyTrigger, false);
				storyTrigger.Activate();
			}
		}

		public void SatisfyTrigger(IStoryTrigger trigger)
		{
			this.children[trigger] = true;
			using (Dictionary<IStoryTrigger, bool>.ValueCollection.Enumerator enumerator = this.children.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current)
					{
						return;
					}
				}
			}
			this.parent.SatisfyTrigger(this);
		}

		public override void Destroy()
		{
			foreach (IStoryTrigger current in this.children.Keys)
			{
				current.Destroy();
			}
			this.children.Clear();
			base.Destroy();
		}
	}
}
