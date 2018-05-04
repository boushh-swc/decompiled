using System;

namespace StaRTS.Utils.Core
{
	public class RefCount
	{
		private int count;

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public RefCount()
		{
			this.count = 0;
		}

		public RefCount(int count)
		{
			this.count = count;
		}

		public int AddRef()
		{
			return ++this.count;
		}

		public int Release()
		{
			return (this.count <= 0) ? 0 : (--this.count);
		}
	}
}
