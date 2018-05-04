using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers.Holonet
{
	public class HolonetAnimationController
	{
		public const float CLOSE_DURATION = 3f;

		private const string HIDE_HOLONET = "HideHolonet";

		private const string OPEN_TO_INCOMING = "OpenHolonetToIncoming";

		private const string OPEN_TO_COMMAND_CENTER = "OpenHolonetToCC";

		private const string SHOW_COMMAND_CENTER = "ShowCommandCenterTab";

		private const string SHOW_DEV_NOTES = "ShowDevNotesTab";

		private const string SHOW_TRANSMISSIONS = "ShowTransmissionsTab";

		private const string SHOW_BLANK = "ShowBlank";

		private HolonetScreen screen;

		private Animator anim;

		public HolonetAnimationController(HolonetScreen screen)
		{
			this.screen = screen;
			this.anim = this.screen.Root.GetComponent<Animator>();
		}

		public void OpenToIncomingTransmission()
		{
			this.anim.SetTrigger("OpenHolonetToIncoming");
		}

		public void OpenToCommandCenter()
		{
			this.anim.SetTrigger("OpenHolonetToCC");
		}

		public void CloseHolonet()
		{
			this.anim.SetTrigger("HideHolonet");
		}

		public void ShowCommandCenter()
		{
			this.anim.ResetTrigger("ShowBlank");
			this.anim.SetTrigger("ShowCommandCenterTab");
		}

		public void ShowDevNotes()
		{
			this.anim.SetTrigger("ShowDevNotesTab");
		}

		public void ShowTransmissionLog()
		{
			this.anim.SetTrigger("ShowTransmissionsTab");
		}

		private void ResetAllTriggers()
		{
			this.anim.ResetTrigger("HideHolonet");
			this.anim.ResetTrigger("OpenHolonetToIncoming");
			this.anim.ResetTrigger("OpenHolonetToCC");
			this.anim.ResetTrigger("ShowCommandCenterTab");
			this.anim.ResetTrigger("ShowDevNotesTab");
			this.anim.ResetTrigger("ShowTransmissionsTab");
			this.anim.ResetTrigger("ShowBlank");
		}

		public void Cleanup()
		{
			this.screen = null;
			this.anim = null;
		}
	}
}
