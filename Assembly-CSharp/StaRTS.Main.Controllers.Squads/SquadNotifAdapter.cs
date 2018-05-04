using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Commands.Squads;
using StaRTS.Main.Models.Commands.Squads.Requests;
using StaRTS.Main.Models.Commands.Squads.Responses;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Squads
{
	public class SquadNotifAdapter : AbstractSquadServerAdapter
	{
		private GetSquadNotifsRequest request;

		public SquadNotifAdapter()
		{
			this.request = new GetSquadNotifsRequest();
			this.request.PlayerId = Service.CurrentPlayer.PlayerId;
			this.request.Since = 0u;
			this.request.BattleVersion = "30.0";
		}

		public void SetNotifStartDate(uint notifStartDate)
		{
			this.request.Since = notifStartDate;
		}

		protected override void Poll()
		{
			GetSquadNotifsCommand getSquadNotifsCommand = new GetSquadNotifsCommand(this.request);
			getSquadNotifsCommand.AddSuccessCallback(new AbstractCommand<GetSquadNotifsRequest, SquadNotifsResponse>.OnSuccessCallback(this.OnGetNotifs));
			Service.ServerAPI.Sync(getSquadNotifsCommand);
		}

		private void OnGetNotifs(SquadNotifsResponse response, object cookie)
		{
			base.OnPollFinished(response);
		}

		protected override void PopulateSquadMsgsReceived(object response)
		{
			SquadNotifsResponse squadNotifsResponse = (SquadNotifsResponse)response;
			List<object> notifs = squadNotifsResponse.Notifs;
			if (notifs != null)
			{
				int i = 0;
				int count = notifs.Count;
				while (i < count)
				{
					SquadMsg squadMsg = SquadMsgUtils.GenerateMessageFromNotifObject(notifs[i]);
					if (squadMsg != null)
					{
						this.list.Add(squadMsg);
						if (squadMsg.TimeSent > this.request.Since)
						{
							this.request.Since = squadMsg.TimeSent;
						}
					}
					i++;
				}
			}
		}

		public void ResetPollTimer(uint since)
		{
			if (since > this.request.Since)
			{
				this.request.Since = since;
			}
			base.ResetPollTimer();
		}
	}
}
