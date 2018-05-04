using StaRTS.Main.Models;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public interface ISocialDataController
	{
		OnRequestDelegate InviteFriendsCB
		{
			get;
			set;
		}

		OnFBFriendsDelegate FriendsDetailsCB
		{
			get;
			set;
		}

		bool HaveAllData
		{
			get;
		}

		bool HaveFriendData
		{
			get;
		}

		bool HaveSelfData
		{
			get;
		}

		string Gender
		{
			get;
		}

		string FullName
		{
			get;
		}

		string FirstName
		{
			get;
		}

		string LastName
		{
			get;
		}

		string FacebookLocale
		{
			get;
		}

		string FacebookId
		{
			get;
		}

		List<SocialFriendData> Friends
		{
			get;
		}

		Dictionary<string, SocialFriendData> PlayerIdToFriendData
		{
			get;
		}

		List<string> InstalledFBIDs
		{
			get;
		}

		bool IsLoggedIn
		{
			get;
		}

		void InviteFriends(OnRequestDelegate callback);

		void GetSelfPicture(OnGetProfilePicture callback, object cookie);

		void GetFriendPicture(SocialFriendData friend, OnGetProfilePicture callback, object cookie);

		void DestroyFriendPicture(Texture2D texture);

		void Logout();

		void CheckFacebookLoginOnStartup();

		void Login(OnAllDataFetchedDelegate callback);

		void StaticReset();

		void PopulateFacebookData();

		void UpdateFriends();
	}
}
