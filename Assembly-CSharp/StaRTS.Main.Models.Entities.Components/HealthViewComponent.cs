using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Views;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class HealthViewComponent : ComponentBase
	{
		private const float SECONDARY_HEIGHT_OFFSET = 1f;

		private UXSlider slider;

		private UXSlider secondarySlider;

		private TooltipHelper tooltipHelper;

		private TooltipHelper secondaryTooltipHelper;

		private bool secondaryOnly;

		public int HealthAmount
		{
			get;
			private set;
		}

		public int MaxHealthAmount
		{
			get;
			private set;
		}

		public int SecondaryHealthAmount
		{
			get;
			private set;
		}

		public int SecondaryMaxHealthAmount
		{
			get;
			private set;
		}

		public bool IsInitialized
		{
			get;
			private set;
		}

		public bool AutoRegenerating
		{
			get;
			set;
		}

		public bool HasRubble
		{
			get;
			set;
		}

		public HealthViewComponent()
		{
			this.tooltipHelper = new TooltipHelper();
			this.secondaryTooltipHelper = new TooltipHelper();
			this.slider = null;
			this.secondarySlider = null;
			this.IsInitialized = false;
		}

		public void SetupElements()
		{
			this.SetupElements(this.Entity.Get<GameObjectViewComponent>(), true, false);
		}

		public void SetupElements(GameObjectViewComponent view, bool primary, bool secondary)
		{
			if (this.IsInitialized)
			{
				return;
			}
			if (this.HealthAmount >= this.MaxHealthAmount && this.SecondaryHealthAmount >= this.SecondaryMaxHealthAmount)
			{
				this.TeardownElements();
				return;
			}
			this.IsInitialized = true;
			if (primary && TooltipHelper.WouldOverlapAnotherTooltip(view))
			{
				return;
			}
			this.secondaryOnly = (secondary && !primary);
			UXController uXController = Service.UXController;
			string text = "HealthSlider" + view.MainGameObject.GetInstanceID().ToString();
			if (primary)
			{
				this.slider = uXController.MiscElementsManager.CreateHealthSlider(text, uXController.WorldUIParent, false);
				if (this.slider != null)
				{
					this.slider.Value = 1f;
					this.tooltipHelper.SetupElements(view, this.slider, 0f, true, true);
				}
			}
			if (secondary)
			{
				this.secondarySlider = uXController.MiscElementsManager.CreateHealthSlider("s_" + text, uXController.WorldUIParent, true);
				if (this.secondarySlider != null)
				{
					this.secondarySlider.Value = 1f;
					this.secondaryTooltipHelper.SetupElements(view, this.secondarySlider, 1f, true, false);
				}
			}
		}

		public void SetEnabled(bool enable)
		{
			if (!this.IsInitialized)
			{
				return;
			}
			this.tooltipHelper.Enabled = enable;
			this.secondaryTooltipHelper.Enabled = enable;
		}

		public void UpdateLocation()
		{
			if (!this.IsInitialized)
			{
				return;
			}
			if (this.slider != null)
			{
				this.tooltipHelper.Slot = -1;
				if (this.tooltipHelper.UpdateLocation(true))
				{
					this.TeardownElements();
					return;
				}
			}
			if (this.secondarySlider != null)
			{
				this.secondaryTooltipHelper.UpdateLocation(false);
			}
		}

		public void UpdateHealth(int health, int maxHealth, bool secondary)
		{
			if (secondary || this.secondaryOnly)
			{
				this.SecondaryHealthAmount = health;
				this.SecondaryMaxHealthAmount = maxHealth;
				if (this.secondarySlider != null)
				{
					this.secondarySlider.Value = ((maxHealth > 0) ? ((float)health / (float)maxHealth) : 0f);
				}
			}
			else
			{
				this.HealthAmount = health;
				this.MaxHealthAmount = maxHealth;
				if (this.slider != null)
				{
					this.slider.Value = ((maxHealth > 0) ? ((float)health / (float)maxHealth) : 0f);
				}
			}
		}

		public void TeardownElements()
		{
			if (!this.IsInitialized)
			{
				return;
			}
			this.IsInitialized = false;
			MiscElementsManager miscElementsManager = Service.UXController.MiscElementsManager;
			if (this.slider != null)
			{
				miscElementsManager.DestroyHealthSlider(this.slider, false);
				this.slider = null;
			}
			this.tooltipHelper.TeardownElements(true);
			if (this.secondarySlider != null)
			{
				miscElementsManager.DestroyHealthSlider(this.secondarySlider, true);
				this.secondarySlider = null;
			}
			this.secondaryTooltipHelper.TeardownElements(false);
		}

		public override void OnRemove()
		{
			this.RemoveSelf();
		}

		public bool WillFadeOnTimer()
		{
			return this.tooltipHelper.HasFadeTimer() || this.secondaryTooltipHelper.HasFadeTimer();
		}

		public void GoAwayIn(float seconds)
		{
			this.tooltipHelper.GoAwayIn(seconds, new Action(this.RemoveSelf));
			this.secondaryTooltipHelper.GoAwayIn(seconds, new Action(this.RemoveSelf));
		}

		private void RemoveSelf()
		{
			this.TeardownElements();
		}
	}
}
