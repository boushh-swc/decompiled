using System;
using UnityEngine;

namespace StaRTS.Utils.Diagnostics
{
	public class UnityLogAppender : BaseLogAppender
	{
		private const int MAX_LENGTH = 10000;

		public bool CurrentlyLogging
		{
			get;
			set;
		}

		protected override void Trace(string formattedMessage)
		{
			this.CurrentlyLogging = true;
			if (formattedMessage.Length > 10000)
			{
				formattedMessage = formattedMessage.Substring(0, 10000);
			}
			LogLevel level = this.entry.Level;
			if (level != LogLevel.Error)
			{
				if (level != LogLevel.Warn)
				{
					Debug.Log(formattedMessage);
				}
				else
				{
					Debug.LogWarning(formattedMessage);
				}
			}
			else
			{
				Debug.LogError(formattedMessage);
			}
			this.CurrentlyLogging = false;
		}
	}
}
