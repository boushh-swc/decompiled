using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens;
using System;

namespace StaRTS.Main.Controllers
{
	public class ScreenInfo
	{
		public UXElement Screen
		{
			get;
			set;
		}

		public int Depth
		{
			get;
			set;
		}

		public int ScreenPanelThickness
		{
			get;
			set;
		}

		public bool IsModal
		{
			get;
			private set;
		}

		public bool VisibleScrim
		{
			get;
			private set;
		}

		public bool WasVisible
		{
			get;
			set;
		}

		public QueueScreenBehavior QueueBehavior
		{
			get;
			private set;
		}

		public ScreenInfo(UXElement screen, bool modal) : this(screen, modal, true, QueueScreenBehavior.Default)
		{
		}

		public ScreenInfo(UXElement screen, bool modal, bool visibleScrim, QueueScreenBehavior subType)
		{
			this.Screen = screen;
			this.Depth = 0;
			this.ScreenPanelThickness = 0;
			this.IsModal = modal;
			this.VisibleScrim = visibleScrim;
			this.QueueBehavior = subType;
			this.WasVisible = true;
		}

		public void OnEnqueued()
		{
			if (this.QueueBehavior == QueueScreenBehavior.DeferTillClosed)
			{
				this.QueueBehavior = QueueScreenBehavior.QueueAndDeferTillClosed;
			}
			else if (this.QueueBehavior == QueueScreenBehavior.Default)
			{
				this.QueueBehavior = QueueScreenBehavior.Queue;
			}
		}

		public void OnDequeued()
		{
			if (this.QueueBehavior == QueueScreenBehavior.QueueAndDeferTillClosed)
			{
				this.QueueBehavior = QueueScreenBehavior.DeferTillClosed;
			}
			else if (this.QueueBehavior == QueueScreenBehavior.Queue)
			{
				this.QueueBehavior = QueueScreenBehavior.Default;
			}
		}

		public bool ShouldDefer()
		{
			return this.QueueBehavior == QueueScreenBehavior.DeferTillClosed || this.QueueBehavior == QueueScreenBehavior.QueueAndDeferTillClosed;
		}

		public bool HasQueueBehavior()
		{
			return this.QueueBehavior == QueueScreenBehavior.Queue || this.QueueBehavior == QueueScreenBehavior.QueueAndDeferTillClosed;
		}

		public bool CanShowMoreScreens()
		{
			return this.QueueBehavior != QueueScreenBehavior.DeferTillClosed && this.QueueBehavior != QueueScreenBehavior.QueueAndDeferTillClosed;
		}

		public bool IsPersistentAndOpen()
		{
			bool result = false;
			if (this.Screen is PersistentAnimatedScreen)
			{
				PersistentAnimatedScreen persistentAnimatedScreen = (PersistentAnimatedScreen)this.Screen;
				if (persistentAnimatedScreen.IsOpen() || persistentAnimatedScreen.IsOpening())
				{
					result = true;
				}
			}
			return result;
		}
	}
}
