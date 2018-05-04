using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers.Entities.StateMachines.Attack;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Entities.Components
{
	public class ShooterComponent : ComponentBase
	{
		private const int MAX_TARGET_HISTORY = 5;

		public uint MinAttackRangeSquared;

		public uint MaxAttackRangeSquared;

		public uint RetargetingOffsetSquared;

		private SmartEntity mTarget;

		public IShooterVO ShooterVO
		{
			get;
			set;
		}

		public IShooterVO OriginalShooterVO
		{
			get;
			set;
		}

		public bool FirstTargetAcquired
		{
			get;
			set;
		}

		public uint ShotsRemainingInClip
		{
			get;
			set;
		}

		public bool ShouldCountClips
		{
			get;
			set;
		}

		public uint NumClipsUsed
		{
			get;
			set;
		}

		public bool TargetingDelayed
		{
			get;
			set;
		}

		public int TargetingDelayAmount
		{
			get;
			set;
		}

		public uint MinimumFrameCountForNextTargeting
		{
			get;
			set;
		}

		public bool Searching
		{
			get;
			set;
		}

		public bool ReevaluateTarget
		{
			get;
			set;
		}

		public bool IsMelee
		{
			get;
			private set;
		}

		public bool isSkinned
		{
			get;
			set;
		}

		public AttackFSM AttackFSM
		{
			get;
			set;
		}

		public int TargetOriginalBoardX
		{
			get;
			set;
		}

		public int TargetOriginalBoardZ
		{
			get;
			set;
		}

		public uint LastTargetID
		{
			get;
			set;
		}

		public List<uint> LastTargets
		{
			get;
			set;
		}

		public SmartEntity Target
		{
			get
			{
				return this.mTarget;
			}
			set
			{
				this.mTarget = value;
				if (this.mTarget != null && this.mTarget.TransformComp != null)
				{
					TransformComponent transformComp = this.mTarget.TransformComp;
					this.TargetOriginalBoardX = transformComp.CenterGridX();
					this.TargetOriginalBoardZ = transformComp.CenterGridZ();
				}
			}
		}

		public ShooterComponent(IShooterVO shooterVO)
		{
			this.OriginalShooterVO = shooterVO;
			this.SetVOData(shooterVO);
			this.LastTargetID = 0u;
			this.LastTargets = new List<uint>();
			this.Reset();
		}

		public void AddEntityTargetIdToHistory(uint targetEntityID)
		{
			if (this.LastTargets.Count == 5)
			{
				this.LastTargets.RemoveAt(0);
			}
			this.LastTargets.Add(targetEntityID);
		}

		public bool IsPotentialTargetNew(uint targetEntityID)
		{
			return !this.LastTargets.Contains(targetEntityID);
		}

		public void Reset()
		{
			this.Searching = true;
			this.ReevaluateTarget = false;
			this.FirstTargetAcquired = false;
			this.MinimumFrameCountForNextTargeting = 0u;
			this.ShotsRemainingInClip = 0u;
			this.NumClipsUsed = 0u;
			this.ShouldCountClips = false;
			this.TargetingDelayed = false;
			this.TargetingDelayAmount = 0;
			this.TargetOriginalBoardX = 0;
			this.TargetOriginalBoardZ = 0;
			this.AttackFSM = null;
			this.mTarget = null;
		}

		public void SetVOData(IShooterVO shooterVO)
		{
			this.ShooterVO = shooterVO;
			this.MinAttackRangeSquared = shooterVO.MinAttackRange * shooterVO.MinAttackRange;
			this.MaxAttackRangeSquared = shooterVO.MaxAttackRange * shooterVO.MaxAttackRange;
			this.RetargetingOffsetSquared = shooterVO.RetargetingOffset * shooterVO.RetargetingOffset;
			if (shooterVO is TroopTypeVO)
			{
				this.IsMelee = (shooterVO.MaxAttackRange < 4u);
			}
			else
			{
				this.IsMelee = false;
			}
		}

		public bool PrimaryTargetMoved()
		{
			if (this.mTarget == null || this.mTarget.TransformComp == null)
			{
				return false;
			}
			TransformComponent transformComp = this.mTarget.TransformComp;
			bool flag = (long)GameUtils.SquaredDistance(this.TargetOriginalBoardX, this.TargetOriginalBoardZ, transformComp.CenterGridX(), transformComp.CenterGridZ()) > (long)((ulong)this.RetargetingOffsetSquared);
			if (flag)
			{
				this.TargetOriginalBoardX = transformComp.CenterGridX();
				this.TargetOriginalBoardZ = transformComp.CenterGridZ();
			}
			return flag;
		}
	}
}
