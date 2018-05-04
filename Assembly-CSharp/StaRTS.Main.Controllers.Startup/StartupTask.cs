using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class StartupTask
	{
		protected float curPercentage;

		protected string curDescription;

		protected float startPercentage;

		protected float endPercentage;

		public StartupTaskController Startup
		{
			get;
			set;
		}

		public float EndPercentage
		{
			get
			{
				return this.endPercentage;
			}
			set
			{
				this.endPercentage = value;
			}
		}

		public float Percentage
		{
			get
			{
				return this.curPercentage;
			}
		}

		public string Description
		{
			get
			{
				return this.curDescription;
			}
		}

		public StartupTask(float startPercentage)
		{
			this.startPercentage = startPercentage;
			this.curPercentage = startPercentage;
			this.endPercentage = startPercentage;
			this.curDescription = string.Empty;
		}

		public virtual void Start()
		{
		}

		protected void Complete()
		{
			this.curPercentage = this.endPercentage;
			this.Startup.OnTaskComplete(this);
		}
	}
}
