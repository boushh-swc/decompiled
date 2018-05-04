using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Holonet
{
	public class DevNotesHolonetController : IHolonetContoller
	{
		private const int MAX_DEVNOTES_BADGE_COUNT = 3;

		public List<DevNoteEntryVO> DevNotes;

		public HolonetControllerType ControllerType
		{
			get
			{
				return HolonetControllerType.DevNotes;
			}
		}

		public DevNotesHolonetController()
		{
			this.DevNotes = new List<DevNoteEntryVO>();
		}

		public void PrepareContent(int lastTimeViewed)
		{
			int num = 0;
			this.DevNotes.Clear();
			int serverTime = (int)Service.ServerAPI.ServerTime;
			StaticDataController staticDataController = Service.StaticDataController;
			this.DevNotes = new List<DevNoteEntryVO>();
			foreach (DevNoteEntryVO current in staticDataController.GetAll<DevNoteEntryVO>())
			{
				if (current.StartTime <= serverTime && serverTime < current.EndTime)
				{
					this.DevNotes.Add(current);
					if (current.StartTime > lastTimeViewed)
					{
						num++;
					}
				}
			}
			HolonetController holonetController = Service.HolonetController;
			this.DevNotes.Sort(new Comparison<DevNoteEntryVO>(holonetController.CompareTimestamps));
			holonetController.ContentPrepared(this, Math.Min(num, 3));
		}
	}
}
