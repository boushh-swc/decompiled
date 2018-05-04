using System;

namespace Net.RichardLord.Ash.Core
{
	public abstract class ComponentBase
	{
		public Entity Entity;

		public virtual void OnRemove()
		{
		}
	}
}
