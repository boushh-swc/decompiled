using System;

namespace Net.RichardLord.Ash.Core
{
	public abstract class Node<TSibling>
	{
		public Entity Entity
		{
			get;
			set;
		}

		public TSibling Previous
		{
			get;
			set;
		}

		public TSibling Next
		{
			get;
			set;
		}

		public Node()
		{
		}

		public object GetProperty(string propertyName)
		{
			return base.GetType().GetProperty(propertyName).GetValue(this, null);
		}

		public virtual bool IsValid()
		{
			return this.Entity != null;
		}

		public void SetProperty(string propertyName, object value)
		{
			base.GetType().GetProperty(propertyName).SetValue(this, value, null);
		}
	}
}
