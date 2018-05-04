using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Story.Actions
{
	public class ClusterANDStoryAction : AbstractStoryAction, IStoryReactor
	{
		private const int UID_LIST_ARG = 0;

		private const int CREATED = 0;

		private const int PREPARED = 1;

		private const int EXECUTED = 2;

		private Dictionary<IStoryAction, int> children;

		public ClusterANDStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(1);
			bool flag = false;
			bool flag2 = false;
			this.children = new Dictionary<IStoryAction, int>();
			StaticDataController staticDataController = Service.StaticDataController;
			string[] array = this.prepareArgs[0].Split(new char[]
			{
				','
			});
			IStoryAction[] array2 = new IStoryAction[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				StoryActionVO storyActionVO;
				try
				{
					storyActionVO = staticDataController.Get<StoryActionVO>(array[i].Trim());
				}
				catch (KeyNotFoundException ex)
				{
					Service.Logger.ErrorFormat("Error in Story Action {0}.  Could not find Action {1}.", new object[]
					{
						this.vo.Uid,
						array[i]
					});
					throw ex;
				}
				if (storyActionVO.Uid == this.vo.Uid)
				{
					Service.Logger.ErrorFormat("Error in Story Action {0}.  ClusterAND cannot contain itself as a child.", new object[]
					{
						this.vo.Uid
					});
					return;
				}
				if (storyActionVO.ActionType == "ShowHolo")
				{
					flag = true;
				}
				else if (storyActionVO.ActionType == "PlayHoloAnim")
				{
					flag2 = true;
				}
				IStoryAction storyAction = StoryActionFactory.GenerateStoryAction(storyActionVO, this);
				this.children.Add(storyAction, 0);
				array2[i] = storyAction;
			}
			if (flag && flag2)
			{
				Service.Logger.ErrorFormat("ClusterAND {0} contains both ShowHolo and AnimateHolo. {1}", new object[]
				{
					this.vo.Uid,
					"Please only animate holograms AFTER ShowHolo is complete"
				});
				return;
			}
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].Prepare();
			}
		}

		public override void Execute()
		{
			base.Execute();
			IStoryAction[] array = new IStoryAction[this.children.Keys.Count];
			this.children.Keys.CopyTo(array, 0);
			for (int i = 0; i < array.Length; i++)
			{
				Service.QuestController.LogAction(array[i].VO);
				array[i].Execute();
			}
		}

		public void ChildPrepared(IStoryAction child)
		{
			this.children[child] = 1;
			foreach (int current in this.children.Values)
			{
				if (current < 1)
				{
					return;
				}
			}
			this.parent.ChildPrepared(this);
		}

		public void ChildComplete(IStoryAction child)
		{
			this.children[child] = 2;
			foreach (int current in this.children.Values)
			{
				if (current < 2)
				{
					return;
				}
			}
			this.parent.ChildComplete(this);
		}
	}
}
