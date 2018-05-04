using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Cameras;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Text;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXInput : UXElement, IEventObserver
	{
		private UXInputComponent inputComponent;

		public virtual string Text
		{
			get
			{
				return this.FilterWhitespace(this.inputComponent.Text);
			}
			set
			{
				this.inputComponent.Text = this.FilterWhitespace(value);
			}
		}

		public bool EnableInput
		{
			get
			{
				return this.inputComponent.NGUIInput.enabled;
			}
			set
			{
				this.inputComponent.NGUIInput.enabled = value;
			}
		}

		public UXInput(UXCamera uxCamera, UXInputComponent component) : base(uxCamera, component.gameObject, null)
		{
			this.inputComponent = component;
			Service.EventManager.RegisterObserver(this, EventId.DenyUserInput);
			Service.EventManager.RegisterObserver(this, EventId.AllowUserInput);
		}

		public override void InternalDestroyComponent()
		{
			Service.EventManager.UnregisterObserver(this, EventId.DenyUserInput);
			Service.EventManager.UnregisterObserver(this, EventId.AllowUserInput);
			this.inputComponent.Input = null;
			UnityEngine.Object.Destroy(this.inputComponent);
			this.inputComponent = null;
			base.InternalDestroyComponent();
		}

		public UIInput GetUIInputComponent()
		{
			return this.inputComponent.NGUIInput;
		}

		public void InitText(string initText)
		{
			this.inputComponent.InitText = initText;
		}

		private string FilterWhitespace(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return s;
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			bool flag2 = false;
			int i = 0;
			int length = s.Length;
			while (i < length)
			{
				char value = '\0';
				char c;
				uint u;
				bool flag3 = this.ParseUTF16(s, i, length, out c, ref value, out u);
				if (flag3)
				{
					i++;
				}
				bool flag4 = (!flag3 && char.IsWhiteSpace(c)) || this.IsEmoji(u);
				if (flag4)
				{
					if (!flag)
					{
						flag2 = true;
					}
				}
				else
				{
					if (flag2)
					{
						stringBuilder.Append(' ');
						flag2 = false;
					}
					stringBuilder.Append(c);
					if (flag3)
					{
						stringBuilder.Append(value);
					}
				}
				flag = flag4;
				i++;
			}
			return stringBuilder.ToString();
		}

		private bool ParseUTF16(string s, int i, int length, out char c, ref char d, out uint u)
		{
			c = s[i];
			ushort num = (ushort)c;
			u = (uint)num;
			if (num >= 55296 && num <= 56319 && ++i < length)
			{
				ushort num2 = (ushort)s[i];
				if (num2 >= 56320 && num2 <= 57343)
				{
					d = (char)num2;
					u = (u << 10) + (uint)num2 - 56613888u;
					return true;
				}
			}
			return false;
		}

		private bool IsEmoji(uint u)
		{
			return 126976u <= u && u <= 128895u;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.DenyUserInput)
			{
				if (id == EventId.AllowUserInput)
				{
					base.GetUIWidget.enabled = true;
				}
			}
			else
			{
				base.GetUIWidget.enabled = false;
			}
			return EatResponse.NotEaten;
		}
	}
}
