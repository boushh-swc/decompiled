using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Holonet;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers.Holonet;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Holonet
{
	public class TransmissionsHolonetController : IHolonetContoller
	{
		private const string TRANSM_TUT_DAILY_CRATE_REBEL_UID = "transm_tut_daily_crate_r";

		private const string TRANSM_TUT_DAILY_CRATE_EMPIRE_UID = "transm_tut_daily_crate_e";

		private TransmissionsHolonetPopupView popupView;

		private TransmissionVO battleVO;

		private int transmissionIndex;

		private List<TransmissionVO> incomingTransmissions;

		private int lastTransmissionTimeViewed;

		public HolonetControllerType ControllerType
		{
			get
			{
				return HolonetControllerType.Transmissions;
			}
		}

		public List<TransmissionVO> Transmissions
		{
			get;
			private set;
		}

		public TransmissionsHolonetController()
		{
			this.Transmissions = new List<TransmissionVO>();
			this.incomingTransmissions = new List<TransmissionVO>();
			this.battleVO = new TransmissionVO();
			this.battleVO.Type = TransmissionType.Battle;
			this.lastTransmissionTimeViewed = 0;
		}

		public void PrepareContent(int lastTimeViewed)
		{
			this.lastTransmissionTimeViewed = lastTimeViewed;
			HolonetGetMessagesCommand holonetGetMessagesCommand = new HolonetGetMessagesCommand(new PlayerIdRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			holonetGetMessagesCommand.AddSuccessCallback(new AbstractCommand<PlayerIdRequest, HolonetGetMessagesResponse>.OnSuccessCallback(this.OnGetMessagesSuccess));
			holonetGetMessagesCommand.AddFailureCallback(new AbstractCommand<PlayerIdRequest, HolonetGetMessagesResponse>.OnFailureCallback(this.OnGetMessagesFailure));
			Service.ServerAPI.Sync(holonetGetMessagesCommand);
		}

		private void OnGetMessagesFailure(uint status, object cookie)
		{
			Service.HolonetController.ContentPrepared(this, 0);
		}

		private void OnGetMessagesSuccess(HolonetGetMessagesResponse response, object cookie)
		{
			this.FinishPreparingTransmissions(response.MessageVOs);
		}

		private void FinishPreparingTransmissions(List<TransmissionVO> msgVOs)
		{
			int num = 0;
			this.Transmissions.Clear();
			this.incomingTransmissions.Clear();
			this.transmissionIndex = 0;
			SquadWarManager warManager = Service.SquadController.WarManager;
			SharedPlayerPrefs sharedPlayerPrefs = Service.SharedPlayerPrefs;
			int pref = sharedPlayerPrefs.GetPref<int>("DailyCrateTransTutorialViewed");
			int serverTime = (int)Service.ServerAPI.ServerTime;
			StaticDataController staticDataController = Service.StaticDataController;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			foreach (TransmissionVO current in staticDataController.GetAll<TransmissionVO>())
			{
				if (current.Faction == currentPlayer.Faction && current.StartTime > 0 && current.StartTime <= serverTime && serverTime < current.EndTime)
				{
					if (!this.IsDailyCrateTutorialTransmission(current) || pref != 1)
					{
						this.Transmissions.Add(current);
						if (current.StartTime > this.lastTransmissionTimeViewed)
						{
							num++;
						}
					}
				}
			}
			HolonetController holonetController = Service.HolonetController;
			msgVOs.Sort(new Comparison<TransmissionVO>(holonetController.CompareTimestamps));
			int hOLONET_EVENT_MESSAGE_MAX_COUNT = GameConstants.HOLONET_EVENT_MESSAGE_MAX_COUNT;
			int count = msgVOs.Count;
			int num2 = 0;
			while (num2 < hOLONET_EVENT_MESSAGE_MAX_COUNT && num2 < count)
			{
				TransmissionVO transmissionVO = msgVOs[num2];
				if (!this.DuplicateTransmission(transmissionVO))
				{
					this.Transmissions.Add(transmissionVO);
					if (transmissionVO.StartTime > this.lastTransmissionTimeViewed)
					{
						num++;
						if (this.IsWarTransmission(transmissionVO) && !warManager.IsTimeWithinCurrentSquadWarPhase(transmissionVO.StartTime))
						{
							num--;
						}
					}
				}
				else
				{
					Service.Logger.Error("Duplicate entry in transmission event messages repsonse: " + transmissionVO.Uid);
				}
				num2++;
			}
			this.Transmissions.Sort(new Comparison<TransmissionVO>(holonetController.CompareTimestamps));
			int pref2 = sharedPlayerPrefs.GetPref<int>("HighestViewedSquadLvlUP");
			int hOLONET_MAX_INCOMING_TRANSMISSIONS = GameConstants.HOLONET_MAX_INCOMING_TRANSMISSIONS;
			int count2 = this.Transmissions.Count;
			int num3 = 0;
			TransmissionVO transmissionVO2 = null;
			int num4 = 0;
			while (num3 < hOLONET_MAX_INCOMING_TRANSMISSIONS && num4 < count2)
			{
				TransmissionVO transmissionVO3 = this.Transmissions[num4];
				if (this.IsIncomingTransmission(transmissionVO3))
				{
					if (!this.IsWarTransmission(transmissionVO3) || warManager.IsTimeWithinCurrentSquadWarPhase(transmissionVO3.StartTime))
					{
						if (this.IsSquadLevelUpTransmission(transmissionVO3))
						{
							if (!Service.PerkManager.HasPlayerSeenPerkTutorial())
							{
								goto IL_314;
							}
							bool flag = transmissionVO2 == null || transmissionVO3.SquadLevel > transmissionVO2.SquadLevel;
							if (!flag || pref2 >= transmissionVO3.SquadLevel)
							{
								goto IL_314;
							}
							if (transmissionVO2 != null)
							{
								num3--;
								this.incomingTransmissions.Remove(transmissionVO2);
							}
							transmissionVO2 = transmissionVO3;
						}
						if (pref == 0)
						{
							if (this.IsDailyCrateTransmission(transmissionVO3))
							{
								goto IL_314;
							}
							if (this.IsDailyCrateTutorialTransmission(transmissionVO3))
							{
								sharedPlayerPrefs.SetPref("DailyCrateTransTutorialViewed", "1");
							}
						}
						num3++;
						this.incomingTransmissions.Add(transmissionVO3);
					}
				}
				IL_314:
				num4++;
			}
			this.incomingTransmissions.Sort(new Comparison<TransmissionVO>(this.CompareIncommingPriorites));
			holonetController.ContentPrepared(this, num);
		}

		private bool IsDailyCrateTransmission(TransmissionVO vo)
		{
			return vo.Type == TransmissionType.DailyCrateReward;
		}

		private bool IsDailyCrateTutorialTransmission(TransmissionVO vo)
		{
			return vo.Uid == "transm_tut_daily_crate_r" || vo.Uid == "transm_tut_daily_crate_e";
		}

		private bool IsSquadLevelUpTransmission(TransmissionVO vo)
		{
			return vo.Type == TransmissionType.GuildLevelUp;
		}

		private bool IsWarTransmission(TransmissionVO vo)
		{
			return vo.Type == TransmissionType.WarPreparation || vo.Type == TransmissionType.WarStart || vo.Type == TransmissionType.WarEnded;
		}

		public void InitTransmissionsTest(List<TransmissionVO> testTransmissions)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			StaticDataController staticDataController = Service.StaticDataController;
			this.incomingTransmissions.Clear();
			if (testTransmissions != null)
			{
				foreach (TransmissionVO current in testTransmissions)
				{
					if (current.Faction == currentPlayer.Faction)
					{
						this.incomingTransmissions.Add(current);
					}
				}
			}
			foreach (TransmissionVO current2 in staticDataController.GetAll<TransmissionVO>())
			{
				if (current2.Faction == currentPlayer.Faction)
				{
					this.incomingTransmissions.Add(current2);
				}
			}
			HolonetController holonetController = Service.HolonetController;
			holonetController.SetTabCount(this, this.incomingTransmissions.Count);
		}

		private bool DuplicateTransmission(TransmissionVO vo)
		{
			int count = this.Transmissions.Count;
			for (int i = 0; i < count; i++)
			{
				if (this.Transmissions[i].Uid == vo.Uid)
				{
					return true;
				}
			}
			return false;
		}

		private int CompareIncommingPriorites(TransmissionVO a, TransmissionVO b)
		{
			return b.Priority - a.Priority;
		}

		private bool IsIncomingTransmission(TransmissionVO vo)
		{
			return this.IsDailyCrateTutorialTransmission(vo) || (vo.StartTime > this.lastTransmissionTimeViewed && (vo.Type == TransmissionType.Conflict || vo.Type == TransmissionType.WarPreparation || vo.Type == TransmissionType.WarStart || vo.Type == TransmissionType.WarEnded || vo.Type == TransmissionType.Generic || vo.Type == TransmissionType.GuildLevelUp || vo.Type == TransmissionType.DailyCrateReward));
		}

		public int GetInCommingTransmissionCount()
		{
			return this.incomingTransmissions.Count;
		}

		public bool HasNewBattles()
		{
			return this.battleVO.AttackerData != null && this.battleVO.AttackerData.Count > 0;
		}

		public void InitBattlesTransmission(List<BattleEntry> battles)
		{
			this.battleVO.InitBattleData(battles);
			if (this.HasNewBattles())
			{
				this.incomingTransmissions.Add(this.battleVO);
				this.incomingTransmissions.Sort(new Comparison<TransmissionVO>(this.CompareIncommingPriorites));
				int hOLONET_MAX_INCOMING_TRANSMISSIONS = GameConstants.HOLONET_MAX_INCOMING_TRANSMISSIONS;
				if (this.incomingTransmissions.Count > hOLONET_MAX_INCOMING_TRANSMISSIONS)
				{
					this.incomingTransmissions.RemoveAt(this.incomingTransmissions.Count - 1);
				}
			}
		}

		public void UpdateIncomingTransmission(int index)
		{
			if (index >= 1)
			{
				this.transmissionIndex = index - 1;
				if (this.transmissionIndex >= this.incomingTransmissions.Count)
				{
					this.transmissionIndex = this.incomingTransmissions.Count - 1;
				}
				else if (this.transmissionIndex < 0)
				{
					this.transmissionIndex = 0;
				}
				this.popupView.RefreshView(this.incomingTransmissions[this.transmissionIndex]);
			}
		}

		public void DismissIncomingTransmission(int index)
		{
			if (index >= 1)
			{
				this.transmissionIndex = index - 1;
				if (this.transmissionIndex < this.incomingTransmissions.Count)
				{
					this.incomingTransmissions.RemoveAt(this.transmissionIndex);
					if (this.transmissionIndex > this.incomingTransmissions.Count)
					{
						this.transmissionIndex = this.incomingTransmissions.Count - 1;
					}
				}
				if (this.transmissionIndex < this.incomingTransmissions.Count)
				{
					this.popupView.RefreshView(this.incomingTransmissions[this.transmissionIndex]);
				}
			}
		}

		public bool IsTransmissionPopupOpened()
		{
			return this.popupView != null;
		}

		public void OnTransmissionPopupClosed()
		{
			if (this.popupView != null)
			{
				this.battleVO.ResetAttackerData();
				this.popupView = null;
				this.incomingTransmissions.Clear();
				this.transmissionIndex = 0;
				Service.HolonetController.EnableAllHolonetTabButtons();
			}
		}

		public void OnTransmissionPopupIntialized(TransmissionsHolonetPopupView popup)
		{
			Service.HolonetController.DisableAllHolonetTabButtons();
			this.popupView = popup;
			this.popupView.SetMaxTransmissionIndex(this.incomingTransmissions.Count);
			if (this.HasNewBattles())
			{
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				this.battleVO.CharacterID = GameUtils.GetTransmissionHoloId(currentPlayer.Faction, currentPlayer.Planet.Uid);
				this.battleVO.Faction = currentPlayer.Faction;
			}
			this.popupView.RefreshView(this.incomingTransmissions[this.transmissionIndex]);
		}
	}
}
