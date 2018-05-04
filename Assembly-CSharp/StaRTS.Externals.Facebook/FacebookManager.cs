using Facebook.Unity;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Externals.Facebook
{
	public class FacebookManager : MonoBehaviour
	{
		private const string FACEBOOK_GAMEOBJECT_NAME = "UnityFacebookSDKPlugin";

		private static FacebookDelegate<IAppRequestResult> OnInviteCallback;

		private static FacebookDelegate<IShareResult> OnPostCallback;

		private static bool mFacebookIsInitialized;

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
			FB.LogInWithReadPermissions(text.Split(new char[]
			{
				','
			}), new FacebookDelegate<ILoginResult>(FacebookManager.OnFacebookLogin));
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
			FB.AppRequest(message, null, null, null, null, "InviteRequest", title, new FacebookDelegate<IAppRequestResult>(FacebookManager.OnInviteFriends));
		}

		public static void Post(FacebookPostParameters param, FacebookDelegate<IShareResult> callback)
		{
			FacebookManager.OnPostCallback = callback;
			if (Service.GameIdleController != null)
			{
				Service.GameIdleController.Enabled = false;
			}
			FB.FeedShare(string.Empty, param.Link, param.LinkName, param.LinkCaption, param.LinkDescription, param.Picture, param.MediaSource, new FacebookDelegate<IShareResult>(FacebookManager.OnPost));
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
