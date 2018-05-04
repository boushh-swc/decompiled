using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Story.Trigger
{
	public class AbstractStoryTrigger : IStoryTrigger, ISerializable, IStoryReactor
	{
		public const string UID_KEY = "uid";

		protected StoryTriggerVO vo;

		protected ITriggerReactor parent;

		protected string[] prepareArgs;

		protected IStoryAction updateAction;

		protected bool updateActionPrepared;

		public string Reaction
		{
			get;
			set;
		}

		public StoryTriggerVO VO
		{
			get
			{
				return this.vo;
			}
		}

		public bool HasReaction
		{
			get
			{
				return !string.IsNullOrEmpty(this.Reaction);
			}
		}

		public AbstractStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent)
		{
			this.vo = vo;
			this.parent = parent;
			this.Reaction = vo.Reaction;
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
			if (!string.IsNullOrEmpty(vo.UpdateAction))
			{
				StaticDataController staticDataController = Service.StaticDataController;
				try
				{
					StoryActionVO storyActionVO = staticDataController.Get<StoryActionVO>(vo.UpdateAction);
					this.updateAction = StoryActionFactory.GenerateStoryAction(storyActionVO, this);
					if (!string.IsNullOrEmpty(this.updateAction.VO.Reaction))
					{
						Service.Logger.ErrorFormat("Story chaining is not currently supported for UIActions. {0}, {1}", new object[]
						{
							vo.Uid,
							vo.UpdateAction
						});
					}
				}
				catch (KeyNotFoundException ex)
				{
					Service.Logger.ErrorFormat("Error in StoryTrigger {0}.  Could not find UiAction {1}.", new object[]
					{
						vo.Uid,
						vo.UpdateAction
					});
					throw ex;
				}
			}
		}

		public virtual void Activate()
		{
			Service.Logger.DebugFormat("Activating trigger {0}", new object[]
			{
				this.vo.Uid
			});
			if (this.updateAction != null)
			{
				this.updateAction.Prepare();
			}
		}

		public virtual void ChildPrepared(IStoryAction action)
		{
			this.updateActionPrepared = true;
		}

		public virtual void ChildComplete(IStoryAction action)
		{
		}

		public void UpdateAction()
		{
			if (this.updateActionPrepared)
			{
				this.updateAction.Execute();
			}
		}

		public virtual void Destroy()
		{
			Service.Logger.DebugFormat("Destroying trigger {0}", new object[]
			{
				this.vo.Uid
			});
			this.vo = null;
			this.parent = null;
			this.prepareArgs = null;
			this.updateAction = null;
		}

		public string ToJson()
		{
			Serializer serializer = Serializer.Start();
			this.AddData(ref serializer);
			return serializer.End().ToString();
		}

		protected virtual void AddData(ref Serializer serializer)
		{
			serializer.AddString("uid", this.vo.Uid);
		}

		public virtual ISerializable FromObject(object obj)
		{
			return this;
		}

		public virtual bool IsPreSatisfied()
		{
			return false;
		}
	}
}
