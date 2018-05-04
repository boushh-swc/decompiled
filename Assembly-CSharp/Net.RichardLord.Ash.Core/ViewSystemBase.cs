using System;

namespace Net.RichardLord.Ash.Core
{
	public abstract class ViewSystemBase : SystemBase<ViewSystemBase>
	{
		internal float DT
		{
			get;
			private set;
		}

		protected abstract void Update(float dt);

		public override void Update()
		{
			this.Update(this.DT);
			this.DT = 0f;
		}

		public void AccumulateDT(float dt)
		{
			this.DT += dt;
		}
	}
}
