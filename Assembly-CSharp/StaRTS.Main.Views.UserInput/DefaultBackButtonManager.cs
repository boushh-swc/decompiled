using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UserInput
{
	public class DefaultBackButtonManager : IBackButtonManager
	{
		public DefaultBackButtonManager()
		{
			Service.IBackButtonManager = this;
		}

		public void RegisterBackButtonHandler(IBackButtonHandler handler)
		{
		}

		public void UnregisterBackButtonHandler(IBackButtonHandler handler)
		{
		}
	}
}
