using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.UserInput;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXButton : UXElement
	{
		private Color defaultButtonColor = new Color(255f, 255f, 255f, 1f);

		private UXButtonComponent component;

		public UIButton NGUIButton;

		private UserInputInhibitor inhibitor;

		public UXButtonClickedDelegate OnClicked
		{
			get;
			set;
		}

		public UXButtonPressedDelegate OnPressed
		{
			get;
			set;
		}

		public UXButtonReleasedDelegate OnReleased
		{
			get;
			set;
		}

		public bool VisuallyDisabled
		{
			get;
			private set;
		}

		public UXButton(UXCamera uxCamera, UXButtonComponent component) : base(uxCamera, component.gameObject, component.NGUIButton)
		{
			this.component = component;
			this.OnClicked = null;
			this.OnPressed = null;
			this.OnReleased = null;
			this.NGUIButton = component.NGUIButton;
			this.VisuallyDisabled = false;
			if (this.NGUIButton != null)
			{
				this.defaultButtonColor = this.NGUIButton.defaultColor;
			}
			this.inhibitor = ((Service.UserInputInhibitor == null) ? null : Service.UserInputInhibitor);
		}

		public override void InternalDestroyComponent()
		{
			this.component.Button = null;
			UnityEngine.Object.Destroy(this.component);
		}

		public void InternalOnClick()
		{
			if (this.OnClicked != null && (this.inhibitor == null || this.inhibitor.IsAllowable(this)))
			{
				this.OnClicked(this);
				base.SendClickEvent();
			}
		}

		public void InternalOnPress()
		{
			if (this.OnPressed != null && (this.inhibitor == null || this.inhibitor.IsAllowable(this)))
			{
				this.OnPressed(this);
			}
		}

		public void InternalOnRelease()
		{
			if (this.OnReleased != null && (this.inhibitor == null || this.inhibitor.IsAllowable(this)))
			{
				this.OnReleased(this);
			}
		}

		public void VisuallyDisableButton()
		{
			this.VisuallyDisabled = true;
			this.NGUIButton.defaultColor = this.NGUIButton.disabledColor;
			this.NGUIButton.UpdateColor(true);
		}

		public void VisuallyEnableButton()
		{
			this.VisuallyDisabled = false;
			this.NGUIButton.defaultColor = this.defaultButtonColor;
			this.NGUIButton.UpdateColor(true);
		}

		public void SetDefaultColor(float r, float g, float b, float a)
		{
			this.NGUIButton.defaultColor = new Color(r, g, b, a);
			this.NGUIButton.UpdateColor(true);
		}
	}
}
