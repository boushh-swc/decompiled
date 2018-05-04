using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player.Store
{
	public class InventoryCrates : ISerializable
	{
		private const uint DAILY_CRATE_SYNC_BUFFER_SEC = 5u;

		private const string DAILY_CRATE_CONTEXT = "daily";

		public Dictionary<string, CrateData> Available
		{
			get;
			private set;
		}

		public uint NextDailyCrateTime
		{
			get;
			private set;
		}

		public InventoryCrates()
		{
			this.Available = new Dictionary<string, CrateData>();
		}

		public string ToJson()
		{
			return "{}";
		}

		public uint GetNextDailCrateTimeWithSyncBuffer()
		{
			return this.NextDailyCrateTime + 5u;
		}

		public void UpdateAndBadgeFromServerObject(object obj)
		{
			int count = this.Available.Count;
			this.FromObject(obj);
			int count2 = this.Available.Count;
			int delta = Math.Max(count2 - count, 1);
			GameUtils.UpdateInventoryCrateBadgeCount(delta);
		}

		public void UpdateBadgingBasedOnAvailableCrates()
		{
			ServerPlayerPrefs serverPlayerPrefs = Service.ServerPlayerPrefs;
			int num = Convert.ToInt32(serverPlayerPrefs.GetPref(ServerPref.NumInventoryCratesNotViewed));
			int availableCountAfterLastView = this.GetAvailableCountAfterLastView();
			num = availableCountAfterLastView - num;
			GameUtils.UpdateInventoryCrateBadgeCount(num);
		}

		public CrateData GetDailyCrateIfAvailable()
		{
			CrateData crateData = null;
			foreach (CrateData current in this.Available.Values)
			{
				if (current.Context == "daily" && !current.Claimed && (crateData == null || current.ReceivedTimeStamp > crateData.ReceivedTimeStamp))
				{
					crateData = current;
				}
			}
			return crateData;
		}

		public string GetNextDailyCrateId()
		{
			string result = null;
			DayOfWeek dayOfWeek = DateUtils.DateFromSeconds(this.NextDailyCrateTime).DayOfWeek;
			int num = (int)(dayOfWeek + 6);
			if (num >= 7)
			{
				num %= 6;
			}
			string[] array = GameConstants.CRATE_DAY_OF_THE_WEEK_REWARD.Split(new char[]
			{
				' '
			});
			if (array.Length >= 7)
			{
				result = array[num];
			}
			else
			{
				Service.Logger.Error("InventoryCrates.GetNextDailyCrateId Day of the week list invalid");
			}
			return result;
		}

		private int GetAvailableCountAfterLastView()
		{
			int num = 0;
			uint time = ServerTime.Time;
			SharedPlayerPrefs sharedPlayerPrefs = Service.SharedPlayerPrefs;
			uint pref = sharedPlayerPrefs.GetPref<uint>("HQInvLastViewTime");
			foreach (CrateData current in this.Available.Values)
			{
				if (current.ReceivedTimeStamp > pref && current.ExpiresTimeStamp > time)
				{
					num++;
				}
			}
			return num;
		}

		private void ParseDataObjectIntoAvailable(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				foreach (KeyValuePair<string, object> current in dictionary)
				{
					CrateData crateData = new CrateData();
					crateData.FromObject(current.Value);
					this.Available.Add(current.Key, crateData);
				}
			}
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("available"))
			{
				this.Available.Clear();
				this.ParseDataObjectIntoAvailable(dictionary["available"]);
			}
			if (dictionary.ContainsKey("nextDailyCrateTime"))
			{
				this.NextDailyCrateTime = Convert.ToUInt32(dictionary["nextDailyCrateTime"]);
			}
			if (Service.GameStateMachine.CurrentState is HomeState)
			{
				Service.InventoryCrateRewardController.ScheduleGivingNextDailyCrate();
			}
			Service.EventManager.SendEvent(EventId.CrateInventoryUpdated, null);
			return this;
		}
	}
}
