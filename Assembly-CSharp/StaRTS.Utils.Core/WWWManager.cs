using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Utils.Core
{
	public class WWWManager
	{
		private List<WWW> outstandingWWWs;

		public WWWManager()
		{
			Service.WWWManager = this;
			this.outstandingWWWs = new List<WWW>();
		}

		public void CancelAll()
		{
			int i = 0;
			int count = this.outstandingWWWs.Count;
			while (i < count)
			{
				this.outstandingWWWs[i].Dispose();
				i++;
			}
			this.outstandingWWWs.Clear();
		}

		public static void Add(WWW www)
		{
			if (Service.WWWManager != null)
			{
				Service.WWWManager.outstandingWWWs.Add(www);
			}
		}

		public static bool Remove(WWW www)
		{
			return Service.WWWManager != null && Service.WWWManager.outstandingWWWs.Remove(www);
		}
	}
}
