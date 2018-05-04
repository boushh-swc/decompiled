using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.Projectors;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Controls
{
	public class DeployableTroopControl : IEventObserver, IViewFrameTimeObserver
	{
		private UXSprite cardDimmer;

		private UXElement glowElement;

		private UXElement abilityActivateFxSprite;

		private GeometryProjector geometry;

		private bool observingHeroEvents;

		private bool observingTroopEquipmentEvents;

		private bool observingStarshipEvents;

		private float coolDownTime;

		private float totalCoolDownTime;

		private UXElement EquipmentFX;

		public bool Enabled
		{
			get;
			private set;
		}

		public uint HeroEntityID
		{
			get;
			private set;
		}

		public string DeployableUid
		{
			get;
			private set;
		}

		public bool IsHero
		{
			get;
			private set;
		}

		public bool IsStarship
		{
			get;
			private set;
		}

		public UXCheckbox TroopCheckbox
		{
			get;
			private set;
		}

		public UXLabel TroopCountLabel
		{
			get;
			private set;
		}

		public HeroAbilityState AbilityState
		{
			get;
			private set;
		}

		public bool DisableDueToBuildingDestruction
		{
			get;
			set;
		}

		public DeployableTroopControl(UXCheckbox troopCheckbox, UXLabel troopCountLabel, UXSprite dimmerSprite, UXElement optionalGlowSprite, UXElement optionalActivateFxSprite, GeometryProjector optionalGeometry, bool isHero, bool isStarship, string uid, UXElement equipmentFX)
		{
			this.IsHero = isHero;
			this.IsStarship = isStarship;
			this.DeployableUid = uid;
			this.TroopCheckbox = troopCheckbox;
			this.TroopCountLabel = troopCountLabel;
			this.AbilityState = HeroAbilityState.Dormant;
			this.cardDimmer = dimmerSprite;
			this.glowElement = optionalGlowSprite;
			this.abilityActivateFxSprite = optionalActivateFxSprite;
			this.geometry = optionalGeometry;
			this.EquipmentFX = equipmentFX;
			if (this.abilityActivateFxSprite != null)
			{
				this.abilityActivateFxSprite.Visible = false;
			}
			this.Enable();
			if (this.EquipmentFX == null)
			{
				return;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			if (this.IsEquipmentActiveOnDeployable(staticDataController, this.DeployableUid))
			{
				EventManager eventManager = Service.EventManager;
				eventManager.RegisterObserver(this, EventId.WorldInTransitionComplete);
				eventManager.RegisterObserver(this, EventId.HoloCommScreenDestroyed, EventPriority.AfterDefault);
				this.observingStarshipEvents = this.IsStarship;
				this.observingTroopEquipmentEvents = !this.IsStarship;
			}
			if (this.IsHero)
			{
				EventManager eventManager2 = Service.EventManager;
				string ability = staticDataController.Get<TroopTypeVO>(this.DeployableUid).Ability;
				if (!string.IsNullOrEmpty(ability))
				{
					TroopAbilityVO troopAbilityVO = staticDataController.Get<TroopAbilityVO>(ability);
					if (!troopAbilityVO.Auto)
					{
						this.totalCoolDownTime = (troopAbilityVO.CoolDownTime + troopAbilityVO.Duration) * 0.001f;
						eventManager2.RegisterObserver(this, EventId.HeroDeployed);
						eventManager2.RegisterObserver(this, EventId.HeroKilled);
						eventManager2.RegisterObserver(this, EventId.TroopAbilityDeactivate);
						eventManager2.RegisterObserver(this, EventId.TroopAbilityCoolDownComplete);
						this.observingHeroEvents = true;
					}
				}
			}
		}

		private bool IsEquipmentActiveOnDeployable(StaticDataController sdc, string deployableUid)
		{
			CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
			List<string> list = (currentBattle.Type != BattleType.PveDefend) ? currentBattle.AttackerEquipment : currentBattle.DefenderEquipment;
			if (list == null)
			{
				return false;
			}
			if (deployableUid.Equals("squadTroops"))
			{
				return false;
			}
			string b = (!this.IsStarship) ? sdc.Get<TroopTypeVO>(deployableUid).TroopID : sdc.Get<SpecialAttackTypeVO>(deployableUid).SpecialAttackID;
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				EquipmentVO equipmentVO = sdc.Get<EquipmentVO>(list[i]);
				for (int j = 0; j < equipmentVO.EffectUids.Length; j++)
				{
					EquipmentEffectVO equipmentEffectVO = sdc.Get<EquipmentEffectVO>(equipmentVO.EffectUids[j]);
					string[] array = (!this.IsStarship) ? equipmentEffectVO.AffectedTroopIds : equipmentEffectVO.AffectedSpecialAttackIds;
					if (array != null)
					{
						for (int k = 0; k < array.Length; k++)
						{
							if (array[k] == b)
							{
								return true;
							}
						}
					}
				}
				i++;
			}
			return false;
		}

		private void PlayActiveEquipmentAnimation()
		{
			if (this.EquipmentFX != null)
			{
				this.EquipmentFX.Visible = true;
				this.EquipmentFX.Root.GetComponent<Animation>().Play();
			}
		}

		private void StopActiveEquipmentAnimation()
		{
			if (this.EquipmentFX != null)
			{
				this.EquipmentFX.Visible = false;
				this.EquipmentFX.Root.GetComponent<Animation>().Stop();
			}
		}

		public void StopObserving()
		{
			if (this.IsHero)
			{
				this.AbilityState = HeroAbilityState.Dormant;
				this.StopCoolDown();
				if (this.observingHeroEvents)
				{
					EventManager eventManager = Service.EventManager;
					eventManager.UnregisterObserver(this, EventId.HeroDeployed);
					eventManager.UnregisterObserver(this, EventId.HeroKilled);
					eventManager.UnregisterObserver(this, EventId.TroopAbilityDeactivate);
					eventManager.UnregisterObserver(this, EventId.TroopAbilityCoolDownComplete);
					this.observingHeroEvents = false;
				}
			}
			if (this.observingTroopEquipmentEvents)
			{
				EventManager eventManager2 = Service.EventManager;
				eventManager2.UnregisterObserver(this, EventId.WorldInTransitionComplete);
				eventManager2.UnregisterObserver(this, EventId.HoloCommScreenDestroyed);
				this.observingTroopEquipmentEvents = false;
			}
			else if (this.observingStarshipEvents)
			{
				EventManager eventManager3 = Service.EventManager;
				eventManager3.UnregisterObserver(this, EventId.WorldInTransitionComplete);
				eventManager3.UnregisterObserver(this, EventId.HoloCommScreenDestroyed);
				this.observingStarshipEvents = false;
			}
		}

		public void Enable()
		{
			this.Enabled = true;
			this.cardDimmer.Visible = false;
			this.TroopCountLabel.TextColor = UXUtils.GetCostColor(this.TroopCountLabel, true, false);
		}

		public void Disable()
		{
			this.Disable(true);
			StaticDataController staticDataController = Service.StaticDataController;
			if (this.IsEquipmentActiveOnDeployable(staticDataController, this.DeployableUid))
			{
				this.StopActiveEquipmentAnimation();
			}
		}

		public void Disable(bool clearTroopCountLabel)
		{
			this.Enabled = false;
			this.cardDimmer.Visible = true;
			this.cardDimmer.FillAmount = 1f;
			if (this.glowElement != null)
			{
				this.glowElement.Visible = false;
			}
			if (this.abilityActivateFxSprite != null)
			{
				this.abilityActivateFxSprite.Visible = false;
			}
			this.TroopCheckbox.Selected = false;
			this.TroopCountLabel.TextColor = UXUtils.GetCostColor(this.TroopCountLabel, false, false);
			if (clearTroopCountLabel)
			{
				this.TroopCountLabel.Text = string.Empty;
			}
			if (this.IsHero && this.AbilityState == HeroAbilityState.InUse)
			{
				this.StopCoolDown();
			}
		}

		public void PrepareHeroAbility()
		{
			this.Enable();
			if (this.glowElement != null)
			{
				this.glowElement.Visible = true;
			}
			if (this.abilityActivateFxSprite != null)
			{
				this.abilityActivateFxSprite.Visible = true;
				this.abilityActivateFxSprite.Root.GetComponent<Animation>().Play();
			}
			if (this.geometry != null)
			{
				this.geometry.Config.AnimState = AnimState.AbilityPose;
				this.geometry.Renderer.Render(this.geometry.Config);
			}
			this.AbilityState = HeroAbilityState.Prepared;
		}

		public void UseHeroAbility()
		{
			this.Disable();
			this.AbilityState = HeroAbilityState.InUse;
			this.coolDownTime = this.totalCoolDownTime;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		private void PutHeroAbilityOnCoolDown()
		{
			this.AbilityState = HeroAbilityState.CoolingDown;
		}

		private void StopCoolDown()
		{
			this.coolDownTime = 0f;
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
		}

		public void OnViewFrameTime(float dt)
		{
			this.coolDownTime -= dt;
			if (this.coolDownTime < 0f)
			{
				this.StopCoolDown();
				if (this.abilityActivateFxSprite != null)
				{
					this.abilityActivateFxSprite.Visible = true;
					this.abilityActivateFxSprite.Root.GetComponent<Animation>().Play();
				}
			}
			this.cardDimmer.FillAmount = this.coolDownTime / this.totalCoolDownTime;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			BattleController battleController = Service.BattleController;
			CurrentBattle currentBattle = battleController.GetCurrentBattle();
			bool flag = currentBattle != null && currentBattle.Canceled;
			if ((id == EventId.WorldInTransitionComplete || id == EventId.HoloCommScreenDestroyed) && flag)
			{
				this.PlayActiveEquipmentAnimation();
				return EatResponse.NotEaten;
			}
			if (cookie == null)
			{
				return EatResponse.NotEaten;
			}
			SmartEntity smartEntity = (SmartEntity)cookie;
			if (smartEntity == null || smartEntity.TroopComp == null)
			{
				return EatResponse.NotEaten;
			}
			string uid = smartEntity.TroopComp.TroopType.Uid;
			if (uid != this.DeployableUid)
			{
				return EatResponse.NotEaten;
			}
			switch (id)
			{
			case EventId.HeroDeployed:
				this.HeroEntityID = smartEntity.ID;
				break;
			case EventId.TroopAbilityDeactivate:
				this.PutHeroAbilityOnCoolDown();
				break;
			case EventId.TroopAbilityCoolDownComplete:
				this.StopCoolDown();
				this.PrepareHeroAbility();
				break;
			case EventId.HeroKilled:
				this.Disable();
				this.StopObserving();
				break;
			}
			return EatResponse.NotEaten;
		}
	}
}
