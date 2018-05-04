using StaRTS.Main.Models.Squads;
using StaRTS.Main.Views.UX.Elements;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Squads
{
	public abstract class AbstractSquadMsgDisplay : ISquadMsgDisplay
	{
		protected UXTable table;

		public AbstractSquadMsgDisplay(UXTable table)
		{
			this.table = table;
		}

		public virtual void ProcessNewMessages(List<SquadMsg> messages)
		{
			bool flag = false;
			int i = 0;
			int count = messages.Count;
			while (i < count)
			{
				bool flag2 = this.ProcessMessage(messages[i]);
				flag = (flag || flag2);
				i++;
			}
			if (flag)
			{
				this.table.RepositionItems();
			}
		}

		protected virtual bool ProcessMessage(SquadMsg msg)
		{
			return true;
		}

		public virtual void RemoveMessage(SquadMsg msg)
		{
		}

		public virtual void Destroy()
		{
			this.table.Clear();
			this.table = null;
		}
	}
}
