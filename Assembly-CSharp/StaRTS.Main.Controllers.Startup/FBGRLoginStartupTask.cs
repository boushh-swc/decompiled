using Facebook.Unity;
using StaRTS.Externals.Facebook;
using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class FBGRLoginStartupTask : StartupTask
	{
		public FBGRLoginStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			if (FacebookManager.IsLoggedIn)
			{
				this.OnFacebookLogin(null);
			}
			else
			{
				FacebookManager.OnFacebookLoggedIn = new FacebookLoggedInDelegate(this.OnFacebookLogin);
				FacebookManager.OnFacebookLogInFailed = new FacebookLogInFailedDelegate(this.OnFacebookLoginFailed);
				FacebookManager.Login();
			}
		}

		private void OnFacebookLogin(ILoginResult result)
		{
			FacebookManager.OnFacebookLoggedIn = null;
			FacebookManager.OnFacebookLogInFailed = null;
			base.Complete();
		}

		private void OnFacebookLoginFailed(bool showDialog)
		{
			FacebookManager.OnFacebookLoggedIn = null;
			FacebookManager.OnFacebookLogInFailed = null;
			base.Complete();
		}
	}
}
