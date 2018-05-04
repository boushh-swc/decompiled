using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Battle
{
	public class DeploymentRecord : ISerializable
	{
		private string uid;

		private string action;

		private uint time;

		private int boardX;

		private int boardZ;

		public string Uid
		{
			get
			{
				return this.uid;
			}
		}

		public string Action
		{
			get
			{
				return this.action;
			}
		}

		public uint Time
		{
			get
			{
				return this.time;
			}
		}

		public int BoardX
		{
			get
			{
				return this.boardX;
			}
		}

		public int BoardZ
		{
			get
			{
				return this.boardZ;
			}
		}

		public DeploymentRecord()
		{
		}

		public DeploymentRecord(string uid, string action, uint time, int boardX, int boardZ)
		{
			this.uid = uid;
			this.action = action;
			this.time = time;
			this.boardX = boardX;
			this.boardZ = boardZ;
		}

		public string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("uid", this.uid);
			serializer.Add<uint>("time", this.time);
			serializer.Add<int>("x", this.boardX);
			serializer.Add<int>("z", this.boardZ);
			serializer.AddString("action", this.action);
			return serializer.End().ToString();
		}

		public ISerializable FromObject(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.uid = (dictionary["uid"] as string);
			this.time = (uint)dictionary["time"];
			this.boardX = (int)dictionary["x"];
			this.boardZ = (int)dictionary["z"];
			return this;
		}
	}
}
