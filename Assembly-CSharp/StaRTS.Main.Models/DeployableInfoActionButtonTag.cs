using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models
{
	public class DeployableInfoActionButtonTag
	{
		public string ActionId
		{
			get;
			private set;
		}

		public List<string> DataList
		{
			get;
			private set;
		}

		public DeployableInfoActionButtonTag(string action, string data)
		{
			this.ActionId = action;
			if (!string.IsNullOrEmpty(data))
			{
				this.DataList = new List<string>(data.Split(new char[]
				{
					' '
				}));
			}
		}
	}
}
