using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class ProcessingScreen : ScreenBase
	{
		private static ProcessingScreen instance;

		private static bool showing;

		private static bool loaded;

		private static bool added;

		public ProcessingScreen() : base("gui_loading_small_anim")
		{
		}

		public static void Show()
		{
			ProcessingScreen.showing = true;
			if (ProcessingScreen.instance == null)
			{
				ProcessingScreen.instance = new ProcessingScreen();
			}
		}

		public static void Hide()
		{
			ProcessingScreen.showing = false;
			if (ProcessingScreen.instance != null)
			{
				ProcessingScreen.instance.RefreshView();
			}
		}

		public static void StaticReset()
		{
			ProcessingScreen.instance = null;
			ProcessingScreen.loaded = false;
			ProcessingScreen.showing = false;
			ProcessingScreen.added = false;
		}

		public static bool IsShowing()
		{
			return ProcessingScreen.showing;
		}

		protected override void OnScreenLoaded()
		{
			ProcessingScreen.loaded = true;
			if (ProcessingScreen.instance != null)
			{
				ProcessingScreen.instance.RefreshView();
			}
		}

		public override void RefreshView()
		{
			if (ProcessingScreen.loaded && ProcessingScreen.showing && !ProcessingScreen.added)
			{
				ProcessingScreen.added = true;
				Service.ScreenController.AddScreen(ProcessingScreen.instance);
			}
			else if (!ProcessingScreen.showing)
			{
				ProcessingScreen.loaded = false;
				ProcessingScreen.instance.Close(false);
				ProcessingScreen.instance = null;
				ProcessingScreen.added = false;
			}
		}
	}
}
