using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Commands.Player.Identity;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class PlayerIdentityController
	{
		public delegate void GetOtherPlayerIdentityCallback(PlayerIdentityInfo info);

		private const char ID_DELIMITER = '_';

		private const int DEFAULT_IDENTITY_INDEX = 0;

		private const int SECOND_IDENTITY_INDEX = 1;

		private const int MAX_NUM_FORCED_RELOADS = 3;

		private PlayerIdentityInfo otherPlayerIdentityInfo;

		public PlayerIdentityController()
		{
			Service.PlayerIdentityController = this;
		}

		public void GetOtherPlayerIdentity(PlayerIdentityController.GetOtherPlayerIdentityCallback callback)
		{
			if (this.otherPlayerIdentityInfo != null)
			{
				if (callback != null)
				{
					callback(this.otherPlayerIdentityInfo);
				}
			}
			else
			{
				int identityIndex = (!this.IsFirstIdentity(Service.CurrentPlayer.PlayerId)) ? 0 : 1;
				PlayerIdentityGetCommand playerIdentityGetCommand = new PlayerIdentityGetCommand(new PlayerIdentityRequest
				{
					IdentityIndex = identityIndex
				});
				playerIdentityGetCommand.AddSuccessCallback(new AbstractCommand<PlayerIdentityRequest, PlayerIdentityGetResponse>.OnSuccessCallback(this.OnGetOtherPlayerIdentity));
				playerIdentityGetCommand.Context = callback;
				Service.ServerAPI.Sync(playerIdentityGetCommand);
			}
		}

		private void OnGetOtherPlayerIdentity(PlayerIdentityGetResponse response, object cookie)
		{
			this.otherPlayerIdentityInfo = response.Info;
			PlayerIdentityController.GetOtherPlayerIdentityCallback getOtherPlayerIdentityCallback = cookie as PlayerIdentityController.GetOtherPlayerIdentityCallback;
			if (getOtherPlayerIdentityCallback != null)
			{
				getOtherPlayerIdentityCallback(this.otherPlayerIdentityInfo);
			}
		}

		public void SwitchToNewIdentity()
		{
			this.SwitchIdentity(1);
		}

		public void SwitchIdentity(string playerId)
		{
			int identityIndex = this.GetIdentityIndex(playerId);
			this.SwitchIdentity(identityIndex);
		}

		public void SwitchIdentity(int identityIndex)
		{
			PlayerIdentitySwitchCommand playerIdentitySwitchCommand = new PlayerIdentitySwitchCommand(new PlayerIdentityRequest
			{
				IdentityIndex = identityIndex
			});
			playerIdentitySwitchCommand.AddSuccessCallback(new AbstractCommand<PlayerIdentityRequest, PlayerIdentitySwitchResponse>.OnSuccessCallback(this.OnPlayerIdentitySwitched));
			Service.ServerAPI.Sync(playerIdentitySwitchCommand);
		}

		private void OnPlayerIdentitySwitched(PlayerIdentitySwitchResponse response, object cookie)
		{
			this.InternalSwitchPlayer(response.PlayerId);
		}

		public void HandleInactiveIdentityError(string activePlayerId)
		{
			if (!string.IsNullOrEmpty(activePlayerId))
			{
				if (Engine.NumReloads >= 3)
				{
					Service.Logger.ErrorFormat("Faction flipping error: Max number of forced reloads reached for {0}", new object[]
					{
						activePlayerId
					});
				}
				else
				{
					this.InternalSwitchPlayer(activePlayerId);
				}
			}
			else
			{
				Service.Logger.Error("Inactive identity error but no active player id.");
			}
		}

		private void InternalSwitchPlayer(string playerId)
		{
			Service.NotificationController.ClearAllPendingLocalNotifications();
			if (Service.ServerPlayerPrefs != null)
			{
				Service.ServerPlayerPrefs.SavePrefsLocally();
			}
			PlayerPrefs.SetString("prefPlayerId", playerId);
			Service.Engine.Reload();
		}

		public bool IsFirstIdentity(string playerId)
		{
			int identityIndex = this.GetIdentityIndex(playerId);
			return identityIndex == 0;
		}

		private int GetIdentityIndex(string playerId)
		{
			int result = 0;
			int num = playerId.LastIndexOf('_');
			if (num >= 0 && num < playerId.Length - 1)
			{
				string s = playerId.Substring(num + 1);
				int.TryParse(s, out result);
			}
			return result;
		}
	}
}
