using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens
{
	public class SquadWarInfoScreen : ClosableScreen
	{
		public const int PAGE_DEFAULT = -1;

		public const int PAGE_OVERVIEW = 0;

		public const int PAGE_WARBOARD = 1;

		public const int PAGE_PREPARATION = 2;

		public const int PAGE_WAR = 3;

		public const int PAGE_REWARD = 4;

		private const string GROUP_OVERVIEW = "Overview";

		private const string GROUP_WARBOARD = "WarBoard";

		private const string GROUP_PREPARATION = "Preparation";

		private const string GROUP_WAR = "War";

		private const string GROUP_REWARD = "Reward";

		private const string LABEL_TITLE = "LabelTitle";

		private const string LABEL_OVERVIEW = "LabelBtnFilterOverview";

		private const string LABEL_WARBOARD = "LabelBtnFilterWarBoard";

		private const string LABEL_PREPARATION = "LabelBtnFilterPreparation";

		private const string LABEL_WAR = "LabelBtnFilterWar";

		private const string LABEL_REWARD = "LabelBtnFilterReward";

		private const string BUTTON_OVERVIEW = "BtnFilterOverview";

		private const string BUTTON_WARBOARD = "BtnFilterWarBoard";

		private const string BUTTON_PREPARATION = "BtnFilterPreparation";

		private const string BUTTON_WAR = "BtnFilterWar";

		private const string BUTTON_REWARD = "BtnFilterReward";

		private const string BUTTON_NEXT = "BtnNext";

		private const string BUTTON_PREV = "BtnPrevious";

		private const string WAR_INFO_TITLE = "WAR_INFO_TITLE";

		private const string WAR_INFO_TAB_OVERVIEW = "WAR_INFO_TAB_OVERVIEW";

		private const string WAR_INFO_TAB_WARBOARD = "WAR_INFO_TAB_WARBOARD";

		private const string WAR_INFO_TAB_PREPARATION = "WAR_INFO_TAB_PREPARATION";

		private const string WAR_INFO_TAB_WAR = "WAR_INFO_TAB_WAR";

		private const string WAR_INFO_TAB_REWARD = "WAR_INFO_TAB_REWARD";

		private List<AbstractSquadWarInfoScreenTab> tabs;

		private int startingIndex;

		private int currentIndex = -1;

		private UXButton prevButton;

		private UXButton nextButton;

		protected override bool WantTransitions
		{
			get
			{
				return false;
			}
		}

		public SquadWarInfoScreen(int pageIndex) : base("gui_squadwar_info")
		{
			this.startingIndex = pageIndex;
			base.IsAlwaysOnTop = true;
		}

		protected override void OnScreenLoaded()
		{
			this.InitButtons();
			UXLabel element = base.GetElement<UXLabel>("LabelTitle");
			element.Text = this.lang.Get("WAR_INFO_TITLE", new object[0]);
			UXLabel element2 = base.GetElement<UXLabel>("LabelBtnFilterOverview");
			element2.Text = this.lang.Get("WAR_INFO_TAB_OVERVIEW", new object[0]);
			UXLabel element3 = base.GetElement<UXLabel>("LabelBtnFilterWarBoard");
			element3.Text = this.lang.Get("WAR_INFO_TAB_WARBOARD", new object[0]);
			UXLabel element4 = base.GetElement<UXLabel>("LabelBtnFilterPreparation");
			element4.Text = this.lang.Get("WAR_INFO_TAB_PREPARATION", new object[0]);
			UXLabel element5 = base.GetElement<UXLabel>("LabelBtnFilterWar");
			element5.Text = this.lang.Get("WAR_INFO_TAB_WAR", new object[0]);
			UXLabel element6 = base.GetElement<UXLabel>("LabelBtnFilterReward");
			element6.Text = this.lang.Get("WAR_INFO_TAB_REWARD", new object[0]);
			this.tabs = new List<AbstractSquadWarInfoScreenTab>();
			this.tabs.Add(new SquadWarInfoScreenOverviewTab(this, base.GetElement<UXCheckbox>("BtnFilterOverview"), base.GetElement<UXElement>("Overview")));
			this.tabs.Add(new SquadWarInfoScreenWarBoardTab(this, base.GetElement<UXCheckbox>("BtnFilterWarBoard"), base.GetElement<UXElement>("WarBoard")));
			this.tabs.Add(new SquadWarInfoScreenPreparationTab(this, base.GetElement<UXCheckbox>("BtnFilterPreparation"), base.GetElement<UXElement>("Preparation")));
			this.tabs.Add(new SquadWarInfoScreenWarTab(this, base.GetElement<UXCheckbox>("BtnFilterWar"), base.GetElement<UXElement>("War")));
			this.tabs.Add(new SquadWarInfoScreenRewardTab(this, base.GetElement<UXCheckbox>("BtnFilterReward"), base.GetElement<UXElement>("Reward")));
			int i = 0;
			int count = this.tabs.Count;
			while (i < count)
			{
				this.tabs[i].ShowContents(false);
				i++;
			}
			this.ShowPage(this.startingIndex);
		}

		protected override void InitButtons()
		{
			this.prevButton = base.GetElement<UXButton>("BtnPrevious");
			this.prevButton.OnClicked = new UXButtonClickedDelegate(this.OnPrevButtonClicked);
			this.nextButton = base.GetElement<UXButton>("BtnNext");
			this.nextButton.OnClicked = new UXButtonClickedDelegate(this.OnNextButtonClicked);
			base.InitButtons();
		}

		private void OnPrevButtonClicked(UXButton button)
		{
			this.ShowPage(Math.Max(0, this.currentIndex - 1));
		}

		private void OnNextButtonClicked(UXButton button)
		{
			this.ShowPage(Math.Min(this.tabs.Count - 1, this.currentIndex + 1));
		}

		public void ShowPage(int index)
		{
			if (index == -1)
			{
				index = this.DetermineDefaultPage();
			}
			this.ShowTab(this.tabs[index]);
		}

		private void ShowTab(AbstractSquadWarInfoScreenTab tab)
		{
			tab.TabButton.DelayedSelect(true);
		}

		public void OnTabShown(AbstractSquadWarInfoScreenTab tab)
		{
			int num = this.tabs.IndexOf(tab);
			if (num == -1)
			{
				Service.Logger.ErrorFormat("Cannot find tab in tabs list.", new object[0]);
				return;
			}
			this.UpdateIndex(num);
		}

		private void UpdateIndex(int index)
		{
			this.prevButton.Visible = (index > 0);
			this.nextButton.Visible = (index < this.tabs.Count - 1);
			this.currentIndex = index;
		}

		private int DetermineDefaultPage()
		{
			if (Service.GameStateMachine.CurrentState is WarBaseEditorState)
			{
				return 1;
			}
			switch (Service.SquadController.WarManager.GetCurrentStatus())
			{
			case SquadWarStatusType.PhasePrep:
			case SquadWarStatusType.PhasePrepGrace:
				return 2;
			case SquadWarStatusType.PhaseAction:
			case SquadWarStatusType.PhaseActionGrace:
				return 3;
			case SquadWarStatusType.PhaseCooldown:
				return 4;
			default:
				return 0;
			}
		}

		protected override void HandleClose(UXButton button)
		{
			if (Service.GameStateMachine.CurrentState is WarBaseEditorState)
			{
				Service.UXController.MiscElementsManager.AddSquadWarTickerStatus();
			}
			base.HandleClose(button);
		}
	}
}
