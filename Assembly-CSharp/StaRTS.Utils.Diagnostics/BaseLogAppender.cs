using System;

namespace StaRTS.Utils.Diagnostics
{
	public abstract class BaseLogAppender : ILogAppender
	{
		protected LogEntry entry;

		protected abstract void Trace(string formattedMessage);

		public void AddLogMessage(LogEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			this.entry = entry;
			string formattedMessage = string.Format("{0} {1}: {2}", entry.Timestamp, entry.Level, entry.Message);
			this.Trace(formattedMessage);
		}
	}
}
