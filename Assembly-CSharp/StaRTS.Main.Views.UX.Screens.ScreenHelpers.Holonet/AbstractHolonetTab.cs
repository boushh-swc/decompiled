using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.Holonet;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers.Holonet
{
	public class AbstractHolonetTab
	{
		public delegate void TimestampsHandler(uint[] timestamps);

		private const float HOLONET_TAB_ANIMATION_DURATION = 1.5f;

		protected HolonetScreen screen;

		protected EventManager eventManager;

		protected Lang lang;

		protected UXElement tabItem;

		protected UXCheckbox tabButton;

		protected UXElement badgeGroup;

		protected UXLabel badgeLabel;

		protected UXElement topLevelGroup;

		protected UXSprite highlight;

		protected UXLabel label;

		private List<UXTexture> deferredTextures;

		public HolonetControllerType HolonetControllerType;

		public AbstractHolonetTab(HolonetScreen screen, HolonetControllerType holonetControllerType)
		{
			this.screen = screen;
			this.HolonetControllerType = holonetControllerType;
			this.lang = Service.Lang;
			this.eventManager = Service.EventManager;
			this.deferredTextures = new List<UXTexture>();
		}

		protected void InitializeTab(string topLevelGroupName, string tabLabelId)
		{
			this.topLevelGroup = this.screen.GetElement<UXElement>(topLevelGroupName);
			this.tabItem = this.screen.NavTable.CloneTemplateItem(topLevelGroupName);
			this.tabButton = this.screen.NavTable.GetSubElement<UXCheckbox>(topLevelGroupName, "NavTapProperties");
			this.highlight = this.screen.NavTable.GetSubElement<UXSprite>(topLevelGroupName, "SpriteTabHighlight");
			this.badgeGroup = this.screen.NavTable.GetSubElement<UXElement>(topLevelGroupName, "BadgeGroup");
			this.badgeLabel = this.screen.NavTable.GetSubElement<UXLabel>(topLevelGroupName, "BadgeLabel");
			this.label = this.screen.NavTable.GetSubElement<UXLabel>(topLevelGroupName, "NavItemLabel");
			string text = Service.Lang.Get(tabLabelId, new object[0]);
			this.label.Text = text;
			this.AddSelectionButtonToNavTable();
			this.tabButton.OnSelected = new UXCheckboxSelectedDelegate(this.SetVisibleByTabButton);
			Service.ViewTimerManager.CreateViewTimer(1.5f, false, new TimerDelegate(this.OnAnimationTimerComplete), null);
		}

		public void OnAnimationTimerComplete(uint id, object cookie)
		{
			this.LoadDeferredTextures();
		}

		protected void DeferTexture(UXTexture texture, string assetName)
		{
			texture.DeferTextureForLoad(assetName);
			this.deferredTextures.Add(texture);
		}

		private void LoadDeferredTextures()
		{
			int i = 0;
			int count = this.deferredTextures.Count;
			while (i < count)
			{
				this.deferredTextures[i].LoadDeferred();
				i++;
			}
			this.deferredTextures.Clear();
		}

		protected virtual void AddSelectionButtonToNavTable()
		{
			this.screen.NavTable.AddItem(this.tabItem, this.screen.NavTable.Count);
		}

		public void EnableTabButton()
		{
			this.tabButton.SetSelectable(true);
		}

		public void DisableTabButton()
		{
			this.tabButton.SetSelectable(false);
		}

		public void SetBadgeCount(int count)
		{
			this.badgeGroup.Visible = (count > 0);
			this.badgeLabel.Text = count.ToString();
		}

		protected virtual void SetVisibleByTabButton(UXCheckbox button, bool selected)
		{
			if (selected)
			{
				Service.EventManager.SendEvent(EventId.HolonetChangeTabs, null);
				this.screen.OpenTab(this.HolonetControllerType);
			}
		}

		public virtual void OnTabOpen()
		{
			this.tabButton.Selected = true;
			this.tabButton.PlayTween(true);
			this.highlight.PlayTween(true);
			Service.HolonetController.SetLastViewed(this);
		}

		public virtual void OnTabClose()
		{
			this.tabButton.Selected = false;
			this.tabButton.PlayTween(false);
			this.highlight.PlayTween(false);
		}

		protected void PrepareButton(ICallToAction vo, int btnIndex, UXButton button, UXLabel btnLabel)
		{
			if (button == null || btnLabel == null)
			{
				return;
			}
			string text = (btnIndex != 1) ? vo.Btn2 : vo.Btn1;
			if (string.IsNullOrEmpty(text))
			{
				button.Visible = false;
			}
			else
			{
				button.Visible = true;
				if (btnIndex == 1)
				{
					button.OnClicked = new UXButtonClickedDelegate(this.FeaturedButton1Clicked);
				}
				else
				{
					button.OnClicked = new UXButtonClickedDelegate(this.FeaturedButton2Clicked);
				}
				button.Tag = vo;
				btnLabel.Text = this.lang.Get(text, new object[0]);
				if (this.IsButtonRewardAction(vo, btnIndex))
				{
					StaticDataController staticDataController = Service.StaticDataController;
					LimitedTimeRewardVO vo2;
					if (btnIndex == 1)
					{
						vo2 = staticDataController.Get<LimitedTimeRewardVO>(vo.Btn1Data);
					}
					else
					{
						vo2 = staticDataController.Get<LimitedTimeRewardVO>(vo.Btn2Data);
					}
					this.SetupRewardButton(ref button, ref btnLabel, vo2);
				}
			}
		}

		protected bool HasRewardButton1BeenClaimed(LimitedTimeRewardVO vo)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			return currentPlayer.HolonetRewards.Contains(vo.Uid);
		}

		protected void SetupRewardButton(ref UXButton btn, ref UXLabel btnLabel, LimitedTimeRewardVO vo)
		{
			if (this.HasRewardButton1BeenClaimed(vo))
			{
				btnLabel.Text = this.lang.Get("hn_reward_been_claimed", new object[0]);
				btn.Enabled = false;
			}
		}

		protected void SendCallToActionBI(string action, string uid, EventId id)
		{
			string cookie = action + "|" + uid + "|cta_button";
			this.eventManager.SendEvent(id, cookie);
		}

		protected virtual void FeaturedButton1Clicked(UXButton button)
		{
		}

		protected virtual void FeaturedButton2Clicked(UXButton button)
		{
		}

		private bool IsButtonRewardAction(ICallToAction vo, int index)
		{
			if (index == 1)
			{
				return vo.Btn1Action == "reward";
			}
			return vo.Btn2Action == "reward";
		}

		public virtual void OnDestroyTab()
		{
		}

		public virtual string GetBITabName()
		{
			return string.Empty;
		}
	}
}
