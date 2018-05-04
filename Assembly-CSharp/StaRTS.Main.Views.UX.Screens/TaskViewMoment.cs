using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class TaskViewMoment
	{
		public float Delay;

		public float Duration;

		public float Elapsed;

		public Action<TaskViewMoment> UpdateCallback;

		public object UpdateCookie;

		public Action<TaskViewMoment> CompleteCallback;

		public object CompleteCookie;

		public Action<TaskViewMoment> StartCallback;

		public object StartCookie;
	}
}
