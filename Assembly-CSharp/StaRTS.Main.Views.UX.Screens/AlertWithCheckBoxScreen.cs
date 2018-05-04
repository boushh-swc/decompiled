using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class AlertWithCheckBoxScreen : AlertScreen, IViewClockTimeObserver
	{
		public delegate void OnCheckBoxScreenModalResult(object result, bool selected);

		private const string CONFIRMATION_BOX_GROUP = "buttonConfirmation";

		private const string CONFIRMATION_BOX_LABEL = "LabelConfirmation";

		private int timerEndSeconds;

		private Color timerColor;

		private string timerTextID;

		private string checkboxTextID;

		private bool buttonOption2Enabled;

		private AlertWithCheckBoxScreen.OnCheckBoxScreenModalResult resultCallback;

		private UXCheckbox checkbox;

		public AlertWithCheckBoxScreen(string title, string message, string checkBoxTextID, string timerTextID, int timerLeftSeconds, Color timerColor, AlertWithCheckBoxScreen.OnCheckBoxScreenModalResult callback) : base(false, title, message, string.Empty, false)
		{
			this.timerEndSeconds = timerLeftSeconds;
			this.timerColor = timerColor;
			this.timerTextID = timerTextID;
			this.resultCallback = callback;
			base.OnModalResult = new OnScreenModalResult(this.OnScreenClosed);
			this.checkboxTextID = checkBoxTextID;
		}

		public AlertWithCheckBoxScreen(string title, string message, string checkBoxTextID, AlertWithCheckBoxScreen.OnCheckBoxScreenModalResult callback) : this(title, message, checkBoxTextID, string.Empty, 0, Color.white, callback)
		{
		}

		protected override void SetupControls()
		{
			base.SetupControls();
			if (this.timerEndSeconds > 0)
			{
				Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
				this.labelTimer.Visible = true;
				this.labelTimer.TextColor = this.timerColor;
				this.UpdateTimer();
			}
			base.GetElement<UXLabel>("LabelConfirmation").Text = this.lang.Get(this.checkboxTextID, new object[0]);
			this.checkbox = base.GetElement<UXCheckbox>("buttonConfirmation");
			this.checkbox.Visible = true;
			this.checkbox.Selected = false;
			this.primaryButton.Visible = !this.buttonOption2Enabled;
			this.buttonOption2.Visible = this.buttonOption2Enabled;
			this.primary2OptionButton.Visible = this.buttonOption2Enabled;
			this.secondary2OptionButton.Visible = this.buttonOption2Enabled;
			this.primaryButton.OnClicked = new UXButtonClickedDelegate(this.OnOKClicked);
			this.primary2OptionButton.OnClicked = new UXButtonClickedDelegate(this.OnOKClicked);
			this.secondary2OptionButton.OnClicked = new UXButtonClickedDelegate(this.OnCancelClicked);
		}

		public void OnViewClockTime(float dt)
		{
			this.timerEndSeconds -= Mathf.CeilToInt(1f);
			if (this.timerEndSeconds > 0)
			{
				this.UpdateTimer();
			}
			else
			{
				this.labelTimer.Visible = false;
				Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			}
		}

		private void OnOKClicked(UXButton btn)
		{
			this.Close(true);
		}

		private void OnCancelClicked(UXButton btn)
		{
			this.Close(null);
		}

		private void UpdateTimer()
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(this.timerTextID))
			{
				text = this.lang.Get(this.timerTextID, new object[]
				{
					LangUtils.FormatTime((long)this.timerEndSeconds)
				});
			}
			else
			{
				text = LangUtils.FormatTime((long)this.timerEndSeconds);
			}
			this.labelTimer.Text = text;
		}

		private void OnScreenClosed(object result, object cookie)
		{
			if (this.resultCallback != null)
			{
				this.resultCallback(result, this.checkbox.Selected);
			}
		}

		public override void OnDestroyElement()
		{
			if (this.timerEndSeconds > 0)
			{
				Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			}
			base.OnDestroyElement();
		}

		public void Set2ButtonGroupEnabledState(bool enabled)
		{
			this.buttonOption2Enabled = enabled;
		}
	}
}
