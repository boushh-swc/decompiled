using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.UserInput;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXCheckbox : UXElement
	{
		private const float AUTO_DESELECT_TIME = 0.1f;

		private UXCheckboxComponent component;

		private UserInputInhibitor inhibitor;

		private int radioGroup;

		public UXCheckboxSelectedDelegate OnSelected
		{
			get;
			set;
		}

		public bool Selected
		{
			get
			{
				return this.component.Selected;
			}
			set
			{
				this.component.Selected = ((this.inhibitor != null) ? (value && this.inhibitor.IsAllowable(this)) : value);
			}
		}

		public int RadioGroup
		{
			get
			{
				return this.component.RadioGroup;
			}
			set
			{
				this.radioGroup = value;
				this.component.RadioGroup = this.radioGroup;
			}
		}

		public UXCheckbox(UXCamera uxCamera, UXCheckboxComponent component) : base(uxCamera, component.gameObject, component.NGUIButton)
		{
			this.component = component;
			this.inhibitor = Service.UserInputInhibitor;
			this.radioGroup = this.RadioGroup;
			this.OnSelected = null;
		}

		public override void InternalDestroyComponent()
		{
			this.component.RemoveDelegate();
			this.component.Checkbox = null;
			UnityEngine.Object.Destroy(this.component);
		}

		public void SetSelected(bool selected)
		{
			this.component.Selected = selected;
		}

		public void InternalOnSelect(bool selected)
		{
			if (this.inhibitor == null || this.inhibitor.IsAllowable(this))
			{
				if (this.OnSelected != null)
				{
					this.OnSelected(this, selected);
				}
			}
			else if (this.radioGroup != 0 && selected)
			{
				Service.ViewTimerManager.CreateViewTimer(0.1f, false, new TimerDelegate(this.OnDeselectTimer), this);
			}
			if (this.radioGroup == 0 || selected)
			{
				base.SendClickEvent();
			}
		}

		private void OnDeselectTimer(uint id, object cookie)
		{
			UXCheckbox uXCheckbox = cookie as UXCheckbox;
			if (uXCheckbox.component != null)
			{
				uXCheckbox.component.Selected = false;
			}
		}

		public void SetTweenTarget(UXElement element)
		{
			UIButton nGUIButton = this.component.NGUIButton;
			nGUIButton.tweenTarget = element.Root;
		}

		public void SetSelectable(bool selectable)
		{
			UIToggle nGUICheckbox = this.component.NGUICheckbox;
			if (nGUICheckbox != null && nGUICheckbox.enabled != selectable)
			{
				nGUICheckbox.enabled = selectable;
			}
			UIPlayTween nGUITween = this.component.NGUITween;
			if (nGUITween != null && nGUITween.enabled != selectable)
			{
				nGUITween.enabled = selectable;
			}
		}

		public void SetAnimationAndSprite(UXSprite uxSprite)
		{
			this.component.NGUICheckbox.activeAnimation = uxSprite.Root.GetComponent<Animation>();
			this.component.NGUICheckbox.activeSprite = uxSprite.GetUIWidget;
		}

		public void DelayedSelect(bool value)
		{
			this.component.DelayedSelect(value);
		}
	}
}
