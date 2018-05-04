using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class TimerAlertScreen : AlertScreen, IViewClockTimeObserver
	{
		private int endTimeStamp;

		private string timerTextUID;

		public TimerAlertScreen(string titleUID, string timerTextUID, string messageUID, string buttonLabelUID, int endTimeStamp, OnScreenModalResult onModalResult) : base(false, string.Empty, string.Empty, string.Empty, false)
		{
			this.endTimeStamp = endTimeStamp;
			this.timerTextUID = timerTextUID;
			this.title = this.lang.Get(titleUID, new object[0]);
			this.message = this.lang.Get(messageUID, new object[0]);
			this.primaryLabelOverride = this.lang.Get(buttonLabelUID, new object[0]);
			base.OnModalResult = onModalResult;
		}

		protected override void SetupControls()
		{
			base.SetupControls();
			this.labelTimer.Visible = true;
			this.UpdateTimeText();
			Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
		}

		private void UpdateTimeText()
		{
			uint serverTime = Service.ServerAPI.ServerTime;
			int num = this.endTimeStamp - (int)serverTime;
			string text = string.Empty;
			if (!string.IsNullOrEmpty(this.timerTextUID))
			{
				text = this.lang.Get(this.timerTextUID, new object[]
				{
					LangUtils.FormatTime((long)num)
				});
			}
			else
			{
				text = LangUtils.FormatTime((long)num);
			}
			this.labelTimer.Text = text;
		}

		public void OnViewClockTime(float dt)
		{
			this.UpdateTimeText();
		}

		public override void OnDestroyElement()
		{
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			base.OnDestroyElement();
		}
	}
}
