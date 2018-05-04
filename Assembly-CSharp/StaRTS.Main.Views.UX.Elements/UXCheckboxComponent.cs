using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXCheckboxComponent : MonoBehaviour
	{
		private bool started;

		private UIToggle toggle;

		public UIToggle NGUICheckbox
		{
			get
			{
				return this.toggle;
			}
			set
			{
				if (this.toggle != null)
				{
					EventDelegate.Remove(this.toggle.onChange, new EventDelegate.Callback(this.OnSelectStateChanged));
				}
				this.toggle = value;
				if (this.toggle != null)
				{
					EventDelegate.Add(this.toggle.onChange, new EventDelegate.Callback(this.OnSelectStateChanged), false);
				}
			}
		}

		public UIButton NGUIButton
		{
			get;
			set;
		}

		public UIPlayTween NGUITween
		{
			get;
			set;
		}

		public UXCheckbox Checkbox
		{
			get;
			set;
		}

		public bool Selected
		{
			get
			{
				return this.NGUICheckbox != null && this.NGUICheckbox.value;
			}
			set
			{
				if (this.NGUICheckbox != null)
				{
					int radioGroup = 0;
					if (!value)
					{
						radioGroup = this.RadioGroup;
						this.RadioGroup = 0;
					}
					this.NGUICheckbox.value = value;
					if (!value)
					{
						this.RadioGroup = radioGroup;
					}
				}
			}
		}

		public int RadioGroup
		{
			get
			{
				return (!(this.NGUICheckbox == null)) ? this.NGUICheckbox.group : 0;
			}
			set
			{
				if (this.NGUICheckbox != null)
				{
					this.NGUICheckbox.group = value;
				}
			}
		}

		public void RemoveDelegate()
		{
			if (this.NGUICheckbox != null)
			{
				EventDelegate.Remove(this.NGUICheckbox.onChange, new EventDelegate.Callback(this.OnSelectStateChanged));
			}
		}

		public void DelayedSelect(bool value)
		{
			if (base.gameObject.activeSelf)
			{
				base.StartCoroutine(this.DelayedSelectCoroutine(value));
			}
		}

		[DebuggerHidden]
		private IEnumerator DelayedSelectCoroutine(bool value)
		{
			yield return null;
			this.NGUICheckbox.value = value;
			yield break;
		}

		private void Start()
		{
			this.started = true;
		}

		private void OnSelectStateChanged()
		{
			if (this.started && this.Checkbox != null)
			{
				this.Checkbox.InternalOnSelect(this.Selected);
			}
		}
	}
}
