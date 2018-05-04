using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Tags
{
	public class QueuedUnitTrainingTag
	{
		public List<Contract> Contracts
		{
			get;
			set;
		}

		public IDeployableVO UnitVO
		{
			get;
			set;
		}

		public float TimeLeftFloat
		{
			get;
			set;
		}

		public int TimeLeft
		{
			get
			{
				if (this.Contracts.Count == 0)
				{
					return 0;
				}
				return this.Contracts[0].GetRemainingTimeForView();
			}
		}

		public int TimeTotal
		{
			get
			{
				if (this.Contracts.Count == 0)
				{
					return 0;
				}
				return this.Contracts[0].TotalTime;
			}
		}

		public QueuedUnitTrainingTag()
		{
			this.TimeLeftFloat = 0f;
			this.Contracts = new List<Contract>();
		}
	}
}
