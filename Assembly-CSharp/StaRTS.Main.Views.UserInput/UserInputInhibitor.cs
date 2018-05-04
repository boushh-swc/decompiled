using Net.RichardLord.Ash.Core;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UserInput
{
	public class UserInputInhibitor
	{
		private bool deny;

		private HashSet<UXElement> allowableElements;

		private HashSet<Entity> allowableEntities;

		private bool deployAllowed;

		private uint denyTimerId;

		private HashSet<UXElement> globalyAllowableElements;

		public UserInputInhibitor()
		{
			Service.UserInputInhibitor = this;
			this.allowableElements = new HashSet<UXElement>();
			this.allowableEntities = new HashSet<Entity>();
			this.globalyAllowableElements = new HashSet<UXElement>();
		}

		public bool IsAllowable(UXElement queryElement)
		{
			return !this.deny || this.allowableElements.Contains(queryElement);
		}

		public bool IsAllowable(Entity queryEntity)
		{
			return !this.deny || this.allowableEntities.Contains(queryEntity);
		}

		public bool IsDeployAllowed()
		{
			return !this.deny || this.deployAllowed;
		}

		public void AllowOnly(UXElement element)
		{
			this.DenyAll();
			this.allowableElements.Add(element);
		}

		public void AddToAllow(UXElement element)
		{
			if (!this.allowableElements.Contains(element))
			{
				this.allowableElements.Add(element);
			}
		}

		public void AlwaysAllowElement(UXElement element)
		{
			this.AddToAllow(element);
			if (!this.globalyAllowableElements.Contains(element))
			{
				this.globalyAllowableElements.Add(element);
			}
		}

		public void AllowOnly(HashSet<UXElement> elements)
		{
			this.DenyAll();
			this.allowableElements = elements;
		}

		public void AllowOnly(Entity entity)
		{
			this.DenyAll();
			this.allowableEntities.Add(entity);
		}

		public void AllowDeploy()
		{
			this.deployAllowed = true;
		}

		public void DenyAll()
		{
			this.deny = true;
			this.allowableElements.Clear();
			this.allowableEntities.Clear();
			foreach (UXElement current in this.globalyAllowableElements)
			{
				this.allowableElements.Add(current);
			}
			this.deployAllowed = false;
			Service.EventManager.SendEvent(EventId.DenyUserInput, null);
		}

		public void AllowAll()
		{
			this.deny = false;
			Service.EventManager.SendEvent(EventId.AllowUserInput, null);
		}

		public bool IsDenying()
		{
			return this.deny;
		}

		public void DenyAllForSeconds(float seconds)
		{
			this.DenyAll();
			if (this.denyTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.denyTimerId);
			}
			this.denyTimerId = Service.ViewTimerManager.CreateViewTimer(seconds, false, new TimerDelegate(this.OnDenyTimerTimeout), null);
		}

		private void OnDenyTimerTimeout(uint timerId, object cookie)
		{
			this.AllowAll();
		}
	}
}
