using System;

namespace StaRTS.Main.Views.UserInput
{
	public interface IBackButtonManager
	{
		void RegisterBackButtonHandler(IBackButtonHandler handler);

		void UnregisterBackButtonHandler(IBackButtonHandler handler);
	}
}
