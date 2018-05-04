using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Squads;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.Squads
{
	public class SquadScreenChatView : AbstractSquadScreenViewModule
	{
		private const string CHAT_CONTAINER = "ChatContainer";

		private const string TAB_BUTTON_NAME = "SocialChatBtn";

		private const string CHAT_TABLE = "ChatTable";

		public const string CHAT = "chat";

		private const int NUM_CREATE_PER_FRAME = 10;

		private const int NUM_DESTROY_PER_FRAME = 10;

		private const float FRAME_DELAY_TIME = 0.01f;

		private const float DESTROY_DELAY_TIME = 0.5f;

		private UXElement chatContainer;

		private UXCheckbox tabButton;

		private SquadMsgChatDisplay chatDisplay;

		private SquadScreenChatFilterView chatFilter;

		private SquadScreenChatInputView chatInput;

		private SquadScreenChatTroopDonationProgressView donationProgress;

		private int numExistingMsgsProcessed;

		private HashSet<SquadMsg> existingMsgsProcessed;

		private uint timestampUpdateTimerId;

		private const float CHAT_TIMESTAP_UPDATE_INTERVAL = 60f;

		public bool ChatDisplaySetup
		{
			get;
			private set;
		}

		public SquadScreenChatView(SquadSlidingScreen screen) : base(screen)
		{
			this.timestampUpdateTimerId = 0u;
			this.chatFilter = new SquadScreenChatFilterView(screen);
			this.chatInput = new SquadScreenChatInputView(screen);
			this.donationProgress = new SquadScreenChatTroopDonationProgressView(screen);
			this.ChatDisplaySetup = false;
			this.numExistingMsgsProcessed = 0;
			this.existingMsgsProcessed = null;
		}

		public override void OnScreenLoaded()
		{
			this.chatContainer = this.screen.GetElement<UXElement>("ChatContainer");
			this.tabButton = this.screen.GetElement<UXCheckbox>("SocialChatBtn");
			this.tabButton.OnSelected = new UXCheckboxSelectedDelegate(this.OnTabButtonSelected);
			this.chatFilter.OnScreenLoaded();
			this.chatInput.OnScreenLoaded();
			this.donationProgress.OnScreenLoaded();
			this.chatDisplay = new SquadMsgChatDisplay(this.screen, this.screen.GetElement<UXTable>("ChatTable"));
			Service.SquadController.MsgManager.RegisterObserver(this.chatDisplay);
		}

		public override void ShowView()
		{
			this.chatContainer.Visible = true;
			Service.EventManager.SendEvent(EventId.SquadSelect, null);
			Service.EventManager.SendEvent(EventId.UISquadScreenTabShown, "chat");
			this.timestampUpdateTimerId = Service.ViewTimerManager.CreateViewTimer(60f, true, new TimerDelegate(this.OnTimestampUpdateTimer), null);
			this.chatInput.ShowView();
			this.chatFilter.OnChatViewOpened();
			this.donationProgress.ShowView();
			this.tabButton.Selected = true;
		}

		public void SetupChatDisplay()
		{
			if (!this.ChatDisplaySetup)
			{
				this.ChatDisplaySetup = true;
				ProcessingScreen.Show();
				this.numExistingMsgsProcessed = 0;
				this.existingMsgsProcessed = new HashSet<SquadMsg>();
				this.SetupChatDisplayFrameDelayed();
			}
		}

		public override void HideView()
		{
			this.chatContainer.Visible = false;
			if (this.timestampUpdateTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.timestampUpdateTimerId);
				this.timestampUpdateTimerId = 0u;
			}
			this.chatFilter.HideView();
			this.chatInput.HideView();
			this.donationProgress.HideView();
			this.tabButton.Selected = false;
		}

		public override void RefreshView()
		{
		}

		private void OnTimestampUpdateTimer(uint id, object cookie)
		{
			this.chatDisplay.UpdateAllTimestamps();
		}

		private void OnTabButtonSelected(UXCheckbox checkbox, bool selected)
		{
			if (selected)
			{
				SquadController squadController = Service.SquadController;
				squadController.StateManager.SquadScreenState = SquadScreenState.Chat;
				this.screen.RefreshViews();
			}
		}

		public override void OnDestroyElement()
		{
			this.chatFilter.OnDestroyElement();
			this.chatInput.OnDestroyElement();
			this.donationProgress.OnDestroyElement();
			Service.SquadController.MsgManager.UnregisterObserver(this.chatDisplay);
			if (this.timestampUpdateTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.timestampUpdateTimerId);
				this.timestampUpdateTimerId = 0u;
			}
			this.chatDisplay.Cleanup();
			if (this.ChatDisplaySetup)
			{
				this.DestroyChatDisplayFrameDelayed();
				this.ChatDisplaySetup = false;
			}
			if (this.existingMsgsProcessed != null)
			{
				this.existingMsgsProcessed.Clear();
				this.existingMsgsProcessed = null;
			}
			this.numExistingMsgsProcessed = 0;
		}

		public int UpdateBadges()
		{
			return this.chatFilter.UpdateBadges();
		}

		public override bool IsVisible()
		{
			return this.chatContainer.Visible;
		}

		private void SetupChatDisplayFrameDelayed()
		{
			this.KillExistingTimers();
			uint item = Service.ViewTimerManager.CreateViewTimer(0.01f, true, new TimerDelegate(this.OnSetupChatDisplayTimer), null);
			Service.SquadController.StateManager.SquadScreenTimers.Add(item);
		}

		private void OnSetupChatDisplayTimer(uint timerId, object cookie)
		{
			if (this.chatDisplay != null)
			{
				List<SquadMsg> existingMessages = Service.SquadController.MsgManager.GetExistingMessages();
				int count = existingMessages.Count;
				int num = count - this.numExistingMsgsProcessed;
				if (num > 10)
				{
					num = 10;
				}
				if (num > 0)
				{
					int i = this.numExistingMsgsProcessed;
					int num2 = this.numExistingMsgsProcessed + num;
					while (i < num2)
					{
						SquadMsg item = existingMessages[i];
						if (!this.existingMsgsProcessed.Contains(item))
						{
							this.numExistingMsgsProcessed++;
							this.chatDisplay.ProcessMessage(existingMessages[i], false);
							this.existingMsgsProcessed.Add(existingMessages[i]);
						}
						i++;
					}
				}
				else
				{
					this.KillExistingTimers();
					this.chatDisplay.OnExistingMessagesSetup();
					ProcessingScreen.Hide();
				}
			}
		}

		private void DestroyChatDisplayFrameDelayed()
		{
			this.KillExistingTimers();
			uint item = Service.ViewTimerManager.CreateViewTimer(0.5f, false, new TimerDelegate(this.OnStartChatDisplayDestroyTimer), null);
			Service.SquadController.StateManager.SquadScreenTimers.Add(item);
		}

		private void OnStartChatDisplayDestroyTimer(uint timerId, object cookie)
		{
			uint item = Service.ViewTimerManager.CreateViewTimer(0.01f, true, new TimerDelegate(this.OnDestroyChatDisplayTimer), null);
			Service.SquadController.StateManager.SquadScreenTimers.Remove(timerId);
			Service.SquadController.StateManager.SquadScreenTimers.Add(item);
		}

		private void OnDestroyChatDisplayTimer(uint timerId, object cookie)
		{
			if (this.chatDisplay != null && this.chatDisplay.RemoveElementsByCount(10) == 0)
			{
				this.KillExistingTimers();
				this.chatDisplay.Destroy();
				this.chatDisplay = null;
			}
		}

		private void KillExistingTimers()
		{
			ViewTimerManager viewTimerManager = Service.ViewTimerManager;
			List<uint> squadScreenTimers = Service.SquadController.StateManager.SquadScreenTimers;
			int i = 0;
			int count = squadScreenTimers.Count;
			while (i < count)
			{
				viewTimerManager.KillViewTimer(squadScreenTimers[i]);
				i++;
			}
			squadScreenTimers.Clear();
		}
	}
}
