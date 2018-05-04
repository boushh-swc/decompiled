using StaRTS.Utils;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaRTS.Main.Models.Player.World
{
	public class ContractTO : ISerializable
	{
		public string Uid
		{
			get;
			set;
		}

		public uint EndTime
		{
			get;
			set;
		}

		public ContractType ContractType
		{
			get;
			set;
		}

		public string BuildingKey
		{
			get;
			set;
		}

		public string Tag
		{
			get;
			set;
		}

		public List<string> PerkIds
		{
			get;
			set;
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)obj;
			this.Uid = (string)dictionary["uid"];
			this.EndTime = Convert.ToUInt32(dictionary["endTime"]);
			string name = StringUtils.ToLowerCaseUnderscoreSeperated((string)dictionary["contractType"]);
			this.ContractType = StringUtils.ParseEnum<ContractType>(name);
			this.BuildingKey = (string)dictionary["buildingId"];
			this.Tag = ((!dictionary.ContainsKey("tag")) ? string.Empty : ((string)dictionary["tag"]));
			this.PerkIds = new List<string>();
			if (dictionary.ContainsKey("perkIds"))
			{
				List<object> list = dictionary["perkIds"] as List<object>;
				int i = 0;
				int count = list.Count;
				while (i < count)
				{
					this.PerkIds.Add(list[i] as string);
					i++;
				}
			}
			return this;
		}

		public string ToJson()
		{
			Serializer serializer = Serializer.Start();
			return serializer.End().ToString();
		}

		public void AddString(StringBuilder sb)
		{
			sb.Append(this.Uid).Append("|").Append(this.BuildingKey).Append("|").Append(this.EndTime).Append("|").Append(this.ContractType.ToString()).Append("|").Append("\n");
		}
	}
}
