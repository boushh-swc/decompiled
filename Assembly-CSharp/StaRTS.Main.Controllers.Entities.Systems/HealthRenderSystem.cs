using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Controllers.Entities.Systems
{
	public class HealthRenderSystem : ViewSystemBase
	{
		private EntityController entityController;

		private NodeList<HealthViewNode> nodeList;

		private const float HP_REGENERATION_INTERVAL_TIME = 0.1f;

		private const int HP_REGENERATION_PER_INTERVAL = 80;

		public const float HP_REGENERATION_PERCENT_THRESHOLD_RUBBLE = 0.2f;

		private const float REGENERATION_FINISHED_EVENT_DELAY = 1f;

		private float timeSinceRegenerationUpdate;

		public override void AddToGame(Game game)
		{
			this.entityController = Service.EntityController;
			this.nodeList = this.entityController.GetNodeList<HealthViewNode>();
		}

		public override void RemoveFromGame(Game game)
		{
		}

		public void ForceUpdate()
		{
			this.Update(0.1f);
		}

		protected override void Update(float dt)
		{
			if (Service.BuildingTooltipController != null)
			{
				Service.BuildingTooltipController.ResetTooltipSlots();
			}
			for (HealthViewNode healthViewNode = this.nodeList.Head; healthViewNode != null; healthViewNode = healthViewNode.Next)
			{
				HealthViewComponent healthView = healthViewNode.HealthView;
				if (healthView != null)
				{
					healthView.UpdateLocation();
					if (healthView.AutoRegenerating)
					{
						this.UpdateAutoRegeneration(healthView, dt);
					}
				}
			}
		}

		private void UpdateAutoRegeneration(HealthViewComponent healthView, float dt)
		{
			this.timeSinceRegenerationUpdate += dt;
			if (this.timeSinceRegenerationUpdate >= 0.1f)
			{
				this.timeSinceRegenerationUpdate = 0f;
				int num = healthView.HealthAmount + 80;
				int maxHealthAmount = healthView.MaxHealthAmount;
				if (num > maxHealthAmount)
				{
					num = maxHealthAmount;
				}
				healthView.UpdateHealth(num, maxHealthAmount, false);
				if (num == maxHealthAmount)
				{
					healthView.AutoRegenerating = false;
					healthView.TeardownElements();
					Service.ViewTimerManager.CreateViewTimer(1f, false, new TimerDelegate(this.NotifyAutoRegenerationFinished), healthView);
				}
				this.UpdateRubbleStateFromHealthView(healthView);
			}
		}

		public void UpdateRubbleStateFromHealthView(HealthViewComponent healthView)
		{
			float num = (healthView.MaxHealthAmount != 0) ? ((float)healthView.HealthAmount / (float)healthView.MaxHealthAmount) : 0f;
			bool flag = num < 0.2f;
			if (healthView.HasRubble)
			{
				if (!flag)
				{
					Service.FXManager.RemoveAttachedRubbleFromEntity(healthView.Entity);
					healthView.HasRubble = false;
				}
			}
			else if (flag)
			{
				Service.FXManager.CreateAndAttachRubbleToEntity(healthView.Entity);
				healthView.HasRubble = true;
			}
		}

		private void NotifyAutoRegenerationFinished(uint timerId, object cookie)
		{
			Service.EventManager.SendEvent(EventId.EntityHealthViewRegenerated, cookie);
		}
	}
}
