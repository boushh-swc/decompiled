using StaRTS.Main.Models;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Utils.Diagnostics
{
	public class Logger
	{
		public LogLevel ErrorLevels;

		private List<ILogAppender> logAppenders = new List<ILogAppender>();

		private List<LogEntry> pendingEntries;

		private bool started;

		public Logger()
		{
			Service.Logger = this;
		}

		public void AddAppender(ILogAppender appender)
		{
			if (appender == null)
			{
				throw new ArgumentNullException("appender");
			}
			this.logAppenders.Add(appender);
		}

		public void Start()
		{
			this.Start(this.logAppenders);
		}

		public void Start(List<ILogAppender> appenders)
		{
			if (appenders.Count == 0)
			{
				this.AddAppender(new UnityLogAppender());
			}
			this.started = true;
			this.FlushPendingEntries();
		}

		public void FlushPendingEntries()
		{
			if (this.pendingEntries == null)
			{
				return;
			}
			for (int i = 0; i < this.pendingEntries.Count; i++)
			{
				this.ProcessEntry(this.pendingEntries[i]);
			}
			this.pendingEntries = null;
		}

		protected void ProcessEntry(LogEntry entry)
		{
			if (entry == null)
			{
				throw new ArgumentNullException("entry");
			}
			entry.Timestamp = DateTime.UtcNow;
			if (!this.started)
			{
				if (this.pendingEntries == null)
				{
					this.pendingEntries = new List<LogEntry>();
				}
				this.pendingEntries.Add(entry);
				return;
			}
			if ((this.ErrorLevels & entry.Level) == LogLevel.None)
			{
				return;
			}
			int i = 0;
			int count = this.logAppenders.Count;
			while (i < count)
			{
				this.logAppenders[i].AddLogMessage(entry);
				i++;
			}
		}

		public void Debug(string message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			this.ProcessEntry(new LogEntry
			{
				Message = message,
				Level = LogLevel.Debug
			});
		}

		public void DebugFormat(string message, params object[] args)
		{
			this.Debug(string.Format(message, args));
		}

		public void Warn(string message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			this.ProcessEntry(new LogEntry
			{
				Message = message,
				Level = LogLevel.Warn
			});
		}

		public void WarnFormat(string message, params object[] args)
		{
			this.Warn(string.Format(message, args));
		}

		public void Error(string message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			LogEntry logEntry = new LogEntry();
			logEntry.Message = message;
			if (GameConstants.LOG_STACK_TRACE_TO_BI)
			{
				logEntry.Message = message + " StackTrace:" + Environment.StackTrace;
			}
			logEntry.Level = LogLevel.Error;
			this.ProcessEntry(logEntry);
		}

		public void ErrorFormat(string message, params object[] args)
		{
			this.Error(string.Format(message, args));
		}
	}
}
