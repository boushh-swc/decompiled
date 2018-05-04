using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Episodes
{
	public class EpisodeProgressInfo : ISerializable
	{
		public bool introStoryViewed;

		public string uid
		{
			get;
			private set;
		}

		public DateTime endTime
		{
			get;
			private set;
		}

		public List<string> finishedTasks
		{
			get;
			private set;
		}

		public int currentTaskIndex
		{
			get;
			private set;
		}

		public EpisodeTaskProgressInfo currentTask
		{
			get;
			private set;
		}

		public EpisodeGrindProgressInfo grindInfo
		{
			get;
			private set;
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("uid"))
			{
				this.uid = (string)dictionary["uid"];
			}
			if (dictionary.ContainsKey("endTime"))
			{
				uint seconds = Convert.ToUInt32(dictionary["endTime"]);
				this.endTime = DateUtils.DateFromSeconds(seconds);
			}
			if (dictionary.ContainsKey("finishedTasks") && dictionary["finishedTasks"] != null)
			{
				List<object> list = (List<object>)dictionary["finishedTasks"];
				if (list.Count > 0)
				{
					this.finishedTasks = new List<string>();
					int i = 0;
					int count = list.Count;
					while (i < count)
					{
						string item = (string)list[i];
						this.finishedTasks.Add(item);
						i++;
					}
				}
			}
			if (dictionary.ContainsKey("currentTaskIndex"))
			{
				this.currentTaskIndex = Convert.ToInt32(dictionary["currentTaskIndex"]);
			}
			if (dictionary.ContainsKey("currentTask") && dictionary["currentTask"] != null)
			{
				this.currentTask = new EpisodeTaskProgressInfo();
				this.currentTask.FromObject(dictionary["currentTask"]);
			}
			if (dictionary.ContainsKey("introStoryViewed"))
			{
				this.introStoryViewed = Convert.ToBoolean(dictionary["introStoryViewed"]);
			}
			if (dictionary.ContainsKey("grind"))
			{
				this.grindInfo = new EpisodeGrindProgressInfo();
				if (dictionary["grind"] != null)
				{
					this.grindInfo.FromObject(dictionary["grind"]);
				}
			}
			return this;
		}

		public string ToJson()
		{
			Service.Logger.Warn("Attempting to inappropriately serialize EpisodeProgressInfo");
			return string.Empty;
		}
	}
}
