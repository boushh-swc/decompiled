using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.VictoryConditions
{
	public class AbstractCondition
	{
		protected ConditionVO vo;

		protected IConditionParent parent;

		protected EventManager events;

		protected string[] prepareArgs;

		public AbstractCondition(ConditionVO vo, IConditionParent parent)
		{
			this.events = Service.EventManager;
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

		public virtual void Destroy()
		{
		}

		public ConditionVO GetConditionVo()
		{
			return this.vo;
		}

		public virtual bool IsConditionSatisfied()
		{
			return false;
		}

		public virtual void GetProgress(out int current, out int total)
		{
			current = 0;
			total = 1;
		}

		public virtual void Start()
		{
		}
	}
}
