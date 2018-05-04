using Facebook.Unity;
using StaRTS.Externals.GameServices;
using StaRTS.Externals.Manimal;
using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player.Account.External;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class AccountSyncController : IEventObserver, IAccountSyncController
	{
		private GetExternalAccountsResponse externalAccountInfo;

		private bool recoveryRegisterPending;

		private bool facebookRegisterPending;

		private bool gameServicesRegisterPending;

		private OnUpdateExternalAccountInfoResponseReceived updateCallback;

		private Action recoverAccountCallback;

		public AccountSyncController()
		{
			Service.IAccountSyncController = this;
			this.recoveryRegisterPending = false;
			this.facebookRegisterPending = false;
			this.gameServicesRegisterPending = false;
			Service.EventManager.RegisterObserver(this, EventId.GameServicesSignedIn);
			Service.EventManager.RegisterObserver(this, EventId.StartupTasksCompleted);
		}

		public string GetDerivedAccountProviderId(AccountProvider provider)
		{
			string result = string.Empty;
			if (this.externalAccountInfo == null)
			{
				return result;
			}
			if (provider == AccountProvider.RECOVERY)
			{
				result = this.externalAccountInfo.DerivedRecoveryId;
			}
			else if (provider == AccountProvider.FACEBOOK)
			{
				result = this.externalAccountInfo.DerivedFacebookAccountId;
			}
			else if (provider == AccountProvider.GAMECENTER)
			{
				result = this.externalAccountInfo.DerivedGameCenterAccountId;
			}
			else if (provider == AccountProvider.GOOGLEPLAY)
			{
				result = this.externalAccountInfo.DerivedGooglePlayAccountId;
			}
			return result;
		}

		public void UpdateExternalAccountInfo(OnUpdateExternalAccountInfoResponseReceived callback)
		{
			this.updateCallback = callback;
			GetExternalAccountsCommand getExternalAccountsCommand = new GetExternalAccountsCommand(new PlayerIdRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			getExternalAccountsCommand.AddSuccessCallback(new AbstractCommand<PlayerIdRequest, GetExternalAccountsResponse>.OnSuccessCallback(this.OnGetExternalAccountInfo));
			this.SendServerCommand(getExternalAccountsCommand, false);
		}

		private void OnGetExternalAccountInfo(GetExternalAccountsResponse response, object cookie)
		{
			this.externalAccountInfo = response;
			if (this.recoveryRegisterPending)
			{
				this.RegisterRecoveryAccount();
				this.recoveryRegisterPending = false;
			}
			if (this.facebookRegisterPending)
			{
				this.RegisterFacebookAccount();
				this.facebookRegisterPending = false;
			}
			if (this.gameServicesRegisterPending)
			{
				this.RegisterGameServicesAccount();
				this.gameServicesRegisterPending = false;
			}
			if (this.updateCallback != null)
			{
				this.updateCallback();
				this.updateCallback = null;
			}
		}

		public void LoadAccount(string playerId, string playerSecret)
		{
			PlayerPrefs.SetString("prefPlayerId", playerId);
			PlayerPrefs.SetString("prefPlayerSecret", playerSecret);
			Service.Engine.Reload();
		}

		public void OnFacebookSignIn()
		{
			this.RegisterFacebookAccount();
		}

		private void RegisterGameServicesAccount()
		{
			string userId = GameServicesManager.GetUserId();
			if (string.IsNullOrEmpty(userId))
			{
				return;
			}
			if (this.externalAccountInfo == null)
			{
				this.gameServicesRegisterPending = true;
				return;
			}
			if (this.IsRecoveryAvailable())
			{
				return;
			}
			string authToken = GameServicesManager.GetAuthToken();
			if (string.IsNullOrEmpty(authToken))
			{
				return;
			}
			this.RegisterExternalAccount(new RegisterExternalAccountRequest
			{
				OverrideExistingAccountRegistration = false,
				ExternalAccountId = userId,
				ExternalAccountSecurityToken = authToken,
				Provider = AccountProvider.GOOGLEPLAY,
				PlayerId = Service.CurrentPlayer.PlayerId,
				OtherLinkedProvider = AccountProvider.FACEBOOK
			});
		}

		private void RegisterRecoveryAccount()
		{
			if (this.externalAccountInfo == null)
			{
				this.recoveryRegisterPending = true;
				return;
			}
			if (!this.IsRecoveryAvailable())
			{
				return;
			}
			RegisterExternalAccountRequest registerExternalAccountRequest = new RegisterExternalAccountRequest();
			registerExternalAccountRequest.OverrideExistingAccountRegistration = false;
			registerExternalAccountRequest.PlayerId = Service.CurrentPlayer.PlayerId;
			registerExternalAccountRequest.ExternalAccountId = this.externalAccountInfo.DerivedRecoveryId;
			registerExternalAccountRequest.ExternalAccountSecurityToken = registerExternalAccountRequest.PlayerId;
			registerExternalAccountRequest.Provider = AccountProvider.RECOVERY;
			this.RegisterExternalAccount(registerExternalAccountRequest);
		}

		private bool IsRecoveryAvailable()
		{
			return this.externalAccountInfo != null && !string.IsNullOrEmpty(this.externalAccountInfo.DerivedRecoveryId);
		}

		private void RegisterFacebookAccount()
		{
			string userId = AccessToken.CurrentAccessToken.UserId;
			if (this.externalAccountInfo == null)
			{
				this.facebookRegisterPending = true;
				return;
			}
			if (this.IsRecoveryAvailable())
			{
				return;
			}
			this.RegisterExternalAccount(new RegisterExternalAccountRequest
			{
				OverrideExistingAccountRegistration = false,
				ExternalAccountId = userId,
				ExternalAccountSecurityToken = AccessToken.CurrentAccessToken.TokenString,
				Provider = AccountProvider.FACEBOOK,
				PlayerId = Service.CurrentPlayer.PlayerId,
				OtherLinkedProvider = AccountProvider.GOOGLEPLAY
			});
		}

		public void RegisterExternalAccount(RegisterExternalAccountRequest request)
		{
			RegisterExternalAccountCommand registerExternalAccountCommand = new RegisterExternalAccountCommand(request);
			registerExternalAccountCommand.AddSuccessCallback(new AbstractCommand<RegisterExternalAccountRequest, RegisterExternalAccountResponse>.OnSuccessCallback(this.OnAccountRegisterSuccess));
			registerExternalAccountCommand.AddFailureCallback(new AbstractCommand<RegisterExternalAccountRequest, RegisterExternalAccountResponse>.OnFailureCallback(this.OnAccountRegisterFailure));
			registerExternalAccountCommand.Context = registerExternalAccountCommand;
			this.SendServerCommand(registerExternalAccountCommand, true);
		}

		public void UnregisterFacebookAccount()
		{
			if (this.externalAccountInfo == null || this.externalAccountInfo.DerivedFacebookAccountId == null)
			{
				return;
			}
			if (this.externalAccountInfo.DerivedGameCenterAccountId != null || this.externalAccountInfo.DerivedGooglePlayAccountId != null)
			{
				return;
			}
			UnregisterExternalAccountRequest unregisterExternalAccountRequest = new UnregisterExternalAccountRequest();
			unregisterExternalAccountRequest.PlayerId = Service.CurrentPlayer.PlayerId;
			unregisterExternalAccountRequest.Provider = AccountProvider.FACEBOOK;
			UnregisterExternalAccountCommand unregisterExternalAccountCommand = new UnregisterExternalAccountCommand(unregisterExternalAccountRequest);
			unregisterExternalAccountCommand.AddSuccessCallback(new AbstractCommand<UnregisterExternalAccountRequest, DefaultResponse>.OnSuccessCallback(this.OnAccountUnregisterSuccess));
			unregisterExternalAccountCommand.Context = unregisterExternalAccountRequest.Provider;
			this.SendServerCommand(unregisterExternalAccountCommand, true);
		}

		public void UnregisterGameServicesAccount()
		{
			if (this.externalAccountInfo == null || (this.externalAccountInfo.DerivedGameCenterAccountId == null && this.externalAccountInfo.DerivedGooglePlayAccountId == null))
			{
				return;
			}
			if (this.externalAccountInfo.DerivedFacebookAccountId != null)
			{
				return;
			}
			UnregisterExternalAccountRequest unregisterExternalAccountRequest = new UnregisterExternalAccountRequest();
			unregisterExternalAccountRequest.PlayerId = Service.CurrentPlayer.PlayerId;
			unregisterExternalAccountRequest.Provider = AccountProvider.GOOGLEPLAY;
			UnregisterExternalAccountCommand unregisterExternalAccountCommand = new UnregisterExternalAccountCommand(unregisterExternalAccountRequest);
			unregisterExternalAccountCommand.AddSuccessCallback(new AbstractCommand<UnregisterExternalAccountRequest, DefaultResponse>.OnSuccessCallback(this.OnAccountUnregisterSuccess));
			unregisterExternalAccountCommand.Context = unregisterExternalAccountRequest.Provider;
			this.SendServerCommand(unregisterExternalAccountCommand, true);
		}

		public void RecoverAccount(Action callback)
		{
			RecoverExternalAccountCommand recoverExternalAccountCommand = new RecoverExternalAccountCommand(new PlayerIdRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId
			});
			recoverExternalAccountCommand.AddSuccessCallback(new AbstractCommand<PlayerIdRequest, DefaultResponse>.OnSuccessCallback(this.OnSyncConflictLoadExistingConfirmSuccess));
			this.recoverAccountCallback = callback;
			this.SendServerCommand(recoverExternalAccountCommand, true);
		}

		private void OnSyncConflictLoadExistingConfirmSuccess(DefaultResponse response, object cookie)
		{
			if (this.recoverAccountCallback != null)
			{
				this.recoverAccountCallback();
				this.recoverAccountCallback = null;
			}
		}

		private void SendServerCommand(ICommand command, bool immediate)
		{
			ServerAPI serverAPI = Service.ServerAPI;
			if (!serverAPI.Enabled && Service.CurrentPlayer.CampaignProgress.FueInProgress)
			{
				if (immediate)
				{
					serverAPI.Enabled = true;
					serverAPI.Sync(command);
					serverAPI.Enabled = false;
				}
				else
				{
					serverAPI.Enqueue(command);
				}
			}
			else if (immediate)
			{
				serverAPI.Sync(command);
			}
			else
			{
				serverAPI.Enqueue(command);
			}
		}

		private void OnFacebookAccountRegisterSuccess(RegisterExternalAccountResponse response, object cookie)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (!currentPlayer.IsConnectedAccount)
			{
				currentPlayer.IsConnectedAccount = true;
			}
			if (response.ExternalAccountReward > 0)
			{
				currentPlayer.Inventory.ModifyCrystals(response.ExternalAccountReward);
			}
		}

		private void OnAccountRegisterSuccess(RegisterExternalAccountResponse response, object cookie)
		{
			RegisterExternalAccountCommand registerExternalAccountCommand = (RegisterExternalAccountCommand)cookie;
			switch (registerExternalAccountCommand.RequestArgs.Provider)
			{
			case AccountProvider.FACEBOOK:
				this.externalAccountInfo.DerivedFacebookAccountId = response.DerivedExternalAccountId;
				this.OnFacebookAccountRegisterSuccess(response, cookie);
				break;
			case AccountProvider.GAMECENTER:
				this.externalAccountInfo.DerivedGameCenterAccountId = response.DerivedExternalAccountId;
				break;
			case AccountProvider.GOOGLEPLAY:
				this.externalAccountInfo.DerivedGooglePlayAccountId = response.DerivedExternalAccountId;
				break;
			case AccountProvider.RECOVERY:
				this.externalAccountInfo.DerivedRecoveryId = response.DerivedExternalAccountId;
				break;
			}
		}

		private void OnAccountRegisterFailure(uint status, object cookie)
		{
			RegisterExternalAccountCommand registerExternalAccountCommand = (RegisterExternalAccountCommand)cookie;
			Lang lang = Service.Lang;
			string title = lang.Get("ACCOUNT_SYNC_ERROR", new object[0]);
			string message = null;
			string derivedExternalAccountId = registerExternalAccountCommand.ResponseResult.DerivedExternalAccountId;
			if (status != 2200u)
			{
				if (status != 2201u)
				{
					if (status == 1318u)
					{
						switch (registerExternalAccountCommand.RequestArgs.Provider)
						{
						case AccountProvider.FACEBOOK:
							message = lang.Get("ACCOUNT_SYNC_AUTH_ERROR_FACEBOOK", new object[0]);
							break;
						case AccountProvider.GAMECENTER:
							message = lang.Get("ACCOUNT_SYNC_AUTH_ERROR_GAMECENTER", new object[0]);
							break;
						case AccountProvider.GOOGLEPLAY:
							message = lang.Get("ACCOUNT_SYNC_AUTH_ERROR_GOOGLEPLAY", new object[0]);
							break;
						case AccountProvider.RECOVERY:
							message = lang.Get("ACCOUNT_SYNC_AUTH_ERROR_RECOVERY", new object[0]);
							break;
						}
						ProcessingScreen.Hide();
						AlertScreen.ShowModal(false, title, message, null, null);
					}
				}
				else
				{
					switch (registerExternalAccountCommand.RequestArgs.Provider)
					{
					case AccountProvider.FACEBOOK:
						if (this.externalAccountInfo.DerivedFacebookAccountId != null && this.externalAccountInfo.DerivedFacebookAccountId != derivedExternalAccountId)
						{
							message = lang.Get("ACCOUNT_SYNC_ERROR_FACEBOOK", new object[0]);
						}
						else if (this.externalAccountInfo.DerivedGooglePlayAccountId != null)
						{
							message = lang.Get("ACCOUNT_SYNC_ERROR_FACEBOOK_GOOGLEPLAY", new object[0]);
						}
						break;
					case AccountProvider.GAMECENTER:
						if (this.externalAccountInfo.DerivedGameCenterAccountId != null && this.externalAccountInfo.DerivedGameCenterAccountId != derivedExternalAccountId)
						{
							message = lang.Get("ACCOUNT_SYNC_ERROR_GAMECENTER", new object[0]);
						}
						else if (this.externalAccountInfo.DerivedFacebookAccountId != null)
						{
							message = lang.Get("ACCOUNT_SYNC_ERROR_GAMECENTER_FACEBOOK", new object[0]);
						}
						break;
					case AccountProvider.GOOGLEPLAY:
						if (this.externalAccountInfo.DerivedGooglePlayAccountId != null && this.externalAccountInfo.DerivedGooglePlayAccountId != derivedExternalAccountId)
						{
							message = lang.Get("ACCOUNT_SYNC_ERROR_GOOGLEPLAY", new object[0]);
						}
						else if (this.externalAccountInfo.DerivedFacebookAccountId != null)
						{
							message = lang.Get("ACCOUNT_SYNC_ERROR_GOOGLEPLAY_FACEBOOK", new object[0]);
						}
						break;
					case AccountProvider.RECOVERY:
						message = lang.Get("ACCOUNT_SYNC_ERROR_RECOVERY", new object[0]);
						break;
					}
					ProcessingScreen.Hide();
					AlertScreen.ShowModal(false, title, message, null, null);
				}
			}
			else
			{
				ProcessingScreen.Hide();
				AccountSyncScreen accountSyncScreen = AccountSyncScreen.CreateSyncConflictScreen(registerExternalAccountCommand);
				if (registerExternalAccountCommand.RequestArgs.Provider == AccountProvider.RECOVERY)
				{
					accountSyncScreen.OverrideDescription(lang.Get("ACCOUNT_SYNC_CONFLICT_RECOVERY", new object[0]));
				}
				Service.ScreenController.AddScreen(accountSyncScreen);
			}
		}

		private void OnAccountUnregisterSuccess(DefaultResponse response, object cookie)
		{
			switch ((AccountProvider)cookie)
			{
			case AccountProvider.FACEBOOK:
				this.externalAccountInfo.DerivedFacebookAccountId = null;
				break;
			case AccountProvider.GAMECENTER:
				this.externalAccountInfo.DerivedGameCenterAccountId = null;
				break;
			case AccountProvider.GOOGLEPLAY:
				this.externalAccountInfo.DerivedGooglePlayAccountId = null;
				break;
			case AccountProvider.RECOVERY:
				this.externalAccountInfo.DerivedRecoveryId = null;
				break;
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.GameServicesSignedIn)
			{
				if (id == EventId.StartupTasksCompleted)
				{
					this.RegisterRecoveryAccount();
				}
			}
			else
			{
				this.RegisterGameServicesAccount();
			}
			return EatResponse.NotEaten;
		}
	}
}
