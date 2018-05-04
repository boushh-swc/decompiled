using StaRTS.Main.Utils.Events;
using System;

namespace StaRTS.Main.Controllers.Goals
{
	public class EpisodeProgressData
	{
		public string episodeUid;

		public string taskUid;

		public int progress;

		public int prevProgress;

		public EventId progressType;

		public int progressIndex;

		public EpisodeProgressData(string episodeUid, string taskUid, int progress, int prevProgress, EventId progressType, int progressIndex)
		{
			this.episodeUid = episodeUid;
			this.taskUid = taskUid;
			this.progress = progress;
			this.prevProgress = prevProgress;
			this.progressType = progressType;
			this.progressIndex = progressIndex;
		}
	}
}
