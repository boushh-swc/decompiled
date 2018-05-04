using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Story.Actions;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Story
{
	public class ActionChain : IStoryReactor
	{
		private List<IStoryAction> actions;

		private int currentActionIndex;

		private int recursiveCounter;

		private const int RECURSIVE_LIMIT = 500;

		private bool destroying;

		public bool Valid
		{
			get;
			set;
		}

		public ActionChain(string firstActionUid)
		{
			this.Valid = true;
			this.actions = new List<IStoryAction>();
			StaticDataController staticDataController = Service.StaticDataController;
			string text = firstActionUid;
			this.recursiveCounter = 0;
			bool flag = true;
			while (!string.IsNullOrEmpty(text) && this.recursiveCounter < 500)
			{
				this.recursiveCounter++;
				if (this.recursiveCounter > 500)
				{
					Service.Logger.ErrorFormat("Bad Metadata.  The story chain that starts with {0} has caused a loop.", new object[]
					{
						firstActionUid
					});
					this.Valid = false;
					return;
				}
				try
				{
					StoryActionVO vo = staticDataController.Get<StoryActionVO>(text);
					IStoryAction storyAction = StoryActionFactory.GenerateStoryAction(vo, this);
					this.actions.Add(storyAction);
					text = storyAction.Reaction;
					if (storyAction is EndChainStoryAction)
					{
						flag = false;
					}
				}
				catch (KeyNotFoundException ex)
				{
					Service.Logger.ErrorFormat("Error in Story Chain Starting with {0}.  Could not find Action {1}. {2}", new object[]
					{
						firstActionUid,
						text,
						ex.Message
					});
					this.Valid = false;
					return;
				}
			}
			if (flag)
			{
				IStoryAction item = StoryActionFactory.GenerateStoryAction(new StoryActionVO
				{
					ActionType = "EndChain",
					Uid = "autoEnd" + Service.QuestController.AutoIncrement()
				}, this);
				this.actions.Add(item);
			}
			this.currentActionIndex = -1;
			this.PrepareNextAction();
		}

		public void ChildComplete(IStoryAction action)
		{
			this.ExecuteNextAction();
		}

		public void ChildPrepared(IStoryAction action)
		{
			this.PrepareNextAction();
		}

		private void PrepareNextAction()
		{
			this.currentActionIndex++;
			if (this.currentActionIndex >= this.actions.Count)
			{
				this.currentActionIndex = -1;
				this.ExecuteNextAction();
			}
			else
			{
				this.actions[this.currentActionIndex].Prepare();
			}
		}

		private void ExecuteNextAction()
		{
			if (this.destroying)
			{
				return;
			}
			this.currentActionIndex++;
			if (this.currentActionIndex >= this.actions.Count)
			{
				this.Destroy();
			}
			else
			{
				StoryActionVO vO = this.actions[this.currentActionIndex].VO;
				Service.QuestController.LogAction(vO);
				this.actions[this.currentActionIndex].Execute();
			}
		}

		public void Destroy()
		{
			this.destroying = true;
			int i = this.currentActionIndex;
			int count = this.actions.Count;
			while (i < count)
			{
				this.actions[i].Destroy();
				i++;
			}
			this.actions.Clear();
		}
	}
}
