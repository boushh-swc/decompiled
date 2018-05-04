using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers
{
	public abstract class AbstractTabHelper
	{
		private string filterPanel;

		private string filterGrid;

		private string filterTemplate;

		private string filterButton;

		private string filterButtonLabel;

		private UXGrid tabGrid;

		private Action tabChangedCallback;

		private Dictionary<int, UXLabel> tabLabels;

		private List<UXCheckbox> checkboxes;

		public int CurrentTab
		{
			get;
			private set;
		}

		protected AbstractTabHelper(string panel, string grid, string template, string button, string buttonLabel)
		{
			this.filterPanel = panel;
			this.filterGrid = grid;
			this.filterTemplate = template;
			this.filterButton = button;
			this.filterButtonLabel = buttonLabel;
		}

		public void CreateTabs(ScreenBase screen, Action tabChangedCallback, Dictionary<int, string> tabInfo, int currentTab)
		{
			this.tabChangedCallback = tabChangedCallback;
			this.CurrentTab = currentTab;
			this.tabLabels = new Dictionary<int, UXLabel>();
			this.checkboxes = new List<UXCheckbox>();
			UXElement element = screen.GetElement<UXElement>(this.filterPanel);
			element.Visible = true;
			this.tabGrid = screen.GetElement<UXGrid>(this.filterGrid);
			this.tabGrid.SetTemplateItem(this.filterTemplate);
			int num = 1;
			foreach (KeyValuePair<int, string> current in tabInfo)
			{
				this.CreateTab(current.Value, current.Key, num++);
			}
			if (this.checkboxes.Count > 0)
			{
				this.checkboxes[0].Selected = true;
			}
			this.tabGrid.RepositionItemsFrameDelayed();
		}

		private void CreateTab(string tabTitle, int tab, int sortOrder)
		{
			string itemUid = "tab_" + sortOrder;
			UXElement item = this.tabGrid.CloneTemplateItem(itemUid);
			UXCheckbox subElement = this.tabGrid.GetSubElement<UXCheckbox>(itemUid, this.filterButton);
			subElement.Tag = tab;
			subElement.OnSelected = new UXCheckboxSelectedDelegate(this.OnTabSelected);
			this.checkboxes.Add(subElement);
			UXLabel subElement2 = this.tabGrid.GetSubElement<UXLabel>(itemUid, this.filterButtonLabel);
			subElement2.Text = tabTitle;
			if (this.CurrentTab != tab)
			{
				subElement2.TextColor = UXUtils.COLOR_INACTIVE_TROOP_TAB;
			}
			this.tabLabels.Add(tab, subElement2);
			this.tabGrid.AddItem(item, sortOrder);
		}

		private void OnTabSelected(UXCheckbox checkbox, bool selected)
		{
			UXLabel uXLabel = this.tabLabels[this.CurrentTab];
			uXLabel.TextColor = UXUtils.COLOR_INACTIVE_TROOP_TAB;
			this.CurrentTab = (int)checkbox.Tag;
			UXLabel uXLabel2 = this.tabLabels[this.CurrentTab];
			if (selected)
			{
				uXLabel2.TextColor = Color.white;
				if (this.tabChangedCallback != null)
				{
					this.tabChangedCallback();
				}
				Service.EventManager.SendEvent(EventId.UIFilterSelected, null);
			}
		}

		public int GetFilterCount()
		{
			return this.tabGrid.Count;
		}

		public void Destroy()
		{
			if (this.tabGrid != null)
			{
				this.tabGrid.Clear();
			}
			if (this.tabLabels != null)
			{
				this.tabLabels.Clear();
			}
			if (this.checkboxes != null)
			{
				this.checkboxes.Clear();
			}
			this.tabChangedCallback = null;
			this.tabLabels = null;
			this.checkboxes = null;
		}

		public void HideTabs(ScreenBase screen)
		{
			UXElement element = screen.GetElement<UXElement>(this.filterPanel);
			element.Visible = false;
		}

		public void ShowTabs(ScreenBase screen)
		{
			UXElement element = screen.GetElement<UXElement>(this.filterPanel);
			element.Visible = true;
		}

		public void SetSelectable(bool selectable)
		{
			if (this.checkboxes != null)
			{
				int i = 0;
				int count = this.checkboxes.Count;
				while (i < count)
				{
					this.checkboxes[i].SetSelectable(selectable);
					i++;
				}
			}
		}
	}
}
