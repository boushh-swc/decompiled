using Facebook.Unity;
using StaRTS.Externals.Facebook;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Leaderboard;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class SocialDataController : ISocialDataController
	{
		private const string KEY_FRIEND_DATA_LIST = "data";

		private const string KEY_GENDER = "gender";

		private const string KEY_FULL_NAME = "name";

		private const string KEY_FIRST_NAME = "first_name";

		private const string KEY_LAST_NAME = "last_name";

		private const string KEY_LOCALE = "locale";

		private const string KEY_ID = "id";

		private const string KEY_RECIPIENTS = "to";

		private List<OnAllDataFetchedDelegate> allDataFetchedCallbacks;

		public OnRequestDelegate InviteFriendsCB
		{
			get;
			set;
		}

		public OnFBFriendsDelegate FriendsDetailsCB
		{
			get;
			set;
		}

		public bool HaveAllData
		{
			get
			{
				return this.HaveFriendData && this.HaveSelfData;
			}
		}

		public bool HaveFriendData
		{
			get;
			private set;
		}

		public bool HaveSelfData
		{
			get;
			private set;
		}

		public string Gender
		{
			get;
			private set;
		}

		public string FullName
		{
			get;
			private set;
		}

		public string FirstName
		{
			get;
			private set;
		}

		public string LastName
		{
			get;
			private set;
		}

		public string FacebookLocale
		{
			get;
			private set;
		}

		public string FacebookId
		{
			get;
			private set;
		}

		public List<SocialFriendData> Friends
		{
			get;
			private set;
		}

		public Dictionary<string, SocialFriendData> PlayerIdToFriendData
		{
			get;
			private set;
		}

		public List<string> InstalledFBIDs
		{
			get;
			private set;
		}

		public bool IsLoggedIn
		{
			get
			{
				return FacebookManager.IsLoggedIn;
			}
		}

		public SocialDataController()
		{
			this.allDataFetchedCallbacks = new List<OnAllDataFetchedDelegate>();
			this.HaveFriendData = false;
			this.HaveSelfData = false;
			Service.ISocialDataController = this;
		}

		public void StaticReset()
		{
			FacebookManager.StaticReset();
		}

		public void PopulateFacebookData()
		{
			if (FacebookManager.IsLoggedIn)
			{
				FacebookManager.GetFriends(new FacebookDelegate<IGraphResult>(this.OnGetFriendsData));
				FacebookManager.GetSelfData(new FacebookDelegate<IGraphResult>(this.OnGetSelfData));
			}
		}

		public void UpdateFriends()
		{
			string friendIds = string.Empty;
			if (FacebookManager.IsLoggedIn && this.InstalledFBIDs != null && this.InstalledFBIDs.Count > 0)
			{
				friendIds = string.Join(",", this.InstalledFBIDs.ToArray());
			}
			Service.LeaderboardController.UpdateFriends(friendIds, new LeaderboardController.OnUpdateData(this.OnGetFriendsData));
		}

		private void OnGetFriendsData(IGraphResult result)
		{
			this.Friends = new List<SocialFriendData>();
			this.PlayerIdToFriendData = new Dictionary<string, SocialFriendData>();
			this.InstalledFBIDs = new List<string>();
			if (!string.IsNullOrEmpty(result.RawResult))
			{
				object obj = new JsonParser(result.RawResult).Parse();
				Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
				List<object> list = dictionary["data"] as List<object>;
				for (int i = 0; i < list.Count; i++)
				{
					SocialFriendData socialFriendData = (SocialFriendData)new SocialFriendData().FromObject(list[i]);
					this.Friends.Add(socialFriendData);
					if (socialFriendData.Installed)
					{
						this.InstalledFBIDs.Add(socialFriendData.Id);
					}
				}
				this.CommonFriendDataActions();
			}
			else
			{
				Service.Logger.ErrorFormat("Error fetching FB friends data: {0}", new object[]
				{
					result.Error
				});
			}
		}

		private void CallFriendsDetailsCB()
		{
			OnFBFriendsDelegate friendsDetailsCB = this.FriendsDetailsCB;
			this.FriendsDetailsCB = null;
			if (friendsDetailsCB != null)
			{
				friendsDetailsCB();
			}
		}

		private void OnGetFriendsData(bool success)
		{
			if (!success)
			{
				this.CommonFriendDataActions();
				return;
			}
			if (this.Friends == null)
			{
				this.CallFriendsDetailsCB();
				return;
			}
			List<PlayerLBEntity> list = Service.LeaderboardController.Friends.List;
			Dictionary<string, PlayerLBEntity> dictionary = new Dictionary<string, PlayerLBEntity>();
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				PlayerLBEntity playerLBEntity = list[i];
				if (!string.IsNullOrEmpty(playerLBEntity.SocialID) && !dictionary.ContainsKey(playerLBEntity.SocialID))
				{
					dictionary.Add(playerLBEntity.SocialID, playerLBEntity);
				}
				i++;
			}
			int j = 0;
			int count2 = this.Friends.Count;
			while (j < count2)
			{
				SocialFriendData socialFriendData = this.Friends[j];
				if (dictionary.ContainsKey(socialFriendData.Id))
				{
					socialFriendData.PlayerData = dictionary[socialFriendData.Id];
					if (!this.PlayerIdToFriendData.ContainsKey(socialFriendData.PlayerData.PlayerID))
					{
						this.PlayerIdToFriendData.Add(socialFriendData.PlayerData.PlayerID, socialFriendData);
					}
				}
				j++;
			}
			dictionary.Clear();
			this.CommonFriendDataActions();
		}

		private void CommonFriendDataActions()
		{
			this.CallFriendsDetailsCB();
			this.HaveFriendData = true;
			this.DoCallbacksIfHaveAllData();
		}

		private void OnGetSelfData(IGraphResult result)
		{
			this.HaveSelfData = true;
			if (result != null && !string.IsNullOrEmpty(result.RawResult))
			{
				object obj = new JsonParser(result.RawResult).Parse();
				Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
				if (dictionary.ContainsKey("gender"))
				{
					this.Gender = (string)dictionary["gender"];
				}
				if (dictionary.ContainsKey("name"))
				{
					this.FullName = (string)dictionary["name"];
				}
				if (dictionary.ContainsKey("locale"))
				{
					this.FacebookLocale = (string)dictionary["locale"];
				}
				if (dictionary.ContainsKey("first_name"))
				{
					this.FirstName = (string)dictionary["first_name"];
				}
				if (dictionary.ContainsKey("last_name"))
				{
					this.LastName = (string)dictionary["last_name"];
				}
				if (dictionary.ContainsKey("id"))
				{
					this.FacebookId = (string)dictionary["id"];
				}
			}
			this.DoCallbacksIfHaveAllData();
		}

		private void DoCallbacksIfHaveAllData()
		{
			if (this.HaveAllData)
			{
				ProcessingScreen.Hide();
				for (int i = 0; i < this.allDataFetchedCallbacks.Count; i++)
				{
					this.allDataFetchedCallbacks[i]();
				}
				this.allDataFetchedCallbacks.Clear();
			}
		}

		public void InviteFriends(OnRequestDelegate callback)
		{
			if (callback != null)
			{
				this.InviteFriendsCB = callback;
			}
			Lang lang = Service.Lang;
			string message = lang.Get("FB_INVITE_MESSAGE", new object[0]);
			string title = lang.Get("FB_INVITE_TITLE", new object[0]);
			FacebookManager.InviteFriends(message, title, new FacebookDelegate<IAppRequestResult>(this.OnFacebookInviteFriends));
		}

		private void OnFacebookInviteFriends(IAppRequestResult result)
		{
			if (this.InviteFriendsCB != null)
			{
				this.InviteFriendsCB(result);
				this.InviteFriendsCB = null;
			}
			Dictionary<string, object> jsonFromFBResult = this.GetJsonFromFBResult(result);
			if (jsonFromFBResult != null)
			{
				string trackingCode = "InviteRequest";
				if (jsonFromFBResult.ContainsKey("data"))
				{
					trackingCode = (string)jsonFromFBResult["data"];
				}
				if (jsonFromFBResult.ContainsKey("to"))
				{
					string text = (string)jsonFromFBResult["to"];
					if (string.IsNullOrEmpty(text))
					{
						return;
					}
					string[] array = text.Split(new char[]
					{
						','
					});
					Service.BILoggingController.TrackSendMessage(trackingCode, text, array.Length);
				}
			}
		}

		private Dictionary<string, object> GetJsonFromFBResult(IAppRequestResult result)
		{
			Dictionary<string, object> result2 = null;
			string error = result.Error;
			if (string.IsNullOrEmpty(error) && !string.IsNullOrEmpty(result.RawResult))
			{
				object obj = new JsonParser(result.RawResult).Parse();
				result2 = (Dictionary<string, object>)obj;
			}
			return result2;
		}

		public void GetSelfPicture(OnGetProfilePicture callback, object cookie)
		{
			Service.Engine.StartCoroutine(this.DownloadProfileImageCoroutine("https://graph.facebook.com/" + this.FacebookId + "/picture?type=square", callback, cookie));
		}

		public void GetFriendPicture(SocialFriendData friend, OnGetProfilePicture callback, object cookie)
		{
			Service.Engine.StartCoroutine(this.DownloadProfileImageCoroutine(friend.PictureURL, callback, cookie));
		}

		[DebuggerHidden]
		private IEnumerator DownloadProfileImageCoroutine(string url, OnGetProfilePicture callback, object cookie)
		{
			WWW wWW = new WWW(url);
			WWWManager.Add(wWW);
			yield return wWW;
			if (WWWManager.Remove(wWW))
			{
				string error = wWW.error;
				if (string.IsNullOrEmpty(error))
				{
					callback(wWW.texture, cookie);
				}
				else
				{
					Service.Logger.ErrorFormat("Error fetching picture at {0}", new object[]
					{
						url
					});
				}
				wWW.Dispose();
			}
			yield break;
		}

		public void DestroyFriendPicture(Texture2D texture)
		{
			UnityEngine.Object.Destroy(texture);
		}

		public void Logout()
		{
			this.HaveFriendData = false;
			this.HaveSelfData = false;
			this.Friends = null;
			this.PlayerIdToFriendData = null;
			Service.LeaderboardController.Friends.List.Clear();
			FacebookManager.Logout();
			Service.IAccountSyncController.UnregisterFacebookAccount();
		}

		public void CheckFacebookLoginOnStartup()
		{
			if (this.IsLoggedIn)
			{
				this.OnFacebookLogin(null);
			}
		}

		public void Login(OnAllDataFetchedDelegate callback)
		{
			if (this.HaveAllData)
			{
				callback();
				return;
			}
			this.allDataFetchedCallbacks.Add(callback);
			ProcessingScreen.Show();
			FacebookManager.OnFacebookLoggedIn = new FacebookLoggedInDelegate(this.OnFacebookLogin);
			FacebookManager.OnFacebookLogInFailed = new FacebookLogInFailedDelegate(this.OnFacebookLoginFailed);
			FacebookManager.Login();
			Service.BILoggingController.TrackAuthorization("allow", "f");
		}

		private void OnFacebookLogin(ILoginResult result)
		{
			this.PopulateFacebookData();
			Service.IAccountSyncController.OnFacebookSignIn();
		}

		private void OnFacebookLoginFailed(bool showDialog)
		{
			ProcessingScreen.Hide();
			this.allDataFetchedCallbacks.Clear();
			if (showDialog)
			{
				Lang lang = Service.Lang;
				AlertScreen.ShowModal(false, lang.Get("FACEBOOK_PERMISSION_ERROR_TITLE", new object[0]), lang.Get("FACEBOOK_PERMISSION_ERROR_MESSAGE", new object[0]), null, null);
			}
		}
	}
}
