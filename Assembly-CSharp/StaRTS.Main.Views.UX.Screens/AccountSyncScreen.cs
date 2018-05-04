using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player.Account.External;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Views.UserInput;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens
{
	public class AccountSyncScreen : ClosableScreen
	{
		private const string LABEL_TITLE = "LabelTitle";

		private const string LABEL_BODY = "LabelBody";

		private const string LABEL_ACCOUNT_TITLE = "AccountTitle";

		private const string LABEL_LAST_SYNCED = "LabelLastSynced";

		private const string GROUP_ONE_ACCOUNT = "OneAccount";

		private const string LABEL_ONE_CALL_SIGN = "OneLabelCallSign";

		private const string LABEL_ONE_HQ_LEVEL = "OneLabelHQLevel";

		private const string LABEL_ONE_MEDALS = "OneLabelMedals";

		private const string LABEL_ONE_FACTION = "OneLabelFaction";

		private const string LABEL_ONE_SQUAD = "OneLabelSquad";

		private const string GROUP_TWO_ACCOUNTS = "TwoAccounts";

		private const string LABEL_TWO_PRIMARY_TITLE = "TwoAccountsLabelTitlePrimary";

		private const string LABEL_TWO_PRIMARY_CALL_SIGN = "TwoAccountsLabelCallSignPrimary";

		private const string LABEL_TWO_PRIMARY_HQ_LEVEL = "TwoAccountsLabelHQLevelPrimary";

		private const string LABEL_TWO_PRIMARY_MEDALS = "TwoAccountsLabelMedalsPrimary";

		private const string LABEL_TWO_PRIMARY_FACTION = "TwoAccountsLabelFactionPrimary";

		private const string LABEL_TWO_PRIMARY_SQUAD = "TwoAccountsLabelSquadPrimary";

		private const string LABEL_TWO_SECONDARY_TITLE = "TwoAccountsLabelTitleSecondary";

		private const string LABEL_TWO_SECONDARY_CALL_SIGN = "TwoAccountsLabelCallSignSecondary";

		private const string LABEL_TWO_SECONDARY_HQ_LEVEL = "TwoAccountsLabelHQLevelSecondary";

		private const string LABEL_TWO_SECONDARY_MEDALS = "TwoAccountsLabelMedalsSecondary";

		private const string LABEL_TWO_SECONDARY_FACTION = "TwoAccountsLabelFactionSecondary";

		private const string LABEL_TWO_SECONDARY_SQUAD = "TwoAccountsLabelSquadSecondary";

		private const string BUTTON_OK = "BtnOk";

		private const string BUTTON_LABEL_OK = "LabelBtnOk";

		private const string BUTTON_CANCEL = "BtnCancel";

		private const string BUTTON_LABEL_CANCEL = "LabelBtnCancel";

		private const string BUTTON_LOAD = "BtnLoad";

		private const string BUTTON_LABEL_LOAD = "LabelBtnLoad";

		private const string BUTTON_CONNECT_NEW = "BtnConnectNew";

		private const string BUTTON_LABEL_CONNECT_NEW = "LabelConnectNew";

		private const string BUTTON_CONNECT_NEW_CONFIRM = "BtnConnectNewConfirm";

		private const string BUTTON_LABEL_CONNECT_NEW_CONFIRM = "LabelConnectNewConfirm";

		private UXLabel labelAccountTitle;

		private UXLabel labelBody;

		private UXLabel labelLastSynced;

		private UXLabel labelTitle;

		private UXElement groupOneAccount;

		private UXElement groupTwoAccounts;

		private UXButton buttonOk;

		private UXButton buttonCancel;

		private UXButton buttonLoad;

		private UXButton buttonConnectNew;

		private UXButton buttonConnectNewConfirm;

		private RegisterExternalAccountCommand command;

		private string accountProvider;

		private string overrideDescriptionText;

		private AccountSyncScreen() : base("gui_account_sync")
		{
		}

		public static AccountSyncScreen CreateSyncConflictScreen(RegisterExternalAccountCommand command)
		{
			AccountSyncScreen accountSyncScreen = new AccountSyncScreen();
			accountSyncScreen.command = command;
			accountSyncScreen.accountProvider = accountSyncScreen.GetAccountProviderString(command.RequestArgs.Provider);
			return accountSyncScreen;
		}

		public void OverrideDescription(string description)
		{
			if (this.labelBody == null)
			{
				this.overrideDescriptionText = description;
			}
			else
			{
				this.labelBody.Text = description;
			}
		}

		protected override void OnScreenLoaded()
		{
			this.labelAccountTitle = base.GetElement<UXLabel>("AccountTitle");
			this.labelBody = base.GetElement<UXLabel>("LabelBody");
			this.labelLastSynced = base.GetElement<UXLabel>("LabelLastSynced");
			this.labelTitle = base.GetElement<UXLabel>("LabelTitle");
			this.groupOneAccount = base.GetElement<UXElement>("OneAccount");
			this.groupTwoAccounts = base.GetElement<UXElement>("TwoAccounts");
			base.GetElement<UXLabel>("TwoAccountsLabelTitlePrimary").Text = this.lang.Get("ACCOUNT_SYNC_PRIMARY_ACCOUNT", new object[0]);
			base.GetElement<UXLabel>("TwoAccountsLabelTitleSecondary").Text = this.lang.Get("ACCOUNT_SYNC_SECONDARY_ACCOUNT", new object[0]);
			this.InitButtons();
			this.CloseButton.Visible = false;
			this.allowClose = true;
		}

		protected override void InitButtons()
		{
			base.InitButtons();
			this.buttonLoad = base.GetElement<UXButton>("BtnLoad");
			this.buttonOk = base.GetElement<UXButton>("BtnOk");
			this.buttonCancel = base.GetElement<UXButton>("BtnCancel");
			this.buttonConnectNew = base.GetElement<UXButton>("BtnConnectNew");
			this.buttonConnectNewConfirm = base.GetElement<UXButton>("BtnConnectNewConfirm");
			UserInputInhibitor userInputInhibitor = Service.UserInputInhibitor;
			if (userInputInhibitor != null)
			{
				userInputInhibitor.AlwaysAllowElement(this.CloseButton);
				userInputInhibitor.AlwaysAllowElement(this.buttonLoad);
				userInputInhibitor.AlwaysAllowElement(this.buttonOk);
				userInputInhibitor.AlwaysAllowElement(this.buttonCancel);
				userInputInhibitor.AlwaysAllowElement(this.buttonConnectNew);
				userInputInhibitor.AlwaysAllowElement(this.buttonConnectNewConfirm);
			}
			base.GetElement<UXLabel>("LabelBtnOk").Text = this.lang.Get("s_Ok", new object[0]);
			base.GetElement<UXLabel>("LabelBtnLoad").Text = this.lang.Get("ACCOUNT_SYNC_LOAD", new object[0]);
			base.GetElement<UXLabel>("LabelBtnCancel").Text = this.lang.Get("s_Cancel", new object[0]);
			base.GetElement<UXLabel>("LabelConnectNew").Text = this.lang.Get("ACCOUNT_SYNC_CONNECT_NEW_GAME", new object[0]);
			base.GetElement<UXLabel>("LabelConnectNewConfirm").Text = this.lang.Get("ACCOUNT_SYNC_CONNECT_NEW_GAME", new object[0]);
			this.ShowSyncConflict();
		}

		private void PopulateCurrentPlayerInfo()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			bool flag = currentPlayer.NumIdentities > 1;
			this.groupOneAccount.Visible = !flag;
			this.groupTwoAccounts.Visible = flag;
			AccountSyncAccountType accountSyncAccountType;
			if (flag)
			{
				Service.PlayerIdentityController.GetOtherPlayerIdentity(new PlayerIdentityController.GetOtherPlayerIdentityCallback(this.OnOtherPlayerIdentityFetched));
				accountSyncAccountType = this.GetTypeForMultipleAccountId(currentPlayer.PlayerId);
			}
			else
			{
				accountSyncAccountType = AccountSyncAccountType.SingleAccount;
			}
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			this.PopulatePlayerInfo(accountSyncAccountType, currentPlayer.PlayerName, currentPlayer.Map.FindHighestHqLevel(), currentPlayer.PlayerMedals, currentPlayer.Faction, (currentSquad == null) ? null : currentSquad.SquadName);
			if (accountSyncAccountType == AccountSyncAccountType.MultipleAccountsPrimary || accountSyncAccountType == AccountSyncAccountType.MultipleAccountsSecondary)
			{
				AccountSyncAccountType type = (accountSyncAccountType != AccountSyncAccountType.MultipleAccountsPrimary) ? AccountSyncAccountType.MultipleAccountsPrimary : AccountSyncAccountType.MultipleAccountsSecondary;
				this.ShowPlayerInfoLoading(type);
			}
		}

		private void OnOtherPlayerIdentityFetched(PlayerIdentityInfo info)
		{
			AccountSyncAccountType typeForMultipleAccountId = this.GetTypeForMultipleAccountId(info.PlayerId);
			this.PopulatePlayerInfo(typeForMultipleAccountId, info.PlayerName, info.HQLevel, info.Medals, info.Faction, info.SquadName);
		}

		private AccountSyncAccountType GetTypeForMultipleAccountId(string playerId)
		{
			return (!Service.PlayerIdentityController.IsFirstIdentity(playerId)) ? AccountSyncAccountType.MultipleAccountsSecondary : AccountSyncAccountType.MultipleAccountsPrimary;
		}

		private void PopulatePlayerInfo(AccountSyncAccountType type, string playerName, int hqLevel, int medals, FactionType faction, string squadName)
		{
			UXLabel uXLabel;
			UXLabel uXLabel2;
			UXLabel uXLabel3;
			UXLabel uXLabel4;
			UXLabel uXLabel5;
			this.UpdateAndGetLabels(type, out uXLabel, out uXLabel2, out uXLabel3, out uXLabel4, out uXLabel5);
			uXLabel.Visible = true;
			uXLabel2.Visible = true;
			uXLabel3.Visible = true;
			uXLabel4.Visible = true;
			uXLabel5.Visible = true;
			uXLabel.Text = this.lang.Get("ACCOUNT_CONFLICT_CALL_SIGN", new object[]
			{
				playerName
			});
			uXLabel2.Text = this.lang.Get("ACCOUNT_CONFLICT_HQ_LEVEL", new object[]
			{
				hqLevel
			});
			uXLabel3.Text = this.lang.Get("ACCOUNT_CONFLICT_PLAYER_RATING", new object[]
			{
				medals
			});
			uXLabel4.Text = this.lang.Get("ACCOUNT_CONFLICT_FACTION", new object[]
			{
				StringUtils.ParseEnum<FactionType>(faction.ToString())
			});
			if (string.IsNullOrEmpty(squadName))
			{
				squadName = this.lang.Get("general_none", new object[0]);
			}
			uXLabel5.Text = this.lang.Get("ACCOUNT_CONFLICT_GUILD_NAME", new object[]
			{
				squadName
			});
		}

		private void ShowPlayerInfoLoading(AccountSyncAccountType type)
		{
			UXLabel uXLabel;
			UXLabel uXLabel2;
			UXLabel uXLabel3;
			UXLabel uXLabel4;
			UXLabel uXLabel5;
			this.UpdateAndGetLabels(type, out uXLabel, out uXLabel2, out uXLabel3, out uXLabel4, out uXLabel5);
			uXLabel.Text = this.lang.Get("s_Loading", new object[0]);
			uXLabel.Visible = true;
			uXLabel2.Visible = false;
			uXLabel3.Visible = false;
			uXLabel4.Visible = false;
			uXLabel5.Visible = false;
		}

		private void UpdateAndGetLabels(AccountSyncAccountType type, out UXLabel callsignLabel, out UXLabel hqLevelLabel, out UXLabel medalsLabel, out UXLabel factionLabel, out UXLabel squadLabel)
		{
			callsignLabel = null;
			hqLevelLabel = null;
			medalsLabel = null;
			factionLabel = null;
			squadLabel = null;
			if (type != AccountSyncAccountType.SingleAccount)
			{
				if (type != AccountSyncAccountType.MultipleAccountsPrimary)
				{
					if (type == AccountSyncAccountType.MultipleAccountsSecondary)
					{
						this.groupOneAccount.Visible = false;
						this.groupTwoAccounts.Visible = true;
						callsignLabel = base.GetElement<UXLabel>("TwoAccountsLabelCallSignSecondary");
						hqLevelLabel = base.GetElement<UXLabel>("TwoAccountsLabelHQLevelSecondary");
						medalsLabel = base.GetElement<UXLabel>("TwoAccountsLabelMedalsSecondary");
						factionLabel = base.GetElement<UXLabel>("TwoAccountsLabelFactionSecondary");
						squadLabel = base.GetElement<UXLabel>("TwoAccountsLabelSquadSecondary");
					}
				}
				else
				{
					this.groupOneAccount.Visible = false;
					this.groupTwoAccounts.Visible = true;
					callsignLabel = base.GetElement<UXLabel>("TwoAccountsLabelCallSignPrimary");
					hqLevelLabel = base.GetElement<UXLabel>("TwoAccountsLabelHQLevelPrimary");
					medalsLabel = base.GetElement<UXLabel>("TwoAccountsLabelMedalsPrimary");
					factionLabel = base.GetElement<UXLabel>("TwoAccountsLabelFactionPrimary");
					squadLabel = base.GetElement<UXLabel>("TwoAccountsLabelSquadPrimary");
				}
			}
			else
			{
				this.groupOneAccount.Visible = true;
				this.groupTwoAccounts.Visible = false;
				callsignLabel = base.GetElement<UXLabel>("OneLabelCallSign");
				hqLevelLabel = base.GetElement<UXLabel>("OneLabelHQLevel");
				medalsLabel = base.GetElement<UXLabel>("OneLabelMedals");
				factionLabel = base.GetElement<UXLabel>("OneLabelFaction");
				squadLabel = base.GetElement<UXLabel>("OneLabelSquad");
			}
		}

		private void ShowSyncConfirmation()
		{
			this.labelTitle.Text = this.lang.Get("ACCOUNT_SYNC", new object[0]);
			this.labelBody.Text = this.lang.Get("ACCOUNT_SYNC_SUCCESS", new object[]
			{
				this.accountProvider
			});
			this.labelAccountTitle.Text = this.lang.Get("ACCOUNT_SYNC_NEW_SYNCED", new object[]
			{
				this.accountProvider
			});
			this.labelLastSynced.Text = this.lang.Get("ACCOUNT_SYNC_LAST_DATE", new object[]
			{
				DateTime.Now.Month,
				DateTime.Now.Day,
				DateTime.Now.Year
			});
			this.labelLastSynced.Visible = true;
			this.buttonOk.Visible = true;
			this.buttonOk.OnClicked = new UXButtonClickedDelegate(this.OnCloseButtonClicked);
			this.buttonLoad.Visible = false;
			this.buttonCancel.Visible = false;
			this.buttonConnectNew.Visible = false;
			this.buttonConnectNewConfirm.Visible = false;
		}

		private void ShowSyncConflict()
		{
			this.labelTitle.Text = this.lang.Get("ACCOUNT_SYNC", new object[0]);
			this.labelBody.Text = ((!string.IsNullOrEmpty(this.overrideDescriptionText)) ? this.overrideDescriptionText : this.lang.Get("ACCOUNT_SYNC_CONFLICT", new object[]
			{
				this.accountProvider,
				this.accountProvider
			}));
			this.labelAccountTitle.Text = this.lang.Get("ACCOUNT_SYNC_EXISTING_SYNCED", new object[]
			{
				this.accountProvider
			});
			int lastSyncedTimeStamp = this.command.ResponseResult.LastSyncedTimeStamp;
			if (lastSyncedTimeStamp > 0)
			{
				DateTime dateTime = DateUtils.DateFromSeconds(lastSyncedTimeStamp);
				this.labelLastSynced.Text = this.lang.Get("ACCOUNT_SYNC_LAST_DATE", new object[]
				{
					dateTime.Month,
					dateTime.Day,
					dateTime.Year
				});
				this.labelLastSynced.Visible = true;
			}
			else
			{
				this.labelLastSynced.Visible = false;
			}
			this.buttonLoad.Visible = true;
			this.buttonLoad.OnClicked = new UXButtonClickedDelegate(this.OnSyncConflictLoadExistingClicked);
			this.buttonConnectNew.Visible = true;
			this.buttonConnectNew.OnClicked = new UXButtonClickedDelegate(this.OnSyncConflictConnectNewClicked);
			this.buttonOk.Visible = false;
			this.buttonCancel.Visible = false;
			this.buttonConnectNewConfirm.Visible = false;
			PlayerIdentityInfo playerIdentityInfo = null;
			PlayerIdentityInfo playerIdentityInfo2 = null;
			PlayerIdentityController playerIdentityController = Service.PlayerIdentityController;
			Dictionary<string, PlayerIdentityInfo> playerIdentities = this.command.ResponseResult.PlayerIdentities;
			foreach (PlayerIdentityInfo current in playerIdentities.Values)
			{
				if (playerIdentityController.IsFirstIdentity(current.PlayerId))
				{
					playerIdentityInfo = current;
				}
				else
				{
					playerIdentityInfo2 = current;
				}
			}
			if (playerIdentityInfo2 != null)
			{
				this.PopulatePlayerInfo(AccountSyncAccountType.MultipleAccountsPrimary, playerIdentityInfo.PlayerName, playerIdentityInfo.HQLevel, playerIdentityInfo.Medals, playerIdentityInfo.Faction, playerIdentityInfo.SquadName);
				this.PopulatePlayerInfo(AccountSyncAccountType.MultipleAccountsSecondary, playerIdentityInfo2.PlayerName, playerIdentityInfo2.HQLevel, playerIdentityInfo2.Medals, playerIdentityInfo2.Faction, playerIdentityInfo2.SquadName);
			}
			else
			{
				this.PopulatePlayerInfo(AccountSyncAccountType.SingleAccount, playerIdentityInfo.PlayerName, playerIdentityInfo.HQLevel, playerIdentityInfo.Medals, playerIdentityInfo.Faction, playerIdentityInfo.SquadName);
			}
		}

		private void OnSyncConflictLoadExistingClicked(UXButton button)
		{
			this.labelTitle.Text = this.lang.Get("ACCOUNT_SYNC_CONFIRM_LOAD", new object[0]);
			this.labelBody.Text = this.lang.Get("ACCOUNT_SYNC_LOAD_INFO", new object[0]);
			this.buttonLoad.Visible = true;
			this.buttonLoad.OnClicked = new UXButtonClickedDelegate(this.OnSyncConflictLoadExistingConfirmClicked);
			this.buttonCancel.Visible = true;
			this.buttonCancel.OnClicked = new UXButtonClickedDelegate(this.OnSyncConflictCancelClicked);
			this.buttonOk.Visible = false;
			this.buttonConnectNew.Visible = false;
			this.buttonConnectNewConfirm.Visible = false;
		}

		private void OnSyncConflictLoadExistingConfirmClicked(UXButton button)
		{
			this.labelTitle.Text = this.lang.Get("ACCOUNT_SYNC", new object[0]);
			this.labelBody.Text = this.lang.Get("ACCOUNT_SYNC_LOAD_SUCCESS", new object[]
			{
				this.accountProvider
			});
			this.buttonOk.Visible = false;
			this.buttonLoad.Visible = false;
			this.buttonCancel.Visible = false;
			this.buttonConnectNew.Visible = false;
			this.buttonConnectNewConfirm.Visible = false;
			if (this.command.RequestArgs.Provider == AccountProvider.RECOVERY)
			{
				Service.IAccountSyncController.RecoverAccount(new Action(this.OnSyncConflictLoadExistingConfirmSuccess));
			}
			else
			{
				this.OnSyncConflictLoadExistingConfirmSuccess();
			}
		}

		private void OnSyncConflictLoadExistingConfirmSuccess()
		{
			RegisterExternalAccountResponse responseResult = this.command.ResponseResult;
			foreach (KeyValuePair<string, PlayerIdentityInfo> current in responseResult.PlayerIdentities)
			{
				if (current.Value.ActiveIdentity)
				{
					Service.BILoggingController.TrackAccountRestore(this.command.RequestArgs.Provider, Service.CurrentPlayer.PlayerId, current.Key);
					Service.ViewTimerManager.CreateViewTimer(0.2f, false, new TimerDelegate(this.ExecuteReloadTask), new KeyValuePair<string, string>(current.Key, responseResult.Secret));
					break;
				}
			}
		}

		private void ExecuteReloadTask(uint id, object cookie)
		{
			KeyValuePair<string, string> keyValuePair = (KeyValuePair<string, string>)cookie;
			Service.IAccountSyncController.LoadAccount(keyValuePair.Key, keyValuePair.Value);
		}

		private void OnSyncConflictConnectNewClicked(UXButton button)
		{
			this.labelTitle.Text = this.lang.Get("ACCOUNT_SYNC_CONFIRM_CONNECT_NEW_GAME", new object[0]);
			this.labelBody.Text = this.lang.Get("ACCOUNT_SYNC_CONNECT_NEW_GAME_INFO", new object[]
			{
				this.accountProvider,
				this.accountProvider
			});
			this.labelAccountTitle.Text = this.lang.Get("ACCOUNT_SYNC_NEW_SYNC", new object[]
			{
				this.accountProvider
			});
			this.buttonConnectNewConfirm.Visible = true;
			this.buttonConnectNewConfirm.OnClicked = new UXButtonClickedDelegate(this.OnSyncConflictConnectNewConfirmClicked);
			this.buttonCancel.Visible = true;
			this.buttonCancel.OnClicked = new UXButtonClickedDelegate(this.OnSyncConflictCancelClicked);
			this.buttonOk.Visible = false;
			this.buttonLoad.Visible = false;
			this.buttonConnectNew.Visible = false;
			this.PopulateCurrentPlayerInfo();
		}

		private void OnSyncConflictConnectNewConfirmClicked(UXButton button)
		{
			this.ShowSyncConfirmation();
			RegisterExternalAccountRequest requestArgs = this.command.RequestArgs;
			requestArgs.OverrideExistingAccountRegistration = true;
			string accountId = string.Empty;
			foreach (KeyValuePair<string, PlayerIdentityInfo> current in this.command.ResponseResult.PlayerIdentities)
			{
				if (!current.Value.ActiveIdentity)
				{
					accountId = current.Key;
					break;
				}
			}
			Service.BILoggingController.TrackAccountDiscard(this.command.RequestArgs.Provider, Service.CurrentPlayer.PlayerId, accountId);
			Service.IAccountSyncController.RegisterExternalAccount(requestArgs);
		}

		private void OnSyncConflictCancelClicked(UXButton button)
		{
			this.ShowSyncConflict();
		}

		private string GetAccountProviderString(AccountProvider provider)
		{
			string text = null;
			switch (provider)
			{
			case AccountProvider.FACEBOOK:
				text = "ACCOUNT_PROVIDER_FACEBOOK";
				break;
			case AccountProvider.GAMECENTER:
				text = "ACCOUNT_PROVIDER_GAMECENTER";
				break;
			case AccountProvider.GOOGLEPLAY:
				text = "ACCOUNT_PROVIDER_GOOGLEPLAY";
				break;
			case AccountProvider.RECOVERY:
				text = "ACCOUNT_PROVIDER_RECOVERY";
				break;
			}
			return (text == null) ? null : this.lang.Get(text, new object[0]);
		}
	}
}
