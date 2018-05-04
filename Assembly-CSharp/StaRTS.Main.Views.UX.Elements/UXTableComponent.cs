using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXTableComponent : AbstractUXListComponent
	{
		public UITable NGUITable
		{
			get;
			set;
		}

		public UXTable Table
		{
			get;
			set;
		}

		public Vector2 Padding
		{
			get
			{
				return (!(this.NGUITable == null)) ? this.NGUITable.padding : Vector2.zero;
			}
		}

		public override void Init()
		{
			base.Init();
			if (this.NGUITable != null)
			{
				this.NGUITable.sorting = UITable.Sorting.Alphabetic;
			}
		}

		public override void RepositionItems(bool delayedReposition)
		{
			bool flag = true;
			if (this.NGUITable != null)
			{
				this.NGUITable.Reposition();
				if (base.gameObject.activeInHierarchy && delayedReposition)
				{
					flag = false;
					base.StartCoroutine(this.DelayedReposition());
				}
				if (base.NGUIPanel != null && base.NGUIScrollView != null)
				{
					base.NGUIScrollView.ResetPosition();
					base.NGUIScrollView.RestrictWithinBounds(true);
					Vector3 position = base.NGUIPanel.gameObject.transform.position;
					Vector4 baseClipRegion = base.NGUIPanel.baseClipRegion;
					base.NGUIPanel.gameObject.transform.position = position;
					base.NGUIPanel.baseClipRegion = baseClipRegion;
				}
			}
			this.UpdateScrollArrows();
			if (flag)
			{
				base.OnReposition();
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedReposition()
		{
			yield return null;
			this.NGUITable.Reposition();
			base.OnReposition();
			yield break;
		}

		public override float GetCurrentScrollPosition(bool softClip)
		{
			if (base.NGUIScrollView == null || this.NGUITable == null)
			{
				return 0f;
			}
			Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(base.transform);
			Vector3[] worldCorners = base.NGUIScrollView.panel.worldCorners;
			float num = worldCorners[0].y + bounds.extents.y;
			float num2 = worldCorners[2].y - bounds.extents.y;
			float y = bounds.center.y;
			float num3 = (y - num) / (num2 - num);
			num3 -= 1f;
			num3 *= -1f;
			return Mathf.Clamp01(num3);
		}

		public override void Scroll(float location)
		{
			if (this.NGUITable != null && base.NGUIScrollView != null)
			{
				float x = 0f;
				base.NGUIScrollView.SetDragAmount(x, location, false);
			}
		}

		protected override void OnDrag()
		{
			if (this.Table != null)
			{
				this.Table.InternalOnDrag();
			}
			this.UpdateScrollArrows();
		}
	}
}
