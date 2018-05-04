using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player.Objectives
{
	public class ObjectiveGroup : ISerializable
	{
		private string planetId;

		public string GroupId
		{
			get;
			set;
		}

		public string GroupSeriesId
		{
			get;
			private set;
		}

		public int StartTimestamp
		{
			get;
			set;
		}

		public int GraceTimestamp
		{
			get;
			set;
		}

		public int EndTimestamp
		{
			get;
			set;
		}

		public List<ObjectiveProgress> ProgressObjects
		{
			get;
			set;
		}

		public ObjectiveGroup(ObjectiveGroup cloneFrom)
		{
			this.GroupId = cloneFrom.GroupId;
			this.GroupSeriesId = this.GroupId.Substring(0, this.GroupId.LastIndexOf('_'));
			this.StartTimestamp = cloneFrom.StartTimestamp;
			this.GraceTimestamp = cloneFrom.GraceTimestamp;
			this.EndTimestamp = cloneFrom.EndTimestamp;
			this.ProgressObjects = new List<ObjectiveProgress>();
			foreach (ObjectiveProgress current in cloneFrom.ProgressObjects)
			{
				this.ProgressObjects.Add(new ObjectiveProgress(current));
			}
			this.planetId = cloneFrom.planetId;
		}

		public ObjectiveGroup(string planetId)
		{
			this.planetId = planetId;
			this.ProgressObjects = new List<ObjectiveProgress>();
		}

		public string ToJson()
		{
			Service.Logger.Warn("Attempting to inappropriately serialize an ObjectiveGroup");
			return string.Empty;
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("groupId"))
			{
				this.GroupId = Convert.ToString(dictionary["groupId"]);
				this.GroupSeriesId = this.GroupId.Substring(0, this.GroupId.LastIndexOf('_'));
			}
			if (dictionary.ContainsKey("startTime"))
			{
				this.StartTimestamp = Convert.ToInt32(dictionary["startTime"]);
			}
			if (dictionary.ContainsKey("graceTime"))
			{
				this.GraceTimestamp = Convert.ToInt32(dictionary["graceTime"]);
			}
			if (dictionary.ContainsKey("endTime"))
			{
				this.EndTimestamp = Convert.ToInt32(dictionary["endTime"]);
			}
			if (dictionary.ContainsKey("progress"))
			{
				List<object> list = dictionary["progress"] as List<object>;
				int i = 0;
				int count = list.Count;
				while (i < count)
				{
					this.ProgressObjects.Add(new ObjectiveProgress(this.planetId).FromObject(list[i]) as ObjectiveProgress);
					i++;
				}
			}
			return this;
		}
	}
}
