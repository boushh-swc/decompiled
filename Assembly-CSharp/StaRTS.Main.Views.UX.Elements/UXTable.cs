using StaRTS.Main.Views.Cameras;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXTable : AbstractUXList
	{
		private UXTableComponent tableComponent;

		private bool autoCenterPositionSet;

		private Vector3 autoCenterPosition;

		public Action RepositionCallback
		{
			get
			{
				return this.tableComponent.RepositionCallback;
			}
			set
			{
				this.tableComponent.RepositionCallback = value;
			}
		}

		public Vector2 Padding
		{
			get
			{
				return this.tableComponent.Padding;
			}
		}

		public bool HideInactive
		{
			get
			{
				return this.tableComponent != null && this.tableComponent.NGUITable != null && this.tableComponent.NGUITable.hideInactive;
			}
			set
			{
				if (this.tableComponent != null && this.tableComponent.NGUITable != null)
				{
					this.tableComponent.NGUITable.hideInactive = value;
				}
			}
		}

		public UXTable(UXFactory uxFactory, UXCamera uxCamera, UXTableComponent component) : base(uxFactory, uxCamera, component)
		{
			this.tableComponent = component;
		}

		public override void InternalDestroyComponent()
		{
			this.tableComponent.Table = null;
			this.tableComponent = null;
			base.InternalDestroyComponent();
		}

		public void HideScrollArrows()
		{
			this.tableComponent.HideScrollArrows();
		}

		public override void ClearWithoutDestroy()
		{
			base.ClearWithoutDestroy();
			if (this.autoCenterPositionSet)
			{
				base.LocalPosition = this.autoCenterPosition;
			}
		}

		public override void Clear()
		{
			base.Clear();
			if (this.autoCenterPositionSet)
			{
				base.LocalPosition = this.autoCenterPosition;
			}
		}

		public void AutoCenter()
		{
			if (base.Count == 0)
			{
				return;
			}
			if (!this.autoCenterPositionSet)
			{
				this.autoCenterPositionSet = true;
				this.autoCenterPosition = base.LocalPosition;
			}
			base.Width = 0f;
			Vector3 localPosition = this.autoCenterPosition;
			localPosition.x -= 0.5f * (UXUtils.CalculateElementSize(this).x + this.Padding.x);
			base.LocalPosition = localPosition;
		}
	}
}
