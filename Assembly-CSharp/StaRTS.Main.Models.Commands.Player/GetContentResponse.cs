using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Player
{
	public class GetContentResponse : AbstractResponse
	{
		public List<string> CdnRoots
		{
			get;
			private set;
		}

		public string AppCode
		{
			get;
			private set;
		}

		public string Environment
		{
			get;
			private set;
		}

		public string ManifestPath
		{
			get;
			private set;
		}

		public string ManifestVersion
		{
			get;
			private set;
		}

		public List<string> Patches
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("secureCdnRoots"))
			{
				List<object> list = dictionary["secureCdnRoots"] as List<object>;
				this.CdnRoots = new List<string>();
				int i = 0;
				int count = list.Count;
				while (i < count)
				{
					this.CdnRoots.Add((string)list[i]);
					i++;
				}
			}
			if (dictionary.ContainsKey("appCode"))
			{
				this.AppCode = (string)dictionary["appCode"];
			}
			if (dictionary.ContainsKey("environment"))
			{
				this.Environment = (string)dictionary["environment"];
			}
			if (dictionary.ContainsKey("manifest"))
			{
				this.ManifestPath = (string)dictionary["manifest"];
			}
			if (dictionary.ContainsKey("manifestVersion"))
			{
				this.ManifestVersion = (string)dictionary["manifestVersion"];
			}
			if (dictionary.ContainsKey("patches"))
			{
				List<object> list2 = dictionary["patches"] as List<object>;
				this.Patches = new List<string>();
				for (int j = 0; j < list2.Count; j++)
				{
					this.Patches.Add((string)list2[j]);
				}
			}
			return this;
		}
	}
}
