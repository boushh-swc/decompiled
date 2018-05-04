using StaRTS.Main.Views.Cameras;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXGrid : AbstractUXList
	{
		public delegate void OnCentered(UXElement element, int index);

		private UXGrid.OnCentered onCentered;

		private UXGrid.OnCentered onCenteredFinished;

		private UXGridComponent gridComponent;

		private Comparison<UXElement> comparisonCallback;

		public Action RepositionCallback
		{
			get
			{
				return this.gridComponent.RepositionCallback;
			}
			set
			{
				this.gridComponent.RepositionCallback = value;
			}
		}

		public float CellWidth
		{
			get
			{
				return this.gridComponent.CellWidth * this.uxCamera.Scale;
			}
			set
			{
				this.gridComponent.CellWidth = value / this.uxCamera.Scale;
			}
		}

		public float CellHeight
		{
			get
			{
				return this.gridComponent.CellHeight * this.uxCamera.Scale;
			}
			set
			{
				this.gridComponent.CellHeight = value / this.uxCamera.Scale;
			}
		}

		public int MaxItemsPerLine
		{
			get
			{
				return this.gridComponent.MaxItemsPerLine;
			}
			set
			{
				this.gridComponent.MaxItemsPerLine = value;
			}
		}

		public bool IsScrollable
		{
			get
			{
				if (this.gridComponent.transform.parent != null)
				{
					UIScrollView component = this.gridComponent.transform.parent.GetComponent<UIScrollView>();
					return component != null && component.enabled;
				}
				return false;
			}
			set
			{
				if (this.gridComponent.transform.parent != null)
				{
					UIScrollView component = this.gridComponent.transform.parent.GetComponent<UIScrollView>();
					if (component != null)
					{
						component.enabled = value;
					}
				}
			}
		}

		public UXGrid(UXFactory uxFactory, UXCamera uxCamera, UXGridComponent component) : base(uxFactory, uxCamera, component)
		{
			this.gridComponent = component;
		}

		public override void InternalDestroyComponent()
		{
			this.gridComponent.Grid = null;
			this.gridComponent = null;
			base.InternalDestroyComponent();
		}

		public float GetCurrentScrollPosition(bool softClip)
		{
			return this.gridComponent.GetCurrentScrollPosition(softClip);
		}

		public void HideScrollArrows()
		{
			this.gridComponent.HideScrollArrows();
		}

		public void UpdateScrollArrows()
		{
			this.gridComponent.UpdateScrollArrows();
		}

		public bool IsGridComponentScrollable()
		{
			return this.gridComponent.IsScrollable();
		}

		public void SetAnimateSmoothly(bool value)
		{
			this.gridComponent.NGUIGrid.animateSmoothly = value;
		}

		public Transform GetParent()
		{
			return this.gridComponent.transform.parent;
		}

		public void ScrollToItem(int i)
		{
			int num = this.addedItems.Count;
			int maxItemsPerLine = this.MaxItemsPerLine;
			if (maxItemsPerLine > 0)
			{
				i %= maxItemsPerLine;
				if (num > maxItemsPerLine)
				{
					num = maxItemsPerLine;
				}
			}
			float location;
			if (i < 0 || num <= 1)
			{
				location = 0f;
			}
			else if (i >= num)
			{
				location = 1f;
			}
			else
			{
				location = (float)i / (float)(num - 1);
			}
			base.Scroll(location);
		}

		public void SmoothScrollToItem(int i)
		{
			if (!this.EnsureCenterOnChild())
			{
				return;
			}
			if (0 <= i && i < this.addedItems.Count)
			{
				this.gridComponent.NGUICenterOnChild.CenterOn(this.addedItems[i].GetUIWidget.transform);
			}
			else
			{
				Service.Logger.Warn("SmoothScrollToItem invalid Index:" + i);
			}
		}

		public void CullScrollObjects(bool enable, float cullFactor)
		{
			this.gridComponent.CullScrollObjects(enable, cullFactor);
		}

		public void CenterElementsInPanel()
		{
			this.gridComponent.CenterElementsInPanel();
		}

		public UXElement ScrollToNextElement()
		{
			if (!this.EnsureCenterOnChild())
			{
				return null;
			}
			GameObject centeredObject = this.gridComponent.NGUICenterOnChild.centeredObject;
			int num = 0;
			for (int i = 0; i < this.addedItems.Count; i++)
			{
				if (this.addedItems[i].Root == centeredObject)
				{
					num = i;
				}
			}
			num++;
			if (num >= this.addedItems.Count)
			{
				num = 0;
			}
			this.SmoothScrollToItem(num);
			return this.addedItems[num];
		}

		public void SetCenteredCallback(UXGrid.OnCentered onCentered)
		{
			if (!this.EnsureCenterOnChild())
			{
				return;
			}
			this.onCentered = onCentered;
			this.gridComponent.NGUICenterOnChild.onCenter = new UICenterOnChild.OnCenterCallback(this.OnCenteredCallback);
		}

		private void OnCenteredCallback(GameObject centeredObject)
		{
			for (int i = 0; i < this.addedItems.Count; i++)
			{
				if (this.addedItems[i].Root == centeredObject)
				{
					this.onCentered(this.addedItems[i], i);
					return;
				}
			}
		}

		public void SetCenteredFinishedCallback(UXGrid.OnCentered onCenteredFinished)
		{
			if (!this.EnsureCenterOnChild())
			{
				return;
			}
			this.onCenteredFinished = onCenteredFinished;
			this.gridComponent.NGUICenterOnChild.onFinished = new SpringPanel.OnFinished(this.OnCenteredFinishedCallback);
		}

		private void OnCenteredFinishedCallback()
		{
			GameObject centeredObject = this.gridComponent.NGUICenterOnChild.centeredObject;
			for (int i = 0; i < this.addedItems.Count; i++)
			{
				if (this.addedItems[i].Root == centeredObject)
				{
					this.onCenteredFinished(this.addedItems[i], i);
					return;
				}
			}
		}

		public UXElement GetCenteredElement()
		{
			if (!this.EnsureCenterOnChild())
			{
				return null;
			}
			GameObject centeredObject = this.gridComponent.NGUICenterOnChild.centeredObject;
			int i = 0;
			int count = this.addedItems.Count;
			while (i < count)
			{
				if (this.addedItems[i].Root == centeredObject)
				{
					return this.addedItems[i];
				}
				i++;
			}
			if (this.addedItems.Count > 0)
			{
				return this.addedItems[0];
			}
			return null;
		}

		public int GetCenteredElementIndex()
		{
			if (!this.EnsureCenterOnChild())
			{
				return 0;
			}
			GameObject centeredObject = this.gridComponent.NGUICenterOnChild.centeredObject;
			for (int i = 0; i < this.addedItems.Count; i++)
			{
				if (this.addedItems[i].Root == centeredObject)
				{
					return i;
				}
			}
			return 0;
		}

		public override void OnDestroyElement()
		{
			if (this.gridComponent != null)
			{
				if (this.gridComponent.NGUICenterOnChild != null)
				{
					this.gridComponent.NGUICenterOnChild.onFinished = null;
				}
				if (this.gridComponent.NGUIGrid != null)
				{
					this.gridComponent.NGUIGrid.onCustomSort = null;
				}
			}
			this.onCentered = null;
			this.comparisonCallback = null;
			base.OnDestroyElement();
		}

		public bool EnsureCenterOnChild()
		{
			if (this.gridComponent == null)
			{
				Service.Logger.Warn("Missing gridComponent: " + this.gridComponent.gameObject.name);
				return false;
			}
			if (this.gridComponent.NGUICenterOnChild == null)
			{
				Service.Logger.Warn("Missing GUICenterOnChild: " + this.gridComponent.gameObject.name);
				return false;
			}
			return true;
		}

		private bool IsArrangementCellSnap()
		{
			return this.gridComponent.NGUIGrid.arrangement == UIGrid.Arrangement.CellSnap;
		}

		private bool IsArrangementHorizontal()
		{
			return this.gridComponent.NGUIGrid.arrangement == UIGrid.Arrangement.Horizontal;
		}

		private bool IsArrangementVertical()
		{
			return this.gridComponent.NGUIGrid.arrangement == UIGrid.Arrangement.Vertical;
		}

		public void SetSortComparisonCallback(Comparison<UXElement> callback)
		{
			this.comparisonCallback = callback;
			this.gridComponent.NGUIGrid.onCustomSort = new Comparison<Transform>(this.NGUIOnCustomSortCallbacK);
		}

		public int NGUIOnCustomSortCallbacK(Transform transformA, Transform transformB)
		{
			UXElement uXElement = this.FindUXElementForTransform(transformA);
			UXElement uXElement2 = this.FindUXElementForTransform(transformB);
			if (uXElement == null || uXElement2 == null)
			{
				Service.Logger.Warn("Missing NGUIOnCustomSortCallbacK UXElement Reference");
				return 0;
			}
			return this.comparisonCallback(uXElement, uXElement2);
		}

		private UXElement FindUXElementForTransform(Transform transform)
		{
			int i = 0;
			int count = this.addedItems.Count;
			while (i < count)
			{
				if (this.addedItems[i].Root.transform == transform)
				{
					return this.addedItems[i];
				}
				i++;
			}
			return null;
		}

		public void SetSortModeAlphebetical()
		{
			this.gridComponent.NGUIGrid.sorting = UIGrid.Sorting.Alphabetic;
		}

		public void SetSortModeCustom()
		{
			this.gridComponent.NGUIGrid.sorting = UIGrid.Sorting.Custom;
		}

		public void SetSortModeHorizontal()
		{
			this.gridComponent.NGUIGrid.sorting = UIGrid.Sorting.Horizontal;
		}

		public void SetSortModeNone()
		{
			this.gridComponent.NGUIGrid.sorting = UIGrid.Sorting.None;
		}

		public void SetSortModeVertical()
		{
			this.gridComponent.NGUIGrid.sorting = UIGrid.Sorting.Vertical;
		}

		public int GetSortedIndex(UXElement element)
		{
			return this.gridComponent.NGUIGrid.GetIndex(element.Root.transform);
		}

		public void RepositionElement(UXElement element)
		{
			Vector3 localPosition = element.LocalPosition;
			float z = localPosition.z;
			int num = this.addedItems.IndexOf(element);
			if (num < 0)
			{
				Service.Logger.Warn("Add element to grid before positioning: " + element.Root.name);
				return;
			}
			if (this.IsArrangementCellSnap())
			{
				if (this.CellWidth > 0f)
				{
					localPosition.x = Mathf.Round(localPosition.x / this.CellWidth) * this.CellWidth;
				}
				if (this.CellHeight > 0f)
				{
					localPosition.y = Mathf.Round(localPosition.y / this.CellHeight) * this.CellHeight;
				}
			}
			else if (this.IsArrangementHorizontal())
			{
				localPosition = new Vector3(this.CellWidth * (float)num, 0f, z);
			}
			else
			{
				localPosition = new Vector3(0f, -this.CellHeight * (float)num, z);
			}
			element.LocalPosition = localPosition;
		}

		public void ResetScrollViewPosition()
		{
			this.component.ResetScrollViewPosition();
		}
	}
}
