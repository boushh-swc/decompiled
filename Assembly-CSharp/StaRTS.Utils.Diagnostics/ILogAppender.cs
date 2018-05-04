using System;

namespace StaRTS.Utils.Diagnostics
{
	public interface ILogAppender
	{
		void AddLogMessage(LogEntry logEntry);
	}
}
