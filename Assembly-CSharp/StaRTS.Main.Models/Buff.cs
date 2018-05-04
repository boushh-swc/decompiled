using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models
{
	public class Buff
	{
		public const string ORIGINATOR = "originator";

		public Dictionary<string, object> BuffData;

		private int msRemaining;

		private int msToNextProc;

		private int armorTypeIndex;

		public BuffTypeVO BuffType
		{
			get;
			private set;
		}

		public int ProcCount
		{
			get;
			private set;
		}

		public int StackSize
		{
			get;
			private set;
		}

		public BuffVisualPriority VisualPriority
		{
			get;
			private set;
		}

		public IValueObject Details
		{
			get;
			private set;
		}

		public Buff(BuffTypeVO buffType, ArmorType armorType, BuffVisualPriority visualPriority)
		{
			this.BuffType = buffType;
			this.ProcCount = 0;
			this.StackSize = 0;
			this.msRemaining = this.BuffType.Duration;
			this.msToNextProc = this.BuffType.MillisecondsToFirstProc;
			this.armorTypeIndex = (int)armorType;
			this.VisualPriority = visualPriority;
			this.BuffData = new Dictionary<string, object>();
			if (buffType.Modify == BuffModify.Summon)
			{
				this.Details = Service.StaticDataController.Get<SummonDetailsVO>(buffType.Details);
			}
		}

		public void AddStack()
		{
			if (this.BuffType.MaxStacks == 0u || this.StackSize < (int)this.BuffType.MaxStacks)
			{
				this.StackSize++;
			}
			if (this.BuffType.IsRefreshing)
			{
				this.msRemaining = this.BuffType.Duration;
				this.msToNextProc = this.BuffType.MillisecondsToFirstProc;
			}
		}

		public bool RemoveStack()
		{
			return this.StackSize == 0 || --this.StackSize == 0;
		}

		public void UpgradeBuff(BuffTypeVO newBuffType)
		{
			this.UpgradeTime(ref this.msRemaining, this.BuffType.Duration, newBuffType.Duration);
			if (this.ProcCount == 0)
			{
				this.UpgradeTime(ref this.msToNextProc, this.BuffType.MillisecondsToFirstProc, newBuffType.MillisecondsToFirstProc);
			}
			else
			{
				this.UpgradeTime(ref this.msToNextProc, this.BuffType.MillisecondsPerProc, newBuffType.MillisecondsPerProc);
			}
			this.BuffType = newBuffType;
		}

		private void UpgradeTime(ref int msTimeLeft, int oldTotalTime, int newTotalTime)
		{
			if (msTimeLeft < 0 || newTotalTime < 0)
			{
				msTimeLeft = newTotalTime;
			}
			else
			{
				msTimeLeft += newTotalTime - oldTotalTime;
				if (msTimeLeft < 0)
				{
					msTimeLeft = 0;
				}
			}
		}

		public void UpdateBuff(uint dt, out bool proc, out bool done)
		{
			proc = false;
			done = false;
			int num = 0;
			if (this.msRemaining >= 0)
			{
				this.msRemaining -= (int)dt;
				if (this.msRemaining <= 0)
				{
					done = true;
					num = this.msRemaining;
					this.msRemaining = 0;
				}
			}
			if (this.msToNextProc >= 0)
			{
				this.msToNextProc -= (int)dt;
				if (this.msToNextProc <= 0)
				{
					if (this.msToNextProc <= num)
					{
						proc = true;
						this.ProcCount++;
					}
					num = this.msToNextProc;
					this.msToNextProc = this.BuffType.MillisecondsPerProc + num;
					if (this.msToNextProc < 0 && this.BuffType.MillisecondsPerProc >= 0)
					{
						this.msToNextProc = 0;
					}
				}
			}
		}

		public void ApplyStacks(ref int modifyValue, int modifyValueMax)
		{
			int num = this.BuffType.Values[this.armorTypeIndex];
			switch (this.BuffType.ApplyAs)
			{
			case BuffApplyAs.Relative:
				modifyValue += this.StackSize * num;
				break;
			case BuffApplyAs.Absolute:
				modifyValue = this.StackSize * num;
				break;
			case BuffApplyAs.RelativePercent:
				for (int i = 0; i < this.StackSize; i++)
				{
					modifyValue += IntMath.GetPercent(num, modifyValue);
				}
				break;
			case BuffApplyAs.AbsolutePercent:
				for (int j = 0; j < this.StackSize; j++)
				{
					modifyValue = IntMath.GetPercent(num, modifyValue);
				}
				break;
			case BuffApplyAs.RelativePercentOfMax:
				modifyValue += this.StackSize * IntMath.GetPercent(num, modifyValueMax);
				break;
			case BuffApplyAs.AbsolutePercentOfMax:
				modifyValue = this.StackSize * IntMath.GetPercent(num, modifyValueMax);
				break;
			}
		}
	}
}
