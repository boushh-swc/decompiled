using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Entities.Components
{
	public class BuffComponent : ComponentBase, IEventObserver
	{
		private MutableIterator miter;

		private MutableIterator niter;

		private MutableIterator oiter;

		private BuffSleepState sleepState;

		private List<Buff> buffRetryList;

		private EventId buildingLoadedEvent;

		private EventId troopLoadedEvent;

		private VisualReadyDelegate onBuildingVisualReady;

		private VisualReadyDelegate onTroopVisualReady;

		public List<Buff> Buffs
		{
			get;
			private set;
		}

		public BuffComponent()
		{
			this.buildingLoadedEvent = EventId.BuildingViewReady;
			this.troopLoadedEvent = EventId.TroopViewReady;
			this.Buffs = new List<Buff>();
			this.buffRetryList = new List<Buff>();
			this.miter = new MutableIterator();
			this.niter = new MutableIterator();
			this.oiter = new MutableIterator();
			this.sleepState = BuffSleepState.Sleeping;
		}

		public override void OnRemove()
		{
			this.onBuildingVisualReady = null;
			this.onTroopVisualReady = null;
			Service.EventManager.UnregisterObserver(this, this.buildingLoadedEvent);
			Service.EventManager.UnregisterObserver(this, this.troopLoadedEvent);
			this.Die();
		}

		public void Die()
		{
			if (this.sleepState == BuffSleepState.Dead)
			{
				return;
			}
			this.oiter.Init(this.Buffs);
			while (this.oiter.Active())
			{
				this.OnRemovingBuff(this.Buffs[this.oiter.Index]);
				this.oiter.Next();
			}
			this.oiter.Reset();
			this.Buffs.Clear();
			this.sleepState = BuffSleepState.Dead;
			this.miter.Reset();
			this.niter.Reset();
		}

		public bool HasBuff(string buffID)
		{
			int i = 0;
			int count = this.Buffs.Count;
			while (i < count)
			{
				Buff buff = this.Buffs[i];
				if (buff.BuffType.BuffID == buffID && buff.StackSize > 0)
				{
					return true;
				}
				i++;
			}
			return false;
		}

		public Buff AddBuffStack(BuffTypeVO buffType, ArmorType armorType, BuffVisualPriority visualPriority, SmartEntity originator)
		{
			int num = this.FindBuff(buffType.BuffID);
			Buff buff;
			if (num < 0)
			{
				buff = new Buff(buffType, armorType, visualPriority);
				buff.AddStack();
				this.Buffs.Add(buff);
				if (this.sleepState == BuffSleepState.Sleeping)
				{
					this.sleepState = BuffSleepState.Awake;
				}
				if (!buff.BuffData.ContainsKey("originator"))
				{
					buff.BuffData.Add("originator", originator);
				}
				else
				{
					buff.BuffData["originator"] = originator;
				}
				this.SendBuffEvent(EventId.AddedBuff, buff);
			}
			else
			{
				buff = this.Buffs[num];
				if (buffType.Lvl > buff.BuffType.Lvl)
				{
					buff.UpgradeBuff(buffType);
				}
				buff.BuffData["originator"] = originator;
				buff.AddStack();
			}
			this.OnBuffStackAdded();
			return buff;
		}

		private void OnBuffStackAdded()
		{
			this.UpdateBuffs(0u);
		}

		public bool RemoveBuffStack(BuffTypeVO buffType)
		{
			int num = this.FindBuff(buffType.BuffID);
			if (num >= 0)
			{
				Buff buff = this.Buffs[num];
				if (buff.RemoveStack())
				{
					this.RemoveBuffAt(num);
				}
			}
			return this.Buffs.Count == 0;
		}

		public void ApplyActiveBuffs(BuffModify modify, ref int modifyValue, int modifyValueMax)
		{
			if (this.sleepState != BuffSleepState.Awake)
			{
				return;
			}
			int i = 0;
			int count = this.Buffs.Count;
			while (i < count)
			{
				Buff buff = this.Buffs[i];
				if (buff.ProcCount > 0 && buff.BuffType.Modify == modify)
				{
					buff.ApplyStacks(ref modifyValue, modifyValueMax);
				}
				i++;
			}
		}

		public bool IsBuffPrevented(BuffTypeVO buffType)
		{
			BuffSleepState buffSleepState = this.sleepState;
			if (buffSleepState == BuffSleepState.Sleeping)
			{
				return false;
			}
			if (buffSleepState != BuffSleepState.Dead)
			{
				int i = 0;
				int count = this.Buffs.Count;
				while (i < count)
				{
					if (this.Buffs[i].BuffType.PreventTags.Overlaps(buffType.Tags))
					{
						return true;
					}
					i++;
				}
				return false;
			}
			return true;
		}

		public void RemoveBuffsCanceledBy(BuffTypeVO buffType)
		{
			this.niter.Init(this.Buffs);
			while (this.niter.Active())
			{
				int index = this.niter.Index;
				Buff buff = this.Buffs[index];
				if (buff.BuffType.BuffID != buffType.BuffID && buff.BuffType.Tags.Overlaps(buffType.CancelTags))
				{
					this.RemoveBuffAt(index);
				}
				this.niter.Next();
			}
			this.niter.Reset();
		}

		public void UpdateBuffs(uint dt)
		{
			if (this.sleepState != BuffSleepState.Awake)
			{
				return;
			}
			this.miter.Init(this.Buffs);
			while (this.miter.Active())
			{
				int num = this.miter.Index;
				Buff buff = this.Buffs[num];
				bool flag;
				bool flag2;
				buff.UpdateBuff(dt, out flag, out flag2);
				if (flag)
				{
					this.SendBuffEvent(EventId.ProcBuff, buff);
				}
				if (flag2)
				{
					num = this.FindBuff(buff.BuffType.BuffID);
					if (num >= 0)
					{
						this.RemoveBuffAt(num);
					}
				}
				this.miter.Next();
			}
			this.miter.Reset();
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (this.IsBuffReadyForVisualRetry(id, cookie))
			{
				this.SendRecheckEvents();
				Service.EventManager.UnregisterObserver(this, id);
			}
			return EatResponse.NotEaten;
		}

		public void RegisterForVisualReAddOnBuilding(Buff buff)
		{
			this.AddUniqueRetryBuffToList(buff);
			Service.EventManager.RegisterObserver(this, this.buildingLoadedEvent, EventPriority.Default);
		}

		public void RegisterForVisualReAddOnTroop(Buff buff)
		{
			this.AddUniqueRetryBuffToList(buff);
			Service.EventManager.RegisterObserver(this, this.troopLoadedEvent, EventPriority.Default);
		}

		public void SetTroopLoadedEvent(EventId loadedEvent, VisualReadyDelegate trpVisualReady)
		{
			EventManager eventManager = Service.EventManager;
			bool flag = eventManager.IsEventListenerRegistered(this, this.troopLoadedEvent);
			if (flag)
			{
				eventManager.UnregisterObserver(this, this.troopLoadedEvent);
				eventManager.RegisterObserver(this, loadedEvent);
			}
			this.troopLoadedEvent = loadedEvent;
			this.onTroopVisualReady = trpVisualReady;
		}

		public void SetBuildingLoadedEvent(EventId loadedEvent, VisualReadyDelegate bldVisualReady)
		{
			EventManager eventManager = Service.EventManager;
			bool flag = eventManager.IsEventListenerRegistered(this, this.buildingLoadedEvent);
			if (flag)
			{
				eventManager.UnregisterObserver(this, this.buildingLoadedEvent);
				eventManager.RegisterObserver(this, loadedEvent);
			}
			this.buildingLoadedEvent = loadedEvent;
			this.onBuildingVisualReady = bldVisualReady;
		}

		private bool IsBuffReadyForVisualRetry(EventId id, object cookie)
		{
			SmartEntity smartEntity = (SmartEntity)this.Entity;
			bool flag = true;
			if (this.onBuildingVisualReady != null)
			{
				flag = this.onBuildingVisualReady(id, cookie, smartEntity);
			}
			else if (this.onTroopVisualReady != null)
			{
				flag = this.onTroopVisualReady(id, cookie, smartEntity);
			}
			return flag && smartEntity.GameObjectViewComp != null && smartEntity.GameObjectViewComp.MainGameObject != null;
		}

		private void AddUniqueRetryBuffToList(Buff buff)
		{
			if (!this.buffRetryList.Contains(buff))
			{
				this.buffRetryList.Add(buff);
			}
		}

		private void SendRecheckEvents()
		{
			int count = this.buffRetryList.Count;
			for (int i = 0; i < count; i++)
			{
				this.SendBuffEvent(EventId.AddedBuff, this.buffRetryList[i]);
			}
			this.buffRetryList.Clear();
		}

		private void RemoveBuffAt(int index)
		{
			this.OnRemovingBuff(this.Buffs[index]);
			this.Buffs.RemoveAt(index);
			this.miter.OnRemove(index);
			this.niter.OnRemove(index);
			this.oiter.OnRemove(index);
			if (this.Buffs.Count == 0 && this.sleepState == BuffSleepState.Awake)
			{
				this.sleepState = BuffSleepState.Sleeping;
			}
		}

		private void OnRemovingBuff(Buff buff)
		{
			if (this.buffRetryList.Count > 0)
			{
				this.buffRetryList.Remove(buff);
			}
			this.SendBuffEvent(EventId.RemovingBuff, buff);
		}

		private void SendBuffEvent(EventId id, Buff buff)
		{
			SmartEntity target = (SmartEntity)this.Entity;
			BuffEventData cookie = new BuffEventData(buff, target);
			Service.EventManager.SendEvent(id, cookie);
		}

		private int FindBuff(string buffID)
		{
			int i = 0;
			int count = this.Buffs.Count;
			while (i < count)
			{
				if (this.Buffs[i].BuffType.BuffID == buffID)
				{
					return i;
				}
				i++;
			}
			return -1;
		}
	}
}
