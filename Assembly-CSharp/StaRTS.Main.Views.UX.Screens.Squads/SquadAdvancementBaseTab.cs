using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Controls;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.Squads
{
	public class SquadAdvancementBaseTab : IEventObserver
	{
		private const string FILTER_STRING_ID_PREFIX = "PERK_FILTER_NAME_";

		private const string PERK_INFO_BTN = "BtnInfoPerks";

		protected const string PERK_GRID_ITEM_PREFIX = "PerkItem_";

		protected UXElement baseView;

		protected UXLabel tabLabel;

		protected UXGrid filterGrid;

		protected UXGrid gridToFilter;

		private string filterGridButtonName;

		private string filterGridLabelName;

		protected Dictionary<string, List<UXElement>> filterMap;

		protected Dictionary<string, JewelControl> perkBadgeMap;

		protected SquadSlidingScreen screen;

		protected string lastFilterId;

		private UXButton perkInfoBtn;

		protected bool openedModalOnTop;

		protected float lastGridPosition;

		private SquadScreenUpgradeCelebPerkInfoView celebPerkInfoView;

		public virtual bool Visible
		{
			get
			{
				return this.baseView != null && this.baseView.Visible;
			}
			set
			{
				if (this.baseView != null)
				{
					this.baseView.Visible = value;
				}
				if (value)
				{
					this.OnShow();
				}
				else
				{
					this.OnHide();
				}
			}
		}

		public SquadAdvancementBaseTab(SquadSlidingScreen screen, string baseViewName, string tabLabelName, string tabLabelString)
		{
			this.perkBadgeMap = new Dictionary<string, JewelControl>();
			this.filterMap = new Dictionary<string, List<UXElement>>();
			this.screen = screen;
			this.baseView = screen.GetElement<UXElement>(baseViewName);
			this.tabLabel = screen.GetElement<UXLabel>(tabLabelName);
			this.tabLabel.Text = tabLabelString;
			this.perkInfoBtn = screen.GetElement<UXButton>("BtnInfoPerks");
			this.perkInfoBtn.OnClicked = new UXButtonClickedDelegate(this.OnPerkInfoButtonClicked);
			Service.EventManager.RegisterObserver(this, EventId.SquadScreenOpenedOrClosed);
		}

		public bool ShouldBlockInput()
		{
			return this.openedModalOnTop;
		}

		public virtual bool ShouldBlockTabChanges()
		{
			return this.openedModalOnTop;
		}

		protected void InitFilterGrid(string filterGridName, string filterGridTemplateName, string filterGridButtonName, string filterGridLabelName, UXGrid gridToFilter)
		{
			this.gridToFilter = gridToFilter;
			this.filterGridButtonName = filterGridButtonName;
			this.filterGridLabelName = filterGridLabelName;
			this.filterGrid = this.screen.GetElement<UXGrid>(filterGridName);
			this.filterGrid.SetTemplateItem(filterGridTemplateName);
		}

		protected void RefreshFilterGrid()
		{
			if (this.filterGrid == null)
			{
				return;
			}
			this.filterGrid.Clear();
			this.filterMap.Clear();
			int count = this.gridToFilter.Count;
			List<UXElement> list = new List<UXElement>();
			for (int i = 0; i < count; i++)
			{
				UXElement item = this.gridToFilter.GetItem(i);
				PerkVO perkData = item.Tag as PerkVO;
				this.AddFilterData(perkData, item, ref list);
			}
			list.Sort(new Comparison<UXElement>(this.SortByFilterId));
			string b = this.FindAllFilterOption();
			if (this.lastFilterId == null)
			{
				this.lastFilterId = b;
			}
			int count2 = list.Count;
			for (int j = 0; j < count2; j++)
			{
				UXElement uXElement = list[j];
				string a = (string)uXElement.Tag;
				if (a == b)
				{
					this.filterGrid.AddItem(uXElement, 0);
				}
				else
				{
					this.filterGrid.AddItem(uXElement, j + 1);
				}
			}
			if (this.lastFilterId != null)
			{
				string itemUid = this.lastFilterId + this.baseView.Root.name;
				UXCheckbox subElement = this.filterGrid.GetSubElement<UXCheckbox>(itemUid, this.filterGridButtonName);
				subElement.Selected = true;
				this.FilterGridBasedOnId(this.lastFilterId);
			}
			this.filterGrid.RepositionItems();
		}

		private string FindAllFilterOption()
		{
			foreach (KeyValuePair<string, List<UXElement>> current in this.filterMap)
			{
				if (current.Value.Count == this.gridToFilter.Count)
				{
					return current.Key;
				}
			}
			return null;
		}

		private void AddFilterData(PerkVO perkData, UXElement perkItem, ref List<UXElement> sortList)
		{
			string[] filterTabs = perkData.FilterTabs;
			int num = filterTabs.Length;
			for (int i = 0; i < num; i++)
			{
				string text = filterTabs[i];
				if (!this.filterMap.ContainsKey(text))
				{
					this.filterMap.Add(text, new List<UXElement>());
					UXElement item = this.CreateFilterTab(text);
					sortList.Add(item);
				}
				this.filterMap[text].Add(perkItem);
			}
		}

		private UXElement CreateFilterTab(string filterId)
		{
			Lang lang = Service.Lang;
			string itemUid = filterId + this.baseView.Root.name;
			UXElement uXElement = this.filterGrid.CloneTemplateItem(itemUid);
			uXElement.Tag = filterId;
			UXCheckbox subElement = this.filterGrid.GetSubElement<UXCheckbox>(itemUid, this.filterGridButtonName);
			subElement.OnSelected = new UXCheckboxSelectedDelegate(this.OnFilterClicked);
			subElement.Tag = filterId;
			UXLabel subElement2 = this.filterGrid.GetSubElement<UXLabel>(itemUid, this.filterGridLabelName);
			string id = "PERK_FILTER_NAME_" + filterId.ToUpper();
			subElement2.Text = lang.Get(id, new object[0]);
			return uXElement;
		}

		private void OnFilterClicked(UXCheckbox btn, bool selected)
		{
			if (this.ShouldBlockInput())
			{
				btn.Selected = !selected;
				return;
			}
			if (selected)
			{
				this.lastGridPosition = 0f;
				string filterId = (string)btn.Tag;
				this.FilterGridBasedOnId(filterId);
				Service.EventManager.SendEvent(EventId.UIFilterSelected, null);
			}
		}

		protected virtual void FilterGridBasedOnId(string filterId)
		{
			this.lastFilterId = filterId;
			List<UXElement> list = this.filterMap[filterId];
			int count = this.gridToFilter.Count;
			for (int i = 0; i < count; i++)
			{
				UXElement item = this.gridToFilter.GetItem(i);
				if (list.Contains(item) && this.CanShowGridItem(item))
				{
					item.Visible = true;
				}
				else
				{
					item.Visible = false;
				}
			}
			this.gridToFilter.RepositionItems();
		}

		protected virtual bool CanShowGridItem(UXElement item)
		{
			return true;
		}

		private int SortByFilterId(UXElement a, UXElement b)
		{
			string strB = (string)a.Tag;
			string strA = (string)b.Tag;
			return string.Compare(strA, strB);
		}

		public virtual void RefreshPerkStates()
		{
			this.RefreshFilterGrid();
		}

		protected UXElement FetchPerkGridItem(UXGrid perkGrid, string itemUid)
		{
			UXElement uXElement = null;
			if (perkGrid != null)
			{
				int i = 0;
				int count = perkGrid.Count;
				while (i < count)
				{
					UXElement item = perkGrid.GetItem(i);
					if (item.Root.name.Contains(itemUid))
					{
						uXElement = item;
						uXElement.SetRootName(itemUid);
						break;
					}
					i++;
				}
			}
			if (uXElement == null)
			{
				uXElement = perkGrid.CloneTemplateItem(itemUid);
			}
			return uXElement;
		}

		protected virtual void OnShow()
		{
		}

		protected virtual void OnHide()
		{
		}

		protected void OnRepositionComplete(AbstractUXList list)
		{
			list.Scroll(this.lastGridPosition);
		}

		private void CleanUpPerkUpgradeCeleb()
		{
			this.openedModalOnTop = false;
			if (this.celebPerkInfoView != null)
			{
				this.celebPerkInfoView.HideAndCleanUp();
				this.celebPerkInfoView = null;
			}
		}

		public virtual void OnDestroyElement()
		{
			Service.EventManager.UnregisterObserver(this, EventId.SquadScreenOpenedOrClosed);
			if (this.filterGrid != null)
			{
				this.filterGrid.Clear();
				this.filterGrid = null;
			}
			if (this.filterMap != null)
			{
				this.filterMap.Clear();
				this.filterMap = null;
			}
			this.lastFilterId = null;
			this.CleanUpPerkUpgradeCeleb();
		}

		protected virtual void RegisterEvents()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.PerkUnlocked);
			eventManager.RegisterObserver(this, EventId.PerkUpgraded);
		}

		protected virtual void UnregisterEvents()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.PerkUnlocked);
			eventManager.UnregisterObserver(this, EventId.PerkUpgraded);
		}

		protected void SetupPerkBadge(PerkVO perkVO, string perkItemID, string jewelId)
		{
			PerkViewController perkViewController = Service.PerkViewController;
			PerkManager perkManager = Service.PerkManager;
			bool flag = true;
			Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
			int level = currentSquad.Level;
			Dictionary<string, string> available = currentSquad.Perks.Available;
			if (perkManager.IsPerkLevelLocked(perkVO, level) || perkManager.IsPerkReputationLocked(perkVO, level, available))
			{
				flag = false;
			}
			string perkGroup = perkVO.PerkGroup;
			JewelControl jewelControl;
			if (this.perkBadgeMap.ContainsKey(perkGroup))
			{
				jewelControl = this.perkBadgeMap[perkGroup];
			}
			else
			{
				string name = UXUtils.FormatAppendedName(jewelId, perkItemID);
				jewelControl = JewelControl.Create(this.screen, name);
				this.perkBadgeMap.Add(perkGroup, jewelControl);
			}
			if (jewelControl != null)
			{
				bool flag2 = perkViewController.IsPerkGroupBadged(perkVO.PerkGroup);
				if (flag2 && flag)
				{
					jewelControl.Text = "!";
				}
				else
				{
					jewelControl.Value = 0;
				}
			}
		}

		protected void ShowPerkCelebration(PerkVO perkData)
		{
			PerkManager perkManager = Service.PerkManager;
			if (!perkManager.HasPlayerSeenPerkTutorial())
			{
				return;
			}
			this.openedModalOnTop = true;
			this.celebPerkInfoView = new SquadScreenUpgradeCelebPerkInfoView(this.screen, perkData);
			this.celebPerkInfoView.Show();
		}

		private void OnPerkInfoButtonClicked(UXButton button)
		{
			if (this.ShouldBlockInput())
			{
				return;
			}
			SquadAdvancementInfoScreen squadAdvancementInfoScreen = new SquadAdvancementInfoScreen();
			Service.ScreenController.AddScreen(squadAdvancementInfoScreen);
		}

		public virtual EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.PerkUpgraded && id != EventId.PerkUnlocked)
			{
				if (id == EventId.SquadScreenOpenedOrClosed)
				{
					if (!(bool)cookie)
					{
						this.lastFilterId = null;
					}
				}
			}
			else
			{
				PerkViewController perkViewController = Service.PerkViewController;
				PerkVO perkVO = (PerkVO)cookie;
				string perkGroup = perkVO.PerkGroup;
				if (perkViewController.IsPerkGroupBadged(perkGroup))
				{
					this.ShowPerkCelebration(perkVO);
					perkViewController.RemovePerkGroupFromBadgeList(perkGroup);
					this.screen.UpdateBadges();
				}
			}
			return EatResponse.NotEaten;
		}
	}
}
