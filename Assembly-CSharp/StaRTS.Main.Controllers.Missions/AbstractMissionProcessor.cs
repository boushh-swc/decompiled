using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Missions
{
	public class AbstractMissionProcessor
	{
		protected MissionConductor parent;

		public AbstractMissionProcessor(MissionConductor parent)
		{
			this.parent = parent;
		}

		public virtual void Start()
		{
			Service.Logger.ErrorFormat("The mission processor for mission {0} does not have a Start() method defined.", new object[]
			{
				this.parent.MissionVO.Uid
			});
		}

		public virtual void Resume()
		{
			Service.Logger.ErrorFormat("The mission processor for mission {0} does not have a Resume() method defined.", new object[]
			{
				this.parent.MissionVO.Uid
			});
		}

		public virtual void OnIntroHookComplete()
		{
		}

		public virtual void OnSuccessHookComplete()
		{
		}

		public virtual void OnFailureHookComplete()
		{
		}

		public virtual void OnGoalFailureHookComplete()
		{
		}

		public virtual void OnCancel()
		{
		}

		public virtual void Destroy()
		{
		}

		protected void PauseBattle()
		{
			Service.SimTimeEngine.ScaleTime(0u);
			Service.UserInputInhibitor.DenyAll();
		}

		protected void ResumeBattle()
		{
			Service.SimTimeEngine.ScaleTime(1u);
			Service.UserInputInhibitor.AllowAll();
		}
	}
}
