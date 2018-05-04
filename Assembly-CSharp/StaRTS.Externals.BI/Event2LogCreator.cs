using StaRTS.Externals.EnvironmentManager;
using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Player;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StaRTS.Externals.BI
{
	public class Event2LogCreator : ILogCreator
	{
		private const string ID = "id";

		private const string AUTHORIZATION = "Authorization";

		private const string EXPECT = "Expect";

		private const string CONTENT_TYPE = "Content-Type";

		private const string APPLICATION_JSON = "application/json";

		private const string APPLICATION_NAME = "n";

		private const string APPLICATION_PLATFORM = "p";

		private const string APPLICATION_VERSION = "v";

		private const string APPLICATION_LOGGING_LIB_VERSION = "llv";

		private const string APPLICATION_SUBSECTION = "app";

		private const string APPLICATION_LOGGING_LIB_VERSION_DEFAULT = "2.0";

		private const string SESSION_LOCALIZATION = "lo";

		private const string SESSION_SUBSECTION = "session";

		private const string GUEST_PLAYER_ID = "ply";

		private const string GUEST_SUBSECTION = "guest";

		private const string DEVICE_MODEL = "mo";

		private const string DEVICE_MACHINE = "m";

		private const string DEVICE_OS_VERSION = "ov";

		private const string DEVICE_ID_TYPE = "idt";

		private const string DEVICE_USER_AGENT = "ua";

		private const string DEVICE_OS = "o";

		private const string DEVICE_SUBSECTION = "device";

		private const string ACTION_TIER1 = "t1";

		private const string ACTION_TIER2 = "t2";

		private const string ACTION_TIER3 = "t3";

		private const string ACTION_TIER4 = "t4";

		private const string ACTION_TIER5 = "t5";

		private const string ACTION_TIER6 = "t6";

		private const string EVENT_PROPERTIES = "event_prop";

		private const string EVENT_SOURCE = "s";

		private const string EVENT_SOURCE_MOBILE_CLIENT = "mc";

		private const string EVENT_ONLINE_FLAG = "of";

		private const string EVENT_LOG_TIMESTAMP = "lts";

		private const string EVENT_TIMESTAMP = "ts";

		private const string EVENT_SESSION_SEQUENCE_NUM = "sn";

		private const string EVENT_NAME = "n";

		private const string UNKNOWN = "u";

		private const string EVENT = "event";

		private const string EVENTS = "events";

		private const string ACTION = "action";

		private const string ACTION_TIMING = "timing";

		private const string TIMING_CONTEXT = "c";

		private const string TIMING_LOCATION = "l";

		private const string TIMING_ELAPSED_TIME_MS = "etms";

		private const string TIMING_PATH_NAME = "pn";

		private const string TIMING_RESULT = "r";

		private int logSequenceNumber;

		private Dictionary<string, string> headers;

		private Dictionary<string, string> app;

		private Dictionary<string, string> session;

		private Dictionary<string, string> device;

		private Serializer guest;

		private Dictionary<string, string> paramDict;

		private bool debug;

		private string primaryURL;

		private string secondaryNoProxyURL;

		public Event2LogCreator(string primaryURL, string secondaryNoProxyURL)
		{
			this.primaryURL = primaryURL;
			this.secondaryNoProxyURL = secondaryNoProxyURL;
			this.logSequenceNumber = 0;
			this.headers = new Dictionary<string, string>();
			this.debug = false;
		}

		public void SetURL(string primaryURL, string secondaryNoProxyURL)
		{
			this.primaryURL = primaryURL;
			this.secondaryNoProxyURL = secondaryNoProxyURL;
		}

		private void SetupSantardizedData()
		{
			this.headers.Clear();
			this.headers["Authorization"] = "FD B276DCA8-9CD5-4493-85D6-75D2E500BCC9:19042B0A0628EF36039165C24A240F78F6EDBDFB8BD86BA3";
			this.headers["Expect"] = string.Empty;
			this.headers["Content-Type"] = "application/json";
			this.guest = Serializer.Start();
			if (Service.CurrentPlayer != null)
			{
				this.guest.AddString("ply", Service.CurrentPlayer.PlayerId);
			}
			else
			{
				string @string = PlayerPrefs.GetString("prefPlayerId", "u");
				this.guest.AddString("ply", @string);
			}
			this.guest.End();
			if (Service.EnvironmentController == null || Service.ServerAPI == null)
			{
				return;
			}
			EnvironmentController environmentController = Service.EnvironmentController;
			this.app = new Dictionary<string, string>();
			this.app.Add("p", environmentController.GetPlatform());
			this.app.Add("v", "5.2.0.10309");
			this.app.Add("llv", "2.0");
			this.app.Add("n", "starts");
			this.session = new Dictionary<string, string>();
			this.session.Add("lo", environmentController.GetLocale());
			this.session.Add("id", Service.ServerAPI.SessionId);
			this.device = new Dictionary<string, string>();
			this.device.Add("mo", environmentController.GetModel());
			this.device.Add("ov", environmentController.GetOSVersion());
			this.device.Add("idt", environmentController.GetDeviceIdType());
			this.device.Add("id", environmentController.GetDeviceIDForEvent2());
			this.device.Add("ua", SystemInfo.operatingSystem);
			this.device.Add("m", environmentController.GetMachine());
			this.device.Add("o", environmentController.GetOS());
		}

		private void SetStandardDataForAction(out string tier1, out string tier2, out string tier6)
		{
			string arg = "u";
			string text = "u";
			string str = string.Empty;
			int num = -1;
			string str2 = string.Empty;
			if (Service.CurrentPlayer != null)
			{
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				arg = currentPlayer.Faction.ToString();
				text = currentPlayer.PlanetId;
				if (currentPlayer.Map != null)
				{
					num = currentPlayer.Map.FindHighestHqLevel();
				}
			}
			if (Service.SquadController != null)
			{
				SquadController squadController = Service.SquadController;
				str = ((squadController.StateManager.GetCurrentSquad() == null) ? string.Empty : squadController.StateManager.GetCurrentSquad().SquadID);
			}
			if (Service.BattleController != null)
			{
				CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
				if (currentBattle != null)
				{
					str2 = currentBattle.BattleUid;
				}
			}
			tier1 = arg + "|" + num;
			tier2 = text;
			tier6 = str + "|" + str2;
		}

		private void SerializeActionEventProperties(Serializer eventProperties, string tier1, string tier2, string tier3, string tier4, string tier5, string tier6)
		{
			eventProperties.AddString("t1", tier1);
			eventProperties.AddString("t2", tier2);
			eventProperties.AddString("t3", tier3);
			eventProperties.AddString("t4", tier4);
			eventProperties.AddString("t5", tier5);
			eventProperties.AddString("t6", tier6);
		}

		public string GetValueFromLog(BILog log, string key)
		{
			if (this.paramDict.ContainsKey(key))
			{
				return WWW.UnEscapeURL(this.paramDict[key]);
			}
			return "u";
		}

		public BILogData CreateWWWDataFromBILog(BILog log)
		{
			uint serverTime = Service.ServerAPI.ServerTime;
			this.paramDict = log.GetParamDict();
			string text = (!this.paramDict.ContainsKey("tag")) ? "u" : WWW.UnEscapeURL(this.paramDict["tag"]);
			string empty = string.Empty;
			string tier = string.Empty;
			string tier2 = string.Empty;
			string tier3 = string.Empty;
			string tier4 = string.Empty;
			string tier5 = string.Empty;
			Serializer serializer = Serializer.Start();
			string text2 = text;
			if (text2 != null)
			{
				if (Event2LogCreator.<>f__switch$map6 == null)
				{
					Event2LogCreator.<>f__switch$map6 = new Dictionary<string, int>(11)
					{
						{
							"authorization",
							0
						},
						{
							"clicked_link",
							1
						},
						{
							"user_info",
							2
						},
						{
							"player_info",
							3
						},
						{
							"device_info",
							4
						},
						{
							"game_action",
							5
						},
						{
							"step_timing",
							6
						},
						{
							"geo",
							7
						},
						{
							"performance",
							8
						},
						{
							"error",
							9
						},
						{
							"send_message",
							10
						}
					};
				}
				int num;
				if (Event2LogCreator.<>f__switch$map6.TryGetValue(text2, out num))
				{
					string val;
					switch (num)
					{
					case 0:
						val = "action";
						this.SetStandardDataForAction(out empty, out tier, out tier5);
						tier2 = "authorization";
						tier3 = this.GetValueFromLog(log, "type");
						tier4 = this.GetValueFromLog(log, "step");
						this.SerializeActionEventProperties(serializer, empty, tier, tier2, tier3, tier4, tier5);
						break;
					case 1:
						val = "action";
						this.SetStandardDataForAction(out empty, out tier, out tier5);
						tier = this.GetValueFromLog(log, "app");
						tier2 = "clicked_link";
						tier3 = this.GetValueFromLog(log, "is_new_user");
						tier4 = this.GetValueFromLog(log, "log_app");
						tier5 = this.GetValueFromLog(log, "tracking_code");
						this.SerializeActionEventProperties(serializer, empty, tier, tier2, tier3, tier4, tier5);
						break;
					case 2:
						this.LogIgnoredEvent("user_info");
						return null;
					case 3:
						this.LogIgnoredEvent("player_info");
						return null;
					case 4:
						this.LogIgnoredEvent("device_info");
						return null;
					case 5:
						val = "action";
						this.SetStandardDataForAction(out empty, out tier, out tier5);
						tier2 = this.GetValueFromLog(log, "context");
						tier3 = this.GetValueFromLog(log, "action");
						tier4 = this.GetValueFromLog(log, "message");
						this.SerializeActionEventProperties(serializer, empty, tier, tier2, tier3, tier4, tier5);
						break;
					case 6:
					{
						val = "timing";
						string valueFromLog = this.GetValueFromLog(log, "context");
						serializer.AddString("c", valueFromLog);
						string valueFromLog2 = this.GetValueFromLog(log, "location");
						serializer.AddString("l", valueFromLog2);
						string text3 = this.GetValueFromLog(log, "path_name");
						if (string.IsNullOrEmpty(text3))
						{
							text3 = "u";
						}
						serializer.AddString("pn", text3);
						string valueFromLog3 = this.GetValueFromLog(log, "elapsed_time_ms");
						int num2 = 0;
						int.TryParse(valueFromLog3, out num2);
						if (num2 < 1)
						{
							this.LogIgnoredEvent("step_timing Ignoring Step timing event with no elapsed time.");
							return null;
						}
						serializer.Add<int>("etms", num2);
						serializer.Add<int>("r", 1);
						break;
					}
					case 7:
						this.LogIgnoredEvent("geo");
						return null;
					case 8:
						val = "action";
						this.SetStandardDataForAction(out empty, out tier, out tier5);
						tier = this.GetValueFromLog(log, "time_since_start");
						tier2 = "performance";
						tier3 = this.GetValueFromLog(log, "fps");
						tier4 = this.GetValueFromLog(log, "memory_used");
						tier5 = this.GetValueFromLog(log, "display_state");
						this.SerializeActionEventProperties(serializer, empty, tier, tier2, tier3, tier4, tier5);
						break;
					case 9:
						val = "action";
						this.SetStandardDataForAction(out empty, out tier, out tier5);
						tier2 = "error";
						tier3 = this.GetValueFromLog(log, "context");
						tier4 = this.GetValueFromLog(log, "reason");
						tier5 = this.GetValueFromLog(log, "message");
						this.SerializeActionEventProperties(serializer, empty, tier, tier2, tier3, tier4, tier5);
						break;
					case 10:
						val = "action";
						this.SetStandardDataForAction(out empty, out tier, out tier5);
						tier = this.GetValueFromLog(log, "tracking_code");
						tier2 = "send_message";
						tier3 = this.GetValueFromLog(log, "send_timestamp");
						tier4 = this.GetValueFromLog(log, "target_user_id");
						tier5 = this.GetValueFromLog(log, "num_sent");
						this.SerializeActionEventProperties(serializer, empty, tier, tier2, tier3, tier4, tier5);
						break;
					default:
						goto IL_4AD;
					}
					if (this.app == null)
					{
						this.SetupSantardizedData();
					}
					Serializer serializer2 = Serializer.Start();
					serializer2.AddDictionary<string>("app", this.app);
					serializer2.AddDictionary<string>("session", this.session);
					serializer2.Add<string>("guest", this.guest.ToString());
					serializer2.AddDictionary<string>("device", this.device);
					Serializer serializer3 = Serializer.Start();
					Serializer serializer4 = Serializer.Start();
					serializer4.Add<string>("event_prop", serializer.End().ToString());
					serializer4.AddString("s", "mc");
					serializer4.AddString("of", "1");
					serializer4.Add<uint>("lts", serverTime);
					serializer4.Add<uint>("ts", serverTime);
					serializer4.AddString("id", Guid.NewGuid().ToString());
					serializer4.Add<int>("sn", this.logSequenceNumber);
					this.logSequenceNumber++;
					serializer4.AddString("n", val);
					serializer3.Add<string>("event", serializer4.End().ToString());
					string val2 = "[" + serializer3.End().ToString() + "]";
					serializer2.Add<string>("events", val2);
					serializer2.End();
					byte[] bytes = Encoding.UTF8.GetBytes(serializer2.ToString());
					string url = (!log.UseSecondaryUrl) ? this.primaryURL : this.secondaryNoProxyURL;
					return new BILogData
					{
						url = url,
						postData = bytes,
						headers = this.headers
					};
				}
			}
			IL_4AD:
			this.LogIgnoredEvent(text);
			return null;
		}

		private void LogIgnoredEvent(string eventType)
		{
		}
	}
}
