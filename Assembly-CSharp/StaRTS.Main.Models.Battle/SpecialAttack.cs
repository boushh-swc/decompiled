using StaRTS.Assets;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.Battle
{
	public class SpecialAttack
	{
		private const string ATTACHED_SHADOW_TRIGGER = "Stored";

		private const string FALLING_SHADOW_TRIGGER = "Drop";

		private const string GROUND_SHADOW_TRIGGER = "Idle";

		public AssetHandle Handle;

		public AssetHandle AttachmentHandle;

		private int shotIndex;

		public GameObject StarshipGameObject
		{
			get;
			set;
		}

		public GameObject StarshipDetachableGameObject
		{
			get;
			set;
		}

		public ShadowAnim ShadowAnimator
		{
			get;
			set;
		}

		public ShadowAnim DetachableObjectShadowAnimator
		{
			get;
			private set;
		}

		public Animation DetachableShadowAnimation
		{
			get;
			set;
		}

		public List<GameObject> GunLocators
		{
			get;
			set;
		}

		public LandingTakeOffEffectAnim EffectAnimator
		{
			get;
			set;
		}

		public SpecialAttackTypeVO VO
		{
			get;
			private set;
		}

		public TeamType TeamType
		{
			get;
			private set;
		}

		public Vector3 TargetWorldPos
		{
			get;
			set;
		}

		public int TargetBoardX
		{
			get;
			set;
		}

		public int TargetBoardZ
		{
			get;
			set;
		}

		public ShieldGeneratorComponent TargetShield
		{
			get;
			set;
		}

		public List<Buff> SpecialAttackBuffs
		{
			get;
			private set;
		}

		public uint AttackerIndex
		{
			get;
			set;
		}

		public uint SpawnDelay
		{
			get;
			set;
		}

		public bool PlayerDeployed
		{
			get;
			private set;
		}

		public SpecialAttack(SpecialAttackTypeVO vo, TeamType teamType, Vector3 targetWorldPos, int targetBoardX, int targetBoardZ, bool playerDeployed)
		{
			this.VO = vo;
			this.TeamType = teamType;
			this.TargetWorldPos = targetWorldPos;
			this.TargetBoardX = targetBoardX;
			this.TargetBoardZ = targetBoardZ;
			this.SpecialAttackBuffs = new List<Buff>();
			this.StarshipGameObject = null;
			this.GunLocators = null;
			this.StarshipDetachableGameObject = null;
			this.DetachableShadowAnimation = null;
			this.TargetShield = null;
			this.PlayerDeployed = playerDeployed;
		}

		public void SetupDetachableShadowAnimator()
		{
			if (this.StarshipDetachableGameObject != null)
			{
				this.DetachableShadowAnimation = this.StarshipDetachableGameObject.GetComponent<Animation>();
				this.DetachableObjectShadowAnimator = new ShadowAnim(this.StarshipDetachableGameObject, 0f);
				this.DetachableObjectShadowAnimator.PlayShadowAnim(true);
			}
			else
			{
				this.DetachableShadowAnimation = null;
			}
		}

		public void UpdateDetachableShadowAnimator(SpecialAttackDetachableObjectState state)
		{
			if (this.DetachableShadowAnimation != null)
			{
				switch (state)
				{
				case SpecialAttackDetachableObjectState.Attached:
					this.DetachableShadowAnimation.Play("Stored");
					break;
				case SpecialAttackDetachableObjectState.Falling:
					this.DetachableShadowAnimation.Play("Drop");
					break;
				case SpecialAttackDetachableObjectState.OnGround:
					this.DetachableShadowAnimation.Play("Idle");
					break;
				}
			}
		}

		public GameObject GetGunLocator()
		{
			if (this.GunLocators == null || this.GunLocators.Count < 1)
			{
				return null;
			}
			this.shotIndex = (this.shotIndex + 1) % this.GunLocators.Count;
			if (this.GunLocators == null)
			{
				return null;
			}
			return this.GunLocators[this.shotIndex];
		}

		private int FindSpecialAttackBuff(string buffID)
		{
			int count = this.SpecialAttackBuffs.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.SpecialAttackBuffs[i].BuffType.BuffID == buffID)
				{
					return i;
				}
			}
			return -1;
		}

		public void ApplySpecialAttackBuffs(ref int modifyValue)
		{
			int i = 0;
			int count = this.SpecialAttackBuffs.Count;
			while (i < count)
			{
				Buff buff = this.SpecialAttackBuffs[i];
				buff.ApplyStacks(ref modifyValue, modifyValue);
				i++;
			}
		}

		public void AddAppliedBuff(BuffTypeVO buffVO, BuffVisualPriority visualPriority)
		{
			int num = this.FindSpecialAttackBuff(buffVO.BuffID);
			if (num < 0)
			{
				Buff buff = new Buff(buffVO, ArmorType.FlierVehicle, visualPriority);
				buff.AddStack();
				this.SpecialAttackBuffs.Add(buff);
			}
			else
			{
				Buff buff = this.SpecialAttackBuffs[num];
				if (buffVO.Lvl > buff.BuffType.Lvl)
				{
					buff.UpgradeBuff(buffVO);
				}
				buff.AddStack();
			}
		}

		public void ClearAppliedBuffs()
		{
			this.SpecialAttackBuffs = null;
		}
	}
}
