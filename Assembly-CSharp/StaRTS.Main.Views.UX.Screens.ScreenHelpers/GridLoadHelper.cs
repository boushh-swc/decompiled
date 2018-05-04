using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers
{
	public class GridLoadHelper : IViewFrameTimeObserver
	{
		public delegate UXElement CreateUXElementFromGridItem(object itemObject, object cookie, int position);

		public delegate void AddItemsOverTimeFinished(object cookie);

		private const int ELEMENTS_ADDED_PER_TICK = 2;

		private const int ELEMENT_OVERTIME_ADD_FREQUENCY = 3;

		protected UXGrid grid;

		protected GridLoadHelper.CreateUXElementFromGridItem itemToUXElementCreator;

		protected GridLoadHelper.AddItemsOverTimeFinished overTimeAddItemsFinished;

		protected ArrayList overTimeItemAdditionList;

		private int overTimeItemAdditionTimeCount;

		private object cookie;

		public bool IsBusyAddingItems
		{
			get;
			private set;
		}

		public bool IsAddedItems
		{
			get;
			private set;
		}

		public GridLoadHelper(GridLoadHelper.CreateUXElementFromGridItem itemToUXElementCreator, UXGrid grid)
		{
			this.itemToUXElementCreator = itemToUXElementCreator;
			this.grid = grid;
			this.ResetGrid(false);
		}

		public UXGrid GetGrid()
		{
			return this.grid;
		}

		public void AddElement(UXElement element)
		{
			this.grid.AddItem(element, this.grid.Count);
		}

		public void RemoveElement(UXElement element)
		{
			this.grid.RemoveItem(element);
		}

		public int GetNexElementPosition()
		{
			return this.grid.Count;
		}

		private void ClearGrid()
		{
			if (this.grid != null)
			{
				this.grid.Clear();
			}
		}

		public object GetCookie()
		{
			return this.cookie;
		}

		private void InvalidateExistingItems()
		{
			this.IsAddedItems = false;
			this.cookie = null;
		}

		private void ResetGrid(bool unregisterObserver)
		{
			if (unregisterObserver && this.IsBusyAddingItems)
			{
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
			this.ClearGrid();
			this.InvalidateExistingItems();
			this.ResetAfterOverTimeAdditionCompleted();
		}

		public void ResetGrid()
		{
			this.ResetGrid(true);
		}

		private void ResetAfterOverTimeAdditionCompleted()
		{
			this.IsBusyAddingItems = false;
			this.overTimeItemAdditionList = null;
			this.overTimeItemAdditionTimeCount = 3;
			this.overTimeAddItemsFinished = null;
		}

		private void OverTimeAdditionCompleted()
		{
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			if (this.overTimeAddItemsFinished != null)
			{
				this.overTimeAddItemsFinished(this.cookie);
			}
			this.ResetAfterOverTimeAdditionCompleted();
			this.IsAddedItems = true;
		}

		public void ExpediteAddingItems()
		{
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			this.AddItems<object>(ListUtils.ConvertArrayList<object>(this.overTimeItemAdditionList), this.cookie);
			this.ResetAfterOverTimeAdditionCompleted();
		}

		public void StartAddingItemOverTime<T>(List<T> items, object cookie, GridLoadHelper.AddItemsOverTimeFinished addItemsOverTimeFinished)
		{
			this.IsBusyAddingItems = true;
			this.overTimeItemAdditionTimeCount = 3;
			this.overTimeItemAdditionList = new ArrayList(items);
			this.cookie = cookie;
			this.overTimeAddItemsFinished = addItemsOverTimeFinished;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			this.AddItemsInGroup();
		}

		public void AddItems<T>(List<T> items, object cookie)
		{
			for (int i = 0; i < items.Count; i++)
			{
				this.AddItem<T>(items[i], cookie);
			}
			this.grid.RepositionItemsFrameDelayed();
			this.cookie = cookie;
		}

		private void AddItem<T>(T item, object cookie)
		{
			UXElement uXElement = this.itemToUXElementCreator(item, cookie, this.GetNexElementPosition());
			if (uXElement != null)
			{
				this.AddElement(uXElement);
				this.grid.RepositionElement(uXElement);
			}
		}

		private void AddItemsInGroup()
		{
			for (int i = 0; i < 2; i++)
			{
				if (this.overTimeItemAdditionList == null || this.overTimeItemAdditionList.Count <= 0)
				{
					this.OverTimeAdditionCompleted();
					break;
				}
				this.AddItem<object>(this.overTimeItemAdditionList[0], this.cookie);
				this.overTimeItemAdditionList.RemoveAt(0);
			}
		}

		public void OnViewFrameTime(float dt)
		{
			if (--this.overTimeItemAdditionTimeCount == 0)
			{
				this.overTimeItemAdditionTimeCount = 3;
				this.AddItemsInGroup();
			}
		}

		public void OnDestroyElement()
		{
			if (this.IsBusyAddingItems)
			{
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
		}
	}
}
