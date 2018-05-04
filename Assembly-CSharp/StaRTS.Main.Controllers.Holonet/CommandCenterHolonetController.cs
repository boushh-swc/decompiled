using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Holonet;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Holonet
{
	public class CommandCenterHolonetController : IHolonetContoller
	{
		public List<CommandCenterVO> FeaturedItems;

		private int lastTimeCCViewed;

		public HolonetControllerType ControllerType
		{
			get
			{
				return HolonetControllerType.CommandCenter;
			}
		}

		public CommandCenterHolonetController()
		{
			this.FeaturedItems = new List<CommandCenterVO>();
		}

		public void PrepareContent(int lastTimeViewed)
		{
			this.FeaturedItems.Clear();
			this.lastTimeCCViewed = lastTimeViewed;
			HolonetGetCommandCenterEntriesCommand holonetGetCommandCenterEntriesCommand = new HolonetGetCommandCenterEntriesCommand(lastTimeViewed, new PlayerIdRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			holonetGetCommandCenterEntriesCommand.AddSuccessCallback(new AbstractCommand<PlayerIdRequest, HolonetGetCommandCenterEntriesResponse>.OnSuccessCallback(this.OnSuccess));
			holonetGetCommandCenterEntriesCommand.AddFailureCallback(new AbstractCommand<PlayerIdRequest, HolonetGetCommandCenterEntriesResponse>.OnFailureCallback(this.OnFailure));
			Service.ServerAPI.Enqueue(holonetGetCommandCenterEntriesCommand);
		}

		private void OnFailure(uint status, object cookie)
		{
			HolonetController holonetController = Service.HolonetController;
			holonetController.ContentPrepared(this, 0);
		}

		private void OnSuccess(HolonetGetCommandCenterEntriesResponse response, object cookie)
		{
			int num = 0;
			int num2 = (int)cookie;
			int serverTime = (int)Service.ServerAPI.ServerTime;
			int i = 0;
			int count = response.CCVOs.Count;
			while (i < count)
			{
				CommandCenterVO commandCenterVO = response.CCVOs[i];
				if (!this.IsDuplicateFeaturedItem(commandCenterVO))
				{
					if (commandCenterVO.StartTime <= serverTime && serverTime < commandCenterVO.EndTime && AudienceConditionUtils.IsValidForClient(commandCenterVO.AudienceConditions))
					{
						this.FeaturedItems.Add(commandCenterVO);
						if (commandCenterVO.StartTime > num2)
						{
							num++;
						}
					}
				}
				else
				{
					Service.Logger.Error("Duplicate entry in commander center featured items repsonse: " + commandCenterVO.Uid);
				}
				i++;
			}
			HolonetController holonetController = Service.HolonetController;
			this.FeaturedItems.Sort(new Comparison<CommandCenterVO>(this.ComparePriority));
			InventoryCrates crates = Service.CurrentPlayer.Prizes.Crates;
			CrateData dailyCrateIfAvailable = crates.GetDailyCrateIfAvailable();
			if (dailyCrateIfAvailable != null && (ulong)dailyCrateIfAvailable.ReceivedTimeStamp > (ulong)((long)num2))
			{
				num++;
			}
			holonetController.ContentPrepared(this, num);
		}

		public bool HasNewPromoEntry()
		{
			int i = 0;
			int count = this.FeaturedItems.Count;
			while (i < count)
			{
				CommandCenterVO commandCenterVO = this.FeaturedItems[i];
				if (commandCenterVO.IsPromo && commandCenterVO.StartTime > this.lastTimeCCViewed)
				{
					return true;
				}
				i++;
			}
			return false;
		}

		private int ComparePriority(CommandCenterVO a, CommandCenterVO b)
		{
			return a.Priority - b.Priority;
		}

		private bool IsDuplicateFeaturedItem(CommandCenterVO vo)
		{
			int count = this.FeaturedItems.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.FeaturedItems[i].Uid == vo.Uid)
				{
					return true;
				}
			}
			return false;
		}
	}
}
