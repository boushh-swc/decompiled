using System;

namespace StaRTS.Main.Utils.Events
{
	public class AudioEventData
	{
		public EventId EventId
		{
			get;
			set;
		}

		public object EventCookie
		{
			get;
			set;
		}

		public AudioEventData(EventId eventId, string eventCookie)
		{
			this.EventId = eventId;
			this.EventCookie = eventCookie;
		}
	}
}
