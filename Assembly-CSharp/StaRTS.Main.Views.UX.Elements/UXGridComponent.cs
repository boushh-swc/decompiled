using StaRTS.Utils.Core;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXGridComponent : AbstractUXListComponent
	{
		private bool manualCullGUIObjects;

		private float cullingFactor = 2f;

		public UIGrid NGUIGrid
		{
			get;
			set;
		}

		public UXGrid Grid
		{
			get;
			set;
		}

		public float CellWidth
		{
			get
			{
				return (!(this.NGUIGrid == null)) ? this.NGUIGrid.cellWidth : 0f;
			}
			set
			{
				if (this.NGUIGrid != null)
				{
					this.NGUIGrid.cellWidth = value;
				}
			}
		}

		public float CellHeight
		{
			get
			{
				return (!(this.NGUIGrid == null)) ? this.NGUIGrid.cellHeight : 0f;
			}
			set
			{
				if (this.NGUIGrid != null)
				{
					this.NGUIGrid.cellHeight = value;
				}
			}
		}

		public int MaxItemsPerLine
		{
			get
			{
				return (!(this.NGUIGrid == null)) ? this.NGUIGrid.maxPerLine : 0;
			}
			set
			{
				if (this.NGUIGrid != null)
				{
					this.NGUIGrid.maxPerLine = value;
				}
			}
		}

		public override void Init()
		{
			base.Init();
			if (this.NGUIGrid != null)
			{
				this.NGUIGrid.sorting = UIGrid.Sorting.Alphabetic;
			}
		}

		public override float GetCurrentScrollPosition(bool softClip)
		{
			if (base.NGUIScrollView == null || this.NGUIGrid == null || base.NGUIScrollView.panel == null)
			{
				return 0f;
			}
			bool flag = this.NGUIGrid.arrangement == UIGrid.Arrangement.Vertical;
			Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(base.transform);
			Vector3[] worldCorners = base.NGUIScrollView.panel.worldCorners;
			float num6;
			if (flag)
			{
				Vector4 finalClipRegion = base.NGUIScrollView.panel.finalClipRegion;
				float num = 0f;
				if (softClip && base.NGUIScrollView.panel.clipping == UIDrawCall.Clipping.SoftClip)
				{
					float num2 = base.NGUIScrollView.panel.clipSoftness.y / finalClipRegion.w;
					num = num2 * (worldCorners[2].y - worldCorners[0].y);
				}
				float num3 = num + bounds.extents.y;
				float num4 = worldCorners[0].y + num3;
				float num5 = worldCorners[2].y - num3;
				float y = bounds.center.y;
				num6 = (y - num4) / (num5 - num4);
				num6 -= 1f;
				num6 *= -1f;
			}
			else
			{
				float num7 = worldCorners[0].x + bounds.extents.x;
				float num8 = worldCorners[2].x - bounds.extents.x;
				float x = bounds.center.x;
				num6 = (x - num7) / (num8 - num7);
			}
			return Mathf.Clamp01(num6);
		}

		public override void RepositionItems(bool delayedReposition)
		{
			bool flag = true;
			if (this.NGUIGrid != null)
			{
				this.NGUIGrid.Reposition();
				if (base.gameObject.activeInHierarchy && delayedReposition)
				{
					flag = false;
					base.StartCoroutine(this.DelayedReposition());
				}
				if (base.NGUIPanel != null && base.NGUIScrollView != null)
				{
					Vector3 position = base.NGUIPanel.gameObject.transform.position;
					Vector4 baseClipRegion = base.NGUIPanel.baseClipRegion;
					base.NGUIScrollView.ResetPosition();
					base.NGUIScrollView.RestrictWithinBounds(true);
					Vector3 position2 = base.NGUIPanel.gameObject.transform.position;
					Vector4 baseClipRegion2 = base.NGUIPanel.baseClipRegion;
					if (this.NGUIGrid.arrangement == UIGrid.Arrangement.Vertical)
					{
						position2.x = position.x;
						baseClipRegion2.x = baseClipRegion.x;
					}
					else
					{
						position2.y = position.y;
						baseClipRegion2.y = baseClipRegion.y;
					}
					base.NGUIPanel.gameObject.transform.position = position2;
					base.NGUIPanel.baseClipRegion = baseClipRegion2;
				}
			}
			this.UpdateScrollArrows();
			if (flag)
			{
				base.OnReposition();
			}
		}

		public override void UpdateScrollArrows()
		{
			if (this.Grid != null && this.Grid.Visible)
			{
				base.UpdateScrollArrows();
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedReposition()
		{
			yield return null;
			this.NGUIGrid.Reposition();
			base.OnReposition();
			yield break;
		}

		public override void Scroll(float location)
		{
			if (this.NGUIGrid != null && base.NGUIScrollView != null)
			{
				float x = 0f;
				float y = 0f;
				if (this.NGUIGrid.arrangement == UIGrid.Arrangement.Vertical)
				{
					y = location;
				}
				else
				{
					x = location;
				}
				base.NGUIScrollView.SetDragAmount(x, y, false);
				base.NGUIScrollView.UpdateScrollbars(true);
				this.UpdateScrollArrows();
			}
		}

		public override Vector3 GetItemDimension()
		{
			if (this.NGUIGrid == null)
			{
				return Vector3.zero;
			}
			return (this.NGUIGrid.arrangement != UIGrid.Arrangement.Vertical) ? (Vector3.right * this.NGUIGrid.cellWidth) : (Vector3.up * this.NGUIGrid.cellHeight);
		}

		protected override void OnDrag()
		{
			if (this.Grid != null)
			{
				this.Grid.InternalOnDrag();
			}
			this.UpdateScrollArrows();
		}

		public void CullScrollObjects(bool enable, float cullFactor)
		{
			this.manualCullGUIObjects = enable;
			if (enable)
			{
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
				base.GetComponentInParent<UIScrollView>().disableDragIfFits = false;
				if (cullFactor < 2f)
				{
					cullFactor = 2f;
				}
				this.cullingFactor = cullFactor;
			}
			else
			{
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
		}

		public override void OnViewFrameTime(float dt)
		{
			base.OnViewFrameTime(dt);
			if (!this.manualCullGUIObjects)
			{
				return;
			}
			if (this.Grid == null)
			{
				return;
			}
			foreach (UXElement current in this.Grid.GetElementList())
			{
				if (current != null)
				{
					Bounds bounds = UXUtils.CalculateAbsoluteWidgetBound(current.GetUIWidget.transform);
					Vector3[] worldCorners = base.NGUIPanel.worldCorners;
					Vector3 vector = bounds.center - bounds.size * this.cullingFactor;
					Vector3 vector2 = bounds.center + bounds.size * this.cullingFactor;
					if (this.NGUIGrid.arrangement == UIGrid.Arrangement.Horizontal)
					{
						if (vector.x > worldCorners[2].x || vector2.x < worldCorners[0].x)
						{
							current.GetUIWidget.gameObject.SetActive(false);
						}
						else
						{
							current.GetUIWidget.gameObject.SetActive(true);
						}
					}
					else if (vector.y > worldCorners[2].y || vector2.y < worldCorners[0].y)
					{
						current.GetUIWidget.gameObject.SetActive(false);
					}
					else
					{
						current.GetUIWidget.gameObject.SetActive(true);
					}
				}
			}
		}

		public void CenterElementsInPanel()
		{
			if (base.gameObject.activeInHierarchy)
			{
				base.StartCoroutine(this.DelayedCenterElementsInPanel());
			}
			else
			{
				this.DoCenterElementsInPanel();
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedCenterElementsInPanel()
		{
			yield return null;
			this.DoCenterElementsInPanel();
			yield break;
		}

		private void DoCenterElementsInPanel()
		{
			UIScrollView component = this.NGUIGrid.transform.parent.GetComponent<UIScrollView>();
			if (component != null)
			{
				component.contentPivot = UIWidget.Pivot.Center;
				component.ResetPosition();
			}
			else
			{
				Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(this.NGUIGrid.transform);
				UIPanel componentInParent = this.NGUIGrid.GetComponentInParent<UIPanel>();
				Vector3[] worldCorners = componentInParent.worldCorners;
				Vector3 zero = Vector3.zero;
				zero.x = bounds.center.x - (worldCorners[0].x + worldCorners[2].x) / 2f;
				zero.y = bounds.center.y - (worldCorners[0].y + worldCorners[2].y) / 2f;
				this.NGUIGrid.transform.position -= zero;
			}
		}
	}
}
