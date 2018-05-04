using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player.Objectives
{
	public class ObjectiveProgress : ISerializable
	{
		public string ObjectiveUid;

		public string PlanetId;

		public int HQ = 1;

		public int Count;

		public int Target = 1;

		public ObjectiveState State;

		public bool ClaimAttempt;

		public ObjectiveVO VO
		{
			get
			{
				return Service.StaticDataController.Get<ObjectiveVO>(this.ObjectiveUid);
			}
		}

		public ObjectiveProgress(ObjectiveProgress cloneFrom)
		{
			this.ObjectiveUid = cloneFrom.ObjectiveUid;
			this.PlanetId = cloneFrom.PlanetId;
			this.HQ = cloneFrom.HQ;
			this.Count = cloneFrom.Count;
			this.Target = cloneFrom.Target;
			this.State = cloneFrom.State;
		}

		public ObjectiveProgress(string planetId)
		{
			this.PlanetId = planetId;
		}

		public string ToJson()
		{
			Service.Logger.Warn("Attempting to inappropriately serialize an ObjectiveGroup");
			return string.Empty;
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("uid"))
			{
				this.ObjectiveUid = (dictionary["uid"] as string);
			}
			if (dictionary.ContainsKey("hq"))
			{
				this.HQ = Convert.ToInt32(dictionary["hq"]);
			}
			if (dictionary.ContainsKey("count"))
			{
				this.Count = Convert.ToInt32(dictionary["count"]);
			}
			if (dictionary.ContainsKey("target"))
			{
				this.Target = Convert.ToInt32(dictionary["target"]);
			}
			if (dictionary.ContainsKey("state"))
			{
				this.State = StringUtils.ParseEnum<ObjectiveState>(dictionary["state"] as string);
			}
			return this;
		}
	}
}
