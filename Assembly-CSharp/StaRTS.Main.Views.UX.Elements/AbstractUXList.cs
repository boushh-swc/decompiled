using StaRTS.Main.Views.Cameras;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class AbstractUXList : UXElement, IViewFrameTimeObserver
	{
		public const int ORDER_LIMIT = 100000000;

		protected const string NAME_FORMAT = "{0:D8}_{1}";

		public bool BypassLocalPositionOnAdd;

		public bool DupeOrdersAllowed;

		protected UXFactory uxFactory;

		protected UXElement templateItem;

		protected List<UXElement> addedItems;

		protected List<int> addedItemOrders;

		protected Vector3 itemSize;

		protected AbstractUXListComponent component;

		private int repositionWaitFrames;

		private UXDragDelegate repositionCallback;

		public UXDragDelegate OnDrag
		{
			get;
			set;
		}

		public Vector4 ClipRegion
		{
			get
			{
				return this.component.ClipRegion;
			}
			set
			{
				this.component.ClipRegion = value;
			}
		}

		public int Count
		{
			get
			{
				return this.addedItems.Count;
			}
		}

		public AbstractUXList(UXFactory uxFactory, UXCamera uxCamera, AbstractUXListComponent component) : base(uxCamera, component.gameObject, null)
		{
			this.uxFactory = uxFactory;
			this.component = component;
			this.addedItems = new List<UXElement>();
			this.addedItemOrders = new List<int>();
			this.templateItem = null;
			this.OnDrag = null;
			component.Init();
			this.RepositionItems();
		}

		public override void InternalDestroyComponent()
		{
			this.component.InternalDestroyComponent();
			UnityEngine.Object.Destroy(this.component);
		}

		public List<UXElement> GetElementList()
		{
			return this.addedItems;
		}

		public virtual int AddItem(UXElement item, int order)
		{
			int num = -1;
			int i = 0;
			int count = this.addedItemOrders.Count;
			while (i < count)
			{
				int num2 = this.addedItemOrders[i];
				if (order <= num2)
				{
					if (!this.DupeOrdersAllowed && order == num2)
					{
						Service.Logger.WarnFormat("Item {0} matches order of {1}", new object[]
						{
							item.Root.name,
							this.addedItems[i].Root.name
						});
					}
					num = i;
					this.addedItems.Insert(i, item);
					this.addedItemOrders.Insert(i, order);
					break;
				}
				i++;
			}
			if (num < 0)
			{
				num = this.addedItems.Count;
				this.addedItems.Add(item);
				this.addedItemOrders.Add(order);
			}
			item.Parent = this;
			if (order < 0 || order >= 100000000)
			{
				Service.Logger.Warn("Invalid grid / table item order: " + order);
			}
			string newName = string.Format("{0:D8}_{1}", order, item.Root.name);
			this.uxFactory.RenameElement(item.Root, newName);
			if (!this.BypassLocalPositionOnAdd)
			{
				item.LocalPosition = Vector3.right * ((float)(this.Count - 1) * this.itemSize.x);
			}
			return num;
		}

		public Vector3 SetTemplateItem(string templateItemName)
		{
			this.templateItem = this.uxFactory.GetElement<UXElement>(templateItemName);
			this.templateItem.Visible = true;
			this.itemSize = UXUtils.CalculateElementSize(this.templateItem);
			this.templateItem.Visible = false;
			return this.itemSize;
		}

		public UXElement CloneTemplateItem(string itemUid)
		{
			if (this.templateItem == null)
			{
				Service.Logger.Error("Must SetTemplateItem() before cloning");
				return null;
			}
			return this.CloneItem(itemUid, this.templateItem);
		}

		public UXElement CloneItem(string itemUid, UXElement itemToClone)
		{
			if (itemToClone == null)
			{
				Service.Logger.Error("Must send a valid item to clone");
				return null;
			}
			UXElement uXElement = this.uxFactory.CloneElement<UXElement>(itemToClone, itemUid, base.Root);
			if (uXElement != null)
			{
				uXElement.Parent = null;
			}
			return uXElement;
		}

		public T GetSubElement<T>(string itemUid, string name) where T : UXElement
		{
			string name2 = UXUtils.FormatAppendedName(name, itemUid);
			return this.uxFactory.GetElement<T>(name2);
		}

		public T GetOptionalSubElement<T>(string itemUid, string name) where T : UXElement
		{
			string name2 = UXUtils.FormatAppendedName(name, itemUid);
			return this.uxFactory.GetOptionalElement<T>(name2);
		}

		public void RemoveItem(UXElement item)
		{
			int num = this.addedItems.IndexOf(item);
			if (num < 0)
			{
				return;
			}
			this.addedItems.RemoveAt(num);
			this.addedItemOrders.RemoveAt(num);
			item.Parent = null;
			item.LocalPosition = Vector3.zero;
		}

		public UXElement GetItem(int i)
		{
			return (i >= 0 && i < this.addedItems.Count) ? this.addedItems[i] : null;
		}

		public virtual void ClearWithoutDestroy()
		{
			this.addedItems.Clear();
			this.addedItemOrders.Clear();
		}

		public virtual void Clear()
		{
			int i = 0;
			int count = this.addedItems.Count;
			while (i < count)
			{
				UXElement uXElement = this.addedItems[i];
				uXElement.Root.transform.parent = null;
				this.uxFactory.DestroyElement(uXElement);
				i++;
			}
			this.ClearWithoutDestroy();
		}

		public void ChangeScrollDirection(bool goingDown)
		{
			base.GetUIWidget.pivot = ((!goingDown) ? UIWidget.Pivot.BottomLeft : UIWidget.Pivot.TopLeft);
		}

		public void RepositionItemsFrameDelayed()
		{
			this.RepositionItemsFrameDelayed(null);
		}

		public void RepositionItemsFrameDelayed(UXDragDelegate callback)
		{
			this.RepositionItemsFrameDelayed(callback, 1);
		}

		public void RepositionItemsFrameDelayed(UXDragDelegate callback, int frames)
		{
			this.component.RepositionItems(true);
			this.repositionCallback = callback;
			this.repositionWaitFrames = frames;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		public void RepositionItems()
		{
			this.RepositionItems(true);
		}

		public void RepositionItems(bool delayedReposition)
		{
			if (this.templateItem != null)
			{
				this.templateItem.Visible = false;
			}
			try
			{
				this.component.RepositionItems(delayedReposition);
			}
			catch
			{
				Service.Logger.ErrorFormat("NGUI grid crashed, have artist fix: " + base.Root.name + " In the past, Rebundling seems to fix the issue.", new object[0]);
			}
		}

		public void Scroll(float location)
		{
			this.component.Scroll(location);
		}

		public void InternalOnDrag()
		{
			if (this.OnDrag != null)
			{
				this.OnDrag(this);
			}
		}

		public void OnViewFrameTime(float dt)
		{
			if (--this.repositionWaitFrames == 0)
			{
				if (Service.ViewTimeEngine != null)
				{
					Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
				}
				else if (Service.Logger != null)
				{
					Service.Logger.Error("AbstractUXList.OnViewFrameTime: For some reason ViewTimeEngine is null.");
				}
				this.RepositionItems();
				if (this.repositionCallback != null)
				{
					this.repositionCallback(this);
					this.repositionCallback = null;
				}
			}
		}

		public override void OnDestroyElement()
		{
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			base.OnDestroyElement();
		}
	}
}
