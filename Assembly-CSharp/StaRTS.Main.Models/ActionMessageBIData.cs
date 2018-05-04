using System;
using System.Runtime.InteropServices;

namespace StaRTS.Main.Models
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct ActionMessageBIData
	{
		public string Action
		{
			get;
			private set;
		}

		public string Message
		{
			get;
			private set;
		}

		public ActionMessageBIData(string action, string message)
		{
			this = default(ActionMessageBIData);
			this.Action = action;
			this.Message = message;
		}
	}
}
