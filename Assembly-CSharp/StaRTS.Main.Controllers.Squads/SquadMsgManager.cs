using StaRTS.Main.Models;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Views.UX.Squads;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Squads
{
	public class SquadMsgManager
	{
		private SquadController controller;

		private List<SquadMsg> msgs;

		private Dictionary<string, SquadMsg> msgsByIds;

		private List<ISquadMsgDisplay> observers;

		private List<SquadMsg> msgsToProcess;

		private Dictionary<string, List<SquadMsg>> linkedMsgs;

		public SquadMsgManager(SquadController controller)
		{
			this.controller = controller;
			this.msgs = new List<SquadMsg>();
			this.msgsByIds = new Dictionary<string, SquadMsg>();
			this.observers = new List<ISquadMsgDisplay>();
			this.msgsToProcess = new List<SquadMsg>();
			this.linkedMsgs = new Dictionary<string, List<SquadMsg>>();
		}

		public void Enable()
		{
			this.controller.ServerManager.AddSquadMsgCallback(new SquadController.SquadMsgsCallback(this.OnNewSquadMsgs));
		}

		public void RegisterObserver(ISquadMsgDisplay observer)
		{
			if (this.observers.Contains(observer))
			{
				return;
			}
			this.observers.Add(observer);
		}

		public void UnregisterObserver(ISquadMsgDisplay observer)
		{
			if (this.observers.Contains(observer))
			{
				this.observers.Remove(observer);
			}
		}

		private void OnNewSquadMsgs(List<SquadMsg> msgs)
		{
			int count = msgs.Count;
			if (count == 0)
			{
				return;
			}
			this.msgsToProcess.Clear();
			for (int i = 0; i < count; i++)
			{
				SquadMsg squadMsg = msgs[i];
				if (string.IsNullOrEmpty(squadMsg.NotifId) || !this.msgsByIds.ContainsKey(squadMsg.NotifId))
				{
					this.msgs.Add(squadMsg);
					this.msgsToProcess.Add(squadMsg);
					SquadMsg parentMsg = this.GetParentMsg(squadMsg);
					if (parentMsg != null)
					{
						List<SquadMsg> list;
						if (this.linkedMsgs.ContainsKey(parentMsg.NotifId))
						{
							list = this.linkedMsgs[parentMsg.NotifId];
						}
						else
						{
							list = new List<SquadMsg>();
							this.linkedMsgs.Add(parentMsg.NotifId, list);
						}
						list.Add(squadMsg);
					}
					if (!string.IsNullOrEmpty(squadMsg.NotifId))
					{
						this.msgsByIds.Add(squadMsg.NotifId, squadMsg);
					}
				}
			}
			if (this.msgsToProcess.Count > 0)
			{
				this.msgs.Sort(new Comparison<SquadMsg>(this.SortMsg));
				this.msgsToProcess.Sort(new Comparison<SquadMsg>(this.SortMsg));
			}
			this.TrimMessages(this.msgs, this.msgsToProcess);
			this.controller.OnNewSquadMsgsReceived(this.msgsToProcess);
			int j = 0;
			int count2 = this.observers.Count;
			while (j < count2)
			{
				this.observers[j].ProcessNewMessages(this.msgsToProcess);
				j++;
			}
		}

		private SquadMsg GetParentMsg(SquadMsg msg)
		{
			SquadMsg result = null;
			if (msg.Type == SquadMsgType.TroopDonation && msg.DonationData != null && !string.IsNullOrEmpty(msg.DonationData.RequestId) && this.msgsByIds.ContainsKey(msg.DonationData.RequestId))
			{
				result = this.msgsByIds[msg.DonationData.RequestId];
			}
			return result;
		}

		private void TrimMessages(List<SquadMsg> msgs, List<SquadMsg> msgsToProcess)
		{
			List<SquadMsg> list = new List<SquadMsg>();
			for (int i = msgs.Count - 1 - this.controller.MessageLimit; i >= 0; i--)
			{
				SquadMsg squadMsg = msgs[i];
				if (squadMsg.Type != SquadMsgType.TroopDonation)
				{
					list.Add(squadMsg);
					if (!string.IsNullOrEmpty(squadMsg.NotifId) && this.linkedMsgs.ContainsKey(squadMsg.NotifId))
					{
						List<SquadMsg> list2 = this.linkedMsgs[squadMsg.NotifId];
						if (list2 != null)
						{
							list.AddRange(list2);
						}
					}
				}
			}
			int j = 0;
			int count = list.Count;
			while (j < count)
			{
				SquadMsg squadMsg2 = list[j];
				this.RemoveMsg(squadMsg2, false);
				msgsToProcess.Remove(squadMsg2);
				j++;
			}
		}

		public List<SquadMsg> GetExistingMessages()
		{
			return this.msgs;
		}

		private int SortMsg(SquadMsg a, SquadMsg b)
		{
			if (a.TimeSent < b.TimeSent)
			{
				return -1;
			}
			if (a.TimeSent > b.TimeSent)
			{
				return 1;
			}
			if (a.DonationData != null && b.DonationData == null)
			{
				return 1;
			}
			if (a.DonationData == null && b.DonationData != null)
			{
				return -1;
			}
			if (GameConstants.PHOTON_CHAT_COMPLEX_COMPARE_ENABLED && a.ChatData != null && b.ChatData != null)
			{
				return string.Compare(a.ChatData.Message, b.ChatData.Message);
			}
			return 0;
		}

		public SquadMsg GetMsgById(string id)
		{
			return (!this.msgsByIds.ContainsKey(id)) ? null : this.msgsByIds[id];
		}

		public void RemoveMsgsByType(string playerId, SquadMsgType[] types)
		{
			if (types == null)
			{
				return;
			}
			for (int i = this.msgs.Count - 1; i >= 0; i--)
			{
				SquadMsg squadMsg = this.msgs[i];
				if (squadMsg.OwnerData != null && squadMsg.OwnerData.PlayerId == playerId)
				{
					int j = 0;
					int num = types.Length;
					while (j < num)
					{
						if (squadMsg.Type == types[j])
						{
							this.RemoveMsg(squadMsg, true);
							break;
						}
						j++;
					}
				}
			}
		}

		private void RemoveMsg(SquadMsg msg, bool notifyObservers)
		{
			this.msgs.Remove(msg);
			if (!string.IsNullOrEmpty(msg.NotifId))
			{
				this.msgsByIds.Remove(msg.NotifId);
				this.linkedMsgs.Remove(msg.NotifId);
			}
			if (notifyObservers)
			{
				int i = 0;
				int count = this.observers.Count;
				while (i < count)
				{
					this.observers[i].RemoveMessage(msg);
					i++;
				}
			}
		}

		public void ClearAllMsgs()
		{
			this.msgs.Clear();
			this.msgsByIds.Clear();
			this.msgsToProcess.Clear();
			this.linkedMsgs.Clear();
		}

		public void Destroy()
		{
			this.ClearAllMsgs();
			this.controller = null;
			this.msgs = null;
			this.msgsByIds = null;
			this.observers.Clear();
			this.observers = null;
			this.msgsToProcess = null;
			this.linkedMsgs = null;
		}
	}
}
