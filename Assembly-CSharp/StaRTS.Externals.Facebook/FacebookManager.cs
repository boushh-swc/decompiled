using Facebook.Unity;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StaRTS.Externals.Facebook
{
	public class FacebookManager : MonoBehaviour
	{
		private const string FACEBOOK_GAMEOBJECT_NAME = "UnityFacebookSDKPlugin";

		private static FacebookDelegate<IAppRequestResult> OnInviteCallback;

		private static FacebookDelegate<IShareResult> OnPostCallback;

		private static bool mFacebookIsInitialized;

		[CompilerGenerated]
		private static FacebookDelegate<ILoginResult> <>f__mg$cache0;

		[CompilerGenerated]
		private static FacebookDelegate<IAppRequestResult> <>f__mg$cache1;

		[CompilerGenerated]
		private static FacebookDelegate<IShareResult> <>f__mg$cache2;

		public static FacebookLoggedInDelegate OnFacebookLoggedIn
		{
			get;
			set;
		}

		public static FacebookLogInFailedDelegate OnFacebookLogInFailed
		{
			get;
			set;
		}

		public static bool IsLoggedIn
		{
			get
			{
				return FB.IsLoggedIn;
			}
		}

		public static void StaticReset()
		{
			FacebookManager.OnFacebookLoggedIn = null;
			FacebookManager.OnFacebookLogInFailed = null;
			FacebookManager.OnInviteCallback = null;
			FacebookManager.mFacebookIsInitialized = false;
		}

		public static void Login()
		{
			if (Service.GameIdleController != null)
			{
				Service.GameIdleController.Enabled = false;
			}
			string text = "public_profile,user_friends,email";
			IEnumerable<string> arg_49_0 = text.Split(new char[]
			{
				','
			});
			if (FacebookManager.<>f__mg$cache0 == null)
			{
				FacebookManager.<>f__mg$cache0 = new FacebookDelegate<ILoginResult>(FacebookManager.OnFacebookLogin);
			}
			FB.LogInWithReadPermissions(arg_49_0, FacebookManager.<>f__mg$cache0);
		}

		public static void Logout()
		{
			FB.LogOut();
		}

		public static void GetFriends(FacebookDelegate<IGraphResult> callback)
		{
			FB.API("me/friends?limit=100&fields=id,name,picture.type(square),installed", HttpMethod.GET, callback, null);
		}

		public static void GetSelfData(FacebookDelegate<IGraphResult> callback)
		{
			FB.API("me/", HttpMethod.GET, callback, null);
		}

		public static void InviteFriends(string message, string title, FacebookDelegate<IAppRequestResult> callback)
		{
			FacebookManager.OnInviteCallback = callback;
			if (Service.GameIdleController != null)
			{
				Service.GameIdleController.Enabled = false;
			}
			IEnumerable<string> arg_4B_1 = null;
			IEnumerable<object> arg_4B_2 = null;
			IEnumerable<string> arg_4B_3 = null;
			int? arg_4B_4 = null;
			string arg_4B_5 = "InviteRequest";
			if (FacebookManager.<>f__mg$cache1 == null)
			{
				FacebookManager.<>f__mg$cache1 = new FacebookDelegate<IAppRequestResult>(FacebookManager.OnInviteFriends);
			}
			FB.AppRequest(message, arg_4B_1, arg_4B_2, arg_4B_3, arg_4B_4, arg_4B_5, title, FacebookManager.<>f__mg$cache1);
		}

		public static void Post(FacebookPostParameters param, FacebookDelegate<IShareResult> callback)
		{
			FacebookManager.OnPostCallback = callback;
			if (Service.GameIdleController != null)
			{
				Service.GameIdleController.Enabled = false;
			}
			string arg_61_0 = string.Empty;
			Uri arg_61_1 = param.Link;
			string arg_61_2 = param.LinkName;
			string arg_61_3 = param.LinkCaption;
			string arg_61_4 = param.LinkDescription;
			Uri arg_61_5 = param.Picture;
			string arg_61_6 = param.MediaSource;
			if (FacebookManager.<>f__mg$cache2 == null)
			{
				FacebookManager.<>f__mg$cache2 = new FacebookDelegate<IShareResult>(FacebookManager.OnPost);
			}
			FB.FeedShare(arg_61_0, arg_61_1, arg_61_2, arg_61_3, arg_61_4, arg_61_5, arg_61_6, FacebookManager.<>f__mg$cache2);
		}

		private void Start()
		{
			if (!FacebookManager.mFacebookIsInitialized)
			{
				FB.Init(new InitDelegate(this.OnFacebookInit), new HideUnityDelegate(this.OnHideUnity), null);
			}
		}

		private void OnFacebookInit()
		{
			FacebookManager.mFacebookIsInitialized = true;
		}

		private void OnHideUnity(bool isGameShown)
		{
			if (Service.GameIdleController == null)
			{
				return;
			}
			if (isGameShown)
			{
				Service.GameIdleController.Enabled = true;
			}
			else
			{
				Service.GameIdleController.Enabled = false;
			}
		}

		private static void OnFacebookLogin(ILoginResult result)
		{
			if (FB.IsLoggedIn || !Service.GameIdleController.Enabled)
			{
				if (Service.GameIdleController != null)
				{
					Service.GameIdleController.Enabled = true;
				}
				if (FB.IsLoggedIn)
				{
					if (FacebookManager.OnFacebookLoggedIn != null)
					{
						FacebookManager.OnFacebookLoggedIn(result);
					}
				}
				else if (FacebookManager.OnFacebookLogInFailed != null)
				{
					FacebookManager.OnFacebookLogInFailed(result != null);
				}
			}
		}

		private static void OnInviteFriends(IAppRequestResult result)
		{
			if (Service.GameIdleController != null)
			{
				Service.GameIdleController.Enabled = true;
			}
			if (FacebookManager.OnInviteCallback != null)
			{
				FacebookManager.OnInviteCallback(result);
				FacebookManager.OnInviteCallback = null;
			}
		}

		private static void OnPost(IShareResult result)
		{
			if (Service.GameIdleController != null)
			{
				Service.GameIdleController.Enabled = true;
			}
			if (FacebookManager.OnPostCallback != null)
			{
				FacebookManager.OnPostCallback(result);
				FacebookManager.OnPostCallback = null;
			}
		}
	}
}
