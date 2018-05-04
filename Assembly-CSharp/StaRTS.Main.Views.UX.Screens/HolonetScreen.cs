using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.Holonet;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens.ScreenHelpers.Holonet;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens
{
	public class HolonetScreen : ClosableScreen, IEventObserver
	{
		private AbstractHolonetTab commandCenterTab;

		private AbstractHolonetTab devNotesTab;

		private AbstractHolonetTab transmissionsTab;

		private TransmissionsHolonetPopupView incommingTransmissions;

		private HolonetAnimationController anims;

		private int commandCenterCount;

		private int devNotesCount;

		private int transmissionsCount;

		private UXElement incomingTransmissionsGroup;

		private List<AbstractHolonetTab> tabs;

		private AbstractHolonetTab currentTab;

		private EventManager eventManager;

		public bool AutoDisplayMCACrate;

		public UXTable NavTable
		{
			get;
			private set;
		}

		public AbstractHolonetTab PreviousTab
		{
			get;
			private set;
		}

		protected override bool WantTransitions
		{
			get
			{
				return base.IsClosing;
			}
		}

		public HolonetScreen(int commandCenterCount, int devNotesCount, int transmissionsCount) : base("gui_holonet")
		{
			this.allowClose = false;
			this.AutoDisplayMCACrate = false;
			this.commandCenterCount = commandCenterCount;
			this.devNotesCount = devNotesCount;
			this.transmissionsCount = transmissionsCount;
		}

		protected override void OnScreenLoaded()
		{
			this.anims = new HolonetAnimationController(this);
			this.eventManager = Service.EventManager;
			this.InitButtons();
			this.tabs = new List<AbstractHolonetTab>();
			this.incomingTransmissionsGroup = base.GetElement<UXElement>("IncomingTransmissionsGroup");
			this.incomingTransmissionsGroup.Visible = false;
			this.NavTable = base.GetElement<UXTable>("NavTable");
			this.NavTable.SetTemplateItem("NavItem");
			this.commandCenterTab = new CommandCenterHolonetTab(this, HolonetControllerType.CommandCenter);
			this.tabs.Add(this.commandCenterTab);
			this.devNotesTab = new DevNotesHolonetTab(this, HolonetControllerType.DevNotes);
			this.tabs.Add(this.devNotesTab);
			this.transmissionsTab = new TransmissionsHolonetTab(this, HolonetControllerType.Transmissions);
			this.tabs.Add(this.transmissionsTab);
			this.commandCenterTab.SetBadgeCount(this.commandCenterCount);
			this.devNotesTab.SetBadgeCount(this.devNotesCount);
			this.transmissionsTab.SetBadgeCount(this.transmissionsCount);
			Service.EventManager.RegisterObserver(this, EventId.GameStateChanged, EventPriority.Default);
			HolonetController holonetController = Service.HolonetController;
			int inCommingTransmissionCount = holonetController.TransmissionsController.GetInCommingTransmissionCount();
			bool flag = inCommingTransmissionCount > 0;
			if (!holonetController.CommandCenterController.HasNewPromoEntry() && (flag || holonetController.HasNewBattles()))
			{
				holonetController.SetLastViewed(this.transmissionsTab);
				this.incommingTransmissions = new TransmissionsHolonetPopupView(this);
				this.anims.OpenToIncomingTransmission();
			}
			else
			{
				this.ShowDefaultTabs();
				this.anims.OpenToCommandCenter();
			}
			this.eventManager.SendEvent(EventId.HolonetOpened, null);
			this.allowClose = true;
		}

		public void EnableAllTabButtons()
		{
			if (this.tabs != null)
			{
				int count = this.tabs.Count;
				for (int i = 0; i < count; i++)
				{
					this.tabs[i].EnableTabButton();
				}
			}
		}

		public void DisableAllTabButtons()
		{
			if (this.tabs != null)
			{
				int count = this.tabs.Count;
				for (int i = 0; i < count; i++)
				{
					this.tabs[i].DisableTabButton();
				}
			}
		}

		public void ShowDefaultTabs()
		{
			this.OpenTab(HolonetControllerType.CommandCenter);
		}

		public override void OnDestroyElement()
		{
			Service.EventManager.UnregisterObserver(this, EventId.GameStateChanged);
			int i = 0;
			int count = this.tabs.Count;
			while (i < count)
			{
				this.tabs[i].OnDestroyTab();
				i++;
			}
			this.tabs.Clear();
			this.tabs = null;
			if (this.incommingTransmissions != null)
			{
				this.incommingTransmissions.CleanUp();
			}
			if (this.NavTable != null)
			{
				this.NavTable.Clear();
				this.NavTable = null;
			}
			this.anims.Cleanup();
			this.anims = null;
			base.OnDestroyElement();
			this.Visible = false;
		}

		public void HideAllTabs()
		{
			int i = 0;
			int count = this.tabs.Count;
			while (i < count)
			{
				this.tabs[i].OnTabClose();
				i++;
			}
		}

		public void OpenTab(HolonetControllerType tabType)
		{
			if (this.tabs == null)
			{
				return;
			}
			if (this.currentTab != null && this.currentTab.HolonetControllerType != tabType)
			{
				this.PreviousTab = this.currentTab;
			}
			int i = 0;
			int count = this.tabs.Count;
			while (i < count)
			{
				if (this.currentTab == null || (this.tabs[i].HolonetControllerType == tabType && this.tabs[i].HolonetControllerType != this.currentTab.HolonetControllerType))
				{
					this.currentTab = this.tabs[i];
					this.currentTab.OnTabOpen();
					if (this.PreviousTab != null && this.PreviousTab != this.currentTab)
					{
						this.eventManager.SendEvent(EventId.HolonetTabClosed, this.PreviousTab.GetBITabName());
					}
					this.eventManager.SendEvent(EventId.HolonetTabOpened, this.currentTab.GetBITabName());
				}
				i++;
			}
			if (this.PreviousTab != null && this.PreviousTab != this.currentTab)
			{
				this.PreviousTab.OnTabClose();
			}
			if (this.currentTab == this.commandCenterTab)
			{
				this.anims.ShowCommandCenter();
			}
			else if (this.currentTab == this.devNotesTab)
			{
				this.anims.ShowDevNotes();
			}
			else if (this.currentTab == this.transmissionsTab)
			{
				this.anims.ShowTransmissionLog();
			}
		}

		public override EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.GameStateChanged)
			{
				IState currentState = Service.GameStateMachine.CurrentState;
				if (!(currentState is HomeState))
				{
					this.Close(null);
				}
			}
			return base.OnEvent(id, cookie);
		}

		public override void Close(object modalResult)
		{
			base.Close(modalResult);
			string cookie = string.Empty;
			HolonetController holonetController = Service.HolonetController;
			if (holonetController.TransmissionsController.IsTransmissionPopupOpened() && this.incommingTransmissions != null)
			{
				cookie = this.incommingTransmissions.GetBITabName();
			}
			else if (this.currentTab != null)
			{
				cookie = this.currentTab.GetBITabName();
			}
			string cookie2 = (this.currentTab == null) ? string.Empty : this.currentTab.GetBITabName();
			this.eventManager.SendEvent(EventId.HolonetTabClosed, cookie2);
			this.eventManager.SendEvent(EventId.HolonetClosed, cookie);
		}
	}
}
