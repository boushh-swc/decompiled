using StaRTS.Main.Models;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX
{
	public class HUDResourceView
	{
		private UXSlider slider;

		private UXLabel label;

		private Animator animator;

		private string resourceId;

		private bool animSpinningOn;

		private int currentViewAmount;

		private int desiredAmount;

		private int maxAmount;

		private int tickerStartAmount;

		private int tickerDeltaAmount;

		private float tickerDuration;

		private float tickerDt;

		private uint animOffTimerId;

		private int animOffNameHash;

		private const string ANIM_TRIGGER_START = "SpinStart";

		private const string ANIM_TRIGGER_STOP = "SpinStop";

		private const float ANIM_OFF_DELAY = 0.5f;

		public bool NeedsUpdate
		{
			get;
			private set;
		}

		public bool Visible
		{
			set
			{
				if (this.animator != null)
				{
					this.animator.enabled = value;
					this.animator.gameObject.SetActive(value);
				}
			}
		}

		public HUDResourceView(string resourceId, UXSlider slider, UXLabel label, UXElement animatorElement)
		{
			this.resourceId = resourceId;
			this.slider = slider;
			this.label = label;
			if (animatorElement != null)
			{
				this.animator = animatorElement.Root.GetComponent<Animator>();
				if (this.animator != null)
				{
					this.animOffNameHash = this.animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
				}
			}
			this.currentViewAmount = -1;
			this.desiredAmount = -1;
			this.maxAmount = -1;
			this.tickerStartAmount = -1;
			this.tickerDeltaAmount = 0;
			this.tickerDuration = 0f;
			this.tickerDt = 0f;
			this.animOffTimerId = 0u;
			this.animSpinningOn = false;
			this.NeedsUpdate = false;
		}

		public void SetAmount(int desiredAmount, int maxAmount, bool animate)
		{
			if (this.desiredAmount != desiredAmount || this.maxAmount != maxAmount)
			{
				bool flag = this.desiredAmount == -1;
				bool flag2 = desiredAmount > this.desiredAmount;
				this.desiredAmount = desiredAmount;
				this.maxAmount = maxAmount;
				if (animate && !flag && this.currentViewAmount != desiredAmount)
				{
					this.tickerStartAmount = this.currentViewAmount;
					this.tickerDeltaAmount = desiredAmount - this.currentViewAmount;
					int num = maxAmount;
					if (this.resourceId == "crystals")
					{
						num = GameConstants.HUD_RESOURCE_TICKER_CRYSTAL_THRESHOLD;
					}
					if (num > 0)
					{
						this.tickerDuration = Mathf.Clamp(Mathf.Abs((float)this.tickerDeltaAmount) / (float)num, GameConstants.HUD_RESOURCE_TICKER_MIN_DURATION, GameConstants.HUD_RESOURCE_TICKER_MAX_DURATION);
					}
					if (flag2 && !this.animSpinningOn && this.animator != null && this.animator.isActiveAndEnabled)
					{
						this.animator.SetTrigger("SpinStart");
						this.animSpinningOn = true;
						this.KillAnimOffTimer();
					}
					this.NeedsUpdate = true;
				}
				else
				{
					this.tickerStartAmount = desiredAmount;
					this.currentViewAmount = desiredAmount;
					this.SyncUI();
				}
			}
		}

		public void Update(float dt)
		{
			this.tickerDt += dt;
			if (this.tickerDt < this.tickerDuration)
			{
				float f = Easing.SineEaseOut(this.tickerDt, (float)this.tickerStartAmount, (float)this.tickerDeltaAmount, this.tickerDuration);
				this.currentViewAmount = Mathf.FloorToInt(f);
			}
			else
			{
				this.currentViewAmount = this.desiredAmount;
			}
			if (this.currentViewAmount == this.desiredAmount)
			{
				this.tickerDt = 0f;
				this.NeedsUpdate = false;
				if (this.animSpinningOn && this.animator != null && this.animator.isActiveAndEnabled)
				{
					this.animSpinningOn = false;
					this.animator.SetTrigger("SpinStop");
					this.KillAnimOffTimer();
					Service.ViewTimerManager.CreateViewTimer(0.5f, false, new TimerDelegate(this.OnAnimTimerComplete), null);
				}
			}
			this.SyncUI();
		}

		public void TweakAmount()
		{
			int num = this.currentViewAmount;
			this.SetAmount(this.currentViewAmount - 1, this.maxAmount, false);
			this.SetAmount(num, this.maxAmount, false);
		}

		private void OnAnimTimerComplete(uint id, object cookie)
		{
			if (this.animator != null)
			{
				this.animator.Play(this.animOffNameHash);
			}
			this.animOffTimerId = 0u;
		}

		private void KillAnimOffTimer()
		{
			if (this.animOffTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.animOffTimerId);
				this.animOffTimerId = 0u;
			}
		}

		private void SyncUI()
		{
			if (this.slider != null)
			{
				this.slider.Value = ((this.maxAmount > 0) ? ((float)this.currentViewAmount / (float)this.maxAmount) : 0f);
			}
			if (this.label != null && Service.Lang != null)
			{
				Lang lang = Service.Lang;
				string text = lang.ThousandsSeparated(this.currentViewAmount);
				if (this.maxAmount == -1)
				{
					this.label.Text = text;
				}
				else
				{
					string text2 = lang.ThousandsSeparated(this.maxAmount);
					this.label.Text = lang.Get("FRACTION", new object[]
					{
						text,
						text2
					});
				}
			}
		}
	}
}
