using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers
{
	public class ScreenParticleHandler : IEventObserver
	{
		private List<ScreenParticleFXCookie> particleFxes;

		private ScreenBase screen;

		private bool canShowParticles = true;

		public ScreenParticleHandler(ScreenBase screenBase)
		{
			this.particleFxes = new List<ScreenParticleFXCookie>();
			this.screen = screenBase;
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.ScreenLoaded, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ScreenClosing, EventPriority.Default);
		}

		public void Destroy()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.ScreenLoaded);
			eventManager.UnregisterObserver(this, EventId.ScreenClosing);
			this.CancelScheduledParticleFXes();
			this.HideAllParticleElements();
			if (this.particleFxes != null)
			{
				int i = 0;
				int count = this.particleFxes.Count;
				while (i < count)
				{
					this.particleFxes[i].Destroy();
					i++;
				}
				this.particleFxes.Clear();
				this.particleFxes = null;
			}
			this.screen = null;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			ScreenBase screenBase = (ScreenBase)cookie;
			if (id != EventId.ScreenLoaded)
			{
				if (id == EventId.ScreenClosing)
				{
					screenBase = (ScreenBase)cookie;
					if (this.screen != screenBase && !Service.ScreenController.IsFatalAlertActive())
					{
						this.canShowParticles = true;
						this.ShowAllParticleElements();
					}
				}
			}
			else
			{
				screenBase = (ScreenBase)cookie;
				if (screenBase != this.screen)
				{
					this.canShowParticles = false;
					this.HideAllParticleElements();
				}
			}
			return EatResponse.NotEaten;
		}

		public void ScheduleParticleFX(ScreenParticleFXCookie cookie)
		{
			if (cookie == null)
			{
				Service.Logger.WarnFormat("Null cookie passed, cannot schedule particle fx", new object[0]);
				return;
			}
			if (string.IsNullOrEmpty(cookie.ElementName))
			{
				Service.Logger.ErrorFormat("ScreenParticleHandler.ScheduleParticleFX: particle fx elementName is null", new object[0]);
				return;
			}
			this.particleFxes.Add(cookie);
			cookie.DelayTimer = Service.ViewTimerManager.CreateViewTimer(cookie.Delay, false, new TimerDelegate(this.OnParticleTimerDone), cookie);
		}

		private void OnParticleTimerDone(uint timerId, object cookie)
		{
			if (cookie == null || this.screen == null)
			{
				return;
			}
			ScreenParticleFXCookie screenParticleFXCookie = (ScreenParticleFXCookie)cookie;
			screenParticleFXCookie.Element = this.screen.GetOptionalElement<UXElement>(screenParticleFXCookie.ElementName);
			if (screenParticleFXCookie.Element == null)
			{
				Service.Logger.ErrorFormat("ScreenParticleHandler.OnParticleTimerDone: Could not find fx:{0} id in UI", new object[]
				{
					screenParticleFXCookie.ElementName
				});
				return;
			}
			this.ShowParticleElement(screenParticleFXCookie);
		}

		public void ShowAllParticleElements()
		{
			if (!this.canShowParticles)
			{
				return;
			}
			if (this.particleFxes == null || this.particleFxes.Count == 0)
			{
				return;
			}
			int i = 0;
			int count = this.particleFxes.Count;
			while (i < count)
			{
				this.ShowParticleElement(this.particleFxes[i]);
				i++;
			}
		}

		public void ShowParticleElement(ScreenParticleFXCookie cookie)
		{
			if (this.canShowParticles)
			{
				this.SetParticleFXVisibility(cookie, true);
			}
		}

		public void HideAllParticleElements()
		{
			if (this.particleFxes == null || this.particleFxes.Count == 0)
			{
				return;
			}
			int i = 0;
			int count = this.particleFxes.Count;
			while (i < count)
			{
				this.HideParticleElement(this.particleFxes[i]);
				i++;
			}
		}

		public void HideParticleElement(ScreenParticleFXCookie cookie)
		{
			if (cookie == null)
			{
				return;
			}
			this.SetParticleFXVisibility(cookie, false);
		}

		private void SetParticleFXVisibility(ScreenParticleFXCookie cookie, bool isVisible)
		{
			if (cookie.Element == null)
			{
				return;
			}
			cookie.Element.Visible = isVisible;
		}

		private void CancelScheduledParticleFXes()
		{
			if (this.particleFxes == null || this.particleFxes.Count == 0)
			{
				return;
			}
			int i = 0;
			int count = this.particleFxes.Count;
			while (i < count)
			{
				uint delayTimer = this.particleFxes[i].DelayTimer;
				if (delayTimer != 0u)
				{
					Service.ViewTimerManager.KillViewTimer(delayTimer);
					this.particleFxes[i].DelayTimer = 0u;
				}
				i++;
			}
		}
	}
}
