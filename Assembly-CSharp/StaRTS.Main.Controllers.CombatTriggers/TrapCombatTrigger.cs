using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers.TrapConditions;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.CombatTriggers
{
	public class TrapCombatTrigger : ICombatTrigger
	{
		private const string TARGET_TYPE_SELF = "self";

		private const string TARGET_TYPE_INTRUDER = "intruder";

		private List<TrapCondition> triggerConditions;

		private TrapController controller;

		private TrapComponent comp;

		private TransformComponent transform;

		private HealthComponent health;

		public uint AreaRadius;

		public object Owner
		{
			get;
			private set;
		}

		public CombatTriggerType Type
		{
			get;
			private set;
		}

		public uint LastDitchDelayMillis
		{
			get
			{
				return 4294967295u;
			}
		}

		public TrapCombatTrigger(TrapNode node)
		{
			this.controller = Service.TrapController;
			this.comp = node.TrapComp;
			this.transform = node.TransformComp;
			this.health = node.HealthComp;
			this.Owner = node.Entity;
			this.Type = CombatTriggerType.Area;
			this.AreaRadius = 0u;
			this.triggerConditions = node.TrapComp.Type.ParsedTrapConditions;
			int i = 0;
			int count = this.triggerConditions.Count;
			while (i < count)
			{
				if (this.triggerConditions[i] is RadiusTrapCondition)
				{
					this.Type = CombatTriggerType.Area;
					this.AreaRadius = ((RadiusTrapCondition)this.triggerConditions[i]).Radius;
				}
				i++;
			}
		}

		public void Trigger(Entity intruder)
		{
			SmartEntity smartEntity = (SmartEntity)intruder;
			TroopType type = smartEntity.TroopComp.TroopType.Type;
			ArmorType armorType = smartEntity.TroopComp.TroopType.ArmorType;
			if (smartEntity.TeamComp.TeamType != TeamType.Attacker)
			{
				return;
			}
			if (TroopController.IsEntityPhantom(smartEntity))
			{
				return;
			}
			int i = 0;
			int count = this.triggerConditions.Count;
			while (i < count)
			{
				TrapCondition trapCondition = this.triggerConditions[i];
				if (trapCondition is IntruderTypeTrapCondition && ((IntruderTypeTrapCondition)trapCondition).IntruderType != type)
				{
					return;
				}
				if (trapCondition is ArmorNotTrapCondition && ((ArmorNotTrapCondition)trapCondition).IntruderArmorTypes.Contains(armorType))
				{
					return;
				}
				i++;
			}
			int boardX = 0;
			int boardZ = 0;
			string a = this.comp.Type.TargetType.ToLower();
			if (a == "self")
			{
				boardX = this.transform.X;
				boardZ = this.transform.Z;
			}
			else if (a == "intruder")
			{
				boardX = smartEntity.TransformComp.X;
				boardZ = smartEntity.TransformComp.Z;
			}
			this.health.Health = 0;
			this.controller.ExecuteTrap(this.comp, boardX, boardZ);
		}

		public bool IsAlreadyTriggered()
		{
			return this.comp.CurrentState != TrapState.Armed;
		}
	}
}
