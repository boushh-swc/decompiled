using StaRTS.Externals.Manimal;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Utils;
using System;
using System.Text;

namespace StaRTS.Main.Models
{
	public class Contract
	{
		public string Tag;

		private int internalRemainingTime;

		public string ProductUid
		{
			get;
			private set;
		}

		public DeliveryType DeliveryType
		{
			get;
			private set;
		}

		public int TotalTime
		{
			get;
			private set;
		}

		public ContractTO ContractTO
		{
			get;
			set;
		}

		public double LastUpdateTime
		{
			get;
			set;
		}

		public bool DoNotShiftTimesForUnfreeze
		{
			get;
			set;
		}

		public Contract(string productUid, DeliveryType deliveryType, int totalTime, double startTime) : this(productUid, deliveryType, totalTime, startTime, string.Empty)
		{
		}

		public Contract(string productUid, DeliveryType deliveryType, int totalTime, double startTime, string tag)
		{
			this.ProductUid = productUid;
			this.DeliveryType = deliveryType;
			this.TotalTime = totalTime;
			this.internalRemainingTime = totalTime;
			this.LastUpdateTime = startTime;
			this.DoNotShiftTimesForUnfreeze = false;
			this.Tag = tag;
		}

		public void UpdateRemainingTime()
		{
			this.internalRemainingTime = this.GetRemainingTimeForSim();
		}

		public bool IsReadyToBeFinished()
		{
			return this.internalRemainingTime <= 0;
		}

		public int GetRemainingTimeForView()
		{
			return this.internalRemainingTime;
		}

		public int GetRemainingTimeForSim()
		{
			int num = 0;
			if (this.ContractTO != null)
			{
				uint endTime = this.ContractTO.EndTime;
				uint time = ServerTime.Time;
				if (endTime > time)
				{
					num = GameUtils.GetTimeDifferenceSafe(endTime, time);
					if (num > this.TotalTime)
					{
						num = this.TotalTime;
					}
				}
			}
			return num;
		}

		public void AddString(StringBuilder sb)
		{
			sb.Append(this.ContractTO.Uid).Append("|").Append(this.ContractTO.BuildingKey).Append("|").Append(this.ContractTO.EndTime).Append("|").Append(this.ContractTO.ContractType.ToString()).Append("|").Append("\n");
		}
	}
}
