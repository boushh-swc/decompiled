using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class AbstractStoryAction : IStoryAction
	{
		protected StoryActionVO vo;

		protected IStoryReactor parent;

		protected string[] prepareArgs;

		public StoryActionVO VO
		{
			get
			{
				return this.vo;
			}
		}

		public virtual string Reaction
		{
			get
			{
				return this.vo.Reaction;
			}
		}

		public AbstractStoryAction(StoryActionVO vo, IStoryReactor parent)
		{
			this.vo = vo;
			this.parent = parent;
			if (!string.IsNullOrEmpty(vo.PrepareString))
			{
				this.prepareArgs = vo.PrepareString.Split(new char[]
				{
					'|'
				});
			}
			else
			{
				this.prepareArgs = new string[0];
			}
		}

		public virtual void Prepare()
		{
		}

		public virtual void Execute()
		{
			if (!string.IsNullOrEmpty(this.vo.LogType) && !string.IsNullOrEmpty(this.vo.LogTag))
			{
				Service.EventManager.SendEvent(EventId.LogStoryActionExecuted, this.vo);
			}
		}

		protected void VerifyArgumentCount(int requiredArguments)
		{
			if (this.prepareArgs.Length != requiredArguments)
			{
				Service.Logger.ErrorFormat("The StoryAction {0} has an incorrect number of arguments: {1}", new object[]
				{
					this.vo.Uid,
					this.vo.PrepareString
				});
			}
		}

		protected void VerifyArgumentCount(int[] requiredArguments)
		{
			bool flag = false;
			for (int i = 0; i < requiredArguments.Length; i++)
			{
				if (requiredArguments[i] == this.prepareArgs.Length)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Service.Logger.ErrorFormat("The StoryAction {0} has an incorrect number of arguments: {1}", new object[]
				{
					this.vo.Uid,
					this.vo.PrepareString
				});
			}
		}

		public virtual void Destroy()
		{
		}
	}
}
