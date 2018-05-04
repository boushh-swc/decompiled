using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Externals.Manimal.TransferObjects.Request
{
	public class Batch : AbstractRequest
	{
		private string authKey;

		private uint lastLoginTime;

		private bool pickupMessages;

		public List<ICommand> Commands
		{
			get;
			private set;
		}

		public bool Sync
		{
			get;
			set;
		}

		public uint Tries
		{
			get;
			set;
		}

		public Batch()
		{
			this.Commands = new List<ICommand>();
			this.Init();
		}

		public Batch(ICommand command)
		{
			this.Commands = new List<ICommand>(1);
			this.Commands.Add(command);
			this.Init();
		}

		public Batch(List<ICommand> commands)
		{
			this.Commands = new List<ICommand>(commands);
			this.Init();
		}

		private void Init()
		{
			this.Sync = true;
			this.Tries = 1u;
		}

		public void Prepare(string authKey, uint lastLoginTime, bool pickupMessages)
		{
			this.authKey = authKey;
			this.lastLoginTime = lastLoginTime;
			this.pickupMessages = pickupMessages;
		}

		public override string ToJson()
		{
			return Serializer.Start().AddString("authKey", this.authKey).AddBool("pickupMessages", this.pickupMessages).Add<uint>("lastLoginTime", this.lastLoginTime).AddArray<ICommand>("commands", this.Commands).End().ToString();
		}
	}
}
