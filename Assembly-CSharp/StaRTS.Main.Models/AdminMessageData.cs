using System;

namespace StaRTS.Main.Models
{
	public class AdminMessageData
	{
		public string Uid
		{
			get;
			private set;
		}

		public string Message
		{
			get;
			private set;
		}

		public bool IsCritical
		{
			get;
			private set;
		}

		public AdminMessageData(string uid, string message, bool isCritical)
		{
			this.Uid = uid;
			this.Message = message;
			this.IsCritical = isCritical;
		}
	}
}
